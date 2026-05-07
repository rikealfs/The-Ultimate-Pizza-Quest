using UnityEngine;

public class ConveyorMove : MonoBehaviour
{
    public float speed = 2f;

    void Awake()
{
    if (GetComponents<ConveyorMove>().Length > 1)
    {
        Destroy(this);
    }
}

    void Update()
    {
        DraggableItem drag = GetComponent<DraggableItem>();

        if (drag != null && drag.IsDragging) return;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }
}