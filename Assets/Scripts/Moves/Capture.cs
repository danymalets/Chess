using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capture: Move, ICapture
{
    public Vector2Int CaptureSquare { get; }

    public Capture(
        Vector2Int sourceSquare,
        Vector2Int targetSquare,
        Vector2Int captureSquare)
        : base(sourceSquare, targetSquare)
    {
        CaptureSquare = captureSquare;
    }

    public override void Make(Position position)
    {
        position.Board[CaptureSquare.x, CaptureSquare.y] = null;
        base.Make(position);
    }

    public override bool IsUselessMove(Position position) => false;

    public override bool Equals(Move other)
    {
        return base.Equals(other) && CaptureSquare == ((Capture)other).CaptureSquare;
    }

    public override string ToString()
    {
        return base.ToString() + "x" + SquareToString(CaptureSquare);
    }
}
