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
        public override Version Version => new Version(1, 2, 1);

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
                    Log.Debug($"Loaded {HiddenHudPlayers.Count} hidden HUD player(s).");
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
    }
}