using System.Collections;
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
 
    [Header("Skip Solid Areas")]
    [Tooltip("If ticked, cells that overlap solid ground are not created, " +
             "so only open air gets gravity cells.")]
    public bool skipSolidCells = false;
 
    [Tooltip("Set to the Ground layer only.")]
    public LayerMask solidLayer;
 
    private IEnumerator Start()
    {
        if (!generateOnStart)
            yield break;
 
        // Wait one frame so every collider in the scene is registered.
        // (yield return null still runs while Time.timeScale is 0.)
        if (skipSolidCells)
            yield return null;
 
        GenerateGrid();
    }
 
    public void GenerateGrid()
    {
        ClearGrid();
 
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Built from this object's position, so you can place grids anywhere
                Vector3 position = transform.position + new Vector3(
                    (x + 0.5f) * cellSize,
                    (y + 0.5f) * cellSize,
                    0f
                );
 
                if (skipSolidCells && IsSolid(position))
                    continue;
 
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
 
    private bool IsSolid(Vector3 position)
    {
        // Check a box a bit smaller than the cell so cells that only
        // touch a wall edge still count as open air
        Vector2 checkSize = Vector2.one * (cellSize * 0.5f);
 
        return Physics2D.OverlapBox(
            position,
            checkSize,
            0f,
            solidLayer
        ) != null;
    }
 
    private void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}