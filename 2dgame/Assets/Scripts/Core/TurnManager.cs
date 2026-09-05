using System;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Менеджер ходов для пошаговой системы игры.
    /// Синглтон, управляющий состоянием игры и переключением ходов.
    /// 
    /// Примечание для перехода с Godot:
    /// В Godot вы могли использовать сигналы (signals) для уведомления о смене хода.
    /// В Unity мы используем события C# (events/Action), что является стандартным подходом.
    /// enum GameState аналогичен использованию состояний в Godot state machine.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        private static TurnManager _instance;

        /// <summary>
        /// Перечисление состояний игры.
        /// Определяет, чей сейчас ход или состояние окончания игры.
        /// </summary>
        public enum GameState
        {
            PlayerTurn,  // Ход игрока
            EnemyTurn,   // Ход противника
            GameOver     // Игра окончена
        }

        /// <summary>
        /// Экземпляр синглтона TurnManager.
        /// Создаёт GameObject с компонентом TurnManager если он ещё не существует.
        /// </summary>
        public static TurnManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("TurnManager");
                    _instance = go.AddComponent<TurnManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// Текущее состояние игры.
        /// Только для чтения извне, изменяется через методы EndTurn() и SetGameOver().
        /// </summary>
        public GameState CurrentState { get; private set; } = GameState.PlayerTurn;

        /// <summary>
        /// Событие, вызываемое при изменении состояния хода.
        /// Подписывайтесь на это событие чтобы реагировать на смену хода.
        /// </summary>
        /// <remarks>
        /// В Godot вы бы использовали: signal turn_changed() и emit_signal("turn_changed").
        /// В Unity это эквивалентно event Action on_turn_changed и on_turn_changed?.Invoke().
        /// </remarks>
        public event Action OnTurnChanged;

        private void Awake()
        {
            // Убедимся, что только один экземпляр существует
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Завершает текущий ход и переключает состояние игры.
        /// PlayerTurn <-> EnemyTurn (циклически).
        /// Вызывает событие OnTurnChanged для уведомления подписчиков.
        /// </summary>
        /// <remarks>
        /// В Godot это было бы: current_state = ENEMY_TURN; emit_signal("turn_changed").
        /// Здесь мы используем switch для более читаемого кода.
        /// </remarks>
        public void EndTurn()
        {
            switch (CurrentState)
            {
                case GameState.PlayerTurn:
                    CurrentState = GameState.EnemyTurn;
                    Debug.Log("[TurnManager] Ход игрока завершён. Теперь ход противника.");
                    break;

                case GameState.EnemyTurn:
                    CurrentState = GameState.PlayerTurn;
                    Debug.Log("[TurnManager] Ход противника завершён. Теперь ход игрока.");
                    break;

                case GameState.GameOver:
                    Debug.LogWarning("[TurnManager] Игра окончена. Смена хода невозможна.");
                    return;
            }

            // Вызываем событие если есть подписчики
            // Аналог emit_signal в Godot
            OnTurnChanged?.Invoke();
        }

        /// <summary>
        /// Устанавливает состояние GameOver.
        /// </summary>
        public void SetGameOver()
        {
            CurrentState = GameState.GameOver;
            Debug.Log("[TurnManager] Игра окончена.");
            OnTurnChanged?.Invoke();
        }

        /// <summary>
        /// Проверяет, является ли текущий ход ходом игрока.
        /// Удобный метод для быстрой проверки в других скриптах.
        /// </summary>
        /// <returns>true если сейчас ход игрока</returns>
        public bool IsPlayerTurn()
        {
            return CurrentState == GameState.PlayerTurn;
        }

        /// <summary>
        /// Проверяет, является ли текущий ход ходом противника.
        /// </summary>
        /// <returns>true если сейчас ход противника</returns>
        public bool IsEnemyTurn()
        {
            return CurrentState == GameState.EnemyTurn;
        }
    }
}
