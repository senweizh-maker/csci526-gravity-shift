using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class CameraZone : MonoBehaviour
{
    [Tooltip("Orthographic size while the player is inside this zone.")]
    public float zoomSize = 9f;
 
    [Tooltip("Optional. Camera centers here. If empty, uses this object's position.")]
    public Transform focusPoint;
 
    private FollowCamera followCamera;
 
    private void Reset()
    {
        
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
 
    private void Awake()
    {
        followCamera = FindFirstObjectByType<FollowCamera>();
    }
 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || followCamera == null)
            return;
 
        Vector2 focus = focusPoint != null
            ? (Vector2)focusPoint.position
            : (Vector2)transform.position;
 
        followCamera.SetZoneOverride(zoomSize, focus);
    }
 
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || followCamera == null)
            return;
 
        followCamera.ClearZoneOverride();
    }
}