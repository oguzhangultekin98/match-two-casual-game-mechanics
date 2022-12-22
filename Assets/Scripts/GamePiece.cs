using System.Collections;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private GameGrid grid;
    public GameGrid Grid => grid;

    private MovablePiece movableComponent;
    public MovablePiece MovableComponent => movableComponent;

    private ColorPiece colorComponent;
    public ColorPiece ColorComponent => colorComponent;

    private ClearablePiece clearableComponent;
    public ClearablePiece ClearableComponent => clearableComponent;

    private int xCord;
    private int yCord;

    public int XCord
    {
        get => xCord;
        set { if (IsMovable) { xCord = value; } }
    }

    public int YCord
    {
        get => yCord;
        set { if (IsMovable) { yCord = value; } }
    }

    private PieceType type;

    public PieceType Type   
    {
        get { return type; }  
        set { type = value; } 
    }

    public bool IsMovable => movableComponent != null;

    public bool IsColored => colorComponent != null;

    private void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
        colorComponent = GetComponent<ColorPiece>();
        clearableComponent = GetComponent<ClearablePiece>();
    }

    public void Initialize(int x, int y, GameGrid grid, PieceType type)
    {
        xCord = x;
        yCord = y;
        this.grid = grid;
        this.type = type;
    }

    private void OnMouseDown()
    {
        grid.PressPiece(this);
    }
}
