﻿using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Hazel;

namespace Impostor.Commands.Core
{
    public partial class Structures
    {
        public static class MessageFlag
        {
            public const string LoginApiRequest = "0";      // A request to log in, with a given API key.
            public const string LoginApiAccepted = "1";     // The API Key is correct, so the login is successful.
            public const string LoginApiRejected = "2";     // The API key is incorrect, so the login is rejected.
            public const string ConsoleLogMessage = "3";    // The only working text message, so far.
            public const string ConsoleCommand = "4";       // A command sent from the dashboard to the API.
            public const string HeartbeatMessage = "5";     // Data for the graphs.
            public const string GameListMessage = "6";      // A relic of the past.
            public const string DoKickOrDisconnect = "7";   // A message when a client is kicked (not implemented) or the server shuts down.
            public const string FetchLogs = "8";            // A specialized message. This is only server sided, and will indicate that a log file exists or not.
            public const string SelectProtocol = "x9";      // A special message, not used by the dashboard.
        }
        [Serializable]
        public class BaseMessage
        {
            /// <summary>
            /// The message data.
            /// </summary>
            public string Text { get; set; }
            /// <summary>
            /// The type of the message. JS, please no!!!
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// The UNIX date epoch.
            /// </summary>
            public ulong Date { get; set; }
            /// <summary>
            /// The source of the message.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Additional data for some messages (e.g the heartbeat)
            /// </summary>
            public float[] Flags { get; set; }
        }

        public static class DashboardCommands
        {
            public const string ServerWideBroadcast = "/broadcast";
            public const string HelpMessage = "/help";
            public const string ListKeys = "/keys";
            public const string DeleteKey = "/removekey";
            public const string AddKey = "/addkey";
            public const string PlayerInfo = "/playerinfo";
            public const string ListColors = "/broadcastcolors";
            public const string ListLogs = "/logs";
            public const string FetchLog = "/fetchlog";
            public const string AnnouncementMultiCommand = "/announcement";
        }

        public static class PlayerCommands
        {
            public const string MapChange = "/map";
            public const string ImpostorChange = "/impostors";
            public const string MaxPlayersChange = "/players";
            public const string Help = "/help";
        }

        public class Exceptions
        {
            public class PlayerNullException : Exception
            {
                public PlayerNullException() : base("Broadcast error : cannot get carrier player.") { }
            }

            public class CommandPrefixException : Exception
            {
                public CommandPrefixException() : base("Command registration error : Commands must start with '/'.") { }
            }

            public class PleaseProvideDocsException : Exception
            {
                public PleaseProvideDocsException() : base("Please provide docs for the command!") {}
            }
        }

        public class PluginConfiguration
        {
            #region Dashboard
            public bool UseSsl { get; set; }
            public List<string> ApiKeys { get; set; }
            public ushort WebApiPort { get; set; }
            public ushort WebsitePort { get; set; }
            public string ListenInterface { get; set; }
            public bool LegacyAnnouncement { get; set; }
            #endregion
            public static PluginConfiguration GetDefaultConfig()
            {
                var cfg = new PluginConfiguration();
                #region Dashboard
                cfg.UseSsl = false;
                cfg.ApiKeys = new List<string>() { Guid.NewGuid().ToString() };
                cfg.WebApiPort = 22023;
                cfg.WebsitePort = 22024;
                cfg.ListenInterface = "0.0.0.0";
                cfg.LegacyAnnouncement = false;
                #endregion
                return cfg;
            }

            public void SaveTo(string path)
            {
                if (File.Exists(path)) File.Delete(path);
                File.WriteAllText(path, JsonSerializer.Serialize<PluginConfiguration>(this));
            }

            public static PluginConfiguration LoadFrom(string path)
            {
                return JsonSerializer.Deserialize<PluginConfiguration>(File.ReadAllText(path));
            }
        }

        public class PlayerCommandConfiguration
        {
            public bool EnableMapChange { get; set; }
            public bool EnableImpostorChange { get; set; }
            public bool EnableMaxPlayersChange { get; set; }
            public bool EnableReportCommand { get; set; }
            public ushort ReportsRequiredForBan { get; set; }

            public static PlayerCommandConfiguration GetDefaultConfig()
            {
                return new PlayerCommandConfiguration() {
                    EnableMapChange = true,
                    EnableImpostorChange = true,
                    EnableMaxPlayersChange = true,
                    EnableReportCommand = false,
                    ReportsRequiredForBan = 10
                };
            }
            public void SaveTo(string path)
            {
                if (File.Exists(path)) File.Delete(path);
                File.WriteAllText(path, JsonSerializer.Serialize<PlayerCommandConfiguration>(this));
            }

            public static PlayerCommandConfiguration LoadFrom(string path)
            {
                return JsonSerializer.Deserialize<PlayerCommandConfiguration>(File.ReadAllText(path));
            }
        }

        public static class ServerSources
        {
            public const string CommandSystem = "cmd-sys";
            public const string DebugSystem = "dbg-sys";
            public const string DebugSystemCritical = "dbg-sys / CRITICAL";
            public const string SystemInfo = "sysinfo";
            public const string ExternalCall = "impostor/externalcall";
        }

        #region Game Enums
        public enum PlayerColor
        {
            Red, Blue, Green, Pink, Orange, Yellow, Black, White, Purple, Brown, Cyan, Lime
        }
        public enum HatId
        {
            NoHat = 0x00,
            Astronaut = 0x01,
            BaseballCap = 0x02,
            BrainSlug = 0x03,
            BushHat = 0x04,
            CaptainsHat = 0x05,
            DoubleTopHat = 0x06,
            Flowerpot = 0x07,
            Goggles = 0x08,
            HardHat = 0x09,
            Military = 0x0a,
            PaperHat = 0x0b,
            PartyHat = 0x0c,
            Police = 0x0d,
            Stethescope = 0x0e,
            TopHat = 0x0f,
            TowelWizard = 0x10,
            Ushanka = 0x11,
            Viking = 0x12,
            WallCap = 0x13,
            Snowman = 0x14,
            Reindeer = 0x15,
            Lights = 0x16,
            Santa = 0x17,
            Tree = 0x18,
            Present = 0x19,
            Candycanes = 0x1a,
            ElfHat = 0x1b,
            NewYears2018 = 0x1c,
            WhiteHat = 0x1d,
            Crown = 0x1e,
            Eyebrows = 0x1f,
            HaloHat = 0x20,
            HeroCap = 0x21,
            PipCap = 0x22,
            PlungerHat = 0x23,
            ScubaHat = 0x24,
            StickminHat = 0x25,
            StrawHat = 0x26,
            TenGallonHat = 0x27,
            ThirdEyeHat = 0x28,
            ToiletPaperHat = 0x29,
            Toppat = 0x2a,
            Fedora = 0x2b,
            Goggles_2 = 0x2c,
            Headphones = 0x2d,
            MaskHat = 0x2e,
            PaperMask = 0x2f,
            Security = 0x30,
            StrapHat = 0x31,
            Banana = 0x32,
            Beanie = 0x33,
            Bear = 0x34,
            Cheese = 0x35,
            Cherry = 0x36,
            Egg = 0x37,
            Fedora_2 = 0x38,
            Flamingo = 0x39,
            FlowerPin = 0x3a,
            Helmet = 0x3b,
            Plant = 0x3c,
            BatEyes = 0x3d,
            BatWings = 0x3e,
            Horns = 0x3f,
            Mohawk = 0x40,
            Pumpkin = 0x41,
            ScaryBag = 0x42,
            Witch = 0x43,
            Wolf = 0x44,
            Pirate = 0x45,
            Plague = 0x46,
            Machete = 0x47,
            Fred = 0x48,
            MinerCap = 0x49,
            WinterHat = 0x4a,
            Archae = 0x4b,
            Antenna = 0x4c,
            Balloon = 0x4d,
            BirdNest = 0x4e,
            BlackBelt = 0x4f,
            Caution = 0x50,
            Chef = 0x51,
            CopHat = 0x52,
            DoRag = 0x53,
            DumSticker = 0x54,
            Fez = 0x55,
            GeneralHat = 0x56,
            GreyThing = 0x57,
            HunterCap = 0x58,
            JungleHat = 0x59,
            MiniCrewmate = 0x5a,
            NinjaMask = 0x5b,
            RamHorns = 0x5c,
            Snowman_2 = 0x5d
        }
        public enum PetId
        {
            None = 0x00,
            Alien = 0x01,
            Crewmate = 0x02,
            Doggy = 0x03,
            Stickmin = 0x04,
            Hamster = 0x05,
            Robot = 0x06,
            UFO = 0x07,
            Ellie = 0x08,
            Squig = 0x09,
            Bedcrab = 0x0a
        }
        public enum SkinId
        {
            None = 0x00,
            Astro = 0x01,
            Capt = 0x02,
            Mech = 0x03,
            Military = 0x04,
            Police = 0x05,
            Science = 0x06,
            SuitB = 0x07,
            SuitW = 0x08,
            Wall = 0x09,
            Hazmat = 0x0a,
            Security = 0x0b,
            Tarmac = 0x0c,
            Miner = 0x0d,
            Winter = 0x0e,
            Archae = 0x0f
        }
        public enum RpcCalls : byte
        {
            PlayAnimation = 0,
            CompleteTask = 1,
            SyncSettings = 2,
            SetInfected = 3,
            Exiled = 4,
            CheckName = 5,
            SetName = 6,
            CheckColor = 7,
            SetColor = 8,
            SetHat = 9,
            SetSkin = 10,
            ReportDeadBody = 11,
            MurderPlayer = 12,
            SendChat = 13,
            StartMeeting = 14,
            SetScanner = 15,
            SendChatNote = 16,
            SetPet = 17,
            SetStartCounter = 18,
            EnterVent = 19,
            ExitVent = 20,
            SnapTo = 21,
            Close = 22,
            VotingComplete = 23,
            CastVote = 24,
            ClearVote = 25,
            AddVote = 26,
            CloseDoorsOfType = 27,
            RepairSystem = 28,
            SetTasks = 29,
            UpdateGameData = 30,
        }
        #endregion

        public static readonly byte[] MaxPlayers = new byte[]
        {
            10,8,6,4
        };
        public static readonly byte[] MaxImpostors = new byte[]
        {
            3,2,1
        };
        public static readonly string[] Maps = new string[]
        {
            "skeld","polus","mirahq"
        };

        public enum BroadcastType
        {
            Warning, Error, Information,Manual
        }
    }
}
