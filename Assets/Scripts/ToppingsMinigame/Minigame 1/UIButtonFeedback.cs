using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Vector3 targetScale;
    Vector3 originalScale;

    public float speed = 10f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * 0.85f;

        if (audioSource != null && clickSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); 
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}