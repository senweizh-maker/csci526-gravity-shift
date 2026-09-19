using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class GravityBoxController : MonoBehaviour
{
    [Header("Gravity")]
    public float gravityAcceleration = 20f;
    public float maxSpeed = 10f;

    [Header("Crush")]
    public LayerMask solidLayer;
    public float crushCheckMargin = 0.12f;

    private Rigidbody2D rb;

    private GravityCell currentGravityCell;

    private Vector2 gravityDirection = Vector2.down;

    public Vector2 GravityDirection => gravityDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        UpdateGravity();
        ApplyGravity();
        LimitSpeed();
    }

    private void UpdateGravity()
    {
        if (GravityGridManager.Instance == null)
        {
            gravityDirection = Vector2.down;
            return;
        }

        currentGravityCell =
            GravityGridManager.Instance.GetCellAtPoint(
                rb.worldCenterOfMass,
                currentGravityCell
            );

        if (currentGravityCell != null)
        {
            gravityDirection =
                currentGravityCell.GetGravityVector();
        }
        else
        {
            gravityDirection = Vector2.down;
        }
    }

    private void ApplyGravity()
    {
        rb.AddForce(
            gravityDirection
            * gravityAcceleration
            * rb.mass,
            ForceMode2D.Force
        );
    }

    private void LimitSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized
                * maxSpeed;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        Collider2D playerCollider = collision.collider;

        Vector2 boxCenter = rb.worldCenterOfMass;
        Vector2 playerCenter = playerCollider.bounds.center;

        Vector2 boxToPlayer =
            (playerCenter - boxCenter).normalized;


        float alignment =
            Vector2.Dot(boxToPlayer, gravityDirection);

        if (alignment < 0.65f)
            return;

        Bounds bounds = playerCollider.bounds;

        float playerExtent =
            Mathf.Abs(gravityDirection.x) * bounds.extents.x
            +
            Mathf.Abs(gravityDirection.y) * bounds.extents.y;

        float distance =
            playerExtent + crushCheckMargin;

        RaycastHit2D hit =
            Physics2D.Raycast(
                playerCenter,
                gravityDirection,
                distance,
                solidLayer
            );

        if (hit.collider != null)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.PlayerDied();
            }
        }
    }
}