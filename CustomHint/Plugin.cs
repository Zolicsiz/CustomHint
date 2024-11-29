using System;
using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using MEC;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CustomHintPlugin
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }
        public EventHandlers EventHandlers { get; private set; }
        public HashSet<string> HiddenHudPlayers { get; private set; } = new HashSet<string>();

        private CoroutineHandle _hintCoroutine;

        private string HudConfig = FileDotNet.GetPath("HiddenHudPlayers.yml");

        private static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private static readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public bool IsHintSystemEnabled { get; internal set; } = true;

        public override string Name => "CustomHint";
        public override string Author => "Narin";
        public override Version Version => new Version(1, 2, 3);

        public override void OnEnabled()
        {
            Instance = this;

            if (!Config.IsEnabled)
            {
                return;
            }

            LoadHiddenHudPlayers();

            EventHandlers = new EventHandlers();
            Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += EventHandlers.OnRoundEnded;

            Log.Debug($"{Name} has been enabled.");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnded;

            Timing.KillCoroutines(_hintCoroutine);
            SaveHiddenHudPlayers();

            Instance = null;
            EventHandlers = null;

            Log.Debug($"{Name} has been disabled.");
            base.OnDisabled();
        }

        public void LoadHiddenHudPlayers()
        {
            try
            {
                string directory = Path.GetDirectoryName(HudConfig);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(HudConfig))
                {
                    string yamlContent = File.ReadAllText(HudConfig);
                    HiddenHudPlayers = Deserializer.Deserialize<HashSet<string>>(yamlContent) ?? new HashSet<string>();
                    Log.Debug($"Loaded {HiddenHudPlayers.Count} hidden HUD player(s): {string.Join(", ", HiddenHudPlayers)}");
                }
                else
                {
                    HiddenHudPlayers = new HashSet<string>();
                    SaveHiddenHudPlayers();
                    Log.Debug("Created new HUD configuration file.");
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to load hidden HUD players: {ex}");
                HiddenHudPlayers = new HashSet<string>();
            }
        }

        public void SaveHiddenHudPlayers()
        {
            try
            {
                var validPlayers = new HashSet<string>();
                foreach (var userId in HiddenHudPlayers)
                {
                    var player = Player.Get(userId);
                    if (player == null || !player.DoNotTrack)
                    {
                        validPlayers.Add(userId);
                    }
                }

                string yamlContent = Serializer.Serialize(validPlayers);

                yamlContent = "# List of SteamID64 players who have HUD disabled.\n" + yamlContent;

                File.WriteAllText(HudConfig, yamlContent);
                Log.Debug("Saved hidden HUD players.");
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to save hidden HUD players: {ex}");
            }
        }

        public bool IsHudHiddenForPlayer(Player player)
        {
            if (player.DoNotTrack)
                return false;

            return HiddenHudPlayers.Contains(player.UserId);
        }

        public static string ReplaceColorsInString(string input)
        {
            const string serverNamePlaceholder = "[SERVERNAME_PLACEHOLDER]";
            input = input.Replace("{servername}", serverNamePlaceholder);

            Dictionary<string, string> colorMapping = new Dictionary<string, string>
            {
                { "pink", "#FF96DE" },
                { "red", "#C50000" },
                { "brown", "#944710" },
                { "silver", "#A0A0A0" },
                { "light_green", "#32CD32" },
                { "crimson", "#DC143C" },
                { "cyan", "#00B7EB" },
                { "aqua", "#00FFFF" },
                { "deep_pink", "#FF1493" },
                { "tomato", "#FF6448" },
                { "yellow", "#FAFF86" },
                { "magenta", "#FF0090" },
                { "blue_green", "#4DFFB8" },
                { "orange", "#FF9666" },
                { "lime", "#BFFF00" },
                { "green", "#228B22" },
                { "emerald", "#50C878" },
                { "carmine", "#960018" },
                { "nickel", "#727472" },
                { "mint", "#98FF98" },
                { "army_green", "#4B5320" },
                { "pumpkin", "#EE7600" }
            };

            foreach (var pair in colorMapping)
            {
                string oldTag = $"<color={pair.Key}>";
                string newTag = $"<color={pair.Value}>";
                input = input.Replace(oldTag, newTag, StringComparison.OrdinalIgnoreCase);
            }

            input = input.Replace(serverNamePlaceholder, "{servername}");
            return input;
        }
    }
}
