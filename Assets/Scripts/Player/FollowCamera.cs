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
    [Tooltip("Orthographic size while holding the planning key.")]
    public float planningSize = 8f;
    [Tooltip("Optional. Camera centers here while zoomed out (e.g. the middle of the tunnel).")]
    public Transform planningFocus;
    public float zoomSmoothTime = 0.2f;
 
    [Header("Level Bounds (optional)")]
    public bool useBounds = false;
    public Vector2 boundsMin;
    public Vector2 boundsMax;
 
    private Camera cam;
    private Vector3 moveVelocity;
    private float sizeVelocity;
 
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
 
    private void LateUpdate()
    {
        if (target == null)
            return;
 
        bool planning =
            Keyboard.current != null &&
            Keyboard.current.zKey.isPressed;
 
        float targetSize = planning ? planningSize : followSize;
 
        Vector3 focus =
            (planning && planningFocus != null)
                ? planningFocus.position
                : target.position;
 
        
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
