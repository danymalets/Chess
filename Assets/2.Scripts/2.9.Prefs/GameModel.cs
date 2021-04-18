using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameModel
{
    public string Title;
    public int PlayerColor;
    public int Level;
    public string Date;
    public string Time;
    public int MoveDuration;
    public List<string> Moves;

    public GameModel(
        string title,
        int playerColor,
        int level,
        int moveDuration,
        List<string> moves,
        string date,
        string time)
    {
        Title = title;
        PlayerColor = playerColor;
        Level = level;
        MoveDuration = moveDuration;
        Moves = moves;
        Date = date;
        Time = time;
    }
}
