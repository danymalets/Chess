using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameModel
{
    public string Type;
    public int Level;
    public string Date;
    public string Time;
    public int MoveDuration;
    public List<string> Moves;

    public GameModel(GameController controller)
    {
        if (controller is SinglePlayer singlePlayer)
        {
            Type = singlePlayer.PlayerColor == Color.White ? "white" : "black";
            Level = singlePlayer.Level;
            MoveDuration = -1;
        }
        else if (controller is TwoPlayers twoPlayers)
        {
            Type = "two";
            Level = -1;
            MoveDuration = (int)twoPlayers.MoveDuration.TotalSeconds;
        }
        else
        {
            throw new ArgumentException($"Cannot save controller {controller.GetType()}");
        }
        DateTime now = DateTime.Now;
        Date = now.ToString("dd.MM.yyyy");
        Time = now.ToString("hh:mm");
        Moves = controller.Game.StringMoves;
        Debug.Log(Moves.Count);
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

public static class Prefs
{
    const string HISTORY = "History";

    public static List<GameModel> GetGameModels()
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

    public static void SetGameModels(List<GameModel> gameModels)
    {
        History history = new History(gameModels);
        string json = JsonUtility.ToJson(history);
        PlayerPrefs.SetString(HISTORY, json);
    }

    public static void AddGameModel(GameModel gameModel)
    {
        List<GameModel> gameModels = GetGameModels();
        gameModels.Add(gameModel);
        SetGameModels(gameModels);
    }
}
