using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
public class GravityCell : MonoBehaviour
{
    public GravityDirection direction = GravityDirection.Down;

    public bool isActive = false;
    public bool isSelected = false;

    public SpriteRenderer background;
    public TMP_Text arrowText;

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        if (background == null)
            background = GetComponent<SpriteRenderer>();

        RefreshVisual();
    }

    public void SetDirection(GravityDirection newDirection)
    {
        direction = newDirection;
        isActive = true;

        RefreshVisual();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        RefreshVisual();
    }

    public void Clear()
    {
        isActive = false;
        isSelected = false;

        RefreshVisual();
    }

    public bool ContainsPoint(Vector2 point)
    {
        return boxCollider.OverlapPoint(point);
    }


    public bool ContainsPointWithMargin(Vector2 point, float margin)
    {
        Bounds bounds = boxCollider.bounds;


        bounds.Expand(new Vector3(
            margin * 2f,
            margin * 2f,
            0f
        ));

        return bounds.Contains(
            new Vector3(point.x, point.y, bounds.center.z)
        );
    }

    public Vector2 GetGravityVector()
    {
        return GravityDirectionUtility.ToVector(direction);
    }

    private void RefreshVisual()
    {
        if (background != null)
        {
            Color c = background.color;

            if (isSelected)
                c.a = 0.45f;
            else if (isActive)
                c.a = 0.25f;
            else
                c.a = 0.08f;

            background.color = c;
        }

        if (arrowText != null)
        {
            arrowText.gameObject.SetActive(isActive);

            if (isActive)
            {
                arrowText.text =
                    GravityDirectionUtility.ToArrow(direction);
            }
        }
    }
}