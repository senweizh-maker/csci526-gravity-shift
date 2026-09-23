using UnityEngine;

public class ExitGoal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelManager.Instance is NULL!");
            return;
        }

        LevelManager.Instance.CompleteLevel();
    }
}