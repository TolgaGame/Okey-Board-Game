using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private List<Player> players;
    private List<Tile> tilePool;
    private int currentPlayerIndex;
    private bool isGameStarted;
    private const int MAX_PLAYERS = 4;
    private const int TILES_PER_PLAYER = 14;

    public GameManager()
    {
        players = new List<Player>();
        tilePool = new List<Tile>();
        currentPlayerIndex = 0;
        isGameStarted = false;
        InitializeTilePool();
    }

    private void InitializeTilePool()
    {
        // Her renk için 1-13 arası taşları oluştur
        foreach (TileColor color in System.Enum.GetValues(typeof(TileColor)))
        {
            for (int i = 1; i <= 13; i++)
            {
                tilePool.Add(new Tile(color, (TileNumber)i));
            }
        }

        // Sahte okey taşlarını ekle
        tilePool.Add(new Tile(TileColor.Red, TileNumber.One, true));
        tilePool.Add(new Tile(TileColor.Blue, TileNumber.One, true));
        tilePool.Add(new Tile(TileColor.Yellow, TileNumber.One, true));
        tilePool.Add(new Tile(TileColor.Black, TileNumber.One, true));
    }

    public bool AddPlayer(Player player)
    {
        if (players.Count >= MAX_PLAYERS || isGameStarted)
            return false;

        players.Add(player);
        GameEvents.OnPlayerJoined?.Invoke(player);
        return true;
    }

    public void RemovePlayer(Player player)
    {
        if (players.Remove(player))
        {
            GameEvents.OnPlayerLeft?.Invoke(player);
            if (isGameStarted)
            {
                CheckGameEnd();
            }
        }
    }

    public void StartGame()
    {
        if (players.Count < 2 || isGameStarted)
            return;

        isGameStarted = true;
        ShuffleTilePool();
        DealInitialTiles();
        GameEvents.OnGameStarted?.Invoke();
    }

    private void ShuffleTilePool()
    {
        for (int i = tilePool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = tilePool[i];
            tilePool[i] = tilePool[j];
            tilePool[j] = temp;
        }
    }

    private void DealInitialTiles()
    {
        foreach (var player in players)
        {
            for (int i = 0; i < TILES_PER_PLAYER; i++)
            {
                if (tilePool.Count > 0)
                {
                    var tile = tilePool[0];
                    tilePool.RemoveAt(0);
                    player.AddTile(tile);
                }
            }
        }
    }

    public void NextTurn()
    {
        if (!isGameStarted || tilePool.Count == 0)
            return;

        var currentPlayer = players[currentPlayerIndex];
        GameEvents.OnPlayerTurnEnded?.Invoke(currentPlayer);

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        currentPlayer = players[currentPlayerIndex];
        currentPlayer.IsMyTurn = true;

        if (tilePool.Count > 0)
        {
            var drawnTile = tilePool[0];
            tilePool.RemoveAt(0);
            currentPlayer.AddTile(drawnTile);
        }

        GameEvents.OnPlayerTurnStarted?.Invoke(currentPlayer);
    }

    private void CheckGameEnd()
    {
        if (players.Count < 2)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        isGameStarted = false;
        currentPlayerIndex = 0;
        GameEvents.OnGameEnded?.Invoke();
    }

    public Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public List<Player> GetPlayers()
    {
        return new List<Player>(players);
    }

    public int GetRemainingTiles()
    {
        return tilePool.Count;
    }
} 