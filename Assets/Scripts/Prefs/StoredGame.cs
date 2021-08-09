using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoredGame
{
    public int Type;
    public int PlayerColor;
    public int Level;
    public int MoveDuration;
    public List<string> Moves;

    public string Date;
    public string Time;

    public StoredGame(StoredType type, List<string> moves)
    {
        Type = (int)type;
        Moves = moves;

        DateTime now = DateTime.Now;
        Date = now.ToString("dd.MM.yyyy");
        Time = now.ToString("HH:mm");
    }

    public GameController ToGameController()
    {
        switch ((StoredType)Type)
        {
            case StoredType.SinglePlayer:
                return new SinglePlayer((Color)PlayerColor, Level, new Game(Moves));
            case StoredType.TwoPlayers:
                return new TwoPlayers(new TimeSpan(0, 0, MoveDuration), new Game(Moves));
            default:
                throw new InvalidOperationException($"Failed to get {Type} Controller");
        }
    }

    public string GetTitle()
    {
        switch ((StoredType)Type)
        {
            case StoredType.SinglePlayer:
                return "Один игрок";
            case StoredType.TwoPlayers:
                return "Два игрока";
            default:
                throw new InvalidOperationException($"Failed to find {Type} Controller");
        }
    }

    public string GetInfo()
    {
        switch ((StoredType)Type)
        {
            case StoredType.SinglePlayer:
                return $"{Level} уровень, за {((Color)PlayerColor == Color.White ? "белых" : "чёрных")}";
            case StoredType.TwoPlayers:
                if (MoveDuration >= 60) return $"ход {MoveDuration / 60} мин.";
                else return $"ход {MoveDuration} сек.";
            default:
                throw new InvalidOperationException($"Failed to find {Type} Controller");
        }
    }

    public string GetDateTime()
    {
        return $"{Date} – {Time}";
    }

    public int GetRotation()
    {
        switch ((StoredType)Type)
        {
            case StoredType.SinglePlayer:
                return ((Color)PlayerColor == Color.White ? 0 : 180);
            case StoredType.TwoPlayers:
                return 0;
            default:
                throw new InvalidOperationException($"Failed to find {Type} Controller");
        }
    }
}
