using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavedGame : MonoBehaviour
{
    [SerializeField] private Board _board;

    [SerializeField] private Text _type;
    [SerializeField] private Text _dateTime;

    public GameModel GameModel;

    public event Action<GameModel> SavedGameDeleted;
    public event Action<GameModel> SavedGameSelected;

    public void InitBoard(Position position, bool rotate)
    {
        _board.InitBoard();
        _board.InitPieces(position);
        _board.SetRotation(rotate ? 180 : 0);
    }

    public void OnDeleteButtonClicked()
    {
        Destroy(gameObject);
        SavedGameDeleted?.Invoke(GameModel);
    }

    public void OnSelectionButtonClicked()
    {
        SavedGameSelected?.Invoke(GameModel);
    }
}
