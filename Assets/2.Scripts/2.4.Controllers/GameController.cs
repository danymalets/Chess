using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameController
{
    public static GameController Singleton { get; set; }

    public Game Game { get; private set; }

    protected GameUI _ui;
    protected Board _board;

    public GameController()
    {
        Game = new Game();
    }

    public GameController(List<string> moves)
    {
        Game = new Game(moves);
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
        if (this is ISave)
        {
            Prefs.AddGameController(this);
        }
    }

    public static GameController FromGameModel(GameModel gameModel)
    {
        if (gameModel.Title == typeof(SinglePlayer).Name)
        {
            return new SinglePlayer(
                (Color)gameModel.PlayerColor,
                gameModel.Level,
                gameModel.Moves);
        }
        else if (gameModel.Title == typeof(SinglePlayer).Name)
        {
            return new TwoPlayers(new TimeSpan(0, 0, gameModel.MoveDuration));
        }
        else
        {
            throw new ArgumentException($"Type {gameModel.Title} not found");
        }
    }
}
