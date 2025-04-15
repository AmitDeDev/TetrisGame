using UnityEngine;

public class ShapeSpawnerController : MonoBehaviour
{
    [Header("Shape Prefabs")]
    public GameObject[] shapePrefabs;

    [Header("Spawn Settings")]
    public Vector2 spawnPosition = new Vector2(5, 18); 

    private void Start()
    {
        SpawnNext();
    }

    public void SpawnNext()
    {
        int index = Random.Range(0, shapePrefabs.Length);
        Instantiate(shapePrefabs[index], spawnPosition, Quaternion.identity);
    }
}