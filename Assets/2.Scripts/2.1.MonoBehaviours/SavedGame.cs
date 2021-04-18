using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavedGame : MonoBehaviour
{
    [SerializeField] private BoardImage _board;

    [SerializeField] private Text _title;
    [SerializeField] private Text _info;
    [SerializeField] private Text _dateTime;

    private GameController _gameController;

    public event Action<GameController> SavedGameDeleted;
    public event Action<GameController> SavedGameSelected;

    public void Init(GameController controller)
    {
        _gameController = controller;

        _board.InitBoard();
        _board.InitPieces(controller.Game.Position);

        if (controller is SinglePlayer singlePlayer)
        {
            _title.text = $"Один игрок\nза {(singlePlayer.PlayerColor)}";
        }
        else if (controller is TwoPlayers twoPlayers)
        {
            
        }
        else
        {
            throw new ArgumentException($"{controller.GetType()} ???");
        }
    }

    public void Rotate()
    {
        _board.SetRotation(180);
    }

    public void OnDeleteButtonClicked()
    {
        Destroy(gameObject);
        SavedGameDeleted?.Invoke(_gameController);
    }

    public void OnSelectionButtonClicked()
    {
        SavedGameSelected?.Invoke(_gameController);
    }
}
