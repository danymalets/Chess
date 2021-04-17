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

    public override void Init(GameUI ui, Board board)
    {
        base.Init(ui, board);

        _board.InitPieces(Game.Position);

        _ui.MoveFound += OnMoveFound;
        _board.MoveShown += OnRivalMoveShown;

        _ui.SetStatus(Game.GetStatus());
        _ui.StartSearchMove(Game, _level);
    }

    private void OnMoveFound(Move move)
    {
        Game.MakeMove(move);
        _board.ShowMove(move);
    }

    private void OnRivalMoveShown()
    {
        _ui.SetStatus(Game.GetStatus());
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
