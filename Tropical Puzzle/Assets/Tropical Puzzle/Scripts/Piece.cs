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

                    UIManager.Instance.MoveCounter--;
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

    public void SwapSprite(Piece newPiece)
    {
        if (_spriteRenderer.sprite == newPiece.GetComponent<SpriteRenderer>().sprite)
        {
            return;
        }

        Sprite oldPiece = newPiece._spriteRenderer.sprite;
        newPiece._spriteRenderer.sprite = this._spriteRenderer.sprite;
        this._spriteRenderer.sprite = oldPiece;

        int tmpID = newPiece.iD;
        newPiece.iD = this.iD;
        this.iD = tmpID;
    }

    private GameObject GetNeighbor(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    private List<GameObject> GetAllNeighbors()
    {
        List<GameObject> neighbors = new List<GameObject>();

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
        List<GameObject> matchingPieces = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);

        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == _spriteRenderer.sprite)
        {
            matchingPieces.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, direction);
        }

        return matchingPieces;
    }

    private bool ClearMatch(Vector2[] directions)
    {
        List<GameObject> matchingPieces = new List<GameObject>();

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

        bool hMatch = ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        bool vMatch = ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });

        if (hMatch || vMatch)
        {
            _spriteRenderer.sprite = null;
            StopCoroutine(GridManager.Instance.FindNullPieces());
            StartCoroutine(GridManager.Instance.FindNullPieces());
        }
    }
}
