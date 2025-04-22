using Fusion;
using UnityEngine;
using System;

public class NetworkTile : NetworkBehaviour
{
    [Networked] public TileColor Color { get; set; }
    [Networked] public TileNumber Number { get; set; }
    [Networked] public NetworkBool IsFake { get; set; }
    [Networked] public NetworkBool IsSelected { get; set; }
    [Networked] public NetworkId OwnerId { get; set; }

    private NetworkPlayer owner;
    public event Action<NetworkPlayer> OnOwnerChanged;

    public void Initialize(TileColor color, TileNumber number, bool isFake = false)
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkTile: Initialize called without state authority");
            return;
        }

        Color = color;
        Number = number;
        IsFake = isFake;
        IsSelected = false;
        OwnerId = NetworkId.None;
    }

    public void SetOwner(NetworkPlayer player)
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogError("NetworkTile: SetOwner called without state authority");
            return;
        }

        var oldOwner = owner;
        owner = player;
        OwnerId = player != null ? player.Object.Id : NetworkId.None;

        if (oldOwner != owner)
        {
            OnOwnerChanged?.Invoke(owner);
        }
    }

    public NetworkPlayer GetOwner()
    {
        if (owner == null && OwnerId != NetworkId.None)
        {
            owner = Runner.FindObject<NetworkPlayer>(OwnerId);
            if (owner == null)
            {
                Debug.LogWarning($"NetworkTile: Could not find owner with ID {OwnerId}");
                OwnerId = NetworkId.None;
            }
        }
        return owner;
    }

    public bool CanBeNextTo(NetworkTile other)
    {
        if (other == null)
        {
            Debug.LogError("NetworkTile: CanBeNextTo called with null tile");
            return false;
        }

        if (Color != other.Color) return false;
        
        int currentNumber = (int)Number;
        int otherNumber = (int)other.Number;
        
        return Mathf.Abs(currentNumber - otherNumber) == 1;
    }

    public override string ToString()
    {
        return $"{Color} {Number}";
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        owner = null;
        OnOwnerChanged = null;
    }
} 