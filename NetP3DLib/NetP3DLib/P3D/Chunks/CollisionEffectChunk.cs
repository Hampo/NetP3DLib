using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Collision_Effect)]
public class CollisionEffectChunk : Chunk
{
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
    public override uint DataLength => sizeof(uint) + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(SoundResourceDataName).Length;

    public CollisionEffectChunk(BinaryReader br) : base((uint)ChunkIdentifier.Collision_Effect)
    {
        ClassType = (ClassTypes)br.ReadUInt32();
        PhyPropID = br.ReadUInt32();
        SoundResourceDataName = br.ReadP3DString();
    }

    public CollisionEffectChunk(ClassTypes classType, uint phyPropID, string soundResourceDataName) : base((uint)ChunkIdentifier.Collision_Effect)
    {
        ClassType = classType;
        PhyPropID = phyPropID;
        SoundResourceDataName = soundResourceDataName;
    }

    public override void Validate()
    {
        if (SoundResourceDataName == null)
            throw new InvalidDataException($"{nameof(SoundResourceDataName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(SoundResourceDataName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(SoundResourceDataName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write((uint)ClassType);
        bw.Write(PhyPropID);
        bw.WriteP3DString(SoundResourceDataName);
    }
}