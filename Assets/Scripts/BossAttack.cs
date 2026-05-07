using UnityEngine;

public class BossAttack : MonoBehaviour
{
    // bullet prefab
    [SerializeField] private GameObject bossBullet;

    // where the bullets will spawn from
    [SerializeField] private Transform spawnPoint;

    // actual player who boss will be trying to shoot
    [SerializeField] private Transform player;

    // shooting time
    [SerializeField] private float fireRate = 1.5f;

    // phase two fire rate, made faster to reflect boss getting harder
    [SerializeField] private float phaseTwoFireRate = 0.8f;

    // spread of the bullets
    [SerializeField] private float spread = 15f;

    // boss cant shoot when paused 
    private bool isPaused = false;


    // tracks for phase two
    private bool checkPhaseTwo = false;

    // trachs for next shot so it can allow it to shoot
    private float nextFireTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // determines whether its paused or not 
    public void setPause(bool paused)
    {
        isPaused = paused;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPaused)
        {
            return;
        }
        
        if(player == null)
        {
            return;
        }

        // make sure taht the current time is more than fire time, if not dont shoot
        if(Time.time < nextFireTime)
        {
            return;
        }

        Shoot();

        // adjust fire rate depending on the phase
        float currentFireRate;

        if(checkPhaseTwo)
        {
            // change fire rate to phase two if it passes the condition
            currentFireRate = phaseTwoFireRate;
        }
        else
        {
            // just be normal otherwise
            currentFireRate = fireRate;
        }

        // for next shot
        nextFireTime = Time.time + currentFireRate;
        
    }

    private void Shoot()
    {
        // this would be the direction to the player so the boss will know where to shoot
        Vector3 direction = (player.position - spawnPoint.position).normalized;

        // for phase two will do a typa split shot
        if(checkPhaseTwo)
        {
            fireBullet(direction);
            fireBullet(Rotate(direction, spread));
            fireBullet(Rotate(direction, -spread));
        }
        else
        {
            // first phase just shoot one bullet
            fireBullet(direction);
        }

        // spawn bullet at spawn spawnPoint
        GameObject bullet = Instantiate(bossBullet, spawnPoint.position, Quaternion.identity);

        // controls direction of bullet
        BossBullet bulletScript = bullet.GetComponent<BossBullet>();
        if(bulletScript != null)
        {
            bulletScript.Direction(direction);
        }
    }

    
    private void fireBullet(Vector3 direction)
    {
        GameObject bullet = Instantiate(bossBullet, spawnPoint.position, Quaternion.identity);
        BossBullet bulletScript = bullet.GetComponent<BossBullet>();
        if(bulletScript != null)
        {
            bulletScript.Direction(direction);
        }
        
    }

    // used to rotate the bullet
    private Vector3 Rotate(Vector3 vector, float degrees)
    {
        return Quaternion.Euler(0,0, degrees) * vector;
    }

    // for entering phase two duhhhh
    public void enterPhaseTwo()
    {
        checkPhaseTwo = true;

    }
}
