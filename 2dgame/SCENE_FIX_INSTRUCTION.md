# Решение проблем с ошибками десериализации в Unity

## Проблема
При открытии сцены `TilemapPlayer.unity` возникают критические ошибки:
- Broken text PPtr
- GUID 000000000000... fileID is invalid
- Transform child can't be loaded

Это означает, что файл сцены или мета-файлы (.meta) повредились.

## Решение 1: Быстрое исправление через Unity Editor (Reimport)

Если файлы на месте и вы хотите попробовать восстановить текущую сцену:

1. **Закройте Unity Editor**
2. В проводнике перейдите в папку проекта: `Assets/Scenes/`
3. **Удалите файл `.meta` для сцены**: `TilemapPlayer.unity.meta`
4. **Откройте Unity Editor** - он автоматически переимпортирует сцену и создаст новый .meta файл
5. Если ошибки остались, попробуйте **удалить всю папку `Library/`** в корне проекта и перезапустить Unity

## Решение 2: Автоматическое создание новой сцены (Рекомендуется)

Если сцена повреждена окончательно, используйте Editor-скрипт для создания новой правильной сцены:

### Шаг 1: Убедитесь, что все скрипты на месте
Проверьте наличие файлов:
- `Assets/Scripts/Core/GridManager.cs`
- `Assets/Scripts/Core/TurnManager.cs`
- `Assets/Scripts/Core/PlayerMovement.cs`
- `Assets/Scripts/Core/PlayerTurnController.cs`
- `Assets/Scripts/Editor/SceneCreator.cs` ← **Важно: Editor-скрипт**

### Шаг 2: Запустите автоматическое создание сцены

1. Откройте Unity Editor
2. Дождитесь завершения компиляции всех скриптов
3. В верхнем меню выберите: **Tools > Create TilemapPlayer Scene**
4. Скрипт автоматически создаст новую сцену со следующей иерархией:

```
_Mangers (пустой объект для всех менеджеров)
  ├── GridManager (компонент)
  └── TurnManager (компонент)

Grid (объект сетки, аналог Godot TileMap)
  └── Tilemap (компоненты Tilemap + TilemapRenderer)

Player (объект игрока)
  ├── SpriteRenderer
  ├── PlayerMovement (компонент)
  ├── PlayerTurnController (компонент)
  └── Rigidbody2D (компонент)

Main Camera (если не существовала)
```

### Шаг 3: Сохраните сцену
Сцена автоматически сохранится в `Assets/Scenes/TilemapPlayer.unity`

## Решение 3: Ручное восстановление (если нужно точное соответствие)

Если автоматическое создание не подходит:

1. **Создайте новую сцену**: File > New Scene > 2D (URP)
2. **Создайте объект _Managers**:
   - Правый клик в Hierarchy > Create Empty
   - Переименуйте в `_Managers`
   - Добавьте компоненты: Add Component > GridManager, TurnManager
3. **Создайте Grid**:
   - Правый клик в Hierarchy > 2D Object > Tilemap > Rectangular
   - Это автоматически создаст Grid с дочерним Tilemap
4. **Создайте Player**:
   - Правый клик в Hierarchy > Create Empty
   - Переименуйте в `Player`
   - Добавьте компоненты: SpriteRenderer, PlayerMovement, PlayerTurnController, Rigidbody2D
   - В PlayerMovement назначьте Tilemap из поля Grid
   - В PlayerTurnController назначьте PlayerMovement
5. **Сохраните сцену**: File > Save As > `Assets/Scenes/TilemapPlayer.unity`

## Настройка спрайтов Kenney 1-Bit Pack

После создания сцены настройте тайлы:

### Создание Sprite Atlas (опционально, но рекомендуется)
1. Правый клик в Project > Create > 2D > Sprite Atlas
2. Назовите `KenneyAtlas`
3. Перетащите спрайтлист из `Assets/Sprites/` в поле Objects For Atlasing
4. Нажмите Apply

### Создание Tile Palette для тайлов 16x16
1. Окно: Window > 2D > Tile Palette
2. Нажмите "Create New Palette"
3. Название: `KenneyPalette`
4. Cell Size: `16` x `16` (пикселей)
5. Padding: `1` (отступ между тайлами)
6. Перетащите спрайтлист в палитру
7. Unity автоматически разрежет его на отдельные тайлы

### Назначение спрайта игроку
1. Выберите объект `Player` в Hierarchy
2. В Inspector найдите компонент SpriteRenderer
3. Перетащите любой спрайт из `Assets/Sprites/` в поле Sprite
4. Или создайте отдельный спрайт для игрока

## Проверка работы

1. Откройте сцену `TilemapPlayer.unity`
2. Нажмите Play
3. Нажимайте WASD или стрелки
4. В Console должны появляться сообщения:
   - "Движение: [направление]"
   - "Движение в клетку [координаты]"
   - "[TurnManager] Ход игрока завершён..."

## Примечания для перехода с Godot

| Godot | Unity |
|-------|-------|
| TileMap | Grid + Tilemap + TilemapRenderer |
| cell_size | Grid.cellSize |
| world_to_map() | GridManager.WorldToCell() |
| map_to_world() | GridManager.CellToWorld() |
| signal turn_changed | event Action OnTurnChanged |
| emit_signal() | OnTurnChanged?.Invoke() |
| _input(event) | Update() с Input.GetKeyDown() |
| _process(delta) | Update() с Time.deltaTime |
| _ready() | Start() |
| Tween | Vector3.Lerp() с таймером |

## Если ничего не помогает

1. Закройте Unity
2. Удалите папки: `Library/`, `Temp/`, `Obj/`
3. Откройте Unity заново (долгая переимпортация)
4. Попробуйте снова запустить Tools > Create TilemapPlayer Scene
