using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    public bool hasInitialized = false;

    public bool collectDone = false;
    public bool sortingUnlocked = false;
    public bool sortingDone = false;

    public int collectedScore = 0;
    public int sortedScore = 0;

    public int tickets = 0;

    public int mushroomCount = 0;
    public int pepperoniCount = 0;
    public int pepperCount = 0;

    void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!hasInitialized)
        {
            ResetProgress();
            hasInitialized = true;
        }
    }
    else
    {
        Destroy(gameObject);
    }
}

    public void ResetProgress()
{
    collectDone = false;
    sortingUnlocked = false;
    sortingDone = false;
}

    public void CompleteCollect()
    {
        collectDone = true;
        sortingUnlocked = true;
    }

    public void CompleteSorting(int score)
    {
        sortingDone = true;
        sortedScore = score;
    }
}