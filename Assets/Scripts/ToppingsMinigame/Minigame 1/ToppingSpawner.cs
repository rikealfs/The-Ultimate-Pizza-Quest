using UnityEngine;

public class ToppingSpawner : MonoBehaviour
{
    public GameObject[] toppingPrefabs;
    public float spawnRate = 1f;
    public float xRange = 8f;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, spawnRate);
    }

    void Spawn()
    {
        if (!CollectGameManager.Instance.gameActive) return;

        float x = Random.Range(-xRange, xRange);
        Vector3 pos = new Vector3(x, 6f, 0);

        int index = Random.Range(0, toppingPrefabs.Length);
        Instantiate(toppingPrefabs[index], pos, Quaternion.identity);
    }
}