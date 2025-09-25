using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class StatePropCallbackDataChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.State_Prop_Callback_Data;
    
    public enum Events : int
    {
        StateChange = -1,
        RemoveFromWorld,
        Spawn5Coins,
        RemoveCollisionVolume,
        FireEnergyBolt,
        KillSpeed,
        Spawn10Coins,
        Spawn15Coins,
        Spawn20Coins,
        RadiateForce,
        EmitLeaves,
        ObjectDestroyed,
        Spawn5CoinsZ,
        Spawn1Coin,
        ColaDestroyed,
        CamShake,
        RemoveFirstCollisionVolume,
        RemoveSecondCollisionVolume,
        RemoveThirdCollisionVolume,
    }

    public Events Event { get; set; }
    public float OnFrame { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes((int)Event));
            data.AddRange(BitConverter.GetBytes(OnFrame));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(int) + sizeof(float);

    public StatePropCallbackDataChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Event = (Events)br.ReadUInt32();
        OnFrame = br.ReadSingle();
    }

    public StatePropCallbackDataChunk(string name, Events @event, float onFrame) : base(ChunkID)
    {
        Name = name;
        Event = @event;
        OnFrame = onFrame;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write((int)Event);
        bw.Write(OnFrame);
    }

    internal override Chunk CloneSelf() => new StatePropCallbackDataChunk(Name, Event, OnFrame);
}