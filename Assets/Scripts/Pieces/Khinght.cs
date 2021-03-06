using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    private const int Value = 60;

    private static readonly int[,] s_main = 
    {
        { -10,  -8,  -6,  -6,  -6,  -6,  -8, -10 },
        {  -8,  -4,   0,   0,   0,   0,  -4,  -8 },
        {  -6,   0,   2,   3,   3,   2,   0,  -6 },
        {  -6,   1,   3,   4,   4,   3,   1,  -6 },
        {  -6,   0,   3,   4,   4,   3,   0,  -6 },
        {  -6,   1,   2,   3,   3,   2,   1,  -6 },
        {  -8,  -4,   0,   1,   1,   0,  -4,  -8 },
        { -10,  -8,  -6,  -6,  -6,  -6,  -8, -10 }
    };

    private static readonly int[,] s_endGame = 
    {
        { -10,  -8,  -6,  -6,  -6,  -6,  -8, -10 },
        {  -8,  -4,   0,   0,   0,   0,  -4,  -8 },
        {  -6,   0,   2,   3,   3,   2,   0,  -6 },
        {  -6,   0,   3,   4,   4,   3,   0,  -6 },
        {  -6,   0,   3,   4,   4,   3,   0,  -6 },
        {  -6,   0,   2,   3,   3,   2,   0,  -6 },
        {  -8,  -4,   0,   0,   0,   0,  -4,  -8 },
        { -10,  -8,  -6,  -6,  -6,  -6,  -8, -10 }
    };

    public Knight(Position position, Vector2Int location, Color color)
        : base(position, location, color) { }

    public static readonly List<Vector2Int> Offsets = new List<Vector2Int>()
    {
        new Vector2Int(-2, -1),
        new Vector2Int(-2, 1),
        new Vector2Int(-1, -2),
        new Vector2Int(-1, 2),
        new Vector2Int(1, -2),
        new Vector2Int(1, 2),
        new Vector2Int(2, -1),
        new Vector2Int(2, 1)
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
        return moves;
    }

    public override int GetMainValue() => Value + GetValue(s_main);
    public override int GetEndgameValue() => Value + GetValue(s_endGame);

    protected override int GetNumber() => 2;
}
