using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class GravityGridManager : MonoBehaviour
{
    public static GravityGridManager Instance;

    [Header("Selection")]
    public Camera mainCamera;
    public LayerMask gravityCellLayer;

    [Header("Capacity")]
    public int maxActiveCells = 1;

    [Header("Gravity Boundary")]
    [SerializeField]
    private float exitMargin = 0.12f;

    [Header("UI")]
    public TMP_Text capacityText;

    private GravityCell selectedCell;

    private List<GravityCell> activeCells =
        new List<GravityCell>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Vector2 mouseScreen =
            Mouse.current.position.ReadValue();

        Vector3 world =
            mainCamera.ScreenToWorldPoint(mouseScreen);

        Collider2D hit =
            Physics2D.OverlapPoint(
                new Vector2(world.x, world.y),
                gravityCellLayer
            );

        if (hit == null)
            return;

        GravityCell cell =
            hit.GetComponent<GravityCell>();

        if (cell != null)
        {
            SelectCell(cell);
        }
    }

    public void SelectCell(GravityCell cell)
    {
        if (selectedCell != null)
        {
            selectedCell.SetSelected(false);
        }

        selectedCell = cell;
        selectedCell.SetSelected(true);
    }

    private void SetDirection(GravityDirection direction)
    {
        if (selectedCell == null)
        {
            Debug.Log("Select a gravity cell first.");
            return;
        }

        if (!selectedCell.isActive)
        {
            if (activeCells.Count >= maxActiveCells)
            {
                Debug.Log("Maximum gravity cells reached.");
                return;
            }

            activeCells.Add(selectedCell);
        }

        selectedCell.SetDirection(direction);

        UpdateUI();
    }

    public void SetUp()
    {
        SetDirection(GravityDirection.Up);
    }

    public void SetDown()
    {
        SetDirection(GravityDirection.Down);
    }

    public void SetLeft()
    {
        SetDirection(GravityDirection.Left);
    }

    public void SetRight()
    {
        SetDirection(GravityDirection.Right);
    }

    public void ClearSelected()
    {
        if (selectedCell == null)
            return;

        if (selectedCell.isActive)
        {
            activeCells.Remove(selectedCell);
        }

        selectedCell.Clear();
        selectedCell = null;

        UpdateUI();
    }

    public GravityCell GetCellAtPoint(
        Vector2 point,
        GravityCell previousCell)
    {
        if (previousCell != null &&
            previousCell.isActive &&
            previousCell.ContainsPointWithMargin(
                point,
                exitMargin
            ))
        {
            return previousCell;
        }

        foreach (GravityCell cell in activeCells)
        {
            if (cell.ContainsPoint(point))
            {
                return cell;
            }
        }

        return null;
    }

    public void AddCapacity(int amount)
    {
        maxActiveCells += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (capacityText != null)
        {
            int remaining =
                maxActiveCells - activeCells.Count;

            capacityText.text =
                "Gravity Cells Left: " + remaining;
        }
    }
}