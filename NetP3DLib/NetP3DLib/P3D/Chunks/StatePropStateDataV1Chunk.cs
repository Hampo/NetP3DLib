using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class StatePropStateDataV1Chunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.State_Prop_State_Data_V1;
    
    public uint AutoTransition { get; set; }
    public uint OutState { get; set; }
    public uint NumDrawables => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.State_Prop_Visibilities_Data).Count();
    public uint NumFrameControllers => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.State_Prop_Frame_Controller_Data).Count();
    public uint NumEvents => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.State_Prop_Event_Data).Count();
    public uint NumCallbacks => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.State_Prop_Callback_Data).Count();
    public float OutFrame { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(AutoTransition));
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
        AutoTransition = br.ReadUInt32();
        OutState = br.ReadUInt32();
        var numDrawables = br.ReadUInt32();
        var numFrameControllers = br.ReadUInt32();
        var numEvents = br.ReadUInt32();
        var numCallbacks = br.ReadUInt32();
        OutFrame = br.ReadSingle();
    }

    public StatePropStateDataV1Chunk(string name, uint autoTransition, uint outState, float outFrame) : base(ChunkID)
    {
        Name = name;
        AutoTransition = autoTransition;
        OutState = outState;
        OutFrame = outFrame;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(AutoTransition);
        bw.Write(OutState);
        bw.Write(NumDrawables);
        bw.Write(NumFrameControllers);
        bw.Write(NumEvents);
        bw.Write(NumCallbacks);
        bw.Write(OutFrame);
    }

    private static readonly HashSet<uint> ChunkSortPriority = [
        (uint)ChunkIdentifier.State_Prop_Visibilities_Data,
        (uint)ChunkIdentifier.State_Prop_Frame_Controller_Data,
        (uint)ChunkIdentifier.State_Prop_Event_Data,
        (uint)ChunkIdentifier.State_Prop_Callback_Data,
    ];
    /// <summary>
    /// Children must be in the order: <c>Visibilities</c>; <c>Frame Controllers</c>; <c>Events</c>; <c>Callbacks</c>.
    /// <para>This function sort child chunks in the correct order.</para>
    /// </summary>
    public void SortChildren()
    {
        List<Chunk> newChildren = new(Children.Count);

        foreach (uint id in ChunkSortPriority)
            newChildren.AddRange(Children.Where(x => x.ID == id));

        newChildren.AddRange(Children.Where(x => !ChunkSortPriority.Contains(x.ID)));

        Children.Clear();
        Children.AddRange(newChildren);
    }
}