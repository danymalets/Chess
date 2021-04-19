using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    const int VALUE = 60;

    static readonly int[,] MAIN = new int[,]
    {
        {  -4,  -2,  -2,  -2,  -2,  -2,  -2,  -4 },
        {  -2,   0,   0,   0,   0,   0,   0,  -2 },
        {  -2,   0,   1,   2,   2,   1,   0,  -2 },
        {  -2,   1,   1,   2,   2,   1,   1,  -2 },
        {  -2,   0,   2,   2,   2,   2,   0,  -2 },
        {  -2,   2,   2,   2,   2,   2,   2,  -2 },
        {  -2,   1,   0,   0,   0,   0,   1,  -2 },
        {  -4,  -2,  -2,  -2,  -2,  -2,  -2,  -4 }
    };

    static readonly int[,] END_GAME = new int[,]
    {
        {  -4,  -2,  -2,  -2,  -2,  -2,  -2,  -4 },
        {  -2,   0,   0,   0,   0,   0,   0,  -2 },
        {  -2,   0,   1,   2,   2,   1,   0,  -2 },
        {  -2,   0,   2,   2,   2,   2,   0,  -2 },
        {  -2,   0,   2,   2,   2,   2,   0,  -2 },
        {  -2,   0,   1,   2,   2,   1,   0,  -2 },
        {  -2,   0,   0,   0,   0,   0,   0,  -2 },
        {  -4,  -2,  -2,  -2,  -2,  -2,  -2,  -4 }
    };

    public Bishop(Position position, Vector2Int location, Color color)
        : base(position, location, color) { }

    public static List<Vector2Int> Offsets = new List<Vector2Int>()
    {
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 1)
    };

    public override List<Move> GetPossibleMoves()
    {
        List<Move> moves = new List<Move>();
        foreach (var offset in Offsets)
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

    public override int GetMainValue() => VALUE + GetValue(MAIN);

    public override int GetEndgameValue() => VALUE + GetValue(END_GAME);

    public override int GetNumber() => 3;
}
