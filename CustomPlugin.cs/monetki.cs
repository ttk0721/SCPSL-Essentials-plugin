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

namespace CustomPlugin
{
    public class Monetki
    {
        private readonly CustomPlugin plugin;
        private readonly System.Random rng = new System.Random(); // Lokalna instancja rng
        private readonly PluginConfig config; // Referencja do konfiguracji
        private readonly HashSet<ushort> usedCoins = new HashSet<ushort>(); // Zbiór przechowujący seriale użytych monetek

        public Monetki(CustomPlugin plugin, PluginConfig config)
        {
            this.plugin = plugin;
            this.config = config;
        }

        // Resetowanie użytych monetek na początku rundy
        [PluginEvent]
        private void OnRoundStart(RoundStartEvent ev)
        {
            usedCoins.Clear();
            Log.Info("[Monetki] Zresetowano listę użytych monetek na początku rundy.");
        }

        // Zdarzenie rzutu monetą z efektami
        [PluginEvent]
        private void OnPlayerCoinFlip(PlayerCoinFlipEvent ev)
        {
            // Sprawdzamy, czy mechanika monetek jest włączona
            if (!config.CoinsEnabled)
            {
                ev.Player.SendBroadcast("Mechanika monetek jest wyłączona!", 5);
                return;
            }

            Player player = ev.Player;

            // Sprawdzamy, czy gracz trzyma monetkę
            if (player.CurrentItem == null || player.CurrentItem.ItemTypeId != ItemType.Coin)
            {
                player.SendBroadcast("Nie trzymasz monetki!", 5);
                return;
            }

            // Sprawdzamy, czy monetka została już użyta w tej rundzie
            ushort coinSerial = player.CurrentItem.ItemSerial;
            if (usedCoins.Contains(coinSerial))
            {
                player.SendBroadcast("Ta monetka została już użyta w tej rundzie!", 5);
                return;
            }

            // Tworzymy listę dostępnych efektów na podstawie konfiguracji
            var availableEffects = new List<(int Index, Func<Player, string> Effect, string Name)>();
            if (config.CoinEffectTeleportPlayer) availableEffects.Add((0, TeleportPlayer, "Teleportacja"));
            if (config.CoinEffectHealPlayer) availableEffects.Add((1, HealPlayer, "Leczenie"));
            if (config.CoinEffectChangePlayerClass) availableEffects.Add((2, ChangePlayerClass, "Zmiana klasy"));
            if (config.CoinEffectGiveRandomItem) availableEffects.Add((5, GiveRandomItem, "Losowy przedmiot"));
            if (config.CoinEffectGrantDamageImmunity) availableEffects.Add((6, GrantDamageImmunity, "Odporność na obrażenia"));
            if (config.CoinEffectDropCurrentItem) availableEffects.Add((7, DropCurrentItem, "Upuszczenie przedmiotu"));
            if (config.CoinEffectSwapWithRandomPlayer) availableEffects.Add((8, SwapWithRandomPlayer, "Zamiana miejscami"));
            if (config.CoinEffectTeleportToRandomRoom) availableEffects.Add((9, TeleportToRandomRoom, "Teleportacja do pomieszczenia"));
            if (config.CoinEffectBoostDamageOutput) availableEffects.Add((10, BoostDamageOutput, "Zwiększenie obrażeń"));
            if (config.CoinEffectPullNearbyPlayers) availableEffects.Add((11, PullNearbyPlayers, "Przyciągnięcie graczy"));
            if (config.CoinEffectDisguisePlayer) availableEffects.Add((12, DisguisePlayer, "Przebranie"));
            if (config.CoinEffectSwapInventoryWithRandom) availableEffects.Add((16, SwapInventoryWithRandom, "Wymiana ekwipunku"));
            if (config.CoinEffectCreateDecoyClone) availableEffects.Add((18, CreateDecoyClone, "Stworzenie klona"));
            if (config.CoinEffectShuffleAllPlayers) availableEffects.Add((19, ShuffleAllPlayers, "Przetasowanie pozycji"));
            if (config.CoinEffectToggleWeapons) availableEffects.Add((20, ToggleWeapons, "Przełącz bronie"));
            if (config.CoinEffectCreateForceField) availableEffects.Add((22, CreateForceField, "Pole siłowe"));
            if (config.CoinEffectTimeShiftPlayers) availableEffects.Add((24, TimeShiftPlayers, "Przesunięcie czasowe"));

            // Sprawdzamy, czy są dostępne jakieś efekty
            if (availableEffects.Count == 0)
            {
                player.SendBroadcast("Brak włączonych efektów monetek!", 5);
                return;
            }

            // Oznaczamy monetkę jako użyta
            usedCoins.Add(coinSerial);

            // Uruchamiamy efekt z opóźnieniem
            ApplyEffectWithDelay(player, availableEffects);
        }

        // Metoda do opóźnionego wykonania efektu
        private async void ApplyEffectWithDelay(Player player, List<(int Index, Func<Player, string> Effect, string Name)> availableEffects)
        {
            // Opóźnienie 1 sekundy, aby zsynchronizować z animacją podrzucenia monetki
            await Task.Delay(1000); // 1000 ms = 1 sekunda

            // Losujemy efekt spośród dostępnych
            var selectedEffect = availableEffects[rng.Next(availableEffects.Count)];
            string effectName = selectedEffect.Effect(player);

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
            player.SendBroadcast($"Efekt: Broń {(state ? "zablokowana" : "odblokowana")}!", 5);
            return "Przełącz bronie";
        }

        private string CreateForceField(Player player)
        {
            var shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shield.transform.position = player.Position;
            shield.transform.localScale = new Vector3(3f, 3f, 3f);
            shield.AddComponent<ForceFieldController>().Initialize(15f);
            player.SendBroadcast("Stworzono pole siłowe na 15 sekund!", 5);
            return "Pole siłowe";
        }

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
}