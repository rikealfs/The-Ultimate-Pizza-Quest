using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // starting values, but can be adjusted 
    public float speed = 5f;
    public float rotationSpeed = 720f; 
    public Joystick joystick;
    public Animator anim;
    
    private Rigidbody rb;

    void Start()
    {
        // Grabs the Rigidbody from the parent object
        rb = GetComponent<Rigidbody>();
        
        // Freezes rotation so the physics engine doesn't tilt the player over
        rb.freezeRotation = true; 
    }

    void Update()
    {
       // gets input from the joystick 
        float h = joystick.Horizontal;
        float v = joystick.Vertical;
        Vector3 moveDirection = new Vector3(h, 0, v).normalized;

        // for handling player animation 
        // Use magnitude so it's always a positive number (0 to 1)
        float moveMagnitude = new Vector2(h, v).magnitude;
        anim.SetFloat("Speed", moveMagnitude);

        // For movement and rotation 
        if (moveDirection.magnitude >= 0.1f)
        {
            //calculates where the player wants to look and rotates in that direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            //for player movement 
            rb.MovePosition(transform.position + moveDirection * speed * Time.deltaTime);
        }
    }
}