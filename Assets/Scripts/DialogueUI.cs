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

    public void ShowDialogue(string speakerName, string text)
    {
        panel.SetActive(true);

        nameText.text = speakerName; // 👈 NEW
        currentText = text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        isTyping = true;

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }

    // Skip button calls this
    public void SkipText()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentText;
            isTyping = false;
        }
    }

    // Exit button calls this
    public void HideDialogue()
    {
        panel.SetActive(false);
    }
}