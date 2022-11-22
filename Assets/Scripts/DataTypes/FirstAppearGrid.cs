using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FirstAppearGrid
{
    [SerializeField]
    private PieceType type;
    public PieceType Type
    {
        get { return type; }
        set { type = value; }
    }

    [SerializeField]
    private ColorType color;
    public ColorType Color
    {
        get { return color; }
        set { color = value; }
    }
}
