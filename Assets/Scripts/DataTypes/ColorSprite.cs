using UnityEngine;

[System.Serializable]
public struct ColorSprite
{
    public ColorType color;
    public Sprite Sprite;

    public Color GetRGBColor()
    {
        switch (color)
        {
            case ColorType.YELLOW:
                return Color.yellow;
            case ColorType.PURPLE:
                return new Color(143,0,254,1);
            case ColorType.RED:
                return Color.red;
            case ColorType.BLUE:
                return Color.blue;
            case ColorType.GREEN:
                return Color.green;
            default:
                break;
        }

        return Color.black;
    }
}