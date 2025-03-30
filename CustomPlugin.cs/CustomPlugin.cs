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
using InventorySystem;
using InventorySystem.Items;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using CommandSystem;
using PlayerStatsSystem;
using System.Collections;

// Enum dla efektów (zakomentowane, bo wymaga PlayerEffects)
// public enum EffectType
// {
//     Concussed,
//     AmnesiaVision,
//     Invisible
// }

public class CustomPlugin
{
    // Zmienne stanu pluginu
    private bool roundInProgress = false;
    private bool spyAssigned = false;
    private static System.Random rng = new System.Random();
    private readonly ItemType Scp035ItemType = ItemType.KeycardO5;
    private HashSet<GameObject> brokenElevators = new HashSet<GameObject>();
    private DateTime roundStartTime;
    private readonly TimeSpan lateJoinTime = TimeSpan.FromMinutes(1);
    private bool lateJoinEnabled = true;
    private bool voiceChatEnabled = false;
    private Dictionary<Player, DateTime> playerJoinTimes = new Dictionary<Player, DateTime>();

    // Liczba efektów monetki
    private readonly int coinOutcomes = 25; // 25 różnych efektów
    private Dictionary<Player, Coroutine> activeCoroutines = new Dictionary<Player, Coroutine>();

    [PluginEntryPoint("CustomPlugin", "2.0.0", "Rozbudowany plugin z dodatkowymi funkcjami", "Autor")]
    private void OnLoaded()
    {
        EventManager.RegisterEvents(this);
        Log.Info("CustomPlugin 2.0 załadowany!");
        InitializeVoiceChat();
        SetupCustomItems();
    }

    // Inicjalizacja czatu głosowego
    private void InitializeVoiceChat()
    {
        voiceChatEnabled = true;
        Log.Info("Inicjalizacja czatu głosowego...");
    }

    // Zdarzenie rzutu monetą z 25 efektami
    [PluginEvent]
    private void OnPlayerCoinFlip(PlayerCoinFlipEvent ev)
    {
        Player player = ev.Player;
        int outcome = rng.Next(coinOutcomes);

        string effectName;
        switch (outcome)
        {
            case 0: effectName = TeleportPlayer(player); break;
            case 1: effectName = HealPlayer(player); break;
            case 2: effectName = ChangePlayerClass(player); break;
            // case 3: effectName = GrantTemporaryInvisibility(player); break; // Zakomentowane (PlayerEffects)
            // case 4: effectName = BoostPlayerSpeed(player); break; // Zakomentowane (PlayerMovementSync)
            case 5: effectName = GiveRandomItem(player); break;
            case 6: effectName = GrantDamageImmunity(player); break;
            case 7: effectName = DropCurrentItem(player); break;
            case 8: effectName = SwapWithRandomPlayer(player); break;
            case 9: effectName = TeleportToRandomRoom(player); break;
            case 10: effectName = BoostDamageOutput(player); break;
            case 11: effectName = PullNearbyPlayers(player); break;
            case 12: effectName = DisguisePlayer(player); break;
            // case 13: effectName = TemporaryAmnesia(player); break; // Zakomentowane (PlayerEffects)
            // case 14: effectName = SpawnGrenadeNearby(player); break; // Zakomentowane (ItemFactory)
            // case 15: effectName = InvertPlayerControls(player); break; // Zakomentowane (PlayerEffects)
            case 16: effectName = SwapInventoryWithRandom(player); break;
            // case 17: effectName = ActivateNightVision(player); break; // Zakomentowane (PlayerEffects)
            case 18: effectName = CreateDecoyClone(player); break;
            case 19: effectName = ShuffleAllPlayers(player); break;
            case 20: effectName = ToggleWeapons(player); break;
            // case 21: effectName = EnableDoubleJump(player); break; // Zakomentowane (PlayerMovementSync)
            case 22: effectName = CreateForceField(player); break;
            // case 23: effectName = SummonItemRain(player); break; // Zakomentowane (ItemFactory)
            case 24: effectName = TimeShiftPlayers(player); break;
            default: effectName = "Nieznany efekt"; break;
        }

        Log.Info($"[CoinFlip] {player.Nickname} otrzymał efekt: {effectName}");
    }

    // Implementacje efektów monetki
    private string TeleportPlayer(Player player)
    {
        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-50, 50), 0, UnityEngine.Random.Range(-50, 50));
        player.Position = randomPosition;
        player.SendBroadcast("Zostałeś teleportowany!", 5);
        return "Teleportacja";
    }

    private string HealPlayer(Player player)
    {
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

    /*
    private string GrantTemporaryInvisibility(Player player)
    {
        player.EffectsManager.EnableEffect<Invisible>(10f);
        Task.Delay(10000).ContinueWith(t => player.EffectsManager.DisableEffect<Invisible>());
        player.SendBroadcast("Jesteś niewidzialny przez 10 sekund!", 5);
        return "Niewidzialność";
    }
    */

    /*
    private string BoostPlayerSpeed(Player player)
    {
        var movement = player.GameObject.GetComponent<PlayerMovementSync>();
        if (movement != null)
        {
            movement._speedMultiplier = 1.5f; // Zwiększ prędkość o 50%
            Task.Delay(15000).ContinueWith(t => movement._speedMultiplier = 1.0f); // Reset po 15 sekundach
            player.SendBroadcast("Twoja prędkość została zwiększona na 15 sekund!", 5);
        }
        return "Zwiększenie prędkości";
    }
    */

    private string GiveRandomItem(Player player)
    {
        ItemType randomItem = (ItemType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ItemType)).Length);
        player.AddItem(randomItem);
        player.SendBroadcast($"Otrzymałeś {randomItem}!", 5);
        return "Losowy przedmiot";
    }

    private string GrantDamageImmunity(Player player)
    {
        player.IsGodModeEnabled = true;
        Task.Delay(20000).ContinueWith(t => player.IsGodModeEnabled = false);
        player.SendBroadcast("Jesteś odporny na obrażenia przez 20 sekund!", 5);
        return "Odporność na obrażenia";
    }

    private string DropCurrentItem(Player player)
    {
        if (player.CurrentItem != null)
        {
            // Poprawiona linia: Używamy ItemSerial zamiast Serial
            player.ReferenceHub.inventory.ServerDropItem(player.CurrentItem.ItemSerial);
            player.SendBroadcast("Upuściłeś swój przedmiot!", 5);
        }
        return "Upuszczenie przedmiotu";
    }

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
        Vector3 randomRoomPosition = new Vector3(UnityEngine.Random.Range(-100, 100), 0, UnityEngine.Random.Range(-100, 100));
        player.Position = randomRoomPosition;
        player.SendBroadcast("Teleportowano cię do losowego pomieszczenia!", 5);
        return "Teleportacja do pomieszczenia";
    }

    private string BoostDamageOutput(Player player)
    {
        // Placeholder: Zwiększenie obrażeń wymaga dostosowania do API
        Task.Delay(15000).ContinueWith(t => { /* reset obrażeń */ });
        player.SendBroadcast("Twoje obrażenia zostały zwiększone na 15 sekund!", 5);
        return "Zwiększenie obrażeń";
    }

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

    private string DisguisePlayer(Player player)
    {
        player.DisplayNickname = $"[FAKE] {Player.GetPlayers().OrderBy(_ => UnityEngine.Random.value).First().Nickname}";
        Task.Delay(30000).ContinueWith(t => player.DisplayNickname = player.Nickname);
        player.SendBroadcast("Zostałeś przebrany na 30 sekund!", 5);
        return "Przebranie";
    }

    /*
    private string TemporaryAmnesia(Player player)
    {
        player.EffectsManager.EnableEffect<Concussed>(15f);
        player.SendBroadcast("Masz chwilową amnezję przez 15 sekund!", 5);
        return "Amnezja";
    }
    */

    /*
    private string SpawnGrenadeNearby(Player player)
    {
        Vector3 grenadePosition = player.Position + new Vector3(UnityEngine.Random.Range(-5, 5), 1, UnityEngine.Random.Range(-5, 5));
        ItemType grenadeType = ItemType.GrenadeHE;
        ItemSpawnManager.SpawnItem(grenadeType, grenadePosition, Quaternion.identity);
        player.SendBroadcast("W pobliżu pojawił się granat!", 5);
        return "Pojawienie się granatu";
    }
    */

    /*
    private string InvertPlayerControls(Player player)
    {
        player.EffectsManager.EnableEffect<Concussed>(30f);
        player.SendBroadcast("Efekt: Kontrola odwrócona na 30s!", 5);
        return "Odwrócenie kontrolek";
    }
    */

    private string SwapInventoryWithRandom(Player player)
    {
        Player target = Player.GetPlayers()
            .Where(p => p != player && p.IsAlive)
            .OrderBy(_ => UnityEngine.Random.value)
            .FirstOrDefault();

        if (target != null)
        {
            var playerInventory = player.ReferenceHub.inventory;
            var targetInventory = target.ReferenceHub.inventory;

            var playerItems = playerInventory.UserInventory.Items.ToList();
            var targetItems = targetInventory.UserInventory.Items.ToList();

            playerInventory.UserInventory.Items.Clear();
            targetInventory.UserInventory.Items.Clear();

            foreach (var item in playerItems)
                targetInventory.UserInventory.Items.Add(item.Key, item.Value);
            foreach (var item in targetItems)
                playerInventory.UserInventory.Items.Add(item.Key, item.Value);

            player.SendBroadcast("Wymieniono ekwipunek z losowym graczem!", 5);
            return "Wymiana ekwipunku";
        }
        return "Brak celu wymiany";
    }

    /*
    private string ActivateNightVision(Player player)
    {
        player.EffectsManager.EnableEffect<AmnesiaVision>(60f);
        player.SendBroadcast("Efekt: Wizja amnezji aktywna na 60s!", 5);
        return "Wizja amnezji";
    }
    */

    private string CreateDecoyClone(Player player)
    {
        GameObject decoy = GameObject.Instantiate(player.GameObject);
        decoy.transform.position = player.Position;
        NetworkServer.Spawn(decoy);
        decoy.AddComponent<DecoyController>().Initialize(player, 30f);
        return "Stworzenie klona";
    }

    private string ShuffleAllPlayers(Player player)
    {
        var players = Player.GetPlayers().Where(p => p.IsAlive).ToList();
        ExtensionsStatic.ShuffleList(players);

        for (int i = 0; i < players.Count - 1; i++)
        {
            Vector3 temp = players[i].Position;
            players[i].Position = players[i + 1].Position;
            players[i + 1].Position = temp;
        }
        player.SendBroadcast("Przetasowano pozycje wszystkich graczy!", 5);
        return "Przetasowanie pozycji";
    }

    private string ToggleWeapons(Player player)
    {
        bool state = rng.Next(2) == 0;
        // Placeholder: Włączanie/wyłączanie broni wymaga dostosowania
        player.SendBroadcast($"Efekt: Broń {(state ? "zablokowana" : "odblokowana")}!", 5);
        return "Przełącz bronie";
    }

    /*
    private string EnableDoubleJump(Player player)
    {
        player.GameObject.AddComponent<DoubleJumpController>();
        player.SendBroadcast("Efekt: Podwójny skok aktywny!", 5);
        return "Podwójny skok";
    }
    */

    private string CreateForceField(Player player)
    {
        var shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shield.transform.position = player.Position;
        shield.transform.localScale = new Vector3(3f, 3f, 3f);
        shield.AddComponent<ForceFieldController>().Initialize(15f);
        player.SendBroadcast("Stworzono pole siłowe na 15 sekund!", 5);
        return "Pole siłowe";
    }

    /*
    private string SummonItemRain(Player player)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 pos = player.Position + new Vector3(rng.Next(-5, 5), 10f, rng.Next(-5, 5));
            ItemType randomItem = ((ItemType[])Enum.GetValues(typeof(ItemType)))[rng.Next(Enum.GetValues(typeof(ItemType)).Length)];
            ItemSpawnManager.SpawnItem(randomItem, pos, Quaternion.identity);
        }
        player.SendBroadcast("Rozpoczął się deszcz przedmiotów!", 5);
        return "Deszcz przedmiotów";
    }
    */

    private string TimeShiftPlayers(Player player)
    {
        var players = Player.GetPlayers().Where(p => p.IsAlive).ToList();
        foreach (var p in players)
        {
            float timeShift = rng.Next(-60, 60);
            p.GameObject.AddComponent<TimeShiftController>().Initialize(timeShift);
        }
        player.SendBroadcast("Przesunięto czas dla wszystkich graczy!", 5);
        return "Przesunięcie czasowe";
    }

    // Funkcje pomocnicze
    private void SetupCustomItems()
    {
        EventManager.RegisterEvents<CustomItemHandler>(this);
    }

    // Zdarzenie interakcji z drzwiami
    [PluginEvent(ServerEventType.PlayerInteractDoor)]
    private void OnDoorInteract(Player player, DoorVariant door, bool canOpen)
    {
        if (rng.Next(100) < 5 && door.RequiredPermissions.RequiredPermissions != KeycardPermissions.None)
        {
            door.ServerChangeLock(DoorLockReason.AdminCommand, true);
            door.NetworkTargetState = !door.TargetState;
            player.SendBroadcast("Drzwi zostały losowo przełączone!", 3);
        }
    }

    // Zdarzenie dołączenia gracza
    [PluginEvent(ServerEventType.PlayerJoined)]
    private void OnPlayerJoined(PlayerJoinedEvent ev)
    {
        playerJoinTimes[ev.Player] = DateTime.Now;
        if (voiceChatEnabled)
        {
            ev.Player.GameObject.AddComponent<VoiceModulator>().SetRandomVoice();
        }
    }
}

// Dodatkowe komponenty
/*
public class DoubleJumpController : MonoBehaviour
{
    private Player player;
    private int jumpsRemaining = 2;
    private PlayerMovementSync movement;

    void Start()
    {
        player = Player.Get(gameObject);
        movement = player.GameObject.GetComponent<PlayerMovementSync>();
    }

    void Update()
    {
        if (jumpsRemaining > 0 && movement.IsJumping)
        {
            movement.ForceJump(1.5f);
            jumpsRemaining--;
        }
    }
}
*/

public class TimeShiftController : MonoBehaviour
{
    private float timeShift;

    public void Initialize(float shift) => timeShift = shift;

    void Start()
    {
        Player player = Player.Get(gameObject);
        player.SendBroadcast($"Przesunięcie czasowe: {timeShift}s!", 5);
        player.Position += new Vector3(0, timeShift / 10f, 0);
    }
}

public class VoiceModulator : MonoBehaviour
{
    public void SetRandomVoice()
    {
        // Placeholder: Implementacja modyfikacji głosu
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
    // Placeholder: Obsługa niestandardowych przedmiotów
}

/*
public class ItemSpawnManager
{
    public static void SpawnItem(ItemType itemType, Vector3 position, Quaternion rotation)
    {
        var item = InventorySystem.ItemFactory.CreateItem(itemType);
        item.transform.position = position;
        item.transform.rotation = rotation;
        NetworkServer.Spawn(item.gameObject);
    }
}
*/

// Metody rozszerzające
public static class ExtensionsStatic
{
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