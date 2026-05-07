using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ChooseToppingManager : MonoBehaviour
{
    public TextMeshProUGUI ticketText;

    public TextMeshProUGUI mushroomText;
    public TextMeshProUGUI pepperoniText;
    public TextMeshProUGUI pepperText;

    public Button mushroomAddButton;
    public Button pepperoniAddButton;
    public Button pepperAddButton;

    public AudioSource audioSource;
    public AudioClip clickSound;

    int tickets;

    int mushroom = 0;
    int pepperoni = 0;
    int pepper = 0;

    void Start()
    {
        tickets = GameProgressManager.Instance.tickets;
        UpdateUI();
    }

    void UpdateUI()
    {
        ticketText.text = tickets.ToString();

        mushroomText.text = mushroom.ToString();
        pepperoniText.text = pepperoni.ToString();
        pepperText.text = pepper.ToString();

        bool canAdd = tickets > 0;

        mushroomAddButton.interactable = canAdd;
        pepperoniAddButton.interactable = canAdd;
        pepperAddButton.interactable = canAdd;
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void AddMushroom()
    {
        if (tickets <= 0) return;

        mushroom++;
        tickets--;
        PlayClickSound();
        UpdateUI();
    }

    public void RemoveMushroom()
    {
        if (mushroom <= 0) return;

        mushroom--;
        tickets++;
        PlayClickSound();
        UpdateUI();
    }

    public void AddPepperoni()
    {
        if (tickets <= 0) return;

        pepperoni++;
        tickets--;
        PlayClickSound();
        UpdateUI();
    }

    public void RemovePepperoni()
    {
        if (pepperoni <= 0) return;

        pepperoni--;
        tickets++;
        PlayClickSound();
        UpdateUI();
    }

    public void AddPepper()
    {
        if (tickets <= 0) return;

        pepper++;
        tickets--;
        PlayClickSound();
        UpdateUI();
    }

    public void RemovePepper()
    {
        if (pepper <= 0) return;

        pepper--;
        tickets++;
        PlayClickSound();
        UpdateUI();
    }

    public void Confirm()
    {
        // GameProgressManager.Instance.mushroomCount = mushroom;
        // GameProgressManager.Instance.pepperoniCount = pepperoni;
        // GameProgressManager.Instance.pepperCount = pepper;

        GameProgressManager.Instance.CompleteChoose(mushroom, pepperoni, pepper, tickets);

        SceneManager.LoadScene("ToppingStartingHub");
    }
}