using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavedGames : MonoBehaviour
{
    [SerializeField] private Animation _animation;

    [SerializeField] private Transform _savedGamesTransform;

    [SerializeField] private GameObject _savedGamePanelPrefab;

    private List<GameModel> gameModels;

    private AsyncOperation _loading;

    private void Start()
    {
        _animation.Play();
#if DEBUG
        if (!Main.IsGameLoaded) { SceneManager.LoadScene("Menu"); return; }
#endif
        gameModels = Prefs.GetGameModels();
        foreach (GameModel gameModel in gameModels)
        {
            GameController controller = GameController.GetGameController(gameModel);
            GameObject _savedGamePanel = Instantiate(_savedGamePanelPrefab, _savedGamesTransform);
            SavedGame savedGame = _savedGamePanel.GetComponent<SavedGame>();
            savedGame.GameModel = gameModel;
            savedGame.SavedGameDeleted += OnSavedGameDeleted;
            savedGame.SavedGameSelected += OnSavedGameSelected;
            savedGame.InitBoard(controller.Game.Position);

            if (controller is SinglePlayer singlePlayer && singlePlayer.PlayerColor == Color.Black)
                savedGame.Rotate();
        }
    }

    private void OnSavedGameDeleted(GameModel gameModel)
    {
        gameModels.Remove(gameModel);
        Prefs.SetGameModels(gameModels);
    }

    private void OnSavedGameSelected(GameModel gameModel)
    {
        gameModels.Remove(gameModel);
        Prefs.SetGameModels(gameModels);
        GameController controller = GameController.GetGameController(gameModel);
        GameController.Singleton = controller;

        _animation.Play("SavedLeftClosing");
        _loading = SceneManager.LoadSceneAsync("Game");
        _loading.allowSceneActivation = false;
    }

    public void OnButtonExitClicked()
    {
        _animation.Play("SavedRightClosing");
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
