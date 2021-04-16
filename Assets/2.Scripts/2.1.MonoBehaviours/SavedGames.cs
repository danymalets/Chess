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
        if (!MenuUI.IsGameLoaded) { SceneManager.LoadScene("Menu"); return; }
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
            

            if (controller is SinglePlayer singlePlayer)
            {
                if (singlePlayer.PlayerColor == Color.White)
                {
                    savedGame.Init(
                        controller.Game.Position,
                        $"За белых\n{singlePlayer.Level} уровень",
                        $"{gameModel.Date}\n{gameModel.Time}");
                }
                else
                {
                    savedGame.Init(
                        controller.Game.Position,
                        $"За чёрных\n{singlePlayer.Level} уровень",
                        $"{gameModel.Date}\n{gameModel.Time}");
                    savedGame.Rotate();
                }
            }
            else if (controller is TwoPlayers twoPlayers)
            {
                int min = twoPlayers.MoveDuration.Minutes;
                int sec = twoPlayers.MoveDuration.Seconds;
                string duration;
                if (min == 0) duration = $"{sec} с.";
                else if (sec == 0) duration = $"{min} м.";
                else duration = $"{min} м. {sec} с.";
                savedGame.Init(
                    controller.Game.Position,
                    $"Два игрока\nход: {duration}",
                    $"{gameModel.Date}\n{gameModel.Time}");
            }
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
