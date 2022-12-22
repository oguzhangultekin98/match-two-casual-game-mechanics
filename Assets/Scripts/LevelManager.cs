using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableEvents.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelDataScriptableObject levelData;
    [SerializeField] private SimpleScriptableEvent Event_PlayerHasNoMoveLeft;
    private int levelMoveAmount;

    private void Awake()
    {
        LevelGoalHandler levelGoalHandler = transform.parent.GetComponentInChildren<LevelGoalHandler>();
        levelGoalHandler.SetLevelGoals(levelData.Value.LevelGoals);
        levelMoveAmount = levelData.Value.LevelMoveAmount;
    }

    public void PlayerClickedOnMatch()
    {
        levelMoveAmount--;
        if (levelMoveAmount < 1)
            Event_PlayerHasNoMoveLeft.Raise();
    }
}
