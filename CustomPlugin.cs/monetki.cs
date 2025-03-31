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
using Mirror;
using System.Collections;

public partial class CustomPlugin
{
    // Liczba efektów monetki
    private readonly int coinOutcomes = 25; // 25 różnych efektów
    private Dictionary<Player, Coroutine> activeCoroutines = new Dictionary<Player, Coroutine>();

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
}