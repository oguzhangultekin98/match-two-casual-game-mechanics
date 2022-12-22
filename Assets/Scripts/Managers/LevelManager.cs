using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableEvents.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelDataScriptableObject levelData;
    [SerializeField] private SimpleScriptableEvent Event_PlayerHasNoMoveLeft;

    private List<GameGrid> initializedGameGrids;
    private int levelMoveAmount;

    private void Awake()
    {
        InitializeLevelGoalHandler();
        levelMoveAmount = levelData.Value.LevelMoveAmount;
        initializedGameGrids = new List<GameGrid>();
    }

    private void InitializeLevelGoalHandler()
    {
        LevelGoalHandler levelGoalHandler = transform.parent.GetComponentInChildren<LevelGoalHandler>();
        levelGoalHandler.Initialize(levelData.Value.LevelGoals);
    }
    #region Event_Methods
    public void PlayerClickedOnMatch()
    {
        levelMoveAmount--;
        Debug.Log("Remaining amount of move: " + levelMoveAmount);
        if (levelMoveAmount < 1)
            Event_PlayerHasNoMoveLeft.Raise();
    }

    public void InitilizeGameGrid(GameObject gridGO)
    {
        if (gridGO.TryGetComponent(out GameGrid gameGrid))
        {
            initializedGameGrids.Add(gameGrid);
            gameGrid.Initilize(levelData.Value);
        }
    }
    #endregion
}
