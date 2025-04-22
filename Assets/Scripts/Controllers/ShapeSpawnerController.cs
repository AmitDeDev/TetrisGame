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
    
    [Header("Startup Lines")]
    public bool generateStartingLines = false;
    public int numberOfLinesToGenerate = 5;
    public int holesPerLine = 2;
    
    [Header("Ghost Shape")]
    public Sprite ghostBlockSprite;

    private void Start()
    {
        if (generateStartingLines)
        {
            GenerateStartingLines();
        }
        
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
    
    private void GenerateStartingLines()
    {
        for (int y = 0; y < numberOfLinesToGenerate; y++)
        {
            // generate holes per line
            HashSet<int> holes = new HashSet<int>();
            while (holes.Count < holesPerLine)
            {
                holes.Add(Random.Range(0, GridManager.GridWidth));
            }

            // pick a sprite to use for this row
            Sprite rowSprite = null;
            if (blockSprites.Count > 0)
            {
                rowSprite = blockSprites[Random.Range(0, blockSprites.Count)];
            }

            for (int x = 0; x < GridManager.GridWidth; x++)
            {
                if (holes.Contains(x)) continue;

                // create a single block
                GameObject block = new GameObject($"StartupBlock_X{x}_Y{y}");
                block.transform.position = new Vector2(x, y);
                block.transform.parent = GridManager.Instance.transform;

                SpriteRenderer sr = block.AddComponent<SpriteRenderer>();
                sr.sprite = rowSprite;

                GridManager.grid[x, y] = block.transform;
            }

            // sprite for row sprite inheritance
            if (GridManager.Instance != null && GridManager.Instance.rowSpriteInheritance && rowSprite != null)
            {
                GridManager.SetRowSprite(y, rowSprite);
            }

        }
    }
}