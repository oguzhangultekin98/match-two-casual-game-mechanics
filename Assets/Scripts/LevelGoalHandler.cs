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

    private void Awake()
    {
        FillMissinSlotsArray();
        SetMissionSlotVisuals();
    }

    public void SetLevelGoals(List<LevelGoal> LevelGoals)
    {
        levelGoals = LevelGoals;
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

    public void GamePieceCleared(GameObject clearedPiece)
    {
        GamePiece piece = clearedPiece.GetComponent<GamePiece>();
        PieceType type = piece.Type;
        ColorType colorType = ColorType.Default;
        if (piece.ColorComponent)
        {
            colorType = piece.ColorComponent.Color;
        }
        PieceCleared(type,colorType);
    }

    private bool PieceCleared(PieceType type, ColorType colorType)
    {
        for (int i = 0; i < levelGoals.Count; i++)
        {
            if (levelGoals[i].PieceType == PieceType.REGULAR)
            {
                if (levelGoals[i].ColorType == colorType && levelGoals[i].Amount > 0)
                {
                    levelGoals[i].Amount--;
                    if (levelGoals[i].Amount == 0)
                        CheckIfAllGoalsCompleted();

                    return true;
                }
                continue;
            }

            if (levelGoals[i].PieceType == type && levelGoals[i].Amount > 0)
            {
                levelGoals[i].Amount--;
                if (levelGoals[i].Amount == 0)
                    CheckIfAllGoalsCompleted();

                return true;
            }
        }
        return false;
    }

    private void CheckIfAllGoalsCompleted()
    {
        for (int i = 0; i < levelGoals.Count; i++)
        {
            if (levelGoals[i].Amount != 0)
                return;
        }
        Event_AllGoalsAccomplished.Raise();
    }
}
