using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int Row;
    public int Col;
    public SlotType SlotType;
    public GamePiece HoldingPiece;

    public Slot(int row, int col, SlotType slotType, GamePiece holdingPiece)
    {
        Row = row;
        Col = col;
        SlotType = slotType;
        HoldingPiece = holdingPiece;
    }
}
