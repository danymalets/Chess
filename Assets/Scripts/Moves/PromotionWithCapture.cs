using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionWithCapture : Promotion, ICapture
{
    public Vector2Int CaptureSquare { get; set; }

    public PromotionWithCapture(
        Vector2Int sourceSquare,
        Vector2Int targetSquare,
        Type newPiece)
        : base(sourceSquare, targetSquare, newPiece)
    {
        CaptureSquare = targetSquare;
    }

    public override string ToString()
    {
        return base.ToString().Substring(0, 4) + "*" + PieceToString(NewPiece);
    }
}
