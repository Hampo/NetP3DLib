using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SkinChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Skin;
    
    public uint Version { get; set; }
    public string SkeletonName { get; set; }
    public uint NumOldPrimitiveGroups => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Old_Primitive_Group).Count();
    public uint NumPrimitiveGroups => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Primitive_Group).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(SkeletonName));
            data.AddRange(BitConverter.GetBytes(NumOldPrimitiveGroups + NumPrimitiveGroups));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(SkeletonName).Length + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public SkinChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        SkeletonName = br.ReadP3DString();
        var numPrimitiveGroups = br.ReadUInt32();
    }

    public SkinChunk(string name, uint version, string skeletonName) : base(ChunkID)
    {
        Name = name;
        Version = version;
        SkeletonName = skeletonName;
    }

    public override void Validate()
    {
        if (SkeletonName == null)
            throw new InvalidDataException($"{nameof(SkeletonName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(SkeletonName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(SkeletonName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(SkeletonName);
        bw.Write(NumOldPrimitiveGroups + NumPrimitiveGroups);
    }
}