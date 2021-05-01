using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpriteManager")]
public class SpriteManager : ScriptableObject
{
    [SerializeField] private List<SpritePack> _spritePacks;

    [SerializeField] private SpritePack EasterEgg;

    //[HideInInspector]
    public SpritePack CurrentPack;

    private int _currentIndex = 0;

    public void OnEnable()
    {
        string name = Prefs.SpritePack;
        for (int i = 0; i < _spritePacks.Count; i++)
        {
            if (_spritePacks[i].Name == name) _currentIndex = i;
        }
        CurrentPack = _spritePacks[_currentIndex];
    }


    public void Prev()
    {
        _currentIndex = (_currentIndex - 1 + _spritePacks.Count) % _spritePacks.Count;
        CurrentPack = _spritePacks[_currentIndex];
        Prefs.SpritePack = CurrentPack.Name;
    }

    public void Next()
    {
        _currentIndex = (_currentIndex + 1) % _spritePacks.Count;
        CurrentPack = _spritePacks[_currentIndex];
        Prefs.SpritePack = CurrentPack.Name;
    }

    public void SetEasterEgg()
    {
        CurrentPack = EasterEgg;
    }
}
