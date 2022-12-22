using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ScriptableEvents.Events;

public class LevelGoalHandler : MonoBehaviour
{
    [SerializeField] private ColorSpriteScriptableObj[] colorSpriteScriptableObjects;
    [SerializeField] private Sprite notImplementedSprite;
    [SerializeField] private RectTransform[] slots;
    [SerializeField] private SimpleScriptableEvent Event_AllGoalsAccomplished;

    private List<LevelGoal> levelGoals;
    private MissionSlot[] missionSlots;

    public void Initialize(List<LevelGoal> LevelGoals)
    {
        levelGoals = LevelGoals;
        FillMissinSlotsArray();
        SetMissionSlotVisuals();
    }

    private void FillMissinSlotsArray()
    {
        missionSlots = new MissionSlot[levelGoals.Count];
        for (int i = 0; i < levelGoals.Count; i++)
        {
            missionSlots[i].ColorType = (ColorType)(int)levelGoals[i].ColorType;
            missionSlots[i].Slotobject = slots[i];
        }
    }

    private void SetMissionSlotVisuals()
    {
        if (levelGoals.Count > missionSlots.Length)
            Debug.LogWarning("There aren't enough slots");
        for (int i = 0; i < levelGoals.Count; i++)
        {
            Image missionImage = missionSlots[i].Slotobject.GetComponent<Image>();
            missionSlots[i].ColorType = levelGoals[i].ColorType;
            missionImage.sprite = GetRelatedSprite(levelGoals[i].ColorType);
            missionImage.color = Color.white;
            Debug.Log("Clear " + levelGoals[i].Amount +" "+ levelGoals[i].ColorType);
        }
    }

    private Sprite GetRelatedSprite(ColorType colorType)
    {
        for (int i = 0; i < colorSpriteScriptableObjects.Length; i++)
        {
            if (colorType == colorSpriteScriptableObjects[i].value.color)
                return colorSpriteScriptableObjects[i].value.Sprites[0];
        }
        return notImplementedSprite;
    }

    private void AdjustGoalsProgress(PieceType type, ColorType colorType)
    {
        for (int i = 0; i < levelGoals.Count; i++)
        {
            if (levelGoals[i].PieceType == PieceType.REGULAR)
            {
                if (levelGoals[i].ColorType == colorType && levelGoals[i].Amount > 0)
                {
                    levelGoals[i].Amount--;
                    Debug.Log("Clear " + levelGoals[i].Amount + " " + levelGoals[i].ColorType);
                }
            }
            else if(levelGoals[i].PieceType == type && levelGoals[i].Amount > 0)
            { 
                levelGoals[i].Amount--;
            }
        }
    }

    private bool CheckIfAllGoalsCompleted()
    {
        for (int i = 0; i < levelGoals.Count; i++)
        {
            if (levelGoals[i].Amount != 0)
                return false;
        }
        return true;
    }

    #region Event_Methods
    public void GamePieceCleared(GameObject clearedPiece)
    {
        GamePiece piece = clearedPiece.GetComponent<GamePiece>();
        PieceType type = piece.Type;
        ColorType colorType = ColorType.Default;
        if (piece.ColorComponent)
        {
            colorType = piece.ColorComponent.Color;
        }
        AdjustGoalsProgress(type, colorType);
        if (CheckIfAllGoalsCompleted())
            Event_AllGoalsAccomplished.Raise();
    }
    #endregion
}
