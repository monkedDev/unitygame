using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

namespace Core.Editor
{
    /// <summary>
    /// Editor-скрипт для автоматического создания сцены TilemapPlayer.
    /// Создаёт правильную иерархию объектов программно, чтобы избежать ошибок десериализации.
    /// 
    /// Использование:
    /// 1. Откройте Unity Editor
    /// 2. Меню: Tools > Create TilemapPlayer Scene
    /// 3. Скрипт автоматически создаст новую сцену со всеми необходимыми объектами
    /// </summary>
    public class SceneCreator
    {
        [MenuItem("Tools/Create TilemapPlayer Scene")]
        public static void CreateTilemapPlayerScene()
        {
            // Создаём новую сцену
            var scene = UnityEngine.SceneManagement.SceneManager.CreateScene("TilemapPlayer");
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
            
            // 1. Создаём объект _Managers для всех менеджеров
            GameObject managersObj = new GameObject("_Managers");
            managersObj.transform.position = Vector3.zero;
            
            // Добавляем GridManager
            GridManager gridManager = managersObj.AddComponent<GridManager>();
            gridManager.cellSize = 1f;
            
            // Добавляем TurnManager
            TurnManager turnManager = managersObj.AddComponent<TurnManager>();
            
            Debug.Log("[SceneCreator] Создан объект _Managers с GridManager и TurnManager");
            
            // 2. Создаём объект Grid (аналог Godot TileMap)
            GameObject gridObj = new GameObject("Grid");
            gridObj.transform.position = Vector3.zero;
            Grid grid = gridObj.AddComponent<Grid>();
            grid.cellSize = new Vector3(1f, 1f, 0f);
            grid.cellGap = Vector3.zero;
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;
            
            Debug.Log("[SceneCreator] Создан объект Grid");
            
            // 3. Создаём Tilemap внутри Grid
            GameObject tilemapObj = new GameObject("Tilemap");
            tilemapObj.transform.SetParent(gridObj.transform);
            tilemapObj.transform.localPosition = Vector3.zero;
            
            Tilemap tilemap = tilemapObj.AddComponent<Tilemap>();
            TilemapRenderer tilemapRenderer = tilemapObj.AddComponent<TilemapRenderer>();
            
            // Настраиваем TilemapRenderer
            tilemapRenderer.chunkSize = new Vector3Int(32, 32, 32);
            tilemapRenderer.maskInteraction = SpriteMaskInteraction.None;
            
            Debug.Log("[SceneCreator] Создан Tilemap с TilemapRenderer");
            
            // 4. Создаём объект Player
            GameObject playerObj = new GameObject("Player");
            playerObj.transform.position = new Vector3(0f, 0f, 0f);
            playerObj.tag = "Player";
            
            // Добавляем SpriteRenderer
            SpriteRenderer spriteRenderer = playerObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;
            spriteRenderer.color = Color.white;
            
            // Добавляем PlayerMovement
            PlayerMovement playerMovement = playerObj.AddComponent<PlayerMovement>();
            playerMovement.SetTilemap(tilemap);
            
            // Добавляем PlayerTurnController
            PlayerTurnController playerController = playerObj.AddComponent<PlayerTurnController>();
            playerController.SetPlayerMovement(playerMovement);
            
            // Добавляем Rigidbody2D (для будущих коллизий)
            Rigidbody2D rb2d = playerObj.AddComponent<Rigidbody2D>();
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            rb2d.gravityScale = 0f;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            Debug.Log("[SceneCreator] Создан объект Player со всеми компонентами");
            
            // 5. Создаём Main Camera если её нет
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                cameraObj.transform.position = new Vector3(0f, 0f, -10f);
                cameraObj.tag = "MainCamera";
                
                Camera camera = cameraObj.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.Color;
                camera.backgroundColor = new Color(0.192f, 0.302f, 0.475f, 0f);
                camera.orthographic = true;
                camera.orthographicSize = 5f;
                camera.nearClipPlane = 0.3f;
                camera.farClipPlane = 1000f;
                camera.cullingMask = ~0;
                camera.depth = -1;
                
                Debug.Log("[SceneCreator] Создана Main Camera");
            }
            
            // Сохраняем сцену
            string scenePath = "Assets/Scenes/TilemapPlayer.unity";
            
            // Создаём директорию если не существует
            if (!System.IO.Directory.Exists("Assets/Scenes"))
            {
                System.IO.Directory.CreateDirectory("Assets/Scenes");
            }
            
            UnityEngine.SceneManagement.SceneManager.SaveScene(scene, scenePath);
            Debug.Log($"[SceneCreator] Сцена сохранена в {scenePath}");
            
            // Открываем сцену
            var loadedScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(loadedScene);
            
            Debug.Log("[SceneCreator] Сцена TilemapPlayer успешно создана!");
            Debug.Log("[SceneCreator] === ИНСТРУКЦИЯ ===");
            Debug.Log("[SceneCreator] 1. Откройте сцену Assets/Scenes/TilemapPlayer.unity");
            Debug.Log("[SceneCreator] 2. Все менеджеры находятся на объекте _Managers");
            Debug.Log("[SceneCreator] 3. Grid содержит Tilemap для рендеринга тайлов");
            Debug.Log("[SceneCreator] 4. Player имеет все необходимые компоненты для движения");
            Debug.Log("[SceneCreator] 5. Для настройки спрайтов Kenney создайте Sprite Atlas и Tile Palette");
        }
    }
}
