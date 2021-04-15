using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotVsBot : GameController
{
    private int _level;

    public BotVsBot(int level) : base()
    {
        _level = level;
    }

    public override void Init(UI ui, Board board, Field field)
    {
        base.Init(ui, board, field);

        _board.InitPieces(Game.Position);
        _field.InitSquares(_board);

        _ui.MoveFound += OnMoveFound;
        _field.MoveShown += OnRivalMoveShown;

        _ui.ChangeStatus(Game.GetStatus());
        _ui.StartSearchMove(Game, _level);
    }

    private void OnMoveFound(Move move)
    {
        Game.MakeMove(move);
        _field.ShowMove(move);
    }

    private void OnRivalMoveShown()
    {
        _ui.ChangeStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            _ui.StartSearchMove(Game, _level);
        }
    }

    public override string ToString()
    {
        return $"Бот против бота, {_level} уровень";
    }
}
