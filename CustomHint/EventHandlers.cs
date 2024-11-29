using System;
using MEC;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using System.Collections.Generic;
using PlayerRoles;
using System.Linq;
using System.IO;

namespace CustomHintPlugin
{
    public class EventHandlers
    {
        private CoroutineHandle _hintCoroutine;
        private DateTime _roundStartTime;
        private bool _isRoundActive;
        private List<string> spectatorHints = new List<string>();
        private Queue<string> randomizedHints = new Queue<string>();

        public void OnWaitingForPlayers()
        {
            if (Plugin.Instance.Config.Debug)
                Log.Debug("Waiting for players, disabling hints.");

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

            LoadSpectatorHints();
            _hintCoroutine = Timing.RunCoroutine(UpdateHints());
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (Plugin.Instance.Config.Debug)
                Log.Debug("Round ended, disabling hints.");

            _isRoundActive = false;
            _roundStartTime = default;

            Timing.KillCoroutines(_hintCoroutine);
            randomizedHints.Clear();
            Log.Debug("Cleared spectator hints queue.");
        }

        private IEnumerator<float> UpdateHints()
        {
            while (_isRoundActive)
            {
                TimeSpan roundDuration = DateTime.UtcNow - _roundStartTime;

                foreach (var player in Player.List)
                {
                    if (Plugin.Instance.IsHudHiddenForPlayer(player))
                        continue;

                    if (player.Role.Type == RoleTypeId.Spectator ||
                        (!Plugin.Instance.Config.ExcludedRoles.Contains(RoleTypeId.Overwatch) &&
                         player.Role.Type == RoleTypeId.Overwatch))
                    {
                        if (Plugin.Instance.Config.HintForSpectatorsIsEnabled)
                        {
                            UpdateSpectatorHint(player, roundDuration);
                        }
                    }
                    else if (!Plugin.Instance.Config.ExcludedRoles.Contains(player.Role.Type))
                    {
                        UpdatePlayerHint(player, roundDuration);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
        private void UpdatePlayerHint(Player player, TimeSpan roundDuration)
        {
            if (Plugin.Instance.IsHudHiddenForPlayer(player))
                return;

            string hintMessage = (roundDuration.TotalSeconds <= 59)
                ? Plugin.Instance.Config.HintMessageUnderMinute
                : (roundDuration.TotalMinutes < 60)
                ? Plugin.Instance.Config.HintMessageUnderHour
                : Plugin.Instance.Config.HintMessageOverHour;

            hintMessage = hintMessage
                .Replace("{round_duration_hours}", roundDuration.Hours.ToString("D2"))
                .Replace("{round_duration_minutes}", roundDuration.Minutes.ToString("D2"))
                .Replace("{round_duration_seconds}", roundDuration.Seconds.ToString("D2"))
                .Replace("{player_nickname}", player.Nickname)
                .Replace("{player_role}", GetColoredRoleName(player))
                .Replace("{tps}", Server.Tps.ToString("F1"))
                .Replace("{servername}", Server.Name)
                .Replace("{ip}", Server.IpAddress)
                .Replace("{port}", Server.Port.ToString());

            hintMessage = Plugin.ReplaceColorsInString(hintMessage);

            player.ShowHint(hintMessage, 1f);
        }

        private void UpdateSpectatorHint(Player player, TimeSpan roundDuration)
        {
            if (Plugin.Instance.IsHudHiddenForPlayer(player))
                return;

            if (randomizedHints.Count == 0 && spectatorHints.Count > 0)
            {
                randomizedHints = new Queue<string>(spectatorHints.OrderBy(_ => Guid.NewGuid()));
            }

            if (randomizedHints.Count == 0)
                return;

            string currentHint = randomizedHints.Dequeue();

            string hintMessage = Plugin.Instance.Config.HintMessageForSpectators
                .Replace("{round_duration_hours}", roundDuration.Hours.ToString("D2"))
                .Replace("{round_duration_minutes}", roundDuration.Minutes.ToString("D2"))
                .Replace("{round_duration_seconds}", roundDuration.Seconds.ToString("D2"))
                .Replace("{player_nickname}", player.Nickname)
                .Replace("{player_role}", GetColoredRoleName(player))
                .Replace("{tps}", Server.Tps.ToString("F1"))
                .Replace("{servername}", Server.Name)
                .Replace("{ip}", Server.IpAddress)
                .Replace("{port}", Server.Port.ToString())
                .Replace("{hints}", currentHint);

            hintMessage = Plugin.ReplaceColorsInString(hintMessage);

            player.ShowHint(hintMessage, 1f);
        }

        private string GetColoredRoleName(Player player)
        {
            return player.Group != null
                ? $"<color={player.Group.BadgeColor ?? Plugin.Instance.Config.DefaultRoleColor}>{player.Group.BadgeText}</color>"
                : $"<color={Plugin.Instance.Config.DefaultRoleColor}>{Plugin.Instance.Config.DefaultRoleName}</color>";
        }

        public void LoadSpectatorHints()
        {
            string hintsFilePath = FileDotNet.GetPath("SpectatorHints.txt");

            try
            {
                if (File.Exists(hintsFilePath))
                {
                    spectatorHints = File.ReadAllLines(hintsFilePath)
                        .Where(line => !line.TrimStart().StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                        .ToList();

                    Log.Debug($"Loaded {spectatorHints.Count} spectator hints.");
                }
                else
                {
                    Log.Warn("SpectatorHints.txt not found. No hints loaded.");
                    spectatorHints = new List<string>();
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to load SpectatorHints.txt: {ex}");
                spectatorHints = new List<string>();
            }
        }

        public static string ReplaceColorsInString(string input)
        {
            const string serverNamePlaceholder = "[SERVERNAME_PLACEHOLDER]";
            input = input.Replace("{servername}", serverNamePlaceholder);

            Dictionary<string, string> colorMapping = new Dictionary<string, string>
            {
                { "red", "#FF0000" },
                { "green", "#00FF00" },
                { "blue", "#0000FF" },
                { "yellow", "#FFFF00" },
                { "white", "#FFFFFF" },
                { "black", "#000000" }
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