using System.Collections;
using System.Collections.Generic;
using ScriptableEvents.Events;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private IntScriptableEvent Event_GameStateChanged;
    public GameState gameState;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void LevelCompleted()
    {
        Debug.Log("LevelCompleted");
        gameState = GameState.GameEnd;
    }

    private void ChangeGameState(GameState state)
    {
        gameState = state;
        Event_GameStateChanged.Raise((int)state);
    }

    public void AllGoalsAccomplished()
    {
        ChangeGameState(GameState.GameEnd);
    }

    public void PlayerHasNoMoveLeft()
    {
        ChangeGameState(GameState.GameEnd);
    }
}
