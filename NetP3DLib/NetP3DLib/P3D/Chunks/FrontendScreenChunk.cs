using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendScreenChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Frontend_Screen;
    
    public uint Version { get; set; }
    public uint NumPageNames
    {
        get => (uint)PageNames.Count;
        set
        {
            if (value == NumPageNames)
                return;

            if (value < NumPageNames)
            {
                while (NumPageNames > value)
                    PageNames.RemoveAt(PageNames.Count - 1);
            }
            else
            {
                while (NumPageNames < value)
                    PageNames.Add(string.Empty);
            }
        }
    }
    public List<string> PageNames { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumPageNames));
            foreach (var pageName in PageNames)
                data.AddRange(BinaryExtensions.GetP3DStringBytes(pageName));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint) + (uint)PageNames.Sum(x => BinaryExtensions.GetP3DStringBytes(x).Length);

    public FrontendScreenChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        var numPageNames = br.ReadInt32();
        PageNames.Capacity = numPageNames;
        for (int i = 0; i < numPageNames; i++)
            PageNames.Add(br.ReadP3DString());
    }

    public FrontendScreenChunk(string name, uint version, IList<string> pageNames) : base(ChunkID)
    {
        Name = name;
        Version = version;
        PageNames.AddRange(pageNames);
    }

    public override void Validate()
    {
        if (PageNames.Any(x => x == null || x.Length > 255))
            throw new InvalidDataException($"All {nameof(PageNames)} must have a value, with a max length of 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumPageNames);
        foreach (var pageName in PageNames)
            bw.WriteP3DString(pageName);
    }
}