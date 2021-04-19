using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AI 
{
    const int INF = (int)2e9;
    const int CHECK_MATE = (int)1e9;
    const int CHECK_MATE_HEIGHT = (int)1e8;


    public static IEnumerable<(Move, float)> GetMove(
        Position position,
        int level,
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
                level,
                bestValue,
                childrenBestMoves,
                prohibitedPositions);
            childrenBestMoves.Add(childBestMove);
            if (-value > bestValue)
            {
                bestValue = -value;
                bestMove = move;
            }
            part += 1f / moves.Count;
            yield return (bestMove, part);
        }
    }

    private static (Move, int) Solve(
        Position position,
        int height,
        int level,
        int breakPoint,
        List<Move> maybeBestMoves,
        List<Position> prohibitedPositions)
    {
        if (position.IsOpponentInCheck()) return (null, INF);
        if (height <= 4)
            foreach (Position other in prohibitedPositions)
            {
                if (position.Equals(other)) return (null, -19);
            }
        if (height == level) return (null, GetValue(position));
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
                level,
                bestValue,
                childrenBestMoves,
                prohibitedPositions);

            childrenBestMoves.Add(childBestMove);
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
            int j = Random.Range(0, i + 1);
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
