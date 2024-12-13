using System.ComponentModel;
using Exiled.API.Interfaces;

namespace CustomHintPlugin
{
    public class Translations : ITranslation
    {
        [Description("Hint message for spectators.")]
        public string HintMessageForSpectators { get; set; } = "<size=75%>{servername}\n{ip}:{port}\n\n{player_nickname}, spec, duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.\nRole: {player_role}\nTPS: {tps}/60\n\nInformation:\nClass-D personnal: {classd_num} || Scientists: {scientist_num} || Facility Guards: {facilityguard_num} || MTF: {mtf_num} || CI: {ci_num} || SCPs: {scp_num} || Spectators: {spectators_num}\nGenerators activated: {generators_activated}/{generators_max}\n\n{hints}</size>";

        [Description("Hint message for rounds lasting up to 59 seconds.")]
        public string HintMessageUnderMinute { get; set; } = "<size=75%>{servername}\n{ip}:{port}\n\nQuick start! {player_nickname}, round time: {round_duration_seconds}s.\nRole: {player_role}\nTPS: {tps}/60</size>";

        [Description("Hint message for rounds lasting from 1 minute to 59 minutes and 59 seconds.")]
        public string HintMessageUnderHour { get; set; } = "<size=75%>{servername}\n{ip}:{port}\n\nStill going, {player_nickname}! Time: {round_duration_minutes}:{round_duration_seconds}.\nRole: {player_role}\nTPS: {tps}/60</size>";

        [Description("Hint message for rounds lasting 1 hour or more.")]
        public string HintMessageOverHour { get; set; } = "<size=75%>{servername}\n{ip}:{port}\n\nLong run, {player_nickname}! Duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.\nRole: {player_role}\nTPS: {tps}/60</size>";

        [Description("Message displayed when the HUD is successfully hidden.")]
        public string HideHudSuccessMessage { get; set; } = "<color=green>You have successfully hidden the server HUD! To get the HUD back, use .showhud</color>";

        [Description("Message displayed when the HUD is already hidden.")]
        public string HideHudAlreadyHiddenMessage { get; set; } = "<color=red>You've already hidden the server HUD.</color>";

        [Description("Message displayed when the HUD is successfully shown.")]
        public string ShowHudSuccessMessage { get; set; } = "<color=green>You have successfully returned the server HUD! To hide again, use .hidehud</color>";

        [Description("Message displayed when the HUD is already shown.")]
        public string ShowHudAlreadyShownMessage { get; set; } = "<color=red>You already have the server HUD displayed.</color>";

        [Description("Message displayed when DNT (Do Not Track) mode is enabled.")]
        public string DntEnabledMessage { get; set; } = "<color=red>Disable DNT (Do Not Track) mode.</color>";

        [Description("Message displayed when commands are disabled on the server.")]
        public string CommandDisabledMessage { get; set; } = "<color=red>This command is disabled on the server.</color>";
    }
}
