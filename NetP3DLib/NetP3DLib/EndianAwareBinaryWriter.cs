using System;
using System.IO;
using System.Text;

namespace NetP3DLib;

public class EndianAwareBinaryWriter : BinaryWriter
{
    private readonly Endianness _endianness = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

    public EndianAwareBinaryWriter(Stream output) : base(output)
    {
    }

    public EndianAwareBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
    {
    }

    public EndianAwareBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
    {
    }

    public EndianAwareBinaryWriter(Stream output, Endianness endianness) : base(output)
    {
        _endianness = endianness;
    }

    public EndianAwareBinaryWriter(Stream output, Encoding encoding, Endianness endianness) : base(output, encoding)
    {
        _endianness = endianness;
    }

    public EndianAwareBinaryWriter(Stream output, Encoding encoding, bool leaveOpen, Endianness endianness) : base(output, encoding, leaveOpen)
    {
        _endianness = endianness;
    }

    public EndianAwareBinaryWriter(Endianness endianness)
    {
        _endianness = endianness;
    }

    public override void Write(short value) => Write(value, _endianness);

    public override void Write(int value) => Write(value, _endianness);

    public override void Write(long value) => Write(value, _endianness);

    public override void Write(ushort value) => Write(value, _endianness);

    public override void Write(uint value) => Write(value, _endianness);

    public override void Write(ulong value) => Write(value, _endianness);

    public void Write(short value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(int value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(long value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(ushort value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(uint value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(ulong value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    private void WriteForEndianness(byte[] bytesToWrite, Endianness endianness)
    {
        if ((endianness == Endianness.Little && !BitConverter.IsLittleEndian)
            || (endianness == Endianness.Big && BitConverter.IsLittleEndian))
        {
            Array.Reverse(bytesToWrite);
        }

        Write(bytesToWrite);
    }
}
