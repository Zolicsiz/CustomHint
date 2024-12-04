# CustomHint
Рекомендуется использовать `AdvancedHints.dll` для удобства.  
Для предложений, пожалуйста, пишите в Discord: @narin4ik  

**Стандартная конфигурация (cfg):**   
```
custom_hint:
# Включен ли плагин (bool)?
  is_enabled: true
  # Режим отладки?
  debug: false
  # Включить или отключить подсказки для наблюдателей.
  hint_for_spectators_is_enabled: true
  # Интервал смены подсказок для наблюдателей (в секундах).
  hint_message_time: 5
  # Сообщение подсказки для наблюдателей.
  hint_message_for_spectators: |-
    {servername}
    {ip}:{port}

    {player_nickname}, наблюдатель, время: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Роль: {player_role}
    TPS: {tps}/60

    {hints}
  # Сообщение подсказки для раундов, продолжающихся до 59 секунд.
  hint_message_under_minute: |-
    {servername}
    {ip}:{port}

    Быстрый старт! {player_nickname}, время раунда: {round_duration_seconds}с.
    Роль: {player_role}
    TPS: {tps}/60
  # Сообщение подсказки для раундов от 1 минуты до 59 минут и 59 секунд.
  hint_message_under_hour: |-
    {servername}
    {ip}:{port}

    Идем дальше, {player_nickname}! Время: {round_duration_minutes}:{round_duration_seconds}.
    Роль: {player_role}
    TPS: {tps}/60
  # Сообщение подсказки для раундов, продолжающихся 1 час или больше.
  hint_message_over_hour: |-
    {servername}
    {ip}:{port}

    Долгий бой, {player_nickname}! Время: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Роль: {player_role}
    TPS: {tps}/60
  # Название роли по умолчанию для игроков без роли.
  default_role_name: 'Игрок'
  # Цвет роли по умолчанию (для игроков без ролей).
  default_role_color: 'белый'
  # Игнорируемые роли.
  excluded_roles:
  - Overwatch
  - Filmmaker
  - Scp079
  # Включить или отключить команды, связанные с HUD.
  enable_hud_commands: true
  # Сообщение, отображаемое при успешном скрытии HUD.
  hide_hud_success_message: '<color=green>Вы успешно скрыли HUD сервера! Чтобы вернуть HUD, используйте .showhud</color>'
  # Сообщение, отображаемое, если HUD уже скрыт.
  hide_hud_already_hidden_message: '<color=red>Вы уже скрыли HUD сервера.</color>'
  # Сообщение, отображаемое при успешном возврате HUD.
  show_hud_success_message: '<color=green>Вы успешно вернули HUD сервера! Чтобы снова скрыть, используйте .hidehud</color>'
  # Сообщение, отображаемое, если HUD уже отображается.
  show_hud_already_shown_message: '<color=red>HUD сервера уже отображается.</color>'
  # Сообщение, отображаемое при включении режима DNT (Do Not Track).
  dnt_enabled_message: '<color=red>Отключите режим DNT (Do Not Track).</color>'
  # Сообщение, отображаемое, если команды отключены на сервере.
  command_disabled_message: '<color=red>Эта команда отключена на сервере.</color>'
  # Сообщение, отображаемое при включении системы подсказок.
  toggle_on_message: '<color=green>Система подсказок и команды теперь включены для этого раунда.</color>'
  # Сообщение, отображаемое при отключении системы подсказок.
  toggle_off_message: '<color=red>Система подсказок и команды теперь отключены для этого раунда.</color>'
  # Сообщение, отображаемое, если команды отключены для раунда.
  commands_disabled_message: '<color=red>Команды .showhud и .hidehud отключены для этого раунда.</color>'
```

**Заполнители (placeholders):**  
| Заполнитель       | Описание                                 |
| ----------------- | ---------------------------------------- |
| {servername}      | Имя сервера.                            |
| {ip}              | IP адрес сервера.                       |
| {port}            | Порт сервера.                           |
| {tps}             | TPS сервера.                            |
| {player_nickname} | Никнейм игрока.                         |
| {player_role}     | Роль игрока.                            |
| {round_duration_seconds} | Длительность раунда в секундах.    |
| {round_duration_minutes} | Длительность раунда в минутах.    |
| {round_duration_hours}   | Длительность раунда в часах.      |
| {hints}           | Подсказки из файла SpectatorHints.txt.  |
