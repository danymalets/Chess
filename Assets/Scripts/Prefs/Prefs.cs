using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Prefs
{
    private const string SpritePackString = "SpritePack";
    private const string RoomNameString = "RoomName";
    private const string LevelSliderString = "LevelSlider";
    private const string MoveDurationSliderString = "MoveDurationSlider";
    private const string HistoryString = "History";

    public static string SpritePack
    {
        get => PlayerPrefs.GetString(SpritePackString, "default");
        set => PlayerPrefs.SetString(SpritePackString, value);
    }

    public static string RoomName
    {
        get
        {
            if (PlayerPrefs.HasKey(RoomNameString))
            {
                return PlayerPrefs.GetString(RoomNameString);
            }
            else
            {
                string name = Random.Range(0, 10).ToString()
                            + Random.Range(0, 10).ToString()
                            + Random.Range(0, 10).ToString();
                PlayerPrefs.SetString(RoomNameString, name);
                return name;
            }
        }
        set => PlayerPrefs.SetString(RoomNameString, value);
    }

    public static int LevelSlider
    {
        get => PlayerPrefs.GetInt(LevelSliderString, 4);
        set => PlayerPrefs.SetInt(LevelSliderString, value);
    }

    public static int MoveDurationSlider
    {
        get => PlayerPrefs.GetInt(MoveDurationSliderString, 4);
        set => PlayerPrefs.SetInt(MoveDurationSliderString, value);
    }

    
    
    public static void AddGameController(IStorable controller)
    {
        AddStoredGame(controller.GetStoredGame());
    }

    public static List<StoredGame> GetStoredGames()
    {
        if (PlayerPrefs.HasKey(HistoryString))
        {
            string json = PlayerPrefs.GetString(HistoryString);
            History history = JsonUtility.FromJson<History>(json);
            return history.StoredGames;
        }
        else
        {
            return new List<StoredGame>();
        }
    }

    public static void SetStoredGames(List<StoredGame> storedGames)
    {
        History history = new History(storedGames);
        string json = JsonUtility.ToJson(history);
        PlayerPrefs.SetString(HistoryString, json);
    }

    private static void AddStoredGame(StoredGame storedGame)
    {
        List<StoredGame> storedGames = GetStoredGames();
        storedGames.Add(storedGame);
        SetStoredGames(storedGames);
    }
}

[Serializable]
public struct History
{
    public List<StoredGame> StoredGames;

    public History(List<StoredGame> storedGames)
    {
        StoredGames = storedGames;
    }
}
