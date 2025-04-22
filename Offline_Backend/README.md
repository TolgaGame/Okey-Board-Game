# Türk Okey Oyunu - Unity Backend

Bu proje, Unity ile geliştirilmiş Türk Okey oyununun backend yapısını içerir. Service Locator pattern, event system ve SOLID prensiplerini kullanarak modern ve sürdürülebilir bir mimari sunar.

## Özellikler

- Service Locator pattern ile servis yönetimi
- Event-driven mimari
- SOLID prensiplerine uygun tasarım
- Modüler ve genişletilebilir yapı
- Temiz kod prensipleri
- Oyun kuralları ve mantığı
- Puan hesaplama sistemi
- Seri ve grup kombinasyonları kontrolü

## Proje Yapısı

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── ServiceLocator.cs
│   │   └── GameEvents.cs
│   ├── Models/
│   │   ├── Tile.cs
│   │   └── Player.cs
│   ├── Managers/
│   │   └── GameManager.cs
│   └── Logic/
│       └── GameLogic.cs
```

## Kurulum

1. Unity projenizi açın
2. `Assets/Scripts` klasörünü projenize ekleyin
3. Unity Editor'de yeni bir boş GameObject oluşturun ve adını "GameManager" olarak değiştirin
4. GameManager GameObject'ine `GameManagerComponent` script'ini ekleyin

## Unity Editor Kurulumu

1. Hierarchy penceresinde sağ tıklayın ve Create Empty seçin
2. Oluşturulan GameObject'in adını "GameManager" olarak değiştirin
3. Inspector penceresinde Add Component butonuna tıklayın
4. "GameManagerComponent" script'ini ekleyin

## Kullanım

### 1. Servislerin Kaydedilmesi

```csharp
// GameManagerComponent.cs içinde
private void Awake()
{
    var gameManager = new GameManager();
    var gameLogic = new GameLogic(gameManager);
    
    ServiceLocator.Instance.Register(gameManager);
    ServiceLocator.Instance.Register(gameLogic);
}
```

### 2. Oyuncuların Eklenmesi

```csharp
// Örnek: LobbyManager.cs içinde
public class LobbyManager : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = ServiceLocator.Instance.Get<GameManager>();
    }

    public void AddPlayer(string playerName)
    {
        var player = new Player(System.Guid.NewGuid().ToString(), playerName);
        if (gameManager.AddPlayer(player))
        {
            Debug.Log($"Player {playerName} added successfully");
        }
    }
}
```

### 3. Event'lerin Dinlenmesi

```csharp
// Örnek: UIManager.cs içinde
public class UIManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnPlayerTurnStarted += HandlePlayerTurnStarted;
        GameEvents.OnTileDrawn += HandleTileDrawn;
        GameEvents.OnGameStarted += HandleGameStarted;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerTurnStarted -= HandlePlayerTurnStarted;
        GameEvents.OnTileDrawn -= HandleTileDrawn;
        GameEvents.OnGameStarted -= HandleGameStarted;
    }

    private void HandlePlayerTurnStarted(Player player)
    {
        // UI güncelleme işlemleri
        Debug.Log($"{player.Name}'s turn started");
    }

    private void HandleTileDrawn(Player player, Tile tile)
    {
        // UI güncelleme işlemleri
        Debug.Log($"{player.Name} drew {tile}");
    }

    private void HandleGameStarted()
    {
        // UI güncelleme işlemleri
        Debug.Log("Game started!");
    }
}
```

### 4. Oyun Mantığının Kullanımı

```csharp
// Örnek: GameController.cs içinde
public class GameController : MonoBehaviour
{
    private GameLogic gameLogic;
    private GameManager gameManager;

    private void Start()
    {
        gameLogic = ServiceLocator.Instance.Get<GameLogic>();
        gameManager = ServiceLocator.Instance.Get<GameManager>();
    }

    public void OnTileSelected(Tile tile)
    {
        var currentPlayer = gameManager.GetCurrentPlayer();
        
        if (gameLogic.IsValidMove(currentPlayer, tile))
        {
            currentPlayer.DiscardTile(tile);
            gameManager.NextTurn();
        }
    }
}
```

## Oyun Kuralları

### Taşlar
- Her renk için 1-13 arası sayılar
- 4 farklı renk: Kırmızı, Mavi, Sarı, Siyah
- Her renk için 1 adet sahte okey taşı

### Oyun Akışı
1. Her oyuncuya 14 taş dağıtılır
2. Sırayla oyuncular taş çeker ve atar
3. Kazanma için:
   - Seri kombinasyonu (aynı renkte ardışık 3 taş)
   - Grup kombinasyonu (farklı renklerde aynı sayıdan 3 taş)

### Puanlama
- Seri kombinasyonu: 10 puan
- Grup kombinasyonu: 15 puan

## Örnek Senaryo

```csharp
// Oyun başlatma
public void StartGame()
{
    // Servisleri kaydet
    var gameManager = new GameManager();
    var gameLogic = new GameLogic(gameManager);
    ServiceLocator.Instance.Register(gameManager);
    ServiceLocator.Instance.Register(gameLogic);

    // Oyuncuları ekle
    var player1 = new Player("1", "Player1");
    var player2 = new Player("2", "Player2");
    gameManager.AddPlayer(player1);
    gameManager.AddPlayer(player2);

    // Event'leri dinle
    GameEvents.OnPlayerTurnStarted += (player) => {
        Debug.Log($"{player.Name}'s turn started");
    };

    // Oyunu başlat
    gameManager.StartGame();
}
```

## Unity Editor'de Test

1. GameManager GameObject'ini seçin
2. Inspector'da GameManagerComponent ayarlarını yapın
3. Play moduna geçin
4. Console penceresinde event'leri ve oyun akışını takip edin

## Öneriler

1. UI elementlerini ayrı bir Canvas altında organize edin
2. Event'leri dinleyen script'leri OnEnable/OnDisable metodlarında yönetin
3. ServiceLocator'ı sadece global servisler için kullanın
4. Oyun mantığını GameLogic sınıfında tutun
5. UI güncellemelerini event'ler üzerinden yapın

## Hata Ayıklama

1. ServiceLocator'da servis kaydı yapılmadığında exception fırlatılır
2. Geçersiz hamleler GameLogic tarafından kontrol edilir
3. Event'ler null check ile çağrılır
4. Oyun durumu GameManager tarafından yönetilir

## Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun 