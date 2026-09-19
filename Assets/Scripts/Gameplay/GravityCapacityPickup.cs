using UnityEngine;
using TMPro;

public class GravityCapacityPickup : MonoBehaviour
{
    public int amount = 1;

    private bool collected = false;

    [SerializeField]
    private TMP_Text amountText;

    private void Awake()
    {
        if (amountText == null)
        {
            amountText = GetComponentInChildren<TMP_Text>();
        }

        UpdateText();
    }

    private void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (amountText != null)
        {
            amountText.text = "+" + amount;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (GravityGridManager.Instance == null)
        {
            Debug.LogError("GravityGridManager.Instance is NULL!");
            return;
        }

        collected = true;

        GravityGridManager.Instance.AddCapacity(amount);

        Destroy(gameObject);
    }

    // 修改 Inspector 中 amount 时，也自动刷新文字
    private void OnValidate()
    {
        if (amountText == null)
        {
            amountText = GetComponentInChildren<TMP_Text>();
        }

        UpdateText();
    }
}