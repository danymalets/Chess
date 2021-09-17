using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteConfirmation : MonoBehaviour
{
    private SavedGamePanel _savedGame;

    public event Action<SavedGamePanel> ConfirmDeleteButtonClicked;

    public void Open(SavedGamePanel savedGame)
    {
        _savedGame = savedGame;
        gameObject.SetActive(true);
    }

    public void OnConfirmButtonClicked()
    {
        ConfirmDeleteButtonClicked?.Invoke(_savedGame);
        gameObject.SetActive(false);
    }

    public void OnCancelButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
