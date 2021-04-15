using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardImage : MonoBehaviour
{
    [SerializeField] protected SpriteManager _spriteManager;

    [Header("Scene Objects")]
    [SerializeField] protected Transform _boardTransform;
    [SerializeField] protected Transform _fieldTransform;

    [Header("Object Prefabs")]
    [SerializeField] protected GameObject _tile;
    [SerializeField] protected GameObject _piece;

    [Header("Highlight Prefabs")]
    [SerializeField] protected GameObject _lastMoveHighlightPrefab;
    [SerializeField] protected GameObject _checkHighlightPrefab;
    [SerializeField] protected GameObject _stalemateHighlightPrefab;
    [SerializeField] protected GameObject _checkmateHighlightPrefab;

    protected SpritePack _spritePack;

    protected (GameObject, GameObject) _lastMoveHighlights;
    protected GameObject _checkHighlight;
    protected GameObject _stalemateHighlight;
    protected GameObject _checkmateHighlight;

    protected Position _position;

    protected Transform[,] _tiles = new Transform[Position.SIZE, Position.SIZE];
    protected Transform[,] _pieces = new Transform[Position.SIZE, Position.SIZE];

    public virtual void InitBoard()
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

                _tiles[x, y] = Instantiate(_tile, _fieldTransform).transform;
            }
        }
    }

    public void InitPieces(Position position)
    {
        _position = position;

        Debug.Log("Y " + _position.Board[4, 4]);
        for (int x = 0; x < Position.SIZE; x++)
        {
            for (int y = 0; y < Position.SIZE; y++)
            {
                if (_position.Board[x, y] != null)
                {
                    _piece.GetComponent<Image>().sprite = _spritePack.GetPiece(
                            _position.Board[x, y].Color,
                            _position.Board[x, y].GetType());
                    _pieces[x, y] = Instantiate(_piece, _tiles[x, y]).transform;
                }
            }
        }
        CheckPosition();
    }

    protected void CheckPosition()
    {
        Vector2Int kingSquare = _position.GetKingSquare(_position.WhoseMove);
        if (_position.IsStalemate())
        {
            _stalemateHighlight = Instantiate(_stalemateHighlightPrefab, _tiles[kingSquare.x, kingSquare.y]);
            _stalemateHighlight.transform.SetAsFirstSibling();
        }
        else if (_position.IsCheckmate())
        {
            _checkmateHighlight = Instantiate(_checkmateHighlightPrefab, _tiles[kingSquare.x, kingSquare.y]);
            _checkmateHighlight.transform.SetAsFirstSibling();
        }
        else if (_position.IsInCheck())
        {
            _checkHighlight = Instantiate(_checkHighlightPrefab, _tiles[kingSquare.x, kingSquare.y]);
            _checkHighlight.transform.SetAsFirstSibling();
        }
    }


    public virtual void SetRotation(int angle)
    {
        _boardTransform.localRotation = Quaternion.Euler(0, 0, angle);
        for (int x = 0; x < Position.SIZE; x++)
        {
            for (int y = 0; y < Position.SIZE; y++)
            {
                _tiles[x, y].localRotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}
