using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameController
{
    public static GameController Singleton { get; set; }

    protected UI _ui;
    protected Board _board;

    public Game Game;

    public GameController()
    {
        Game = new Game();
    }

    public GameController(List<string> moves)
    {
        Game = new Game(moves);
    }

    public virtual void Init(UI ui, Board board)
    {
        _ui = ui;
        _board = board;
        _ui.SetGameTitle(ToString());
    }

    public virtual void Finish()
    {
        _board.Clear();
        _ui.Clear();
        if (this is ISave)
        {
            Prefs.AddGameModel(new GameModel(this));
        }
    }

    public static GameController GetGameController(GameModel gameModel)
    {
        if (gameModel.Type == "white")
        {
            return new SinglePlayer(Color.White, gameModel.Level, gameModel.Moves);
        }
        else if (gameModel.Type == "black")
        {
            return new SinglePlayer(Color.Black, gameModel.Level, gameModel.Moves);
        }
        else if (gameModel.Type == "two")
        {
            return new TwoPlayers(new TimeSpan(0, 0, gameModel.MoveDuration), gameModel.Moves);
        }
        else
        {
            throw new ArgumentException($"Type {gameModel.Type} not found");
        }
    }
}
