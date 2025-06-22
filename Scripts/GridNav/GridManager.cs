using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private GeneratorLayersScriptable generatorLayersScripts;
    [SerializeField] 
    private Vector2Int gridSize = new Vector2Int(10, 10);
    [SerializeField] 
    private string[] tags;

    private int[,] gridInt;

    void Awake()
    {
        gridInt = new int[gridSize.x, gridSize.y];
        StartCoroutine(GenerateLevel());
    }

    IEnumerator GenerateLevel()
    {
        // Инициализация сетки (можно изменить тип, если нужно хранить больше информации)
        gridInt = new int[gridSize.x, gridSize.y];
        GameObject[,] cellObjects = new GameObject[gridSize.x, gridSize.y]; // Для хранения ссылок на созданные объекты

        // Проход по всем клеткам сетки
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // Применяем каждый шумовый слой по порядку
                foreach (var noiseLayer in generatorLayersScripts.noiseLayers)
                {
                    // Генерация шумового значения для текущей позиции
                    float noiseValue = Mathf.PerlinNoise(
                        x * noiseLayer.NoiseScale,
                        y * noiseLayer.NoiseScale);

                    // Проверка пороговых значений для этого слоя
                    for (int i = 0; i < noiseLayer.thresholds.Length; i++)
                    {
                        if (noiseValue < noiseLayer.thresholds[i])
                        {
                            if (cellObjects[x, y] != null)
                            {
                                Destroy(cellObjects[x, y]);
                            }

                            // Создаем префаб, если условие выполнено
                            var cell = Instantiate(
                                noiseLayer.prefabs[i],
                                new Vector3(x, 0, y),
                                Quaternion.identity);

                            cell.name = $"Cell_{x}_{y}";
                            cell.transform.SetParent(transform);
                            cellObjects[x, y] = cell; // Сохраняем ссылку

                            // Обновляем сетку (если нужно)
                            for (int k = 0; k < tags.Length; k++)
                            {
                                if (cell.CompareTag(tags[k]))
                                {
                                    gridInt[x, y] = 1;
                                    break;
                                }
                            }

                            // Прерываем проверку порогов для этого слоя
                            break;
                        }
                    }

                    // Можно добавить yield return null здесь для распределения нагрузки по кадрам
                }
            }
            yield return null; // Распределяем генерацию по кадрам
        }
    }

    public Vector2Int Size()
    {
        return new Vector2Int(gridSize.x, gridSize.y);
    }

    public int[,] GetGrid()
    {
        return gridInt;
    }
}