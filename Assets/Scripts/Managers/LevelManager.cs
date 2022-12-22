using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableEvents.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelDataScriptableObject levelData;
    [SerializeField] private SimpleScriptableEvent Event_PlayerHasNoMoveLeft;

    private List<GameGrid> imitlizedGameGrids;
    private int levelMoveAmount;

    private void Awake()
    {
        LevelGoalHandler levelGoalHandler = transform.parent.GetComponentInChildren<LevelGoalHandler>();
        levelGoalHandler.SetLevelGoals(levelData.Value.LevelGoals);
        levelMoveAmount = levelData.Value.LevelMoveAmount;
        imitlizedGameGrids = new List<GameGrid>();
    }

    public void PlayerClickedOnMatch()
    {
        levelMoveAmount--;
        if (levelMoveAmount < 1)
            Event_PlayerHasNoMoveLeft.Raise();
    }

    public void InitilizeGameGrid(GameObject gridGO)
    {
        if (gridGO.TryGetComponent(out GameGrid gameGrid))
        {
            imitlizedGameGrids.Add(gameGrid);
            gameGrid.InitilizeGameGrid(levelData.Value);
        }
    }
}
