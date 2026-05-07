using UnityEngine;

public class Topping : MonoBehaviour
{
    public ToppingType type;
    public float fallSpeed = 5f;

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -6f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandleCatch();
            Destroy(gameObject);
        }
    }

    void HandleCatch()
    {
        CollectGameManager.Instance.AddTopping(type);
    }
}