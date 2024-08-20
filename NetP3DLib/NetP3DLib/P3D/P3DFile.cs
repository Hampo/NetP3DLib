using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D;

public class P3DFile
{
    public const uint SIGNATURE = 0xFF443350;
    public const uint SIGNATURE_SWAP = 0x503344FF;
    public const uint COMPRESSED_SIGNATURE = 0x5A443350;
    public const uint COMPRESSED_SIGNATURE_SWAP = 0x5033445A;
    public const uint HEADER_SIZE = sizeof(uint) + sizeof(uint) + sizeof(uint); // ID + HeaderLength + ChunkLength

    public List<Chunk> Chunks { get; private set; } = [];

    public uint Size => HEADER_SIZE + (uint)Chunks.Sum(x => x.Size);
    public byte[] Bytes
    {
        get
        {
            List<byte> bytes = [];
            bytes.AddRange(BitConverter.GetBytes(SIGNATURE));
            bytes.AddRange(BitConverter.GetBytes(HEADER_SIZE));
            bytes.AddRange(BitConverter.GetBytes(Size));
            foreach (var chunk in Chunks)
                bytes.AddRange(chunk.Bytes);
            return [.. bytes];
        }
    }

    private static readonly HashSet<uint> ChunkSortPriority = [
        (uint)ChunkIdentifier.History,
        (uint)ChunkIdentifier.Export_Info,
        (uint)ChunkIdentifier.Image,
        (uint)ChunkIdentifier.Texture,
        (uint)ChunkIdentifier.Set,
        (uint)ChunkIdentifier.Shader,
        (uint)ChunkIdentifier.Texture_Font,
        (uint)ChunkIdentifier.Image_Font,
        (uint)ChunkIdentifier.Sprite,
        (uint)ChunkIdentifier.Old_Vertex_Anim_Key_Frame,
        (uint)ChunkIdentifier.Animation,
        (uint)ChunkIdentifier.Light,
        (uint)ChunkIdentifier.Light_Group,
        (uint)ChunkIdentifier.Photon_Map,
        (uint)ChunkIdentifier.Camera,
        (uint)ChunkIdentifier.Skeleton,
        (uint)ChunkIdentifier.Lens_Flare_Group,
        (uint)ChunkIdentifier.Particle_System_Factory,
        (uint)ChunkIdentifier.Particle_System_2,
        (uint)ChunkIdentifier.Old_Billboard_Quad,
        (uint)ChunkIdentifier.Old_Billboard_Quad_Group,
        (uint)ChunkIdentifier.Mesh,
        (uint)ChunkIdentifier.Physics_Object,
        (uint)ChunkIdentifier.Collision_Object,
        (uint)ChunkIdentifier.Anim_Dyna_Phys,
        (uint)ChunkIdentifier.Anim_Coll,
        (uint)ChunkIdentifier.Anim_Dyna_Phys_Wrapper,
        (uint)ChunkIdentifier.Breakable_Object,
        (uint)ChunkIdentifier.Inst_Particle_System,
        (uint)ChunkIdentifier.Anim,
        (uint)ChunkIdentifier.Locator,
        (uint)ChunkIdentifier.Spline,
        (uint)ChunkIdentifier.Skin,
        (uint)ChunkIdentifier.Expression_Group,
        (uint)ChunkIdentifier.Expression_Mixer,
        (uint)ChunkIdentifier.Wall,
        (uint)ChunkIdentifier.Fenceline,
        (uint)ChunkIdentifier.Intersection,
        (uint)ChunkIdentifier.Road_Data_Segment,
        (uint)ChunkIdentifier.Road_Segment,
        (uint)ChunkIdentifier.Road,
        (uint)ChunkIdentifier.Path,
        (uint)ChunkIdentifier.Composite_Drawable,
        (uint)ChunkIdentifier.Animated_Object_Factory,
        (uint)ChunkIdentifier.Animated_Object,
        (uint)ChunkIdentifier.Scenegraph,
        (uint)ChunkIdentifier.Old_Frame_Controller,
        (uint)ChunkIdentifier.Multi_Controller,
        (uint)ChunkIdentifier.Old_Locator,
        (uint)ChunkIdentifier.Game_Attr,
        (uint)ChunkIdentifier.Smart_Prop,
        (uint)ChunkIdentifier.State_Prop_Data_V1,
    ];

    public P3DFile() { }

    public P3DFile (string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Could not find the specified file.", filePath);

        FileStream fs = null;
        MemoryStream ms = null;
        EndianAwareBinaryReader br = null;
        try
        {
            fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs.Length < HEADER_SIZE)
                throw new InvalidDataException("Specified file too short. Must be at least 12 bytes.");

            byte[] signatureBuffer = new byte[4];
            fs.Read(signatureBuffer, 0, 4);
            uint signature = BitConverter.ToUInt32(signatureBuffer, 0);

            switch (signature)
            {
                case SIGNATURE:
                    br = new(fs, BinaryExtensions.DefaultEndian);
                    break;
                case SIGNATURE_SWAP:
                    br = new(fs, BinaryExtensions.SwappedEndian);
                    break;
                case COMPRESSED_SIGNATURE:
                    fs.Position = 0;
                    using (EndianAwareBinaryReader br2 = new(fs, BinaryExtensions.DefaultEndian))
                    {
                        byte[] decryptedBytes = LZR_Compression.DecompressFile(br2);
                        ms = new(decryptedBytes);
                        br = new(ms, BinaryExtensions.DefaultEndian);
                        if (br.ReadUInt32() != SIGNATURE)
                            throw new InvalidDataException($"Decompressed file signature 0x{signature:X} is invalid.");
                    }
                    break;
                case COMPRESSED_SIGNATURE_SWAP:
                    fs.Position = 0;
                    using (EndianAwareBinaryReader br2 = new(fs, BinaryExtensions.SwappedEndian))
                    {
                        byte[] decryptedBytes = LZR_Compression.DecompressFile(br2);
                        ms = new(decryptedBytes);
                        br = new(ms, BinaryExtensions.SwappedEndian);
                        if (br.ReadUInt32() != SIGNATURE)
                            throw new InvalidDataException($"Decompressed file signature 0x{signature:X} is invalid.");
                    }
                    break;
                default:
                    throw new InvalidDataException($"File signature 0x{signature:X} is invalid.");
            }

            uint headerSize = br.ReadUInt32();
            if (headerSize != HEADER_SIZE)
                throw new InvalidDataException($"P3D File header size incorrect. Expected 12, read {headerSize}.");

            uint fileSize = br.ReadUInt32();
            if (fileSize != br.BaseStream.Length)
                throw new InvalidDataException($"P3D File full size incorrect. Expected {br.BaseStream.Length}, read {fileSize}.");

            uint bytesRead = headerSize;
            while (bytesRead < fileSize)
            {
                Chunk c = ChunkLoader.LoadChunk(br);
                Chunks.Add(c);
                bytesRead += c.Size;
            }
        }
        finally
        {
            br?.Close();
            br?.Dispose();
            
            ms?.Close();
            ms?.Dispose();

            fs?.Close();
            fs?.Dispose();
        }
    }

    /// <summary>
    /// Sort <see cref="Chunks"/> in the priority order <c>Simpsons: Hit &amp; Run</c> expects.
    /// <para>Chunk IDs without a sort priority will remain in their original order at the end of the file.</para>
    /// </summary>
    public void SortChunks()
    {
        List<Chunk> newChunks = new(Chunks.Count);

        foreach (uint id in ChunkSortPriority)
            newChunks.AddRange(Chunks.Where(x => x.ID == id));

        newChunks.AddRange(Chunks.Where(x => !ChunkSortPriority.Contains(x.ID)));

        Chunks = newChunks;
    }

    public T GetFirstChunkOfType<T>() where T : Chunk => Chunks.OfType<T>()?.FirstOrDefault();

    public T GetFirstChunkOfType<T>(string name) where T : NamedChunk => Chunks.OfType<T>()?.FirstOrDefault(x => x.Name.Equals(name));

    public T GetLastChunkOfType<T>() where T : Chunk => Chunks.OfType<T>()?.LastOrDefault();

    public T GetLastChunkOfType<T>(string name) where T : NamedChunk => Chunks.OfType<T>()?.LastOrDefault(x => x.Name.Equals(name));

    public T[] GetChunksOfType<T>() where T : Chunk => Chunks.OfType<T>().ToArray();

    public T[] GetChunksOfType<T>(string name) where T : NamedChunk => Chunks.OfType<T>().Where(x => x.Name.Equals(name)).ToArray();

    public void Write(string filePath) => Write(filePath, BinaryExtensions.DefaultEndian);

    public void Write(string filePath, Endianness endianness)
    {
        using FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        Write(fs, endianness);
    }

    public void Write(Stream stream) => Write(stream, BinaryExtensions.DefaultEndian);

    public void Write(Stream stream, Endianness endianness)
    {
        if (!stream.CanWrite)
            throw new ArgumentException("Cannot write to stream.", nameof(stream));

        foreach (var c in Chunks)
            c.Validate();

        EndianAwareBinaryWriter bw = new(stream, endianness);

        bw.Write(SIGNATURE);
        bw.Write(HEADER_SIZE);
        bw.Write(Size);
        foreach (Chunk chunk in Chunks)
            chunk.Write(bw);
    }
}
