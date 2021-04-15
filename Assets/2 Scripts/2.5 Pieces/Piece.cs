using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece: ICloneable
{
    public Position Position;
    public Vector2Int Square;
    public Color Color;

    public Piece(Position position, Vector2Int square, Color color)
    {
        Position = position;
        Square = square;
        Color = color;
    }

    public Piece(Position position, Piece piece)
    {
        Position = position;
        Square = piece.Square;
        Color = piece.Color;
    }

    public abstract List<Move> GetPossibleMoves();

    public List<Move> GetSmartMoves()
    {
        List<Move> possibleMoves = GetPossibleMoves();
        List<Move> moves = new List<Move>();
        foreach (Move move in possibleMoves)
        {
            Position position = new Position(Position);
            move.Make(position);
            if (!position.IsOpponentInCheck()) moves.Add(move);
        }
        return moves;
    }

    public Piece Move(Vector2Int square)
    {
        Square = square;
        return this;
    }

    public object Clone() => MemberwiseClone();

    public abstract int GetMainValue();

    public abstract int GetEndgameValue();

    protected int GetValue(int[,] values)
    {
        if (Color == Color.White)
        {
            return values[Square.x, Square.y];
        }
        else
        {
            return values[Position.SIZE - 1 - Square.x, Square.y];
        }
    }
}
