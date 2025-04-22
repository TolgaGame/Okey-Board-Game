using Fusion;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] private NetworkGameManager networkGameManagerPrefab;
    [SerializeField] private NetworkPlayer networkPlayerPrefab;

    private NetworkGameManager gameManager;
    private NetworkPlayer localPlayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupNetworkRunner();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupNetworkRunner()
    {
        networkRunner.AddCallbacks(this);
    }

    public async Task StartGame(GameMode mode, string roomName)
    {
        try
        {
            var startGameArgs = new StartGameArgs()
            {
                GameMode = mode,
                SessionName = roomName,
                Scene = SceneManager.GetActiveScene().buildIndex
            };

            var result = await networkRunner.StartGame(startGameArgs);
            
            if (result.Ok)
            {
                Debug.Log($"Game started successfully in {mode} mode");
            }
            else
            {
                Debug.LogError($"Failed to start game: {result.ShutdownReason}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error starting game: {e.Message}");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            var playerObject = runner.Spawn(networkPlayerPrefab);
            var networkPlayer = playerObject.GetComponent<NetworkPlayer>();
            networkPlayer.Object.AssignInputAuthority(player);
            
            if (gameManager == null)
            {
                var gameManagerObject = runner.Spawn(networkGameManagerPrefab);
                gameManager = gameManagerObject.GetComponent<NetworkGameManager>();
            }
            
            gameManager.AddPlayer(networkPlayer);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer && gameManager != null)
        {
            var playerObject = runner.GetPlayerObject(player);
            if (playerObject != null)
            {
                var networkPlayer = playerObject.GetComponent<NetworkPlayer>();
                if (networkPlayer != null)
                {
                    gameManager.RemovePlayer(networkPlayer);
                }
            }
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"Game shutdown: {shutdownReason}");
        
        if (gameManager != null)
        {
            Destroy(gameManager.gameObject);
            gameManager = null;
        }
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        request.Accept();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"Connection failed: {reason}");
        // UI'da hata mesajı göster
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        // Handle custom simulation messages
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // Handle session list updates
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        // Handle custom authentication responses
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        // Handle host migration
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        // Handle reliable data received
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene load completed");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene load started");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Handle network input
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // Handle missing input
    }
} 