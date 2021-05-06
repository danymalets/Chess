using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = System.Random;
using ThreadPriority = System.Threading.ThreadPriority;

public class AI 
{
    private static Random _random = new Random();

    private static int[] COUNTS = new int[]
    {
        51,
        377,
        4444,
        19853,
        353128,
        2043788
    };

    const int INF = (int)2e9;
    const int CHECK_MATE = (int)1e9;
    const int CHECK_MATE_HEIGHT = (int)1e7;
    const int TRIPLE_REPETITION = -19;
    const int STALE_MATE = -19;
    const int USELESS_MOVES = -19;

    private Game _game;
    private int _level;

    private float _solvedPart;
    private int _count = 0;
    private int _maxCount;

    private bool _solved = true;
    private Move _move;

    private bool _mandatoryCompletion = true;

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
        Thread thread = new Thread(new ThreadStart(Solving))
        {
            //Priority = ThreadPriority.Highest
        };
        thread.Start();
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

    public float GetPart()
    {
        return Math.Min(_solvedPart, (float)_count / _maxCount);
    }

    public void Stop()
    {
        _mandatoryCompletion = false;
        _maxCount = 0;
        while (!_solved)
        {
            Thread.Sleep(0);
        }
    }

    static int mx=0;

    private Move GetMove(int level)
    {
        int uselessMoves = _game.UselessMoves();
        var tempHistory = new Dictionary<Position, int>();
        foreach (var pair in _game.History)
        {
            tempHistory[pair.Key] = pair.Value;
        }
        Debug.Log("u" + tempHistory[_game.Position]);
        _mandatoryCompletion = true;
        Move bestMove = Solve(_game.Position, level, tempHistory, uselessMoves);
        Debug.Log("count = " + _count);
        mx = Math.Max(_count, mx);
        Debug.Log("max = " + mx);
        _mandatoryCompletion = false;
        while (true)
        {
            Move move = Solve(_game.Position, ++level, tempHistory, uselessMoves);
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

    private Move Solve(
        Position position,
        int maxHeight,
        Dictionary<Position, int> history,
        int uselessMoves)
    {
        var t =  Solve(
            position,
            0,
            maxHeight,
            -INF - 1,
            new List<Move>(),
            history,
            uselessMoves,
            1f);
        Debug.Log("result = " + t.Item2);
        return t.Item1;
    }

    private (Move, int) Solve(
        Position position,
        int height,
        int maxHeight,
        int breakPoint,
        List<Move> maybeBestMoves,
        Dictionary<Position, int> history,
        int uselessMoves,
        float part)
    {
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
       

        float childPart = part / moves.Count;

        int bestValue = -INF;
        Move bestMove = null;
        List<Move> childrenBestMoves = new List<Move>();


        int j = 0;
        if (height == 0)
        {
            Shuffle(moves);
        }
        else
        {
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
        }
        for (int i = j; i < moves.Count; i++)
        {
            if (moves[i] is ICapture || moves[i] is Promotion)
            {
                (moves[j], moves[i]) = (moves[i], moves[j]);
                j++;
            }
        }
        foreach (Move move in moves)
        {
            Position newPosition = new Position(position);
            move.Make(newPosition);
            Move childBestMove;
            int value;

            int count;
            if (history.ContainsKey(newPosition))
            {
                count = history[newPosition];
            }
            else
            {
                count = 0;
            }

            count++;


            history[newPosition] = count;

            if (move.IsUselessMove(position)) uselessMoves++;

            if (count == 3)
            {
                _solvedPart += childPart;
                childBestMove = null;
                value = -TRIPLE_REPETITION;
            }
            else if (uselessMoves >= 50)
            {
                _solvedPart += childPart;
                childBestMove = null;
                value = -USELESS_MOVES - GetValue(newPosition) / 20;
            }
            else
            {
                (childBestMove, value) = Solve(
                    newPosition,
                    height + 1,
                    maxHeight,
                    bestValue,
                    childrenBestMoves,
                    history,
                    uselessMoves,
                    childPart);
            }

            if (move.IsUselessMove(position)) uselessMoves--;

            part -= childPart;

            count--;

            if (count == 0)
            {
                history.Remove(newPosition);
            }
            else
            {
                history[newPosition] = count;
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
                return (null, -CHECK_MATE + CHECK_MATE_HEIGHT * height + GetValue(position));
            }
            else
            {
                return (null, STALE_MATE);
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
            int j = _random.Next(0, i);
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
