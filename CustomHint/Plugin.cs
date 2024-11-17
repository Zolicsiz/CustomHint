using System;
using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using MEC;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using HintAPI;

namespace CustomHintPlugin
{
    public class CustomHintPlugin : Plugin<Config>
    {
        public static CustomHintPlugin Instance { get; private set; }
        public EventHandlers EventHandlers { get; private set; }
        public HashSet<string> HiddenHudPlayers { get; private set; } = new HashSet<string>();

        private CoroutineHandle _hintCoroutine;

        private const string HudConfigFolder = "Exiled/Configs/CustomHint";
        private const string HudConfigFile = HudConfigFolder + "/HiddenHudPlayers.yml";

        private static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private static readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public override string Name => "CustomHint";
        public override string Author => "Narin";
        public override Version Version => new Version(1, 2, 0);

        public override void OnEnabled()
        {
            Instance = this;

            if (!Config.IsEnabled)
            {
                Log.Warn("Plugin is disabled in the configuration.");
                return;
            }

            LoadHiddenHudPlayers();

            EventHandlers = new EventHandlers();
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += EventHandlers.OnRoundEnded;

            Log.Info($"{Name} has been enabled.");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnded;

            Timing.KillCoroutines(_hintCoroutine);
            SaveHiddenHudPlayers();

            Instance = null;
            EventHandlers = null;

            Log.Info($"{Name} has been disabled.");
            base.OnDisabled();
        }

        public void LoadHiddenHudPlayers()
        {
            try
            {
                if (!Directory.Exists(HudConfigFolder))
                {
                    Directory.CreateDirectory(HudConfigFolder);
                }

                if (File.Exists(HudConfigFile))
                {
                    string yamlContent = File.ReadAllText(HudConfigFile);
                    HiddenHudPlayers = Deserializer.Deserialize<HashSet<string>>(yamlContent) ?? new HashSet<string>();
                    Log.Info($"Loaded {HiddenHudPlayers.Count} hidden HUD player(s).");
                }
                else
                {
                    HiddenHudPlayers = new HashSet<string>();
                    SaveHiddenHudPlayers();
                    Log.Info("Created new HUD configuration file.");
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
                string yamlContent = Serializer.Serialize(HiddenHudPlayers);
                File.WriteAllText(HudConfigFile, yamlContent);
                Log.Info("Saved hidden HUD players.");
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to save hidden HUD players: {ex}");
            }
        }
    }
}
