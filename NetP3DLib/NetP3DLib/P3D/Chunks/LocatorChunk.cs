using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
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
            _typeData.PropertyChanged -= TypeData_PropertyChanged;
            _typeData = value;
            value.SizeChanged += TypeData_SizeChanged;
            value.PropertyChanged += TypeData_PropertyChanged;
        }
    }
    
    private Vector3 _position;
    public Vector3 Position
    {
        get => _position;
        set
        {
            if (_position == value)
                return;
    
            _position = value;
            OnPropertyChanged(nameof(Position));
        }
    }
    
    public uint TriggerCount => GetChildCount(ChunkIdentifier.Trigger_Volume);

    public override byte[] DataBytes
    {
        get
        {
            var data = new List<byte>((int)DataLength);

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
    public LocatorChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), ReadLocatorData(br), br.ReadVector3())
    {
        var triggerCount = br.ReadUInt32();
    }

    private static LocatorData ReadLocatorData(EndianAwareBinaryReader br)
    {
        var type = (LocatorTypes)br.ReadUInt32();
        var data = br.ReadUInt32Array(out _);
        return type switch
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
    }

    public LocatorChunk(string name, LocatorData typeData, Vector3 position) : base(ChunkID, name)
    {
        TypeData = typeData;
        _position = position;
    }

    private void TypeData_SizeChanged(int delta) => RecalculateSize((uint)(HeaderSize - delta));

    private void TypeData_PropertyChanged() => OnPropertyChanged(nameof(TypeData));

    protected override void WriteData(EndianAwareBinaryWriter bw)
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
        public event Action<int>? SizeChanged;
        protected void OnSizeChanged(int delta) => SizeChanged?.Invoke(delta);

        public event Action? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke();

        public LocatorTypes LocatorType { get; }
        public virtual uint DataLen => (uint)DataArray.Count;
        public abstract List<uint> DataArray { get; }

        public LocatorData(LocatorTypes locatorType)
        {
            LocatorType = locatorType;
        }

        public void WriteData(EndianAwareBinaryWriter bw)
        {
            foreach (var item in DataArray)
                bw.Write(item);
        }

        internal abstract LocatorData Clone();

        internal static (string String, int Index) ParseDataString(IList<uint> data, int offset = 0)
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

        internal static unsafe float ParseDataFloat(uint data) => *(float*)&data;

        internal static unsafe uint CreateFloatData(float input) => *(uint*)&input;
    }

    public class UnknownLocatorData : LocatorData
    {
        private uint[] _data;
        public uint[] Data
        {
            get => _data;
            set
            {
                if (ReferenceEquals(_data, value))
                    return;

                var oldSize = _data.Length;
                _data = value ?? [];
                var newSize = _data.Length;
                OnSizeChanged((newSize - oldSize) * sizeof(uint));
                OnPropertyChanged(nameof(Data));
            }
        }
        public override List<uint> DataArray => [.. Data];

        public UnknownLocatorData(LocatorTypes locatorType, IList<uint> data) : base(locatorType)
        {
            _data = [.. data];
        }

        public UnknownLocatorData(uint locatorType, IList<uint> data) : this((LocatorTypes)locatorType, data)
        { }

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
            // Generic events
            Placeholder = 86,
            TreeOfWoeNegativeFeedback = 87,
            MissionVehicleCreated = 88,
            MissionVehicleReleased = 89,
            Kick = 90,
            Stomp = 91,
            Doublejump = 92,
            BreakCamera = 93,
            BonusMissionCharacterApproached = 94,
            ObjectDestroyed = 95,
            VehicleVehicleCollision = 96,
            VehicleDestroyed = 97,
            VehicleDestroyedByUser = 98,
            VehicleDestroyedSyncSound = 99,
            VehicleDamaged = 100,
            VehicleSuspensionBottomedOut = 101,
            UserVehicleAddedToWorld = 102,
            UserVehicleRemovedFromWorld = 103,
            GetIntoVehicleStart = 104,
            GetIntoVehicleEnd = 105,
            GetOutofVehicleStart = 106,
            GetOutofVehicleEnd = 107,
            EnteringTrafficCar = 108,
            EnteringPlayerCar = 109,
            EnterInteriorStart = 110,
            EnterInteriorTransitionStart = 111,
            EnterInteriorTransitionEnd = 112,
            EnterInteriorEnd = 113,
            ExitInteriorStart = 114,
            ExitInteriorEnd = 115,
            InteriorSwitch = 116,
            CollectedCoins = 117,
            LostCoins = 118,
            SpawnedCoins = 119,
            BossDestroyedPlayerCar = 120,
            BossDamaged = 121,
            ColapropDestroyed = 122,
            VehicleCollectedProp = 123,
            Abducted = 124,
            InteriorLoaded = 125,
            InteriorDumped = 126,
            InteriorLoadStart = 127,
            FirstDynamicZoneStart = 128,
            FirstDynamicZoneEnd = 129,
            AllDynamicZoneEnd = 130,
            DynamicZoneLoadEnded = 131,
            NamedDynamicZoneLoadEnded = 132,
            AllDynamicZonesDumped = 133,
            WayAIAtDestination = 134,
            WayAIHitLastWaypoint = 135,
            WayAIHitWaypoint = 136,
            WayAIHitCheckpoint = 137,
            MissionReset = 138,
            DestinationReached = 139,
            CollectObject = 140,
            GagStart = 141,
            GagEnd = 142,
            InteractiveGag = 143,
            GagFound = 144,
            DumpStatus = 145,
            MissionIntro = 146,
            MissionStart = 147,
            MissionObjectiveNew = 148,
            ShowMissionObjective = 149,
            MissionDrama = 150,
            ChangeMusic = 151,
            ChangeMusicState = 152,
            StageComplete = 153,
            StageTransitionFailed = 154,
            MissionFailure = 155,
            MissionSuccess = 156,
            MissionCharacterReset = 157,
            LevelStart = 158,
            AttemptToEnterGamblerace = 159,
            EnterGambleraceSuccess = 160,
            EnterGambleraceFailure = 161,
            GambleraceSuccess = 162,
            GambleraceFailure = 163,
            UserCancelMissionBriefing = 164,
            UserCancelPauseMenu = 165,
            OutofcarConditionActive = 166,
            OutofcarConditionInactive = 167,
            KickNpc = 168,
            FEMenuSelect = 169,
            FEMenuBack = 170,
            FEMenuUpordown = 171,
            FEStartGameSelected = 172,
            FECheatSuccess = 173,
            FECheatFailure = 174,
            FEContinue = 175,
            FECancel1 = 176,
            FECancel2 = 177,
            FEPauseMenuStart = 178,
            FEPauseMenuEnd = 179,
            FELockedOut = 180,
            FEGagInit = 181,
            FEGagStart = 182,
            FEGagStop = 183,
            FeCreditsNewLine = 184,
            DialogShutup = 185,
            HudLapUpdated = 186,
            HudTimerBlink = 187,
            PhoneBoothBusy = 188,
            Collision = 189,
            CameraChange = 190,
            MinorCrash = 191,
            MinorVehicleCrash = 192,
            BigCrash = 193,
            BigVehicleCrash = 194,
            BigBoomSound = 195,
            BarrelBlowedUp = 196,
            Burnout = 197,
            BurnoutEnd = 198,
            HitBreakable = 199,
            HitMoveable = 200,
            BigAir = 201,
            Footstep = 202,
            JumpTakeoff = 203,
            JumpLanding = 204,
            PedestrianSmackdown = 205,
            TurboStart = 206,
            CharacterTiredNow = 207,
            WrongSideDumbass = 208,
            PedestrianDodge = 209,
            CardCollected = 210,
            BigRedSwitchPressed = 211,
            BreakCameraOrBox = 212,
            TutorialDialogPlay = 213,
            TutorialDialogDone = 214,
            ChaseVehicleSpawned = 215,
            ChaseVehicleDestroyed = 216,
            ChaseVehicleProximity = 217,
            TimeRunningOut = 218,
            RacePassedAi = 219,
            RaceGotPassedByAi = 220,
            PositionalSoundTriggerHit = 221,
            PcNpcCollision = 222,
            PlayerCarHitNpc = 223,
            PlayerMakesLightOfCarHittingNpc = 224,
            KickNpcSound = 225,
            HitHead = 226,
            DeathVolumeSound = 227,
            TrafficSpawn = 228,
            TrafficRemove = 229,
            TrafficGotHit = 230,
            TrafficImpeded = 231,
            PlayerVehicleHorn = 232,
            TrafficHorn = 233,
            DingDong = 234,
            PhoneBoothRideRequest = 235,
            PhoneBoothNewVehicleSelected = 236,
            PhoneBoothOldVehicleReselected = 237,
            PhoneBoothCancelRidereplyLine = 238,
            TailLostDialog = 239,
            MissionSuccessDialog = 240,
            VillainTailEvade = 241,
            VillainCarHitPlayer = 242,
            AmbientGreeting = 243,
            AmbientResponse = 244,
            AmbientAskfood = 245,
            AmbientFoodreply = 246,
            WaspCharging = 247,
            WaspCharged = 248,
            WaspAttacking = 249,
            WaspBlowedUp = 250,
            WaspBulletFired = 251,
            WaspBulletMissed = 252,
            WaspBulletHitCharacterStylizedViolenceFollows = 253,
            WaspApproached = 254,
            MissionCollectiblePickedUp = 255,
            HitAndRunStart = 256,
            HitAndRunCaught = 257,
            HitAndRunEvaded = 258,
            HitAndRunMeterThrob = 259,
            HagglingWithGil = 260,
            StartAnimationSound = 261,
            StopAnimationSound = 262,
            StartAnimEntityDsgSound = 263,
            StopAnimEntityDsgSound = 264,
            PlayBirdSound = 265,
            SupersprintWin = 266,
            SupersprintLose = 267,
            PlayCredits = 268,
            PlayFeMusic = 269,
            PlayMuzak = 270,
            PlayIdleMusic = 271,
            StopTheMusic = 272,
            AvatarVehicleToggle = 273,
            MissionBriefingAccepted = 274,
            GUIMissionLoadComplete = 275,
            GUICountdownStart = 276,
            GUIMissionStart = 277,
            GUILeavingPauseMenu = 278,
            GUIIrisWipeClosed = 279,
            GUIIrisWipeOpen = 280,
            GUIFadeOutDone = 281,
            GUIFadeInDone = 282,
            LetterboxClosed = 283,
            DeathVolumeScreenBlank = 284,
            GuiEnteringMissionSuccessScreen = 285,
            GuiTriggerPattyAndSelmaScreen = 286,
            ConversationInit = 287,
            ConversationInitDialog = 288,
            ConversationStart = 289,
            ConversationSkip = 290,
            ConversationDone = 291,
            ConversationDoneAndFinished = 292,
            InGameplayConversation = 293,
            PcTalk = 294,
            PcShutup = 295,
            NpcTalk = 296,
            NpcShutup = 297,
            ObjectKicked = 298,
            TalkToNpc = 299,
            CameraShake = 300,
            RumbleCollision = 301,
            BonusMissionDialogue = 302,
            DumpDynaSection = 303,
            UnlockedCar = 304,
            UnlockedSkin = 305,
            CompletedAllstreetraces = 306,
            CompletedBonusmissions = 307,
            CollectedAllcoins = 308,
            CollectedAllcards = 309,
            DestroyedAllcameras = 310,
            SwitchSkin = 311,
            RepairCar = 312,
            CollectedWrench = 313,
            ActorCreated = 314,
            ActorRemoved = 315,
            StatepropDestroyed = 316,
            StatepropAddedToSim = 317,
            EnteredTeleportPad = 318,
            ExitedTeleportPad = 319,
            TakingTeleport = 320,
            LoseCollectible = 321,
            CollectedNitro = 322,
            UseNitro = 323,
            CharacterPosReset = 324,
            ToggleFirstperson = 325,
            AnimatedCamShutdown = 326,
            CarExplosionDone = 327,
            StatepropCollectibleDestroyed = 328,
            AvatarOffRoad = 329,
            AvatarOnRoad = 330,
        }

        private Events _event;
        public Events Event
        {
            get => _event;
            set
            {
                if (_event == value)
                    return;
    
                _event = value;
                OnPropertyChanged(nameof(Event));
            }
        }

        private bool _hasParameter = false;
        public bool HasParameter
        {
            get => _hasParameter;
            set
            {
                if (_hasParameter == value)
                    return;

                _hasParameter = value;

                if (value)
                    OnSizeChanged(sizeof(uint));
                else
                    OnSizeChanged(-sizeof(uint));

                OnPropertyChanged(nameof(HasParameter));
            }
        }
        private uint _parameter = 0;
        public uint Parameter
        {
            get => _parameter;
            set
            {
                if (_parameter == value)
                    return;

                _parameter = value;

                OnPropertyChanged(nameof(Parameter));
            }
        }

        public override uint DataLen => HasParameter ? 2u : 1u;
        public override List<uint> DataArray
        {
            get
            {
                List<uint> data =
                [
                    (uint)Event,
                ];
                if (HasParameter)
                    data.Add(Parameter);

                return [.. data];
            }
        }

        public EventLocatorData(IList<uint> data) : base(LocatorTypes.Event)
        {
            _event = (Events)data[0];
            if (data.Count > 1)
            {
                _hasParameter = true;
                _parameter = data[1];
            }
        }

        public EventLocatorData(Events @event) : base(LocatorTypes.Event)
        {
            _event = @event;
        }

        public EventLocatorData(Events @event, uint parameter) : base(LocatorTypes.Event)
        {
            _event = @event;
            _hasParameter = true;
            _parameter = parameter;
        }

        internal override LocatorData Clone() => HasParameter ? new EventLocatorData(Event, Parameter) : new EventLocatorData(Event);

        public override string ToString() => $"_event = {Event}, Parameter = {(HasParameter ? Parameter.ToString() : "null")}";
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

                var oldSize = DataArray.Count;
                _key = value;
                var newSize = DataArray.Count;
                OnSizeChanged((newSize - oldSize) * sizeof(uint));
                OnPropertyChanged(nameof(Key));
            }
        }

        public override List<uint> DataArray => CreateStringData(Key);

        public ScriptLocatorData(IList<uint> data) : base(LocatorTypes.Script)
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

        public override string ToString() => "No data";
    }

    /// <summary>
    /// Car Start locator data.
    /// </summary>
    public class CarStartLocatorData : LocatorData
    {
        private float _rotation;
        public float Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation == value)
                    return;
    
                _rotation = value;
                OnPropertyChanged(nameof(Rotation));
            }
        }

        private bool _hasParkedCar = false;
        public bool HasParkedCar
        {
            get => _hasParkedCar;
            set
            {
                if (_hasParkedCar == value)
                    return;

                _hasParkedCar = value;

                if (value)
                {
                    OnSizeChanged(sizeof(uint));
                }
                else
                {
                    OnSizeChanged(-sizeof(uint));
                    HasFreeCar = false;
                }

                OnPropertyChanged(nameof(HasParkedCar));
            }
        }
        private uint _parkedCar = 0;
        public bool ParkedCar
        {
            get => _parkedCar == 1;
            set
            {
                if (ParkedCar == value)
                    return;

                _parkedCar = value ? 1u : 0u;

                OnPropertyChanged(nameof(ParkedCar));
            }
        }

        private bool _hasFreeCar = false;
        public bool HasFreeCar
        {
            get => _hasFreeCar;
            set
            {
                if (_hasFreeCar == value)
                    return;

                var oldSize = DataArray.Count;
                _hasFreeCar = value;
                var newSize = DataArray.Count;
                OnSizeChanged((newSize - oldSize) * sizeof(uint));

                if (value)
                    HasParkedCar = true;

                OnPropertyChanged(nameof(HasFreeCar));
            }
        }
        private string _freeCar = string.Empty;
        public string FreeCar
        {
            get => _freeCar;
            set
            {
                if (_freeCar == value)
                    return;


                if (string.IsNullOrEmpty(value))
                {
                    HasFreeCar = false;
                    return;
                }

                var oldSize = DataArray.Count;
                _freeCar = value;
                var newSize = DataArray.Count;
                OnSizeChanged((newSize - oldSize) * sizeof(uint));

                OnPropertyChanged(nameof(FreeCar));
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
                if (HasParkedCar)
                {
                    data.Add(_parkedCar);
                    if (HasFreeCar && !string.IsNullOrWhiteSpace(FreeCar))
                        data.AddRange(CreateStringData(FreeCar));
                }

                return [.. data];
            }
        }

        public CarStartLocatorData(IList<uint> data) : base(LocatorTypes.CarStart)
        {
            _rotation = ParseDataFloat(data[0]);
            if (data.Count > 1)
            {
                _hasParkedCar = true;
                _parkedCar = data[1];
                if (data.Count > 2)
                {
                    _hasFreeCar = true;
                    _freeCar = ParseDataString(data, 2).String;
                }
            }
        }

        public CarStartLocatorData(float rotation) : this(rotation, null, null)
        {
        }

        public CarStartLocatorData(float rotation, bool parkedCar) : this(rotation, parkedCar? 1u : 0u, null)
        {
        }

        public CarStartLocatorData(float rotation, bool parkedCar, string? freeCar) : this(rotation, parkedCar ? 1u : 0u, freeCar)
        {
        }

        public CarStartLocatorData(float rotation, uint? parkedCar, string? freeCar) : base(LocatorTypes.CarStart)
        {
            _rotation = rotation;
            if (parkedCar.HasValue)
            {
                _hasParkedCar = true;
                _parkedCar = parkedCar.Value;
            }
            if (!string.IsNullOrWhiteSpace(freeCar))
            {
                _hasParkedCar = true;
                _hasFreeCar = true;
                _freeCar = freeCar!;
            }
        }

        internal override LocatorData Clone() => new CarStartLocatorData(Rotation, HasParkedCar ? _parkedCar : null, HasFreeCar ? _freeCar : null);

        public override string ToString() => $"Rotation = {Rotation}, ParkedCar = {(HasParkedCar ? ParkedCar.ToString() : "null")}, FreeCar = {(HasFreeCar ? FreeCar : "null")}";
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

        public override string ToString() => "No data";
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

                var oldSize = DataArray.Count;
                _dynaLoadData = value;
                var newSize = DataArray.Count;
                OnSizeChanged((newSize - oldSize) * sizeof(uint));
                OnPropertyChanged(nameof(DynaLoadData));
            }
        }

        public override List<uint> DataArray => CreateStringData(DynaLoadData);

        public DynamicZoneLocatorData(IList<uint> data) : base(LocatorTypes.DynamicZone)
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
        private bool _hasOcclusions = false;
        public bool HasOcclusions
        {
            get => _hasOcclusions;
            set
            {
                if (_hasOcclusions == value)
                    return;

                _hasOcclusions = value;

                if (value)
                    OnSizeChanged(sizeof(uint));
                else
                    OnSizeChanged(-sizeof(uint));

                OnPropertyChanged(nameof(HasOcclusions));
            }
        }
        private uint _occlusions = 0;
        public uint Occlusions
        {
            get => _occlusions;
            set
            {
                if (_occlusions == value)
                    return;

                _occlusions = value;

                OnPropertyChanged(nameof(Occlusions));
            }
        }

        public override uint DataLen => HasOcclusions ? 1u : 0u;
        public override List<uint> DataArray => HasOcclusions ? [Occlusions] : [];

        public OcclusionLocatorData(IList<uint> data) : base(LocatorTypes.Occlusion)
        {
            if (data.Count > 0)
            {
                _hasOcclusions = true;
                _occlusions = data[0];
            }
        }

        public OcclusionLocatorData() : base(LocatorTypes.Occlusion)
        { }

        public OcclusionLocatorData(uint occlusions) : base(LocatorTypes.Occlusion)
        {
            _hasOcclusions = true;
            _occlusions = occlusions;
        }

        internal override LocatorData Clone() => HasOcclusions ? new OcclusionLocatorData(Occlusions) : new OcclusionLocatorData();

        public override string ToString() => $"Occlusions = {(HasOcclusions ? Occlusions.ToString() : "null")}";
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

                var oldSize = DataArray.Count;
                _interiorName = value;
                var newSize = DataArray.Count;
                OnSizeChanged((newSize - oldSize) * sizeof(uint));
                OnPropertyChanged(nameof(InteriorName));
            }
        }
    
        private Vector3 _right;
        public Vector3 Right
        {
            get => _right;
            set
            {
                if (_right == value)
                    return;
    
                _right = value;
                OnPropertyChanged(nameof(Right));
            }
        }
    
        private Vector3 _up;
        public Vector3 Up
        {
            get => _up;
            set
            {
                if (_up == value)
                    return;
    
                _up = value;
                OnPropertyChanged(nameof(Up));
            }
        }
    
        private Vector3 _front;
        public Vector3 Front
        {
            get => _front;
            set
            {
                if (_front == value)
                    return;
    
                _front = value;
                OnPropertyChanged(nameof(Front));
            }
        }

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

        public InteriorEntranceLocatorData(IList<uint> data) : base(LocatorTypes.InteriorEntrance)
        {
            var interiorName = ParseDataString(data);
            _interiorName = interiorName.String;
            var index = interiorName.Index;
            _right = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            _up = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            _front = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
        }

        public InteriorEntranceLocatorData(string interiorName, Vector3 right, Vector3 up, Vector3 front) : base(LocatorTypes.InteriorEntrance)
        {
            _interiorName = interiorName;
            _right = right;
            _up = up;
            _front = front;
        }

        internal override LocatorData Clone() => new InteriorEntranceLocatorData(InteriorName, Right, Up, Front);

        public override string ToString() => $"InteriorName = {InteriorName}, _right = {Right}, _up = {Up}, _front = {Front}";
    }

    /// <summary>
    /// Directional locator data.
    /// </summary>
    public class DirectionalLocatorData : LocatorData
    {
        private Vector3 _right;
        public Vector3 Right
        {
            get => _right;
            set
            {
                if (_right == value)
                    return;
    
                _right = value;
                OnPropertyChanged(nameof(Right));
            }
        }
    
        private Vector3 _up;
        public Vector3 Up
        {
            get => _up;
            set
            {
                if (_up == value)
                    return;
    
                _up = value;
                OnPropertyChanged(nameof(Up));
            }
        }
    
        private Vector3 _front;
        public Vector3 Front
        {
            get => _front;
            set
            {
                if (_front == value)
                    return;
    
                _front = value;
                OnPropertyChanged(nameof(Front));
            }
        }
    
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

        public DirectionalLocatorData(IList<uint> data) : base(LocatorTypes.Directional)
        {
            var index = 0;
            _right = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            _up = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
            _front = new(ParseDataFloat(data[index++]), ParseDataFloat(data[index++]), ParseDataFloat(data[index++]));
        }

        public DirectionalLocatorData(Vector3 right, Vector3 up, Vector3 front) : base(LocatorTypes.Directional)
        {
            _right = right;
            _up = up;
            _front = front;
        }

        internal override LocatorData Clone() => new DirectionalLocatorData(Right, Up, Front);

        public override string ToString() => $"_right = {Right}, _up = {Up}, _front = {Front}";
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

                var oldSize = DataArray.Count;
                _objectName = value;
                var newSize = DataArray.Count;
                OnSizeChanged((newSize - oldSize) * sizeof(uint));
                OnPropertyChanged(nameof(ObjectName));
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

                var oldSize = DataArray.Count;
                _jointName = value;
                var newSize = DataArray.Count;
                OnSizeChanged((newSize - oldSize) * sizeof(uint));
                OnPropertyChanged(nameof(JointName));
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

                var oldSize = DataArray.Count;
                _actionName = value;
                var newSize = DataArray.Count;
                OnSizeChanged((newSize - oldSize) * sizeof(uint));
                OnPropertyChanged(nameof(ActionName));
            }
        }
    
        private Intention _buttonInput;
        public Intention ButtonInput
        {
            get => _buttonInput;
            set
            {
                if (_buttonInput == value)
                    return;
    
                _buttonInput = value;
                OnPropertyChanged(nameof(ButtonInput));
            }
        }
    
        private uint _shouldTransform;
        public bool ShouldTransform
        {
            get => _shouldTransform == 1;
            set
            {
                if (ShouldTransform == value)
                    return;

                _shouldTransform = value ? 1u : 0u;
                OnPropertyChanged(nameof(ShouldTransform));
            }
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

        public ActionLocatorData(IList<uint> data) : base(LocatorTypes.Action)
        {
            var objectName = ParseDataString(data);
            ObjectName = objectName.String;
            var jointName = ParseDataString(data, objectName.Index);
            JointName = jointName.String;
            var actionName = ParseDataString(data, jointName.Index);
            ActionName = actionName.String;
            var index = actionName.Index;
            _buttonInput = (Intention)data[index++];
            _shouldTransform = data[index++];
        }

        public ActionLocatorData(string objectName, string jointName, string actionName, Intention buttonInput, bool shouldTransform) : base(LocatorTypes.Action)
        {
            ObjectName = objectName;
            JointName = jointName;
            ActionName = actionName;
            _buttonInput = buttonInput;
            ShouldTransform = shouldTransform;
        }

        internal override LocatorData Clone() => new ActionLocatorData(ObjectName, JointName, ActionName, ButtonInput, ShouldTransform);

        public override string ToString() => $"ObjectName = {ObjectName}, JointName = {JointName}, ActionName = {ActionName}, _buttonInput = {ButtonInput}, ShouldTransform = {ShouldTransform}";
    }

    /// <summary>
    /// FOV locator data.
    /// </summary>
    public class FOVLocatorData : LocatorData
    {
        private float _fov;
        public float FOV
        {
            get => _fov;
            set
            {
                if (_fov == value)
                    return;
    
                _fov = value;
                OnPropertyChanged(nameof(FOV));
            }
        }
    
        private float _type;
        public float Type
        {
            get => _type;
            set
            {
                if (_type == value)
                    return;
    
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }
    
        private float _rate;
        public float Rate
        {
            get => _rate;
            set
            {
                if (_rate == value)
                    return;
    
                _rate = value;
                OnPropertyChanged(nameof(Rate));
            }
        }

        public override uint DataLen => 3;
        public override List<uint> DataArray => [
            CreateFloatData(FOV),
            CreateFloatData(Type),
            CreateFloatData(Rate),
        ];

        public FOVLocatorData(IList<uint> data) : base(LocatorTypes.FOV)
        {
            _fov = ParseDataFloat(data[0]);
            _type = ParseDataFloat(data[1]);
            _rate = ParseDataFloat(data[2]);
        }

        public FOVLocatorData(float fov, float type, float rate) : base(LocatorTypes.FOV)
        {
            _fov = fov;
            _type = type;
            _rate = rate;
        }

        internal override LocatorData Clone() => new FOVLocatorData(FOV, Type, Rate);

        public override string ToString() => $"_fOV = {FOV}, _type = {Type}, _rate = {Rate}";
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

        public override string ToString() => "No data";
    }

    /// <summary>
    /// Static Camera locator data.
    /// </summary>
    public class StaticCameraLocatorData : LocatorData
    {
        private Vector3 _targetPosition;
        public Vector3 TargetPosition
        {
            get => _targetPosition;
            set
            {
                if (_targetPosition == value)
                    return;
    
                _targetPosition = value;
                OnPropertyChanged(nameof(TargetPosition));
            }
        }
    
        private float _fov;
        public float FOV
        {
            get => _fov;
            set
            {
                if (_fov == value)
                    return;
    
                _fov = value;
                OnPropertyChanged(nameof(FOV));
            }
        }
    
        private float _targetLag;
        public float TargetLag
        {
            get => _targetLag;
            set
            {
                if (_targetLag == value)
                    return;
    
                _targetLag = value;
                OnPropertyChanged(nameof(TargetLag));
            }
        }
    
        private uint _followPlayer;
        public bool FollowPlayer
        {
            get => _followPlayer == 1u;
            set
            {
                if (FollowPlayer == value)
                    return;

                _followPlayer = value ? 1u : 0u;
                OnPropertyChanged(nameof(FollowPlayer));
            }
        }

        private bool _hasTransitionTargetRate = false;
        public bool HasTransitionTargetRate
        {
            get => _hasTransitionTargetRate;
            set
            {
                if (_hasTransitionTargetRate == value)
                    return;

                _hasTransitionTargetRate = value;

                if (value)
                {
                    OnSizeChanged(sizeof(float));
                }
                else
                {
                    OnSizeChanged(-sizeof(float));
                    HasFlags = false;
                    HasFlags2 = false;
                }

                OnPropertyChanged(nameof(HasTransitionTargetRate));
            }
        }
        private float _transitionTargetRate = 0.04f;
        public float TransitionTargetRate
        {
            get => _transitionTargetRate;
            set
            {
                if (_transitionTargetRate == value)
                    return;

                _transitionTargetRate = value;
                
                OnPropertyChanged(nameof(TransitionTargetRate));
            }
        }

        private bool _hasFlags = false;
        public bool HasFlags
        {
            get => _hasFlags;
            set
            {
                if (_hasFlags == value)
                    return;

                _hasFlags = value;

                if (value)
                {
                    OnSizeChanged(sizeof(uint));
                    HasTransitionTargetRate = true;
                }
                else
                {
                    OnSizeChanged(-sizeof(uint));
                    HasFlags2 = false;
                }

                OnPropertyChanged(nameof(HasFlags));
            }
        }
        private uint _flags = 0;
        public bool OneShot
        {
            get => (_flags & 1u) != 0u;
            set
            {
                if (OneShot == value)
                    return;

                if (value)
                    _flags |= 1u;
                else
                    _flags &= ~1u;

                OnPropertyChanged(nameof(OneShot));
            }
        }
        public bool DisableFOV
        {
            get => (_flags & (1u << 1)) != 0;
            set
            {
                if (DisableFOV == value)
                    return;

                if (value)
                    _flags |= (1u << 1);
                else
                    _flags &= ~(1u << 1);

                OnPropertyChanged(nameof(DisableFOV));
            }
        }

        private bool _hasFlags2 = false;
        public bool HasFlags2
        {
            get => _hasFlags2;
            set
            {
                if (_hasFlags2 == value)
                    return;

                _hasFlags2 = value;

                if (value)
                {
                    OnSizeChanged(sizeof(uint) * 2);
                    HasTransitionTargetRate = true;
                    HasFlags = true;
                }
                else
                {
                    OnSizeChanged(-sizeof(uint) * 2);
                }

                OnPropertyChanged(nameof(HasFlags2));
            }
        }
        private uint _cutInOut = 0u;
        public bool CutInOut
        {
            get => _cutInOut == 1u;
            set
            {
                if (CutInOut == value)
                    return;

                _cutInOut = value ? 1u : 0u;

                OnPropertyChanged(nameof(CutInOut));
            }
        }
        private uint _flags2 = 0;
        public bool CarOnly
        {
            get => (_flags2 & 1u) == 1u;
            set
            {
                if (CarOnly == value)
                    return;

                if (value)
                    _flags2 |= 1u;
                else
                    _flags2 &= ~1u;

                OnPropertyChanged(nameof(CarOnly));
            }
        }
        public bool OnFootOnly
        {
            get => (_flags2 & (1u << 1)) == (1u << 1);
            set
            {
                if (OnFootOnly == value)
                    return;

                if (value)
                    _flags2 |= (1u << 1);
                else
                    _flags2 &= ~(1u << 1);

                OnPropertyChanged(nameof(OnFootOnly));
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
                data.Add(_followPlayer);

                if (!HasTransitionTargetRate)
                    return [.. data];
                data.Add(CreateFloatData(TransitionTargetRate));

                if (!HasFlags)
                    return [.. data];
                data.Add(_flags);

                if (!HasFlags2)
                    return [.. data];
                data.Add(_cutInOut);
                data.Add(_flags2);

                return [.. data];
            }
        }

        public StaticCameraLocatorData(IList<uint> data) : base(LocatorTypes.StaticCamera)
        {
            _targetPosition = new(ParseDataFloat(data[0]), ParseDataFloat(data[1]), ParseDataFloat(data[2]));
            _fov = ParseDataFloat(data[3]);
            _targetLag = ParseDataFloat(data[4]);
            _followPlayer = data[5];
            if (data.Count <= 6)
                return;

            _hasTransitionTargetRate = true;
            _transitionTargetRate = ParseDataFloat(data[6]);
            if (data.Count <= 7)
                return;

            _hasFlags = true;
            _flags = data[7];
            if (data.Count <= 8)
                return;

            _cutInOut = data[8];
            if (data.Count <= 9)
                return;

            _hasFlags2 = true;
            _flags2 = data[9];
        }

        public StaticCameraLocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer) : base(LocatorTypes.StaticCamera)
        {
            _targetPosition = targetPosition;
            _fov = fov;
            _targetLag = targetLag;
            _followPlayer = followPlayer;
        }

        public StaticCameraLocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer, float transitionTargetRate) : base(LocatorTypes.StaticCamera)
        {
            _targetPosition = targetPosition;
            _fov = fov;
            _targetLag = targetLag;
            _followPlayer = followPlayer;
            _hasTransitionTargetRate = true;
            _transitionTargetRate = transitionTargetRate;
        }

        public StaticCameraLocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer, float transitionTargetRate, bool oneShot, bool disableFOV) : base(LocatorTypes.StaticCamera)
        {
            _targetPosition = targetPosition;
            _fov = fov;
            _targetLag = targetLag;
            _followPlayer = followPlayer;
            _hasTransitionTargetRate = true;
            _transitionTargetRate = transitionTargetRate;
            _hasFlags = true;
            OneShot = oneShot;
            DisableFOV = disableFOV;
        }

        public StaticCameraLocatorData(Vector3 targetPosition, float fov, float targetLag, uint followPlayer, float transitionTargetRate, bool oneShot, bool disableFOV, bool cutInOut, bool carOnly, bool onFootOnly) : base(LocatorTypes.StaticCamera)
        {
            _targetPosition = targetPosition;
            _fov = fov;
            _targetLag = targetLag;
            _followPlayer = followPlayer;
            _hasTransitionTargetRate = true;
            _transitionTargetRate = transitionTargetRate;
            _hasFlags = true;
            OneShot = oneShot;
            DisableFOV = disableFOV;
            _hasFlags2 = true;
            CutInOut = cutInOut;
            CarOnly = carOnly;
            OnFootOnly = onFootOnly;
        }

        internal override LocatorData Clone() => (HasTransitionTargetRate, HasFlags, HasFlags2) switch
        {
            (true, true, true) =>
                new StaticCameraLocatorData(TargetPosition, FOV, TargetLag, _followPlayer,
                                        TransitionTargetRate, OneShot, DisableFOV, CutInOut, CarOnly, OnFootOnly),
            (true, true, _) =>
                new StaticCameraLocatorData(TargetPosition, FOV, TargetLag, _followPlayer,
                                        TransitionTargetRate, OneShot, DisableFOV),
            (true, _, _) =>
                new StaticCameraLocatorData(TargetPosition, FOV, TargetLag, _followPlayer,
                                        TransitionTargetRate),
            _ =>
                new StaticCameraLocatorData(TargetPosition, FOV, TargetLag, _followPlayer)
        };

        public override string ToString() => $"TargetPosition = {TargetPosition}, _fOV = {FOV}, _targetLag = {TargetLag}, FollowPlayer = {_followPlayer}, TransitionTargetRate = {(HasTransitionTargetRate ? TransitionTargetRate.ToString() : "null")}, OneShot = {OneShot}, DisableFOV = {DisableFOV}, CutInOut = {CutInOut}, CarOnly = {CarOnly}, OnFootOnly = {OnFootOnly}";
    }

    /// <summary>
    /// Ped Group locator data.
    /// </summary>
    public class PedGroupLocatorData : LocatorData
    {
        private uint _groupNum;
        public uint GroupNum
        {
            get => _groupNum;
            set
            {
                if (_groupNum == value)
                    return;
    
                _groupNum = value;
                OnPropertyChanged(nameof(GroupNum));
            }
        }
    

        public override uint DataLen => 1;
        public override List<uint> DataArray => [GroupNum];

        public PedGroupLocatorData(IList<uint> data) : base(LocatorTypes.PedGroup)
        {
            _groupNum = data[0];
        }

        public PedGroupLocatorData(uint groupNum) : base(LocatorTypes.PedGroup)
        {
            _groupNum = groupNum;
        }

        internal override LocatorData Clone() => new PedGroupLocatorData(GroupNum);

        public override string ToString() => $"_groupNum = {GroupNum}";
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

        public override string ToString() => "No data";
    }
}
