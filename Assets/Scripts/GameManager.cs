using System.Collections;
using System.Collections.Generic;
using ScriptableEvents.Events;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
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
}
