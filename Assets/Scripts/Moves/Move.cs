using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : IEquatable<Move>
{
    public Vector2Int SourceSquare { get; }
    public Vector2Int TargetSquare { get; }

    public Move(
        Vector2Int sourceSquare,
        Vector2Int targetSquare)
    {
        SourceSquare = sourceSquare;
        TargetSquare = targetSquare;
    }

    public virtual void Make(Position position)
    {
        if (SourceSquare == new Vector2Int(Position.Size - 1, 0)
            || TargetSquare == new Vector2Int(Position.Size - 1, 0))
        {
            position.QueensideCastling[Color.White] = false;
        }
        if (SourceSquare == new Vector2Int(Position.Size - 1, Position.Size - 1)
            || TargetSquare == new Vector2Int(Position.Size - 1, 0))
        {
            position.KingsideCastling[Color.White] = false;
        }
        if (SourceSquare == new Vector2Int(0, 0)
            || TargetSquare == new Vector2Int(0, 0))
        {
            position.QueensideCastling[Color.Black] = false;
        }
        if (SourceSquare == new Vector2Int(0, Position.Size - 1)
            || TargetSquare == new Vector2Int(0, Position.Size - 1))
        {
            position.KingsideCastling[Color.Black] = false;
        }
        else if (position.Board[SourceSquare.x, SourceSquare.y] is King)
        {
            if (position.Board[SourceSquare.x, SourceSquare.y].Color == Color.White)
            {
                position.QueensideCastling[Color.White] = false;
                position.KingsideCastling[Color.White] = false;
            }
            else
            {
                position.QueensideCastling[Color.Black] = false;
                position.KingsideCastling[Color.Black] = false;
            }
        }

        if (position.Board[SourceSquare.x, SourceSquare.y] is Pawn
            && Math.Abs(SourceSquare.x - TargetSquare.x) == 2)
        {
            position.EnPassantAvailable = true;
            position.PawnLine = SourceSquare.y;
        }
        else
        {
            position.EnPassantAvailable = false;
        }

        position.Board[TargetSquare.x, TargetSquare.y] = position.Board[SourceSquare.x, SourceSquare.y].Move(TargetSquare);
        position.Board[SourceSquare.x, SourceSquare.y] = null;

        position.WhoseMove = (Color)(-(int)position.WhoseMove);
    }

    public virtual bool IsUselessMove(Position position)
    {
        Piece piece = position.Board[SourceSquare.x, SourceSquare.y];
        return !(piece is Pawn);
    }

    public virtual bool Equals(Move other)
    {
        return other != null 
               && GetType() == other.GetType() 
               && SourceSquare == other.SourceSquare
               && TargetSquare == other.TargetSquare;
    }

    public override string ToString()
    {
        return SquareToString(SourceSquare) + SquareToString(TargetSquare);
    }

    public static Move FromString(string s)
    {
        if (s.Length == 4)
        {
            return new Move(
                StringToSquare(s.Substring(0, 2)),
                StringToSquare(s.Substring(2, 2)));
        }
        else if (s[4] == '&')
        {
            return new Castling(
                StringToSquare(s.Substring(0, 2)),
                StringToSquare(s.Substring(2, 2)),
                StringToSquare(s.Substring(5, 2)),
                StringToSquare(s.Substring(7, 2)));
        }
        else if (s[4] == 'x')
        {
            return new Capture(
                StringToSquare(s.Substring(0, 2)),
                StringToSquare(s.Substring(2, 2)),
                StringToSquare(s.Substring(5, 2)));
        }
        else if (s[4] == '+')
        {
            return new Promotion(
                   StringToSquare(s.Substring(0, 2)),
                   StringToSquare(s.Substring(2, 2)),
                   Promotion.StringToPiece(s[5].ToString()));
        }
        else if (s[4] == '*')
        {
            return new PromotionWithCapture(
                   StringToSquare(s.Substring(0, 2)),
                   StringToSquare(s.Substring(2, 2)), 
                   Promotion.StringToPiece(s[5].ToString()));
        }
        return null;
    }

    protected static string SquareToString(Vector2Int square)
    {
        return ((char)('a' + square.y)).ToString() +
            (Position.Size - square.x).ToString();
    }

    private static Vector2Int StringToSquare(string s)
    {
        return new Vector2Int(Position.Size - int.Parse(s[1].ToString()), s[0] - 'a');
    }
}
