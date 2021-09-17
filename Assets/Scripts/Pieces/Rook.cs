using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    private const int Value = 100;

    private static readonly int[,] s_main = 
    {
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {   1,   2,   2,   2,   2,   2,   2,   1 },
        {  -1,   0,   0,   0,   0,   0,   0,  -1 },
        {  -1,   0,   0,   0,   0,   0,   0,  -1 },
        {  -1,   0,   0,   0,   0,   0,   0,  -1 },
        {  -1,   0,   0,   0,   0,   0,   0,  -1 },
        {  -1,   0,   0,   0,   0,   0,   0,  -1 },
        {   0,  -1,   0,   1,   1,   0,  -1,   0 }
    };

    private static readonly int[,] s_endGame = 
    {
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {   0,   0,   0,   0,   0,   0,   0,   0 },
        {   0,   0,   0,   0,   0,   0,   0,   0 }
    };

    public Rook(Position position, Vector2Int location, Color color)
        : base(position, location, color) { }

    public static readonly List<Vector2Int> Offsets = new List<Vector2Int>()
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
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

    public override int GetMainValue() => Value + GetValue(s_main);

    public override int GetEndgameValue() => Value + GetValue(s_endGame);

    protected override int GetNumber() => 4;
}
