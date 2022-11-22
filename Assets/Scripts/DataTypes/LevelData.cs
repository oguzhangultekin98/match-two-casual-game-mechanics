using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData 
{
    [SerializeField] private int xDim;
    public int XDim => xDim;

    [SerializeField] private int yDim;
    public int YDim => yDim;

    [SerializeField] private float fillTime;
    public float FillTime => fillTime;

    [SerializeField] private List<ColorType> availableColorsForLevel;
    public List<ColorType> AvailableColorsForLevel => availableColorsForLevel;

    [SerializeField] private List<FirstAppearGrid> firstAppearGrid;
    public List<FirstAppearGrid> FirstAppearGrid => firstAppearGrid;

    [SerializeField] private List<LevelGoal> levelGoals;
    public List<LevelGoal> LevelGoals => levelGoals;

    [SerializeField] private int levelMoveAmount;
    public int LevelMoveAmount => levelMoveAmount;

    [SerializeField] private List<PiecePrefab> piecePrefabs;
    public List<PiecePrefab> PiecePrefabs => piecePrefabs;

    public LevelData(int xDim, int yDim, float fillTime, List<ColorType> availableColors, List<FirstAppearGrid> firstAppearGrid,
        List<LevelGoal> levelGoals, int levelMoveAmount, List<PiecePrefab> prefabs)
    {
        this.xDim = xDim;
        this.yDim = yDim;
        this.fillTime = fillTime;
        this.availableColorsForLevel = availableColors;
        this.firstAppearGrid = firstAppearGrid;
        this.levelGoals = levelGoals;
        this.levelMoveAmount = levelMoveAmount;
        this.piecePrefabs = prefabs;
    }
}
