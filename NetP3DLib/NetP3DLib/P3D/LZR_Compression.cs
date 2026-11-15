using NetP3DLib.P3D.Chunks;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D;

public static class LZR_Compression
{

    /// <summary>
    /// Reads <paramref name="file"/>, and decompresses it if a compressed Pure3D File is detected.
    /// <para>Returns the normal file bytes if not a compressed Pure3D File.</para>
    /// </summary>
    /// <param name="file">The <see cref="BinaryReader"/> to read.</param>
    /// <returns>Returns <paramref name="file"/>'s bytes, decompressed if it was a compressed Pure3D File.</returns>
    public static byte[] DecompressFile(BinaryReader file)
    {
        uint identifier = file.ReadUInt32();
        if (identifier != P3DFile.COMPRESSED_SIGNATURE)
        {
            file.BaseStream.Seek(0, SeekOrigin.Begin);
            return file.ReadBytes((int)file.BaseStream.Length);
        }

        uint uncompressedLength = file.ReadUInt32();
        return Decompress(file, uncompressedLength);
    }

    private static byte[] Decompress(BinaryReader file, uint uncompressedLength)
    {
        byte[] output = new byte[uncompressedLength];
        uint decompressedLength = 0;

        while (decompressedLength < uncompressedLength)
        {
            uint compressedLength = file.ReadUInt32();
            uint blockSize = file.ReadUInt32();
            long blockStart = file.BaseStream.Position;

            DecompressBlock(file, output, decompressedLength, blockSize);
            decompressedLength += blockSize;

            file.BaseStream.Seek(blockStart + compressedLength, SeekOrigin.Begin);
        }
        return output;
    }

    private static void DecompressBlock(BinaryReader br, byte[] output, uint offset, uint size)
    {
        int written = 0;

        while (written < size)
        {
            byte code = br.ReadByte();

            if (code > 15)
            {
                int matchLength = code & 15;

                if (matchLength == 0)
                {
                    matchLength = 15;
                    byte tmp = br.ReadByte();
                    while (tmp == 0)
                    {
                        matchLength += 255;
                        tmp = br.ReadByte();
                    }
                    matchLength += tmp;
                }

                byte tmpByte = br.ReadByte();
                int matchOffset = (code >> 4) | tmpByte << 4;
                int matchPos = (int)(offset + written - matchOffset);

                int len = matchLength >> 2;
                matchLength -= len << 2;

                do
                {
                    output[(int)(offset + written++)] = output[matchPos++];
                    output[(int)(offset + written++)] = output[matchPos++];
                    output[(int)(offset + written++)] = output[matchPos++];
                    output[(int)(offset + written++)] = output[matchPos++];

                    len--;
                } while (len != 0);

                while (matchLength != 0)
                {
                    output[(int)(offset + written++)] = output[matchPos++];

                    matchLength--;
                }
            }
            else
            {
                int runLength = code;

                if (runLength == 0)
                {
                    byte tmp = br.ReadByte();
                    while (tmp == 0)
                    {
                        runLength += 255;
                        tmp = br.ReadByte();
                    }
                    runLength += tmp;

                    br.Read(output, (int)(offset + written), 15);
                    written += 15;
                }

                br.Read(output, (int)(offset + written), runLength);
                written += runLength;
            }
        }
    }

    private const int MINIMUM_MATCH_LENGTH = 4;
    private const int LZR_BLOCK_SIZE = 4096;
    private const int TREE_ROOT = LZR_BLOCK_SIZE;
    private const int UNUSED = LZR_BLOCK_SIZE + 0xffff;

    private struct TreeNode
    {
        public uint Smaller;
        public uint Larger;
    }

    private static readonly TreeNode[] tree = new TreeNode[LZR_BLOCK_SIZE + 1];

    private static void CompressBlock(byte[] input, int inputStart, uint inputSize, byte[] output, ref int outputCount, bool fastest)
    {
        uint inputCount = 0;
        uint literalStart = 0;
        uint literalCount = 0;

        InitTree();

        uint offset = 0;
        uint count = 0;

        while (inputCount < inputSize)
        {
            if ((count > MINIMUM_MATCH_LENGTH) && ((offset & 15) != 0))
            {
                if (literalCount > 0)
                {
                    WriteCount(output, ref outputCount, literalCount, 0);

                    Buffer.BlockCopy(input, inputStart + (int)literalStart, output, outputCount, (int)literalCount);
                    outputCount += (int)literalCount;
                }

                WriteCount(output, ref outputCount, count, (byte)((offset & 0xf) << 4));
                output[outputCount++] = (byte)((offset & 0xff0) >> 4);

                if (fastest)
                {
                    inputCount += count;
                }
                else
                {
                    uint total = count;

                    for (uint c = 0; c < total; c++)
                    {
                        inputCount++;
                        AddString(input, inputStart, inputCount, inputSize, ref offset, ref count);
                    }
                }

                literalCount = 0;
                literalStart = inputCount;
            }
            else
            {
                literalCount++;
                inputCount++;

                if (!fastest)
                {
                    if (AddString(input, inputStart, inputCount, inputSize, ref offset, ref count))
                    {
                        FindLongestMatch(input, inputStart, inputCount, inputSize, ref offset, ref count);
                    }
                }
            }

            if (fastest)
            {
                AddString(input, inputStart, inputCount, inputSize, ref offset, ref count);
            }
        }

        if (literalCount > 0)
        {
            WriteCount(output, ref outputCount, literalCount, 0);

            Buffer.BlockCopy(input, inputStart + (int)literalStart, output, outputCount, (int)literalCount);
            outputCount += (int)literalCount;
        }
    }

    /// <summary>
    /// Converts a <see cref="List{T}"/> of bytes to a compressed <see cref="byte"/> array.
    /// <para>Note: This will ONLY write <c>Little Endian</c> files.</para>
    /// </summary>
    /// <param name="input">The list of bytes to compress.</param>
    /// <param name="fast">If <c>true</c>, a slightly faster algorithm will be used.<para>Defaults to <c>false</c>.</para></param>
    /// <returns>A <see cref="byte"/> array containing the compressed bytes of <paramref name="input"/>.</returns>
    /// <exception cref="InvalidDataException">Thrown if <paramref name="input"/> is not a Little Endian Pure3D file.</exception>
    public static byte[] CompressFile(IList<byte> input, bool fast = false)
    {
        return CompressFile([.. input], fast);
    }

    private static byte[] CompressFile(byte[] input, bool fast)
    {
        uint identifier =
            (uint)input[0] |
            ((uint)input[1] << 8) |
            ((uint)input[2] << 16) |
            ((uint)input[3] << 24);

        if (identifier != P3DFile.SIGNATURE)
            throw new InvalidDataException("The specified file is not a Little Endian P3D file.");

        uint fileSize = (uint)input.Length;

        int blockCount = ((input.Length - 1) / LZR_BLOCK_SIZE) + 1;

        byte[] output = new byte[checked(input.Length + blockCount * 64 + 8)];

        int outputCount = 0;

        WriteUInt32(output, ref outputCount, P3DFile.COMPRESSED_SIGNATURE);
        WriteUInt32(output, ref outputCount, fileSize);

        int start = 0;

        while (start < input.Length)
        {
            int size = Math.Min(LZR_BLOCK_SIZE, input.Length - start);

            int compSizePosition = outputCount;

            WriteUInt32(output, ref outputCount, 0);
            WriteUInt32(output, ref outputCount, (uint)size);

            int compStart = outputCount;

            CompressBlock(input, start, (uint)size, output, ref outputCount, fast);

            WriteUInt32(output, compSizePosition, (uint)(outputCount - compStart));

            start += size;
        }

        // Remove unused capacity.
        Array.Resize(ref output, outputCount);

        return output;
    }

    /// <summary>
    /// Converts a <see cref="P3DFile"/> to a compressed <see cref="byte"/> array.
    /// <para>Note: This will ONLY write <c>Little Endian</c> files.</para>
    /// </summary>
    /// <param name="file">The file to compress.</param>
    /// <param name="includeHistory">If <c>true</c>, a history chunk will be added to the start to indicate how and when it was compressed.<para>Defaults to <c>true</c>.</para></param>
    /// <param name="fast">If <c>true</c>, a slightly faster algorithm will be used.<para>Defaults to <c>false</c>.</para></param>
    /// <returns>A <see cref="byte"/> array containing the compressed bytes of <paramref name="file"/>.</returns>
    public static byte[] CompressFile(P3DFile file, bool includeHistory = true, bool fast = false, bool validate = true)
    {
        if (includeHistory)
            file.Chunks.Insert(0, new HistoryChunk(["Compressed with NetP3DLib", $"Run at {DateTime.Now:R}"]));

        using var stream = new MemoryStream();
        file.Write(stream, validate);

        return CompressFile(stream.ToArray(), fast);
    }

    private static void InitTree()
    {
        tree[TREE_ROOT].Larger = 0;
        tree[0].Larger = UNUSED;
        tree[0].Smaller = UNUSED;
    }

    private static bool AddString(byte[] input, int inputStart, uint inputCount, uint inputSize, ref uint offset, ref uint count)
    {
        uint testNode = tree[TREE_ROOT].Larger;
        bool failure = false;
        count = 0;

        while (true)
        {
            uint i;
            int delta = 0;

            int currentPosition = inputStart + (int)inputCount;
            int testPosition = inputStart + (int)testNode;

            for (i = 0; i < inputSize - inputCount; i++)
            {
                delta = input[currentPosition + (int)i] - input[testPosition + (int)i];

                if (delta != 0)
                    break;
            }

            if (i > count)
            {
                if (((inputCount - testNode) & 15) == 0)
                {
                    failure = true;
                }
                else
                {
                    count = i;
                    offset = inputCount - testNode;
                }
            }

            ref uint child = ref (delta >= 0 ? ref tree[(int)testNode].Larger : ref tree[(int)testNode].Smaller);

            if (child == UNUSED)
            {
                child = inputCount;
                tree[(int)inputCount].Larger = UNUSED;
                tree[(int)inputCount].Smaller = UNUSED;

                return failure;
            }

            testNode = child;
        }
    }

    private static void FindLongestMatch(byte[] input, int inputStart, uint inputCount, uint inputSize, ref uint offset, ref uint count)
    {
        offset = 0;
        count = 0;

        int searchEnd = inputStart + (int)inputCount;
        byte firstByte = input[searchEnd];

        int candidate = Array.IndexOf(input, firstByte, inputStart, (int)inputCount);

        while (candidate >= 0)
        {
            uint candidatePosition = (uint)(candidate - inputStart);

            if (((inputCount - candidatePosition) & 15) != 0)
            {
                uint maxLength = Math.Min(inputSize - candidatePosition, inputSize - inputCount);

                uint currentLength = 1;

                int comparePosition = searchEnd + 1;
                int candidateComparePosition = candidate + 1;

                while ((currentLength < maxLength) &&
                       (input[candidateComparePosition++] == input[comparePosition++]))
                {
                    currentLength++;
                }

                if (currentLength > count)
                {
                    count = currentLength;
                    offset = candidatePosition;
                }
            }

            int nextCandidate = candidate + 1;

            if (nextCandidate >= searchEnd)
                break;

            candidate = Array.IndexOf(input, firstByte, nextCandidate, searchEnd - nextCandidate);
        }

        offset = inputCount - offset;
    }

    private static void WriteCount(byte[] output, ref int outputCount, uint count, byte highBits)
    {
        if (count > 15)
        {
            output[outputCount++] = highBits;

            count -= 15;

            while (count > 255)
            {
                output[outputCount++] = 0;
                count -= 255;
            }

            output[outputCount++] = (byte)count;
        }
        else
        {
            output[outputCount++] = (byte)(highBits | count);
        }
    }

    private static void WriteUInt32(byte[] output, ref int outputCount, uint value)
    {
        output[outputCount++] = (byte)value;
        output[outputCount++] = (byte)(value >> 8);
        output[outputCount++] = (byte)(value >> 16);
        output[outputCount++] = (byte)(value >> 24);
    }

    private static void WriteUInt32(byte[] output, int outputIndex, uint value)
    {
        output[outputIndex] = (byte)value;
        output[outputIndex + 1] = (byte)(value >> 8);
        output[outputIndex + 2] = (byte)(value >> 16);
        output[outputIndex + 3] = (byte)(value >> 24);
    }
}
