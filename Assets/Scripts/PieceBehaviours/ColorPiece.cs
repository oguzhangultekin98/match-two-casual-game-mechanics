using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorPiece : MonoBehaviour
{
    private static int howManyPieceSpawned = 0;
    public int HowManyPieceSpawned => howManyPieceSpawned;
    public ColorSpriteScriptableObj[] colorSprites;

    private ColorType color;
    public ColorType Color
    {
        get => color;
        set => SetColor(value);
    }

    public int NumColors => colorSprites.Length;

    private SpriteRenderer sprite;
    private Dictionary<ColorType, Sprite> colorSpriteDict;
    private void Awake()
    {
        howManyPieceSpawned++;

        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.sortingOrder = howManyPieceSpawned;
        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < colorSprites.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(colorSprites[i].value.color))
            {
                colorSpriteDict.Add(colorSprites[i].value.color, colorSprites[i].value.Sprites[0]);
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
            sprite.sprite = colorSpriteDict[newColor];
        }
    }

    public void UpdateTierVisual()
    {

    }

}
