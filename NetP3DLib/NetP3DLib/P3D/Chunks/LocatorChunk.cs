using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LocatorChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Locator;
    
    public uint LocatorType => TypeData.LocatorType;
    public uint DataLen => TypeData.DataLen;
    public LocatorData TypeData { get; set; }
    public Vector3 Position { get; set; }
    public uint TriggerCount => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Trigger_Volume).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(LocatorType));
            data.AddRange(BitConverter.GetBytes(DataLen));
            foreach (var item in TypeData.DataArray)
                data.AddRange(BitConverter.GetBytes(item));
            data.AddRange(BinaryExtensions.GetBytes(Position));
            data.AddRange(BitConverter.GetBytes(TriggerCount));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) * DataLen + sizeof(float) * 3 + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public LocatorChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        var type = br.ReadUInt32();
        var len = br.ReadInt32();
        List<uint> data = new(len);
        for (int i = 0; i < len; i++)
            data.Add(br.ReadUInt32());
        TypeData = type switch
        {
            0 => new Type0LocatorData(data),
            1 => new Type1LocatorData(data),
            2 => new Type2LocatorData(),
            3 => new Type3LocatorData(data),
            4 => new Type4LocatorData(),
            5 => new Type5LocatorData(data),
            6 => new Type6LocatorData(data),
            7 => new Type7LocatorData(data),
            8 => new Type8LocatorData(data),
            9 => new Type9LocatorData(data),
            10 => new Type10LocatorData(data),
            11 => new Type11LocatorData(),
            12 => new Type12LocatorData(data),
            13 => new Type13LocatorData(data),
            14 => new Type14LocatorData(),
            _ => new UnknownLocatorData(type, data)
        };
        Position = br.ReadVector3();
        var triggerCount = br.ReadUInt32();
    }

    public LocatorChunk(string name, LocatorData typeData, Vector3 position) : base(ChunkID)
    {
        Name = name;
        TypeData = typeData;
        Position = position;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(LocatorType);
        bw.Write(DataLen);
        TypeData.WriteData(bw);
        bw.Write(Position);
        bw.Write(TriggerCount);
    }

    public abstract class LocatorData
    {
        public uint LocatorType { get; }
        public virtual uint DataLen => (uint)DataArray.Count;
        public abstract List<uint> DataArray { get; }

        public LocatorData(uint locatorType)
        {
            LocatorType = locatorType;
        }

        public void WriteData(BinaryWriter bw)
        {
            foreach (var item in DataArray)
                bw.Write(item);
        }

        internal static (string String, int Index) ParseDataString(List<uint> data, int offset = 0)
        {
            StringBuilder result = new();
            int index = offset;

            while (index < data.Count)
            {
                uint value = data[index];
                if (value == 0)
                    break;

                for (int i = 0; i < 4; i++)
                {
                    char ch = (char)(value & 0xFF);
                    if (ch == '\0')
                        return (result.ToString(), index + 1);

                    result.Append(ch);
                    value >>= 8;
                }

                index++;
            }

            return (result.ToString(), index + 1);
        }

        internal static float ParseDataFloat(uint data) => BitConverter.ToSingle(BitConverter.GetBytes(data), 0);

        internal static List<uint> CreateStringData(string input)
        {
            int diff = input.Length % 4;
            string str = $"{input}{new string('\0', 4 - diff)}";

            List<uint> result = [];
            int length = str.Length;
            for (int i = 0; i < length; i += 4)
            {
                uint value = 0;
                for (int j = 0; j < 4 && i + j < length; j++)
                {
                    value |= (uint)(str[i + j] << (8 * j));
                }
                result.Add(value);
            }
            return result;
        }

        internal static uint CreateFloatData(float input) => BitConverter.ToUInt32(BitConverter.GetBytes(input), 0);
    }

    public class UnknownLocatorData : LocatorData
    {
        public List<uint> Data { get; } = [];
        public override List<uint> DataArray => Data;

        public UnknownLocatorData(uint locatorType, IList<uint> data) : base(locatorType)
        {
            Data.AddRange(data);
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", Data)}]";
        }
    }
    
    /// <summary>
    /// Event locator data.
    /// </summary>
    public class Type0LocatorData : LocatorData
    {
        public enum Events : uint
        {
            Flag = 0,
            CameraCut = 1,
            CheckPoint = 2,
            Base = 3,
            Death = 4,
            InteriorExit = 5,
            Bouncepad = 6,
            AmbientSoundCity = 7,
            AmbientSoundSeaside = 8,
            AmbientSoundSuburbs = 9,
            AmbientSoundForest = 10,
            AmbientKwikEMartRooftop = 11,
            AmbientSoundFarm = 12,
            AmbientSoundBarn = 13,
            AmbientSoundPowerplantExterior = 14,
            AmbientSoundPowerplantInterior = 15,
            AmbientSoundRiver = 16,
            AmbientSoundCityBusiness = 17,
            AmbientSoundConstruction = 18,
            AmbientSoundStadium = 19,
            AmbientSoundStadiumTunnel = 20,
            AmbientSoundExpressway = 21,
            AmbientSoundSlum = 22,
            AmbientSoundRailyard = 23,
            AmbientSoundHospital = 24,
            AmbientSoundLightCity = 25,
            AmbientSoundShipyard = 26,
            AmbientSoundQuay = 27,
            AmbientSoundLighthouse = 28,
            AmbientSoundCountryHighway = 29,
            AmbientSoundKrustylu = 30,
            AmbientSoundDam = 31,
            AmbientSoundForestHighway = 32,
            AmbientSoundRetainingWallTunnel = 33,
            AmbientSoundKrustyluExterior = 34,
            AmbientSoundDuffExterior = 35,
            AmbientSoundDuffInterior = 36,
            AmbientSoundStoneCutterTunnel = 37,
            AmbientSoundStoneCutterHall = 38,
            AmbientSoundSewers = 39,
            AmbientSoundBurnsTunnel = 40,
            AmbientSoundPPRoom1 = 41,
            AmbientSoundPPRoom2 = 42,
            AmbientSoundPPRoom3 = 43,
            AmbientSoundPPTunnel1 = 44,
            AmbientSoundPPTunnel2 = 45,
            AmbientSoundMansionInterior = 46,
            ParkedBirds = 47,
            WhackyGravity = 48,
            FarPlane = 49,
            AmbientSoundCountryNight = 50,
            AmbientSoundSuburbsNight = 51,
            AmbientSoundForestNight = 52,
            AmbientSoundHalloween1 = 53,
            AmbientSoundHalloween2 = 54,
            AmbientSoundHalloween3 = 55,
            AmbientSoundPlaceholder3 = 56,
            AmbientSoundPlaceholder4 = 57,
            AmbientSoundPlaceholder5 = 58,
            AmbientSoundPlaceholder6 = 59,
            AmbientSoundPlaceholder7 = 60,
            AmbientSoundPlaceholder8 = 61,
            AmbientSoundPlaceholder9 = 62,
            GooDamage = 63,
            CoinZone = 64,
            LightChange = 65,
            Trap = 66,
            AmbientSoundSeasideNight = 67,
            AmbientSoundLighthouseNight = 68,
            AmbientSoundBreweryNight = 69,
            AmbientSoundPlaceholder10 = 70,
            AmbientSoundPlaceholder11 = 71,
            AmbientSoundPlaceholder12 = 72,
            AmbientSoundPlaceholder13 = 73,
            AmbientSoundPlaceholder14 = 74,
            AmbientSoundPlaceholder15 = 75,
            AmbientSoundPlaceholder16 = 76,
            Special = 77,
            OcclusionZone = 78,
            CarDoor = 79,
            ActionButton = 80,
            InteriorEntrance = 81,
            GenericButtonHandlerEvent = 82,
            FountainJump = 83,
            LoadPedModelGroup = 84,
            Gag = 85,
        }

        public Events Event { get; set; }
        public uint? Parameter { get; set; }

        public override uint DataLen => Parameter.HasValue ? 2u : 1u;
        public override List<uint> DataArray
        {
            get
            {
                List<uint> data =
                [
                    (uint)Event,
                ];
                if (Parameter.HasValue)
                    data.Add(Parameter.Value);

                return [.. data];
            }
        }

        public Type0LocatorData(List<uint> data) : base(0)
        {
            Event = (Events)data[0];
            if (data.Count > 1)
                Parameter = data[1];
        }

        public Type0LocatorData(Events @event) : base(0)
        {
            Event = @event;
            Parameter = null;
        }

        public Type0LocatorData(Events @event, uint parameter) : base(0)
        {
            Event = @event;
            Parameter = parameter;
        }

        public override string ToString()
        {
            return $"Event = {Event}, Parameter = {Parameter?.ToString() ?? "null"}";
        }
    }

    /// <summary>
    /// Script locator data.
    /// </summary>
    public class Type1LocatorData : LocatorData
    {
        public string Key { get; set; }

        public override List<uint> DataArray => CreateStringData(Key);

        public Type1LocatorData(List<uint> data) : base(1)
        {
            Key = ParseDataString(data).String;
        }

        public Type1LocatorData(string key) : base(1)
        {
            Key = key;
        }

        public override string ToString()
        {
            return $"Key = {Key}";
        }
    }

    /// <summary>
    /// Generic locator data.
    /// </summary>
    public class Type2LocatorData : LocatorData
    {
        public override uint DataLen => 0;

        public override List<uint> DataArray => [];

        public Type2LocatorData() : base(2)
        { }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Car Start locator data.
    /// </summary>
    public class Type3LocatorData : LocatorData
    {
        public float Rotation { get; set; }
        public uint? ParkedCar { get; set; }
        public string FreeCar { get; set; }

        public override List<uint> DataArray
        {
            get
            {
                List<uint> data =
                [
                    CreateFloatData(Rotation),
                ];
                if (ParkedCar.HasValue)
                {
                    data.Add(ParkedCar.Value);
                    if (FreeCar != null)
                        data.AddRange(CreateStringData(FreeCar));
                }

                return [.. data];
            }
        }

        public Type3LocatorData(List<uint> data) : base(3)
        {
            Rotation = ParseDataFloat(data[0]);
            ParkedCar = data.Count > 1 ? data[1] : null;
            FreeCar = data.Count > 2 ? ParseDataString(data, 2).String : null;
        }

        public Type3LocatorData(float rotation) : base(3)
        {
            Rotation = rotation;
            ParkedCar = null;
            FreeCar = null;
        }

        public Type3LocatorData(float rotation, uint parkedCar) : base(3)
        {
            Rotation = rotation;
            ParkedCar = parkedCar;
            FreeCar = null;
        }

        public Type3LocatorData(float rotation, uint parkedCar, string freeCar) : base(3)
        {
            Rotation = rotation;
            ParkedCar = parkedCar;
            FreeCar = freeCar;
        }

        public override string ToString()
        {
            return $"Rotation = {Rotation}, ParkedCar = {ParkedCar?.ToString() ?? "null"}, FreeCar = {FreeCar}";
        }
    }

    /// <summary>
    /// Spline locator data.
    /// </summary>
    public class Type4LocatorData : LocatorData
    {
        public override uint DataLen => 0;

        public override List<uint> DataArray => [];

        public Type4LocatorData() : base(4)
        { }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Dynamic Zone locator data.
    /// </summary>
    public class Type5LocatorData : LocatorData
    {
        public string DynaLoadData { get; set; }

        public override List<uint> DataArray => CreateStringData(DynaLoadData);

        public Type5LocatorData(List<uint> data) : base(5)
        {
            DynaLoadData = ParseDataString(data).String;
        }

        public Type5LocatorData(string dynaLoadData) : base(5)
        {
            DynaLoadData = dynaLoadData;
        }

        public override string ToString()
        {
            return DynaLoadData;
        }
    }

    /// <summary>
    /// Occlusion locator data.
    /// </summary>
    public class Type6LocatorData : LocatorData
    {
        public uint? Occlusions { get; set; }
        public override uint DataLen => Occlusions.HasValue ? 1u : 0u;
        public override List<uint> DataArray => Occlusions.HasValue ? [Occlusions.Value] : [];

        public Type6LocatorData(List<uint> data) : base(6)
        {
            Occlusions = data.Count > 0 ? data[0] : null;
        }

        public Type6LocatorData() : base(6)
        { }

        public Type6LocatorData(uint occlusions) : base(6)
        {
            Occlusions = occlusions;
        }

        public override string ToString()
        {
            return $"Occlusions = {Occlusions?.ToString() ?? "null"}";
        }
    }

    /// <summary>
    /// Interior Entrance locator data.
    /// </summary>
    public class Type7LocatorData : LocatorData
    {
        public string InteriorName { get; set; }
        public Vector3 Right { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Front { get; set; }

        public override List<uint> DataArray
        {
            get
            {
                List<uint> data = [];

                data.AddRange(CreateStringData(InteriorName));
                data.Add(CreateFloatData(Right.X));
                data.Add(CreateFloatData(Right.Y));
                data.Add(CreateFloatData(Right.Z));
                data.Add(CreateFloatData(Up.X));
                data.Add(CreateFloatData(Up.Y));
                data.Add(CreateFloatData(Up.Z));
                data.Add(CreateFloatData(Front.X));
                data.Add(CreateFloatData(Front.Y));
                data.Add(CreateFloatData(Front.Z));

                return [.. data];
            }
        }

        public Type7LocatorData(List<uint> data) : base(7)
        {
            var interiorName = ParseDataString(data);
            InteriorName = interiorName.String;
            var index = interiorName.Index;
            Right = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            Up = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            Front = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
        }

        public Type7LocatorData(string interiorName, Vector3 right, Vector3 up, Vector3 front) : base(7)
        {
            InteriorName = interiorName;
            Right = right;
            Up = up;
            Front = front;
        }

        public override string ToString()
        {
            return $"InteriorName = {InteriorName}, Right = {Right}, Up = {Up}, Front = {Front}";
        }
    }

    /// <summary>
    /// Directional locator data.
    /// </summary>
    public class Type8LocatorData : LocatorData
    {
        public Vector3 Right { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Front { get; set; }

        public override uint DataLen => 9;
        public override List<uint> DataArray =>
        [
            CreateFloatData(Right.X),
            CreateFloatData(Right.Y),
            CreateFloatData(Right.Z),
            CreateFloatData(Up.X),
            CreateFloatData(Up.Y),
            CreateFloatData(Up.Z),
            CreateFloatData(Front.X),
            CreateFloatData(Front.Y),
            CreateFloatData(Front.Z),
        ];

        public Type8LocatorData(List<uint> data) : base(8)
        {
            var index = 0;
            Right = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            Up = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            Front = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
        }

        public Type8LocatorData(Vector3 right, Vector3 up, Vector3 front) : base(8)
        {
            Right = right;
            Up = up;
            Front = front;
        }

        public override string ToString()
        {
            return $"Right = {Right}, Up = {Up}, Front = {Front}";
        }
    }

    /// <summary>
    /// Action locator data.
    /// </summary>
    public class Type9LocatorData : LocatorData
    {
        public string ObjectName { get; set; }
        public string JointName { get; set; }
        public string ActionName { get; set; }
        public uint ButtonInput { get; set; }
        public uint ShouldTransform { get; set; }

        public override List<uint> DataArray
        {
            get
            {
                List<uint> data = [];

                data.AddRange(CreateStringData(ObjectName));
                data.AddRange(CreateStringData(JointName));
                data.AddRange(CreateStringData(ActionName));
                data.Add(ButtonInput);
                data.Add(ShouldTransform);

                return [.. data];
            }
        }

        public Type9LocatorData(List<uint> data) : base(9)
        {
            var objectName = ParseDataString(data);
            ObjectName = objectName.String;
            var jointName = ParseDataString(data, objectName.Index);
            JointName = jointName.String;
            var actionName = ParseDataString(data, jointName.Index);
            ActionName = actionName.String;
            var index = actionName.Index;
            ButtonInput = data[index++];
            ShouldTransform = data[index++];
        }

        public Type9LocatorData(string objectName, string jointName, string actionName, uint buttonInput, uint shouldTransform) : base(9)
        {
            ObjectName = objectName;
            JointName = jointName;
            ActionName = actionName;
            ButtonInput = buttonInput;
            ShouldTransform = shouldTransform;
        }

        public override string ToString()
        {
            return $"ObjectName = {ObjectName}, JointName = {JointName}, ActionName = {ActionName}, ButtonInput = {ButtonInput}, ShouldTransform = {ShouldTransform}";
        }
    }

    /// <summary>
    /// FOV locator data.
    /// </summary>
    public class Type10LocatorData : LocatorData
    {
        public float FOV { get; set; }
        public float Type { get; set; }
        public float Rate { get; set; }

        public override uint DataLen => 3;
        public override List<uint> DataArray => [
            CreateFloatData(FOV),
            CreateFloatData(Type),
            CreateFloatData(Rate),
        ];

        public Type10LocatorData(List<uint> data) : base(10)
        {
            FOV = ParseDataFloat(data[0]);
            Type = ParseDataFloat(data[1]);
            Rate = ParseDataFloat(data[2]);
        }

        public Type10LocatorData(float fov, float type, float rate) : base(10)
        {
            FOV = fov;
            Type = type;
            Rate = rate;
        }

        public override string ToString()
        {
            return $"FOV = {FOV}, Type = {Type}, Rate = {Rate}";
        }
    }

    /// <summary>
    /// Breakable Camera locator data.
    /// </summary>
    public class Type11LocatorData : LocatorData
    {
        public override uint DataLen => 0;
        public override List<uint> DataArray => [];

        public Type11LocatorData() : base(11)
        { }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Static Camera locator data.
    /// </summary>
    public class Type12LocatorData : LocatorData
    {
        public Vector3 TargetPosition { get; set; }
        public float FOV { get; set; }
        public float TargetLag { get; set; }
        public uint FollowPlayer { get; set; }
        public float? TransitionTargetRate { get; set; }
        public uint? Flags { get; set; }
        public uint? CutInOut { get; set; }
        public uint? Data { get; set; }

        public override List<uint> DataArray
        {
            get
            {
                List<uint> data = [];

                data.Add(CreateFloatData(TargetPosition.X));
                data.Add(CreateFloatData(TargetPosition.Y));
                data.Add(CreateFloatData(TargetPosition.Z));
                data.Add(CreateFloatData(FOV));
                data.Add(CreateFloatData(TargetLag));
                data.Add(FollowPlayer);

                if (!TransitionTargetRate.HasValue)
                    return [.. data];
                data.Add(CreateFloatData(TransitionTargetRate.Value));

                if (!Flags.HasValue)
                    return [.. data];
                data.Add(Flags.Value);

                if (!CutInOut.HasValue || !Data.HasValue)
                    return [.. data];
                data.Add(CutInOut.Value);
                data.Add(Data.Value);

                return [.. data];
            }
        }

        public Type12LocatorData(List<uint> data) : base(12)
        {
            TargetPosition = new(ParseDataFloat(data[0]), ParseDataFloat(data[1]), ParseDataFloat(data[2]));
            FOV = ParseDataFloat(data[3]);
            TargetLag = ParseDataFloat(data[4]);
            FollowPlayer = data[5];
            TransitionTargetRate = data.Count > 6 ? ParseDataFloat(data[6]) : null;
            Flags = data.Count > 7 ? data[7] : null;
            CutInOut = data.Count > 8 ? data[8] : null;
            Data = data.Count > 9 ? data[9] : null;
        }

        public Type12LocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer) : base(12)
        {
            TargetPosition = targetPosition;
            FOV = fov;
            TargetLag = targetLag;
            FollowPlayer = followPlayer;
            TransitionTargetRate = null;
            Flags = null;
            CutInOut = null;
            Data = null;
        }

        public Type12LocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer, float transitionTargetRate) : base(12)
        {
            TargetPosition = targetPosition;
            FOV = fov;
            TargetLag = targetLag;
            FollowPlayer = followPlayer;
            TransitionTargetRate = transitionTargetRate;
            Flags = null;
            CutInOut = null;
            Data = null;
        }

        public Type12LocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer, float transitionTargetRate, uint flags) : base(12)
        {
            TargetPosition = targetPosition;
            FOV = fov;
            TargetLag = targetLag;
            FollowPlayer = followPlayer;
            TransitionTargetRate = transitionTargetRate;
            Flags = flags;
            CutInOut = null;
            Data = null;
        }

        public Type12LocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer, float transitionTargetRate, uint flags, uint cutInOut, uint data) : base(12)
        {
            TargetPosition = targetPosition;
            FOV = fov;
            TargetLag = targetLag;
            FollowPlayer = followPlayer;
            TransitionTargetRate = transitionTargetRate;
            Flags = flags;
            CutInOut = cutInOut;
            Data = data;
        }

        public override string ToString()
        {
            return $"TargetPosition = {TargetPosition}, FOV = {FOV}, TargetLag = {TargetLag}, FollowPlayer = {FollowPlayer}, TransitionTargetRate = {TransitionTargetRate?.ToString() ?? "null"}, Flags = {Flags?.ToString() ?? "null"}, CutInOut = {CutInOut?.ToString() ?? "null"}, Data = {Data?.ToString() ?? "null"}";
        }
    }

    /// <summary>
    /// Ped Group locator data.
    /// </summary>
    public class Type13LocatorData : LocatorData
    {
        public uint GroupNum { get; set; }

        public override uint DataLen => 1;
        public override List<uint> DataArray => [GroupNum];

        public Type13LocatorData(List<uint> data) : base(13)
        {
            GroupNum = data[0];
        }

        public Type13LocatorData(uint groupNum) : base(13)
        {
            GroupNum = groupNum;
        }

        public override string ToString()
        {
            return $"GroupNum = {GroupNum}";
        }
    }

    /// <summary>
    /// Coin locator data.
    /// </summary>
    public class Type14LocatorData : LocatorData
    {
        public override uint DataLen => 0;
        public override List<uint> DataArray => [];

        public Type14LocatorData() : base(14)
        { }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}