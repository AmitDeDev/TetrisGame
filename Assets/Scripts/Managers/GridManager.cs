using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static int GridWidth = 30;
    public static int GridHeight = 80;

    public static Transform[,] grid = new Transform[GridWidth, GridHeight];
    
    [Header("Row Color Mode")]
    public bool rowSpriteInheritance = true;
    
    public static GridManager Instance;
    
    private static Dictionary<int, Sprite> rowSprites = new Dictionary<int, Sprite>();

    private void Awake()
    {
        Instance = this;
    }

    public static Vector2 RoundToGrid(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public static bool InsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < GridWidth && (int)pos.y >= 0);
    }

    public static void AddToGrid(Transform shape)
    {
        foreach (Transform block in shape)
        {
            Vector2 pos = RoundToGrid(block.position);
            int x = (int)pos.x;
            int y = (int)pos.y;

            if (y < GridHeight)
            {
                grid[x, y] = block;
            }
        }
        
        if (Instance != null && Instance.rowSpriteInheritance)
        {
            Instance.ApplyRowSpriteInheritance(shape);
        }
    }

    public static void CheckForLines()
    {
        for (int y = 0; y < GridHeight; ++y)
        {
            if (IsLineFull(y))
            {
                DeleteLine(y);
                MoveRowsDown(y);
                y--;
                ScoreView.Instance?.AddScore(100);
            }
        }
    }

    private static bool IsLineFull(int y)
    {
        for (int x = 0; x < GridWidth; ++x)
        {
            if (grid[x, y] == null)
                return false;
        }
        return true;
    }

    private static void DeleteLine(int y)
    {
        for (int x = 0; x < GridWidth; ++x)
        {
            GameObject.Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }

        if (rowSprites.ContainsKey(y))
        {
            rowSprites.Remove(y);
        }
    }

    private static void MoveRowsDown(int fromY)
    {
        for (int y = fromY + 1; y < GridHeight; ++y)
        {
            for (int x = 0; x < GridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += Vector3.down;
                }
            }
            
            if (Instance != null && Instance.rowSpriteInheritance)
            {
                if (rowSprites.ContainsKey(y))
                {
                    rowSprites[y - 1] = rowSprites[y];
                    rowSprites.Remove(y);
                }
            }
        }
    }
    
    private void ApplyRowSpriteInheritance(Transform shape)
    {
        foreach (Transform block in shape)
        {
            Vector2 pos = RoundToGrid(block.position);
            int y = (int)pos.y;

            if (y >= GridHeight) continue;

            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            if (rowSprites.ContainsKey(y))
            {
                // Row is already claimed
                sr.sprite = rowSprites[y];
            }
            else
            {
                // First block to claim this row
                rowSprites[y] = sr.sprite;
            }
        }
    }
}
