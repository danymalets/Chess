using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Prefs
{
    const string ROOM_NAME = "RoomName";
    const string LEVEL_SLIDER = "LevelSlider";
    const string DURATION_SLIDER = "DurationSlider";
    const string HISTORY = "History";

    public static string RoomName
    {
        get
        {
            if (PlayerPrefs.HasKey(ROOM_NAME))
            {
                return PlayerPrefs.GetString(ROOM_NAME);
            }
            else
            {
                return Random.Range(0, 10).ToString()
                    + Random.Range(0, 10).ToString()
                    + Random.Range(0, 10).ToString();
            }
        }
        set
        {
            PlayerPrefs.SetString(ROOM_NAME, value);
        }
    }

    public static int LevelSlider
    {
        get
        {
            if (PlayerPrefs.HasKey(LEVEL_SLIDER))
            {
                return PlayerPrefs.GetInt(LEVEL_SLIDER);
            }
            else
            {
                return 3;
            }
        }
        set
        {
            PlayerPrefs.SetInt(LEVEL_SLIDER, value);
        }
    }

    public static int DurationSlider
    {
        get
        {
            if (PlayerPrefs.HasKey(DURATION_SLIDER))
            {
                return PlayerPrefs.GetInt(DURATION_SLIDER);
            }
            else
            {
                return 3;
            }
        }
        set
        {
            PlayerPrefs.SetInt(DURATION_SLIDER, value);
        }
    }

    [Serializable]
    public struct History
    {
        public List<GameModel> GameModels;

        public History(List<GameModel> gameModels)
        {
            GameModels = gameModels;
        }
    }

    public static List<GameController> GetGameControllers()
    {
        List<GameModel> models = GetGameModels();
        List<GameController> controllers = new List<GameController>();
        foreach (GameModel model in models)
        {
            controllers.Add(GameController.FromGameModel(model));
        }
        return controllers;
    }

    public static void SetGameControllers(List<GameController> controllers)
    {
        List<GameModel> models = new List<GameModel>();
        foreach (GameController controller in controllers)
        {
            models.Add(((ISave)controller).GetGameModel());
        }
        SetGameModels(models);
    }

    public static void AddGameController(GameController controller)
    {
        AddGameModel(((ISave)controller).GetGameModel());
    }

    private static List<GameModel> GetGameModels()
    {
        if (PlayerPrefs.HasKey(HISTORY))
        {
            string json = PlayerPrefs.GetString(HISTORY);
            History history = JsonUtility.FromJson<History>(json);
            return history.GameModels;
        }
        else
        {
            return new List<GameModel>();
        }
    }

    private static void SetGameModels(List<GameModel> gameModels)
    {
        History history = new History(gameModels);
        string json = JsonUtility.ToJson(history);
        PlayerPrefs.SetString(HISTORY, json);
    }

    private static void AddGameModel(GameModel gameModel)
    {
        List<GameModel> gameModels = GetGameModels();
        gameModels.Add(gameModel);
        SetGameModels(gameModels);
    }
}
