using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class GravityBoxController : MonoBehaviour
{
    [Header("Gravity")]
    [Tooltip("Pull toward the ground when no active gravity cell touches the box.")]
    public float gravityAcceleration = 20f;
 
    [Tooltip("Pull added by EACH active cell touching the box. Cells stack.")]
    public float accelerationPerCell = 12f;
 
    [Tooltip("Most cells' worth of pull that can stack on the box.")]
    public float maxCellStack = 5f;
 
    public float maxSpeed = 14f;

    [Header("Crush")]
    public LayerMask solidLayer;
    public float crushCheckMargin = 0.12f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;



    private Vector2 gravityDirection = Vector2.down;

    private Vector2 gravityPull = Vector2.down * 20f;

    public Vector2 GravityDirection => gravityDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        rb.gravityScale = 0;
        gravityPull = Vector2.down * gravityAcceleration;
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
            UseDefaultGravity();
            return;
        }

        Bounds bounds = boxCollider.bounds;
        bounds.Expand(new Vector3(-0.1f, -0.1f, 0f));
 
        int count;
        Vector2 net = GravityGridManager.Instance.GetNetGravityInBounds(bounds, out count);

        if (count == 0)
        {
            UseDefaultGravity();
            return;
        }

        net = Vector2.ClampMagnitude(net, maxCellStack);
 
        gravityPull = net * accelerationPerCell + Vector2.down * gravityAcceleration;
 
        if (net.sqrMagnitude > 0.0001f)
            gravityDirection = net.normalized;
    }


    private void UseDefaultGravity()
    {
        gravityDirection = Vector2.down;
        gravityPull = Vector2.down * gravityAcceleration;
    }
 
    private void ApplyGravity()
    {
        rb.AddForce(gravityPull * rb.mass, ForceMode2D.Force);
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