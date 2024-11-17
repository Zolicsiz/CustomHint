using System;
using MEC;
using Exiled.API.Features;
using System.Collections.Generic;

namespace CustomHintPlugin
{
    public class EventHandlers
    {
        private CoroutineHandle _hintCoroutine;
        private DateTime _roundStartTime;
        private bool _isRoundActive;

        public void OnRoundStarted()
        {
            Log.Info("Round started, enabling hints.");
            _isRoundActive = true;
            _roundStartTime = DateTime.UtcNow;
            _hintCoroutine = Timing.RunCoroutine(ContinuousHintDisplay());
        }

        public void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            Log.Info("Round ended, disabling hints.");
            _isRoundActive = false;
            Timing.KillCoroutines(_hintCoroutine);
        }

        private IEnumerator<float> ContinuousHintDisplay()
        {
            while (_isRoundActive)
            {
                TimeSpan roundDuration = DateTime.UtcNow - _roundStartTime;

                foreach (var player in Player.List)
                {
                    if (!CustomHintPlugin.Instance.Config.ExcludedRoles.Contains(player.Role.Type) &&
                        !CustomHintPlugin.Instance.HiddenHudPlayers.Contains(player.UserId))
                    {
                        DisplayHint(player, roundDuration);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        private void DisplayHint(Player player, TimeSpan roundDuration)
        {
            string hintMessage = GetHintMessage(roundDuration)
                .Replace("{round_duration_hours}", roundDuration.Hours.ToString("D2"))
                .Replace("{round_duration_minutes}", roundDuration.Minutes.ToString("D2"))
                .Replace("{round_duration_seconds}", roundDuration.Seconds.ToString("D2"))
                .Replace("{player_nickname}", player.Nickname)
                .Replace("{player_role}", GetColoredRoleName(player))
                .Replace("{tps}", Server.Tps.ToString("F1"));

            HintAPI.HintAPI.Instance.HintManager.RegisterHint(player, hintMessage, 1f, 1, $"CustomHint_{player.Id}");
        }

        private string GetHintMessage(TimeSpan roundDuration)
        {
            if (roundDuration.TotalSeconds <= 59)
                return CustomHintPlugin.Instance.Config.HintMessageUnderMinute;
            if (roundDuration.TotalMinutes < 60)
                return CustomHintPlugin.Instance.Config.HintMessageUnderHour;
            return CustomHintPlugin.Instance.Config.HintMessageOverHour;
        }

        private string GetColoredRoleName(Player player)
        {
            if (player.Group != null)
            {
                string roleName = player.Group.BadgeText;
                string roleColor = player.Group.BadgeColor ?? CustomHintPlugin.Instance.Config.DefaultRoleColor;
                return $"<color={roleColor}>{roleName}</color>";
            }

            return $"<color={CustomHintPlugin.Instance.Config.DefaultRoleColor}>{CustomHintPlugin.Instance.Config.DefaultRoleName}</color>";
        }
    }
}
