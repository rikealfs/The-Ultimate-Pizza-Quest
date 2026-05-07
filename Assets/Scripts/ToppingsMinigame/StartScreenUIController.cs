using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartScreenUIController : MonoBehaviour
{
    public Button collectButton;
    public Button sortButton;

    public TextMeshProUGUI ticketText;

    int tickets;

    void Start()
    {
        tickets = GameProgressManager.Instance.tickets;
        UpdateUI();
    }

    void Update()
    {
        UpdateUI(); // keep UI always in sync
    }

    void UpdateUI()
    {
        var gp = GameProgressManager.Instance;
        if (gp == null) return;
        

        // Collect always available
        collectButton.interactable = true;

        // Sort locked until collect is done
        sortButton.interactable = gp.sortingUnlocked;

        sortButton.image.color = gp.sortingUnlocked ? Color.white : Color.gray;

    }
}