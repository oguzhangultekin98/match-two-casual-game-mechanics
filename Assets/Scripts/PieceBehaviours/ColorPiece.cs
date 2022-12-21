using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorPiece : MonoBehaviour
{
    public ColorSpriteScriptableObj[] colorSprites;

    private ColorType color;
    public ColorType Color
    {
        get => color;
        set => SetColor(value);
    }

    public int NumColors => colorSprites.Length;

    private SpriteRenderer sprite;
    private Dictionary<ColorType, Sprite[]> colorSpriteDict;

    [SerializeField] private int ATierCondition;
    [SerializeField] private int BTierCondition;
    [SerializeField] private int CTierCondition;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        colorSpriteDict = new Dictionary<ColorType, Sprite[]>();

        for (int i = 0; i < colorSprites.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(colorSprites[i].value.color))
            {
                colorSpriteDict.Add(colorSprites[i].value.color, colorSprites[i].value.Sprites);
            }
        }
    }

    public bool IsSpriteAvailable()
    {   
        return sprite != null;
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;

        if (colorSpriteDict.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDict[newColor][0];
        }
    }

    public void UpdateTierVisual(int matchSize)
    {
        SugarTier tier;
        if (matchSize > CTierCondition)
            tier = SugarTier.CTIER;
        else if (matchSize > BTierCondition)
            tier = SugarTier.BTIER;
        else if (matchSize > ATierCondition)
            tier = SugarTier.ATIER;
        else
            tier = SugarTier.REGULARTIER;

        sprite.sprite = colorSpriteDict[color][(int)tier];
    }

}
