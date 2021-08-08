using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameController
{
    public static GameController MainController { get; set; }
    public static int TotalMovesCount { get; set; }

    public Game _game;

    protected GameUI _ui;
    protected Board _board;

    public GameController()
    {
        _game = new Game();
    }

    public virtual void Init(GameUI ui, Board board)
    {
        _ui = ui;
        _board = board;
    }

    public virtual bool QuickExit() => _game == null || _game.GetPositionsCount() == 1;

    protected void GameOver()
    {
        _ui.PlayGameOver();
    }

    public virtual void Finish()
    {
        TotalMovesCount = _game.TotalMovesCount;
        _board.Clear();
        _ui.Clear();
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
