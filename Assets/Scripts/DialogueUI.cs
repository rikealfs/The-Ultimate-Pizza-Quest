using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    
    public GameObject panel;
    public TextMeshProUGUI dialogueText;

    public TextMeshProUGUI nameText;
    private Coroutine typingCoroutine;
    private string currentText;
    private bool isTyping;

    //displays typing text and the npcs name 
    public void ShowDialogue(string speakerName, string text)
    {
        panel.SetActive(true);

        nameText.text = speakerName; 
        currentText = text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        isTyping = true;

        // adds each letter individually to the displayed text string to create a typing affect 
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }

    // Skips over the talking/ typewriter animation in dialogue box
    public void SkipText()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentText;
            isTyping = false;
        }
    }

    // Makes dialogue box invisible when clicked
    public void HideDialogue()
    {
        panel.SetActive(false);
    }
}