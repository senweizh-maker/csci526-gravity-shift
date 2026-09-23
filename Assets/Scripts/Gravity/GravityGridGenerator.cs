using UnityEngine;

public class GravityGridGenerator : MonoBehaviour
{
    [Header("Grid")]
    public int width = 6;
    public int height = 6;
    public float cellSize = 1f;

    [Header("Prefab")]
    public GameObject gravityCellPrefab;

    [Header("Generation")]
    public bool generateOnStart = true;

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateGrid();
        }
    }

    public void GenerateGrid()
    {
        ClearGrid();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(
                    x + 0.5f,
                    y + 0.5f,
                    0f
                );

                GameObject cell = Instantiate(
                    gravityCellPrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                cell.name = $"GravityCell_{x}_{y}";
            }
        }
    }

    private void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}