using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarPiece : GamePiece
{
    private void Awake()
    {
        base.MovableComponent = GetComponent<MovablePiece>();
        base.ColorComponent = GetComponent<ColorPiece>();
        base.ClearableComponent = GetComponent<ClearablePiece>();
    }
}
