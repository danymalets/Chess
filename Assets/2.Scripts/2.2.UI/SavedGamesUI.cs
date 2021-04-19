using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavedGamesUI : MonoBehaviour
{
    [SerializeField] private Animation _animation;

    [SerializeField] private Transform _savedGamesTransform;

    [SerializeField] private GameObject _savedGamePanelPrefab;

    private List<GameController> gameControllers;

    private AsyncOperation _loading;

    private void Start()
    {
        _animation.Play();
#if DEBUG
        if (!MenuUI.IsGameLoaded) { SceneManager.LoadScene("Menu"); return; }
#endif
        gameControllers = Prefs.GetGameControllers();
        foreach (GameController controller in gameControllers)
        {
            SavedGame savedGame = Instantiate(_savedGamePanelPrefab, _savedGamesTransform).GetComponent<SavedGame>();
            savedGame.Init(controller);
            savedGame.SavedGameDeleted += OnSavedGameDeleted;
            savedGame.SavedGameSelected += OnSavedGameSelected;
        }
    }

    private void OnSavedGameDeleted(GameController controller)
    {
        gameControllers.Remove(controller);
        Prefs.SetGameControllers(gameControllers);
    }

    private void OnSavedGameSelected(GameController controller)
    {
        gameControllers.Remove(controller);
        Prefs.SetGameControllers(gameControllers);
        GameController.Singleton = controller;

        _animation.Play("SavedGamesLeftClosing");
        _loading = SceneManager.LoadSceneAsync("Game");
        _loading.allowSceneActivation = false;
    }

    public void OnButtonExitClicked()
    {
        _animation.Play("SavedGamesRightClosing");
        _loading = SceneManager.LoadSceneAsync("Menu");
        _loading.allowSceneActivation = false;
    }

    public void OnLeftClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }

    public void OnRightClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }
}
