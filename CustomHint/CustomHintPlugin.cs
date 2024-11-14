using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Handlers;
using Hints;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace CustomHintPlugin
{
    public class Config : IConfig
    {
        [Description("Plugin enabled (bool)?")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;

        // Configurable hint message
        [Description("Hint message")]
        public string HintMessage { get; set; } = "Hello World!";

        // List of roles that will not receive the hint
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

        // Track whether the round is currently in progress
        private bool _isRoundActive;

        // Override the Name, Author, and Version properties
        public override string Name => "CustomHint";
        public override string Author => "Narin";
        public override Version Version => new Version(1, 0, 0);

        public override void OnEnabled()
        {
            Instance = this;

            // Log the custom enabled message
            Log.Info($"Сосал?)");

            // Subscribe to round events
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            // Log the custom disabled message
            Log.Info($"Сосал?(");

            // Unsubscribe from round events
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            // Stop the hint coroutine if it's running
            Timing.KillCoroutines(_hintCoroutine);

            Instance = null;
            base.OnDisabled();
        }

        private void OnRoundStarted()
        {
            Log.Info($"{Name} is enabled for the new round.");

            // Set the round active flag and start the hint coroutine
            _isRoundActive = true;
            _hintCoroutine = Timing.RunCoroutine(ContinuousHintDisplay());
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Log.Info($"{Name} is now disabled until the next round.");

            // Stop displaying hints when the round ends
            _isRoundActive = false;
            Timing.KillCoroutines(_hintCoroutine);
        }

        private IEnumerator<float> ContinuousHintDisplay()
        {
            while (true)
            {
                // Only display hints if the round is active
                if (_isRoundActive)
                {
                    foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
                    {
                        // Show the hint to players who are not in excluded roles
                        if (!Config.ExcludedRoles.Contains(player.Role.Type))
                        {
                            DisplayHint(player);
                        }
                    }
                }

                yield return Timing.WaitForSeconds(1f); // Refreshes hint every 1 second to ensure continuous display
            }
        }

        private void DisplayHint(Exiled.API.Features.Player player)
        {
            string hintMessage = Config.HintMessage;

            // Displays the hint without a set duration, continuously refreshing every second
            player.ShowHint(hintMessage, 1f); // Updates every second with a 1-second duration
        }
    }
}
