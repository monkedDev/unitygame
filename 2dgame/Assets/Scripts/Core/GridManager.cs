using UnityEngine;

namespace Core
{
    /// <summary>
    /// Менеджер сетки для пошагового 2D рогалика.
    /// Синглтон, управляющий конвертацией между мировыми координатами и координатами ячеек.
    /// 
    /// Примечание для перехода с Godot:
    /// В Godot вы могли использовать TileMap для работы с сеткой. В Unity мы реализуем
    /// собственную систему координат ячеек, что даёт больше контроля над логикой игры.
    /// WorldToCell/CellToWorld аналогичны map_to_world/world_to_map в Godot TileMap.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        private static GridManager _instance;
        
        /// <summary>
        /// Экземпляр синглтона GridManager.
        /// Создаёт GameObject с компонентом GridManager если он ещё не существует.
        /// </summary>
        public static GridManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GridManager");
                    _instance = go.AddComponent<GridManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// Размер одной ячейки сетки в мировых единицах.
        /// По умолчанию 1f - стандартный размер для большинства 2D игр.
        /// В Godot это аналог cell_size в TileMap.
        /// </summary>
        [Tooltip("Размер одной ячейки в мировых единицах (аналог cell_size в Godot)")]
        public float cellSize = 1f;

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
        /// Конвертирует мировые координаты в координаты ячейки сетки.
        /// </summary>
        /// <param name="worldPosition">Позиция в мировом пространстве</param>
        /// <returns>Координаты ячейки как Vector3Int</returns>
        /// <remarks>
        /// В Godot это аналог world_to_map(). 
        /// Используем Mathf.FloorToInt для округления вниз, чтобы получить индекс ячейки.
        /// </remarks>
        public Vector3Int WorldToCell(Vector3 worldPosition)
        {
            // Делим мировую позицию на размер ячейки и округляем до целого
            int x = Mathf.FloorToInt(worldPosition.x / cellSize);
            int y = Mathf.FloorToInt(worldPosition.y / cellSize);
            int z = Mathf.RoundToInt(worldPosition.z / cellSize);
            return new Vector3Int(x, y, z);
        }

        /// <summary>
        /// Конвертирует координаты ячейки в мировые координаты (центр ячейки).
        /// </summary>
        /// <param name="cellPosition">Координаты ячейки</param>
        /// <returns>Мировая позиция центра ячейки</returns>
        /// <remarks>
        /// В Godot это аналог map_to_world().
        /// Добавляем 0.5 * cellSize чтобы получить центр ячейки, а не её верхний левый угол.
        /// </remarks>
        public Vector3 CellToWorld(Vector3Int cellPosition)
        {
            // Умножаем координаты ячейки на размер и добавляем половину размера для центра
            float x = (cellPosition.x + 0.5f) * cellSize;
            float y = (cellPosition.y + 0.5f) * cellSize;
            float z = cellPosition.z * cellSize;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Проверяет, проходимая ли данная ячейка.
        /// Заглушка для будущей реализации.
        /// </summary>
        /// <param name="cell">Координаты ячейки для проверки</param>
        /// <returns>true если ячейка проходима, иначе false</returns>
        /// <remarks>
        /// В будущем здесь будет проверка на стены, препятствия и другие объекты.
        /// В Godot вы могли бы проверять tile_data или использовать collision layers.
        /// </remarks>
        public bool IsCellWalkable(Vector3Int cell)
        {
            // TODO: Реализовать проверку на проходимость
            // - Проверка наличия стен
            // - Проверка других препятствий
            // - Проверка границ карты
            return true;
        }
    }
}
