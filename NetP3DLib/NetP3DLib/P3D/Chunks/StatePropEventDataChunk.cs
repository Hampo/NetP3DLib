using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class StatePropEventDataChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.State_Prop_Event_Data;
    
    public enum Events : uint
    {
        Idle,
        FadeIn,
        FadeOut,
        Moving,
        AttackCharging,
        AttackCharged,
        Attacking,
        Destroyed,
        Hit,
        FeatherTouch,
        Stomp,
        VehicleHit,
    }
    
    public uint ToState { get; set; }
    public Events Event { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(ToState));
            data.AddRange(BitConverter.GetBytes((uint)Event));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    public StatePropEventDataChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        ToState = br.ReadUInt32();
        Event = (Events)br.ReadUInt32();
    }

    public StatePropEventDataChunk(string name, uint toState, Events @event) : base(ChunkID)
    {
        Name = name;
        ToState = toState;
        Event = @event;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(ToState);
        bw.Write((uint)Event);
    }

    internal override Chunk CloneSelf() => new StatePropEventDataChunk(Name, ToState, Event);
}