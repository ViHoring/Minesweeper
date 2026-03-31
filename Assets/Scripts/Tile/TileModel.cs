public class TileModel
{
    //Dados do tile
    public int X { get; }
    public int Y { get; }
    public bool IsFlag { get; }
    public bool IsMine { get; }
    public bool IsBlank { get; }

    public TileModel(int x, int y, bool isFlag, bool isMine, bool isBlank)
    {
        X = x;
        Y = y;
        IsFlag = isFlag;
        IsMine = isMine;
        IsBlank = isBlank;
    }
}