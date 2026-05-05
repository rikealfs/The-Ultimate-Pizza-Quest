using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 720f; // How fast the player turns
    public Joystick joystick;
    public Animator anim;
    
    private Rigidbody rb;

    void Start()
    {
        // Grab the Rigidbody from the parent object
        rb = GetComponent<Rigidbody>();
        
        // Freeze rotation so the physics engine doesn't tilt the player over
        rb.freezeRotation = true; 
    }

    void Update()
    {
        // 1. Get Input
        float h = joystick.Horizontal;
        float v = joystick.Vertical;
        Vector3 moveDirection = new Vector3(h, 0, v).normalized;

        // 2. Handle Animations
        // Use magnitude so it's always a positive number (0 to 1)
        float moveMagnitude = new Vector2(h, v).magnitude;
        anim.SetFloat("Speed", moveMagnitude);

        // 3. Handle Movement & Rotation
        if (moveDirection.magnitude >= 0.1f)
        {
            // Calculate where we want to look
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            
            // Smoothly rotate toward that direction
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move the player
            // We use MovePosition for Rigidbodies to keep physics happy
            rb.MovePosition(transform.position + moveDirection * speed * Time.deltaTime);
        }
    }
}