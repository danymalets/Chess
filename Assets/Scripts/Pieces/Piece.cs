using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece: ICloneable, IEquatable<Piece>
{
    public Position Position;
    public Vector2Int Square;
    public readonly Color Color;

    protected Piece(Position position, Vector2Int square, Color color)
    {
        Position = position;
        Square = square;
        Color = color;
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
            return values[Position.Size - 1 - Square.x, Square.y];
        }
    }

    public override int GetHashCode() => (Color == Color.White ? 0 : 6) + GetNumber();

    protected abstract int GetNumber();

    public bool Equals(Piece other)
    {
        return GetType() == other.GetType() && Color.Equals(other.Color);
    }
}
