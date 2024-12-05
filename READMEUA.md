# CustomHint
## Опис
Плагін, який дозволяє створити власний HUD для сервера.  
Для коректної роботи плагіна необхідно використовувати **AdvancedHints**, який доступний у релізах для всіх версій.  
Для пропозицій, будь ласка, пінгуйте у Discord на сервері EXILED або пишіть в особисті повідомлення: @narin4ik  

## Інструкція
### Як встановити плагін?
Перейдіть на [останній реліз.](https://github.com/BTF-SCPSL/CustomHint/releases) Після цього завантажте всі *dll* файли з релізу та скопіюйте їх на сервер (`.../EXILED/Plugins`).  
Після встановлення *запустіть/перезапустіть* сервер.  
Як тільки ви виконали всі дії, у папці `.../EXILED/Configs` у файлах `[port]-config.yml` та `[port]-translation.yml` згенерується конфігурація.

### Налаштування плагіна
Почнемо з найпростішого, а саме з файлу `[port]-config.yml`. Використовуючи комбінацію *CTRL+F* і вписуючи `custom_hint`, знайдіть конфігурацію плагіна.  
Стандартна конфігурація `[port]-config.yml` виглядає наступним чином, і тут розписані всі пункти:
```yaml
custom_hint:
  # Чи ввімкнено плагін (bool)?
  is_enabled: true
  # Режим налагодження?
  debug: false
  # Увімкнути або вимкнути Hint для спостерігачів.
  hint_for_spectators_is_enabled: true
  # Інтервал зміни підказок для спостерігачів (у секундах).
  hint_message_time: 5
  # Ім'я ролі за замовчуванням для гравців без ролі.
  default_role_name: 'Player'
  # Колір ролі за замовчуванням (для гравців без ролей).
  default_role_color: 'white'
  # Ігноровані ролі.
  excluded_roles:
  - Overwatch
  - Filmmaker
  - Scp079
  # Увімкнути або вимкнути команди, пов'язані з HUD.
  enable_hud_commands: true
```
Після налаштування `[port]-config.yml`, відкрийте `[port]-translation.yml`, знову використовуючи комбінацію *CTRL+F*, і введіть `custom_hint`.  
Там ви побачите наступне:
```yaml
custom_hint:
  # Hint для спостерігачів.
  hint_message_for_spectators: |-
    {servername}
    {ip}:{port}

    {player_nickname}, spec, duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60

    {hints}
  # Hint для раунду, що триває до 59 секунд.
  hint_message_under_minute: |-
    {servername}
    {ip}:{port}

    Quick start! {player_nickname}, round time: {round_duration_seconds}s.
    Role: {player_role}
    TPS: {tps}/60
  # Hint для раунду, що триває від 1 хвилини до 59 хвилин і 59 секунд.
  hint_message_under_hour: |-
    {servername}
    {ip}:{port}

    Still going, {player_nickname}! Time: {round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60
  # Hint для раунду, що триває 1 годину і більше.
  hint_message_over_hour: |-
    {servername}
    {ip}:{port}

    Long run, {player_nickname}! Duration: {round_duration_hours}:{round_duration_minutes}:{round_duration_seconds}.
    Role: {player_role}
    TPS: {tps}/60
  # Повідомлення, яке відображається при успішному прихованні HUD.
  hide_hud_success_message: '<color=green>Ви успішно приховали HUD сервера! Щоб повернути HUD, використовуйте .showhud</color>'
  # Повідомлення, яке відображається, коли HUD вже прихований.
  hide_hud_already_hidden_message: '<color=red>Ви вже приховали HUD сервера.</color>'
  # Повідомлення, яке відображається при успішному поверненні HUD.
  show_hud_success_message: '<color=green>Ви успішно повернули HUD сервера! Щоб приховати знову, використовуйте .hidehud</color>'
  # Повідомлення, яке відображається, коли HUD вже відображається.
  show_hud_already_shown_message: '<color=red>HUD сервера вже відображається.</color>'
  # Повідомлення, яке відображається при ввімкненні режиму DNT (Do Not Track).
  dnt_enabled_message: '<color=red>Вимкніть режим DNT (Do Not Track).</color>'
  # Повідомлення, яке відображається, коли команди відключені на сервері.
  command_disabled_message: '<color=red>Ця команда відключена на сервері.</color>'
  # Повідомлення, яке відображається, коли система підказок увімкнена.
  toggle_on_message: '<color=green>Система підказок і команди тепер увімкнені для цього раунду.</color>'
  # Повідомлення, яке відображається, коли система підказок вимкнена.
  toggle_off_message: '<color=red>Система підказок і команди тепер вимкнені для цього раунду.</color>'
  # Повідомлення, яке відображається, коли команди вимкнені для раунду.
  commands_disabled_message: '<color=red>Команди .showhud і .hidehud вимкнені для цього раунду.</color>'
```
Після локалізації... Voilà! Все готово! Ви можете повністю перезапустити сервер, і CustomHint працюватиме відмінно.  
Дякую тим, хто використовує цей плагін. Успіхів вам!  

## Плейсхолдери
| Плейсхолдер       | Опис                                    |
| ----------------- | --------------------------------------- |
| {servername}      | Назва сервера.                         |
| {ip}              | IP-адреса сервера.                     |
| {port}            | Порт сервера.                          |
| {tps}             | TPS сервера.                           |
| {player_nickname} | Нікнейм гравця.                        |
| {player_role}     | Роль гравця.                           |
| {round_duration_seconds} | Тривалість раунду в секундах.     |
| {round_duration_minutes} | Тривалість раунду в хвилинах.     |
| {round_duration_hours}   | Тривалість раунду в годинах.      |
| {hints}           | Hints з файлу Hints.txt.            |
