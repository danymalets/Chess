using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = System.Random;
using ThreadPriority = System.Threading.ThreadPriority;

public class AI 
{
    private static Random random = new Random();

    private static int[] COUNTS = new int[]
    {
        25,
        300,
        5000,
        25000,
        300000,
        1000000
    };

    const int INF = (int)2e9;
    const int CHECK_MATE = (int)1e9;
    const int CHECK_MATE_HEIGHT = (int)1e7;
    const int TRIPLE_REPETITION = -19;


    private int _level;

    private int _count = 0;
    private int _maxCount;

    private float _solvedPart;

    private Game _game;

    private Dictionary<Position, int> _tempHistory;

    private bool _solved = true;
    private Move _move;

    private bool _mandatoryCompletion = true;

    private Thread _thread;

    public AI(Game game, int level)
    {
        _game = game;
        _level = level;
    }

    public void StartSolving()
    {
        _solvedPart = 0;
        _count = 0;
        _solved = false;
        _maxCount = COUNTS[_level - 1];
        _thread = new Thread(new ThreadStart(Solving))
        {
            Priority = ThreadPriority.Highest
        };
        _thread.Start();
    }

    private void Solving()
    {
        _move = GetMove(_level);
        _solved = true;
    }

    public bool IsSolved(out Move move)
    {
        if (_solved)
        {
            move = _move;
            return true;
        }
        else
        {
            move = null;
            return false;
        }
    }

    private bool IsSolved() => IsSolved(out _);

    public float GetPart()
    {
        return Math.Min(_solvedPart, (float)_count / _maxCount);
    }

    public void Stop()
    {
        _mandatoryCompletion = false;
        _maxCount = 0;
        while (!IsSolved())
        {
            Thread.Sleep(0);
        }
    }

    private Move GetMove(int level)
    {
        _tempHistory = new Dictionary<Position, int>();
        foreach (var pair in _game.History)
        {
            _tempHistory[pair.Key] = pair.Value;
        }
        _mandatoryCompletion = true;
        Move bestMove = Solve(_game.Position, level);
        Debug.Log("count = " + _count);
        _mandatoryCompletion = false;
        while (true)
        {
            Move move = Solve(_game.Position, ++level);
            if (move == null)
            {
                break;
            }
            else
            {
                bestMove = move;
            }
        }
        Debug.Log("level " + (level - 1));
        return bestMove;
    }

    private Move Solve(Position position, int maxHeight) =>
        Solve(position, 0, maxHeight, -INF-1, new List<Move>(), 1f).Item1;

    private (Move, int) Solve(
        Position position,
        int height,
        int maxHeight,
        int breakPoint,
        List<Move> maybeBestMoves,
        float part)
    {
        if (height == 3)
        {
            _solvedPart += part;
            part = 0f;
        }

        _count++;
        if (!_mandatoryCompletion)
        {
            if (_count >= _maxCount) return (null, 0);
        }
        if (position.IsOpponentInCheck())
        {
            _solvedPart += part;
            return (null, INF);
        }

        if (height == maxHeight)
        {
            _solvedPart += part;
            return (null, GetValue(position));
        }
        List<Move> moves = position.GetPossibleMoves();
        if (height == 0) Shuffle(moves);

        float childPart = part / moves.Count;

        int bestValue = -INF;
        Move bestMove = null;
        List<Move> childrenBestMoves = new List<Move>();
        
        int j = 0;
        for (int i = 0; i < moves.Count; i++)
        {
            foreach (Move childBestMove in maybeBestMoves)
            {
                if (moves[i].Equals(childBestMove))
                {
                    (moves[j], moves[i]) = (moves[i], moves[j]);
                    j++;
                    break;
                }
            }
        }
        foreach (Move move in moves)
        {
            Position newPosition = new Position(position);
            move.Make(newPosition);
            Move childBestMove;
            int value;

            int count;
            if (_tempHistory.ContainsKey(newPosition))
            {
                count = _tempHistory[newPosition];
            }
            else
            {
                count = 0;
            }

            _tempHistory[newPosition] = count + 1;

            if (count == 2)
            {
                _solvedPart += childPart;
                childBestMove = null;
                value = TRIPLE_REPETITION;
            }
            else
            {
                (childBestMove, value) = Solve(
                    newPosition,
                    height + 1,
                    maxHeight,
                    bestValue,
                    childrenBestMoves,
                    childPart);
            }

            part -= childPart;

            if (count == 0)
            {
                _tempHistory.Remove(newPosition);
            }
            else
            {
                _tempHistory[newPosition] = count;
            }

            if (!_mandatoryCompletion)
            {
                if (_count >= _maxCount) return (null, 0);
            }

            if (childBestMove != null)
            {
                childrenBestMoves.Add(childBestMove);
            }

            if (-value > bestValue)
            {
                bestValue = -value;
                bestMove = move;
                if (value <= breakPoint)
                {
                    _solvedPart += part;
                    return (bestMove, bestValue);
                }
            }
        }
        if (bestValue == -INF)
        {
            if (position.IsInCheck())
            {
                return (null, -CHECK_MATE + CHECK_MATE_HEIGHT * height);
            }
            else
            {
                return (null, 0);
            }
        }
        else
        {
            return (bestMove, bestValue);
        }
    }

    private void Shuffle(List<Move> moves)
    {
        for (int i = 1; i < moves.Count; i++)
        {
            int j = random.Next(0, i);
            (moves[i], moves[j]) = (moves[j], moves[i]);
        }
    }

    private int GetValue(Position position)
    {
        int count = 0;
        for (int x = 0; x < Position.SIZE; x++)
        {
            for (int y = 0; y < Position.SIZE; y++)
            {
                if (position.Board[x, y] != null) count++;
            }
        }
        int value = 0;
        if (count >= 16)
        {
            for (int x = 0; x < Position.SIZE; x++)
            {
                for (int y = 0; y < Position.SIZE; y++)
                {
                    if (position.Board[x, y] != null)
                    {
                        if (position.Board[x, y].Color == position.WhoseMove)
                        {
                            value += position.Board[x, y].GetMainValue();
                        }
                        else
                        {
                            value -= position.Board[x, y].GetMainValue();
                        }
                    }
                }
            }
        }
        else
        {
            for (int x = 0; x < Position.SIZE; x++)
            {
                for (int y = 0; y < Position.SIZE; y++)
                {
                    if (position.Board[x, y] != null)
                    {
                        if (position.Board[x, y].Color == position.WhoseMove)
                        {
                            value += position.Board[x, y].GetEndgameValue();
                        }
                        else
                        {
                            value -= position.Board[x, y].GetEndgameValue();
                        }
                    }
                }
            }
        }
        return value;
    }
}
