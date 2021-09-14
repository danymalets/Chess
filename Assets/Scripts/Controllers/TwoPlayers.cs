using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoPlayers : GameController, IStorable
{
    private TimeSpan _moveDuration;

    public TwoPlayers(TimeSpan moveDuration) : base()
    {
        _moveDuration = moveDuration;
    }

    public TwoPlayers(
        TimeSpan moveDuration,
        Game game)
    {
        _game = game;
        _moveDuration = moveDuration;
    }

    protected override void InternalInit()
    {
        _ui.SetTitle("Два игрока");

        _timer.UserTimeIsOver += OnUserTimeIsOver;

        _board.InitPieces(_game.Position);

        _board.MoveShown += OnUserMoveShown;
        _board.MoveSelected += OnUserMoveSelected;

        if (!_game.IsEnd)
        {
            _board.EnableMoves();
            _timer.StartUserTimer(_moveDuration);
            if (_game.Position.WhoseMove == Color.Black)
            {
                _board.SetRotation(180);
            }
        }
        _ui.SetStatus(_game.GetStatus());
    }

    private void OnUserTimeIsOver()
    {
        _board.DisableMoves();
        _ui.SetStatus($"У {(_game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время");
    }

    private void OnUserMoveSelected(Move move)
    {
        _game.MakeMove(move);
        _timer.CancelCountdown();
    }

    private void OnUserMoveShown()
    {
        _ui.SetStatus(_game.GetStatus());
        if (!_game.IsEnd)
        {
            _board.SetRotation(_game.Position.WhoseMove == Color.White ? 0 : 180);
            _board.EnableMoves();
            _timer.StartUserTimer(_moveDuration);
        }
        else
        {
            _timer.Clear();
        }
    }

    public StoredGame GetStoredGame()
    {
        return new StoredGame(StoredType.TwoPlayers, _game.StringMoves)
        {
            MoveDuration = (int)_moveDuration.TotalSeconds
        };
    }
}
