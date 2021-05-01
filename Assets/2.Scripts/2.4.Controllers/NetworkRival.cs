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

    public NetworkRival(TimeSpan moveDuration) : base()
    {
        _moveDuration = moveDuration;
    }

    public override void Init(GameUI ui, Board board)
    {
        base.Init(ui, board);

        _ui.SetTitle(ToString());

        ConnectToServer();

        _provider.ConnectedToServer += OnConnectedToServer;
        _provider.RoomCreated += OnRoomCreated;
        _provider.RivalFound += OnRivalFound;

        _provider.ColorReceived += OnGameStarted;

        _provider.MoveReceived += OnMoveReceived;
        _provider.TimeIsOverReceived += OnTimeIsOverReceived;

        _provider.Disconnected += OnDisconnected;
        _provider.RivalDisconnected += OnRivalDisconnected;

        _ui.UserTimeIsOver += OnUserTimeIsOver;
        _ui.RivalTimeIsOver += OnRivalTimeIsOver;

        _board.MoveSelected += OnUserMoveSelected;

        _ui.SetStatus("Подключение");
    }

    protected abstract void ConnectToServer();

    private void OnConnectedToServer() => _ui.SetStatus("Проверка");
    private void OnRoomCreated() => _ui.SetStatus("Ожидание соперника");
    private void OnRivalFound() => _ui.SetStatus("Соперник найден");

    protected void OnGameStarted(Color playerColor)
    {
        _gameStarted = true;

        _ui.SetTitle($"{ToString()}, за {(playerColor == Color.White ? "белых" : "чёрных")}");

        _playerColor = playerColor;

        _board.InitPiecesWithSound(_game.Position);

        _ui.SetStatus(_game.GetStatus());
        if (_playerColor == Color.White)
        {
            _board.EnableMoves();
            _ui.StartUserTimer(_moveDuration);

            _board.MoveShown += OnUserMoveShown;
        }
        else
        {
            _board.SetRotation(180);
            _ui.StartRivalTimer(_moveDuration);

            _board.MoveShown += OnRivalMoveShown;
        }

        _ui.SetStatus(_game.GetStatus());
    }

    private void OnMoveReceived(Move move)
    {
        _game.MakeMove(move);
        _board.ShowMove(move);
        _ui.CanselCoundown();
    }

    private void OnUserMoveSelected(Move move)
    {
        _provider.SendMove(move);
        _game.MakeMove(move);
        _ui.CanselCoundown();
    }

    private void OnUserTimeIsOver()
    {
        _provider.SendTimerIsOver();
        _provider.PreDisconnect();
        _board.DisableMoves();
        _ui.SetStatus($"У {(_game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время");
        _isEnd = true;
        GameOver();
    }

    private void OnRivalTimeIsOver()
    {
        _provider.Disconnect();
        _ui.SetStatus($"У {(_game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время (но это не точно)");
        _isEnd = true;
        GameOver();
    }

    private void OnTimeIsOverReceived()
    {
        _ui.SetTimeIsOver();
        _provider.Disconnect();
        _ui.CanselCoundown();
        _ui.SetStatus($"У {(_game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время");
        _isEnd = true;
        GameOver();
    }

    private void OnUserMoveShown()
    {
        _board.MoveShown -= OnUserMoveShown;

        _ui.SetStatus(_game.GetStatus());
        if (!_game.IsEnd)
        {
            _ui.StartRivalTimer(_moveDuration);

            _board.MoveShown += OnRivalMoveShown;
        }
        else
        {
            _ui.Clear();
            _provider.PreDisconnect();

            _isEnd = true;
            GameOver();
        }
    }

    private void OnRivalMoveShown()
    {
        _board.MoveShown -= OnRivalMoveShown;

        _ui.SetStatus(_game.GetStatus());
        if (!_game.IsEnd)
        {
            _board.EnableMoves();
            _ui.StartUserTimer(_moveDuration);

            _board.MoveShown += OnUserMoveShown;
        }
        else
        {
            _ui.Clear();
            _provider.Disconnect();

            _isEnd = true;
            GameOver();
        }
    }

    private void OnDisconnected()
    {
        _provider.Disconnect();
        _ui.SetStatus($"Техническое поражение. Запрещено выходить из приложения во время игры");
        _ui.Clear();
        _board.DisableMoves();
        _isEnd = true;
        GameOver();
    }

    private void OnRivalDisconnected()
    {
        _provider.Disconnect();
        _ui.SetStatus($"{(_playerColor == Color.White ? "Чёрные" : "Белые")} сдались");
        _ui.Clear();
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
