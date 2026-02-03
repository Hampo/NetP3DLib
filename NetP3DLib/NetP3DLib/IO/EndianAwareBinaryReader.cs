using NetP3DLib.P3D.Extensions;
using System;
using System.IO;
using System.Text;

namespace NetP3DLib.IO;

public class EndianAwareBinaryReader : BinaryReader
{
    public Endianness Endianness { get; } = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

    public EndianAwareBinaryReader(Stream input, Encoding encoding, bool leaveOpen, Endianness endianness) : base(input, encoding, leaveOpen)
    {
        Endianness = endianness;
    }

    public EndianAwareBinaryReader(Stream input) : this(input, Encoding.UTF8, leaveOpen: false, BinaryExtensions.DefaultEndian)
    {}

    public EndianAwareBinaryReader(Stream input, bool leaveOpen) : this(input, Encoding.UTF8, leaveOpen, BinaryExtensions.DefaultEndian)
    {}

    public EndianAwareBinaryReader(Stream input, Encoding encoding) : this(input, encoding, leaveOpen: false, BinaryExtensions.DefaultEndian)
    {}

    public EndianAwareBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : this(input, encoding, leaveOpen, BinaryExtensions.DefaultEndian)
    {}

    public EndianAwareBinaryReader(Stream input, Endianness endianness) : this(input, Encoding.UTF8, leaveOpen: false, endianness)
    {}

    public EndianAwareBinaryReader(Stream input, bool leaveOpen, Endianness endianness) : this(input, Encoding.UTF8, leaveOpen, endianness)
    {}

    public EndianAwareBinaryReader(Stream input, Encoding encoding, Endianness endianness) : this(input, encoding, leaveOpen: false, endianness)
    {}

    public override short ReadInt16() => ReadInt16(Endianness);

    public override int ReadInt32() => ReadInt32(Endianness);

    public override long ReadInt64() => ReadInt64(Endianness);

    public override ushort ReadUInt16() => ReadUInt16(Endianness);

    public override uint ReadUInt32() => ReadUInt32(Endianness);

    public override ulong ReadUInt64() => ReadUInt64(Endianness);

    public override float ReadSingle() => ReadSingle(Endianness);

    public override double ReadDouble() => ReadDouble(Endianness);

    public short ReadInt16(Endianness endianness) => BitConverter.ToInt16(ReadForEndianness(sizeof(short), endianness), 0);

    public int ReadInt32(Endianness endianness) => BitConverter.ToInt32(ReadForEndianness(sizeof(int), endianness), 0);

    public long ReadInt64(Endianness endianness) => BitConverter.ToInt64(ReadForEndianness(sizeof(long), endianness), 0);

    public ushort ReadUInt16(Endianness endianness) => BitConverter.ToUInt16(ReadForEndianness(sizeof(ushort), endianness), 0);

    public uint ReadUInt32(Endianness endianness) => BitConverter.ToUInt32(ReadForEndianness(sizeof(uint), endianness), 0);

    public ulong ReadUInt64(Endianness endianness) => BitConverter.ToUInt64(ReadForEndianness(sizeof(ulong), endianness), 0);

    public float ReadSingle(Endianness endianness) => BitConverter.ToSingle(ReadForEndianness(sizeof(float), endianness), 0);

    public double ReadDouble(Endianness endianness) => BitConverter.ToDouble(ReadForEndianness(sizeof(double), endianness), 0);

    private byte[] ReadForEndianness(int bytesToRead, Endianness endianness)
    {
        var bytesRead = ReadBytes(bytesToRead);

        if (endianness == Endianness.Little && !BitConverter.IsLittleEndian
            || endianness == Endianness.Big && BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytesRead);
        }

        return bytesRead;
    }
}
