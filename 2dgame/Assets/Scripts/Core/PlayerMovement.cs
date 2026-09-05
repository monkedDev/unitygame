using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Скрипт движения игрока по тайловой сетке.
/// Аналог Godot: перемещение по клеткам TileMap с использованием GridManager.
/// 
/// В Godot вы бы использовали TileMap.cell_to_vector() для конвертации позиций.
/// В Unity мы используем GridManager.WorldToCell() и CellToWorld().
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Настройки ссылки на компоненты")]
    [Tooltip("Ссылка на Tilemap для проверки проходимости (аналог Godot TileMap)")]
    [SerializeField] private Tilemap tilemap;
    
    [Header("Настройки движения")]
    [Tooltip("Скорость перемещения между клетками (в секундах на клетку)")]
    [SerializeField] private float moveSpeed = 0.2f;
    
    // Текущая позиция в клетках сетки
    private Vector3Int currentCellPosition;
    
    // Флаг, указывающий, движется ли игрок сейчас
    private bool isMoving = false;
    
    // Целевая позиция для движения
    private Vector3 targetWorldPosition;
    
    // Начальная позиция для интерполяции
    private Vector3 startWorldPosition;
    
    // Таймер движения
    private float moveTimer = 0f;

    /// <summary>
    /// Инициализация при старте.
    /// В Godot это аналог _ready()
    /// </summary>
    private void Start()
    {
        // Получаем текущую позицию в клетках при старте
        currentCellPosition = GridManager.Instance.WorldToCell(transform.position);
        
        // Если Tilemap не назначен, пытаемся найти его в сцене
        if (tilemap == null)
        {
            tilemap = FindFirstObjectByType<Tilemap>();
            if (tilemap == null)
            {
                Debug.LogWarning("Tilemap не найден! Проверьте настройку сцены.");
            }
        }
    }

    /// <summary>
    /// Публичный метод для установки Tilemap из кода (используется Editor-скриптом).
    /// </summary>
    public void SetTilemap(Tilemap newTilemap)
    {
        tilemap = newTilemap;
    }

    /// <summary>
    /// Публичный метод для начала движения в направлении.
    /// Вызывается из PlayerTurnController при нажатии клавиш.
    /// </summary>
    /// <param name="direction">Направление движения (Vector3Int)</param>
    public void TryMove(Vector3Int direction)
    {
        // Если уже движемся, игнорируем ввод
        if (isMoving) return;
        
        // Вычисляем целевую клетку
        Vector3Int targetCell = currentCellPosition + direction;
        
        // Проверяем, проходима ли клетка через GridManager
        // В Godot вы бы проверяли tile_map.get_cellv(target_cell) != -1
        if (!GridManager.Instance.IsCellWalkable(targetCell))
        {
            Debug.Log($"Клетка {targetCell} непроходима!");
            return;
        }
        
        // Дополнительно проверяем наличие тайла в Tilemap (если назначен)
        if (tilemap != null && tilemap.HasTile(targetCell))
        {
            // Можно добавить дополнительную проверку на тип тайла
            // Например, если тайл является стеной
            TileBase tile = tilemap.GetTile<TileBase>(targetCell);
            // Здесь можно проверить тег или имя тайла
        }
        
        // Начинаем движение
        StartMove(targetCell);
    }

    /// <summary>
    /// Запускает процесс перемещения в целевую клетку.
    /// Использует интерполяцию для плавного движения (аналог tween в Godot).
    /// </summary>
    private void StartMove(Vector3Int targetCell)
    {
        isMoving = true;
        currentCellPosition = targetCell;
        
        // Сохраняем начальную позицию
        startWorldPosition = transform.position;
        
        // Вычисляем целевую мировую позицию из клетки
        targetWorldPosition = GridManager.Instance.CellToWorld(currentCellPosition);
        
        // Сбрасываем таймер
        moveTimer = 0f;
        
        Debug.Log($"Движение в клетку {currentCellPosition}");
    }

    /// <summary>
    /// Обновление каждый кадр.
    /// В Godot это аналог _process(delta)
    /// </summary>
    private void Update()
    {
        if (isMoving)
        {
            // Увеличиваем таймер движения
            moveTimer += Time.deltaTime;
            
            // Вычисляем коэффициент интерполяции (от 0 до 1)
            float t = Mathf.Clamp01(moveTimer / moveSpeed);
            
            // Интерполируем позицию (аналог lerp в Godot)
            transform.position = Vector3.Lerp(startWorldPosition, targetWorldPosition, t);
            
            // Если достигли цели
            if (t >= 1f)
            {
                isMoving = false;
                // Точно устанавливаем позицию в центр клетки
                transform.position = targetWorldPosition;
                
                Debug.Log($"Достиг клетки {currentCellPosition}");
            }
        }
    }

    /// <summary>
    /// Возвращает текущую позицию игрока в клетках сетки.
    /// Полезно для других систем (бой, взаимодействие и т.д.)
    /// </summary>
    public Vector3Int GetCurrentCellPosition()
    {
        return currentCellPosition;
    }

    /// <summary>
    /// Телепортирует игрока в указанную клетку без анимации.
    /// Используйте с осторожностью!
    /// </summary>
    public void TeleportToCell(Vector3Int cell)
    {
        currentCellPosition = cell;
        transform.position = GridManager.Instance.CellToWorld(cell);
        isMoving = false;
    }
}
