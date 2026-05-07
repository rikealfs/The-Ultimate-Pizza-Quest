using UnityEngine;

public class BossBullet : MonoBehaviour
{

    // speed of the bullet
    [SerializeField] private float speed = 6f;

    // time that the bullet can be on before despawns
    [SerializeField] private float time = 4f;

    // dmg it does to the player
    [SerializeField] private int dmg = 1;

    // way that the bullet goes, will always be left 
    private Vector3 bulletDirection = Vector3.left;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, time);
    }

    // Update is called once per frame
    void Update()
    {
        // move in the direction and make sure frames dont influence it 
        transform.Translate(bulletDirection * speed * Time.deltaTime);
        
    }

    // aims the bullet in the right direction
    public void Direction(Vector3 dir)
    {
        bulletDirection = dir.normalized;
    }

    void OnTriggerEnter(Collider other)
    {
        // we dont want the boss bullets hitting him now do we
        if(other.CompareTag("Boss"))
        {
            return;
        }

        // do not want the bullets to do something if they collide somehow
        if(other.CompareTag("BossBullets"))
        {
            return;
        }

        // if you hit the player make them take damage
        if(other.CompareTag("Player"))
        {
            Debug.Log("HIT REGISTERED");
            PlayerHealth playerHP = other.GetComponent<PlayerHealth>();
            if(playerHP != null)
            {
                playerHP.TakeDamage(dmg);
            }
        }

        Destroy(gameObject);
    }
}
