using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelGoalHandler : MonoBehaviour
{
    [SerializeField] private LevelDataScriptableObject levelData;
    [SerializeField] private ColorSpriteScriptableObj[] colorSpriteScriptableObjects;
    [SerializeField] private Sprite notImplementedSprite;
    [SerializeField] private RectTransform[] slots;
    private List<LevelGoal> levelGoals;
    private MissionSlot[] missionSlots;

    public static LevelGoalHandler Instance;

    private void Awake()
    {
        levelGoals = levelData.Value.LevelGoals.ToList();

        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

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
        }
    }

    private Sprite GetRelatedSprite(ColorType colorType)
    {
        for (int i = 0; i < colorSpriteScriptableObjects.Length; i++)
        {
            if (colorType == colorSpriteScriptableObjects[i].value.color)
                return colorSpriteScriptableObjects[i].value.Sprite;
        }
        return notImplementedSprite;
    }

    public bool PieceCleared(PieceType type, ColorType colorType)
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

    public Vector3 GetGoalLocation(ColorType color)
    {
        Vector3 location = Vector3.zero;
        Vector3 gamePiecePivotOffset = new Vector3(-0.25f, 0.25f);
        for (int i = 0; i < levelGoals.Count; i++)
        {
            if (color == levelGoals[i].ColorType)
            {
                RectTransform rect = missionSlots[i].Slotobject.GetComponent<RectTransform>();
                Vector3 pieceLocation = rect.transform.position;
                pieceLocation.z = 0f;
                return pieceLocation + gamePiecePivotOffset;
            }
        }
        Debug.Log(location);
        location.z = 0f;
        return location;
    }

    private void CheckIfAllGoalsCompleted()
    {
        for (int i = 0; i < levelGoals.Count; i++)
        {
            if (levelGoals[i].Amount != 0)
                return;
        }
        GameManager.Instance.LevelCompleted();
    }
}
