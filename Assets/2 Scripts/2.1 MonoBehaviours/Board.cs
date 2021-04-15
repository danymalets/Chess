using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private SpriteManager _spriteManager;

    [Header("Scene Objects")]
    [SerializeField] private Transform _boardTransform;
    [SerializeField] private Transform _fieldTransform;

    [Header("Object Prefabs")]
    [SerializeField] private GameObject _tile;
    [SerializeField] private GameObject _piece;

    [Header("Highlight Prefabs")]
    [SerializeField] private GameObject _lastMoveHighlightPrefab;
    [SerializeField] private GameObject _checkHighlightPrefab;
    [SerializeField] private GameObject _stalemateHighlightPrefab;
    [SerializeField] private GameObject _checkmateHighlightPrefab;

    private SpritePack _spritePack;

    private (GameObject, GameObject) _lastMoveHighlights;
    private GameObject _checkHighlight;
    private GameObject _stalemateHighlight;
    private GameObject _checkmateHighlight;

    public Position Position;

    public Transform[,] Tiles = new Transform[Position.SIZE, Position.SIZE];
    public Transform[,] Pieces = new Transform[Position.SIZE, Position.SIZE];

    public void InitBoard()
    {
        _spritePack = _spriteManager.CurrentPack;

        GetComponent<Image>().sprite = _spritePack.GetBoard();

        Sprite white = _spritePack.GetTile(Color.White);
        Sprite black = _spritePack.GetTile(Color.Black);

        for (int x = 0; x < Position.SIZE; x++)
        {
            for (int y = 0; y < Position.SIZE; y++)
            {

                _tile.GetComponent<Image>().sprite = x % 2 == y % 2 ? white : black;

                Tiles[x, y] = Instantiate(_tile, _fieldTransform).transform;
            }
        }
    }

    public void InitPieces(Position position)
    {
        Position = position;

        Debug.Log("Y " + Position.Board[4, 4]);
        for (int x = 0; x < Position.SIZE; x++)
        {
            for (int y = 0; y < Position.SIZE; y++)
            {
                if (Position.Board[x, y] != null)
                {
                    _piece.GetComponent<Image>().sprite = _spritePack.GetPiece(
                            Position.Board[x, y].Color,
                            Position.Board[x, y].GetType());
                    Pieces[x, y] = Instantiate(_piece, Tiles[x, y]).transform;
                }
            }
        }
        CheckPosition();
    }

    public void CheckPosition()
    {
        Vector2Int kingSquare = Position.GetKingSquare(Position.WhoseMove);
        if (Position.IsStalemate())
        {
            _stalemateHighlight = Instantiate(_stalemateHighlightPrefab, Tiles[kingSquare.x, kingSquare.y]);
            _stalemateHighlight.transform.SetAsFirstSibling();
        }
        else if (Position.IsCheckmate())
        {
            _checkmateHighlight = Instantiate(_checkmateHighlightPrefab, Tiles[kingSquare.x, kingSquare.y]);
            _checkmateHighlight.transform.SetAsFirstSibling();
        }
        else if (Position.IsInCheck())
        {
            _checkHighlight = Instantiate(_checkHighlightPrefab, Tiles[kingSquare.x, kingSquare.y]);
            _checkHighlight.transform.SetAsFirstSibling();
        }
    }

    public void RemoveCheckHighlight()
    {
        if (_checkHighlight != null) Destroy(_checkHighlight);
    }

    public void SetRotation(int angle)
    {
        _boardTransform.localRotation = Quaternion.Euler(0, 0, angle);
        for (int x = 0; x < Position.SIZE; x++)
        {
            for (int y = 0; y < Position.SIZE; y++)
            {
                Tiles[x, y].localRotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    public void Clear()
    {
        if (_stalemateHighlight != null) Destroy(_stalemateHighlight);
        else if (_checkmateHighlight != null) Destroy(_checkmateHighlight);
        else if (_checkHighlight != null) Destroy(_checkHighlight);
    }
}
