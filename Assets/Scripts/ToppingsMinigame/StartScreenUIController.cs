using UnityEngine;
using UnityEngine.UI;

public class StartScreenUIController : MonoBehaviour
{
    public Button collectButton;
    public Button sortButton;

    void Start()
    {
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