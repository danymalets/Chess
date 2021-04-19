using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayer : GameController, ISave
{
    public static string Title = "SinglePlayer";

    public Color PlayerColor { get; private set; }
    public int Level { get; private set; }

    public string StartDate { get; private set; }
    public string StartTime { get; private set; }

    public SinglePlayer(Color playerColor, int level) : base()
    {
        PlayerColor = playerColor;
        Level = level;
    }

    public SinglePlayer(Color playerColor, int level, List<string> moves) : base(moves)
    {
        PlayerColor = playerColor;
        Level = level;
    }

    public override void Init(GameUI ui, Board board)
    {
        DateTime now = DateTime.Now;
        StartDate = now.ToString("dd.MM.yyyy");
        StartTime = now.ToString("HH:mm");

        base.Init(ui, board);

        _ui.SetTitle($"Один игрок, {Level} уровень, за {(PlayerColor == Color.White ? "белых" : "чёрных")}");

        _board.InitPieces(Game.Position);

        _board.MoveSelected += OnUserMoveSelected;
        _ui.MoveFound += OnMoveFound;

        _ui.SetStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            if (PlayerColor == Game.Position.WhoseMove)
            {
                _board.MoveShown += OnUserMoveShown;
                _board.EnableMoves();
            }
            else
            {
                _board.MoveShown += OnRivalMoveShown;
                _ui.StartSearchMove(Game, Level);
            }

            if (PlayerColor == Color.Black)
            {
                _board.SetRotation(180);
            }
        }
    }

    private void OnUserMoveSelected(Move move)
    {
        Game.MakeMove(move);
    }

    private void OnUserMoveShown()
    {
        _board.MoveShown -= OnUserMoveShown;

        _ui.SetStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            _ui.StartSearchMove(Game, Level);
            _board.MoveShown += OnRivalMoveShown;
        }
    }

    private void OnMoveFound(Move move)
    {
        Game.MakeMove(move);
        _board.ShowMove(move);
    }

    private void OnRivalMoveShown()
    {
        _board.MoveShown -= OnRivalMoveShown;

        _ui.SetStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            _board.EnableMoves();
            _board.MoveShown += OnUserMoveShown;
        }
    }

    public void Undo()
    {
        if (!Game.IsUndoAllowed()) return;

        _ui.Clear();
        _board.Clear();
        Game.Undo();

        if (Game.Position.WhoseMove != PlayerColor) Game.Undo();

        _board.InitPieces(Game.Position);
        _board.EnableMoves();

        _ui.SetStatus(Game.GetStatus());

        _board.MoveShown -= OnUserMoveShown;
        _board.MoveShown -= OnRivalMoveShown;

        _board.MoveShown += OnUserMoveShown;
    }

    public GameModel GetGameModel()
    {
        return new GameModel(
            Title,
            (int)PlayerColor,
            Level,
            -1,
            Game.StringMoves,
            StartDate,
            StartTime);
    }
}
