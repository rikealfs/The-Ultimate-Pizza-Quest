using UnityEngine;
using UnityEngine.UI;
public class BossHealth : MonoBehaviour
{
    // hp of the boss for now, can change later not sure about how much hp should make it for now
    [SerializeField] private int maxHP = 10;

    // current hp of the boss
    [SerializeField] private int currentHP;

    // reference to slider
    [SerializeField] private Slider hpBar;

    // reference to boss sprite renderer
    [SerializeField] private SpriteRenderer bossSpriteRenderer;

    // sprite you switch to when he gets half hp
    [SerializeField] private Sprite phaseTwo;
    
    // reference to boss attack so we can switch shooting later
    [SerializeField] private BossAttack bossAttack;

    // reference to dialogue
    [SerializeField] private BossDialogue bossDialogue;

    // to show when you win
    [SerializeField] private GameObject winPanel;

    //trigger phase two once only
    private bool phaseTwoTrigger = false;

    // makes sure that death can only be considered once, basically stops it so that boss doesnt spam voicelines when he gets hit by leftover bullets 
    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // make current max cause ofc
        currentHP = maxHP;

        // make sure the UI bar also has the values for the HP
        if(hpBar != null)
        {
            hpBar.maxValue = maxHP;
            hpBar.value = currentHP;
        }
    }

    public void TakeDamage(int dmg)
    {
        // take hp depending on how much dmg we do 
        currentHP -= dmg;
        
        // your dead big bro stop spamming voicelines
        if(isDead)
        {
            return;
        }
        // updating UI bar
        if(hpBar != null)
        {
            hpBar.value = currentHP;
        }

        // hasant gone to phase yet and also passed half hp thresholf
        if(!phaseTwoTrigger && currentHP <= maxHP / 2)
        {
            phaseTwoTrigger = true;

            // makes it so it clears all bullets once phase two happens so leftover bullets dont hit the player or boss
            clearAllBullets();

            // switch to new attacks with the boss transformation
            if(bossAttack != null)
            {
                bossAttack.enterPhaseTwo();
            }

            // switch to the phase 2 sprite
            if(bossSpriteRenderer != null && phaseTwo != null)
            {
                bossSpriteRenderer.sprite = phaseTwo;

                // boss sprite for phase two is facing left already, DO NOT FLIP
                bossSpriteRenderer.flipX = false;
            }

            // play phase 2 stuff
            if(bossDialogue != null)
            {
                bossDialogue.phaseTwoDialogue();
            }
        }

        // just wanna make sure it does the dmg and takes away the amount of hp expected
        Debug.Log(currentHP + " " + dmg);

        // dead if 0 or less
        if(currentHP <= 0)
        {
            Dead();
        }
    }

    // simple death for now 
    private void Dead()
    {
        Debug.Log("Dead() called");

        if(isDead)
        {
            Debug.Log("Already dead, returning");

            return;
        }

        isDead = true;
        clearAllBullets();

        // make sure boss doesnt attack
        if(bossAttack != null)
        {
            bossAttack.setPause(true);
        }

        if(bossDialogue != null)
        {
            Debug.Log("Calling endDialogue and starting win panel coroutine");

            bossDialogue.endDialogue();

            // show after death dialogue
            StartCoroutine(showWinPanelAfterDelay(5f));
            //  mkae sure that voiceline ends before boss ies
            
        }
        
        else
        {
            Debug.Log("No bossDialogue, showing win panel immediately");

            StartCoroutine(showWinPanelAfterDelay(0f));
        }
        
    }

    // wait before showing victory royale
    private System.Collections.IEnumerator showWinPanelAfterDelay(float delay)
    {
        Debug.Log("Coroutine started, waiting " + delay + " seconds");
        yield return new WaitForSeconds(delay);
        Debug.Log("Wait done, calling showWinPanel");

        showWinPanel();

        Destroy(gameObject);
    }

    private void showWinPanel()
    {
        Debug.Log("showWinPanel called, winPanel is " + (winPanel != null ? "assigned" : "NULL"));
        if(winPanel != null)
        {
            // actually show win panel 
            winPanel.SetActive(true);
            Debug.Log("Win panel activated");
        }
    }

    // destroy bullets as soon as you hit phase two
    private void clearAllBullets()
    {
        // find everything thats considered a player bulley and destroy it
        GameObject[] playerBullets = GameObject.FindGameObjectsWithTag("Bullets");
        foreach (GameObject bullet in playerBullets)
        {
            Destroy(bullet);
        }

        // find everything thats considered a boss bullet and destroy it
        GameObject[] bossBullets = GameObject.FindGameObjectsWithTag("BossBullets");
        foreach (GameObject bullet in bossBullets)
        {
            Destroy(bullet);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
