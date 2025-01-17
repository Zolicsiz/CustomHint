using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace CustomHintPlugin
{
    public class Config : IConfig
    {
        [Description("Plugin enabled (bool)?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug mode?")]
        public bool Debug { get; set; } = false;

        [Description("Enable or disable automatic plugin updates.")]
        public bool AutoUpdater { get; set; } = true;

        [Description("Enable or disable hints for spectators.")]
        public bool HintForSpectatorsIsEnabled { get; set; } = true;

        [Description("The interval for changing spectator hints (in seconds).")]
        public float HintMessageTime { get; set; } = 5f;

        [Description("Default role name for players without a role.")]
        public string DefaultRoleName { get; set; } = "Player";

        [Description("Default role color (for players without roles).")]
        public string DefaultRoleColor { get; set; } = "white";

        [Description("Ignored roles.")]
        public List<RoleTypeId> ExcludedRoles { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Overwatch,
            RoleTypeId.Filmmaker,
            RoleTypeId.Scp079
        };

        [Description("Enable or disable HUD-related commands.")]
        public bool EnableHudCommands { get; set; } = true;
    }
}
