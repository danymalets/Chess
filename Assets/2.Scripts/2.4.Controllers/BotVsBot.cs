using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotVsBot : GameController
{
    private int _level;
    private AI _ai;

    public BotVsBot(int level) : base()
    {
        _level = level;
    }

    public override void Init(GameUI ui, Board board)
    {
        base.Init(ui, board);

        _ai = new AI(_game, _level);

        _ui.SetTitle($"Бот против бота, {_level} уровень");

        _board.InitPieces(_game.Position);

        _ui.MoveFound += OnMoveFound;
        _board.MoveShown += OnRivalMoveShown;

        _ui.SetStatus(_game.GetStatus());
        _ui.StartSearchMove(_ai);
    }

    private void OnMoveFound(Move move)
    {
        _game.MakeMove(move);
        _board.ShowMove(move);
    }

    private void OnRivalMoveShown()
    {
        _ui.SetStatus(_game.GetStatus());
        if (!_game.IsEnd)
        {
            _ui.StartSearchMove(_ai);
        }
    }

    public override bool QuickExit() => true;

    public override void Finish()
    {
        _ai.Stop();
        base.Finish();
    }
}
