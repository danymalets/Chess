using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Castling: Move
{
    public Move RookMove;

    public Castling(
        Vector2Int startPosition,
        Vector2Int endPosition,
        Vector2Int rookSourceSquare,
        Vector2Int rookTargetSquare)
        : base(startPosition, endPosition)
    {
        RookMove = new Move(rookSourceSquare, rookTargetSquare);
    }

    public override void Make(Position position)
    {
        if (position.Board[RookMove.SourceSquare.x, RookMove.SourceSquare.y] == null)
        {
            string s = "";
            for (int x = 0; x < Position.SIZE; x++)
            {
                for (int y = 0; y < Position.SIZE; y++)
                {
                    Piece piece = position.Board[x, y];
                    if (piece == null) s += ".";
                    else
                    {
                        string t = "?";
                        if (piece is Pawn) t = "p";
                        if (piece is Rook) t = "r";
                        if (piece is Knight) t = "n";
                        if (piece is Bishop) t = "b";
                        if (piece is Queen) t = "q";
                        if (piece is King) t = "k";
                        if (piece.Color == Color.White) t = t.ToUpper();
                        s += t;
                    }
                }
                s += "\n";
            }
            s += SourceSquare.ToString() + " " + TargetSquare.ToString();
            File.WriteAllText(@"/Users/danielmalec/Desktop/filed.txt", s);
        }
        position.Board[RookMove.TargetSquare.x, RookMove.TargetSquare.y] =
            position.Board[RookMove.SourceSquare.x, RookMove.SourceSquare.y].Move(RookMove.TargetSquare);
        position.Board[RookMove.SourceSquare.x, RookMove.SourceSquare.y] = null;
        base.Make(position);
    }

    public override bool Equals(Move other)
    {
        return base.Equals(other) && RookMove.Equals(((Castling)other).RookMove);
    }

    public override string ToString()
    {
        return base.ToString() + "&" + RookMove.ToString();
    }
}
