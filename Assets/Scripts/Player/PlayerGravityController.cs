using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerGravityController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 7f;
    public float maxSpeed = 12f;
    public float defaultGravityAcceleration = 20f;
    public float cellGravityAcceleration = 10f;
    public float airDeceleration = 15f;

    public LayerMask groundLayer;

    public Transform visual;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    private Vector2 gravityDirection =
        Vector2.down;

    private GravityCell currentGravityCell;

    private float moveInput;
    private bool jumpPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        rb.gravityScale = 0;
    }

    private void Update()
    {
        moveInput = 0;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            moveInput = -1;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            moveInput = 1;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpPressed = true;
        }
    }

    private void LimitSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void FixedUpdate()
    {
        UpdateGravity();

        ApplyGravity();

        ApplyMovement();

        ApplyJump();

        RotateVisual();

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
        float acceleration;

        if (currentGravityCell != null)
        {
            acceleration = cellGravityAcceleration;
        }
        else
        {
            acceleration = defaultGravityAcceleration;
        }

        rb.AddForce(
            gravityDirection
            * acceleration
            * rb.mass,
            ForceMode2D.Force
        );
    }

    private Vector2 GetTangent()
    {
        if (Mathf.Abs(gravityDirection.y) > 0.5f)
        {
            return Vector2.right;
        }
        else
        {
            return Vector2.up;
        }
    }

    private void ApplyMovement()
    {
        Vector2 velocity = rb.linearVelocity;
        float gravitySpeed = Vector2.Dot(velocity, gravityDirection);
        Vector2 gravityVelocity = gravityDirection * gravitySpeed;
        Vector2 perpVelocity = velocity - gravityVelocity;

        bool horizontalGravity = Mathf.Abs(gravityDirection.x) > 0.5f;

        if (horizontalGravity)
        {

            if (gravitySpeed < 0f)
            {
                gravitySpeed = 0f;
            }

            if (moveInput != 0f)
            {
                float inputDirection = Mathf.Sign(moveInput);
                float gravityDirSign = Mathf.Sign(gravityDirection.x);

                if (Mathf.Approximately(inputDirection, gravityDirSign))
                {
                    float boostedSpeed = Mathf.Abs(moveInput) * moveSpeed;
                    gravitySpeed += Mathf.Max(gravitySpeed, boostedSpeed);
                }
                
            }

            rb.linearVelocity = gravityDirection * gravitySpeed + perpVelocity;
        }
        else
        {
            
            float currentHorizontalSpeed = velocity.x;
            float newHorizontalSpeed;

            if (moveInput != 0f)
            {
                newHorizontalSpeed = moveInput * moveSpeed;
            }
            else
            {
                newHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, 0f, airDeceleration * Time.fixedDeltaTime);
            }

            rb.linearVelocity = new Vector2(newHorizontalSpeed, 0f) + gravityVelocity;
        }
    }

    private void ApplyJump()
    {
        if (!jumpPressed)
            return;

        jumpPressed = false;

        if (!IsGrounded())
            return;

        Vector2 tangent = GetTangent();

        float tangentSpeed =
            Vector2.Dot(
                rb.linearVelocity,
                tangent
            );

        rb.linearVelocity =
            tangent * tangentSpeed
            - gravityDirection * jumpSpeed;
    }

    private bool IsGrounded()
    {
        Bounds bounds = playerCollider.bounds;

        float checkDistance;

        if (Mathf.Abs(gravityDirection.x) > 0.5f)
            checkDistance = bounds.extents.x + 0.08f;
        else
            checkDistance = bounds.extents.y + 0.08f;

        RaycastHit2D hit =
            Physics2D.Raycast(
                bounds.center,
                gravityDirection,
                checkDistance,
                groundLayer
            );

        return hit.collider != null;
    }

    private void RotateVisual()
    {
        if (visual == null)
            return;

        float angle =
            Vector2.SignedAngle(
                Vector2.down,
                gravityDirection
            );

        visual.rotation =
            Quaternion.Euler(
                0,
                0,
                angle
            );
    }
}