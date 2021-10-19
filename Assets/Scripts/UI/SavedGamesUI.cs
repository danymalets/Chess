using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavedGamesView : MonoBehaviour, IView
{
    [SerializeField] private DeleteConfirmation _deleteConfirmation;

    [SerializeField] private Animator _animator;

    [SerializeField] private Transform _savedGamesTransform;

    [SerializeField] private GameObject _savedGamePanelPrefab;

    private List<StoredGame> _storedGames;

    private AsyncOperation _loading;

    private void Start()
    {
#if DEBUG
        if (!MenuView.IsGameLoaded) { SceneManager.LoadScene(Scenes.MainMenu); return; }
#endif
        _animator.Play(UIAnimations.RightOpening);
        
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

    public void OnOpeningAnimationPlayed()
    {
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
        SceneTransition.GameController = storedGame.ToGameController();

        _animator.Play(UIAnimations.LeftClosing);
        _loading = SceneManager.LoadSceneAsync(Scenes.Game);
        _loading.allowSceneActivation = false;
    }

    public void OnButtonExitClicked()
    {
        _animator.Play(UIAnimations.RightClosing);
        _loading = SceneManager.LoadSceneAsync(Scenes.MainMenu);
        _loading.allowSceneActivation = false;
    }

    public void OnClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }
}
