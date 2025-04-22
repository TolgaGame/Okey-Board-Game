using Fusion;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetworkGameManager : NetworkBehaviour
{
    [Networked] private NetworkString<_128> GameState { get; set; }
    [Networked] private int CurrentPlayerIndex { get; set; }
    [Networked] private TickTimer GameStartTimer { get; set; }
    
    private const int MAX_PLAYERS = 4;
    private const int TILES_PER_PLAYER = 14;
    private const float GAME_START_DELAY = 3f;

    private List<NetworkPlayer> players = new List<NetworkPlayer>();
    private List<NetworkTile> tilePool = new List<NetworkTile>();

    public event Action<NetworkPlayer> OnPlayerAdded;
    public event Action<NetworkPlayer> OnPlayerRemoved;
    public event Action OnGameStarted;
    public event Action OnGameEnded;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            InitializeTilePool();
            GameState = "WaitingForPlayers";
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        switch (GameState.ToString())
        {
            case "WaitingForPlayers":
                if (players.Count >= 2 && players.Count <= MAX_PLAYERS)
                {
                    if (GameStartTimer.Expired(Runner))
                    {
                        StartGame();
                    }
                }
                break;

            case "Playing":
                if (players.Count < 2)
                {
                    EndGame();
                }
                break;
        }
    }

    public void AddPlayer(NetworkPlayer player)
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkGameManager: AddPlayer called without state authority");
            return;
        }

        if (player == null)
        {
            Debug.LogError("NetworkGameManager: AddPlayer called with null player");
            return;
        }

        if (players.Count >= MAX_PLAYERS)
        {
            Debug.LogError("NetworkGameManager: Maximum players reached");
            return;
        }

        if (GameState.ToString() != "WaitingForPlayers")
        {
            Debug.LogError("NetworkGameManager: Cannot add player while game is in progress");
            return;
        }

        players.Add(player);
        OnPlayerAdded?.Invoke(player);
        GameEvents.OnPlayerJoined?.Invoke(player);
        
        if (players.Count >= 2)
        {
            GameStartTimer = TickTimer.CreateFromSeconds(Runner, GAME_START_DELAY);
        }
    }

    public void RemovePlayer(NetworkPlayer player)
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkGameManager: RemovePlayer called without state authority");
            return;
        }

        if (player == null)
        {
            Debug.LogError("NetworkGameManager: RemovePlayer called with null player");
            return;
        }

        if (players.Remove(player))
        {
            OnPlayerRemoved?.Invoke(player);
            GameEvents.OnPlayerLeft?.Invoke(player);
            
            if (GameState.ToString() == "Playing")
            {
                CheckGameEnd();
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StartGame()
    {
        GameState = "Playing";
        ShuffleTilePool();
        DealInitialTiles();
        OnGameStarted?.Invoke();
        GameEvents.OnGameStarted?.Invoke();
    }

    private void StartGame()
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkGameManager: StartGame called without state authority");
            return;
        }

        RPC_StartGame();
    }

    private void InitializeTilePool()
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkGameManager: InitializeTilePool called without state authority");
            return;
        }

        // Her renk için 1-13 arası taşları oluştur
        foreach (TileColor color in System.Enum.GetValues(typeof(TileColor)))
        {
            for (int i = 1; i <= 13; i++)
            {
                var tile = Runner.Spawn<NetworkTile>();
                tile.Initialize(color, (TileNumber)i);
                tilePool.Add(tile);
            }
        }

        // Sahte okey taşlarını ekle
        for (int i = 0; i < 4; i++)
        {
            var tile = Runner.Spawn<NetworkTile>();
            tile.Initialize((TileColor)i, TileNumber.One, true);
            tilePool.Add(tile);
        }
    }

    private void ShuffleTilePool()
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkGameManager: ShuffleTilePool called without state authority");
            return;
        }

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
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkGameManager: DealInitialTiles called without state authority");
            return;
        }

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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NextTurn()
    {
        if (tilePool.Count == 0) return;

        var currentPlayer = players[CurrentPlayerIndex];
        GameEvents.OnPlayerTurnEnded?.Invoke(currentPlayer);

        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Count;
        currentPlayer = players[CurrentPlayerIndex];
        currentPlayer.IsMyTurn = true;

        if (tilePool.Count > 0)
        {
            var drawnTile = tilePool[0];
            tilePool.RemoveAt(0);
            currentPlayer.AddTile(drawnTile);
        }

        GameEvents.OnPlayerTurnStarted?.Invoke(currentPlayer);
    }

    public void NextTurn()
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkGameManager: NextTurn called without state authority");
            return;
        }

        RPC_NextTurn();
    }

    private void CheckGameEnd()
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkGameManager: CheckGameEnd called without state authority");
            return;
        }

        if (players.Count < 2)
        {
            EndGame();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_EndGame()
    {
        GameState = "WaitingForPlayers";
        CurrentPlayerIndex = 0;
        OnGameEnded?.Invoke();
        GameEvents.OnGameEnded?.Invoke();
    }

    private void EndGame()
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkGameManager: EndGame called without state authority");
            return;
        }

        RPC_EndGame();
    }

    public NetworkPlayer GetCurrentPlayer()
    {
        if (players.Count == 0)
        {
            Debug.LogError("NetworkGameManager: GetCurrentPlayer called with no players");
            return null;
        }

        return players[CurrentPlayerIndex];
    }

    public List<NetworkPlayer> GetPlayers()
    {
        return new List<NetworkPlayer>(players);
    }

    public int GetRemainingTiles()
    {
        return tilePool.Count;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Clear events
        OnPlayerAdded = null;
        OnPlayerRemoved = null;
        OnGameStarted = null;
        OnGameEnded = null;

        // Cleanup
        players.Clear();
        tilePool.Clear();
    }
} 