﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoPlayers : GameController, ISave
{
    public static string Title = "TwoPlayers";

    public TimeSpan MoveDuration { get; private set; }

    public string StartDate { get; private set; }
    public string StartTime { get; private set; }

    public TwoPlayers(TimeSpan moveDuration) : base()
    {
        MoveDuration = moveDuration;
    }

    public TwoPlayers(TimeSpan moveDuration, List<string> moves) : base(moves)
    {
        MoveDuration = moveDuration;
    }

    public override void Init(GameUI ui, Board board)
    {
        DateTime now = DateTime.Now;
        StartDate = now.ToString("dd.MM.yyyy");
        StartTime = now.ToString("HH:mm");

        base.Init(ui, board);

        _ui.SetTitle("Два игрока");

        _ui.UserTimeIsOver += OnUserTimeIsOver;

        _board.InitPieces(Game.Position);

        _board.MoveShown += OnUserMoveShown;
        _board.MoveSelected += OnUserMoveSelected;

        if (!Game.IsEnd)
        {
            _board.EnableMoves();
            _ui.StartUserTimer(MoveDuration);
            if (Game.Position.WhoseMove == Color.Black)
            {
                _board.SetRotation(180);
            }
        }
        _ui.SetStatus(Game.GetStatus());
    }

    private void OnUserTimeIsOver()
    {
        _board.DisableMoves();
        _ui.SetStatus($"У {(Game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время");
    }

    private void OnUserMoveSelected(Move move)
    {
        Game.MakeMove(move);
        _ui.CanselCoundown();
    }

    private void OnUserMoveShown()
    {
        _ui.SetStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            _board.SetRotation(Game.Position.WhoseMove == Color.White ? 0 : 180);
            _board.EnableMoves();
            _ui.StartUserTimer(MoveDuration);
        }
        else
        {
            _ui.Clear();
        }
    }

    public GameModel GetGameModel()
    {
        return new GameModel(
            Title,
            -1,
            -1,
            (int)MoveDuration.TotalSeconds,
            Game.StringMoves,
            StartDate,
            StartTime);
    }
}
