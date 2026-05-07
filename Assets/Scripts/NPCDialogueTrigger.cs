using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    //starting variables 
    public string npcName = "Bob";
    public string dialogue = "Hello there!";
    public DialogueUI dialogueUI;

    private void OnTriggerEnter(Collider other)
    {
        // if the object that collided with the npc is the player, then show the dialogue box 
        if (other.CompareTag("Player"))
        {
            dialogueUI.ShowDialogue(npcName,dialogue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if the player is no longer there, hide the dialogue box 
        if (other.CompareTag("Player"))
        {
            dialogueUI.HideDialogue();
        }
    }
}