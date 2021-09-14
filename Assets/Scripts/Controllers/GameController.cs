using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameController
{
    protected Game _game;

    protected GameUI _ui;
    protected Board _board;
    protected Timer _timer;
    protected MoveFinder _moveFinder;

    protected GameController()
    {
        _game = new Game();
    }

    public void Init(GameUI ui, Board board, Timer timer, MoveFinder moveFinder)
    {
        _ui = ui;
        _board = board;
        _timer = timer;
        _moveFinder = moveFinder;
        InternalInit();
    }

    protected virtual void InternalInit() {}

    public virtual bool QuickExit() => _game == null || _game.GetPositionsCount() == 1;

    protected void GameOver()
    {
        _ui.PlayGameOver();
    }

    public virtual void Finish()
    {
        _board.Clear();
        _timer.Clear();
    }

    public void Save()
    {
        if (this is IStorable controller)
        {
            Prefs.AddGameController(controller);
        }
        else
        {
            throw new InvalidOperationException($"Failed to save {GetType()}");
        }
    }
}
