using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavedGame : MonoBehaviour
{
    [SerializeField] private BoardImage _board;

    [SerializeField] private Text _type;
    [SerializeField] private Text _dateTime;

    public GameModel GameModel;

    public event Action<GameModel> SavedGameDeleted;
    public event Action<GameModel> SavedGameSelected;

    public void Init(Position position, string type, string dateTime)
    {
        _board.InitBoard();
        _board.InitPieces(position);
        _type.text = type;
        _dateTime.text = dateTime;
    }

    public void Rotate()
    {
        _board.SetRotation(180);
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
