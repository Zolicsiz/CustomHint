# Custom Hint
Plugin that allows you to display text on the player's screen.  
It is recommended to use `AdvancedHints.dll` for convenience.  
For suggestions, please write to Discord: @narin4ik  

**Default cfg:**   
```
custom_hint:
# Plugin enabled (bool)?
  is_enabled: true
  # Debug mode?
  debug: false
  # Enable or disable hints for spectators.
  hint_for_spectators_is_enabled: true
  # The interval for changing spectator hints (in seconds).
  hint_message_time: 5
  # Hint message for spectators.
  hint_message_for_spectators: |-
    {servername}
    {ip}:{port}

    {player_nickname}, spec, duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60

    {hints}
  # Hint message for rounds lasting up to 59 seconds.
  hint_message_under_minute: |-
    {servername}
    {ip}:{port}

    Quick start! {player_nickname}, round time: {round_duration_seconds}s.
    Role: {player_role}
    TPS: {tps}/60
  # Hint message for rounds lasting from 1 minute to 59 minutes and 59 seconds.
  hint_message_under_hour: |-
    {servername}
    {ip}:{port}

    Still going, {player_nickname}! Time: {round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60
  # Hint message for rounds lasting 1 hour or more.
  hint_message_over_hour: |-
    {servername}
    {ip}:{port}

    Long run, {player_nickname}! Duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60
  # Default role name for players without a role.
  default_role_name: 'Player'
  # Default role color (for players without roles).
  default_role_color: 'white'
  # Ignored roles.
  excluded_roles:
  - Overwatch
  - Filmmaker
  - Scp079
  # Enable or disable HUD-related commands.
  enable_hud_commands: true
  # Message displayed when the HUD is successfully hidden.
  hide_hud_success_message: '<color=green>You have successfully hidden the server HUD! To get the HUD back, use .showhud</color>'
  # Message displayed when the HUD is already hidden.
  hide_hud_already_hidden_message: '<color=red>You''ve already hidden the HUD server.</color>'
  # Message displayed when the HUD is successfully shown.
  show_hud_success_message: '<color=green>You have successfully returned the server HUD! To hide again, use .hidehud</color>'
  # Message displayed when the HUD is already shown.
  show_hud_already_shown_message: '<color=red>You already have the server HUD displayed.</color>'
  # Message displayed when DNT (Do Not Track) mode is enabled.
  dnt_enabled_message: '<color=red>Disable DNT (Do Not Track) mode.</color>'
  # Message displayed when commands are disabled on the server.
  command_disabled_message: '<color=red>This command is disabled on the server.</color>'
  # Message displayed when the hint system is toggled on.
  toggle_on_message: '<color=green>Hint system and commands are now enabled for this round.</color>'
  # Message displayed when the hint system is toggled off.
  toggle_off_message: '<color=red>Hint system and commands are now disabled for this round.</color>'
  # Message displayed when commands are disabled for the round.
  commands_disabled_message: '<color=red>Commands .showhud and .hidehud are disabled for this round.</color>'
```
**Placeholders:**  
| Placeholder  | Description |
| ------------- | ------------- |
| {servername}  | Server name.  |
| {ip}  | Server IP.  |
| {port}  | Server port.  |
| {tps}  | Server TPS.  |
| {player_nickname}  | Player's nickname.  |
| {player_role}  | Player Role.  |
| {round_duration_seconds}  | Round duration in seconds.  |
| {round_duration_minutes}  | Round duration in minutes.  |
| {round_duration_hours}  | Round duration in hours.  |
| {hints}  | Hints from SpectatorHints.txt for spectators.  |
