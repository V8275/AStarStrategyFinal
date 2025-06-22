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
        // ������������� ����� (����� �������� ���, ���� ����� ������� ������ ����������)
        gridInt = new int[gridSize.x, gridSize.y];
        GameObject[,] cellObjects = new GameObject[gridSize.x, gridSize.y]; // ��� �������� ������ �� ��������� �������

        // ������ �� ���� ������� �����
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // ��������� ������ ������� ���� �� �������
                foreach (var noiseLayer in generatorLayersScripts.noiseLayers)
                {
                    // ��������� �������� �������� ��� ������� �������
                    float noiseValue = Mathf.PerlinNoise(
                        x * noiseLayer.NoiseScale,
                        y * noiseLayer.NoiseScale);

                    // �������� ��������� �������� ��� ����� ����
                    for (int i = 0; i < noiseLayer.thresholds.Length; i++)
                    {
                        if (noiseValue < noiseLayer.thresholds[i])
                        {
                            if (cellObjects[x, y] != null)
                            {
                                Destroy(cellObjects[x, y]);
                            }

                            // ������� ������, ���� ������� ���������
                            var cell = Instantiate(
                                noiseLayer.prefabs[i],
                                new Vector3(x, 0, y),
                                Quaternion.identity);

                            cell.name = $"Cell_{x}_{y}";
                            cell.transform.SetParent(transform);
                            cellObjects[x, y] = cell; // ��������� ������

                            // ��������� ����� (���� �����)
                            for (int k = 0; k < tags.Length; k++)
                            {
                                if (cell.CompareTag(tags[k]))
                                {
                                    gridInt[x, y] = 1;
                                    break;
                                }
                            }

                            // ��������� �������� ������� ��� ����� ����
                            break;
                        }
                    }

                    // ����� �������� yield return null ����� ��� ������������� �������� �� ������
                }
            }
            yield return null; // ������������ ��������� �� ������
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