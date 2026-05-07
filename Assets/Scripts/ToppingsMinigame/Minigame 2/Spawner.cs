using System.Collections.Generic;
using UnityEngine;

public class ConveyorSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] itemPrefabs;

    [Header("Spawn Position")]
    public float spawnX = 9f;
    public float conveyorY = 1.5f;
    public float itemYOffset = 0.1f;

    [Header("Spacing")]
    public float spacing = 2.5f;
    public float spawnInterval = 1.2f;

    [Header("Speed Control")]
    public float speed = 2f;
    public float maxSpeed = 6f;
    public float acceleration = 0.05f;

    [Header("Optional Lanes (leave empty for single lane)")]
    public float[] lanesY;

    private List<GameObject> activeItems = new List<GameObject>();

    private float spawnTimer;

    void Update()
    {
        // Stop everything if game ended
        if (SortingGameManager.Instance != null && !SortingGameManager.Instance.gameActive)
        {
            return;
        }

        // Speed ramp
        speed += Time.deltaTime * acceleration;
        speed = Mathf.Clamp(speed, 2f, maxSpeed);

        // Spawn timer
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        CleanList();

        // Prevent overlap
        GameObject last = GetRightmostItem();
        if (last != null && last.transform.position.x > spawnX - spacing)
        {
            return;
        }

        int index = Random.Range(0, itemPrefabs.Length);

        float y = GetSpawnY();

        Vector3 pos = new Vector3(spawnX, y, 0);

        GameObject item = Instantiate(itemPrefabs[index], pos, Quaternion.identity);

        item.tag = "Ingredient";

        
        ConveyorMove move = item.GetComponent<ConveyorMove>();
        if (move != null)
        {
            move.speed = speed + Random.Range(-0.3f, 0.3f); 
        }

        activeItems.Add(item);
    }

    float GetSpawnY()
    {
        // Multi-lane option
        if (lanesY != null && lanesY.Length > 0)
        {
            return lanesY[Random.Range(0, lanesY.Length)];
        }

        // Single clean conveyor
        return conveyorY + itemYOffset;
    }

    GameObject GetRightmostItem()
    {
        GameObject rightmost = null;
        float maxX = float.MinValue;

        foreach (GameObject item in activeItems)
        {
            if (item == null) continue;

            if (item.transform.position.x > maxX)
            {
                maxX = item.transform.position.x;
                rightmost = item;
            }
        }

        return rightmost;
    }

    void CleanList()
    {
        activeItems.RemoveAll(item => item == null);
    }
}