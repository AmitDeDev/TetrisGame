using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public static int GridWidth = 30;
    public static int GridHeight = 80;

    public static Transform[,] grid = new Transform[GridWidth, GridHeight];

    [Header("Row Color Mode")]
    public bool rowSpriteInheritance = true;

    [Header("Line Clear Effects")]
    public List<GameObject> colorParticles = new List<GameObject>(); 
    public float rowEffectYOffset = 0.5f;
    public List<Sprite> referenceSprites = new List<Sprite>(); 

    public static GridManager Instance;

    private static Dictionary<int, Sprite> rowOwnerSprites = new Dictionary<int, Sprite>();

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
                ScoreView.Instance?.AddScore(100);
                TriggerRowClearEffect(y);
                DeleteLine(y);
                Camera.main.transform.DOShakePosition(0.5f, 0.5f, 20, 100, false);
                MoveRowsDown(y);
                y--;
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

        if (rowOwnerSprites.ContainsKey(y))
        {
            rowOwnerSprites.Remove(y);
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
                if (rowOwnerSprites.ContainsKey(y))
                {
                    rowOwnerSprites[y - 1] = rowOwnerSprites[y];
                    rowOwnerSprites.Remove(y);
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

            if (rowOwnerSprites.ContainsKey(y))
            {
                sr.sprite = rowOwnerSprites[y];
            }
            else
            {
                rowOwnerSprites[y] = sr.sprite;
            }
        }
    }

    private static void TriggerRowClearEffect(int y)
    {
        Vector3 position = new Vector3(GridWidth / 2f, y + Instance.rowEffectYOffset, 0);

        if (!Instance.rowSpriteInheritance)
        {
            foreach (var particlePrefab in Instance.colorParticles)
            {
                if (particlePrefab == null) continue;

                GameObject fx = Instantiate(particlePrefab, position, Quaternion.identity);
                ParticleSystem ps = fx.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                    Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
                }
                else
                {
                    Destroy(fx, 2f);
                }
            }
        }
        else
        {
            if (!rowOwnerSprites.ContainsKey(y)) return;

            Sprite targetSprite = rowOwnerSprites[y];
            int index = Instance.referenceSprites.IndexOf(targetSprite);

            if (index >= 0 && index < Instance.colorParticles.Count)
            {
                GameObject particlePrefab = Instance.colorParticles[index];
                if (particlePrefab != null)
                {
                    GameObject fx = Instantiate(particlePrefab, position, Quaternion.identity);
                    ParticleSystem ps = fx.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.Play();
                        Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
                    }
                    else
                    {
                        Destroy(fx, 2f);
                    }
                }
            }
        }
    }
    
    public static void SetRowSprite(int y, Sprite sprite)
    {
        if (!rowOwnerSprites.ContainsKey(y))
            rowOwnerSprites[y] = sprite;
    }
}
