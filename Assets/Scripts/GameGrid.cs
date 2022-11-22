using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private LevelDataScriptableObject levelDataScriptableObj;
    private LevelData levelData;
    private int levelMoveAmount;

    private Dictionary<PieceType, GameObject> piecePrefabDict;
    private GamePiece[,] pieces;
    bool isGridCheckedForMatches = false;

    [SerializeField]private GameObject bg;

    private void Awake()
    {
        levelData = levelDataScriptableObj.Value;
        levelMoveAmount = levelData.LevelMoveAmount;
        pieces = new GamePiece[levelData.XDim, levelData.YDim];
        CreateGridBackGround();
        CreatePiecePrafabDict();
        if (levelData.FirstAppearGrid.Count == 0)
        {
            FillGridWithPieceType(PieceType.EMPTY);
        }
        else
        {
            FillGridWithFirstAppearGrid();
        }
        StartCoroutine(Fill());
    }

    private IEnumerator Fill()
    {
        while (true)
        {
            yield return new WaitForSeconds(levelData.FillTime);

            bool anyGamePieceMovedBelow = MovePiecesHaveEmptyNeighbourBelow();
            bool anyGamePieceSpawnedOnFirstRow = FillFirstRow();
            bool anyDuckClearedOnLastRow = ClearDucksInLastRow();
            bool isAnyChangeHappenedOnLastCycle = anyGamePieceMovedBelow || anyGamePieceSpawnedOnFirstRow;

            if (!isAnyChangeHappenedOnLastCycle && !isGridCheckedForMatches && !anyDuckClearedOnLastRow)
            {
                CheckMachesOnGrid();
                isGridCheckedForMatches = true;
            }
        }
    }

    private bool MovePiecesHaveEmptyNeighbourBelow()
    {
        bool madeChangesOnGrid = false;
        for (int y = levelData.YDim - 2; y >= 0; y--)
        {
            for (int x = 0; x < levelData.XDim; x++)
            {
                GamePiece piece = pieces[x, y];

                if (!piece.IsMovable())
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

        var current = 0f;
        for (int i = 0; i < levelData.PiecePrefabs.Count; i++)
        {
            if (current <= randomValue && randomValue < current + levelData.PiecePrefabs[i].SpawnRate)
            {
                return levelData.PiecePrefabs[i].Type;
            }
            current += levelData.PiecePrefabs[i].SpawnRate;
        }
        return PieceType.EMPTY;
    }

    private bool ClearDucksInLastRow()
    {
        bool madeChangesOnGrid = false;
        for (int x = 0; x < levelData.XDim; x++)
        {
            GamePiece lastRowPiece = pieces[x, levelData.YDim - 1];

            if (lastRowPiece.Type != PieceType.DUCK)
                continue;
            ClearPiece(lastRowPiece.XCord, lastRowPiece.YCord);
            madeChangesOnGrid = true;
        }
        return madeChangesOnGrid;
    }

    private void CheckMachesOnGrid()
    {
        List<GamePiece> checkedPieces = new List<GamePiece>();
        int biggestNeighbourGroup = -1;
        for (int i = 0; i < levelData.XDim; i++)
        {
            for (int j = 0; j < levelData.YDim; j++)
            {
                GamePiece currentPiece = pieces[i, j];
                if (CheckIfPieceAlreadyInList(checkedPieces, currentPiece) || currentPiece.Type != PieceType.REGULAR)
                    continue;

                List<GamePiece> sameColorNeigbours = new List<GamePiece>();
                sameColorNeigbours.Add(currentPiece);
                GetSameColorNeighbours(currentPiece, sameColorNeigbours, currentPiece.ColorComponent.Color);
                if (biggestNeighbourGroup < sameColorNeigbours.Count)
                    biggestNeighbourGroup = sameColorNeigbours.Count;

            }
        }

        if (biggestNeighbourGroup == 1)
        {
            if (IsPossibleToCreateMatchOnGrid())
                ShuffleGameGrid();
            else
                GenerateNewGrid();
        }
    }

    private void CreateGridBackGround()
    {
        Vector2 gridStartPos = GetWorldPosition(0, 0);
        float gridWidth = (gridStartPos - GetWorldPosition(0, levelData.YDim)).y;
        float gridHeight = (GetWorldPosition(levelData.YDim, 0) - gridStartPos).x;
        float offset = 0.5f;
        Vector2 backGroundInitialPosition = gridStartPos - (new Vector2(1,-1)) * offset / 2;
        bg = Instantiate(bg, backGroundInitialPosition, Quaternion.identity);
        SpriteRenderer renderer = bg.GetComponent<SpriteRenderer>();
        renderer.size = new Vector2(gridWidth+offset , gridHeight + offset);
    }

    private void FillGridWithFirstAppearGrid()
    {
        for (int x = 0; x < levelData.XDim; x++)
        {
            for (int y = 0; y < levelData.YDim; y++)
            {
                var firstAppearGridElement = levelData.FirstAppearGrid[x * levelData.YDim + y];
                var piece = SpawnNewPiece(x, y, firstAppearGridElement.Type);
                if (firstAppearGridElement.Type == PieceType.REGULAR)
                    piece.ColorComponent.Color = firstAppearGridElement.Color;
            }
        }
    }

    private void CreatePiecePrafabDict()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < levelData.PiecePrefabs.Count; i++)
        {
            if (!piecePrefabDict.ContainsKey(levelData.PiecePrefabs[i].Type))
            {
                piecePrefabDict.Add(levelData.PiecePrefabs[i].Type, levelData.PiecePrefabs[i].Prefab);
            }
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

    private void GenerateNewGrid()
    {
        ClearGrid();
        FillGridWithPieceType(PieceType.REGULAR);
        CheckMachesOnGrid();
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
        CheckMachesOnGrid();
    }

    private bool IsPossibleToCreateMatchOnGrid()
    {
        for (int i = 0; i < levelData.AvailableColorsForLevel.Count; i++)
        {
            int numOfSameColorPiecesOnGrid = GetSameColorPiecesOnGrid(levelData.AvailableColorsForLevel[i]).Count;
            if (numOfSameColorPiecesOnGrid >= 2)
            {
                return true;
            }
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
                {
                    sameColorPiecesOnGrid.Add(pieces[i, j]);
                }
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
        pieces[x, y].SetNecessarVariables(x, y, this, type);

        return pieces[x, y];
    }

    public void PressPiece(GamePiece pressedPiece)
    {
        if (GameManager.Instance.gameState == GameState.GameEnd)
            return;

        if (pressedPiece.Type == PieceType.REGULAR)
        {

            isGridCheckedForMatches = false;
            List<GamePiece> sameColorNeighbours = new List<GamePiece>();
            sameColorNeighbours.Add(pressedPiece);
            sameColorNeighbours = GetSameColorNeighbours(pressedPiece, sameColorNeighbours, pressedPiece.ColorComponent.Color);
            if (sameColorNeighbours.Count >= 2)
            {
                levelMoveAmount--;

                ClearPieces(sameColorNeighbours);
                ClearBalloonsAroundPieces(sameColorNeighbours);
                if (levelMoveAmount < 1)
                    GameManager.Instance.LevelCompleted();
            }
        }
        else if (pressedPiece.Type == PieceType.ROCKETLEFT)
        {
            List<GamePiece> sameRowRightGamePieces = GetSameRowRightGamePieces(pressedPiece);
            ClearPieces(sameRowRightGamePieces);
            levelMoveAmount--;
        }
        else if (pressedPiece.Type == PieceType.ROCKETRIGHT)
        {
            List<GamePiece> sameRowLeftGamePieces = GetSameRowLeftGamePieces(pressedPiece);
            ClearPieces(sameRowLeftGamePieces);
            levelMoveAmount--;
        }
    }

    private List<GamePiece> GetSameRowRightGamePieces(GamePiece pressedPiece)
    {
        List<GamePiece> sameRowPieces = new List<GamePiece>();

        for (int i = pressedPiece.XCord; i < levelData.XDim; i++)
        {
            sameRowPieces.Add(pieces[i, pressedPiece.YCord]);
        }

        return sameRowPieces;
    }

    private List<GamePiece> GetSameRowLeftGamePieces(GamePiece pressedPiece)
    {
        List<GamePiece> sameRowPieces = new List<GamePiece>();

        for (int i = pressedPiece.XCord; i >= 0; i--)
        {
            sameRowPieces.Add(pieces[i, pressedPiece.YCord]);
        }

        return sameRowPieces;
    }

    private void ClearBalloonsAroundPieces(List<GamePiece> sameColorNeighbours)
    {
        for (int i = 0; i < sameColorNeighbours.Count; i++)
        {
            CheckNeighboursForBallon(sameColorNeighbours[i]);
        }
    }

    private List<GamePiece> GetSameColorNeighbours(GamePiece startingPiece, List<GamePiece> SameColorNeighbours, ColorType searchingColor)
    {
        var startingPieceXCord = startingPiece.XCord;
        var startingPieceYCord = startingPiece.YCord;

        CheckNeighbours(SameColorNeighbours, searchingColor, startingPieceXCord, startingPieceYCord);

        return SameColorNeighbours;
    }

    private List<GamePiece> CheckNeighbours(List<GamePiece> SameColorNeighbours, ColorType searchingColor, int startingPieceXCord, int startingPieceYCord)
    {
        if (startingPieceYCord > 0)
        {
            var upNeighbour = pieces[startingPieceXCord, startingPieceYCord - 1];
            if(upNeighbour.Type == PieceType.REGULAR)
                CheckNeighbourForMatchingColor(upNeighbour, SameColorNeighbours, searchingColor);
        }

        if (startingPieceYCord < levelData.YDim - 1)
        {
            var belowNeighbour = pieces[startingPieceXCord, startingPieceYCord + 1];
            if (belowNeighbour.Type == PieceType.REGULAR)
                CheckNeighbourForMatchingColor(belowNeighbour, SameColorNeighbours, searchingColor);
        }

        if (startingPieceXCord > 0)
        {
            var leftNeighbour = pieces[startingPieceXCord - 1, startingPieceYCord];
            if (leftNeighbour.Type == PieceType.REGULAR)
                CheckNeighbourForMatchingColor(leftNeighbour, SameColorNeighbours, searchingColor);
        }

        if (startingPieceXCord < levelData.XDim - 1)
        {
            var rightNeighbour = pieces[startingPieceXCord + 1, startingPieceYCord];
            if (rightNeighbour.Type == PieceType.REGULAR)
                CheckNeighbourForMatchingColor(rightNeighbour, SameColorNeighbours, searchingColor);
        }
        return SameColorNeighbours;
    }

    private void CheckNeighbourForMatchingColor(GamePiece neighbour, List<GamePiece> SameColorNeighbours, ColorType searchingColor)
    {

        if (!CheckIfPieceAlreadyInList(SameColorNeighbours, neighbour)
            && CheckColorMatchBetweenPieces(searchingColor, neighbour))
        {
            SameColorNeighbours.Add(neighbour);
            GetSameColorNeighbours(neighbour, SameColorNeighbours, searchingColor);
        }
    }

    private void CheckNeighboursForBallon(GamePiece piece)
    {
        if (piece.YCord > 0)
        {
            var upNeighbour = pieces[piece.XCord, piece.YCord - 1];
            if (upNeighbour.Type == PieceType.BALLOON)
                ClearPiece(piece.XCord, piece.YCord - 1);
        }

        if (piece.YCord < levelData.YDim - 1)
        {
            var belowNeighbour = pieces[piece.XCord, piece.YCord + 1];
            if (belowNeighbour.Type == PieceType.BALLOON)
                ClearPiece(piece.XCord, piece.YCord + 1);
        }

        if (piece.XCord > 0)
        {
            var leftNeighbour = pieces[piece.XCord - 1, piece.YCord];
            if (leftNeighbour.Type == PieceType.BALLOON)
                ClearPiece(piece.XCord - 1, piece.YCord);
        }

        if (piece.XCord < levelData.XDim - 1)
        {
            var rightNeighbour = pieces[piece.XCord + 1, piece.YCord];
            if (rightNeighbour.Type == PieceType.BALLOON)
                ClearPiece(piece.XCord + 1, piece.YCord);
        }
    }

    private bool CheckIfPieceAlreadyInList(List<GamePiece> gamePieceList, GamePiece piece)
    {
        for (int i = 0; i < gamePieceList.Count; i++)
        {
            if (gamePieceList[i].XCord == piece.XCord &&
                gamePieceList[i].YCord == piece.YCord)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckColorMatchBetweenPieces(ColorType firstPiece, GamePiece secondPiece)
    {
        var firstPieceColor = firstPiece;
        var secoundPieceColor = secondPiece.ColorComponent.Color;

        if (firstPieceColor == secoundPieceColor)
            return true;

        return false;
    }

    private void ClearPiece(int xCord, int yCord)
    {
        SoundManager.Instance.PlayPieceClearedSound(pieces[xCord,yCord].Type);
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
}
