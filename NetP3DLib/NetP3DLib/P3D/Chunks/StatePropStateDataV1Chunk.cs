using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class StatePropStateDataV1Chunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.State_Prop_State_Data_V1;

    private uint autoTransition;
    public bool AutoTransition
    {
        get => autoTransition != 0;
        set => autoTransition = value ? 1u : 0u;
    }
    public uint OutState { get; set; }
    public uint NumDrawables => GetChildCount(ChunkIdentifier.State_Prop_Visibilities_Data);
    public uint NumFrameControllers => GetChildCount(ChunkIdentifier.State_Prop_Frame_Controller_Data);
    public uint NumEvents => GetChildCount(ChunkIdentifier.State_Prop_Event_Data);
    public uint NumCallbacks => GetChildCount(ChunkIdentifier.State_Prop_Callback_Data);
    public float OutFrame { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(autoTransition));
            data.AddRange(BitConverter.GetBytes(OutState));
            data.AddRange(BitConverter.GetBytes(NumDrawables));
            data.AddRange(BitConverter.GetBytes(NumFrameControllers));
            data.AddRange(BitConverter.GetBytes(NumEvents));
            data.AddRange(BitConverter.GetBytes(NumCallbacks));
            data.AddRange(BitConverter.GetBytes(OutFrame));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public StatePropStateDataV1Chunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        autoTransition = br.ReadUInt32();
        OutState = br.ReadUInt32();
        var numDrawables = br.ReadUInt32();
        var numFrameControllers = br.ReadUInt32();
        var numEvents = br.ReadUInt32();
        var numCallbacks = br.ReadUInt32();
        OutFrame = br.ReadSingle();
    }

    public StatePropStateDataV1Chunk(string name, bool autoTransition, uint outState, float outFrame) : base(ChunkID)
    {
        Name = name;
        AutoTransition = autoTransition;
        OutState = outState;
        OutFrame = outFrame;
    }

    public override void Validate()
    {
        if (Children.Count == 0)
            throw new InvalidDataException($"There must be at least one child chunk.");

        var currentIndex = 0;
        foreach (var child in Children)
        {
            var expectedIndex = ChunkSortPriority.IndexOf(child.ID);

            if (expectedIndex == -1)
                throw new InvalidDataException($"Invalid child chunk: {child}.");

            if (expectedIndex < currentIndex)
                throw new InvalidDataException($"Child chunk {child} is out of order. Expected order: {string.Join(", ", ChunkSortPriority)}.");

            currentIndex = expectedIndex;
        }

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(autoTransition);
        bw.Write(OutState);
        bw.Write(NumDrawables);
        bw.Write(NumFrameControllers);
        bw.Write(NumEvents);
        bw.Write(NumCallbacks);
        bw.Write(OutFrame);
    }

    internal override Chunk CloneSelf() => new StatePropStateDataV1Chunk(Name, AutoTransition, OutState, OutFrame);

    private static readonly List<uint> ChunkSortPriority = [
        (uint)ChunkIdentifier.State_Prop_Visibilities_Data,
        (uint)ChunkIdentifier.State_Prop_Frame_Controller_Data,
        (uint)ChunkIdentifier.State_Prop_Event_Data,
        (uint)ChunkIdentifier.State_Prop_Callback_Data,
    ];
    /// <summary>
    /// Children must be in the order: <c>Visibilities</c>; <c>Frame Controllers</c>; <c>Events</c>; <c>Callbacks</c>.
    /// <para>This function sorts child chunks into the correct order.</para>
    /// </summary>
    public void SortChildren()
    {
        List<Chunk> newChildren = new(Children.Count);

        foreach (uint id in ChunkSortPriority)
            foreach (var child in Children)
                if (child.ID == id)
                    newChildren.Add(child);

        foreach (var child in Children)
            if (!ChunkSortPriority.Contains(child.ID))
                Children.Add(child);

        Children.Clear();
        Children.AddRange(newChildren);
    }
}