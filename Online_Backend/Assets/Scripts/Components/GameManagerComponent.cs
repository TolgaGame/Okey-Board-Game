using UnityEngine;

public class GameManagerComponent : MonoBehaviour
{
    private GameManager gameManager;
    private GameLogic gameLogic;

    private void Awake()
    {
        // Servisleri oluştur ve kaydet
        gameManager = new GameManager();
        gameLogic = new GameLogic(gameManager);
        
        ServiceLocator.Instance.Register(gameManager);
        ServiceLocator.Instance.Register(gameLogic);

        // Event'leri dinle
        GameEvents.OnGameStarted += HandleGameStarted;
        GameEvents.OnGameEnded += HandleGameEnded;
        GameEvents.OnPlayerWon += HandlePlayerWon;
    }

    private void OnDestroy()
    {
        // Event'leri temizle
        GameEvents.OnGameStarted -= HandleGameStarted;
        GameEvents.OnGameEnded -= HandleGameEnded;
        GameEvents.OnPlayerWon -= HandlePlayerWon;

        // Servisleri temizle
        ServiceLocator.Instance.Unregister<GameManager>();
        ServiceLocator.Instance.Unregister<GameLogic>();
    }

    private void HandleGameStarted()
    {
        Debug.Log("Game started!");
    }

    private void HandleGameEnded()
    {
        Debug.Log("Game ended!");
    }

    private void HandlePlayerWon(Player player)
    {
        Debug.Log($"Player {player.Name} won the game!");
    }

    // Unity Inspector'dan test için kullanılacak metodlar
    public void StartGame()
    {
        gameManager.StartGame();
    }

    public void EndGame()
    {
        gameManager.EndGame();
    }

    public void AddTestPlayer(string playerName)
    {
        var player = new Player(System.Guid.NewGuid().ToString(), playerName);
        gameManager.AddPlayer(player);
    }
} 