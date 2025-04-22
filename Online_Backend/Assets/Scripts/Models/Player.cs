using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public List<Tile> Hand { get; private set; }
    public List<Tile> DiscardedTiles { get; private set; }
    public bool IsReady { get; set; }
    public int Score { get; private set; }
    public bool IsMyTurn { get; set; }

    public Player(string id, string name)
    {
        Id = id;
        Name = name;
        Hand = new List<Tile>();
        DiscardedTiles = new List<Tile>();
        Score = 0;
    }

    public void AddTile(Tile tile)
    {
        Hand.Add(tile);
        GameEvents.OnTileDrawn?.Invoke(this, tile);
    }

    public void DiscardTile(Tile tile)
    {
        if (Hand.Remove(tile))
        {
            DiscardedTiles.Add(tile);
            GameEvents.OnTileDiscarded?.Invoke(this, tile);
        }
    }

    public void AddScore(int points)
    {
        Score += points;
    }

    public bool HasWinningHand()
    {
        // Burada okey oyununun kazanma mantığı implement edilecek
        // Örnek: Seri veya grup kontrolü
        return false;
    }
} 