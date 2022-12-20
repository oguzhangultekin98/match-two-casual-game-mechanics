using System.Collections;
using UnityEngine;

public abstract class GamePiece : MonoBehaviour
{
    private int xCord;
    private int yCord;

    public int XCord
    {
        get => xCord;
        set { if (IsMovable()) { xCord = value; } }
    }

    public int YCord
    {
        get => yCord;
        set { if (IsMovable()) { yCord = value; } }
    }

    private PieceType type;

    public PieceType Type   
    {
        get { return type; }  
        set { type = value; } 
    }


    [HideInInspector] public GameGrid Grid;
    [HideInInspector] public MovablePiece MovableComponent;
    [HideInInspector] public ColorPiece ColorComponent;
    [HideInInspector] public ClearablePiece ClearableComponent;

    public bool IsMovable()
    {
        return MovableComponent != null;
    }

    public bool IsColored()
    {
        return ColorComponent != null;
    }

    public void SetNecessarVariables(int x, int y, GameGrid grid, PieceType type)
    {
        xCord = x;
        yCord = y;
        this.Grid = grid;
        this.type = type;
    }

    private void OnMouseDown()
    {
        Grid.PressPiece(this);
    }
}
