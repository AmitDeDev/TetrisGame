using System.Collections.Generic;
using UnityEngine;

public class ShapeSpawnerController : MonoBehaviour
{
    [Header("Shape Prefabs")]
    public GameObject[] shapePrefabs;

    [Header("Spawn Settings")]
    public Vector2 spawnPosition = new Vector2(5, 18);

    [Header("Block Sprites")]
    public List<Sprite> blockSprites;
    
    [Header("Ghost Shape")]
    public Sprite ghostBlockSprite;


    private void Start()
    {
        SpawnNext();
    }

    public void SpawnNext()
    {
        int shapeIndex = Random.Range(0, shapePrefabs.Length);
        GameObject newShape = Instantiate(shapePrefabs[shapeIndex], spawnPosition, Quaternion.identity);

       ApplyRandomBlockSprite(newShape);
    }

    private void ApplyRandomBlockSprite(GameObject shape)
    {
        if (blockSprites.Count == 0) return;

        Sprite randomSprite = blockSprites[Random.Range(0, blockSprites.Count)];
        
        foreach (Transform block in shape.transform)
        {
            var sr = block.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = randomSprite;
        }
    }
}