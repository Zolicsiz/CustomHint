# CustomHint
## Описание
Плагин позволяющий сделать свой собственный HUD для сервера.  
Для нормальной работы плагина потребуется использовать **AdvancedHints**, который есть в релизах на всех версиях.  
Для предложений, пожалуйста, пингуйте в Discord на сервере EXILED либо пишите в лс: @narin4ik  
## Обучение
### Как устанновить плагин?
Переходим на [последний релиз.](https://github.com/BTF-SCPSL/CustomHint/releases) После чего скачиваем все *dll* файлы из релиза и загружаем на сервер (`.../EXILED/Plugins`).  
После установки *запускаем/перезапускаем* сервер.  
Как только Вы сделали все манипуляции, в `.../EXILED/Configs` в файлах `[port]-config.yml` и `[port]-translation.yml` сгенерируется конфигурация.
### Настройка плагина
Начнём с самого лёгкого, а именно с `[port]-config.yml`. Используя комбинацию *CTRL+F* и вписывая `custom_hint`, находим конфигурацию плагина.  
Дефолтная конфигурация `[port]-config.yml` выглядит вот так, там рассписаны все пункты:
```
custom_hint:
  # Включен ли плагин (bool)?
  is_enabled: true
  # Режим отладки?
  debug: false
  # Включить или отключить Hint для наблюдателей.
  hint_for_spectators_is_enabled: true
  # Интервал смены подсказок для зрителей (в секундах).
  hint_message_time: 5
  # Имя роли по умолчанию для игроков без роли.
  default_role_name: 'Player'
  # Цвет роли по умолчанию (для игроков без ролей).
  default_role_color: 'white'
  # Игнорируемые роли.
  excluded_roles:
  - Overwatch
  - Filmmaker
  - Scp079
  # Включить или отключить команды, связанные с HUD.
  enable_hud_commands: true
```
После того как настроили `[port]-config.yml`, переходим в `[port]-translation.yml`, снова используем комбинацию *CTRL+F* и вписываем `custom_hint`.  
Там увидим следующее:
```
custom_hint:
  # Hint для наблюдателей.
  hint_message_for_spectators: |-
    {servername}
    {ip}:{port}

    {player_nickname}, spec, duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60

    {hints}
  # Hint для раунда, который длится до 59 секунд.
  hint_message_under_minute: |-
    {servername}
    {ip}:{port}

    Quick start! {player_nickname}, round time: {round_duration_seconds}s.
    Role: {player_role}
    TPS: {tps}/60
  # Hint для раунда, который длится от 1 минуты до 59 минут и 59 секунд.
  hint_message_under_hour: |-
    {servername}
    {ip}:{port}

    Still going, {player_nickname}! Time: {round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60
  # Hint для раунда, который длятся 1 час и больше.
  hint_message_over_hour: |-
    {servername}
    {ip}:{port}

    Long run, {player_nickname}! Duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60
  # Сообщение, отображаемое при успешном скрытии HUD.
  hide_hud_success_message: '<color=green>You have successfully hidden the server HUD! To get the HUD back, use .showhud</color>'
  # Сообщение, отображаемое, когда HUD уже скрыт.
  hide_hud_already_hidden_message: '<color=red>You''ve already hidden the HUD server.</color>'
  # Сообщение, отображаемое при успешном возвращении HUD.
  show_hud_success_message: '<color=green>You have successfully returned the server HUD! To hide again, use .hidehud</color>'
  # Сообщение, отображаемое, когда HUD уже отображается.
  show_hud_already_shown_message: '<color=red>You already have the server HUD displayed.</color>'
  # Сообщение, отображаемое при включении режима DNT (Do Not Track).
  dnt_enabled_message: '<color=red>Disable DNT (Do Not Track) mode.</color>'
  # Сообщение, отображаемое, когда команды отключены на сервере.
  command_disabled_message: '<color=red>This command is disabled on the server.</color>'
  # Сообщение, отображаемое, когда система подсказок включена.
  toggle_on_message: '<color=green>Hint system and commands are now enabled for this round.</color>'
  # Сообщение, отображаемое, когда система подсказок отключена.
  toggle_off_message: '<color=red>Hint system and commands are now disabled for this round.</color>'
  # Сообщение, отображаемое, когда команды отключены для раунда.
  commands_disabled_message: '<color=red>Commands .showhud and .hidehud are disabled for this round.</color>'
```
И после локализации... Воуля! Всё готово! Можете рестартнуть *(полнотью)* сервер и CustomHint работает на отлично.  
Спасибо тем, кто использует данный плагин. Удачи Вам!  
## Placeholders
| Placeholder       | Описание                                 |
| ----------------- | ---------------------------------------- |
| {servername}      | Название сервера.                            |
| {ip}              | IP-адрес сервера.                       |
| {port}            | Порт сервера.                           |
| {tps}             | TPS сервера.                            |
| {player_nickname} | Никнейм игрока.                         |
| {player_role}     | Роль игрока.                            |
| {round_duration_seconds} | Длительность раунда в секундах.    |
| {round_duration_minutes} | Длительность раунда в минутах.    |
| {round_duration_hours}   | Длительность раунда в часах.      |
| {hints}           | Подсказки из файла Hints.txt.  |
