using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position: IEquatable<Position>
{
    public const int SIZE = 8;

    static readonly char[,] textBoard = new char[SIZE, SIZE]
    {
        { 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' },
        { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' },
        { '.', '.', '.', '.', '.', '.', '.', '.' },
        { '.', '.', '.', '.', '.', '.', '.', '.' },
        { '.', '.', '.', '.', '.', '.', '.', '.' },
        { '.', '.', '.', '.', '.', '.', '.', '.' },
        { 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' },
        { 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' }
    };

    public Color WhoseMove;

    public Piece[,] Board = new Piece[SIZE, SIZE];

    public Dictionary<Color, bool> KingsideCastling = new Dictionary<Color, bool>();
    public Dictionary<Color, bool> QueensideCastling = new Dictionary<Color, bool>();

    public bool EnPassantAvailable;
    public int PawnLine;

    public Position()
    {
        WhoseMove = Color.White;

        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                Color color = char.IsUpper(textBoard[x, y]) ? Color.White : Color.Black;
                Vector2Int position = new Vector2Int(x, y);
                switch (char.ToUpper(textBoard[x, y]))
                {
                    case 'K': Board[x, y] = new   King(this, position, color); break;
                    case 'Q': Board[x, y] = new  Queen(this, position, color); break;
                    case 'B': Board[x, y] = new Bishop(this, position, color); break;
                    case 'N': Board[x, y] = new Knight(this, position, color); break;
                    case 'R': Board[x, y] = new   Rook(this, position, color); break;
                    case 'P': Board[x, y] = new   Pawn(this, position, color); break;
                    case '.': Board[x, y] = null; break;
                }
            }
        }

        KingsideCastling[Color.White] = true;
        QueensideCastling[Color.White] = true;
        KingsideCastling[Color.Black] = true;
        QueensideCastling[Color.Black] = true;

        EnPassantAvailable = false;
    }

    public Position(Position position)
    {
        WhoseMove = position.WhoseMove;
        KingsideCastling[Color.White] = position.KingsideCastling[Color.White];
        QueensideCastling[Color.White] = position.QueensideCastling[Color.White];
        KingsideCastling[Color.Black] = position.KingsideCastling[Color.Black];
        QueensideCastling[Color.Black] = position.QueensideCastling[Color.Black];
        EnPassantAvailable = position.EnPassantAvailable;
        PawnLine = position.PawnLine;
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                if (position.Board[x, y] != null)
                {
                    Board[x, y] = (Piece)position.Board[x, y].Clone();
                    Board[x, y].Position = this;
                }
            }
        }
    }

    public bool IsCheckmate() => GetSmartMoves().Count == 0 && IsInCheck();

    public bool IsStalemate() => GetSmartMoves().Count == 0 && !IsInCheck();

    public List<Move> GetPossibleMoves()
    {
        List<Move> moves = new List<Move>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (Board[x, y] != null && Board[x, y].Color == WhoseMove)
                {
                    moves.AddRange(Board[x,y].GetPossibleMoves());
                }
            }
        }
        return moves;
    }

    public List<Move> GetSmartMoves()
    {
        List<Move> moves = new List<Move>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (Board[x, y] != null && Board[x, y].Color == WhoseMove)
                {
                    moves.AddRange(Board[x, y].GetSmartMoves());
                }
            }
        }
        return moves;
    }

    public bool IsInCheck() => IsInCheck(WhoseMove);

    public bool IsOpponentInCheck() => IsInCheck((Color)(-(int)WhoseMove));

    private bool IsInCheck(Color defendingColor)
    {
        Color attackingColor = defendingColor == Color.White ? Color.Black : Color.White;
        Vector2Int kingSquare = GetKingSquare(defendingColor);
        return IsSquareUnderAtack(kingSquare, attackingColor);
    }

    public Vector2Int GetKingSquare(Color color)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (Board[x, y] is King && Board[x, y].Color == color)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        throw new InvalidOperationException("King not found");
    }

    public bool IsSquareUnderAtack(Vector2Int square, Color attackingColor)
    {
        foreach (Vector2Int offset in Rook.Offsets)
        {
            Vector2Int newSquare = square;
            while (true)
            {
                newSquare += offset;
                if (!OnBoard(newSquare)) break;
                if (Board[newSquare.x, newSquare.y] == null) continue;

                if (Board[newSquare.x, newSquare.y].Color == attackingColor
                    && (Board[newSquare.x, newSquare.y] is Rook
                    || Board[newSquare.x, newSquare.y] is Queen)) return true;
                else break;
            }
        }
        foreach (Vector2Int offset in Bishop.Offsets)
        {
            Vector2Int newSquare = square;
            while (true)
            {
                newSquare += offset;
                if (!OnBoard(newSquare)) break;
                if (Board[newSquare.x, newSquare.y] == null) continue;

                if (Board[newSquare.x, newSquare.y].Color == attackingColor
                    && (Board[newSquare.x, newSquare.y] is Bishop
                    || Board[newSquare.x, newSquare.y] is Queen)) return true;
                else break;
            }
        }
        foreach (Vector2Int offset in King.Offsets)
        {
            Vector2Int newSquare = square + offset;
            if (OnBoard(newSquare)
                && Board[newSquare.x, newSquare.y] != null
                && Board[newSquare.x, newSquare.y].Color == attackingColor
                && Board[newSquare.x, newSquare.y] is King) return true;
        }
        foreach (Vector2Int offset in Knight.Offsets)
        {
            Vector2Int newSquare = square + offset;
            if (OnBoard(newSquare)
                && Board[newSquare.x, newSquare.y] != null
                && Board[newSquare.x, newSquare.y].Color == attackingColor
                && Board[newSquare.x, newSquare.y] is Knight) return true;
        }
        int pawnX = attackingColor == Color.White ? square.x + 1 : square.x - 1;
        if (pawnX >= 0 && pawnX < SIZE)
        {
            if (square.y - 1 >= 0
                && Board[pawnX, square.y - 1] != null
                && Board[pawnX, square.y - 1].Color == attackingColor
                && Board[pawnX, square.y - 1] is Pawn) return true;
            if (square.y + 1 < SIZE
                && Board[pawnX, square.y + 1] != null
                && Board[pawnX, square.y + 1].Color == attackingColor
                && Board[pawnX, square.y + 1] is Pawn) return true;
        }
        return false;
    }

    public static bool OnBoard(Vector2Int square)
        => square.x >= 0 && square.x < SIZE && square.y >= 0 && square.y < SIZE;

    public bool Equals(Position other)
    {
        if (WhoseMove != other.WhoseMove) return false;
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                if ((Board[x, y] == null) != (other.Board[x, y] == null)) return false;
                if (Board[x, y] != null && (Board[x, y].GetType() != other.Board[x, y].GetType()
                    || Board[x, y].Color != other.Board[x, y].Color)) return false;
            }
        }
        if (EnPassantAvailable == other.EnPassantAvailable
            && QueensideCastling[WhoseMove] == other.QueensideCastling[WhoseMove]
            && KingsideCastling[WhoseMove] == other.KingsideCastling[WhoseMove]) return true;

        List<Move> moves = GetSmartMoves();
        List<Move> otherMoves = other.GetSmartMoves();
        if (moves.Count != otherMoves.Count) return false;
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].GetType() != otherMoves[i].GetType()) return false;
        }
        return true;
    }

    
}
