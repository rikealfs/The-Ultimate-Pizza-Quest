using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRestart : MonoBehaviour
{
    // restarts when clicked
    public void restart()
    {
        // reload scene
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
}
