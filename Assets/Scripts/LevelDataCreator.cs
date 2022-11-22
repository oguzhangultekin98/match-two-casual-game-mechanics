using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class LevelDataCreator : MonoBehaviour
{
    private const int maxDimNum = 50;
    public FirstAppearLevelGrid levelGrid = new FirstAppearLevelGrid(maxDimNum, maxDimNum);
    [SerializeField] private List<ColorType> availableColorsForLevel;
    [SerializeField] private List<PieceType> availablePieceTypesForLevel;
    [SerializeField] private List<LevelGoal> levelGoals;
    [SerializeField] private List<PiecePrefab> piecePrefabs;
    [SerializeField] private int levelMoveAmount;

    public void CreateLevelDataScriptableObj(int xDim, int yDim, AllBlocksAvailable[,] grid, string levelName)
    {
        LevelDataScriptableObject asset = ScriptableObject.CreateInstance<LevelDataScriptableObject>();

        List<FirstAppearGrid> firstAppearGrid = FillGridDataIntoAppearGrid(xDim, yDim, grid);
        asset.Value = new LevelData(xDim, yDim, 0.1f, availableColorsForLevel, firstAppearGrid,levelGoals,levelMoveAmount,piecePrefabs);


        string name = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObjects/LevelDatas/" + levelName + ".asset");

        EditorUtility.SetDirty(asset);
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    private List<FirstAppearGrid>  FillGridDataIntoAppearGrid(int xDim, int yDim, AllBlocksAvailable[,] grid)
    {
        List<FirstAppearGrid> myGridData = new List<FirstAppearGrid>();
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                FirstAppearGrid gameBlock = FillGamePiece(grid[x, y]);
                myGridData.Add(gameBlock);
            }
        }
        return myGridData;
    }

    private FirstAppearGrid FillGamePiece(AllBlocksAvailable gameBlock)
    {
        FirstAppearGrid myPieceInfo = new FirstAppearGrid();

        switch (gameBlock)
        {
            case AllBlocksAvailable.EMPTY:
                myPieceInfo.Type = PieceType.EMPTY;
                break;
            case AllBlocksAvailable.RED:
                myPieceInfo.Type = PieceType.REGULAR;
                myPieceInfo.Color = ColorType.RED;
                break;
            case AllBlocksAvailable.BLUE:
                myPieceInfo.Type = PieceType.REGULAR;
                myPieceInfo.Color = ColorType.BLUE;
                break;
            case AllBlocksAvailable.GREEN:
                myPieceInfo.Type = PieceType.REGULAR;
                myPieceInfo.Color = ColorType.GREEN;
                break;
            case AllBlocksAvailable.PURPLE:
                myPieceInfo.Type = PieceType.REGULAR;
                myPieceInfo.Color = ColorType.PURPLE;
                break;
            case AllBlocksAvailable.YELLOW:
                myPieceInfo.Type = PieceType.REGULAR;
                myPieceInfo.Color = ColorType.YELLOW;
                break;
            case AllBlocksAvailable.DUCK:
                myPieceInfo.Type = PieceType.DUCK;
                myPieceInfo.Color = ColorType.Default;
                break;
            case AllBlocksAvailable.BALLOON:
                myPieceInfo.Type = PieceType.BALLOON;
                myPieceInfo.Color = ColorType.Default;
                break;
            case AllBlocksAvailable.ROCKETLEFT:
                myPieceInfo.Type = PieceType.ROCKETRIGHT;
                myPieceInfo.Color = ColorType.Default;
                break;
            case AllBlocksAvailable.ROCKETRIGHT:
                myPieceInfo.Type = PieceType.ROCKETRIGHT;
                myPieceInfo.Color = ColorType.Default;
                break;
            default:
                break;
        }

        return myPieceInfo;
    }
}
