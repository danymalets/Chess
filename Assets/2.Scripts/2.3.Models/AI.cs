using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class AI 
{
    private static Random random = new Random();

    const int INF = (int)2e9;
    const int CHECK_MATE = (int)1e9;
    const int CHECK_MATE_HEIGHT = (int)1e8;

    private static int Count;
    private static int MaxCount;

    public static Position Position;
    public static int Level;
    public static List<Position> ProhibitedPositions;

    public static bool Solved;
    public static Move Move;

    private static Dictionary<Position, (Move, int)>[] _solvedPositions;

    public static void StartSearchMove()
    {
        _solvedPositions = new Dictionary<Position, (Move, int)>[10];
        for (int i = 0; i < 10; i++)
        {
            _solvedPositions[i] = new Dictionary<Position, (Move, int)>();
        }
        Move = GetMove();
        Solved = true;
    }

    public static Move GetMove()
    {
        Move move;
        int maxHight = Level;
        move = TryGetMove(Position, 1, ProhibitedPositions);
        Count = 0;
        MaxCount = 1_000_000_000;
        for (int level = 1; level <= 4; level++)
        {
            move = TryGetMove(Position, maxHight, ProhibitedPositions);
        }
        if (move == null) Debug.Log("error");
        Debug.Log("count " + Count);
        return move;
    }

    private static Move TryGetMove(
        Position position,
        int maxHight,
        List<Position> prohibitedPositions)
    {
        List<Move> moves = position.GetPossibleMoves();
        int bestValue = -INF;
        Move bestMove = null;
        List<Move> childrenBestMoves = new List<Move>();
        Shuffle(moves);
        float part = 0f;
        foreach (Move move in moves)
        {
            Position newPosition = new Position(position);
            move.Make(newPosition);
            (Move childBestMove, int value) = Solve(
                newPosition,
                1,
                maxHight,
                bestValue,
                childrenBestMoves,
                prohibitedPositions);

            if (childBestMove != null)
            {
                childrenBestMoves.Add(childBestMove);
            }

            if (-value > bestValue)
            {
                bestValue = -value;
                bestMove = move;
            }
            part += 1f / moves.Count;
        }
        return bestMove;
    }

    private static (Move, int) Solve(
        Position position,
        int height,
        int maxHeight,
        int breakPoint,
        List<Move> maybeBestMoves,
        List<Position> prohibitedPositions)
    {
        if (Count++ == MaxCount) return (null, 0);
        if (position.IsOpponentInCheck()) return (null, INF);
        if (height <= 4)
        {
            foreach (Position other in prohibitedPositions)
            {
                if (position.Equals(other)) return (null, -19);
            }
        }
        if (height == maxHeight) return (null, GetValue(position));
        List<Move> moves = position.GetPossibleMoves();
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
            (childBestMove, value) = Solve(
                newPosition,
                height + 1,
                maxHeight,
                bestValue,
                childrenBestMoves,
                prohibitedPositions);

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

    private static void Shuffle(List<Move> moves)
    {
        for (int i = 1; i < moves.Count; i++)
        {
            int j = random.Next(0, i);
            (moves[i], moves[j]) = (moves[j], moves[i]);
        }
    }

    private static int GetValue(Position position)
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
