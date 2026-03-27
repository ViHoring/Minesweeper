public class TileModel
{
    //Dados do tile
    public int X { get; }
    public int Y { get; }
    public bool IsFlag { get; }

    public TileModel(int x, int y, bool isFlag)
    {
        X = x;
        Y = y;
        IsFlag = isFlag;
    }
}