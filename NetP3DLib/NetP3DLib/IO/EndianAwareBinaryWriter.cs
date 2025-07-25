using System;
using System.IO;
using System.Text;

namespace NetP3DLib.IO;

public class EndianAwareBinaryWriter : BinaryWriter
{
    public Endianness Endianness { get; } = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

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
        Endianness = endianness;
    }

    public EndianAwareBinaryWriter(Stream output, Encoding encoding, Endianness endianness) : base(output, encoding)
    {
        Endianness = endianness;
    }

    public EndianAwareBinaryWriter(Stream output, Encoding encoding, bool leaveOpen, Endianness endianness) : base(output, encoding, leaveOpen)
    {
        Endianness = endianness;
    }

    public EndianAwareBinaryWriter(Endianness endianness)
    {
        Endianness = endianness;
    }

    public override void Write(short value) => Write(value, Endianness);

    public override void Write(int value) => Write(value, Endianness);

    public override void Write(long value) => Write(value, Endianness);

    public override void Write(ushort value) => Write(value, Endianness);

    public override void Write(uint value) => Write(value, Endianness);

    public override void Write(ulong value) => Write(value, Endianness);

    public override void Write(float value) => Write(value, Endianness);

    public override void Write(double value) => Write(value, Endianness);

    public void Write(short value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(int value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(long value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(ushort value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(uint value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(ulong value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(float value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    public void Write(double value, Endianness endianness) => WriteForEndianness(BitConverter.GetBytes(value), endianness);

    private void WriteForEndianness(byte[] bytesToWrite, Endianness endianness)
    {
        if (endianness == Endianness.Little && !BitConverter.IsLittleEndian
            || endianness == Endianness.Big && BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytesToWrite);
        }

        Write(bytesToWrite);
    }
}
