using NetP3DLib.IO;
using NetP3DLib.Numerics;
using NetP3DLib.P3D.Exceptions;
using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Text;

namespace NetP3DLib.P3D.Extensions;

public static class BinaryExtensions
{
    /// <summary>
    /// The default <see cref="Endianness"/> of the system.
    /// </summary>
    public static Endianness DefaultEndian { get; } = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;
    /// <summary>
    /// The swapped <see cref="Endianness"/> of the system.
    /// </summary>
    public static Endianness SwappedEndian { get; } = BitConverter.IsLittleEndian ? Endianness.Big : Endianness.Little;

    /// <summary>
    /// Gets the byte array for a Pure3D string.
    /// <para>The format is 1 byte for the string length, followed by that many bytes representing the string.</para>
    /// <para>This uses <c>Simpsons: Hit &amp; Run</c> padding that does not include the length byte.</para>
    /// </summary>
    /// <param name="value">The value to get the bytes of.</param>
    /// <returns>A <c>byte[]</c> representing <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">Throws an exception if <paramref name="value"/> is too long.</exception>
    public static byte[] GetP3DStringBytes(string value)
    {
        if (!value.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(value), value);

        byte[] bytes = Encoding.UTF8.GetBytes(value);
        int length = bytes.Length;

        if (length < 252)
        {
            int diff = length & 3;
            if (diff > 0)
                length += 4 - diff;
        }

        byte[] buffer = new byte[length + 1];
        buffer[0] = (byte)length;
        bytes.CopyTo(buffer, 1);
        return buffer;
    }

    /// <summary>
    /// Gets the byte length for a Pure3D string.
    /// <para>The format is 1 byte for the string length, followed by that many bytes representing the string.</para>
    /// <para>This uses <c>Simpsons: Hit &amp; Run</c> padding that does not include the length byte.</para>
    /// </summary>
    /// <param name="value">The value to get the length of.</param>
    /// <returns>A <c>uint</c> representing the length of <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">Throws an exception if <paramref name="value"/> is too long.</exception>
    public static uint GetP3DStringLength(string value)
    {
        if (!value.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(value), value);

        int length = Encoding.UTF8.GetByteCount(value);

        if (length < 252)
        {
            int diff = length & 3;
            if (diff > 0)
                length += 4 - diff;
        }

        return (uint)length + 1;
    }

    /// <summary>
    /// Reads a Pure3D string from <paramref name="br"/>.
    /// <para>The format is 1 byte for the string length, followed by that many bytes representing the string.</para>
    /// </summary>
    /// <param name="br">The <c>BinaryReader</c> to read from.</param>
    /// <returns>A <c>string</c> with trailing <c>null</c> bytes trimmed.</returns>
    public static string ReadP3DString(this BinaryReader br)
    {
        byte valueLength = br.ReadByte();
        byte[] valueBytes = br.ReadBytes(valueLength);
        string value = Encoding.UTF8.GetString(valueBytes);
        return value.TrimEnd('\0');
    }

    /// <summary>
    /// Writes a Pure3D string to <paramref name="bw"/>.
    /// <para>The format is 1 byte for the string length, followed by that many bytes representing the string.</para>
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    /// <param name="value">The value to write.</param>
    /// <exception cref="ArgumentException">Throws an exception if <paramref name="value"/> is too long.</exception>
    public static void WriteP3DString(this BinaryWriter bw, string value)
    {
        if (!value.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(value), value);

        byte[] bytes = Encoding.UTF8.GetBytes(value);
        int length = bytes.Length;

        if (length < 252)
        {
            int diff = length & 3;
            if (diff > 0)
                length += 4 - diff;
        }

        bw.Write((byte)length);
        bw.Write(bytes, 0, bytes.Length);
        int padding = length - bytes.Length;
        for (int i = 0; i < padding; i++)
            bw.Write((byte)0);
    }

    /// <summary>
    /// Gets the byte array for a FourCC string.
    /// </summary>
    /// <param name="value">The value to get the bytes of.</param>
    /// <returns>A <c>byte[]</c> representing <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">Throws an exception if <paramref name="value"/> is too long.</exception>
    public static byte[] GetFourCCBytes(string value)
    {
        if (!value.IsValidFourCC())
            throw new InvalidFourCCException(nameof(value), value);

        char[] c = [(char)0, (char)0, (char)0, (char)0];
        value.ToCharArray().CopyTo(c, 0);

        return BitConverter.GetBytes(c[3] << 24 | c[2] << 16 | c[1] << 8 | c[0]);
    }

    /// <summary>
    /// Reads a FourCC string from <paramref name="br"/>.
    /// </summary>
    /// <param name="br">The <c>BinaryReader</c> to read from.</param>
    /// <returns>A <c>string</c> with trailing <c>null</c> bytes trimmed.</returns>
    public static string ReadFourCC(this BinaryReader br)
    {
        int val = br.ReadInt32();
        char[] chars =
        [
            (char)(val & 0xFF),
            (char)(val >> 8 & 0xFF),
            (char)(val >> 16 & 0xFF),
            (char)(val >> 24 & 0xFF),
        ];
        return new string(chars).TrimEnd('\0');
    }

    /// <summary>
    /// Writes a FourCC string to <paramref name="bw"/>.
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    /// <param name="value">The value to write.</param>
    /// <exception cref="ArgumentException">Throws an exception if <paramref name="value"/> is too long.</exception>
    public static void WriteFourCC(this BinaryWriter bw, string value)
    {
        if (!value.IsValidFourCC())
            throw new InvalidFourCCException(nameof(value), value);

        char[] c = [(char)0, (char)0, (char)0, (char)0];
        value.ToCharArray().CopyTo(c, 0);

        bw.Write(c[3] << 24 | c[2] << 16 | c[1] << 8 | c[0]);
    }

    /// <summary>
    /// Gets the byte array for a <see cref="Color"/>.
    /// </summary>
    /// <param name="value">The value to get the bytes of.</param>
    /// <returns>A <c>byte[]</c> representing <paramref name="value"/>.</returns>
    public static byte[] GetBytes(Color value) => BitConverter.GetBytes(value.ToArgb());

    /// <summary>
    /// Reads a <see cref="Color"/> from <paramref name="br"/>.
    /// </summary>
    /// <param name="br">The <c>BinaryReader</c> to read from.</param>
    /// <returns>A <c>Color</c>.</returns>
    public static Color ReadColor(this BinaryReader br) => Color.FromArgb(br.ReadInt32());

    /// <summary>
    /// Writes a <see cref="Color"/> to <paramref name="bw"/>.
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    /// <param name="value">The value to write.</param>
    public static void Write(this BinaryWriter bw, Color value) => bw.Write(value.ToArgb());

    /// <summary>
    /// Gets the byte array for a <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value">The value to get the bytes of.</param>
    /// <returns>A <c>byte[]</c> representing <paramref name="value"/>.</returns>
    public static byte[] GetBytes(Vector2 value) => [.. BitConverter.GetBytes(value.X), .. BitConverter.GetBytes(value.Y)];

    /// <summary>
    /// Reads a <see cref="Vector2"/> from <paramref name="br"/>.
    /// </summary>
    /// <param name="br">The <c>BinaryReader</c> to read from.</param>
    /// <returns>A <c>Vector2</c>.</returns>
    public static Vector2 ReadVector2(this BinaryReader br) => new(br.ReadSingle(), br.ReadSingle());

    /// <summary>
    /// Writes a <see cref="Vector2"/> to <paramref name="bw"/>.
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    /// <param name="value">The value to write.</param>
    public static void Write(this BinaryWriter bw, Vector2 value)
    {
        bw.Write(value.X);
        bw.Write(value.Y);
    }

    /// <summary>
    /// Gets the byte array for a <see cref="Vector3"/>.
    /// </summary>
    /// <param name="value">The value to get the bytes of.</param>
    /// <returns>A <c>byte[]</c> representing <paramref name="value"/>.</returns>
    public static byte[] GetBytes(Vector3 value) => [.. BitConverter.GetBytes(value.X), .. BitConverter.GetBytes(value.Y), .. BitConverter.GetBytes(value.Z)];

    /// <summary>
    /// Reads a <see cref="Vector3"/> from <paramref name="br"/>.
    /// </summary>
    /// <param name="br">The <c>BinaryReader</c> to read from.</param>
    /// <returns>A <c>Vector3</c>.</returns>
    public static Vector3 ReadVector3(this BinaryReader br) => new(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

    /// <summary>
    /// Writes a <see cref="Vector3"/> to <paramref name="bw"/>.
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    /// <param name="value">The value to write.</param>
    public static void Write(this BinaryWriter bw, Vector3 value)
    {
        bw.Write(value.X);
        bw.Write(value.Y);
        bw.Write(value.Z);
    }

    /// <summary>
    /// Gets the byte array for a <see cref="Vector4"/>.
    /// </summary>
    /// <param name="value">The value to get the bytes of.</param>
    /// <returns>A <c>byte[]</c> representing <paramref name="value"/>.</returns>
    public static byte[] GetBytes(Vector4 value) => [.. BitConverter.GetBytes(value.W), .. BitConverter.GetBytes(value.X), .. BitConverter.GetBytes(value.Y), .. BitConverter.GetBytes(value.Z)];

    /// <summary>
    /// Reads a <see cref="Vector4"/> from <paramref name="br"/>.
    /// </summary>
    /// <param name="br">The <c>BinaryReader</c> to read from.</param>
    /// <returns>A <c>Vector4</c>.</returns>
    public static Vector4 ReadVector4(this BinaryReader br)
    {
        var w = br.ReadSingle();
        var x = br.ReadSingle();
        var y = br.ReadSingle();
        var z = br.ReadSingle();
        return new(x, y, z, w);
    }

    /// <summary>
    /// Writes a <see cref="Vector4"/> to <paramref name="bw"/>.
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    /// <param name="value">The value to write.</param>
    public static void Write(this BinaryWriter bw, Vector4 value)
    {
        bw.Write(value.W);
        bw.Write(value.X);
        bw.Write(value.Y);
        bw.Write(value.Z);
    }

    /// <summary>
    /// Gets the byte array for a <see cref="Quaternion"/>.
    /// </summary>
    /// <param name="value">The value to get the bytes of.</param>
    /// <returns>A <c>byte[]</c> representing <paramref name="value"/>.</returns>
    public static byte[] GetBytes(Quaternion value) => [.. BitConverter.GetBytes(value.W), .. BitConverter.GetBytes(value.X), .. BitConverter.GetBytes(value.Y), .. BitConverter.GetBytes(value.Z)];

    /// <summary>
    /// Reads a <see cref="Quaternion"/> from <paramref name="br"/>.
    /// </summary>
    /// <param name="br">The <c>BinaryReader</c> to read from.</param>
    /// <returns>A <c>Quaternion</c>.</returns>
    public static Quaternion ReadQuaternion(this BinaryReader br)
    {
        var w = br.ReadSingle();
        var x = br.ReadSingle();
        var y = br.ReadSingle();
        var z = br.ReadSingle();
        return new(x, y, z, w);
    }

    /// <summary>
    /// Writes a <see cref="Quaternion"/> to <paramref name="bw"/>.
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    /// <param name="value">The value to write.</param>
    public static void Write(this BinaryWriter bw, Quaternion value)
    {
        bw.Write(value.W);
        bw.Write(value.X);
        bw.Write(value.Y);
        bw.Write(value.Z);
    }

    /// <summary>
    /// Gets the byte array for a <see cref="Matrix3x2"/>.
    /// </summary>
    /// <param name="value">The value to get the bytes of.</param>
    /// <returns>A <c>byte[]</c> representing <paramref name="value"/>.</returns>
    public static byte[] GetBytes(Matrix3x2 value) =>
    [
        .. BitConverter.GetBytes(value.M11),
        .. BitConverter.GetBytes(value.M12),
        .. BitConverter.GetBytes(value.M21),
        .. BitConverter.GetBytes(value.M22),
        .. BitConverter.GetBytes(value.M31),
        .. BitConverter.GetBytes(value.M32),
    ];

    /// <summary>
    /// Reads a <see cref="Matrix3x2"/> from <paramref name="br"/>.
    /// </summary>
    /// <param name="br">The <c>BinaryReader</c> to read from.</param>
    /// <returns>A <c>Matrix3x2</c>.</returns>
    public static Matrix3x2 ReadMatrix3x2(this BinaryReader br) => new(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

    /// <summary>
    /// Writes a <see cref="Matrix3x2"/> to <paramref name="bw"/>.
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    /// <param name="value">The value to write.</param>
    public static void Write(this BinaryWriter bw, Matrix3x2 value)
    {
        bw.Write(value.M11);
        bw.Write(value.M12);

        bw.Write(value.M21);
        bw.Write(value.M22);

        bw.Write(value.M31);
        bw.Write(value.M32);
    }

    /// <summary>
    /// Gets the byte array for a <see cref="SymmetricMatrix3x3"/>.
    /// </summary>
    /// <param name="value">The value to get the bytes of.</param>
    /// <returns>A <c>byte[]</c> representing <paramref name="value"/>.</returns>
    public static byte[] GetBytes(SymmetricMatrix3x3 value) =>
    [
        .. BitConverter.GetBytes(value.XX),
        .. BitConverter.GetBytes(value.XY),
        .. BitConverter.GetBytes(value.XZ),
        .. BitConverter.GetBytes(value.YY),
        .. BitConverter.GetBytes(value.YZ),
        .. BitConverter.GetBytes(value.ZZ),
    ];

    /// <summary>
    /// Reads a <see cref="SymmetricMatrix3x3"/> from <paramref name="br"/>.
    /// </summary>
    /// <param name="br">The <c>BinaryReader</c> to read from.</param>
    /// <returns>A <c>SymmetricMatrix3x3</c>.</returns>
    public static SymmetricMatrix3x3 ReadSymmetricMatrix3x3(this BinaryReader br) => new(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

    /// <summary>
    /// Writes a <see cref="Matrix3x2"/> to <paramref name="bw"/>.
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    /// <param name="value">The value to write.</param>
    public static void Write(this BinaryWriter bw, SymmetricMatrix3x3 value)
    {
        bw.Write(value.XX);
        bw.Write(value.XY);
        bw.Write(value.XZ);

        bw.Write(value.YY);
        bw.Write(value.YZ);

        bw.Write(value.ZZ);
    }

    /// <summary>
    /// Gets the byte array for a <see cref="Matrix4x4"/>.
    /// </summary>
    /// <param name="value">The value to get the bytes of.</param>
    /// <returns>A <c>byte[]</c> representing <paramref name="value"/>.</returns>
    public static byte[] GetBytes(Matrix4x4 value) =>
    [
        .. BitConverter.GetBytes(value.M11),
        .. BitConverter.GetBytes(value.M12),
        .. BitConverter.GetBytes(value.M13),
        .. BitConverter.GetBytes(value.M14),
        .. BitConverter.GetBytes(value.M21),
        .. BitConverter.GetBytes(value.M22),
        .. BitConverter.GetBytes(value.M23),
        .. BitConverter.GetBytes(value.M24),
        .. BitConverter.GetBytes(value.M31),
        .. BitConverter.GetBytes(value.M32),
        .. BitConverter.GetBytes(value.M33),
        .. BitConverter.GetBytes(value.M34),
        .. BitConverter.GetBytes(value.M41),
        .. BitConverter.GetBytes(value.M42),
        .. BitConverter.GetBytes(value.M43),
        .. BitConverter.GetBytes(value.M44),
    ];

    /// <summary>
    /// Reads a <see cref="Matrix4x4"/> from <paramref name="br"/>.
    /// </summary>
    /// <param name="br">The <c>BinaryReader</c> to read from.</param>
    /// <returns>A <c>Matrix4x4</c>.</returns>
    public static Matrix4x4 ReadMatrix4x4(this BinaryReader br) => new(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

    /// <summary>
    /// Writes a <see cref="Matrix4x4"/> to <paramref name="bw"/>.
    /// </summary>
    /// <param name="bw">The <c>BinaryWriter</c> to write to.</param>
    /// <param name="value">The value to write.</param>
    public static void Write(this BinaryWriter bw, Matrix4x4 value)
    {
        bw.Write(value.M11);
        bw.Write(value.M12);
        bw.Write(value.M13);
        bw.Write(value.M14);

        bw.Write(value.M21);
        bw.Write(value.M22);
        bw.Write(value.M23);
        bw.Write(value.M24);

        bw.Write(value.M31);
        bw.Write(value.M32);
        bw.Write(value.M33);
        bw.Write(value.M34);

        bw.Write(value.M41);
        bw.Write(value.M42);
        bw.Write(value.M43);
        bw.Write(value.M44);
    }
}
