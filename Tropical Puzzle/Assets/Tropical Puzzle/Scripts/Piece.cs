using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private static Color _selectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private static Piece _previousSelected;

    private SpriteRenderer _spriteRenderer;
    private bool _isSelected;

    private readonly Vector2[] _adjacentDirections = { Vector2.up, Vector2.down, Vector2.right, Vector2.left };

    public int iD;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void SelectPiece()
    {
        _isSelected = true;
        _spriteRenderer.color = _selectedColor;
        _previousSelected = gameObject.GetComponent<Piece>();
    }

    private void DeselectPiece()
    {
        _isSelected = false;
        _spriteRenderer.color = Color.white;
        _previousSelected = null;
    }

    private void OnMouseDown()
    {
        if (UIManager.Instance.TimerCounter > 0)
        {
            if (_spriteRenderer == null || GridManager.Instance.IsShifting)
            {
                return;
            }

            if (!_isSelected)
            {
                if (_previousSelected == null)
                {
                    SelectPiece();
                }
                else
                {
                    if (CanSwipe())
                    {
                        SwapSprite(_previousSelected);
                        _previousSelected.FindAllMatches();
                        _previousSelected.DeselectPiece();
                        FindAllMatches();
                    }
                    else
                    {
                        _previousSelected.DeselectPiece();
                        SelectPiece();
                    }
                }
            }
            else
            {
                DeselectPiece();
            }
        }
    }

    private void SwapSprite(Piece newPiece)
    {
        if (_spriteRenderer.sprite == newPiece.GetComponent<SpriteRenderer>().sprite)
        {
            return;
        }

        var oldPiece = newPiece._spriteRenderer.sprite;
        newPiece._spriteRenderer.sprite = this._spriteRenderer.sprite;
        this._spriteRenderer.sprite = oldPiece;

        var tmpId = newPiece.iD;
        newPiece.iD = this.iD;
        this.iD = tmpId;
    }

    private GameObject GetNeighbor(Vector2 direction)
    {
        var hit = Physics2D.Raycast(this.transform.position, direction);

        return hit.collider != null ? hit.collider.gameObject : null;
    }

    private List<GameObject> GetAllNeighbors()
    {
        var neighbors = new List<GameObject>();

        foreach (Vector2 direction in _adjacentDirections)
        {
            neighbors.Add(GetNeighbor(direction));
        }

        return neighbors;
    }

    private bool CanSwipe()
    {
        return GetAllNeighbors().Contains(_previousSelected.gameObject);
    }

    private List<GameObject> FindMatch(Vector2 direction)
    {
        var matchingPieces = new List<GameObject>();
        var hit = Physics2D.Raycast(this.transform.position, direction);

        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == _spriteRenderer.sprite)
        {
            matchingPieces.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, direction);
        }

        return matchingPieces;
    }

    private bool ClearMatch(Vector2[] directions)
    {
        var matchingPieces = new List<GameObject>();

        foreach (var direction in directions)
        {
            matchingPieces.AddRange(FindMatch(direction));
        }

        if (matchingPieces.Count >= GridManager.MinToMatch)
        {
            foreach (var piece in matchingPieces)
            {
                piece.GetComponent<SpriteRenderer>().sprite = null;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void FindAllMatches()
    {
        if (_spriteRenderer.sprite == null)
        {
            return;
        }

        var hMatch = ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        var vMatch = ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });

        if (!hMatch && !vMatch) return;
        _spriteRenderer.sprite = null;
        StopCoroutine(GridManager.Instance.FindNullPieces());
        StartCoroutine(GridManager.Instance.FindNullPieces());
    }
}
