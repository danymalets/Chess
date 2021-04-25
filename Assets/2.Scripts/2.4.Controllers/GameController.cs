using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameController
{
    public static GameController Singleton { get; set; }

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

    public virtual void Finish()
    {
        _board.Clear();
        _ui.Clear();
        if (this is IStorable save)
        {
            Prefs.AddGameController(save);
        }
    }
}
