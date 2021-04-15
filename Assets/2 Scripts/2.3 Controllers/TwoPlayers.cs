using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoPlayers : GameController, ISave
{
    public TimeSpan MoveDuration;

    public TwoPlayers(TimeSpan moveDuration) : base()
    {
        MoveDuration = moveDuration;
    }

    public TwoPlayers(TimeSpan moveDuration, List<string> moves) : base(moves)
    {
        MoveDuration = moveDuration;
    }

    public override void Init(UI ui, Board board, Field field)
    {
        base.Init(ui, board, field);

        _ui.UserTimeIsOver += OnUserTimeIsOver;

        _board.InitPieces(Game.Position);
        _field.InitSquares(_board);

        _field.MoveShown += OnUserMoveShown;
        _field.MoveSelected += OnUserMoveSelected;

        if (!Game.IsEnd)
        {
            _field.EnableMoves();
            _ui.StartUserTimer(MoveDuration);
            if (Game.Position.WhoseMove == Color.Black)
            {
                _board.SetRotation(180);
            }
        }
        _ui.ChangeStatus(Game.GetStatus());
    }

    private void OnUserTimeIsOver()
    {
        _field.DisableMoves();
        _ui.ChangeStatus($"У {(Game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время");
    }

    private void OnUserMoveSelected(Move move)
    {
        Game.MakeMove(move);
        _ui.CanselCoundown();
    }

    private void OnUserMoveShown()
    {
        _ui.ChangeStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            _board.SetRotation(Game.Position.WhoseMove == Color.White ? 0 : 180);
            _field.EnableMoves();
            _ui.StartUserTimer(MoveDuration);
        }
        else
        {
            _ui.Clear();
        }
    }

    public override string ToString()
    {
        return "Два игрока";
    }
}
