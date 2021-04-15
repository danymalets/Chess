using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpriteManager")]
public class SpriteManager : ScriptableObject
{
    [SerializeField] private List<SpritePack> _spritePacks;

    public SpritePack CurrentPack;

    public SpritePack Prev()
    {
        int _currentInd = _spritePacks.IndexOf(CurrentPack);
        _currentInd = (_currentInd - 1 + _spritePacks.Count) % _spritePacks.Count;
        return CurrentPack = _spritePacks[_currentInd];
    }

    public SpritePack Next()
    {
        int _currentInd = _spritePacks.IndexOf(CurrentPack);
        _currentInd = (_currentInd + 1) % _spritePacks.Count;
        return CurrentPack = _spritePacks[_currentInd];
    }
}
