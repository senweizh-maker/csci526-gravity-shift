using UnityEngine;

using UnityEngine.InputSystem;
 
[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;
    public float followSmoothTime = 0.15f;
 
    [Header("Zoom")]
    [Tooltip("Orthographic size while following the player. Match your Level 1 camera size.")]
    public float followSize = 4f;
    [Tooltip("Orthographic size while holding the planning key (Z).")]
    public float planningSize = 8f;
    [Tooltip("Optional. Camera centers here while Z is held.")]
    public Transform planningFocus;
    public float zoomSmoothTime = 0.2f;
 
    [Header("Level Bounds (optional)")]
    public bool useBounds = false;
    public Vector2 boundsMin;
    public Vector2 boundsMax;
 
    private Camera cam;
    private Vector3 moveVelocity;
    private float sizeVelocity;
 
    private bool hasZoneOverride;
    private float zoneSize;
    private Vector2 zoneFocus;
 
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
 
    // Called by CameraZone when the player enters it
    public void SetZoneOverride(float size, Vector2 focus)
    {
        hasZoneOverride = true;
        zoneSize = size;
        zoneFocus = focus;
    }
 
    // Called by CameraZone when the player leaves it
    public void ClearZoneOverride()
    {
        hasZoneOverride = false;
    }
 
    private void LateUpdate()
    {
        if (target == null)
            return;
 
        bool planning =
            Keyboard.current != null &&
            Keyboard.current.zKey.isPressed;
 
        float targetSize;
        Vector3 focus;
 
        if (planning)
        {
            targetSize = planningSize;
            focus = planningFocus != null ? planningFocus.position : target.position;
        }
        else if (hasZoneOverride)
        {
            targetSize = zoneSize;
            focus = zoneFocus;
        }
        else
        {
            targetSize = followSize;
            focus = target.position;
        }
 
        // Unscaled time so the camera still moves while Time.timeScale is 0
        float dt = Time.unscaledDeltaTime;
 
        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize,
            targetSize,
            ref sizeVelocity,
            zoomSmoothTime,
            Mathf.Infinity,
            dt
        );
 
        Vector3 desired = new Vector3(focus.x, focus.y, transform.position.z);
 
        if (useBounds)
            desired = ClampToBounds(desired);
 
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref moveVelocity,
            followSmoothTime,
            Mathf.Infinity,
            dt
        );
    }
 
    private Vector3 ClampToBounds(Vector3 p)
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
 
        float minX = boundsMin.x + halfW;
        float maxX = boundsMax.x - halfW;
        float minY = boundsMin.y + halfH;
        float maxY = boundsMax.y - halfH;
 
        p.x = minX > maxX ? (boundsMin.x + boundsMax.x) * 0.5f : Mathf.Clamp(p.x, minX, maxX);
        p.y = minY > maxY ? (boundsMin.y + boundsMax.y) * 0.5f : Mathf.Clamp(p.y, minY, maxY);
 
        return p;
    }
}