using UnityEngine;

public enum TileColor
{
    Red,
    Blue,
    Yellow,
    Black
}

public enum TileNumber
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Eleven = 11,
    Twelve = 12,
    Thirteen = 13
}

public class Tile
{
    public TileColor Color { get; private set; }
    public TileNumber Number { get; private set; }
    public bool IsFake { get; private set; }
    public bool IsSelected { get; set; }

    public Tile(TileColor color, TileNumber number, bool isFake = false)
    {
        Color = color;
        Number = number;
        IsFake = isFake;
    }

    public override string ToString()
    {
        return $"{Color} {Number}";
    }

    public bool CanBeNextTo(Tile other)
    {
        if (Color != other.Color) return false;
        
        int currentNumber = (int)Number;
        int otherNumber = (int)other.Number;
        
        return Mathf.Abs(currentNumber - otherNumber) == 1;
    }
} 