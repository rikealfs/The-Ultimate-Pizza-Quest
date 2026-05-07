using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SortingUIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    public GameObject floatingTextPrefab;
    public Transform floatingTextParent;

    [Header("End Screen")]
    public GameObject endPanel;
    public TextMeshProUGUI endText;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    void Update()
    {
        var gm = SortingGameManager.Instance;
        if (gm == null) return;

        timerText.text = "Time: " + Mathf.Ceil(gm.timeLeft);
    }

    public void UpdateScore(int score)
    {
        var gm = SortingGameManager.Instance;
        if (gm == null) return;

        int goal = gm.winScore;

        if (score >= goal)
        {
            scoreText.text = $"Score: <color=green>{score}</color> / {goal}";
        }
        else
        {
            scoreText.text = $"Score: {score} / {goal}";
        }
    }

    public void ShowFloatingText(Vector3 worldPos, string text, Color color)
    {
        GameObject obj = Instantiate(floatingTextPrefab, floatingTextParent);

        RectTransform rt = obj.GetComponent<RectTransform>();

        
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = floatingTextParent as RectTransform;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,
            out localPos
        );

        rt.anchoredPosition = localPos;

        FloatingScore fs = obj.GetComponent<FloatingScore>();
        if (fs != null)
        {
            fs.Setup(text, color);
        }
    }

    public void ShowEndScreen(bool win)
    {
        endPanel.SetActive(true);
        endText.text = win ? "YOU WIN!" : "YOU LOSE!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainWorld()
    {
        SceneManager.LoadScene("ToppingStartingHub");
    }

    public void PlayCorrectSound()
{
    if (audioSource != null && correctSound != null)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f); 
        audioSource.PlayOneShot(correctSound);
    }
}

public void PlayWrongSound()
{
    if (audioSource != null && wrongSound != null)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(wrongSound);
    }
}
}