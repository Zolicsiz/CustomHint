using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Handlers;
using Hints;
using MEC;
using PlayerRoles;

namespace CustomHintPlugin
{
    public class Config : IConfig
    {
        [Description("Plugin enabled (bool)?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug mode (bool)?")]
        public bool Debug { get; set; } = false;

        [Description("Hint message for rounds lasting up to 59 seconds.")]
        public string HintMessageUnderMinute { get; set; } = "Quick start! {player_nickname}, round time: {round_duration_seconds}s.\nRole: {player_role}";

        [Description("Hint message for rounds lasting from 1 minute to 59 minutes and 59 seconds.")]
        public string HintMessageUnderHour { get; set; } = "Still going, {player_nickname}! Time: {round_duration_minutes}:{round_duration_seconds}.\nRole: {player_role}";

        [Description("Hint message for rounds lasting 1 hour or more.")]
        public string HintMessageOverHour { get; set; } = "Long run, {player_nickname}! Duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.\nRole: {player_role}";

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
    }

    public class CustomHintPlugin : Plugin<Config>
    {
        public static CustomHintPlugin Instance;
        private CoroutineHandle _hintCoroutine;

        private bool _isRoundActive;
        private DateTime _roundStartTime;

        public override string Name => "CustomHint";
        public override string Author => "Narin";
        public override Version Version => new Version(1, 3, 0);

        public override void OnEnabled()
        {
            Instance = this;

            if (!Config.IsEnabled)
            {
                Log.Warn("Plugin is disabled in the configuration.");
                return;
            }

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
                        if (!Config.ExcludedRoles.Contains(player.Role.Type))
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
            string hintMessage;

            if (roundDuration.TotalSeconds <= 59)
            {
                hintMessage = Config.HintMessageUnderMinute;
            }
            else if (roundDuration.TotalMinutes < 60)
            {
                hintMessage = Config.HintMessageUnderHour;
            }
            else
            {
                hintMessage = Config.HintMessageOverHour;
            }

            string playerRole = GetColoredRoleName(player);

            hintMessage = hintMessage
                .Replace("{round_duration_hours}", roundDuration.Hours.ToString("D2"))
                .Replace("{round_duration_minutes}", roundDuration.Minutes.ToString("D2"))
                .Replace("{round_duration_seconds}", roundDuration.Seconds.ToString("D2"))
                .Replace("{player_nickname}", player.Nickname)
                .Replace("{player_role}", playerRole)
                .Replace("\\n", Environment.NewLine);

            player.ShowHint(hintMessage, 1f);
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
    }
}
