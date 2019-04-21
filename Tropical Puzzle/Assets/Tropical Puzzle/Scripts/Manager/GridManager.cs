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
        Vector2 offset = currentPiece.GetComponent<BoxCollider2D>().size;
        CreateInitialGrid(offset);
    }

    private void CreateInitialGrid(Vector2 offset)
    {
        _pieces = new GameObject[xSize, ySize];

        float startX = this.transform.position.x;
        float startY = this.transform.position.y;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newPiece = Instantiate(currentPiece, new Vector2(startX + (offset.x * x), startY + (offset.y * y)), currentPiece.transform.rotation);
                newPiece.name = $"Piece[{x}][{y}]";
                int idx;
                do
                {
                    idx = Random.Range(0, _prefabs.Count);
                } while ((x > 0 && idx == _pieces[x - 1, y].GetComponent<Piece>().iD) || (y > 0 && idx == _pieces[x, y - 1].GetComponent<Piece>().iD));

                Sprite sprite = _prefabs[idx];
                newPiece.GetComponent<SpriteRenderer>().sprite = sprite;
                newPiece.GetComponent<Piece>().iD = idx;
                newPiece.transform.parent = this.transform;
                _pieces[x, y] = newPiece;
            }
        }
    }

    public IEnumerator FindNullPieces()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (_pieces[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(MakePiecesFall(x, y));
                    break;
                }
            }
        }
    }

    private IEnumerator MakePiecesFall(int x, int yStart, float shiftDelay = 0.05f)
    {
        IsShifting = true;

        List<SpriteRenderer> renderers = new List<SpriteRenderer>();

        int nullPieces = 0;

        for (int y = yStart; y < ySize; y++)
        {
            SpriteRenderer spriteRenderer = _pieces[x, y].GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == null)
            {
                nullPieces++;
            }
            renderers.Add(spriteRenderer);
        }

        for (int i = 0; i < nullPieces; i++)
        {
            yield return new WaitForSeconds(shiftDelay);
            for (int j = 0; j < renderers.Count - 1; j++)
            {
                renderers[j].sprite = renderers[j + 1].sprite;
                renderers[j + 1].sprite = null;
            }
        }

        IsShifting = false;
    }
}
