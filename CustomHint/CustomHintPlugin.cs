using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Handlers;
using Hints;
using MEC;
using PlayerRoles;
using RemoteAdmin;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CustomHintPlugin
{
    public class Config : IConfig
    {
        [Description("Plugin enabled (bool)?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug mode (bool)?")]
        public bool Debug { get; set; } = false;

        [Description("Hint message for rounds lasting up to 59 seconds.")]
        public string HintMessageUnderMinute { get; set; } = "Quick start! {player_nickname}, round time: {round_duration_seconds}s.\nRole: {player_role}\nTPS: {tps}/60";

        [Description("Hint message for rounds lasting from 1 minute to 59 minutes and 59 seconds.")]
        public string HintMessageUnderHour { get; set; } = "Still going, {player_nickname}! Time: {round_duration_minutes}:{round_duration_seconds}.\nRole: {player_role}\nTPS: {tps}/60";

        [Description("Hint message for rounds lasting 1 hour or more.")]
        public string HintMessageOverHour { get; set; } = "Long run, {player_nickname}! Duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.\nRole: {player_role}\nTPS: {tps}/60";

        [Description("Default role name for players without a role.")]
        public string DefaultRoleName { get; set; } = "Player";

        [Description("Default role color (for players without roles).")]
        public string DefaultRoleColor { get; set; } = "white";

        [Description("Ignored roles")]
        public List<RoleTypeId> ExcludedRoles { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Overwatch,
            RoleTypeId.Spectator,
            RoleTypeId.Filmmaker
        };

        [Description("Enable or disable HUD-related commands (.hidehud and .showhud).")]
        public bool EnableHudCommands { get; set; } = true;

        [Description("Message displayed when the HUD is successfully hidden.")]
        public string HideHudSuccessMessage { get; set; } = "<color=green>You have successfully hidden the server HUD! To get the HUD back, use .showhud</color>";

        [Description("Message displayed when the HUD is already hidden.")]
        public string HideHudAlreadyHiddenMessage { get; set; } = "<color=red>You've already hidden the HUD server.</color>";

        [Description("Message displayed when the HUD is successfully shown.")]
        public string ShowHudSuccessMessage { get; set; } = "<color=green>You have successfully returned the server HUD! To hide again, use .hidehud</color>";

        [Description("Message displayed when the HUD is already shown.")]
        public string ShowHudAlreadyShownMessage { get; set; } = "<color=red>You already have the server HUD displayed.</color>";

        [Description("Message displayed when DNT (Do Not Track) mode is enabled.")]
        public string DntEnabledMessage { get; set; } = "<color=red>Disable DNT (Do Not Track) mode.</color>";

        [Description("Message displayed when commands are disabled on the server.")]
        public string CommandDisabledMessage { get; set; } = "<color=red>This command is disabled on the server.</color>";
    }

    public class CustomHintPlugin : Plugin<Config>
    {
        public static CustomHintPlugin Instance;
        private CoroutineHandle _hintCoroutine;
        private bool _isRoundActive;
        private DateTime _roundStartTime;

        private const string HudConfigFolder = "Exiled/Configs/CustomHint";
        private const string HudConfigFile = HudConfigFolder + "/HiddenHudPlayers.yml";

        private static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private static readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public HashSet<string> HiddenHudPlayers { get; private set; } = new HashSet<string>();

        public override string Name => "CustomHint";
        public override string Author => "Narin";
        public override Version Version => new Version(1, 1, 0);

        public override void OnEnabled()
        {
            Instance = this;

            if (!Config.IsEnabled)
            {
                Log.Warn("Plugin is disabled in the configuration.");
                return;
            }

            LoadHiddenHudPlayers();

            Log.Info($"Plugin {Name} enabled with configuration.");
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Timing.KillCoroutines(_hintCoroutine);
            SaveHiddenHudPlayers();
            Instance = null;

            Log.Info($"Plugin {Name} disabled.");
            base.OnDisabled();
        }

        private void OnRoundStarted()
        {
            Log.Info($"{Name} is enabled for the new round.");
            _isRoundActive = true;
            _roundStartTime = DateTime.UtcNow;
            _hintCoroutine = Timing.RunCoroutine(ContinuousHintDisplay());
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Log.Info($"{Name} is now disabled until the next round.");
            _isRoundActive = false;
            Timing.KillCoroutines(_hintCoroutine);
        }

        private IEnumerator<float> ContinuousHintDisplay()
        {
            while (true)
            {
                if (_isRoundActive)
                {
                    TimeSpan roundDuration = DateTime.UtcNow - _roundStartTime;

                    foreach (var player in Exiled.API.Features.Player.List)
                    {
                        if (!Config.ExcludedRoles.Contains(player.Role.Type) && !HiddenHudPlayers.Contains(player.UserId))
                        {
                            DisplayHint(player, roundDuration);
                        }
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        private void DisplayHint(Exiled.API.Features.Player player, TimeSpan roundDuration)
        {
            string hintMessage = GetHintMessage(roundDuration)
                .Replace("{round_duration_hours}", roundDuration.Hours.ToString("D2"))
                .Replace("{round_duration_minutes}", roundDuration.Minutes.ToString("D2"))
                .Replace("{round_duration_seconds}", roundDuration.Seconds.ToString("D2"))
                .Replace("{player_nickname}", player.Nickname)
                .Replace("{player_role}", GetColoredRoleName(player))
                .Replace("{tps}", Exiled.API.Features.Server.Tps.ToString("F1"));

            player.ShowHint(hintMessage, 1f);
        }

        private string GetHintMessage(TimeSpan roundDuration)
        {
            if (roundDuration.TotalSeconds <= 59)
                return Config.HintMessageUnderMinute;
            if (roundDuration.TotalMinutes < 60)
                return Config.HintMessageUnderHour;

            return Config.HintMessageOverHour;
        }

        private string GetColoredRoleName(Exiled.API.Features.Player player)
        {
            if (player.Group != null)
            {
                string roleName = player.Group.BadgeText;
                string roleColor = player.Group.BadgeColor ?? Config.DefaultRoleColor;

                return $"<color={roleColor}>{roleName}</color>";
            }

            return $"<color={Config.DefaultRoleColor}>{Config.DefaultRoleName}</color>";
        }

        private void LoadHiddenHudPlayers()
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
                    Log.Info($"Loaded {HiddenHudPlayers.Count} hidden HUD player(s) from the configuration file.");
                }
                else
                {
                    HiddenHudPlayers = new HashSet<string>();
                    SaveHiddenHudPlayers(); // Создаем файл с пустым списком.
                    Log.Info("Hidden HUD players file not found. Created a new one with default settings.");
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to load HUD configuration: {ex}");
                HiddenHudPlayers = new HashSet<string>();
            }
        }

        private void SaveHiddenHudPlayers()
        {
            try
            {
                string yamlContent = Serializer.Serialize(HiddenHudPlayers);
                File.WriteAllText(HudConfigFile, yamlContent);
                Log.Info("Saved hidden HUD players to the configuration file.");
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to save HUD configuration: {ex}");
            }
        }

        [CommandHandler(typeof(ClientCommandHandler))]
        public class HideHudCommand : ICommand
        {
            public string Command => "hidehud";
            public string[] Aliases => new string[0];
            public string Description => "Hide the server HUD";

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                if (!CustomHintPlugin.Instance.Config.EnableHudCommands)
                {
                    response = CustomHintPlugin.Instance.Config.CommandDisabledMessage;
                    return false;
                }

                if (sender is PlayerCommandSender playerSender)
                {
                    var player = Exiled.API.Features.Player.Get(playerSender.ReferenceHub);

                    if (player == null || player.DoNotTrack)
                    {
                        response = CustomHintPlugin.Instance.Config.DntEnabledMessage;
                        return false;
                    }

                    if (CustomHintPlugin.Instance.HiddenHudPlayers.Contains(player.UserId))
                    {
                        response = CustomHintPlugin.Instance.Config.HideHudAlreadyHiddenMessage;
                        return false;
                    }

                    CustomHintPlugin.Instance.HiddenHudPlayers.Add(player.UserId);
                    CustomHintPlugin.Instance.SaveHiddenHudPlayers();
                    response = CustomHintPlugin.Instance.Config.HideHudSuccessMessage;
                    return true;
                }

                response = "This command is for players only.";
                return false;
            }
        }

        [CommandHandler(typeof(ClientCommandHandler))]
        public class ShowHudCommand : ICommand
        {
            public string Command => "showhud";
            public string[] Aliases => new string[0];
            public string Description => "Show the server HUD";

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                if (!CustomHintPlugin.Instance.Config.EnableHudCommands)
                {
                    response = CustomHintPlugin.Instance.Config.CommandDisabledMessage;
                    return false;
                }

                if (sender is PlayerCommandSender playerSender)
                {
                    var player = Exiled.API.Features.Player.Get(playerSender.ReferenceHub);

                    if (player == null || player.DoNotTrack)
                    {
                        response = CustomHintPlugin.Instance.Config.DntEnabledMessage;
                        return false;
                    }

                    if (!CustomHintPlugin.Instance.HiddenHudPlayers.Contains(player.UserId))
                    {
                        response = CustomHintPlugin.Instance.Config.ShowHudAlreadyShownMessage;
                        return false;
                    }

                    CustomHintPlugin.Instance.HiddenHudPlayers.Remove(player.UserId);
                    CustomHintPlugin.Instance.SaveHiddenHudPlayers();
                    response = CustomHintPlugin.Instance.Config.ShowHudSuccessMessage;
                    return true;
                }

                response = "This command is for players only.";
                return false;
            }
        }
    }
}
