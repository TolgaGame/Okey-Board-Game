# Türk Okey Oyunu - Unity Multiplayer

Bu proje, Unity ve Photon Fusion 2 kullanılarak geliştirilmiş çok oyunculu bir Türk Okey oyunudur. Service Locator pattern, event system ve SOLID prensiplerini kullanarak modern ve sürdürülebilir bir mimari sunar.

## Özellikler

- Photon Fusion 2 ile çok oyunculu destek
- Service Locator pattern ile servis yönetimi
- Event-driven mimari
- SOLID prensiplerine uygun tasarım
- Modüler ve genişletilebilir yapı
- Temiz kod prensipleri
- Oyun kuralları ve mantığı
- Puan hesaplama sistemi
- Seri ve grup kombinasyonları kontrolü
- Güvenli ağ senkronizasyonu
- Hata yönetimi ve loglama sistemi

## Proje Yapısı

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── ServiceLocator.cs
│   │   └── GameEvents.cs
│   ├── Models/
│   │   ├── NetworkTile.cs
│   │   └── NetworkPlayer.cs
│   ├── Managers/
│   │   ├── NetworkManager.cs
│   │   └── NetworkGameManager.cs
│   └── Logic/
│       └── GameLogic.cs
```

## Kurulum

1. Unity projenizi açın
2. Photon Fusion 2 paketini yükleyin
3. `Assets/Scripts` klasörünü projenize ekleyin
4. Unity Editor'de yeni bir boş GameObject oluşturun ve adını "NetworkManager" olarak değiştirin
5. NetworkManager GameObject'ine `NetworkManager` script'ini ekleyin
6. NetworkRunner prefabını oluşturun ve NetworkManager'a atayın
7. NetworkGameManager ve NetworkPlayer prefablarını oluşturun ve NetworkManager'a atayın

## Unity Editor Kurulumu

1. Hierarchy penceresinde sağ tıklayın ve Create Empty seçin
2. Oluşturulan GameObject'in adını "NetworkManager" olarak değiştirin
3. Inspector penceresinde Add Component butonuna tıklayın
4. "NetworkManager" script'ini ekleyin
5. NetworkRunner, NetworkGameManager ve NetworkPlayer prefablarını ilgili alanlara sürükleyin

## Kullanım

### 1. Oyunu Başlatma

```csharp
// Host olarak oyun başlatma
await NetworkManager.Instance.StartGame(GameMode.Host, "OkeyRoom");

// Client olarak oyuna katılma
await NetworkManager.Instance.StartGame(GameMode.Client, "OkeyRoom");
```

### 2. Event'lerin Dinlenmesi

```csharp
public class UIManager : MonoBehaviour
{
    private NetworkGameManager gameManager;
    private NetworkPlayer localPlayer;

    private void Start()
    {
        gameManager = FindObjectOfType<NetworkGameManager>();
        if (gameManager != null)
        {
            gameManager.OnPlayerAdded += HandlePlayerAdded;
            gameManager.OnPlayerRemoved += HandlePlayerRemoved;
            gameManager.OnGameStarted += HandleGameStarted;
            gameManager.OnGameEnded += HandleGameEnded;
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnPlayerAdded -= HandlePlayerAdded;
            gameManager.OnPlayerRemoved -= HandlePlayerRemoved;
            gameManager.OnGameStarted -= HandleGameStarted;
            gameManager.OnGameEnded -= HandleGameEnded;
        }
    }

    private void HandlePlayerAdded(NetworkPlayer player)
    {
        Debug.Log($"Player {player.PlayerName} joined the game");
    }

    private void HandlePlayerRemoved(NetworkPlayer player)
    {
        Debug.Log($"Player {player.PlayerName} left the game");
    }

    private void HandleGameStarted()
    {
        Debug.Log("Game started!");
    }

    private void HandleGameEnded()
    {
        Debug.Log("Game ended!");
    }
}
```

### 3. Taş İşlemleri

```csharp
public class TileController : MonoBehaviour
{
    private NetworkPlayer localPlayer;
    private NetworkTile selectedTile;

    private void Start()
    {
        localPlayer = GetComponent<NetworkPlayer>();
        if (localPlayer != null)
        {
            localPlayer.OnTileAdded += HandleTileAdded;
            localPlayer.OnTileRemoved += HandleTileRemoved;
            localPlayer.OnTileDiscarded += HandleTileDiscarded;
        }
    }

    public void OnTileSelected(NetworkTile tile)
    {
        if (localPlayer != null && localPlayer.IsMyTurn)
        {
            selectedTile = tile;
            tile.IsSelected = true;
        }
    }

    public void OnDiscardButtonClicked()
    {
        if (selectedTile != null && localPlayer != null && localPlayer.IsMyTurn)
        {
            localPlayer.DiscardTile(selectedTile);
            selectedTile = null;
        }
    }

    private void HandleTileAdded(NetworkTile tile)
    {
        Debug.Log($"Tile {tile} added to hand");
    }

    private void HandleTileRemoved(NetworkTile tile)
    {
        Debug.Log($"Tile {tile} removed from hand");
    }

    private void HandleTileDiscarded(NetworkTile tile)
    {
        Debug.Log($"Tile {tile} discarded");
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

## Ağ Yapılandırması

### NetworkManager
- Oyun oturumunu yönetir
- Oyuncu bağlantılarını kontrol eder
- GameManager ve Player prefablarını spawn eder
- Ağ olaylarını yönetir

### NetworkGameManager
- Oyun durumunu senkronize eder
- Taş havuzunu yönetir
- Oyuncu sırasını kontrol eder
- Oyun kurallarını uygular

### NetworkPlayer
- Oyuncu verilerini senkronize eder
- Elindeki taşları yönetir
- Taş çekme ve atma işlemlerini gerçekleştirir
- Puan hesaplamasını yapar

### NetworkTile
- Taş verilerini senkronize eder
- Taş sahibini yönetir
- Taş kombinasyonlarını kontrol eder

## Hata Yönetimi

Proje, kapsamlı bir hata yönetimi sistemi içerir:

1. **State Authority Kontrolleri**
   - Tüm kritik işlemler için state authority kontrolü
   - Yetkisiz işlemlerin engellenmesi
   - Detaylı hata mesajları

2. **Null Kontrolleri**
   - Tüm nesne referansları için null kontrolü
   - Güvenli nesne erişimi
   - Hata durumlarında güvenli çıkış

3. **Event Yönetimi**
   - Event'lerin güvenli tetiklenmesi
   - Event cleanup işlemleri
   - Memory leak önleme

4. **Ağ Hataları**
   - Bağlantı hatalarının yönetimi
   - Otomatik yeniden bağlanma
   - Kullanıcı bilgilendirme

## Öneriler

1. UI elementlerini ayrı bir Canvas altında organize edin
2. Event'leri dinleyen script'leri OnEnable/OnDisable metodlarında yönetin
3. NetworkManager'ı sadece global servisler için kullanın
4. Oyun mantığını NetworkGameManager sınıfında tutun
5. UI güncellemelerini event'ler üzerinden yapın
6. Hata mesajlarını kullanıcıya gösterin
7. Ağ bağlantısı durumunu sürekli kontrol edin

## Hata Ayıklama

1. NetworkManager'da servis kaydı yapılmadığında exception fırlatılır
2. Geçersiz hamleler NetworkGameManager tarafından kontrol edilir
3. Event'ler null check ile çağrılır
4. Oyun durumu NetworkGameManager tarafından yönetilir
5. Ağ bağlantısı durumu sürekli izlenir
6. Hata mesajları detaylı loglanır

## Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun 