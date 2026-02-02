using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrontendScreenChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frontend_Screen;
    
    [DefaultValue(0)]
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
    public override uint DataLength
    {
        get
        {
            uint size = BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);
            foreach (var pageName in PageNames)
                size += BinaryExtensions.GetP3DStringLength(pageName);
            return size;
        }
    }

    public FrontendScreenChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        var numPageNames = br.ReadInt32();
        PageNames = new(numPageNames);
        for (int i = 0; i < numPageNames; i++)
            PageNames.Add(br.ReadP3DString());
    }

    public FrontendScreenChunk(string name, uint version, IList<string> pageNames) : base(ChunkID)
    {
        Name = name;
        Version = version;
        PageNames.AddRange(pageNames);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        foreach (var pageName in PageNames)
            if (!pageName.IsValidP3DString())
                yield return new InvalidP3DStringException(this, nameof(PageNames), pageName);

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumPageNames);
        foreach (var pageName in PageNames)
            bw.WriteP3DString(pageName);
    }

    protected override Chunk CloneSelf() => new FrontendScreenChunk(Name, Version, PageNames);
}
