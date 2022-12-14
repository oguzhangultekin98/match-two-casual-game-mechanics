using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ScriptableEvents.Events;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private SimpleScriptableEvent Event_ClickedOnMatch;
    [SerializeField] private GameObjectScriptableEvent Event_GameGridWaitingToInitilize;

    private LevelData levelData;
    private Dictionary<PieceType, GameObject> piecePrefabDict;
    private GamePiece[,] pieces;

    private bool isGridCheckedForMatches = false;
    private bool isGameEnded;

    private void Start()
    {
        Event_GameGridWaitingToInitilize.Raise(gameObject);
    }

    public void Initilize(LevelData LevelData)
    {
        levelData = LevelData;
        pieces = new GamePiece[levelData.XDim, levelData.YDim];
        CreatePiecePrafabDict();
        FillGridWithPieceType(PieceType.EMPTY);
        StartCoroutine(Fill());
    }

    private IEnumerator Fill()
    {
        while (true)
        {
            yield return new WaitForSeconds(levelData.FillTime);

            bool anyGamePieceMovedBelow = MovePiecesDownIntoEmptySlots();
            bool anyGamePieceSpawnedOnFirstRow = FillFirstRow();
            bool dirty = anyGamePieceMovedBelow || anyGamePieceSpawnedOnFirstRow;

            if (!isGridCheckedForMatches && !dirty)
            {
                List<Match> matches = GetMatchesOnGrid();
                if (matches.Count == 0)
                {
                    if (IsItPossibleToCreateMatchOnGrid())
                        ShuffleGameGrid();
                    else
                        ColorGamePiecesRandomly();
                }
                else
                {
                    UpdateRegularPieceTiers(matches);
                    isGridCheckedForMatches = true;
                }
            }
        }
    }

    private bool MovePiecesDownIntoEmptySlots()
    {
        bool madeChangesOnGrid = false;
        for (int y = levelData.YDim - 2; y >= 0; y--)
        {
            for (int x = 0; x < levelData.XDim; x++)
            {
                GamePiece piece = pieces[x, y];

                if (!piece.IsMovable)
                    continue;

                GamePiece pieceBelow = pieces[x, y + 1];

                if (pieceBelow.Type == PieceType.EMPTY)
                {
                    MovePieceDown(piece, pieceBelow, x, y);
                    madeChangesOnGrid = true;
                }
            }
        }
        return madeChangesOnGrid;
    }

    private bool FillFirstRow()
    {
        bool madeChangesOnGrid = false;
        for (int x = 0; x < levelData.XDim; x++)
        {
            GamePiece pieceBelow = pieces[x, 0];

            if (pieceBelow.Type != PieceType.EMPTY)
                continue;

            Destroy(pieceBelow.gameObject);
            PieceType randomPieceType = GetRandomPieceTypeToSpawn();
            GamePiece newPiece = SpawnNewPiece(x, 0, randomPieceType);

            if (randomPieceType == PieceType.REGULAR)
            {
                ColorType randomColor = GetRandomAvailableColor();
                newPiece.ColorComponent.SetColor(randomColor);
            }
            newPiece.transform.position = GetWorldPosition(newPiece.XCord, -1);
            newPiece.MovableComponent.MoveOnGrid(x, 0, levelData.FillTime);
            madeChangesOnGrid = true;
        }
        return madeChangesOnGrid;
    }

    private PieceType GetRandomPieceTypeToSpawn()
    {
        float totalRange = 0;
        for (int i = 0; i < levelData.PiecePrefabs.Count; i++)
        {
            totalRange += levelData.PiecePrefabs[i].SpawnRate;
        }
        float randomValue = Random.Range(0, totalRange);

        float current = 0f;
        for (int i = 0; i < levelData.PiecePrefabs.Count; i++)
        {
            if (current <= randomValue && randomValue < current + levelData.PiecePrefabs[i].SpawnRate)
                return levelData.PiecePrefabs[i].Type;

            current += levelData.PiecePrefabs[i].SpawnRate;
        }
        return PieceType.EMPTY;
    }


    private List<Match> GetMatchesOnGrid()
    {
        List<GamePiece> checkedPieces = new List<GamePiece>();
        List<Match> matches = new List<Match>();
        for (int i = 0; i < levelData.XDim; i++)
        {
            for (int j = 0; j < levelData.YDim; j++)
            {
                GamePiece currentPiece = pieces[i, j];
                if (checkedPieces.Contains(currentPiece) || currentPiece.Type != PieceType.REGULAR)
                    continue;

                checkedPieces.Add(currentPiece);
                List<GamePiece> sameColorNeigbours = new List<GamePiece>();
                sameColorNeigbours.Add(currentPiece);
                GetSameColorNeighbours(currentPiece, sameColorNeigbours, currentPiece.ColorComponent.Color);
                if (sameColorNeigbours.Count > 1)
                {
                    Match match = new Match();
                    match.AddGamePieces(sameColorNeigbours);
                    checkedPieces.AddRange(sameColorNeigbours);
                    matches.Add(match);
                }
            }
        }

        return matches;
    }

    private void CreatePiecePrafabDict()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < levelData.PiecePrefabs.Count; i++)
        {
            if (!piecePrefabDict.ContainsKey(levelData.PiecePrefabs[i].Type))
                piecePrefabDict.Add(levelData.PiecePrefabs[i].Type, levelData.PiecePrefabs[i].Prefab);
        }
    }

    private void FillGridWithPieceType(PieceType type)
    {
        for (int x = 0; x < levelData.XDim; x++)
        {
            for (int y = 0; y < levelData.YDim; y++)
            {
                SpawnNewPiece(x, y, type);
            }
        }
    }

    private void ColorGamePiecesRandomly()
    {
        for (int i = 0; i < levelData.XDim; i++)
        {
            for (int j = 0; j < levelData.YDim; j++)
            {
                if (pieces[i, j].Type != PieceType.REGULAR)
                    continue;
                ColorType randomColor = GetRandomAvailableColor();
                pieces[i, j].ColorComponent.SetColor(randomColor);
            }
        }
        isGridCheckedForMatches = false;
    }

    private void UpdateRegularPieceTiers(List<Match> matches)
    {
        List<GamePiece> checkedPieces = new List<GamePiece>();
        for (int i = 0; i < matches.Count; i++)
        {
            for (int y = 0; y < matches[i].gamePieces.Count; y++)
            {
                checkedPieces.Add(matches[i].gamePieces[y]);
                matches[i].gamePieces[y].ColorComponent.UpdateTierVisual(matches[i].gamePieces.Count);
            }
        }

        for (int x = 0; x < levelData.XDim; x++)
        {
            for (int y = 0; y < levelData.YDim; y++)
            {
                if (!checkedPieces.Contains(pieces[x, y]) && pieces[x, y].Type == PieceType.REGULAR)
                    pieces[x, y].ColorComponent.UpdateTierVisual(1);
            }
        }
    }

    private void ClearGrid()
    {
        for (int i = 0; i < levelData.XDim; i++)
        {
            for (int j = 0; j < levelData.YDim; j++)
            {
                pieces[i, j].ClearableComponent.DestroyPiece();
            }
        }
    }

    private void ShuffleGameGrid()
    {
        List<GamePiece> movedPieces = new List<GamePiece>();
        for (int i = 0; i < levelData.XDim; i++)
        {
            for (int j = 0; j < levelData.YDim; j++)
            {
                GamePiece randomPiece = GetRandomPiece();
                if (!movedPieces.Contains(pieces[i, j]) && !movedPieces.Contains(randomPiece))
                {
                    movedPieces.Add(pieces[i, j]);
                    movedPieces.Add(randomPiece);
                    SwapPieces(pieces[i, j], randomPiece);
                }
            }
        }
        isGridCheckedForMatches = false;
    }

    private bool IsItPossibleToCreateMatchOnGrid()
    {
        for (int i = 0; i < levelData.AvailableColorsForLevel.Count; i++)
        {
            int numOfSameColorPiecesOnGrid = GetSameColorPiecesOnGrid(levelData.AvailableColorsForLevel[i]).Count;
            if (numOfSameColorPiecesOnGrid >= 2)
                return true;
        }
        return false;
    }

    private List<GamePiece> GetSameColorPiecesOnGrid(ColorType color)
    {
        List<GamePiece> sameColorPiecesOnGrid = new List<GamePiece>();

        for (int i = 0; i < levelData.XDim; i++)
        {
            for (int j = 0; j < levelData.YDim; j++)
            {
                if (pieces[i, j].ColorComponent.Color == color)
                    sameColorPiecesOnGrid.Add(pieces[i, j]);
            }
        }
        return sameColorPiecesOnGrid;
    }

    private GamePiece GetRandomPiece()
    {
        int rndX, rndY;
        rndX = Random.Range(0, levelData.XDim);
        rndY = Random.Range(0, levelData.YDim);

        return pieces[rndX, rndY];
    }

    private void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        int piece1XCord = piece1.XCord;
        int piece1YCord = piece1.YCord;
        GamePiece temp = piece1;

        pieces[piece1XCord, piece1YCord] = piece2;
        pieces[piece2.XCord, piece2.YCord] = temp;

        piece1.MovableComponent.MoveOnGrid(piece2.XCord, piece2.YCord, 0.1f);
        piece2.MovableComponent.MoveOnGrid(piece1XCord, piece1YCord, 0.1f);
    }

    private ColorType GetRandomAvailableColor()
    {
        return levelData.AvailableColorsForLevel[Random.Range(0, levelData.AvailableColorsForLevel.Count)];
    }

    private void MovePieceDown(GamePiece piece, GamePiece pieceBelow, int pieceXCord, int pieceYCord)
    {
        Destroy(pieceBelow.gameObject);
        piece.MovableComponent.MoveOnGrid(pieceXCord, pieceYCord + 1, levelData.FillTime);
        pieces[pieceXCord, pieceYCord + 1] = piece;
        SpawnNewPiece(pieceXCord, pieceYCord, PieceType.EMPTY);
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2((transform.position.x - levelData.XDim / 2.0f + x),
                           (transform.position.y + levelData.YDim / 2.0f - y))/2;
    }

    private GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity, transform);
        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Initialize(x, y, this, type);

        return pieces[x, y];
    }

    public void PressPiece(GamePiece pressedPiece)
    {
        if (isGameEnded)
            return;

        if (pressedPiece.Type == PieceType.REGULAR)
        {
            isGridCheckedForMatches = false;
            List<GamePiece> sameColorNeighbours = new List<GamePiece>();
            sameColorNeighbours.Add(pressedPiece);
            sameColorNeighbours = GetSameColorNeighbours(pressedPiece, sameColorNeighbours, pressedPiece.ColorComponent.Color);
            if (sameColorNeighbours.Count >= 2)
            {
                Event_ClickedOnMatch.Raise();
                ClearPieces(sameColorNeighbours);
            }
        }
    }

    private List<GamePiece> GetSameColorNeighbours(GamePiece startingPiece, List<GamePiece> sameColorNeighbours, ColorType searchingColor)
    {
        int startingPieceXCord = startingPiece.XCord;
        int startingPieceYCord = startingPiece.YCord;

        CheckNeighboursForSameColor(sameColorNeighbours, searchingColor, startingPieceXCord, startingPieceYCord);

        return sameColorNeighbours;
    }

    private List<GamePiece> CheckNeighboursForSameColor(List<GamePiece> sameColorNeighbours, ColorType searchingColor, int startingPieceXCord, int startingPieceYCord)
    {
        if (startingPieceYCord > 0)
        {
            GamePiece upNeighbour = pieces[startingPieceXCord, startingPieceYCord - 1];
            if(upNeighbour.Type == PieceType.REGULAR)
                CheckNeighbourForMatchingColor(upNeighbour, sameColorNeighbours, searchingColor);
        }

        if (startingPieceYCord < levelData.YDim - 1)
        {
            GamePiece belowNeighbour = pieces[startingPieceXCord, startingPieceYCord + 1];
            if (belowNeighbour.Type == PieceType.REGULAR)
                CheckNeighbourForMatchingColor(belowNeighbour, sameColorNeighbours, searchingColor);
        }

        if (startingPieceXCord > 0)
        {
            GamePiece leftNeighbour = pieces[startingPieceXCord - 1, startingPieceYCord];
            if (leftNeighbour.Type == PieceType.REGULAR)
                CheckNeighbourForMatchingColor(leftNeighbour, sameColorNeighbours, searchingColor);
        }

        if (startingPieceXCord < levelData.XDim - 1)
        {
            GamePiece rightNeighbour = pieces[startingPieceXCord + 1, startingPieceYCord];
            if (rightNeighbour.Type == PieceType.REGULAR)
                CheckNeighbourForMatchingColor(rightNeighbour, sameColorNeighbours, searchingColor);
        }
        return sameColorNeighbours;
    }

    private void CheckNeighbourForMatchingColor(GamePiece neighbour, List<GamePiece> sameColorNeighbours, ColorType searchingColor)
    {

        if (!sameColorNeighbours.Contains(neighbour)
            && CheckColorMatchBetweenPieces(searchingColor, neighbour))
        {
            sameColorNeighbours.Add(neighbour);
            GetSameColorNeighbours(neighbour, sameColorNeighbours, searchingColor);
        }
    }

    private bool CheckColorMatchBetweenPieces(ColorType firstPiece, GamePiece secondPiece)
    {
        ColorType firstPieceColor = firstPiece;
        ColorType secoundPieceColor = secondPiece.ColorComponent.Color;

        if (firstPieceColor == secoundPieceColor)
            return true;

        return false;
    }

    private void ClearPiece(int xCord, int yCord)
    {
        pieces[xCord, yCord].ClearableComponent.Clear();
        SpawnNewPiece(xCord, yCord, PieceType.EMPTY);
    }

    private void ClearPieces(List<GamePiece> clearPieceList)
    {
        for (int i = 0; i < clearPieceList.Count; i++)
        {
            ClearPiece(clearPieceList[i].XCord, clearPieceList[i].YCord);
        }
    }
    #region Event_Methods
    public void OnGameStateChange(int gameState)
    {
        GameState state = (GameState)gameState;
        if (state == GameState.GameEnd)
            isGameEnded = true;
    }
    #endregion
}
