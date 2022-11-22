[System.Serializable]
public class FirstAppearLevelGrid
{
    public int xDim;
    public int yDim;
    public AllBlocksAvailable[,] grid;
    public string LevelName;
    public FirstAppearLevelGrid(int x, int y)
    {
        xDim = x;
        yDim = y;
        grid = new AllBlocksAvailable[x, y];
    }
}