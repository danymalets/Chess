using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Promotion: Move
{
    public Type NewPiece { get; }

    public Promotion(
        Vector2Int sourceSquare,
        Vector2Int targetSquare,
        Type newPiece)
        : base(sourceSquare, targetSquare)
    {
        NewPiece = newPiece;
    }

    public override void Make(Position position)
    {
        position.Board[SourceSquare.x, SourceSquare.y] =
            GetPiece(position.Board[SourceSquare.x, SourceSquare.y], NewPiece);
        base.Make(position);
    }

    private Piece GetPiece(Piece oldPiece, Type newPiece)
    {
        if (newPiece == typeof(Knight))
            return new Knight(oldPiece.Position, oldPiece.Square, oldPiece.Color);
        if (newPiece == typeof(Bishop))
            return new Bishop(oldPiece.Position, oldPiece.Square, oldPiece.Color);
        if (newPiece == typeof(Rook))
            return new Rook(oldPiece.Position, oldPiece.Square, oldPiece.Color);
        if (newPiece == typeof(Queen))
            return new Queen(oldPiece.Position, oldPiece.Square, oldPiece.Color);

        throw new ArgumentException($"Pawn cannot be promoted to {newPiece}");
    }

    public override bool Equals(Move other)
    {
        return base.Equals(other) && NewPiece.Equals(((Promotion)other).NewPiece);
    }

    public override string ToString()
    {
        return base.ToString() + "+" + PieceToString(NewPiece);
    }

    protected static string PieceToString(Type piece)
    {
        if (piece == typeof(Knight)) return "N";
        if (piece == typeof(Bishop)) return "B";
        if (piece == typeof(Rook)) return "R";
        if (piece == typeof(Queen)) return "Q";

        throw new ArgumentException($"Pawn cannot be promoted to {piece}");
    }

    public static Type StringToPiece(string s)
    {
        if (s == "N") return typeof(Knight);
        if (s == "B") return typeof(Bishop);
        if (s == "R") return typeof(Rook);
        if (s == "Q") return typeof(Queen);

        throw new ArgumentException($"Pawn cannot be promoted to {s}");
    }
}
