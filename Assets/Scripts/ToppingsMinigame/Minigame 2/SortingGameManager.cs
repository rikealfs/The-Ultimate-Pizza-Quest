using UnityEngine;

public class SortingGameManager : MonoBehaviour
{
    public static SortingGameManager Instance;

    [Header("Game Rules")]
    public int score = 0;
    public int winScore = 300;
    public float timeLeft = 30f;
    public bool gameActive = true;

    [Header("Score Values")]
    public int correctPoints = 10;
    public int wrongPenalty = 5;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!gameActive) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            EndGame();
        }
    }

    public void AddCorrect()
    {
        if (!gameActive) return;

        score += correctPoints;
        SortingUIManager ui = FindFirstObjectByType<SortingUIManager>();
if (ui != null)
{
    ui.UpdateScore(score);
}
    }

    public void AddWrong()
    {
        if (!gameActive) return;

        score -= wrongPenalty;
        if (score < 0) score = 0;

        SortingUIManager ui = FindFirstObjectByType<SortingUIManager>();
if (ui != null)
{
    ui.UpdateScore(score);
}
    }

    void EndGame()
    {
        gameActive = false;

        bool win = score >= winScore;

        SortingUIManager ui = FindFirstObjectByType<SortingUIManager>();
        if (ui != null)
        {
            ui.ShowEndScreen(win);
        }

        Debug.Log(win ? "YOU WIN!" : "YOU LOSE!");

        if (win)
        {
            GameProgressManager.Instance.tickets += 2;
        }
        else
        {
            GameProgressManager.Instance.tickets += 1;
        }
    }
}