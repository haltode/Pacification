public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection dir)
    {
        if((int) dir < 3)
            return dir + 3;
        else
            return dir - 3;
    }

    public static HexDirection Previous(this HexDirection dir)
    {
        if(dir == HexDirection.NE)
            return HexDirection.NW;
        else
            return dir - 1;
    }

    public static HexDirection Next(this HexDirection dir)
    {
        if(dir == HexDirection.NW)
            return HexDirection.NE;
        else
            return dir + 1;
    }
}