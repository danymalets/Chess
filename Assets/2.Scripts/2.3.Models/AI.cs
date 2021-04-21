using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = System.Random;

public class AI 
{
    private static Random random = new Random();

    private static int[] COUNTS = new int[]
    {
        25,
        290,
        5000,
        20000,
        350000,
        1000000
    };

    const int INF = (int)2e9;
    const int CHECK_MATE = (int)1e9;
    const int CHECK_MATE_HEIGHT = (int)1e8;

    private int _level;

    private int _count = 0;
    private int _maxCount;

    private Position _position;
    private Dictionary<Position, int> _history;

    private bool _solved = false;
    private Move _move;

    private bool _mandatoryCompletion = true;

    public AI(Position position, int level, Dictionary<Position, int> history)
    {
        _level = level;
        _position = position;
        _maxCount = COUNTS[level - 1];

        _history = new Dictionary<Position, int>();
        foreach (var temp in history)
        {
            _history[temp.Key] = temp.Value;
        }
    }

    public void StartSolving()
    {
        Thread thread = new Thread(new ThreadStart(Solving));
        thread.Start();
    }

    private void Solving()
    {
        _move = GetMove();
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

    public float GetPart()
    {
        return (float)_count / _maxCount;
    }

    public void Abort()
    {
        _mandatoryCompletion = false;
        _maxCount = 0;
    }

    private Move GetMove()
    {
        _count = 0;
        Move bestMove = Solve(_position, _level, true);
        Debug.Log(_count);
        _mandatoryCompletion = false;
        while (true)
        {
            Move move = Solve(_position, ++_level, false);
            if (move == null)
            {
                break;
            }
            else
            {
                bestMove = move;
            }
        }
        Debug.Log("level " + (_level - 1));
        return bestMove;
    }

    private Move Solve(Position position, int maxHeight, bool mandatoryCompetion) =>
        Solve(position, 0, maxHeight, -INF-1, new List<Move>()).Item1;

    private (Move, int) Solve(
        Position position,
        int height,
        int maxHeight,
        int breakPoint,
        List<Move> maybeBestMoves)
    {
        _count++;
        if (!_mandatoryCompletion)
        {
            if (_count >= _maxCount) return (null, 0);
        }
        if (position.IsOpponentInCheck()) return (null, INF);

        if (height == maxHeight) return (null, GetValue(position));
        List<Move> moves = position.GetPossibleMoves();
        if (height == 0) Shuffle(moves);

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
            if (_history.ContainsKey(newPosition))
            {
                count = _history[newPosition];
            }
            else
            {
                count = 0;
            }

            _history[newPosition] = count + 1;

            if (count == 2)
            {
                childBestMove = null;
                value = -19;
            }
            else
            {
                (childBestMove, value) = Solve(
                    newPosition,
                    height + 1,
                    maxHeight,
                    bestValue,
                    childrenBestMoves);
            }

            if (count == 0)
            {
                _history.Remove(newPosition);
            }
            else
            {
                _history[newPosition] = count;
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
