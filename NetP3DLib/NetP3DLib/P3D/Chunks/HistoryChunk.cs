using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class HistoryChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.History;
    public const int MAX_HISTORY_LINES = 256;
    
    public ushort NumHistory
    {
        get => (ushort)History.Count;
        set
        {
            if (value == NumHistory)
                return;

            if (value < NumHistory)
            {
                while (NumHistory > value)
                    History.RemoveAt(History.Count - 1);
            }
            else
            {
                while (NumHistory < value)
                    History.Add(string.Empty);
            }
            RecalculateSize();
        }
    }
    public SizeAwareList<string> History { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];
            
            data.AddRange(BitConverter.GetBytes(NumHistory));
            foreach (string item in History)
                data.AddRange(BinaryExtensions.GetP3DStringBytes(item));

            return [.. data];
        }
    }
    public override uint DataLength
    {
        get
        {
            uint size = sizeof(ushort);
            foreach (var item in History)
                size += BinaryExtensions.GetP3DStringLength(item);
            return size;
        }
    }

    public HistoryChunk(BinaryReader br) : base(ChunkID)
    {
        ushort lineCount = br.ReadUInt16();
        History = CreateSizeAwareList<string>(lineCount);
        History.SuspendNotifications();
        for (int i = 0; i < lineCount; i++)
            History.Add(br.ReadP3DString());
        History.ResumeNotifications();
    }

    public HistoryChunk(IList<string> history) : base(ChunkID)
    {
        History = CreateSizeAwareList<string>(history.Count);
        History.SuspendNotifications();
        History.AddRange(history);
        History.ResumeNotifications();
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (History.Count > MAX_HISTORY_LINES)
            yield return new InvalidP3DException(this, $"The max number of history lines is {MAX_HISTORY_LINES}.");

        foreach (var history in History)
            if (!history.IsValidP3DString())
                yield return new InvalidP3DStringException(this, nameof(History), history);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumHistory);
        foreach (string item in History)
            bw.WriteP3DString(item);
    }

    protected override Chunk CloneSelf() => new HistoryChunk(History);

    public override string ToString() => $"\"{(History.Count == 0 ? "" : History[0])}\" ({GetChunkType(this)} (0x{ID:X}))";
}
