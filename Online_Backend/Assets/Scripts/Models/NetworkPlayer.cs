using Fusion;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public NetworkString<_128> PlayerName { get; set; }
    [Networked] public NetworkBool IsReady { get; set; }
    [Networked] public NetworkBool IsMyTurn { get; set; }
    [Networked] public int Score { get; set; }

    private List<NetworkTile> hand = new List<NetworkTile>();
    private List<NetworkTile> discardedTiles = new List<NetworkTile>();

    public event Action<NetworkTile> OnTileAdded;
    public event Action<NetworkTile> OnTileRemoved;
    public event Action<NetworkTile> OnTileDiscarded;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            // Local player setup
            PlayerName = "Player " + Object.InputAuthority.PlayerId;
            IsReady = false;
            IsMyTurn = false;
            Score = 0;
        }
    }

    public void AddTile(NetworkTile tile)
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkPlayer: AddTile called without state authority");
            return;
        }

        if (tile == null)
        {
            Debug.LogError("NetworkPlayer: AddTile called with null tile");
            return;
        }

        hand.Add(tile);
        tile.SetOwner(this);
        OnTileAdded?.Invoke(tile);
        GameEvents.OnTileDrawn?.Invoke(this, tile);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_DiscardTile(NetworkId tileId)
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkPlayer: RPC_DiscardTile called without state authority");
            return;
        }

        var tile = Runner.FindObject<NetworkTile>(tileId);
        if (tile == null)
        {
            Debug.LogError($"NetworkPlayer: Could not find tile with ID {tileId}");
            return;
        }

        if (!hand.Contains(tile))
        {
            Debug.LogError("NetworkPlayer: Attempted to discard tile not in hand");
            return;
        }

        hand.Remove(tile);
        discardedTiles.Add(tile);
        tile.SetOwner(null);
        OnTileRemoved?.Invoke(tile);
        OnTileDiscarded?.Invoke(tile);
        GameEvents.OnTileDiscarded?.Invoke(this, tile);
    }

    public void DiscardTile(NetworkTile tile)
    {
        if (!Object.HasInputAuthority)
        {
            Debug.LogError("NetworkPlayer: DiscardTile called without input authority");
            return;
        }

        if (tile == null)
        {
            Debug.LogError("NetworkPlayer: DiscardTile called with null tile");
            return;
        }

        RPC_DiscardTile(tile.Object.Id);
    }

    public void AddScore(int points)
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkPlayer: AddScore called without state authority");
            return;
        }

        Score += points;
    }

    public bool HasWinningHand()
    {
        // Burada okey oyununun kazanma mantığı implement edilecek
        // Örnek: Seri veya grup kontrolü
        return false;
    }

    public List<NetworkTile> GetHand()
    {
        return new List<NetworkTile>(hand);
    }

    public List<NetworkTile> GetDiscardedTiles()
    {
        return new List<NetworkTile>(discardedTiles);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Cleanup when player leaves
        foreach (var tile in hand)
        {
            if (tile != null)
            {
                tile.SetOwner(null);
            }
        }
        hand.Clear();
        discardedTiles.Clear();

        // Clear events
        OnTileAdded = null;
        OnTileRemoved = null;
        OnTileDiscarded = null;
    }
} 