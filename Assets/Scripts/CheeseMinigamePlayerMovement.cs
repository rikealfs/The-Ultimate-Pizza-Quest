using UnityEngine;

public class CheeseMinigamePlayerMovement : MonoBehaviour
{
    // so can use joystick 
    [SerializeField] private Joystick joystick;

    // how fast we want the player to be moving, can edit later depending on whats needed 
    [SerializeField] private float moveSpeed = 5f;

    // how strong the jump should be 
    [SerializeField] private float jumpHeight = 7f;

    // this would be how much the joystick would have to go up to be able to be considered a jump 
    [SerializeField] private float jumpThreshold = 0.7f;

    //bullet prefab reference 
    [SerializeField] private GameObject bulletPrefab;

    //used to decide where the bullet spawns compared to the player
    [SerializeField] private Transform spawnPoint;

    //cooldown between bullets
    [SerializeField] private float shootCooldown = 0.3f;

    // reference to player sprite
    [SerializeField] private SpriteRenderer spriteRenderer;

    // player cant move or do anything when this is true
    private bool isPaused = false;

    // reference to rigidbody
    private Rigidbody rigid;

    // cant jump in midair so have a variable to check whether your on the ground or not
    private bool onGround;

    // request for jump
    private bool queueJump;

    //where the player is facing 
    private int facingDirection = 1;

    //tracks when student shoots again
    private float nextShootTime = 0f;


    public void setPause(bool paused)
    {
        isPaused = paused;

        //stop the players velocity if they pause 
        if(paused && rigid != null)
        {
            rigid.linearVelocity = Vector3.zero;
        }
    }

    // blocks input when dead
    private bool isDead = false;

    // wanna make it so that if the player dies he cant shoot any bullet
    public void setDead()
    {
        isDead = true;
        if(rigid != null)
        {
            rigid.linearVelocity = Vector3.zero;
        }
    }
    void Start()
    {
    // store rigidbody in rigid itself at the start
    rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(isDead)
        {
            return;
        }

        if(isPaused)
        {
            return;
        }
        // has to check whether or not what you did should be considered a jump and wheter or not you were on the ground
        if(joystick.Vertical > jumpThreshold && onGround)
        {
            // both are true? then queue a jump
            queueJump = true;
        }
    }

    void FixedUpdate()
    {
        if(isDead)
        {
            return;
        }

        // paused, shouldnt do anything 
        if(isPaused)
        {
            return;
        }

        // this itself would read basically how far dragged the joysitck is when it comes to the x parts -> -1 = left 1 = right
        float moveX = joystick.Horizontal;

        // x -> horizontal velocity, y -> vertical velocity, y is using rigidbodys y velocity as it simulates a gravity type effect and already works well
        // z is nothing since i want it to be 2d
        Vector3 velocity = new Vector3(moveX * moveSpeed, rigid.linearVelocity.y, 0f);

        // add it to rigidbody linear velocity to try new veloicty
        rigid.linearVelocity = velocity;

        

        // if a jump was queued
        if(queueJump)
        {
            // keep x the same since it shouldnt change anything but now use jump force to actuall perform the jump
            rigid.linearVelocity = new Vector3(rigid.linearVelocity.x, jumpHeight, 0f);

            // make sure it says your not on the ground just so even if you put the joystick vertially you cannot perform a jump
            onGround = false;

            // no more jumps to queue
            queueJump = false;

        }

    // used to show what direction the player is in 
    if (moveX > 0.1f)
    { 
        facingDirection = 1;
    }
    else if (moveX < -0.1f)
    { 
        facingDirection = -1;
    }

    // this is to make it so that wherever your facing with the joystick is where the sprite will also turn so it wont look clunky
    if(spriteRenderer != null)
        {
            spriteRenderer.flipX = (facingDirection == -1);
        }

    }

    // when player is tpuching anything or "colliding" to be considered on the ground what they have to be colliding on has to be considered the ground 
    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    public void Shoot()
    {
        if(isDead)
        {
            return;
        }
        if(isPaused)
        {
            return;
        }
        // less than the next shoot cooldown time, cannot shoot so return
        if (Time.time < nextShootTime)
        {
            return;
        } 

        //shoot the bullet depending on where the player is turned, left bullet travels left, same pattern for right, make sure that it’s in front of the player and not exactly on
        Vector3 spawnPos = transform.position + new Vector3(facingDirection * Mathf.Abs(spawnPoint.localPosition.x), 0f, 0f);

        // makes the bullet
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        //bullet script
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetDirection(facingDirection);

        //reset cooldown time
        nextShootTime = Time.time + shootCooldown;
    }



}
