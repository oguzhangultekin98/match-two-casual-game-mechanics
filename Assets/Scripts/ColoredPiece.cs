using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredPiece : GamePiece
{
    private void Awake()
    {
        MovableComponent = GetComponent<MovablePiece>();
        ColorComponent = GetComponent<ColorPiece>();
        ClearableComponent = GetComponent<ClearablePiece>();
    }
}
