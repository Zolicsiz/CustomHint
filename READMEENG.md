# CustomHint
## Description
A plugin that allows you to create your own custom HUD for the server.  
For the plugin to work properly, you need to use **AdvancedHints**, which is available in releases for all versions.  
For suggestions, please ping in the EXILED Discord server or message: @narin4ik  
## Guide
### How to install the plugin?
Go to the [latest release.](https://github.com/BTF-SCPSL/CustomHint/releases) Download all the *dll* files from the release and upload them to your server (`.../EXILED/Plugins`).  
After installation, *start/restart* your server.  
Once you’ve completed these steps, a configuration will be generated in `.../EXILED/Configs` in the `[port]-config.yml` and `[port]-translation.yml` files.

### Configuring the plugin
Let’s start with the simpler part: `[port]-config.yml`. Use the *CTRL+F* combination and search for `custom_hint` to locate the plugin configuration.  
The default configuration in `[port]-config.yml` looks like this, with all the options explained:
```yaml
custom_hint:
  # Is the plugin enabled (bool)?
  is_enabled: true
  # Debug mode?
  debug: false
  # Enable or disable Hint for spectators.
  hint_for_spectators_is_enabled: true
  # Interval for changing spectator hints (in seconds).
  hint_message_time: 5
  # Default role name for players without roles.
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
```
After configuring `[port]-config.yml`, move to `[port]-translation.yml`, use *CTRL+F* again and search for `custom_hint`.  
You will see the following:
```yaml
custom_hint:
  # Hint for spectators.
  hint_message_for_spectators: |-
    {servername}
    {ip}:{port}

    {player_nickname}, spec, duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60

    {hints}
  # Hint for rounds lasting up to 59 seconds.
  hint_message_under_minute: |-
    {servername}
    {ip}:{port}

    Quick start! {player_nickname}, round time: {round_duration_seconds}s.
    Role: {player_role}
    TPS: {tps}/60
  # Hint for rounds lasting from 1 minute to 59 minutes and 59 seconds.
  hint_message_under_hour: |-
    {servername}
    {ip}:{port}

    Still going, {player_nickname}! Time: {round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60
  # Hint for rounds lasting 1 hour or longer.
  hint_message_over_hour: |-
    {servername}
    {ip}:{port}

    Long run, {player_nickname}! Duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60
  # Message displayed when HUD is successfully hidden.
  hide_hud_success_message: '<color=green>You have successfully hidden the server HUD! To get the HUD back, use .showhud</color>'
  # Message displayed when HUD is already hidden.
  hide_hud_already_hidden_message: '<color=red>You''ve already hidden the HUD server.</color>'
  # Message displayed when HUD is successfully restored.
  show_hud_success_message: '<color=green>You have successfully returned the server HUD! To hide again, use .hidehud</color>'
  # Message displayed when HUD is already visible.
  show_hud_already_shown_message: '<color=red>You already have the server HUD displayed.</color>'
  # Message displayed when DNT (Do Not Track) mode is enabled.
  dnt_enabled_message: '<color=red>Disable DNT (Do Not Track) mode.</color>'
  # Message displayed when commands are disabled on the server.
  command_disabled_message: '<color=red>This command is disabled on the server.</color>'
  # Message displayed when the hint system is enabled.
  toggle_on_message: '<color=green>Hint system and commands are now enabled for this round.</color>'
  # Message displayed when the hint system is disabled.
  toggle_off_message: '<color=red>Hint system and commands are now disabled for this round.</color>'
  # Message displayed when commands are disabled for the round.
  commands_disabled_message: '<color=red>Commands .showhud and .hidehud are disabled for this round.</color>'
```
And after localization... Voilà! Everything is ready! You can restart *(completely)* the server, and CustomHint will work perfectly.  
Thanks to everyone using this plugin. Good luck!  

## Placeholders
| Placeholder       | Description                               |
| ----------------- | ----------------------------------------- |
| {servername}      | Server name.                             |
| {ip}              | Server IP address.                       |
| {port}            | Server port.                             |
| {tps}             | Server TPS.                              |
| {player_nickname} | Player nickname.                         |
| {player_role}     | Player role.                             |
| {round_duration_seconds} | Round duration in seconds.         |
| {round_duration_minutes} | Round duration in minutes.         |
| {round_duration_hours}   | Round duration in hours.           |
| {hints}           | Hints from the Hints.txt file.           |
