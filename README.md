# Custom Hint
Plugin that allows you to display text on the player's screen.  
For suggestions, please write to Discord: @narin4ik  

**Default cfg:**   
```
custom_hint:
# Plugin enabled (bool)?
  is_enabled: true
  # Debug mode (bool)?
  debug: false
  # Hint message for rounds lasting up to 59 seconds.
  hint_message_under_minute: |-
    Quick start! {player_nickname}, round time: {round_duration_seconds}s.
    Role: {player_role}
  # Hint message for rounds lasting from 1 minute to 59 minutes and 59 seconds.
  hint_message_under_hour: |-
    Still going, {player_nickname}! Time: {round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
  # Hint message for rounds lasting 1 hour or more.
  hint_message_over_hour: |-
    Long run, {player_nickname}! Duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
  # Default role name for players without a role.
  default_role_name: 'Player'
  # Default role color (for players without roles).
  default_role_color: 'white'
  # Ignored roles
  excluded_roles:
  - Overwatch
  - Spectator
  - Filmmaker
```
**Placeholders:**  
| Placeholder  | Description |
| ------------- | ------------- |
| {player_nickname}  | Player's nickname.  |
| {player_role}  | Player Role.  |
| {round_duration_seconds}  | Round duration in seconds.  |
| {round_duration_minutes}  | Round duration in minutes.  |
| {round_duration_hours}  | Round duration in hours.  |
