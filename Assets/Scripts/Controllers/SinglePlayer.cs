using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayer : GameController, IStorable, IUndoable
{
    private Color _playerColor;
    private int _level;

    private AI _ai;

    public SinglePlayer(Color playerColor, int level) : base()
    {
        _playerColor = playerColor;
        _level = level;
    }

    public SinglePlayer(
        Color playerColor,
        int level,
        Game game)
    {
        _game = game;
        _playerColor = playerColor;
        _level = level;
    }

    protected override void InternalInit()
    {
        _ai = new AI(_game, _level);

        _ui.SetTitle($"Один игрок, {_level} уровень, за {(_playerColor == Color.White ? "белых" : "чёрных")}");

        _board.InitPieces(_game.Position);

        _board.MoveSelected += OnUserMoveSelected;
        _moveFinder.MoveFound += OnMoveFound;

        _ui.SetStatus(_game.GetStatus());
        if (!_game.IsEnd)
        {
            if (_playerColor == _game.Position.WhoseMove)
            {
                _board.MoveShown += OnUserMoveShown;
                _board.EnableMoves();
            }
            else
            {
                _board.MoveShown += OnRivalMoveShown;
                _moveFinder.StartSearchMove(_ai);
            }

            if (_playerColor == Color.Black)
            {
                _board.SetRotation(180);
            }
        }
    }

    private void OnUserMoveSelected(Move move)
    {
        _game.MakeMove(move);
    }

    private void OnUserMoveShown()
    {
        _board.MoveShown -= OnUserMoveShown;

        _ui.SetStatus(_game.GetStatus());
        if (!_game.IsEnd)
        {
            Debug.Log("R " + _game.Position.WhoseMove);
            _moveFinder.StartSearchMove(_ai);
            _board.MoveShown += OnRivalMoveShown;
        }
        else
        {
            GameOver();
        }
    }

    private void OnMoveFound(Move move)
    {
        _game.MakeMove(move);
        _board.ShowMove(move);
    }

    private void OnRivalMoveShown()
    {
        _board.MoveShown -= OnRivalMoveShown;

        _ui.SetStatus(_game.GetStatus());
        if (!_game.IsEnd)
        {
            _board.EnableMoves();
            _board.MoveShown += OnUserMoveShown;
        }
        else
        {
            GameOver();
        }
    }

    public void Undo()
    {
        if (!IsUndoAllowed()) return;

        _ai.Stop();
        _timer.Clear();
        _board.Clear();
        _game.Undo();

        if (_game.Position.WhoseMove != _playerColor) _game.Undo();

        _board.InitPieces(_game.Position);
        _board.EnableMoves();

        _ui.SetStatus(_game.GetStatus());

        _board.MoveShown -= OnUserMoveShown;
        _board.MoveShown -= OnRivalMoveShown;

        _board.MoveShown += OnUserMoveShown;
    }

    private bool IsUndoAllowed()
    {
        if (_playerColor == Color.White)
        {
            return _game.GetPositionsCount() >= 2;
        }
        else
        {
            return _game.GetPositionsCount() >= 3;
        }
    }

    public StoredGame GetStoredGame()
    {
        return new StoredGame(StoredType.SinglePlayer, _game.StringMoves)
        {
            PlayerColor = (int)_playerColor,
            Level = _level
        };
    }

    public override void Finish()
    {
        _ai.Stop();
        base.Finish();
    }
}
