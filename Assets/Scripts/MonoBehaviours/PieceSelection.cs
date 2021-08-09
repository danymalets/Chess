using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceSelection : MonoBehaviour
{
    [SerializeField] private SpriteManager _spriteManager;

    [SerializeField] private Animation _animation;

    [SerializeField] private Image _knight;
    [SerializeField] private Image _bishop;
    [SerializeField] private Image _rook;
    [SerializeField] private Image _queen;

    public event Action<Type> PieceSelected;

    private SpritePack _spritePack;

    public void Open(Color color)
    {
        _spritePack = _spriteManager.CurrentPack;

        _knight.sprite = _spritePack.GetPiece(color, typeof(Knight));
        _bishop.sprite = _spritePack.GetPiece(color, typeof(Bishop));
        _rook.sprite = _spritePack.GetPiece(color, typeof(Rook));
        _queen.sprite = _spritePack.GetPiece(color, typeof(Queen));

        gameObject.SetActive(true);

        _animation.Play();
    }

    public void KnightSelected()
    {
        PieceSelected?.Invoke(typeof(Knight));
        Close();
    }

    public void BishopSelected()
    {
        PieceSelected?.Invoke(typeof(Bishop));
        Close();
    }

    public void RookSelected()
    {
        PieceSelected?.Invoke(typeof(Rook));
        Close();
    }

    public void QueenSelected()
    {
        PieceSelected?.Invoke(typeof(Queen));
        Close();
    }

    public bool IsActive() => gameObject.activeSelf;

    public void Close() => gameObject.SetActive(false);
}
