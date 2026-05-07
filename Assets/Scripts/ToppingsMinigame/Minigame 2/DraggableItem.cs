using UnityEngine;
using System.Collections;

public class DraggableItem : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    public ToppingType type;

    public bool IsDragging => isDragging;

    private Vector3 startPosition;

    void OnMouseDown()
    {
        isDragging = true;

        startPosition = transform.position;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        offset = transform.position - mousePos;
    }

    void OnMouseDrag()
    {
        if (Input.touchCount > 1) return;
        if (!isDragging) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = Vector3.Lerp(
    transform.position,
    new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, 0),
    Time.deltaTime * 15f
);

        // 🔥 Reset all basket highlights first
        Basket[] allBaskets = FindObjectsByType<Basket>(FindObjectsSortMode.None);
        foreach (Basket b in allBaskets)
        {
            b.Highlight(false);
        }

        // Highlight hovered basket
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 1.5f);

        if (hit != null)
        {
            Basket basket = hit.GetComponent<Basket>();
            if (basket != null)
            {
                basket.Highlight(true);
            }
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        float offsetY = GetComponent<Collider2D>().bounds.extents.y;
        Vector2 dropPoint = new Vector2(transform.position.x, transform.position.y - offsetY);

        Collider2D[] hits = Physics2D.OverlapCircleAll(dropPoint, 1.5f);

        foreach (Collider2D h in hits)
        {
            Basket basket = h.GetComponent<Basket>();

            if (basket != null)
            {
                // Debug.Log("Dropped on: " + basket.name);

                if (basket.acceptedType == type)
                {
                    // Correct 
                    SortingUIManager ui = FindFirstObjectByType<SortingUIManager>();
                    if (ui != null)
                    {
                        ui.ShowFloatingText(transform.position, "+10", Color.green);
                        ui.PlayCorrectSound();
                    }

                    if (SortingGameManager.Instance != null)
                    {
                        SortingGameManager.Instance.AddCorrect();
                    }

                    StartCoroutine(SmoothSnapAndLock(basket));
                    return;
                }
                else
                {
                    // Wrong drop
                    // Debug.Log("WRONG DROP");

                    SortingUIManager ui = FindFirstObjectByType<SortingUIManager>();
                    if (ui != null)
                    {
                        ui.ShowFloatingText(transform.position, "-5", Color.red);
                        ui.PlayWrongSound(); 
                    }

                    // UIManager ui = FindFirstObjectByType<UIManager>();
                    // if (ui != null)
                    // {
                    //     ui.TriggerBombEffect();
                    // }

                    if (SortingGameManager.Instance != null)
                    {
                        SortingGameManager.Instance.AddWrong();
                    }

                    Destroy(gameObject);
                    return;
                }
            }
        }

        // Not dropped on any basket
        // Debug.Log("Not dropped on basket");
        transform.position = startPosition;
    }

    IEnumerator SmoothSnapAndLock(Basket basket)
    {
        Vector3 start = transform.position;

        // stack items slightly
        float stackOffset = basket.transform.childCount * 0.2f;
        Vector3 target = basket.transform.position + new Vector3(0, stackOffset, 0);

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 6f;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        // Move behind basket
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = -1;
        }

        // Stop conveyor movement
        ConveyorMove conveyor = GetComponent<ConveyorMove>();
        if (conveyor != null)
        {
            conveyor.enabled = false;
        }

        // Disable dragging
        isDragging = false;
        enabled = false;

        // Disable collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Parent for organization
        transform.SetParent(basket.transform);

        // // Add topping 
        // if (SortingGameManager.Instance != null)
        // {
        //     SortingGameManager.Instance.AddCorrect();
        // }
    }
}