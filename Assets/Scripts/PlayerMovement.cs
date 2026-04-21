using UnityEngine;

//The joystick component uses a prefab from the Joystick Pack

public class PlayerMovement : MonoBehaviour
{
    // Speed of the player movement
    public float speed = 5f;
    public Joystick joystick;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Getting directional input from the player
        float h = joystick.Horizontal; // Input.GetAxis("Horizontal");
        float v = joystick.Vertical; // Input.GetAxis("Vertical");

        //Apply movement using Space.World so up is always up, regardless of the player's rotation
        Vector3 move = new Vector3(h, 0, v);
        transform.Translate(move * speed * Time.deltaTime, Space.World);

    }
}
