using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace CustomHintPlugin
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;

        // Configurable hint message
        public string HintMessage { get; set; } = "Hello World!";

        // List of roles that will not receive the hint
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

        public override void OnEnabled()
        {
            Instance = this;

            // Start the coroutine to display hints every second
            _hintCoroutine = Timing.RunCoroutine(ContinuousHintDisplay());

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            // Stop the hint coroutine
            Timing.KillCoroutines(_hintCoroutine);

            Instance = null;
            base.OnDisabled();
        }

        private IEnumerator<float> ContinuousHintDisplay()
        {
            while (true)
            {
                foreach (Player player in Player.List)
                {
                    // Show the hint to players who are not in excluded roles
                    if (!Config.ExcludedRoles.Contains(player.Role.Type))
                    {
                        DisplayHint(player);
                    }
                }
                yield return Timing.WaitForSeconds(1f); // Refreshes hint every 1 second to ensure continuous display
            }
        }

        private void DisplayHint(Player player)
        {
            string hintMessage = Config.HintMessage;

            // Displays the hint without a set duration, continuously refreshing every second
            player.ShowHint(hintMessage, 1f); // Updates every second with a 1-second duration
        }
    }
}
