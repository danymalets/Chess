using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    const int VALUE = 20;

    static readonly int[,] MAIN = new int[,]
    {
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {  10,  10,  10,  10,  10,  10,  10,  10 },
        {   2,   2,   4,   6,   6,   4,   2,   2 },
        {   1,   1,   2,   5,   5,   2,   1,   1 },
        {   0,   0,   0,   4,   4,   0,   0,   0 },
        {   1,   0,  -2,   0,   0,  -2,   0,   1 },
        {   1,   2,   2,  -4,  -4,   2,   2,   1 },
        {   0,   0,   0,   0,   0,   0,   0,   0 }
    };

    static readonly int[,] END_GAME = new int[,]
    {
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {  10,  10,  10,  10,  10,  10,  10,  10 },
        {   8,   8,   8,   8,   8,   8,   8,   8 },
        {   5,   5,   5,   5,   5,   5,   5,   5 },
        {   3,   3,   3,   3,   3,   3,   3,   3 },
        {   1,   1,   1,   1,   1,   1,   1,   1 },
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {   0,   0,   0,   0,   0,   0,   0,   0 }
    };

    private static Type[] _newPieces = new Type[]
    {
        typeof(Queen),
        typeof(Rook),
        typeof(Bishop),
        typeof(Knight)
    };

    private static int[] _offsets = new int[] { -1, 0, 1 };

    public Pawn(Position position, Vector2Int square, Color color)
        : base(position, square, color) { }


    public override List<Move> GetPossibleMoves()
    {
        List<Move> moves = new List<Move>();
        int dx = Color == Color.White ? -1 : 1;
        int steps = Color == Color.White ? Position.SIZE - 2 - Square.x : Square.x - 1;

        if (steps == 5)
        {
            foreach (int dy in _offsets)
            {
                if (!Position.OnBoard(Square + new Vector2Int(dx, dy))) continue;
                bool move = dy == 0 && Position.Board[Square.x + dx, Square.y + dy] == null;
                bool capture =
                    dy != 0
                    && Position.Board[Square.x + dx, Square.y + dy] != null
                    && Position.Board[Square.x + dx, Square.y + dy].Color != Color;
                if (move | capture)
                {
                    foreach (Type newPiece in _newPieces)
                    {
                        if (move)
                        {
                            moves.Add(new Promotion(
                                Square,
                                Square + new Vector2Int(dx, dy),
                                newPiece));
                        }
                        else
                        {
                            moves.Add(new PromotionWithCapture(
                                Square,
                                Square + new Vector2Int(dx, dy),
                                newPiece));
                        }
                    }
                }
            }
        }
        else
        {
            foreach (int dy in _offsets)
            {
                if (!Position.OnBoard(Square + new Vector2Int(dx, dy))) continue;
                if (dy == 0)
                {
                    if (Position.Board[Square.x + dx, Square.y + dy] == null)
                    {
                        moves.Add(new Move(Square, Square + new Vector2Int(dx, dy)));
                    }
                }
                else
                {
                    if (Position.Board[Square.x + dx, Square.y + dy] != null
                        && Position.Board[Square.x + dx, Square.y + dy].Color != Color)
                    {
                        moves.Add(new Capture(
                            Square,
                            Square + new Vector2Int(dx, dy),
                            Square + new Vector2Int(dx, dy)));
                    }
                }
            }
        }

        if (steps == 0)
        {
            if (Position.Board[Square.x + dx, Square.y] == null
                && Position.Board[Square.x + 2 * dx, Square.y] == null)
            {
                moves.Add(new Move(Square, Square + new Vector2Int(dx * 2, 0)));
            }
        }

        if (steps == 3 && Position.EnPassantAvailable)
        {
            foreach (int dy in _offsets)
            {
                if (Square.y + dy == Position.PawnLine)
                {
                    moves.Add(new Capture(
                            Square,
                            Square + new Vector2Int(dx, dy),
                            Square + new Vector2Int(0, dy)));
                }
            }
        }

        return moves;
    }

    public override int GetMainValue() => VALUE + GetValue(MAIN);

    public override int GetEndgameValue() => VALUE + GetValue(END_GAME);

    public override int GetNumber() => 1;
}
