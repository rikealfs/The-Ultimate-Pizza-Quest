using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonLoader : MonoBehaviour
{
    public void LoadCollect()
    {
        Debug.Log("Loading Collect Scene");
        SceneManager.LoadScene("ToppingMinigame");
    }

    public void LoadSorting()
    {
        SceneManager.LoadScene("ToppingMinigame2");
    }
}