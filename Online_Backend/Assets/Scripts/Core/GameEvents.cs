using System;
using System.Collections.Generic;

public static class GameEvents
{
    public static event Action<Player> OnPlayerJoined;
    public static event Action<Player> OnPlayerLeft;
    public static event Action<Player, Tile> OnTileDrawn;
    public static event Action<Player, Tile> OnTileDiscarded;
    public static event Action<Player> OnPlayerTurnStarted;
    public static event Action<Player> OnPlayerTurnEnded;
    public static event Action<Player> OnPlayerWon;
    public static event Action OnGameStarted;
    public static event Action OnGameEnded;
    public static event Action OnRoundStarted;
    public static event Action OnRoundEnded;
} 