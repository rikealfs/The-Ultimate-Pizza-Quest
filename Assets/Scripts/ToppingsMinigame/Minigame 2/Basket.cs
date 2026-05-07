using UnityEngine;

public class Basket : MonoBehaviour
{
    public ToppingType acceptedType;

    private SpriteRenderer sr;
    private Vector3 originalScale;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    public void Highlight(bool isOn)
    {
        if (sr != null)
        {
            sr.color = isOn ? Color.yellow : Color.white;
        }

        transform.localScale = isOn ? originalScale * 1.1f : originalScale;
    }
}