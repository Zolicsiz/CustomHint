using System;
using MEC;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using System.Collections.Generic;

namespace CustomHintPlugin
{
    public class EventHandlers
    {
        private CoroutineHandle _hintCoroutine;
        private DateTime _roundStartTime;
        private bool _isRoundActive;

        public void OnWaitingForPlayers()
        {
            if (Plugin.Instance.Config.Debug)
                Log.Debug("Waiting for players event triggered, disabling hints.");

            _isRoundActive = false;

            Timing.KillCoroutines(_hintCoroutine);

            Plugin.Instance.HiddenHudPlayers.Clear();
        }


        public void OnRoundStarted()
        {
            if (Plugin.Instance.Config.Debug)
                Log.Debug("Round started, enabling hints.");

            _isRoundActive = true;
            _roundStartTime = DateTime.UtcNow;
            _hintCoroutine = Timing.RunCoroutine(ContinuousHintDisplay());
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (Plugin.Instance.Config.Debug)
                Log.Debug("Round ended, disabling hints.");

            _isRoundActive = false;
            _roundStartTime = default;
            Timing.KillCoroutines(_hintCoroutine);
        }

        private IEnumerator<float> ContinuousHintDisplay()
        {
            while (_isRoundActive)
            {
                if (Plugin.Instance.IsHintSystemEnabled)
                {
                    TimeSpan roundDuration = DateTime.UtcNow - _roundStartTime;

                    foreach (var player in Player.List)
                    {
                        if (!Plugin.Instance.Config.ExcludedRoles.Contains(player.Role.Type) &&
                            !Plugin.Instance.HiddenHudPlayers.Contains(player.UserId))
                        {
                            DisplayHint(player, roundDuration);
                        }
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        private void DisplayHint(Player player, TimeSpan roundDuration)
        {
            if (Plugin.Instance.IsHudHiddenForPlayer(player))
                return;

            string serverName = Server.Name;
            string serverIp = Server.IpAddress;
            string serverPort = Server.Port.ToString();

            string hintMessage = GetHintMessage(roundDuration)
                .Replace("{round_duration_hours}", roundDuration.Hours.ToString("D2"))
                .Replace("{round_duration_minutes}", roundDuration.Minutes.ToString("D2"))
                .Replace("{round_duration_seconds}", roundDuration.Seconds.ToString("D2"))
                .Replace("{player_nickname}", player.Nickname)
                .Replace("{player_role}", GetColoredRoleName(player))
                .Replace("{tps}", Server.Tps.ToString("F1"))
                .Replace("{servername}", serverName)
                .Replace("{ip}", serverIp)
                .Replace("{port}", serverPort);

            player.ShowHint(hintMessage, 1f);
        }


        private string GetHintMessage(TimeSpan roundDuration)
        {
            if (roundDuration.TotalSeconds <= 59)
                return Plugin.Instance.Config.HintMessageUnderMinute;
            if (roundDuration.TotalMinutes < 60)
                return Plugin.Instance.Config.HintMessageUnderHour;

            return Plugin.Instance.Config.HintMessageOverHour;
        }

        private string GetColoredRoleName(Player player)
        {
            return player.Group != null
                ? $"<color={player.Group.BadgeColor ?? Plugin.Instance.Config.DefaultRoleColor}>{player.Group.BadgeText}</color>"
                : $"<color={Plugin.Instance.Config.DefaultRoleColor}>{Plugin.Instance.Config.DefaultRoleName}</color>";
        }
    }
}