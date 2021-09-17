using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    private const int Value = 180;

    private static readonly int[,] s_main = new int[,]
    {
        {  -4,  -2,  -2,  -1,  -1,  -2,  -2,  -4 },
        {  -2,   0,   0,   0,   0,   1,   0,  -2 },
        {  -2,   0,   1,   1,   1,   1,   1,  -2 },
        {  -1,   0,   1,   1,   1,   1,   0,   0 },
        {  -1,   0,   1,   1,   1,   1,   0,  -1 },
        {  -2,   0,   1,   1,   1,   1,   0,  -2 },
        {  -2,   0,   0,   0,   0,   0,   0,  -2 },
        {  -4,  -2,  -2,  -1,  -1,  -2,  -2,  -4 }
    };

    private static readonly int[,] s_endGame = new int[,]
    {
        {  -4,  -2,  -2,  -1,  -1,  -2,  -2,  -4 },
        {  -2,   0,   0,   0,   0,   0,   0,  -2 },
        {  -2,   0,   1,   1,   1,   1,   0,  -2 },
        {  -1,   0,   1,   1,   1,   1,   0,  -1 },
        {  -1,   0,   1,   1,   1,   1,   0,  -1 },
        {  -2,   0,   1,   1,   1,   1,   0,  -2 },
        {  -2,   0,   0,   0,   0,   0,   0,  -2 },
        {  -4,  -2,  -2,  -1,  -1,  -2,  -2,  -4 }
    };

    public Queen(Position position, Vector2Int location, Color color)
        : base(position, location, color) { }

    private static List<Vector2Int> s_offsets = new List<Vector2Int>()
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 1)
    };

    public override List<Move> GetPossibleMoves()
    {
        List<Move> moves = new List<Move>();
        foreach (var offset in s_offsets)
        {
            Vector2Int newSquare = Square;
            while (true)
            {
                newSquare += offset;
                if (!Position.OnBoard(newSquare))
                {
                    break;
                }
                else if (Position.Board[newSquare.x, newSquare.y] == null)
                {
                    moves.Add(new Move(Square, newSquare));
                }
                else if (Position.Board[newSquare.x, newSquare.y].Color != Color)
                {
                    moves.Add(new Capture(Square, newSquare, newSquare));
                    break;
                }
                else
                {
                    break;
                }
            }
        }
        return moves;
    }

    public override int GetMainValue() => Value + GetValue(s_main);

    public override int GetEndgameValue() => Value + GetValue(s_endGame);

    protected override int GetNumber() => 5;
}
