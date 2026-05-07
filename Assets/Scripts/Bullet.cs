using UnityEngine;

public class Bullet : MonoBehaviour
{   
    // this would be the speed that you want the build to go at 
    [SerializeField] private float bulletSpeed = 15f;

    // how long bullet can last for 
    [SerializeField] private float lifeLength = 2f;

    // direction of the bullet itself 
    private int direction = 1;  


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // destroys the bullet if it never hit the boss and makes it so it doesnt fly forever and take resources for no reason
        Destroy(gameObject, lifeLength);
    }

    // Update is called once per frame
    void Update()
    {
        // wherever your facing, move the bullet by the speed mentioned, make sure to use deltaTime so its framerate independent 
        transform.Translate(Vector3.right * direction * bulletSpeed * Time.deltaTime);
        
    }

    // sets directin of the bullet
    public void SetDirection(int temp)
    {
        direction = temp;
    }

    // bullet collides with something else 
    void OnTriggerEnter(Collider other)
    {
        // make sure its not you your hitting LMAO
        if(other.CompareTag("Player"))
        {
            return;
        }

        // ignore boss bullets for now
        if(other.CompareTag("BossBullets"))
        {
            return;
        }

        // if bullet hits a boss
        if(other.CompareTag("Boss"))
        {
            // take damage, not exactly sure how much yet so ima just put a number i think feels right
            BossHealth bossHP = other.GetComponent<BossHealth>();
            if (bossHP != null)
            {
                bossHP.TakeDamage(1);
            }
        }



        // for now jsut destroy, later if it collides with boss, lower the hp of the boss itself
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        
    }
}
