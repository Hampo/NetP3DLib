using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LocatorChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Locator;

    public enum LocatorTypes : uint
    {
        Event,
        Script,
        Generic,
        CarStart,
        Spline,
        DynamicZone,
        Occlusion,
        InteriorEntrance,
        Directional,
        Action,
        FOV,
        BreakableCamera,
        StaticCamera,
        PedGroup,
        Coin,
    }
    
    public LocatorTypes LocatorType => TypeData.LocatorType;
    public uint DataLen => TypeData.DataLen;
    private LocatorData _typeData = new UnknownLocatorData(15, []);
    public LocatorData TypeData
    {
        get => _typeData;
        set
        {
            _typeData.SizeChanged -= TypeData_SizeChanged;
            _typeData = value;
            value.SizeChanged += TypeData_SizeChanged;
        }
    }
    public Vector3 Position { get; set; }
    public uint TriggerCount => GetChildCount(ChunkIdentifier.Trigger_Volume);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes((uint)LocatorType));
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
        var type = (LocatorTypes)br.ReadUInt32();
        var len = br.ReadInt32();
        List<uint> data = new(len);
        for (int i = 0; i < len; i++)
            data.Add(br.ReadUInt32());
        TypeData = type switch
        {
            LocatorTypes.Event => new EventLocatorData(data),
            LocatorTypes.Script => new ScriptLocatorData(data),
            LocatorTypes.Generic => new GenericLocatorData(),
            LocatorTypes.CarStart => new CarStartLocatorData(data),
            LocatorTypes.Spline => new SplineLocatorData(),
            LocatorTypes.DynamicZone => new DynamicZoneLocatorData(data),
            LocatorTypes.Occlusion => new OcclusionLocatorData(data),
            LocatorTypes.InteriorEntrance => new InteriorEntranceLocatorData(data),
            LocatorTypes.Directional => new DirectionalLocatorData(data),
            LocatorTypes.Action => new ActionLocatorData(data),
            LocatorTypes.FOV => new FOVLocatorData(data),
            LocatorTypes.BreakableCamera => new BreakableCameraLocatorData(),
            LocatorTypes.StaticCamera => new StaticCameraLocatorData(data),
            LocatorTypes.PedGroup => new PedGroupLocatorData(data),
            LocatorTypes.Coin => new CoinLocatorData(),
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

    private void TypeData_SizeChanged()
    {
        int delta = checked((int)(Size - _cachedSize));
        _cachedSize = Size;
        OnSizeChanged(delta);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write((uint)LocatorType);
        bw.Write(DataLen);
        TypeData.WriteData(bw);
        bw.Write(Position);
        bw.Write(TriggerCount);
    }

    public override string ToString() => $"\"{Name}\" ({LocatorType} {GetChunkType(this)} (Type {(int)LocatorType}) (0x{ID:X}))";

    protected override Chunk CloneSelf() => new LocatorChunk(Name, TypeData.Clone(), Position);

    public abstract class LocatorData
    {
        public event Action? SizeChanged;
        protected void OnSizeChanged() => SizeChanged?.Invoke();


        public LocatorTypes LocatorType { get; }
        public virtual uint DataLen => (uint)DataArray.Count;
        public abstract List<uint> DataArray { get; }

        public LocatorData(LocatorTypes locatorType)
        {
            LocatorType = locatorType;
        }

        public void WriteData(BinaryWriter bw)
        {
            foreach (var item in DataArray)
                bw.Write(item);
        }

        internal abstract LocatorData Clone();

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
        public SizeAwareList<uint> Data { get; }
        public override List<uint> DataArray => [..Data];

        public UnknownLocatorData(LocatorTypes locatorType, IList<uint> data) : base(locatorType)
        {
            Data = new SizeAwareList<uint>(OnSizeChanged, data.Count);
            Data.AddRange(data);
        }

        public UnknownLocatorData(uint locatorType, IList<uint> data) : this((LocatorTypes)locatorType, data)
        {}

        internal override LocatorData Clone() => new UnknownLocatorData(LocatorType, Data);

        public override string ToString() => $"[{string.Join(", ", Data)}]";
    }
    
    /// <summary>
    /// Event locator data.
    /// </summary>
    public class EventLocatorData : LocatorData
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
        private uint? _parameter = null;
        public uint? Parameter
        {
            get => _parameter;
            set
            {
                _parameter = value;
                OnSizeChanged();
            }
        }

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

        public EventLocatorData(List<uint> data) : base(LocatorTypes.Event)
        {
            Event = (Events)data[0];
            if (data.Count > 1)
                Parameter = data[1];
        }

        public EventLocatorData(Events @event) : base(LocatorTypes.Event)
        {
            Event = @event;
            Parameter = null;
        }

        public EventLocatorData(Events @event, uint parameter) : base(LocatorTypes.Event)
        {
            Event = @event;
            Parameter = parameter;
        }

        internal override LocatorData Clone() => Parameter.HasValue ? new EventLocatorData(Event, Parameter.Value) : new EventLocatorData(Event);

        public override string ToString() => $"Event = {Event}, Parameter = {Parameter?.ToString() ?? "null"}";
    }

    /// <summary>
    /// Script locator data.
    /// </summary>
    public class ScriptLocatorData : LocatorData
    {
        private string _key = string.Empty;
        public string Key
        {
            get => _key;
            set
            {
                if (_key == value)
                    return;

                _key = value;
                OnSizeChanged();
            }
        }

        public override List<uint> DataArray => CreateStringData(Key);

        public ScriptLocatorData(List<uint> data) : base(LocatorTypes.Script)
        {
            Key = ParseDataString(data).String;
        }

        public ScriptLocatorData(string key) : base(LocatorTypes.Script)
        {
            Key = key;
        }

        internal override LocatorData Clone() => new ScriptLocatorData(Key);

        public override string ToString() => $"Key = {Key}";
    }

    /// <summary>
    /// Generic locator data.
    /// </summary>
    public class GenericLocatorData : LocatorData
    {
        public override uint DataLen => 0;

        public override List<uint> DataArray => [];

        public GenericLocatorData() : base(LocatorTypes.Generic)
        { }

        internal override LocatorData Clone() => new GenericLocatorData();

        public override string ToString() => string.Empty;
    }

    /// <summary>
    /// Car Start locator data.
    /// </summary>
    public class CarStartLocatorData : LocatorData
    {
        public float Rotation { get; set; }
        private uint? _parkedCar = null;
        public uint? ParkedCar
        {
            get => _parkedCar;
            set
            {
                _parkedCar = value;
                OnSizeChanged();
            }
        }
        private string? _freeCar = null;
        public string? FreeCar
        {
            get => _freeCar;
            set
            {
                if (_freeCar == value)
                    return;

                _freeCar = value;
                OnSizeChanged();
            }
        }

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

        public CarStartLocatorData(List<uint> data) : base(LocatorTypes.CarStart)
        {
            Rotation = ParseDataFloat(data[0]);
            ParkedCar = data.Count > 1 ? data[1] : null;
            FreeCar = data.Count > 2 ? ParseDataString(data, 2).String : null;
        }

        public CarStartLocatorData(float rotation) : base(LocatorTypes.CarStart)
        {
            Rotation = rotation;
            ParkedCar = null;
            FreeCar = null;
        }

        public CarStartLocatorData(float rotation, uint parkedCar) : base(LocatorTypes.CarStart)
        {
            Rotation = rotation;
            ParkedCar = parkedCar;
            FreeCar = null;
        }

        public CarStartLocatorData(float rotation, uint parkedCar, string? freeCar) : base(LocatorTypes.CarStart)
        {
            Rotation = rotation;
            ParkedCar = parkedCar;
            FreeCar = freeCar;
        }

        internal override LocatorData Clone() => ParkedCar.HasValue ? new CarStartLocatorData(Rotation, ParkedCar.Value, FreeCar) : new CarStartLocatorData(Rotation);

        public override string ToString() => $"Rotation = {Rotation}, ParkedCar = {ParkedCar?.ToString() ?? "null"}, FreeCar = {FreeCar ?? "null"}";
    }

    /// <summary>
    /// Spline locator data.
    /// </summary>
    public class SplineLocatorData : LocatorData
    {
        public override uint DataLen => 0;

        public override List<uint> DataArray => [];

        public SplineLocatorData() : base(LocatorTypes.Spline)
        { }

        internal override LocatorData Clone() => new SplineLocatorData();

        public override string ToString() => string.Empty;
    }

    /// <summary>
    /// Dynamic Zone locator data.
    /// </summary>
    public class DynamicZoneLocatorData : LocatorData
    {
        private string _dynaLoadData = string.Empty;
        public string DynaLoadData
        {
            get => _dynaLoadData;
            set
            {
                if (_dynaLoadData == value)
                    return;

                _dynaLoadData = value;
                OnSizeChanged();
            }
        }

        public override List<uint> DataArray => CreateStringData(DynaLoadData);

        public DynamicZoneLocatorData(List<uint> data) : base(LocatorTypes.DynamicZone)
        {
            DynaLoadData = ParseDataString(data).String;
        }

        public DynamicZoneLocatorData(string dynaLoadData) : base(LocatorTypes.DynamicZone)
        {
            DynaLoadData = dynaLoadData;
        }

        internal override LocatorData Clone() => new DynamicZoneLocatorData(DynaLoadData);

        public override string ToString() => $"DynaLoadData = {DynaLoadData}";
    }

    /// <summary>
    /// Occlusion locator data.
    /// </summary>
    public class OcclusionLocatorData : LocatorData
    {
        private uint? _occlusions = null;
        public uint? Occlusions
        {
            get => _occlusions;
            set
            {
                _occlusions = value;
                OnSizeChanged();
            }
        }

        public override uint DataLen => Occlusions.HasValue ? 1u : 0u;
        public override List<uint> DataArray => Occlusions.HasValue ? [Occlusions.Value] : [];

        public OcclusionLocatorData(List<uint> data) : base(LocatorTypes.Occlusion)
        {
            Occlusions = data.Count > 0 ? data[0] : null;
        }

        public OcclusionLocatorData() : base(LocatorTypes.Occlusion)
        { }

        public OcclusionLocatorData(uint occlusions) : base(LocatorTypes.Occlusion)
        {
            Occlusions = occlusions;
        }

        internal override LocatorData Clone() => Occlusions.HasValue ? new OcclusionLocatorData(Occlusions.Value) : new OcclusionLocatorData();

        public override string ToString() => $"Occlusions = {Occlusions?.ToString() ?? "null"}";
    }

    /// <summary>
    /// Interior Entrance locator data.
    /// </summary>
    public class InteriorEntranceLocatorData : LocatorData
    {
        private string _interiorName = string.Empty;
        public string InteriorName
        {
            get => _interiorName;
            set
            {
                if (_interiorName == value)
                    return;

                _interiorName = value;
                OnSizeChanged();
            }
        }
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

        public InteriorEntranceLocatorData(List<uint> data) : base(LocatorTypes.InteriorEntrance)
        {
            var interiorName = ParseDataString(data);
            InteriorName = interiorName.String;
            var index = interiorName.Index;
            Right = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            Up = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            Front = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
        }

        public InteriorEntranceLocatorData(string interiorName, Vector3 right, Vector3 up, Vector3 front) : base(LocatorTypes.InteriorEntrance)
        {
            InteriorName = interiorName;
            Right = right;
            Up = up;
            Front = front;
        }

        internal override LocatorData Clone() => new InteriorEntranceLocatorData(InteriorName, Right, Up, Front);

        public override string ToString() => $"InteriorName = {InteriorName}, Right = {Right}, Up = {Up}, Front = {Front}";
    }

    /// <summary>
    /// Directional locator data.
    /// </summary>
    public class DirectionalLocatorData : LocatorData
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

        public DirectionalLocatorData(List<uint> data) : base(LocatorTypes.Directional)
        {
            var index = 0;
            Right = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            Up = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            Front = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
        }

        public DirectionalLocatorData(Vector3 right, Vector3 up, Vector3 front) : base(LocatorTypes.Directional)
        {
            Right = right;
            Up = up;
            Front = front;
        }

        internal override LocatorData Clone() => new DirectionalLocatorData(Right, Up, Front);

        public override string ToString() => $"Right = {Right}, Up = {Up}, Front = {Front}";
    }

    /// <summary>
    /// Action locator data.
    /// </summary>
    public class ActionLocatorData : LocatorData
    {
        public enum Intention : uint
        {
            NONE,
            LeftStickX,
            LeftStickY,
            DoAction,
            Jump,
            Dash,
            Attack,
            DPadUp,
            DPadDown,
            DPadLeft,
            DPadRight,
            GetOutCar,
            MouseLookLeft,
            MouseLookRight,
        }

        private string _objectName = string.Empty;
        public string ObjectName
        {
            get => _objectName;
            set
            {
                if (_objectName == value)
                    return;

                _objectName = value;
                OnSizeChanged();
            }
        }
        private string _jointName = string.Empty;
        public string JointName
        {
            get => _jointName;
            set
            {
                if (_jointName == value)
                    return;

                _jointName = value;
                OnSizeChanged();
            }
        }
        private string _actionName = string.Empty;
        [KnownValues(true, "ToggleOnOff", "ToggleReverse", "PlayAnim", "PlayAnimLoop", "AutoPlayAnim", "AutoPlayAnimLoop", "AutoPlayAnimInOut", "DestroyObject", "PowerCoupling", "UseVendingMachine", "PrankPhone", "SummonVehiclePhone", "Doorbell", "OpenDoor", "TalkFood", "TalkCollectible", "TalkDialog", "TalkMission", "FoodSmall", "FoodLarge", "CollectorCard", "AlienCamera", "PlayOnce", "AutomaticDoor", "Wrench", "Teleport", "PurchaseCar", "PurchaseSkin", "Nitro")]
        public string ActionName
        {
            get => _actionName;
            set
            {
                if (_actionName == value)
                    return;

                _actionName = value;
                OnSizeChanged();
            }
        }
        public Intention ButtonInput { get; set; }
        private uint shouldTransform;
        public bool ShouldTransform
        {
            get => shouldTransform == 1;
            set => shouldTransform = value ? 1u : 0u;
        }

        public override List<uint> DataArray
        {
            get
            {
                List<uint> data = [];

                data.AddRange(CreateStringData(ObjectName));
                data.AddRange(CreateStringData(JointName));
                data.AddRange(CreateStringData(ActionName));
                data.Add((uint)ButtonInput);
                data.Add(ShouldTransform ? 1u : 0u);

                return [.. data];
            }
        }

        public ActionLocatorData(List<uint> data) : base(LocatorTypes.Action)
        {
            var objectName = ParseDataString(data);
            ObjectName = objectName.String;
            var jointName = ParseDataString(data, objectName.Index);
            JointName = jointName.String;
            var actionName = ParseDataString(data, jointName.Index);
            ActionName = actionName.String;
            var index = actionName.Index;
            ButtonInput = (Intention)data[index++];
            shouldTransform = data[index++];
        }

        public ActionLocatorData(string objectName, string jointName, string actionName, Intention buttonInput, bool shouldTransform) : base(LocatorTypes.Action)
        {
            ObjectName = objectName;
            JointName = jointName;
            ActionName = actionName;
            ButtonInput = buttonInput;
            ShouldTransform = shouldTransform;
        }

        internal override LocatorData Clone() => new ActionLocatorData(ObjectName, JointName, ActionName, ButtonInput, ShouldTransform);

        public override string ToString() => $"ObjectName = {ObjectName}, JointName = {JointName}, ActionName = {ActionName}, ButtonInput = {ButtonInput}, ShouldTransform = {ShouldTransform}";
    }

    /// <summary>
    /// FOV locator data.
    /// </summary>
    public class FOVLocatorData : LocatorData
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

        public FOVLocatorData(List<uint> data) : base(LocatorTypes.FOV)
        {
            FOV = ParseDataFloat(data[0]);
            Type = ParseDataFloat(data[1]);
            Rate = ParseDataFloat(data[2]);
        }

        public FOVLocatorData(float fov, float type, float rate) : base(LocatorTypes.FOV)
        {
            FOV = fov;
            Type = type;
            Rate = rate;
        }

        internal override LocatorData Clone() => new FOVLocatorData(FOV, Type, Rate);

        public override string ToString() => $"FOV = {FOV}, Type = {Type}, Rate = {Rate}";
    }

    /// <summary>
    /// Breakable Camera locator data.
    /// </summary>
    public class BreakableCameraLocatorData : LocatorData
    {
        public override uint DataLen => 0;
        public override List<uint> DataArray => [];

        public BreakableCameraLocatorData() : base(LocatorTypes.BreakableCamera)
        { }

        internal override LocatorData Clone() => new BreakableCameraLocatorData();

        public override string ToString() => string.Empty;
    }

    /// <summary>
    /// Static Camera locator data.
    /// </summary>
    public class StaticCameraLocatorData : LocatorData
    {
        public Vector3 TargetPosition { get; set; }
        public float FOV { get; set; }
        public float TargetLag { get; set; }
        public uint FollowPlayer { get; set; }
        private float? _transitionTargetRate = null;
        public float? TransitionTargetRate
        {
            get => _transitionTargetRate;
            set
            {
                _transitionTargetRate = value;
                OnSizeChanged();
            }
        }
        private uint? _flags = null;
        public uint? Flags
        {
            get => _flags;
            set
            {
                _flags = value;
                OnSizeChanged();
            }
        }
        private uint? _cutInOut = null;
        public uint? CutInOut
        {
            get => _cutInOut;
            set
            {
                _cutInOut = value;
                OnSizeChanged();
            }
        }
        private uint? _data = null;
        public uint? Data
        {
            get => _data;
            set
            {
                _data = value;
                OnSizeChanged();
            }
        }

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

        public StaticCameraLocatorData(List<uint> data) : base(LocatorTypes.StaticCamera)
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

        public StaticCameraLocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer) : base(LocatorTypes.StaticCamera)
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

        public StaticCameraLocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer, float? transitionTargetRate) : base(LocatorTypes.StaticCamera)
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

        public StaticCameraLocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer, float? transitionTargetRate, uint? flags) : base(LocatorTypes.StaticCamera)
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

        public StaticCameraLocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer, float? transitionTargetRate, uint? flags, uint? cutInOut, uint? data) : base(LocatorTypes.StaticCamera)
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

        internal override LocatorData Clone() => (TransitionTargetRate.HasValue, Flags.HasValue, CutInOut.HasValue, Data.HasValue) switch
        {
            (true, true, true, true) =>
                new StaticCameraLocatorData(TargetPosition, FOV, TargetLag, FollowPlayer,
                                        TransitionTargetRate!.Value!, Flags!.Value, CutInOut!.Value, Data!.Value),
            (true, true, _, _) =>
                new StaticCameraLocatorData(TargetPosition, FOV, TargetLag, FollowPlayer,
                                        TransitionTargetRate!.Value, Flags!.Value),
            (true, _, _, _) =>
                new StaticCameraLocatorData(TargetPosition, FOV, TargetLag, FollowPlayer,
                                        TransitionTargetRate!.Value),
            _ =>
                new StaticCameraLocatorData(TargetPosition, FOV, TargetLag, FollowPlayer)
        };

        public override string ToString() => $"TargetPosition = {TargetPosition}, FOV = {FOV}, TargetLag = {TargetLag}, FollowPlayer = {FollowPlayer}, TransitionTargetRate = {TransitionTargetRate?.ToString() ?? "null"}, Flags = {Flags?.ToString() ?? "null"}, CutInOut = {CutInOut?.ToString() ?? "null"}, Data = {Data?.ToString() ?? "null"}";
    }

    /// <summary>
    /// Ped Group locator data.
    /// </summary>
    public class PedGroupLocatorData : LocatorData
    {
        public uint GroupNum { get; set; }

        public override uint DataLen => 1;
        public override List<uint> DataArray => [GroupNum];

        public PedGroupLocatorData(List<uint> data) : base(LocatorTypes.PedGroup)
        {
            GroupNum = data[0];
        }

        public PedGroupLocatorData(uint groupNum) : base(LocatorTypes.PedGroup)
        {
            GroupNum = groupNum;
        }

        internal override LocatorData Clone() => new PedGroupLocatorData(GroupNum);

        public override string ToString() => $"GroupNum = {GroupNum}";
    }

    /// <summary>
    /// Coin locator data.
    /// </summary>
    public class CoinLocatorData : LocatorData
    {
        public override uint DataLen => 0;
        public override List<uint> DataArray => [];

        public CoinLocatorData() : base(LocatorTypes.Coin)
        { }

        internal override LocatorData Clone() => new CoinLocatorData();

        public override string ToString() => string.Empty;
    }
}