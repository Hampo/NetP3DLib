using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionEffectChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Effect;
    
    public enum ClassTypes
    {
        WTF,
        Ground,
        PropStatic,
        PropMoveable,
        PropBreakable,
        AnimatedBV,
        Drawable,
        Static,
        PropDrawable,
        PropAnimBreakable,
        PropOnetimeMoveable,
    }

    public ClassTypes ClassType { get; set; }
    public uint PhyPropID { get; set; }
    public string SoundResourceDataName { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes((uint)ClassType));
            data.AddRange(BitConverter.GetBytes(PhyPropID));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(SoundResourceDataName));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(SoundResourceDataName);

    public CollisionEffectChunk(BinaryReader br) : base(ChunkID)
    {
        ClassType = (ClassTypes)br.ReadUInt32();
        PhyPropID = br.ReadUInt32();
        SoundResourceDataName = br.ReadP3DString();
    }

    public CollisionEffectChunk(ClassTypes classType, uint phyPropID, string soundResourceDataName) : base(ChunkID)
    {
        ClassType = classType;
        PhyPropID = phyPropID;
        SoundResourceDataName = soundResourceDataName;
    }

    public override void Validate()
    {
        if (!SoundResourceDataName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(SoundResourceDataName), SoundResourceDataName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write((uint)ClassType);
        bw.Write(PhyPropID);
        bw.WriteP3DString(SoundResourceDataName);
    }
}