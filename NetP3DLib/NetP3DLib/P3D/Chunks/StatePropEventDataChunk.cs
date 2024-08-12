using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.State_Prop_Event_Data)]
public class StatePropEventDataChunk : NamedChunk
{
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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint);

    public StatePropEventDataChunk(BinaryReader br) : base((uint)ChunkIdentifier.State_Prop_Event_Data)
    {
        Name = br.ReadP3DString();
        ToState = br.ReadUInt32();
        Event = (Events)br.ReadUInt32();
    }

    public StatePropEventDataChunk(string name, uint toState, Events @event) : base((uint)ChunkIdentifier.State_Prop_Event_Data)
    {
        Name = name;
        ToState = toState;
        Event = @event;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(ToState);
        bw.Write((uint)Event);
    }
}