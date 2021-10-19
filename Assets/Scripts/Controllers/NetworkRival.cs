using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkRival : GameController
{
    private Color _playerColor;
    protected NetworkRivalProvider _provider;
    protected TimeSpan _moveDuration;

    private bool _isEnd = false;
    private bool _gameStarted = false;

    protected NetworkRival(TimeSpan moveDuration) : base()
    {
        _moveDuration = moveDuration;
    }

    protected override void InternalInit()
    {
        View.SetTitle(ToString());

        ConnectToServer();

        _provider.ConnectedToServer += OnConnectedToServer;
        _provider.RoomCreated += OnRoomCreated;
        _provider.RivalFound += OnRivalFound;

        _provider.ColorReceived += OnGameStarted;

        _provider.MoveReceived += OnMoveReceived;
        _provider.TimeIsOverReceived += OnTimeIsOverReceived;

        _provider.UserLeft += UserLeft;
        _provider.NetworkError += NetworkError;
        _provider.RivalDisconnected += OnRivalDisconnected;

        _timer.UserTimeIsOver += OnUserTimeIsOver;
        _timer.RivalTimeIsOver += OnRivalTimeIsOver;

        _board.MoveSelected += OnUserMoveSelected;

        View.SetStatus("Подключение");
    }

    protected abstract void ConnectToServer();

    private void OnConnectedToServer() => View.SetStatus("Проверка");
    private void OnRoomCreated() => View.SetStatus("Ожидание соперника");
    private void OnRivalFound() => View.SetStatus("Соперник найден");

    protected void OnGameStarted(Color playerColor)
    {
        _gameStarted = true;

        View.SetTitle($"{ToString()}, за {(playerColor == Color.White ? "белых" : "чёрных")}");

        _playerColor = playerColor;

        _board.InitPiecesWithSound(_game.Position);

        View.SetStatus(_game.GetStatus());
        if (_playerColor == Color.White)
        {
            _board.EnableMoves();
            _timer.StartUserTimer(_moveDuration);

            _board.MoveShown += OnUserMoveShown;
        }
        else
        {
            _board.SetRotation(180);
            _timer.StartRivalTimer(_moveDuration);

            _board.MoveShown += OnRivalMoveShown;
        }

        View.SetStatus(_game.GetStatus());
    }

    private void OnMoveReceived(Move move)
    {
        _game.MakeMove(move);
        _board.ShowMove(move);
        _timer.CancelCountdown();
    }

    private void OnUserMoveSelected(Move move)
    {
        _provider.SendMove(move);
        _game.MakeMove(move);
        _timer.CancelCountdown();
    }

    private void OnUserTimeIsOver()
    {
        _provider.SendTimerIsOver();
        _provider.PreDisconnect();
        _board.DisableMoves();
        View.SetStatus($"У {(_game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время");
        _isEnd = true;
        GameOver();
    }

    private void OnRivalTimeIsOver()
    {
        _provider.Disconnect();
        View.SetStatus($"У {(_game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время (но это не точно)");
        _isEnd = true;
        GameOver();
    }

    private void OnTimeIsOverReceived()
    {
        _timer.SetTimeIsOver();
        _provider.Disconnect();
        _timer.CancelCountdown();
        View.SetStatus($"У {(_game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время");
        _isEnd = true;
        GameOver();
    }

    private void OnUserMoveShown()
    {
        _board.MoveShown -= OnUserMoveShown;

        View.SetStatus(_game.GetStatus());
        if (!_game.IsEnd)
        {
            _timer.StartRivalTimer(_moveDuration);

            _board.MoveShown += OnRivalMoveShown;
        }
        else
        {
            _timer.Clear();
            _provider.PreDisconnect();

            _isEnd = true;
            GameOver();
        }
    }

    private void OnRivalMoveShown()
    {
        _board.MoveShown -= OnRivalMoveShown;

        View.SetStatus(_game.GetStatus());
        if (!_game.IsEnd)
        {
            _board.EnableMoves();
            _timer.StartUserTimer(_moveDuration);

            _board.MoveShown += OnUserMoveShown;
        }
        else
        {
            _timer.Clear();
            _provider.Disconnect();

            _isEnd = true;
            GameOver();
        }
    }

    private void UserLeft()
    {
        View.SetStatus($"Техническое поражение. Запрещено выходить из приложения во время игры");
        _timer.Clear();
        _board.DisableMoves();
        _isEnd = true;
        GameOver();
    }

    private void NetworkError()
    {
        if (_gameStarted && !_isEnd)
        {
            View.SetStatus($"Техническое поражение. Ошибка сети");
        }
        else if (!_isEnd)
        {
            View.SetStatus($"Ошибка сети");
        }
        _timer.Clear();
        _board.DisableMoves();
        _isEnd = true;
        GameOver();
    }

    private void OnRivalDisconnected()
    {
        _provider.Disconnect();
        View.SetStatus($"{(_playerColor == Color.White ? "Чёрные" : "Белые")} сдались");
        _timer.Clear();
        _board.DisableMoves();

        _isEnd = true;
        GameOver();
    }

    public override bool QuickExit()
    {
        return _isEnd || !_gameStarted;
    }

    public override void Finish()
    {
        base.Finish();
        _provider.Disconnect();
    }

    public abstract override string ToString();
}
