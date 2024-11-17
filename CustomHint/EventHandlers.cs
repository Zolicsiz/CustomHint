using System;
using MEC;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using System.Collections.Generic;
using HintAPI;

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

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Log.Info("Round ended, disabling hints.");
            _isRoundActive = false;
            Timing.KillCoroutines(_hintCoroutine);
        }

        private IEnumerator<float> ContinuousHintDisplay()
        {
            while (_isRoundActive)
            {
                foreach (var player in Player.List)
                {
                    if (!CustomHintPlugin.Instance.Config.ExcludedRoles.Contains(player.Role.Type) &&
                        !CustomHintPlugin.Instance.HiddenHudPlayers.Contains(player.UserId))
                    {
                        DisplayHint(player);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        private void DisplayHint(Player player)
        {
            var roundDuration = DateTime.UtcNow - _roundStartTime;
            var hintMessage = CustomHintPlugin.Instance.Config.HintMessageUnderMinute
                .Replace("{round_duration_hours}", roundDuration.Hours.ToString("D2"))
                .Replace("{round_duration_minutes}", roundDuration.Minutes.ToString("D2"))
                .Replace("{round_duration_seconds}", roundDuration.Seconds.ToString("D2"))
                .Replace("{player_nickname}", player.Nickname)
                .Replace("{player_role}", GetColoredRoleName(player))
                .Replace("{tps}", Server.Tps.ToString("F1"));

            string hintKey = $"CustomHint_{player.Id}";

            HintAPI.HintAPI.Instance.HintManager.RemoveHint(player, hintKey);

            HintAPI.HintAPI.Instance.HintManager.RegisterHint(player, hintMessage, 1f, 1, hintKey);
        }




        private string GetColoredRoleName(Player player)
        {
            return player.Group != null
                ? $"<color={player.Group.BadgeColor ?? CustomHintPlugin.Instance.Config.DefaultRoleColor}>{player.Group.BadgeText}</color>"
                : $"<color={CustomHintPlugin.Instance.Config.DefaultRoleColor}>{CustomHintPlugin.Instance.Config.DefaultRoleName}</color>";
        }
    }
}
