using UnityEngine;

public class Piece : MonoBehaviour
{
    private static Color _selectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private static Piece _previousSelected = null;

    private SpriteRenderer _spriteRenderer;
    private bool isSelected = false;

    private Vector2[] _adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.right, Vector2.left };

    public int iD;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
