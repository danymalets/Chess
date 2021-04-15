using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    static readonly int[,] MAIN = new int[,]
    {
        {  -6,  -8,  -8, -10, -10,  -8,  -8,  -6 },
        {  -6,  -8,  -8, -10, -10,  -8,  -8,  -6 },
        {  -6,  -8,  -8, -10, -10,  -8,  -8,  -6 },
        {  -6,  -8,  -8, -10, -10,  -8,  -8,  -6 },
        {  -4,  -6,  -6,  -8,  -8,  -6,  -6,  -4 },
        {  -2,  -4,  -4,  -4,  -4,  -4,  -4,  -2 },
        {   4,   4,   0,   0,   0,   0,   4,   4 },
        {   4,   6,   2,   0,   0,   2,   6,   4 }
    };

    static readonly int[,] END_GAME = new int[,]
    {
        {   0,   2,   2,   2,   2,   2,   2,   0 },
        {   3,   3,   3,   3,   3,   3,   3,   3 },
        {   3,   3,   3,   3,   3,   3,   3,   3 },
        {   2,   2,   2,   2,   2,   2,   2,   2 },
        {   2,   2,   2,   2,   2,   2,   2,   2 },
        {   1,   1,   1,   1,   1,   1,   1,   1 },
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {   0,   0,   0,   0,   0,   0,   0,   0 }
    };

    public King(Position position, Vector2Int location, Color color)
        : base(position, location, color) { }

    public static List<Vector2Int> Offsets = new List<Vector2Int>()
    {
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1, 1)
    };

    public override List<Move> GetPossibleMoves()
    {
        List<Move> moves = new List<Move>();
        foreach (var offset in Offsets)
        {
            Vector2Int newSquare = Square + offset;
            if (Position.OnBoard(newSquare))
            {
                if (Position.Board[newSquare.x, newSquare.y] == null)
                {
                    moves.Add(new Move(Square, Square + offset));
                }
                else if (Position.Board[newSquare.x, newSquare.y].Color != Color)
                {
                    moves.Add(new Capture(Square, Square + offset, Square + offset));
                }
            }
        }

        Color atackingColor = Color == Color.White ? Color.Black : Color.White;
        int x = Color == Color.White ? Position.SIZE - 1 : 0;


        if (Position.QueensideCastling[Color] && 
            Position.Board[x, 1] == null &&
            Position.Board[x, 2] == null &&
            Position.Board[x, 3] == null &&
            !Position.IsSquareUnderAtack(new Vector2Int(x, 4), atackingColor) &&
            !Position.IsSquareUnderAtack(new Vector2Int(x, 3), atackingColor))
        {
            moves.Add(new Castling(
                new Vector2Int(x, 4),
                new Vector2Int(x, 2),
                new Vector2Int(x, 0),
                new Vector2Int(x, 3)));
        }
        if (Position.KingsideCastling[Color] &&
            Position.Board[x, 6] == null &&
            Position.Board[x, 5] == null &&
            !Position.IsSquareUnderAtack(new Vector2Int(x, 4), atackingColor) &&
            !Position.IsSquareUnderAtack(new Vector2Int(x, 5), atackingColor))
        {
            moves.Add(new Castling(
                new Vector2Int(x, 4),
                new Vector2Int(x, 6),
                new Vector2Int(x, 7),
                new Vector2Int(x, 5)));
        }
        return moves;
    }

    public override int GetMainValue() => GetValue(MAIN);

    public override int GetEndgameValue() => GetValue(END_GAME);
}
