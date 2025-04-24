using System.Collections;
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

    [Header("Rising Lines Mode")]
    public bool enableRisingLines = false;
    public float risingIntervalSeconds = 10f;
    public int risingHolesPerLine = 2;

    private int nextShapeIndex = -1;

    private void Start()
    {
        if (generateStartingLines)
            GenerateStartingLines();

        if (enableRisingLines)
            StartCoroutine(RisingRoutine());

        SpawnNext();
    }

    public void SpawnNext()
    {
        if (GameOverManager.Instance.IsGameOver)
            return;
        
        if(CheckIfSpawnPositionBlocked())
            return;
        
        int shapeIndex = Random.Range(0, shapePrefabs.Length);
        GameObject newShape = Instantiate(shapePrefabs[shapeIndex], spawnPosition, Quaternion.identity);
        ApplyRandomBlockSprite(newShape);
    }

    private bool CheckIfSpawnPositionBlocked()  
    {
        Vector2Int spawnCell = Vector2Int.RoundToInt(spawnPosition);
        if (GridManager.grid[spawnCell.x, spawnCell.y] != null)
        {
            GameOverManager.Instance.TriggerGameOver(ScoreView.Instance.CurrentScore);
            return true;
        }
        return false;
    }

    private void ApplyRandomBlockSprite(GameObject shape)
    {
        if (blockSprites.Count == 0) return;
        
        Sprite randomSprite = blockSprites[Random.Range(0, blockSprites.Count)];
        foreach (Transform block in shape.transform)
        {
            var sr = block.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = randomSprite;
            }
        }

    }

    private void GenerateStartingLines()
    {
        for (int y = 0; y < numberOfLinesToGenerate; y++)
        {
            var holes = GenerateHolesPerLine(holesPerLine);
            Sprite rowSprite = PickRandomRowSprite();

            for (int x = 0; x < GridManager.GridWidth; x++)
            {
                if (holes.Contains(x)) continue;
                CreateBlockAt(x, y, rowSprite);
            }

            if (GridManager.Instance != null
                && GridManager.Instance.rowSpriteInheritance
                && rowSprite != null)
            {
                GridManager.SetRowSprite(y, rowSprite);
            }
        }
    }

    private IEnumerator RisingRoutine()
    {
        while (enableRisingLines)
        {
            yield return new WaitForSeconds(risingIntervalSeconds);
            GenerateRisingLine();
        }
    }

    private void GenerateRisingLine()
    {
        // shift everything up
        GridManager.ShiftRowsUp();

        // pick holes & color
        var holes = GenerateHolesPerLine(risingHolesPerLine);
        Sprite rowSprite = PickRandomRowSprite();

        // bottom row is y=0
        for (int x = 0; x < GridManager.GridWidth; x++)
        {
            if (holes.Contains(x)) continue;
            CreateBlockAt(x, 0, rowSprite);
        }

        if (GridManager.Instance != null
            && GridManager.Instance.rowSpriteInheritance
            && rowSprite != null)
        {
            GridManager.SetRowSprite(0, rowSprite);
        }
    }

    private HashSet<int> GenerateHolesPerLine(int holesAmount)
    {
        var holes = new HashSet<int>();
        while (holes.Count < holesAmount)
        {
            holes.Add(Random.Range(0, GridManager.GridWidth));
        }
            
        return holes;
    }

    private Sprite PickRandomRowSprite()
    {
        if (blockSprites.Count == 0)
        {
            return null;
        } 
        
        return blockSprites[Random.Range(0, blockSprites.Count)];
    }

    private void CreateBlockAt(int x, int y, Sprite sprite)
    {
        var block = new GameObject($"StartupBlock_X{x}_Y{y}");
        block.transform.position = new Vector2(x, y);
        block.transform.parent = GridManager.Instance.transform;

        var sr = block.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;

        GridManager.grid[x, y] = block.transform;
    }
}
