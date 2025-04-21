using UnityEngine;

public class ShapeController : MonoBehaviour
{
    [Header("Fall Speeds")]
    public float normalFallDelay = 1f;
    public float fastFallDelay = 0.05f;

    private float currentFallDelay;
    private float fallTimer = 0f;
    private bool hasLanded = false;

    private GameObject ghostShape;

    private void Start()
    {
        currentFallDelay = normalFallDelay;
        CreateGhostShape();
    }

    private void Update()
    {
        HandleInput();

        fallTimer += Time.deltaTime;
        if (fallTimer >= currentFallDelay)
        {
            Move(Vector3.down);
            fallTimer = 0f;
        }

        UpdateGhostShape();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Vector3.left);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            Move(Vector3.right);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Rotate(-90);

        if (Input.GetKey(KeyCode.Space))
            currentFallDelay = fastFallDelay;
        else
            currentFallDelay = normalFallDelay;
    }

    private void Move(Vector3 direction)
    {
        transform.position += direction;

        if (!IsValidPosition())
        {
            transform.position -= direction;

            if (direction == Vector3.down && !hasLanded)
            {
                hasLanded = true;

                SnapToGrid();
                GridManager.AddToGrid(transform);
                GridManager.CheckForLines();
                FindObjectOfType<ShapeSpawnerController>().SpawnNext();
                Destroy(ghostShape);
                enabled = false;
            }
        }
    }

    private void Rotate(float angle)
    {
        transform.Rotate(0, 0, angle);

        if (!IsValidPosition())
            transform.Rotate(0, 0, -angle);
        else
            UpdateGhostShape();
    }

    private bool IsValidPosition()
    {
        foreach (Transform block in transform)
        {
            Vector2 pos = GridManager.RoundToGrid(block.position);
            if (!GridManager.InsideGrid(pos))
                return false;
            if (GridManager.grid[(int)pos.x, (int)pos.y] != null)
                return false;
        }
        return true;
    }

    private void SnapToGrid()
    {
        foreach (Transform block in transform)
        {
            block.position = GridManager.RoundToGrid(block.position);
        }
    }

    private void CreateGhostShape()
    {
        ghostShape = Instantiate(gameObject, transform.position, transform.rotation);
        Destroy(ghostShape.GetComponent<ShapeController>()); 

        foreach (Transform block in ghostShape.transform)
        {
            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = FindObjectOfType<ShapeSpawnerController>().ghostBlockSprite;
            }
        }
    }

    private void UpdateGhostShape()
    {
        if (ghostShape == null) return;

        ghostShape.transform.position = transform.position;
        ghostShape.transform.rotation = transform.rotation;
        
        while (IsValidGhostPosition(ghostShape.transform))
        {
            ghostShape.transform.position += Vector3.down;
        }
        
        ghostShape.transform.position += Vector3.up;
    }

    private bool IsValidGhostPosition(Transform ghost)
    {
        foreach (Transform block in ghost)
        {
            Vector2 pos = GridManager.RoundToGrid(block.position);
            if (!GridManager.InsideGrid(pos)) return false;
            if (GridManager.grid[(int)pos.x, (int)pos.y] != null) return false;
        }
        return true;
    }
}
