using UnityEngine;

public class ShapeController : MonoBehaviour
{
    [Header("Fall Speeds")]
    public float normalFallDelay = 1f;
    public float fastFallDelay = 0.05f;

    private float currentFallDelay;
    private float fallTimer = 0f;
    private bool hasLanded = false;

    private void Start()
    {
        currentFallDelay = normalFallDelay;
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
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Vector3.left);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            Move(Vector3.right);

        // if (Input.GetKeyDown(KeyCode.DownArrow))
        //     Move(Vector3.down); 

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

            if (direction == Vector3.down)
            {
                if (!hasLanded)
                {
                    hasLanded = true;

                    SnapToGrid(); // Fix micro spacing
                    GridManager.AddToGrid(transform);
                    GridManager.CheckForLines();
                    FindObjectOfType<ShapeSpawnerController>().SpawnNext();
                    enabled = false;
                }
            }
        }
    }

    private void Rotate(float angle)
    {
        transform.Rotate(0, 0, angle);

        if (!IsValidPosition())
            transform.Rotate(0, 0, -angle);
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
}
