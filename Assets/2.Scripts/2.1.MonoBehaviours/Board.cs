using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : BoardImage
{
    [Header("Highlight Prefabs")]
    [SerializeField] private GameObject _selectedPieceHighlightPrefab;
    [SerializeField] private GameObject _possibleMoveHighlightPrefab;

    [Header("Piece Movement")]
    [SerializeField] private float _movementDuration;
    [Range(0f, 1f)]
    [SerializeField] private float _movementFinalPart;

    [Header("Windows")]
    [SerializeField] private PieceSelection _pieceSelection;

    [Header("Sounds")]
    [SerializeField] private AudioSource _snap;
    [Range(0f, 1f)]
    [SerializeField] private float _maxStereo;

    private GameObject _selectedPieceHighlight;
    private List<GameObject> _possibleMoveHighlights = new List<GameObject>();

    private bool _isMoveAllowed = false;

    private bool _isSquareSelected = false;
    private Vector2Int _selectedSquare;

    private List<Move>[,] _moves = new List<Move>[Position.SIZE, Position.SIZE];

    public event Action<Move> MoveSelected;
    public event Action MoveShown;

    private List<Move> _promotionMoves;

    private void Start()
    {
        base.InitBoard();

        _pieceSelection.PieceSelected += OnPieceSelected;

        for (int x = 0; x < Position.SIZE; x++)
        {
            for (int y = 0; y < Position.SIZE; y++)
            {
                _moves[x, y] = new List<Move>();

                Tile tile = _tiles[x, y].GetComponent<Tile>();
                Vector2Int square = new Vector2Int(x, y);
                tile.Click += () => OnClick(square);
            }
        }
    }

    public void EnableMoves()
    {
        _isMoveAllowed = true;
    }

    public void DisableMoves()
    {
        if (_isSquareSelected) Deselect();
        if (_pieceSelection.IsActive()) _pieceSelection.Close();
        _isMoveAllowed = false;
    }

    public void ShowMove(Move move) => StartCoroutine(Movement(move));

    private IEnumerator Movement(Move move)
    {
        if (_checkHighlight != null) Destroy(_checkHighlight);

        bool finalPart = false;
        Transform piece = _pieces[move.SourceSquare.x, move.SourceSquare.y];

        Transform source = _tiles[move.SourceSquare.x, move.SourceSquare.y];
        Transform target = _tiles[move.TargetSquare.x, move.TargetSquare.y];

        Vector2 sourcePosition = source.position;
        Vector2 targetPosition = target.position;

        piece.SetParent(_boardTransform);

        for (float elapsedTime = 0;
            elapsedTime < _movementDuration || !finalPart;
            elapsedTime += Time.deltaTime)
        {
            piece.position = Vector2.Lerp(
                    source.position,
                    target.position,
                    EasingSmoothLinear(elapsedTime / _movementDuration));

            if (!finalPart && elapsedTime >= _movementDuration - _movementDuration * _movementFinalPart)
            {
                float stereo = move.TargetSquare.y / 3.5f * _maxStereo - _maxStereo;
                if (Math.Abs(_boardTransform.rotation.eulerAngles.z) > 90f) stereo *= -1;
                _snap.panStereo = stereo;
                _snap.Play();

                if (move is ICapture capture)
                {
                    Destroy(_pieces[capture.CaptureSquare.x, capture.CaptureSquare.y].gameObject);
                    _pieces[capture.CaptureSquare.x, capture.CaptureSquare.y] = null;
                }
                finalPart = true;
            }
            yield return null;
        }
        if (move is Promotion promotion)
        {
            Piece newPiece = _position.Board[promotion.TargetSquare.x, promotion.TargetSquare.y];
            piece.GetComponent<Image>().sprite = _spritePack.GetPiece(
                newPiece.Color,
                newPiece.GetType());
        }

        piece.position = target.position;
        piece.SetParent(target);

        _pieces[move.SourceSquare.x, move.SourceSquare.y] = null;
        _pieces[move.TargetSquare.x, move.TargetSquare.y] = piece;

        if (move is Castling castling)
        {
            ShowMove(castling.RookMove);
        }
        else
        {
            MoveShown.Invoke();
            CheckPosition();
        }
    }

    private float EasingSmoothLinear(float x) => (x < 0.5) ? (x * x * 2) : (1 - (1 - x) * (1 - x) * 2);

    private void OnClick(Vector2Int square)
    {
        if (!_isMoveAllowed) return;

        if (_isSquareSelected && square == _selectedSquare)
        {
            Deselect();
        }
        else if (_position.Board[square.x, square.y] != null &&
            _position.Board[square.x, square.y].Color == _position.WhoseMove)
        {
            if (_isSquareSelected) Deselect();

            _selectedPieceHighlight = Instantiate(_selectedPieceHighlightPrefab, _tiles[square.x, square.y]);
            _selectedPieceHighlight.transform.SetAsFirstSibling();

            List<Move> moves = _position.Board[square.x, square.y].GetSmartMoves();
            foreach (var move in moves)
            {
                _moves[move.TargetSquare.x, move.TargetSquare.y].Add(move);
            }
            for (int x = 0; x < Position.SIZE; x++)
            {
                for (int y = 0; y < Position.SIZE; y++)
                {
                    if (_moves[x, y].Count != 0)
                    {
                        _possibleMoveHighlights.Add(Instantiate(_possibleMoveHighlightPrefab, _tiles[x, y]));
                    }
                }
            }

            _isSquareSelected = true;
            _selectedSquare = square;
        }
        else if (_moves[square.x, square.y].Count != 0)
        {
            _isMoveAllowed = false;
            if (_moves[square.x, square.y].Count == 1)
            {
                Move move = _moves[square.x, square.y][0];
                Deselect();
                MoveSelected?.Invoke(move);
                ShowMove(move);
            }
            else if (_moves[square.x, square.y].Count == 4)
            {
                _promotionMoves = _moves[square.x, square.y];
                _pieceSelection.Open(_position.WhoseMove);
            }
            else
            {
                throw new InvalidOperationException($"Must be only 0 or 1 or 4 possible moves " +
                    $"in one square, but {_moves[square.x, square.y].Count} found");
            }
        }
    }

    public void OnPieceSelected(Type piece)
    {
        Move move = null;
        foreach (Promotion promotion in _promotionMoves)
        {
            if (promotion.NewPiece == piece) move = promotion;
        }
        if (move == null) throw new ArgumentException($"Pawn cannot be promoted to {piece}");
        Deselect();
        MoveSelected?.Invoke(move);
        ShowMove(move);
    }

    private void Deselect()
    {
        Destroy(_selectedPieceHighlight);
        foreach (GameObject highlight in _possibleMoveHighlights)
        {
            Destroy(highlight);
        }
        for (int x = 0; x < Position.SIZE; x++)
        {
            for (int y = 0; y < Position.SIZE; y++)
            {
                _moves[x, y].Clear();
            }
        }
        _possibleMoveHighlights.Clear();
        _isSquareSelected = false;
    }


    public void Clear()
    {
        if (_stalemateHighlight != null) Destroy(_stalemateHighlight);
        else if (_checkmateHighlight != null) Destroy(_checkmateHighlight);
        else if (_checkHighlight != null) Destroy(_checkHighlight);

        if (_isSquareSelected) Deselect();

        StopAllCoroutines();

        for (int x = 0; x < Position.SIZE; x++)
        {
            for (int y = 0; y < Position.SIZE; y++)
            {
                if (_pieces[x, y] != null) Destroy(_pieces[x, y].gameObject);
            }
        }
    }
}
