using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToppingsGameManager : MonoBehaviour
{
    public static ToppingsGameManager Instance;

    [Header("Game Settings")]
    public float timeLeft = 45f;
    public bool gameActive = true;

    public Dictionary<ToppingType, int> required = new();
    public Dictionary<ToppingType, int> collected = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeRecipe();
    }

    void Update()
    {
        if (!gameActive) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            EndGame(false);
        }
    }

    void InitializeRecipe()
    {
        required[ToppingType.Pepperoni] = 5;
        required[ToppingType.Mushroom] = 3;
        required[ToppingType.Pepper] = 2;

        foreach (var item in required)
            collected[item.Key] = 0;
    }

    public void AddTopping(ToppingType type)
    {
        if (!gameActive) return;

        if (required.ContainsKey(type))
        {
            collected[type]++;
            UIManager ui = FindFirstObjectByType<UIManager>();
            if (ui != null)
            {
                ui.PlayCorrectSound();
            }

            CheckWin();

        }
        else
        {
            HandleWrong(type);
        }
    }

    void HandleWrong(ToppingType type)
    {
        if (type == ToppingType.Pineapple)
        {
            timeLeft -= 3f;

            PlayerCatcher player = FindFirstObjectByType<PlayerCatcher>();
            if (player != null)
            {
                player.InvertControls(2.5f);
            }

            UIManager ui = FindFirstObjectByType<UIManager>();
            if (ui != null)
            {
                ui.TriggerPineappleEffect();
            }
        }
        if (type == ToppingType.Burnt)
        {
            timeLeft -= 2f;

            UIManager ui = FindFirstObjectByType<UIManager>();
            if (ui != null)
            {
                ui.TriggerBombEffect();
            }
        }

        if (timeLeft < 0) timeLeft = 0;
    }

    void CheckWin()
    {
        foreach (var item in required)
        {
            if (collected[item.Key] < item.Value)
                return;
        }

        EndGame(true);
    }

    void EndGame(bool win)
    {
        gameActive = false;

        Debug.Log(win ? "WIN" : "LOSE");

        if (win)
        {
            GameProgressManager.Instance.CompleteCollect();
        }
        

        SaveResults();

        // Show UI instead of leaving scene
        UIManager ui = FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            ui.ShowEndScreen(win);
        }
    }

    void SaveResults()
    {
        foreach (var item in collected)
        {
            PlayerPrefs.SetInt(item.Key.ToString(), item.Value);
        }
        PlayerPrefs.Save();
    }

    void ReturnToMainWorld()
    {
        SceneManager.LoadScene("ToppingStartingHub");
    }

    public int GetCollected(ToppingType type)
    {
        return collected.ContainsKey(type) ? collected[type] : 0;
    }

    public int GetRequired(ToppingType type)
    {
        return required.ContainsKey(type) ? required[type] : 0;
    }
}