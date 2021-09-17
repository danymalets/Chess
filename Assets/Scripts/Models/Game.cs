using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public Position Position { get; private set; }
    public readonly Dictionary<Position, int> History = new Dictionary<Position, int>();

    public readonly List<string> StringMoves;
    public bool IsEnd { get; private set; }
    
    private bool _tripleRepetition;

    private List<Position> _positions;

    private List<Move> _moves;

    public Game()
    {
        _moves = new List<Move>();
        StringMoves = new List<string>();

        Position = new Position();

        Position copy = new Position(Position);
        _positions = new List<Position>() { copy };
        History[copy] = 1;
    }

    public Game(List<string> moves)
    {
        _moves = new List<Move>();
        StringMoves = moves;
        Position = new Position();

        Position copy = new Position(Position);
        _positions = new List<Position>() { copy };
        History[copy] = 1;

        foreach (string sMove in moves)
        {
            Move move = Move.FromString(sMove);
            _moves.Add(move);
            move.Make(Position);
            _positions.Add(new Position(Position));
        }
    }

    public int GetPositionsCount() => _positions.Count;

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
        if (_tripleRepetition)
        {
            IsEnd = true;
            return "Ничья. Позиция была повторена трижды";
        }
        if (UselessMoves() == 50)
        {
            IsEnd = true;
            return "Ничья. 50 ходов без взятий и движения пешки";
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
            if (_moves[i].IsUselessMove(_positions[i]))
            {
                count++;
                i--;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    public void MakeMove(Move move)
    {
        move.Make(Position);

        Position copy = new Position(Position);
        if (!History.ContainsKey(copy))
        {
            History[copy] = 1;
        }
        else
        {
            History[copy]++;
            if (History[copy] == 3)
            {
                _tripleRepetition = true;
            }
        }
        _positions.Add(copy);
        _moves.Add(move);
        StringMoves.Add(move.ToString());
    }

    public void Undo()
    {
        IsEnd = false;
        _tripleRepetition = false;
        int count = History[Position];
        if (count == 1)
        {
            History.Remove(Position);
        }
        else
        {
            History[Position] = count - 1;
        }
        _positions.RemoveAt(_positions.Count - 1);
        _moves.RemoveAt(_moves.Count - 1);
        StringMoves.RemoveAt(StringMoves.Count - 1);
        Position = new Position(_positions[_positions.Count - 1]);
    }

    private bool CanWin(Color color)
    {
        bool whiteBishop = false;
        bool blackBishop = false;
        int knights = 0;
        for (int x = 0; x < Position.Size; x++)
        {
            for (int y = 0; y < Position.Size; y++)
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
