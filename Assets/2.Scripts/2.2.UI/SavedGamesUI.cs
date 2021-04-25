using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavedGamesUI : MonoBehaviour
{
    [SerializeField] private DeleteConfirmation _deleteConfirmation;

    [SerializeField] private Animation _animation;

    [SerializeField] private Transform _savedGamesTransform;

    [SerializeField] private GameObject _savedGamePanelPrefab;

    private List<StoredGame> _storedGames;

    private AsyncOperation _loading;

    private void Start()
    {
        _animation.Play();
#if DEBUG
        if (!MenuUI.IsGameLoaded) { SceneManager.LoadScene("Menu"); return; }
#endif
        _storedGames = Prefs.GetStoredGames();
        foreach (StoredGame storedGame in _storedGames)
        {
            SavedGamePanel savedGame = Instantiate(_savedGamePanelPrefab, _savedGamesTransform).GetComponent<SavedGamePanel>();
            savedGame.Init(storedGame);
            savedGame.DeleteButtonClicked += OnDeleteButtonClicked;
            savedGame.RunButtonClicked += OnRunButtonClicked;
        }

        _deleteConfirmation.ConfirmDeleteButtonClicked += OnConfirmDeleteButtonClicked;
    }

    private void OnDeleteButtonClicked(SavedGamePanel savedGame)
    {
        _deleteConfirmation.Open(savedGame);
    }

    private void OnConfirmDeleteButtonClicked(SavedGamePanel savedGame)
    {
        savedGame.Destroy();
        _storedGames.Remove(savedGame.StoredGame);
        Prefs.SetStoredGames(_storedGames);
    }

    private void OnRunButtonClicked(StoredGame storedGame)
    {
        _storedGames.Remove(storedGame);
        Prefs.SetStoredGames(_storedGames);
        GameController.Singleton = storedGame.ToGameController();

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
