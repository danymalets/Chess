using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public Position Position;
    public List<Position> RepeatingPositions = new List<Position>();
    public List<string> StringMoves;
    public bool IsEnd = false;

    private List<Position> _positions;

    private List<Move> _moves;

    private Position _tripleRepetition = null;

    public Game()
    {
        _moves = new List<Move>();
        StringMoves = new List<string>();

        Position = new Position();
        _positions = new List<Position>() { new Position(Position) };
    }

    public Game(List<string> moves)
    {
        _moves = new List<Move>();
        StringMoves = moves;
        Position = new Position();
        _positions = new List<Position>() { new Position(Position) };
        foreach (string sMove in moves)
        {
            Move move = Move.FromString(sMove);
            _moves.Add(move);
            move.Make(Position);
            _positions.Add(new Position(Position));
        }
        Debug.Log(Position.Board[4, 4]);
    }

    public string GetStatus()
    {
        if (Position.IsCheckmate())
        {
            IsEnd = true;
            return $"Мат {(Position.WhoseMove == Color.White ? "белым" : "чёрным")}";
        }
        if (Position.IsStalemate())
        {
            IsEnd = true;
            return $"Ничья. Пат {(Position.WhoseMove == Color.White ? "белым" : "чёрным")}";
        }
        if (_tripleRepetition != null)
        {
            IsEnd = true;
            return "Ничья. Позиция была повторена трижды";
        }
        if (UselessMoves() == 50)
        {
            IsEnd = true;
            return "Ничья. 50 бесполезных ходов";
        }
        if (!CanWin(Color.White) && !CanWin(Color.Black))
        {
            IsEnd = true;
            return "Ничья. Недостаточно фигур для мата";
        }
        else if (Position.IsInCheck())
        {
            return $"Шах {(Position.WhoseMove == Color.White ? "белым" : "чёрным")}";
        }
        else
        {
            return $"Ход {(Position.WhoseMove == Color.White ? "белых" : "чёрных")}";
        }
    }

    public int UselessMoves()
    {
        int count = 0;
        int i = _moves.Count - 1;
        while (i >= 0)
        {
            Piece piece = _positions[i].Board[_moves[i].SourceSquare.x, _moves[i].SourceSquare.y];
            if ((piece != null && piece is Pawn) || _moves[i] is ICapture)
            {
                break;
            }
            else
            {
                count++;
                i--;
            }
        }
        return count;
    }

    public void MakeMove(Move move)
    {
        move.Make(Position);
        foreach (Position other in RepeatingPositions)
        {
            if (Position.Equals(other))
            {
                _tripleRepetition = other;
            }
        }
        foreach (Position other in _positions)
        {
            if (Position.Equals(other))
            {
                RepeatingPositions.Add(other);
            }
        }
        _positions.Add(new Position(Position));
        _moves.Add(move);
        StringMoves.Add(move.ToString());
    }

    public void Undo()
    {
        IsEnd = false;
        Position last = _positions[_positions.Count - 1];
        if (last == _tripleRepetition)
        {
            _tripleRepetition = null;
        }
        else
        {
            RepeatingPositions.Remove(last);
        }
        _positions.RemoveAt(_positions.Count - 1);
        _moves.RemoveAt(_moves.Count - 1);
        StringMoves.RemoveAt(StringMoves.Count - 1);
        Position = new Position(_positions[_positions.Count - 1]);
    }

    public bool IsUndoAllowed() => _moves.Count >= 2;

    private bool CanWin(Color color)
    {
        bool whiteBishop = false;
        bool blackBishop = false;
        int knights = 0;
        for (int x = 0; x < Position.SIZE; x++)
        {
            for (int y = 0; y < Position.SIZE; y++)
            {
                if (Position.Board[x, y] != null && Position.Board[x, y].Color == color)
                {
                    Type piece = Position.Board[x, y].GetType();
                    if (piece == typeof(Knight)) knights++;
                    else if (piece == typeof(Rook)) return true;
                    else if (piece == typeof(Queen)) return true;
                    else if (piece == typeof(Pawn)) return true;
                    else if (piece == typeof(Bishop))
                    {
                        if ((x % 2) == (y % 2)) whiteBishop = true;
                        else blackBishop = true;
                    }
                }
            }
        }
        bool bishop = whiteBishop | blackBishop;
        return (knights >= 2) || (bishop && knights >= 1) || (whiteBishop && blackBishop);
    }
}
