using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Prefs
{
    private const string SPRITE_PACK = "SpritePack";
    private const string ROOM_NAME = "RoomName";
    private const string LEVEL_SLIDER = "LevelSlider";
    private const string MOVE_DURATION_SLIDER = "MoveDurationSlider";
    private const string HISTORY = "History";

    public static string SpritePack
    {
        get
        {
            return PlayerPrefs.GetString(SPRITE_PACK);
        }
        set
        {
            PlayerPrefs.SetString(SPRITE_PACK, value);
        }
    }

    public static string RoomName
    {
        get
        {
            if (!PlayerPrefs.HasKey(ROOM_NAME))
            {
                RoomName = Random.Range(0, 10).ToString()
                         + Random.Range(0, 10).ToString()
                         + Random.Range(0, 10).ToString();
            }
            return PlayerPrefs.GetString(ROOM_NAME);
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
                return 4;
            }
        }
        set
        {
            PlayerPrefs.SetInt(LEVEL_SLIDER, value);
        }
    }

    public static int MoveDurationSlider
    {
        get
        {
            if (PlayerPrefs.HasKey(MOVE_DURATION_SLIDER))
            {
                return PlayerPrefs.GetInt(MOVE_DURATION_SLIDER);
            }
            else
            {
                return 4;
            }
        }
        set
        {
            PlayerPrefs.SetInt(MOVE_DURATION_SLIDER, value);
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
    
    public static void AddGameController(IStorable controller)
    {
        AddStoredGame(controller.GetStoredGame());
    }

    public static List<StoredGame> GetStoredGames()
    {
        if (PlayerPrefs.HasKey(HISTORY))
        {
            string json = PlayerPrefs.GetString(HISTORY);
            History history = JsonUtility.FromJson<History>(json);
            return history.StoredGames;
        }
        else
        {
            return new List<StoredGame>();
        }
    }

    public static void SetStoredGames(List<StoredGame> StoredGames)
    {
        History history = new History(StoredGames);
        string json = JsonUtility.ToJson(history);
        PlayerPrefs.SetString(HISTORY, json);
    }

    private static void AddStoredGame(StoredGame StoredGame)
    {
        List<StoredGame> StoredGames = GetStoredGames();
        StoredGames.Add(StoredGame);
        SetStoredGames(StoredGames);
    }
}
