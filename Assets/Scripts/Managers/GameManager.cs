using System.Collections;
using System.Collections.Generic;
using ScriptableEvents.Events;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private IntScriptableEvent Event_GameStateChanged;
    public GameState gameState;

    private void ChangeGameState(GameState state)
    {
        gameState = state;
        Event_GameStateChanged.Raise((int)state);
    }
    #region Event_Methods
    public void AllGoalsAccomplished()
    {
        Debug.Log("All Goals Accomplished");
        ChangeGameState(GameState.GameEnd);
    }

    public void PlayerHasNoMoveLeft()
    {
        Debug.Log("No move left");
        ChangeGameState(GameState.GameEnd);
    }
    #endregion
}
