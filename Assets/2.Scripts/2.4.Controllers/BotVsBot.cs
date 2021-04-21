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

        _ui.SetTitle($"Бот против бота, {_level} уровень");

        _board.InitPieces(Game.Position);

        _ui.MoveFound += OnMoveFound;
        _board.MoveShown += OnRivalMoveShown;

        _ui.SetStatus(Game.GetStatus());
        _ui.StartSearchMove(new AI(Game.Position, _level, Game.History));
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
            _ui.StartSearchMove(new AI(Game.Position, _level, Game.History));
        }
    }
}
