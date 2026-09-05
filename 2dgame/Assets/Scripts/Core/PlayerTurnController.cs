using UnityEngine;
using Core;

/// <summary>
/// Контроллер хода игрока. Обрабатывает ввод и инициирует движение.
/// В Godot это был бы скрипт на игроке с _input(event) методом.
/// </summary>
public class PlayerTurnController : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Ссылка на компонент PlayerMovement для выполнения движения")]
    [SerializeField] private PlayerMovement playerMovement;
    
    /// <summary>
    /// Обновление каждый кадр.
    /// В Godot это аналог _input(event) или _unhandled_input(event).
    /// </summary>
    private void Update()
    {
        // ВАЖНО: Обрабатываем ввод ТОЛЬКО если сейчас ход игрока
        if (TurnManager.Instance.CurrentState != GameState.PlayerTurn)
        {
            return;
        }
        
        // Если игрок уже движется, игнорируем ввод (как в пошаговых играх)
        // Это предотвращает "запоминание" нескольких нажатий
        
        // Получаем ввод (WASD или стрелки)
        // В Godot: Input.get_action_strength("ui_up") и т.д.
        Vector3Int direction = GetInputDirection();
        
        if (direction != Vector3Int.zero)
        {
            string directionName = GetDirectionName(direction);
            Debug.Log($"Движение: {directionName}");
            
            // Если есть компонент PlayerMovement, используем его для движения
            if (playerMovement != null)
            {
                playerMovement.TryMove(direction);
            }
            
            // Завершаем ход игрока после ввода
            // Это переключит состояние на EnemyTurn
            TurnManager.Instance.EndTurn();
        }
    }
    
    /// <summary>
    /// Преобразует нажатия клавиш в направление сетки.
    /// Возвращает Vector3Int.zero, если ничего не нажато.
    /// </summary>
    private Vector3Int GetInputDirection()
    {
        // В Godot: Input.is_action_pressed("ui_right") и т.д.
        // В Unity используем Input.GetKey() или Input.GetAxis()
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            return Vector3Int.up;      // Вверх (0, 1, 0)
        
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            return Vector3Int.down;    // Вниз (0, -1, 0)
        
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            return Vector3Int.left;    // Влево (-1, 0, 0)
        
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            return Vector3Int.right;   // Вправо (1, 0, 0)
        
        return Vector3Int.zero;
    }
    
    /// <summary>
    /// Возвращает читаемое название направления для отладки.
    /// </summary>
    private string GetDirectionName(Vector3Int direction)
    {
        if (direction == Vector3Int.up) return "Вверх";
        if (direction == Vector3Int.down) return "Вниз";
        if (direction == Vector3Int.left) return "Влево";
        if (direction == Vector3Int.right) return "Вправо";
        return "Нет направления";
    }
    
    /// <summary>
    /// Метод для назначения PlayerMovement из инспектора или кода.
    /// Вызывается автоматически, если не назначен вручную.
    /// </summary>
    private void Awake()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogWarning("PlayerMovement не найден на этом объекте!");
            }
        }
    }
}
