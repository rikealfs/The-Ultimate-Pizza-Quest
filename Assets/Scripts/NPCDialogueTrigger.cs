using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public string npcName = "Bob";
    public string dialogue = "Hello there!";
    public DialogueUI dialogueUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueUI.ShowDialogue(npcName,dialogue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueUI.HideDialogue();
        }
    }
}