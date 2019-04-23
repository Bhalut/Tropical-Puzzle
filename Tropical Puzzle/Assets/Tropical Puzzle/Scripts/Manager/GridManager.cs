using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager _instance;
    public static GridManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private GameObject[,] _pieces;

    public List<Sprite> _prefabs = new List<Sprite>();
    public GameObject currentPiece;
    public int xSize, ySize;
    public bool IsShifting { get; set; }

    public const int MinToMatch = 2;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        var offset = currentPiece.GetComponent<BoxCollider2D>().size;
        CreateInitialGrid(offset);
    }

    private void CreateInitialGrid(Vector2 offset)
    {
        _pieces = new GameObject[xSize, ySize];

        var startX = this.transform.position.x;
        var startY = this.transform.position.y;
        for (var x = 0; x < xSize; x++)
        {
            for (var y = 0; y < ySize; y++)
            {
                var newPiece = Instantiate(currentPiece, new Vector2(startX + (offset.x * x), startY + (offset.y * y)), currentPiece.transform.rotation);
                newPiece.name = $"Piece[{x}][{y}]";
                var idx = -1;
                do
                {
                    idx = Random.Range(0, _prefabs.Count);
                } while ((x > 0 && idx == _pieces[x - 1, y].GetComponent<Piece>().iD) || (y > 0 && idx == _pieces[x, y - 1].GetComponent<Piece>().iD));

                var sprite = _prefabs[idx];
                newPiece.GetComponent<SpriteRenderer>().sprite = sprite;
                newPiece.GetComponent<Piece>().iD = idx;
                newPiece.transform.parent = this.transform;
                _pieces[x, y] = newPiece;
            }
        }
    }

    public IEnumerator FindNullPieces()
    {
        for (var x = 0; x < xSize; x++)
        {
            for (var y = 0; y < ySize; y++)
            {
                if (_pieces[x, y].GetComponent<SpriteRenderer>().sprite != null) continue;
                yield return StartCoroutine(MakePiecesFall(x, y));
                break;
            }
        }

        for (var x = 0; x < xSize; x++)
        {
            for (var y = 0; y < ySize; y++)
            {
                _pieces[x, y].GetComponent<Piece>().FindAllMatches();
            }
        }
    }

    private IEnumerator MakePiecesFall(int x, int yStart, float shiftDelay = 0.05f)
    {
        IsShifting = true;

        var renderers = new List<SpriteRenderer>();

        var nullPieces = 0;

        for (var y = yStart; y < ySize; y++)
        {
            var spriteRenderer = _pieces[x, y].GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == null)
            {
                nullPieces++;
            }
            renderers.Add(spriteRenderer);
        }

        for (var i = 0; i < nullPieces; i++)
        {
            UIManager.Instance.Score += 10;

            yield return new WaitForSeconds(shiftDelay);
            for (var j = 0; j < renderers.Count - 1; j++)
            {
                renderers[j].sprite = renderers[j + 1].sprite;
                renderers[j + 1].sprite = GetNewPiece(x, ySize - 1);
            }
        }

        IsShifting = false;
    }

    private Sprite GetNewPiece(int x, int y)
    {
        var possiblePieces = new List<Sprite>();
        possiblePieces.AddRange(_prefabs);

        if (x > 0)
        {
            possiblePieces.Remove(_pieces[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        if (x < xSize - 1)
        {
            possiblePieces.Remove(_pieces[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        if (y > 0)
        {
            possiblePieces.Remove(_pieces[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possiblePieces[Random.Range(0, possiblePieces.Count)];
    }
}
