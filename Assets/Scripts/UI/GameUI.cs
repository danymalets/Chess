using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(MoveFinder))]
public class GameUI : MonoBehaviour, IUI
{
    [Header("Scene objects")]
    [SerializeField] private Board _board;
    [SerializeField] private Button _undo;
    [SerializeField] private Text _title;
    [SerializeField] private Text _status;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [Header("Windows")]
    [SerializeField] private PopUpWindow _simpleExitWindow;
    [SerializeField] private PopUpWindow _exitWindowWithSaving;

    [Header("Sounds")]
    [SerializeField] private AudioSource _gameOver;
    
    private AsyncOperation _loading;

    private GameController _gameController;

    private void Start()
    {
        if (!MenuUI.IsGameLoaded)
        {
            SceneManager.LoadScene(Scenes.MainMenu);
            return;
        }
        
        _gameController = SceneTransition.GameController;
        if (!(_gameController is IUndoable))
        {
            Destroy(_undo.gameObject);
        }
        _animator.Play(UIAnimations.RightOpening);
    }

    public void OnOpeningAnimationPlayed()
    {
        _gameController.Init(this, _board, GetComponent<Timer>(), GetComponent<MoveFinder>());
    }

    public void SetTitle(string title)
    {
        _title.text = title;
    }

    public void SetStatus(string newStatus)
    {
        _status.text = newStatus;
    }


    public void OnUndoButtonClicked()
    {
        if (_loading != null) return;

        if (_gameController is IUndoable undoable)
        {
            undoable.Undo();
        }
    }

    public void OnExitButtonClicked()
    {
        if (_loading != null) return;

        if (_gameController.QuickExit())
        {
            _gameController.Finish();
            _loading = SceneManager.LoadSceneAsync(Scenes.MainMenu);
            _animator.Play(UIAnimations.RightClosing);
            _loading.allowSceneActivation = false;
        }
        else
        {
            if (_gameController is IStorable)
            {
                _exitWindowWithSaving.Open();
            }
            else
            {
                _simpleExitWindow.Open();
            }
        }
    }

    public void OnConfirmExitButtonClicked()
    {
        CloseWindow();
        Exit();
    }

    private void Exit()
    {
        if (_loading != null) return;

        _gameController.Finish();
        _loading = SceneManager.LoadSceneAsync(Scenes.MainMenu);

        _animator.Play(UIAnimations.RightClosing);

        _loading.allowSceneActivation = false;
    }

    public void OnExitWithSavingButtonClicked()
    {
        _gameController.Save();
        OnConfirmExitButtonClicked();
    }
    public void OnClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }

    public void CloseWindow()
    {
        if (_gameController is IStorable)
        {
            _exitWindowWithSaving.Close();
        }
        else
        {
            _simpleExitWindow.Close();
        }
    }

    public void OnApplicationQuit()
    {
        if (_loading == null)
        {
            if (_gameController is IStorable)
            {
                _gameController.Save();
            }
            _gameController.Finish();
        }
    }

    public void PlayGameOver()
    {
        _gameOver.PlayDelayed(0.5f);
    }
}
