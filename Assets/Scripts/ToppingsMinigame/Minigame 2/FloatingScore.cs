using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    public float moveSpeed = 200f;
    public float fadeSpeed = 2f;

    private TextMeshProUGUI text;
    private CanvasGroup canvasGroup;
    private RectTransform rt;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
    }

    public void Setup(string message, Color color)
    {
        text.text = message;
        text.color = color;

        canvasGroup.alpha = 1f;
        rt.localScale = Vector3.one;
        transform.localScale = Vector3.one * 1.5f;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 10f);
        // Move upward 
        rt.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

        // Fade out
        canvasGroup.alpha -= fadeSpeed * Time.deltaTime;

        if (canvasGroup.alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}