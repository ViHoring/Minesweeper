public class TileModel
{
    //Dados do tile
    public int X { get; }
    public int Y { get; }
    public bool IsFlag { get; }
    public bool IsBomb { get; }

    public TileModel(int x, int y, bool isFlag, bool isBomb)
    {
        X = x;
        Y = y;
        IsFlag = isFlag;
        IsBomb = isBomb;
    }
}