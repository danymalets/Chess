using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayer : GameController, ISave
{
    public Color PlayerColor { get; private set; }
    public int Level { get; private set; }

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

    public override void Init(UI ui, Board board, Field field)
    {
        base.Init(ui, board, field);

        _board.InitPieces(Game.Position);
        _field.InitSquares(_board);

        _field.MoveSelected += OnUserMoveSelected;
        _ui.MoveFound += OnMoveFound;

        _ui.ChangeStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            if (PlayerColor == Game.Position.WhoseMove)
            {
                _field.MoveShown += OnUserMoveShown;
                _field.EnableMoves();
            }
            else
            {
                _field.MoveShown += OnRivalMoveShown;
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
        _field.MoveShown -= OnUserMoveShown;

        _ui.ChangeStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            _ui.StartSearchMove(Game, Level);
            _field.MoveShown += OnRivalMoveShown;
        }
    }

    private void OnMoveFound(Move move)
    {
        Game.MakeMove(move);
        _field.ShowMove(move);
    }

    private void OnRivalMoveShown()
    {
        _field.MoveShown -= OnRivalMoveShown;

        _ui.ChangeStatus(Game.GetStatus());
        if (!Game.IsEnd)
        {
            _field.EnableMoves();
            _field.MoveShown += OnUserMoveShown;
        }
    }

    public void Undo()
    {
        if (!Game.IsUndoAllowed()) return;

        _ui.Clear();
        _field.Clear();
        Game.Undo();

        if (Game.Position.WhoseMove != PlayerColor) Game.Undo();

        _board.InitPieces(Game.Position);
        _field.Init(_board);
        _field.EnableMoves();

        _ui.ChangeStatus(Game.GetStatus());

        _field.MoveShown -= OnUserMoveShown;
        _field.MoveShown -= OnRivalMoveShown;

        _field.MoveShown += OnUserMoveShown;
    }

    public override string ToString()
    {
        return $"Один игрок, {Level} уровень, за {(PlayerColor == Color.White ? "белых" : "чёрных")}";
    }
}
