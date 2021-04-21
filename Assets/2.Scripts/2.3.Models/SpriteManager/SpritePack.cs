using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpriteManager/Packs")]
public class SpritePack : ScriptableObject
{
    [Header("Name")]
    public string Name;

    [Header("Board")]
    [SerializeField] private Sprite _board;

    [Header("Tiles")]
    [SerializeField] private Sprite _whiteTile;
    [SerializeField] private Sprite _blackTile;

    [Header("White pieces")]
    [SerializeField] private Sprite _whiteKing;
    [SerializeField] private Sprite _whiteQueen;
    [SerializeField] private Sprite _whiteRook;
    [SerializeField] private Sprite _whiteBishop;
    [SerializeField] private Sprite _whiteKnight;
    [SerializeField] private Sprite _whitePawn;

    [Header("Black pieces")]
    [SerializeField] private Sprite _blackKing;
    [SerializeField] private Sprite _blackQueen;
    [SerializeField] private Sprite _blackRook;
    [SerializeField] private Sprite _blackBishop;
    [SerializeField] private Sprite _blackKnight;
    [SerializeField] private Sprite _blackPawn;

    public Sprite GetBoard() => _board;

    public Sprite GetTile(Color color) => color == Color.White ? _whiteTile : _blackTile;

    public Sprite GetPiece(Color color, Type piece)
    {
        if (color == Color.White)
        {
            if (piece ==   typeof(King)) return _whiteKing;
            if (piece ==  typeof(Queen)) return _whiteQueen;
            if (piece ==   typeof(Rook)) return _whiteRook;
            if (piece == typeof(Bishop)) return _whiteBishop;
            if (piece == typeof(Knight)) return _whiteKnight;
            if (piece ==   typeof(Pawn)) return _whitePawn;
        }
        else
        {
            if (piece ==   typeof(King)) return _blackKing;
            if (piece ==  typeof(Queen)) return _blackQueen;
            if (piece ==   typeof(Rook)) return _blackRook;
            if (piece == typeof(Bishop)) return _blackBishop;
            if (piece == typeof(Knight)) return _blackKnight;
            if (piece ==   typeof(Pawn)) return _blackPawn;
        }
        throw new ArgumentException("Sprite not found");
    }
}
