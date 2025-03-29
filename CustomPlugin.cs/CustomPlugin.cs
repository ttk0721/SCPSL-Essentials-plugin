using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PlayerRoles;
using PlayerRoles.PlayableScps;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Interactables.Interobjects.DoorUtils;
using MapGeneration.Distributors;
using Mirror; // Dodane odwołanie do Mirror dla NetworkBehaviour

// Definiujemy brakujący enum
public enum EffectType
{
    Concussed,
    Visuals939
    // Dodaj więcej typów efektów w razie potrzeby
}

public class CustomPlugin
{
    // Rozszerzone zmienne stanu
    private bool roundInProgress = false;
    private bool spyAssigned = false;
    // Jawne określenie typu Random aby uniknąć niejednoznaczności
    private static System.Random rng = new System.Random();
    private readonly ItemType Scp035ItemType = ItemType.KeycardO5;
    private HashSet<GameObject> brokenElevators = new HashSet<GameObject>();
    private DateTime roundStartTime;
    private readonly TimeSpan lateJoinTime = TimeSpan.FromMinutes(1);
    private bool lateJoinEnabled = true;
    private bool voiceChatEnabled = false;
    private Dictionary<Player, DateTime> playerJoinTimes = new Dictionary<Player, DateTime>();

    // Nowe efekty monetki
    private readonly int coinOutcomes = 25; // Zwiększona liczba efektów do 25
    private Dictionary<Player, UnityEngine.Coroutine> activeCoroutines = new Dictionary<Player, UnityEngine.Coroutine>();

    [PluginEntryPoint("CustomPlugin", "2.0.0", "Rozbudowany plugin z dodatkowymi funkcjami", "Autor")]
    private void OnLoaded()
    {
        EventManager.RegisterEvents(this);
        Log.Info("CustomPlugin 2.0 załadowany!");
        InitializeVoiceChat();
        SetupCustomItems();
    }

    // Implementacja brakującej metody
    private void InitializeVoiceChat()
    {
        voiceChatEnabled = true;
        Log.Info("Inicjalizacja czatu głosowego...");
    }

    // Rozszerzone zdarzenie rzutu monetą
    [PluginEvent]
    private void OnPlayerCoinFlip(PlayerCoinFlipEvent ev)
    {
        Player player = ev.Player;
        int outcome = rng.Next(coinOutcomes);

        // Zastąpienie konstrukcji switch expression standardowym switch dla zgodności z C# 7.3
        string effectName;
        switch (outcome)
        {
            case 0: effectName = TeleportPlayer(player); break;
            case 1: effectName = HealPlayer(player); break;
            case 2: effectName = ChangePlayerClass(player); break;
            case 3: effectName = GrantTemporaryInvisibility(player); break;
            case 4: effectName = BoostPlayerSpeed(player); break;
            case 5: effectName = GiveRandomItem(player); break;
            case 6: effectName = GrantDamageImmunity(player); break;
            case 7: effectName = DropCurrentItem(player); break;
            case 8: effectName = SwapWithRandomPlayer(player); break;
            case 9: effectName = TeleportToRandomRoom(player); break;
            case 10: effectName = BoostDamageOutput(player); break;
            case 11: effectName = PullNearbyPlayers(player); break;
            case 12: effectName = DisguisePlayer(player); break;
            case 13: effectName = TemporaryAmnesia(player); break;
            case 14: effectName = SpawnGrenadeNearby(player); break;
            case 15: effectName = InvertPlayerControls(player); break;
            case 16: effectName = SwapInventoryWithRandom(player); break;
            case 17: effectName = ActivateNightVision(player); break;
            case 18: effectName = CreateDecoyClone(player); break;
            case 19: effectName = ShuffleAllPlayers(player); break;
            case 20: effectName = ToggleWeapons(player); break;
            case 21: effectName = EnableDoubleJump(player); break;
            case 22: effectName = CreateForceField(player); break;
            case 23: effectName = SummonItemRain(player); break;
            case 24: effectName = TimeShiftPlayers(player); break;
            default: effectName = "Nieznany efekt"; break;
        }

        Log.Info($"[CoinFlip] {player.Nickname} otrzymał efekt: {effectName}");
    }

    // Implementacje brakujących metod
    private string TeleportPlayer(Player player)
    {
        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-50, 50), 0, UnityEngine.Random.Range(-50, 50));
        player.Position = randomPosition;
        player.SendBroadcast("Zostałeś teleportowany!", 5);
        return "Teleportacja";
    }

    private string HealPlayer(Player player)
    {
        // Implementacja uzdrawiania gracza
        player.Health = player.MaxHealth;
        player.SendBroadcast("Zostałeś wyleczony!", 5);
        return "Leczenie";
    }

    private string ChangePlayerClass(Player player)
    {
        RoleTypeId newRole = (RoleTypeId)UnityEngine.Random.Range(0, (int)RoleTypeId.Filmmaker);
        player.Role = newRole;
        player.SendBroadcast($"Zmieniono twoją klasę na {newRole}!", 5);
        return "Zmiana klasy";
    }

    //private string GrantTemporaryInvisibility(Player player)
    //{
    //    player.IsInvisible = true;

    //     Dodajemy komponent EffectTimer do obiektu gracza
    //    player.GameObject.AddComponent<EffectTimer>().Initialize(
    //        player,
    //        () => player.IsInvisible = false,
    //        10f
    //    );

    //    player.SendBroadcast("Jesteś niewidzialny przez 10 sekund!", 5);
    //    return "Niewidzialność";
    //}


    //private string BoostPlayerSpeed(Player player)
    //{
    //    player.MoveSpeed *= 1.5f;
    //    UnityEngine.MonoBehaviour.Invoke(() => player.MoveSpeed /= 1.5f, 15f);
    //    player.SendBroadcast("Twoja prędkość została zwiększona na 15 sekund!", 5);
    //    return "Zwiększenie prędkości";
    //}

    private string GiveRandomItem(Player player)
    {
        ItemType randomItem = (ItemType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ItemType)).Length);
        player.AddItem(randomItem);
        player.SendBroadcast($"Otrzymałeś {randomItem}!", 5);
        return "Losowy przedmiot";
    }

    //private string GrantDamageImmunity(Player player)
    //{
    //    // Implementacja odporności na obrażenia
    //    player.IsGodModeEnabled = true;
    //    UnityEngine.MonoBehaviour.Invoke(() => player.IsGodModeEnabled = false, 20f);
    //    player.SendBroadcast("Jesteś odporny na obrażenia przez 20 sekund!", 5);
    //    return "Odporność na obrażenia";
    //}

    //private string DropCurrentItem(Player player)
    //{
    //    if (player.CurrentItem != null)
    //    {
    //        player.DropHeldItem();
    //        player.SendBroadcast("Upuściłeś swój przedmiot!", 5);
    //    }
    //    return "Upuszczenie przedmiotu";
    //}

    private string SwapWithRandomPlayer(Player player)
    {
        Player randomPlayer = Player.GetPlayers()
            .Where(p => p != player && p.IsAlive)
            .OrderBy(_ => UnityEngine.Random.value)
            .FirstOrDefault();
            
        if (randomPlayer != null)
        {
            Vector3 tempPosition = player.Position;
            player.Position = randomPlayer.Position;
            randomPlayer.Position = tempPosition;
            player.SendBroadcast($"Zamieniłeś się miejscami z {randomPlayer.Nickname}!", 5);
        }
        return "Zamiana miejscami";
    }

    private string TeleportToRandomRoom(Player player)
    {
        // Implementacja teleportacji do losowego pokoju
        Vector3 randomRoomPosition = new Vector3(UnityEngine.Random.Range(-100, 100), 0, UnityEngine.Random.Range(-100, 100));
        player.Position = randomRoomPosition;
        player.SendBroadcast("Teleportowano cię do losowego pomieszczenia!", 5);
        return "Teleportacja do pomieszczenia";
    }

    //private string BoostDamageOutput(Player player)
    //{
    //    // Implementacja zwiększenia obrażeń
    //    player.DamageMultiplier = 2.0f;
    //    UnityEngine.MonoBehaviour.Invoke(() => player.DamageMultiplier = 1.0f, 15f);
    //    player.SendBroadcast("Twoje obrażenia zostały zwiększone na 15 sekund!", 5);
    //    return "Zwiększenie obrażeń";
    //}

    private string PullNearbyPlayers(Player player)
    {
        foreach (Player nearbyPlayer in Player.GetPlayers()
            .Where(p => p != player && p.IsAlive && Vector3.Distance(p.Position, player.Position) < 20f))
        {
            nearbyPlayer.Position = player.Position + (nearbyPlayer.Position - player.Position).normalized * 2f;
        }
        player.SendBroadcast("Przyciągnięto pobliskich graczy!", 5);
        return "Przyciągnięcie graczy";
    }

   //private string DisguisePlayer(Player player)
   // {
   //     // Implementacja przebrania gracza
   //     player.DisplayNickname = $"[FAKE] {Player.GetPlayers().OrderBy(_ => UnityEngine.Random.value).First().Nickname}";
   //     UnityEngine.MonoBehaviour.Invoke(() => player.DisplayNickname = player.Nickname, 30f);
   //     player.SendBroadcast("Zostałeś przebrany na 30 sekund!", 5);
   //     return "Przebranie";
   // }

    //private string TemporaryAmnesia(Player player)
    //{
    //    // Implementacja tymczasowej amnezji
    //    PlayerExtensions.ApplyEffect(player, EffectType.Concussed, 1, 15f);
    //    player.SendBroadcast("Masz chwilową amnezję! Niektóre funkcje są niedostępne przez 15 sekund.", 5);
    //    return "Amnezja";
    //}

    private string SpawnGrenadeNearby(Player player)
    {
        Vector3 grenadePosition = player.Position + new Vector3(UnityEngine.Random.Range(-5, 5), 1, UnityEngine.Random.Range(-5, 5));
        ItemType grenadeType = ItemType.GrenadeHE;
        ItemSpawnManager.SpawnItem(grenadeType, grenadePosition, Quaternion.identity);
        player.SendBroadcast("W pobliżu pojawił się granat!", 5);
        return "Pojawienie się granatu";
    }

    // Nowe efekty monetki (dodatkowe efekty)
    //private string InvertPlayerControls(Player player)
    //{
    //    PlayerExtensions.ApplyEffect(player, EffectType.Concussed, 1, 30f);
    //    player.SendBroadcast("Efekt: Kontrola odwrócona na 30s!", 5);
    //    return "Odwrócenie kontrolek";
    //}

    //private string SwapInventoryWithRandom(Player player)
    //{
    //    Player target = Player.GetPlayers()
    //        .Where(p => p != player && p.IsAlive)
    //        .OrderBy(_ => UnityEngine.Random.value)
    //        .FirstOrDefault();
            
    //    if (target != null)
    //    {
    //        var playerItems = player.Items.ToList();
    //        var targetItems = target.Items.ToList();

    //        // Poprawna implementacja wymiany przedmiotów
    //        foreach (var item in playerItems)
    //        {
    //            PlayerExtensions.RemoveItem(player, item);
    //        }

    //        foreach (var item in targetItems)
    //        {
    //            PlayerExtensions.RemoveItem(target, item);
    //        }

    //        foreach (var item in playerItems)
    //        {
    //            PlayerExtensions.AddItem(target, item);
    //        }

    //        foreach (var item in targetItems)
    //        {
    //            PlayerExtensions.AddItem(player, item);
    //        }

    //        return "Wymiana ekwipunku";
    //    }
    //    return "Brak celu wymiany";
    //}

    //private string ActivateNightVision(Player player)
    //{
    //    PlayerExtensions.ApplyEffect(player, EffectType.Visuals939, 1, 60f);
    //    player.SendBroadcast("Efekt: Noktowizja aktywna 60s!", 5);
    //    return "Noktowizja";
    //}

    //private string CreateDecoyClone(Player player)
    //{
    //    var dummy = Player.Create(player.Position);
    //    dummy.DisplayNickname = $"[DUMMY] {player.Nickname}";
    //    dummy.Role = player.Role;
    //    dummy.Scale = new Vector3(0.8f, 0.8f, 0.8f);

    //    dummy.GameObject.AddComponent<DecoyController>().Initialize(player, 30f);
    //    return "Stworzenie klona";
    //}

    private string ShuffleAllPlayers(Player player)
    {
        var players = Player.GetPlayers().Where(p => p.IsAlive).ToList();
        Extensions.ShuffleList(players);

        for (int i = 0; i < players.Count; i++)
        {
            if (i + 1 < players.Count)
            {
                Vector3 temp = players[i].Position;
                players[i].Position = players[i + 1].Position;
                players[i + 1].Position = temp;
            }
        }
        return "Przetasowanie pozycji";
    }

    //private string ToggleWeapons(Player player)
    //{
    //    bool state = rng.Next(2) == 0;
    //    PlayerExtensions.SetWeaponEnabled(player, !state); 
    //    player.SendBroadcast($"Efekt: Broń {(state ? "zablokowana" : "odblokowana")}!", 5);
    //    return "Przełącz bronie";
    //}

    //private string EnableDoubleJump(Player player)
    //{
    //    player.GameObject.AddComponent<DoubleJumpController>();
    //    player.SendBroadcast("Efekt: Podwójny skok aktywny!", 5);
    //    return "Podwójny skok";
    //}

    private string CreateForceField(Player player)
    {
        var shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shield.transform.position = player.Position;
        shield.transform.localScale = new Vector3(3f, 3f, 3f);
        shield.AddComponent<ForceFieldController>().Initialize(15f);
        return "Pole siłowe";
    }

    private string SummonItemRain(Player player)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 pos = player.Position + new Vector3(rng.Next(-5, 5), 10f, rng.Next(-5, 5));
            ItemType randomItem = ((ItemType[])Enum.GetValues(typeof(ItemType)))[rng.Next(Enum.GetValues(typeof(ItemType)).Length)];
            ItemSpawnManager.SpawnItem(randomItem, pos, Quaternion.identity);
        }
        return "Deszcz przedmiotów";
    }

    //private string TimeShiftPlayers(Player player)
    //{
    //    var players = Player.GetPlayers().Where(p => p.IsAlive).ToList();
    //    foreach (var p in players)
    //    {
    //        float timeShift = rng.Next(-60, 60);
    //        p.GameObject.AddComponent<TimeShiftController>().Initialize(timeShift);
    //    }
    //    return "Przesunięcie czasowe";
    //}

    // Rozszerzone komunikaty C.A.S.S.I.E.
    private string[] randomCassieLines = {
        "pitch_0.8 Attention . pitch_1.2 Unusual space-time anomalies detected .",
        "pitch_0.7 WARNING . pitch_1.5 Euclid class containment breach in progress .",
        "pitch_1.3 All personnel . pitch_0.9 Report to nearest evacuation point immediately .",
        // Pozostałe linie...
    };

    // Nowe funkcje pomocnicze
    private void SetupCustomItems()
    {
        EventManager.RegisterEvents<CustomItemHandler>(this); // Dodany brakujący parametr
    }

    // Rozszerzone zdarzenie interakcji z drzwiami
    //[PluginEvent(ServerEventType.PlayerInteractDoor)]
    //private void OnDoorInteract(Player player, DoorVariant door, bool canOpen)
    //{
    //    if (rng.Next(100) < 5 && door.RequiredPermissions.RequiredPermissions != KeycardPermissions.None)
    //    {
    //        door.ServerChangeLock(DoorLockReason.AdminCommand, true); // Dodany brakujący parametr
    //        DoorExtensions.SetOpen(door, !DoorExtensions.IsOpen(door));
    //        player.SendBroadcast("Drzwi zostały losowo przełączone!", 3);
    //    }
    //}

    // Nowy system czasu gry
    [PluginEvent(ServerEventType.PlayerJoined)]
    private void OnPlayerJoined(PlayerJoinedEvent ev)
    {
        playerJoinTimes[ev.Player] = DateTime.Now;
        if (voiceChatEnabled)
        {
            ev.Player.GameObject.AddComponent<VoiceModulator>().SetRandomVoice();
        }
    }

    // Dodatkowe komponenty
    //public class DoubleJumpController : MonoBehaviour
    //{
    //    private Player player;
    //    private int jumpsRemaining = 2;

    //    void Start() => player = Player.Get(gameObject);

    //    void Update()
    //    {
    //        if (PlayerExtensions.IsJumping(player) && jumpsRemaining > 0)
    //        {
    //            player.ReferenceHub.playerStats.GetModule<PlayerMovementController>().ForceJump(1.5f);
    //            jumpsRemaining--;
    //        }
    //    }
    //}

//    public class TimeShiftController : MonoBehaviour
//    {
//        private Player player;
//        private float timeShift;

//        public void Initialize(float shift) => timeShift = shift;

//        void Start()
//        {
//            player = Player.Get(gameObject);
//            player.SendBroadcast($"Przesunięcie czasowe: {timeShift}s!", 5);
//            player.Position += new Vector3(0, timeShift / 10f, 0);
//        }
//    }
//}

// Dodajemy brakujące klasy i metody rozszerzające
public class VoiceModulator : MonoBehaviour
{
    public void SetRandomVoice()
    {
        // Implementacja modyfikacji głosu
    }
}

public class DecoyController : MonoBehaviour
{
    private Player owner;
    private float duration;

    public void Initialize(Player player, float time)
    {
        owner = player;
        duration = time;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }
}

public class ForceFieldController : MonoBehaviour
{
    private float duration;

    public void Initialize(float time)
    {
        duration = time;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }
}

public class CustomItemHandler
{
    // Implementacja obsługi niestandardowych przedmiotów
}

public class ItemSpawnManager
{
    public static void SpawnItem(ItemType itemType, Vector3 position, Quaternion rotation)
    {
        // Implementacja spawnu przedmiotu
    }
}

public class PlayerMovementController
{
    public void ForceJump(float jumpForce)
    {
        // Implementacja wymuszenia skoku
    }
}

// Metody rozszerzające
//public static class PlayerExtensions
//{
//    public static void ApplyEffect(this Player player, EffectType effectType, int intensity, float duration)
//    {
//        player.SendBroadcast($"Zastosowano efekt {effectType} o intensywności {intensity} na {duration} sekund", 5);
//    }

    public static bool IsJumping(this Player player)
    {
        // Implementacja sprawdzenia czy gracz skacze
        return false;
    }

    public static void RemoveItem(this Player player, ItemBase item)
    {
        // Implementacja usuwania przedmiotu
    }

    public static void AddItem(this Player player, ItemBase item)
    {
        // Implementacja dodawania przedmiotu
    }
    
    public static void SetWeaponEnabled(this Player player, bool enabled)
    {
        // Implementacja włączania/wyłączania broni
    }
}

public static class DoorExtensions
{
    public static bool IsOpen(this DoorVariant door)
    {
        // Implementacja sprawdzenia czy drzwi są otwarte
        return false;
    }

    public static void SetOpen(this DoorVariant door, bool isOpen)
    {
        // Implementacja otwierania/zamykania drzwi
    }
}

public static class Extensions
{
    public static T GetRandomValue<T>(this IList<T> list)
    {
        return list[new System.Random().Next(list.Count)];
    }

    public static void ShuffleList<T>(this IList<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
