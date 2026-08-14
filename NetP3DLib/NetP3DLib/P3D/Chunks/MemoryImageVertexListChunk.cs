using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MemoryImageVertexListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Memory_Image_Vertex_List;

    private uint _version;
    public uint Version
    {
        get => _version;
        set
        {
            if (_version == value)
                return;
    
            _version = value;
            OnPropertyChanged(nameof(Version));
        }
    }
    
    private uint _param;
    public uint Param
    {
        get => _param;
        set
        {
            if (_param == value)
                return;
    
            _param = value;
            OnPropertyChanged(nameof(Param));
        }
    }

    public uint NumVertices
    {
        get => (uint)(Vertices?.Count ?? 0);
        set
        {
            if (value == NumVertices)
                return;

            if (value < NumVertices)
            {
                Vertices.RemoveRange((int)value, (int)(NumVertices - value));
            }
            else
            {
                int count = (int)(value - NumVertices);
                var newOffsets = new Vertex[count];

                for (var i = 0; i < count; i++)
                    newOffsets[i] = new();

                Vertices.AddRange(newOffsets);
            }
        }
    }
    public SizeAwareList<Vertex> Vertices { get; }

    public override byte[] DataBytes
    {
        get
        {
            var data = new List<byte>((int)DataLength);

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Param));
            data.AddRange(BitConverter.GetBytes(NumVertices * Vertex.Size));
            foreach (var vertex in Vertices)
                data.AddRange(vertex.DataBytes);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) + NumVertices * Vertex.Size;

    public MemoryImageVertexListChunk(EndianAwareBinaryReader br) : this(br.ReadUInt32(), br.ReadUInt32(), br.ReadArray((int)(br.ReadUInt32() / Vertex.Size), () => new Vertex(br)))
    {
    }

    public MemoryImageVertexListChunk(uint version, uint param, IList<Vertex> vertices) : base(ChunkID)
    {
        _version = version;
        _param = param;
        Vertices = CreateSizeAwareList(vertices, Vertices_CollectionChanged);
    }

    private void Vertices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Vertices));

        if (e.OldItems != null)
            foreach (Vertex oldItem in e.OldItems)
                oldItem.PropertyChanged -= Offsets_PropertyChanged;

        if (e.NewItems != null)
            foreach (Vertex newItem in e.NewItems)
                newItem.PropertyChanged += Offsets_PropertyChanged;
    }

    private void Offsets_PropertyChanged() => OnPropertyChanged(nameof(Vertices));

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Param);
        bw.Write(NumVertices * Vertex.Size);
        foreach (var vertex in Vertices)
            vertex.Write(bw);
    }

    protected override Chunk CloneSelf()
    {
        var vertices = new Vertex[Vertices.Count];
        for (var i = 0; i < Vertices.Count; i++)
            vertices[i] = Vertices[i].Clone();
        return new MemoryImageVertexListChunk(Version, Param, vertices);
    }

    public class Vertex
    {
        public const uint Size = sizeof(float) * 3 + sizeof(uint) + sizeof(float) * 2;

        public event Action? PropertyChanged;

        private Vector3 _position;
        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position == value)
                    return;

                _position = value;
                PropertyChanged?.Invoke();
            }
        }

        private uint _packedNormal;
        public uint PackedNormal
        {
            get => _packedNormal;
            set
            {
                if (_packedNormal == value)
                    return;

                _packedNormal = value;
                PropertyChanged?.Invoke();
            }
        }

        private Vector2 _uv;
        public Vector2 UV
        {
            get => _uv;
            set
            {
                if (_uv == value)
                    return;

                _uv = value;
                PropertyChanged?.Invoke();
            }
        }

        public byte[] DataBytes
        {
            get
            {
                var data = new List<byte>((int)Size);

                data.AddRange(BinaryExtensions.GetBytes(Position));
                data.AddRange(BitConverter.GetBytes(PackedNormal));
                data.AddRange(BinaryExtensions.GetBytes(UV));

                return [.. data];
            }
        }

        public Vertex(BinaryReader br)
        {
            _position = br.ReadVector3();
            _packedNormal = br.ReadUInt32();
            _uv = br.ReadVector2();
        }

        public Vertex(Vector3 position, uint packedNormal, Vector2 uv)
        {
            _position = position;
            _packedNormal = packedNormal;
            _uv = uv;
        }

        public Vertex()
        {
            _position = Vector3.Zero;
            _packedNormal = 0;
            _uv = Vector2.Zero;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Position);
            bw.Write(PackedNormal);
            bw.Write(UV);
        }

        internal Vertex Clone() => new(Position, PackedNormal, UV);

        public override string ToString() => $"{Position} | {PackedNormal} | {UV}";
    }
}
