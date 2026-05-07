using UnityEngine;
using TMPro;
using System.Collections;

public class BossDialogue : MonoBehaviour
{   
    // would be the actual object panel that holds the dialogue
    [SerializeField] private GameObject dialoguePanel;

    // actual dialogue text in the panel
    [SerializeField] private TextMeshProUGUI dialogueText;

    // the cool ahhh voice lines
    [SerializeField] private AudioSource audio;

    // voice clips start, half hp, and end
    [SerializeField] private AudioClip start;
    [SerializeField] private AudioClip half;
    [SerializeField] private AudioClip end;

    // this would be the text that comes wiht the phases
    [SerializeField] [TextArea(3, 6)] private string startText;
    [SerializeField] [TextArea(3, 6)] private string halfText;
    [SerializeField] [TextArea(3, 6)] private string endText;

    // used to pause the player and boss attacks when at that phase 
    [SerializeField] private CheeseMinigamePlayerMovement playerMovement;
    [SerializeField] private BossAttack bossAttack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // play both start text and clip at the beginning 
        PlayDialogue(start, startText);
    }

    // play for phase two
    public void phaseTwoDialogue()
    {
        PlayDialogue(half, halfText);
    }
    
    // playing end stuff
    public void endDialogue()
    {
        PlayDialogue(end, endText);
    }

    private void PlayDialogue(AudioClip clip, string text)
    {
        // used to play general vlips and text when its their time 
        StartCoroutine(DialogueRoutine(clip,text));
    }

    private IEnumerator DialogueRoutine(AudioClip clip, string text)
    {
        // pause plauer and boss since dialogue is about to happen
        if(playerMovement != null)
        {
            playerMovement.setPause(true);
        }

        if(bossAttack != null)
        {
            bossAttack.setPause(true);
        }

        // show panel + text
        if(dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if(dialogueText != null)
        {
            dialogueText.text = text;
        }

        // play actual audio
        if(audio != null && clip != null)
        {
            audio.clip = clip;
            audio.Play();

            yield return new WaitForSeconds(clip.length);
        }

        
        else
        {
            // wait a bit then proceed
            yield return new WaitForSeconds(3f);
        }

        // hide the panel
        if(dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // resume -> player and boss can do whatever
        if(playerMovement != null)
        {
            playerMovement.setPause(false);
        }

        if(bossAttack != null)
        {
            bossAttack.setPause(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
