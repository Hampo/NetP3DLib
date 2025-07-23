using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MeshChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Mesh;
    
    public uint Version { get; set; }
    public uint NumOldPrimitiveGroups => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Old_Primitive_Group).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumOldPrimitiveGroups));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public MeshChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        var numOldPrimitiveGroups = br.ReadUInt32();
    }

    public MeshChunk(string name, uint version) : base(ChunkID)
    {
        Name = name;
        Version = version;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumOldPrimitiveGroups);
    }

    internal override Chunk CloneSelf() => new MeshChunk(Name, Version);
}