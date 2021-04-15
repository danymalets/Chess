using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayer : GameController
{
    private Color _playerColor;
    private NetworkProvider _provider;
    private TimeSpan _moveDuration;

    private bool _rivalDisconnected = false;

    public MultiPlayer(TimeSpan moveDuration): base()
    {
        _moveDuration = moveDuration;
    }

    public override void Init(UI ui, Board board, Field field)
    {
        base.Init(ui, board, field);

        _ui.UserTimeIsOver += OnUserTimeIsOver;
        _ui.RivalTimeIsOver += OnRivalTimeIsOver;

        _field.MoveSelected += OnUserMoveSelected;

        _provider = _ui.GetNetworkProvider();

        _provider.ConnectToServer();

        _provider.ConnectedToServer += OnConnectedToServer;
        _provider.RoomCreated += OnRoomCreated;
        _provider.RivalFound += OnRivalFound;

        _provider.ColorReceived += OnColorReceived;
        _provider.MoveReceived += OnMoveReceived;
        _provider.TimeIsOverReceived += OnTimeIsOverReceived;

        _provider.Disconnected += OnDisconnected;
        _provider.RivalDisconnected += OnRivalDisconnected;

        _ui.ChangeStatus("Подключение к серверу");
    }

    private void OnConnectedToServer() => _ui.ChangeStatus("Проверка");
    private void OnRoomCreated() => _ui.ChangeStatus("Ожидание соперника");
    private void OnRivalFound() => _ui.ChangeStatus("Соперник найден");

    private void OnColorReceived(Color playerColor)
    {
        _playerColor = playerColor;

        _board.InitPieces(Game.Position);
        _field.InitSquares(_board);

        _ui.ChangeStatus(Game.GetStatus());
        if (_playerColor == Color.White)
        {
            _field.EnableMoves();
            _ui.StartUserTimer(_moveDuration);

            _field.MoveShown += OnUserMoveShown;
        }
        else
        {
            _board.SetRotation(180);
            _ui.StartRivalTimer(_moveDuration);

            _field.MoveShown += OnRivalMoveShown;
        }

        _ui.ChangeStatus(Game.GetStatus());
    }

    private void OnMoveReceived(Move move)
    {
        Game.MakeMove(move);
        _field.ShowMove(move);
        _ui.CanselCoundown();
    }

    private void OnUserMoveSelected(Move move)
    {
        _provider.SendMove(move);
        Game.MakeMove(move);
        _ui.CanselCoundown();
        Debug.Log("cansel");
    }

    private void OnUserTimeIsOver()
    {
        _provider.SendTimerIsOver();
        _provider.PreDisconnect();
        _field.DisableMoves();
        _ui.ChangeStatus($"У {(Game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время");
    }

    private void OnRivalTimeIsOver()
    {
        _provider.Disconnect();
        _ui.ChangeStatus($"У {(Game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время (но это не точно)");
    }

    private void OnTimeIsOverReceived()
    {
        _provider.Disconnect();
        _ui.CanselCoundown();
        _ui.ChangeStatus($"У {(Game.Position.WhoseMove == Color.White ? "белых" : "чёрных")} вышло время");
    }

    private void OnUserMoveShown()
    {
        _field.MoveShown -= OnUserMoveShown;

        if (_rivalDisconnected) return;

        _ui.ChangeStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            _ui.StartRivalTimer(_moveDuration);

            _field.MoveShown += OnRivalMoveShown;
        }
        else
        {
            _ui.Clear();
            _provider.PreDisconnect();
        }
    }

    private void OnRivalMoveShown()
    {
        _field.MoveShown -= OnRivalMoveShown;

        if (_rivalDisconnected) return;

        _ui.ChangeStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            _field.EnableMoves();
            _ui.StartUserTimer(_moveDuration);

            _field.MoveShown += OnUserMoveShown;
        }
        else
        {
            _ui.Clear();
            _provider.Disconnect();
        }
    }

    private void OnDisconnected()
    {
        _ui.OnExit();
    }

    private void OnRivalDisconnected()
    {
        _provider.Disconnect();
        _ui.ChangeStatus($"{(_playerColor == Color.White ? "Чёрные" : "Белые")} сдались");
        _ui.Clear();
        _field.DisableMoves();
    }

    public override void Finish()
    {
        base.Finish();
        _provider.Disconnect();
    }

    public override string ToString()
    {
        return "Мультиплеер"; //!
    }
}
