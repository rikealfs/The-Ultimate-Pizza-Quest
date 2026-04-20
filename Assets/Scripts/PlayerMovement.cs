using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Speed of the player movement
    public float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Getting directional input from the player
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //Apply movement using Space.World so up is always up, regardless of the player's rotation
        Vector3 move = new Vector3(h, 0, v);
        transform.Translate(move * speed * Time.deltaTime, Space.World);

    }
}
