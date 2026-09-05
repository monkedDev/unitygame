using UnityEngine;
using Core;

/// <summary>
/// Контроллер хода игрока.
/// Обрабатывает ввод с клавиатуры (WASD или стрелки) только во время хода игрока.
/// 
/// Примечание для перехода с Godot:
/// В Godot вы могли использовать _input(event) или Input.is_action_pressed("ui_up").
/// В Unity мы используем Input.GetKey() в методе Update().
/// Update() в Unity аналогичен _process(delta) в Godot.
/// </summary>
public class PlayerTurnController : MonoBehaviour
{
    /// <summary>
    /// Флаг, предотвращающий множественные нажатия за один ход.
    /// После первого нажатия игрок должен дождаться следующего хода.
    /// </summary>
    private bool _hasMovedThisTurn = false;

    private void Update()
    {
        // ВАЖНО: Обрабатываем ввод ТОЛЬКО если сейчас ход игрока
        // Это ключевое отличие от real-time игр где ввод обрабатывается всегда
        if (TurnManager.Instance.CurrentState != TurnManager.GameState.PlayerTurn)
        {
            return;
        }

        // Если игрок уже сделал ход в этом раунде, игнорируем дальнейший ввод
        if (_hasMovedThisTurn)
        {
            return;
        }

        // Проверяем нажатия клавиш движения
        // WASD и стрелки - стандартные управления в Unity (аналог ui_up/ui_down/ui_left/ui_right в Godot)
        Vector3Int direction = Vector3Int.zero;
        string directionName = "";

        // Вверх (W или Стрелка вверх)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = new Vector3Int(0, 1, 0);
            directionName = "Вверх";
        }
        // Вниз (S или Стрелка вниз)
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = new Vector3Int(0, -1, 0);
            directionName = "Вниз";
        }
        // Влево (A или Стрелка влево)
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = new Vector3Int(-1, 0, 0);
            directionName = "Влево";
        }
        // Вправо (D или Стрелка вправо)
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = new Vector3Int(1, 0, 0);
            directionName = "Вправо";
        }

        // Если было нажато направление, обрабатываем движение
        if (direction != Vector3Int.zero)
        {
            HandleMovement(direction, directionName);
        }
    }

    /// <summary>
    /// Обрабатывает попытку движения игрока.
    /// </summary>
    /// <param name="direction">Направление движения как Vector3Int</param>
    /// <param name="directionName">Название направления для логирования</param>
    /// <remarks>
    /// В текущей реализации только логируем направление и завершаем ход.
    /// В будущем здесь будет:
    /// 1. Проверка проходимости ячейки через GridManager.IsCellWalkable()
    /// 2. Перемещение игрока
    /// 3. Обновление позиции
    /// </remarks>
    private void HandleMovement(Vector3Int direction, string directionName)
    {
        // Выводим отладочное сообщение о направлении движения
        Debug.Log($"Движение: {directionName}");

        // TODO: Реализовать фактическое перемещение игрока
        // Пример будущей логики:
        // 1. Получить текущую позицию игрока в ячейках
        // Vector3Int currentCell = GridManager.Instance.WorldToCell(transform.position);
        // 
        // 2. Вычислить новую позицию
        // Vector3Int targetCell = currentCell + direction;
        // 
        // 3. Проверить проходимость
        // if (GridManager.Instance.IsCellWalkable(targetCell))
        // {
        //     Vector3 worldPos = GridManager.Instance.CellToWorld(targetCell);
        //     transform.position = worldPos;
        // }

        // Помечаем что игрок сделал ход
        _hasMovedThisTurn = true;

        // Завершаем ход игрока, передавая управление противнику
        TurnManager.Instance.EndTurn();
    }

    /// <summary>
    /// Сбрасывает флаг движения при получении события смены хода.
    /// Подписываемся на событие TurnManager.OnTurnChanged.
    /// </summary>
    /// <remarks>
    /// В Godot вы бы подключились к сигналу через Connect("turn_changed", this, "_on_turn_changed").
    /// В Unity используем += для подписки на event.
    /// </remarks>
    private void OnEnable()
    {
        TurnManager.Instance.OnTurnChanged += OnTurnChanged;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnTurnChanged -= OnTurnChanged;
    }

    /// <summary>
    /// Обработчик события смены хода.
    /// Сбрасывает флаг _hasMovedThisTurn когда наступает новый ход игрока.
    /// </summary>
    private void OnTurnChanged()
    {
        // Разрешаем движение снова если наступил ход игрока
        if (TurnManager.Instance.CurrentState == TurnManager.GameState.PlayerTurn)
        {
            _hasMovedThisTurn = false;
        }
    }
}
