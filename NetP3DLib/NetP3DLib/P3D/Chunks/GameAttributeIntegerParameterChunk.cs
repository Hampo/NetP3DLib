using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class GameAttributeIntegerParameterChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Game_Attribute_Integer_Parameter;
    
    public uint Value { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Value));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint);

    public GameAttributeIntegerParameterChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Value = br.ReadUInt32();
    }

    public GameAttributeIntegerParameterChunk(string name, uint value) : base(ChunkID)
    {
        Name = name;
        Value = value;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Value);
    }

    protected override Chunk CloneSelf() => new GameAttributeIntegerParameterChunk(Name, Value);
}