using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static int GridWidth = 30;
    public static int GridHeight = 80;

    public static Transform[,] grid = new Transform[GridWidth, GridHeight];

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
            if ((int)pos.y < GridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = block;
            }
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
        }
    }
}
