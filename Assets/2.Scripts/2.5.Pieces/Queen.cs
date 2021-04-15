using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    const int VALUE = 180;

    static readonly int[,] MAIN = new int[,]
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

    static readonly int[,] END_GAME = new int[,]
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

    public Queen(Position position, Piece piece) : base(position, piece) { }

    private static List<Vector2Int> _offsets = new List<Vector2Int>()
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
        foreach (var offset in _offsets)
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
}
