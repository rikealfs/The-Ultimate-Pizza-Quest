using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{   
    // max hp of the player, was thinking of making it hearts to make it cool later
    [SerializeField] private int maxHP = 5;

    // current hp
    [SerializeField] private int currentHP;

    // reference to hp bar
    [SerializeField] private Slider hpBar;

    // panel that shows if you lose
    [SerializeField] private GameObject gameOver;

    // used so the boss will stop attacking as soon as game over happens
    [SerializeField] private BossAttack bossAttack;

    private bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // start at full
        currentHP = maxHP;

        //sync slider with player hp
        if(hpBar != null)
        {
            hpBar.maxValue = maxHP;
            hpBar.value = currentHP;
        }
        
    }

    public void TakeDamage(int dmg)
    {
        // dead, no need to count more hits
        if(isDead)
        {
            return;
        }
        
        currentHP -= dmg;

        // update hp bar
        if(hpBar != null)
        {
            hpBar.value = currentHP;
        }

        Debug.Log(dmg + " " + currentHP);

        // dead if 0 or less
        if(currentHP <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        isDead = true;
        Debug.Log("YOU DIED GG");

        // make sure cant do anything after death
        CheeseMinigamePlayerMovement input = GetComponent<CheeseMinigamePlayerMovement>();
        if(input != null)
        {
            input.setDead();
        }

        // boss stop attacking
        if(bossAttack != null)
        {
            bossAttack.setPause(true);
        }

        // game over panel
        if(gameOver != null)
        {
            gameOver.SetActive(true);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
