using UnityEngine;
using TMPro;

public class PebbleUI : MonoBehaviour
{
    public static PebbleUI Instance;

    public TMP_Text pebbleText;
    public RectTransform bumpTarget;

    private Vector3 originalScale;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (pebbleText == null)
            pebbleText = GetComponent<TMP_Text>();

        if (bumpTarget == null)
            bumpTarget = GetComponent<RectTransform>();

        originalScale = bumpTarget.localScale;

        // Load pebble count from PebbleManager
        if (PebbleManager.Instance != null)
        {
            UpdatePebbleText(PebbleManager.Instance.pebbles);
        }
        else
        {
            UpdatePebbleText(0);
        }
    }
    
    void OnEnable()
    {
        // Update when UI becomes active
        if (PebbleManager.Instance != null && pebbleText != null)
        {
            UpdatePebbleText(PebbleManager.Instance.pebbles);
        }
    }

    public void UpdatePebbleText(int amount)
    {
        if (pebbleText != null)
            pebbleText.text = amount.ToString();
    }

    public void PlayBump()
    {
        StopAllCoroutines();
        StartCoroutine(BumpRoutine());
    }

    System.Collections.IEnumerator BumpRoutine()
    {
        float duration = 0.12f;
        float t = 0f;
        Vector3 big = originalScale * 1.15f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            bumpTarget.localScale = Vector3.Lerp(originalScale, big, t / duration);
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            bumpTarget.localScale = Vector3.Lerp(big, originalScale, t / duration);
            yield return null;
        }

        bumpTarget.localScale = originalScale;
    }
}
