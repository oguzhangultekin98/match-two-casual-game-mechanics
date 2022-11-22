using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelGoal
{
    [SerializeField]
    private PieceType pieceType;
    public PieceType PieceType
    {
        get { return pieceType; }
        set { pieceType = value; }
    }

    [SerializeField]
    private ColorType colorType;
    public ColorType ColorType
    {
        get { return colorType; }
        set { colorType = value; }
    }

    [SerializeField]
    private int amount;
    public int Amount
    {
        get { return amount; }
        set { amount = value; }
    }
}
