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

    protected override void InternalInit()
    {
        _ai = new AI(_game, _level);

        View.SetTitle($"Бот против бота, {_level} уровень");

        _board.InitPieces(_game.Position);

        _moveFinder.MoveFound += OnMoveFound;
        _board.MoveShown += OnRivalMoveShown;

        View.SetStatus(_game.GetStatus());
        _moveFinder.StartSearchMove(_ai);
    }

    private void OnMoveFound(Move move)
    {
        _game.MakeMove(move);
        _board.ShowMove(move);
    }

    private void OnRivalMoveShown()
    {
        View.SetStatus(_game.GetStatus());
        if (!_game.IsEnd)
        {
            _moveFinder.StartSearchMove(_ai);
        }
        else
        {
            GameOver();
        }
    }

    public override bool QuickExit() => true;

    public override void Finish()
    {
        _ai.Stop();
        base.Finish();
    }
}
