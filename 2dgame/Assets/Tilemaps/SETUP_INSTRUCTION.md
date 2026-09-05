# Инструкция по настройке Tilemap для спрайтов Kenney 1-Bit Pack

## Шаг 1: Импорт спрайта

1. Поместите файл спрайта (например, `kenney_1bit-pack.png`) в папку `Assets/Sprites/`
2. Выберите файл в инспекторе Unity
3. Настройте импорт:
   - **Texture Type**: Sprite (2D and UI)
   - **Sprite Mode**: Multiple (так как это спрайтлист)
   - **Pixels Per Unit**: 16 (для пиксель-арта 16x16)
   - **Filter Mode**: Point (no filter) - для чёткого пиксель-арта
   - **Compression**: None - чтобы не было артефактов

4. Нажмите **Apply**

## Шаг 2: Нарезка спрайта на тайлы

1. Откройте редактор Sprite Editor (кнопка в инспекторе после импорта)
2. В выпадающем меню **Slice** выберите:
   - **Type**: Grid By Cell Size
   - **Pixel Size**: X=17, Y=17 (16 пикселей + 1 пиксель отступ/padding)
   - **Offset**: X=0, Y=0
   - **Padding**: 1 (если есть промежуток между тайлами)

3. Нажмите **Slice**, затем **Apply**

## Шаг 3: Создание Tile Palette

1. Откройте окно **Tile Palette** (Window → 2D → Tile Palette)
2. Нажмите **Create New Palette**
3. Назовите его например "Kenney1BitPalette"
4. Выберите созданные спрайты из папки Sprites
5. Перетащите их на палитру

## Шаг 4: Настройка Tilemap в сцене

1. Откройте сцену `TilemapPlayer`
2. Выделите объект **Grid** → **Tilemap**
3. Используйте **Tile Palette** для рисования тайлов на сцене
4. Для проверки проходимости можно создать отдельный слой collision

## Связь с кодом

### GridManager.cs
```csharp
// Конвертация мировой позиции в клетку сетки
Vector3Int cell = GridManager.Instance.WorldToCell(worldPosition);

// Конвертация клетки в мировую позицию
Vector3 worldPos = GridManager.Instance.CellToWorld(cell);

// Проверка проходимости (нужно реализовать свою логику)
bool walkable = GridManager.Instance.IsCellWalkable(cell);
```

### PlayerMovement.cs
```csharp
// Движение в направлении
playerMovement.TryMove(Vector3Int.up);    // Вверх
playerMovement.TryMove(Vector3Int.down);  // Вниз
playerMovement.TryMove(Vector3Int.left);  // Влево
playerMovement.TryMove(Vector3Int.right); // Вправо
```

## Примечания о переходе с Godot

| Godot | Unity |
|-------|-------|
| TileMap | Grid + Tilemap + TilemapRenderer |
| cell_to_vector() | GridManager.WorldToCell() |
| vector_to_cell() | GridManager.CellToWorld() |
| get_cellv() | tilemap.GetTile() |
| _input(event) | Input.GetKey() в Update() |
| Signal | C# event Action |

## Размеры и настройки

- **Размер тайла**: 16x16 пикселей
- **Отступ (Padding)**: 1 пиксель
- **Cells Per Unit**: 1 (в GridManager.cellSize)
- **Sprite Pixels Per Unit**: 16
