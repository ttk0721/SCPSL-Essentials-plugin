using PlayerRoles;
using System;
using System.Collections.Generic;

namespace CustomPlugin
{
    public class PluginConfig
    {
        // Ogólne ustawienia wtyczki
        public bool IsEnabled { get; set; } = true;

        // Ustawienia dla LosoweKomunikaty
        public bool RandomMessagesEnabled { get; set; } = true;
        public int RandomMessagesMinIntervalSeconds { get; set; } = 60;
        public int RandomMessagesMaxIntervalSeconds { get; set; } = 480;

        // Ustawienia dla OkresoweZamykanieDrzwi
        public bool DoorLockdownEnabled { get; set; } = true;
        public int DoorLockdownMinIntervalSeconds { get; set; } = 180;
        public int DoorLockdownMaxIntervalSeconds { get; set; } = 300;
        public int DoorLockdownDurationSeconds { get; set; } = 15;

        // Ustawienia dla LateJoinSystem
        public bool LateJoinEnabled { get; set; } = true;
        public int LateJoinTimeSeconds { get; set; } = 60;

        // Ustawienia dla Monetki
        public bool CoinsEnabled { get; set; } = true;
        public bool CoinEffectTeleportPlayer { get; set; } = true;
        public bool CoinEffectHealPlayer { get; set; } = true;
        public bool CoinEffectChangePlayerClass { get; set; } = true;
        public bool CoinEffectGiveRandomItem { get; set; } = true;
        public bool CoinEffectGrantDamageImmunity { get; set; } = true;
        public bool CoinEffectDropCurrentItem { get; set; } = true;
        public bool CoinEffectSwapWithRandomPlayer { get; set; } = true;
        public bool CoinEffectTeleportToRandomRoom { get; set; } = true;
        public bool CoinEffectBoostDamageOutput { get; set; } = true;
        public bool CoinEffectPullNearbyPlayers { get; set; } = true;
        public bool CoinEffectDisguisePlayer { get; set; } = true;
        public bool CoinEffectSwapInventoryWithRandom { get; set; } = true;
        public bool CoinEffectCreateDecoyClone { get; set; } = true;
        public bool CoinEffectShuffleAllPlayers { get; set; } = true;
        public bool CoinEffectToggleWeapons { get; set; } = true;
        public bool CoinEffectCreateForceField { get; set; } = true;
        public bool CoinEffectTimeShiftPlayers { get; set; } = true;

        // Ustawienia dla LobbySystem
        public bool LobbySystemEnabled { get; set; } = true;

        // Domyślny szablon YAML z sekcjami, komentarzami i opisami
        public static string DefaultConfigTemplate => @"
# Sekcja: Ogólne ustawienia wtyczki
# ---------------------------------
# Czy wtyczka jest włączona?
# Dozwolone wartości: true, false
IsEnabled: true

# Sekcja: Komunikaty fabularne (LosoweKomunikaty)
# ---------------------------------------------
# Czy komunikaty fabularne są włączone?
# Dozwolone wartości: true, false
RandomMessagesEnabled: true
# Minimalny czas między komunikatami (w sekundach)
# Dozwolone wartości: liczba całkowita >= 1
RandomMessagesMinIntervalSeconds: 60
# Maksymalny czas między komunikatami (w sekundach)
# Dozwolone wartości: liczba całkowita >= RandomMessagesMinIntervalSeconds
RandomMessagesMaxIntervalSeconds: 480

# Sekcja: Okresowe zamykanie drzwi (OkresoweZamykanieDrzwi)
# ------------------------------------------------------
# Czy okresowe zamykanie drzwi jest włączone?
# Dozwolone wartości: true, false
DoorLockdownEnabled: true
# Minimalny czas między lockdownami (w sekundach)
# Dozwolone wartości: liczba całkowita >= 1
DoorLockdownMinIntervalSeconds: 180
# Maksymalny czas między lockdownami (w sekundach)
# Dozwolone wartości: liczba całkowita >= DoorLockdownMinIntervalSeconds
DoorLockdownMaxIntervalSeconds: 300
# Czas trwania lockdownu (w sekundach)
# Dozwolone wartości: liczba całkowita >= 1
DoorLockdownDurationSeconds: 15

# Sekcja: System Late Join (LateJoinSystem)
# ---------------------------------------
# Czy system ""late join"" jest włączony?
# Dozwolone wartości: true, false
LateJoinEnabled: true
# Czas na dołączenie do rundy (w sekundach)
# Dozwolone wartości: liczba całkowita >= 1
LateJoinTimeSeconds: 60

# Sekcja: Mechanika Monetek (Monetki)
# -----------------------------------
# Czy mechanika monetek jest włączona?
# Dozwolone wartości: true, false
CoinsEnabled: true

# Ustawienia poszczególnych efektów monetek
# Dozwolone wartości dla wszystkich efektów: true, false
CoinEffectTeleportPlayer: true        # Teleportuje gracza w losowe miejsce
CoinEffectHealPlayer: true            # Leczy gracza do pełnego zdrowia
CoinEffectChangePlayerClass: true     # Zmienia klasę gracza na losową
CoinEffectGiveRandomItem: true        # Daje graczowi losowy przedmiot
CoinEffectGrantDamageImmunity: true   # Daje graczowi odporność na obrażenia na 20 sekund
CoinEffectDropCurrentItem: true       # Upuszcza aktualny przedmiot gracza
CoinEffectSwapWithRandomPlayer: true  # Zamienia miejscami z losowym graczem
CoinEffectTeleportToRandomRoom: true  # Teleportuje gracza do losowego pomieszczenia
CoinEffectBoostDamageOutput: true     # Zwiększa obrażenia gracza na 15 sekund
CoinEffectPullNearbyPlayers: true     # Przyciąga pobliskich graczy
CoinEffectDisguisePlayer: true        # Przebranie gracza na 30 sekund
CoinEffectSwapInventoryWithRandom: true  # Wymienia ekwipunek z losowym graczem
CoinEffectCreateDecoyClone: true      # Tworzy klona gracza
CoinEffectShuffleAllPlayers: true     # Przetasowuje pozycje wszystkich graczy
CoinEffectToggleWeapons: true         # Włącza/wyłącza broń gracza
CoinEffectCreateForceField: true      # Tworzy pole siłowe wokół gracza
CoinEffectTimeShiftPlayers: true      # Przesuwa czas dla wszystkich graczy

# Sekcja: Lobby
# -----------------------------------
# Czy Lobby przed grą ma być włączone?
# Dozwolone wartości: true, false
LobbySystemEnabled: true
";

        // Metoda do tworzenia domyślnego obiektu konfiguracji
        public static PluginConfig CreateDefault()
        {
            return new PluginConfig
            {
                IsEnabled = true,
                RandomMessagesEnabled = true,
                RandomMessagesMinIntervalSeconds = 60,
                RandomMessagesMaxIntervalSeconds = 480,
                DoorLockdownEnabled = true,
                DoorLockdownMinIntervalSeconds = 180,
                DoorLockdownMaxIntervalSeconds = 300,
                DoorLockdownDurationSeconds = 15,
                LateJoinEnabled = true,
                LateJoinTimeSeconds = 60,
                CoinsEnabled = true,
                CoinEffectTeleportPlayer = true,
                CoinEffectHealPlayer = true,
                CoinEffectChangePlayerClass = true,
                CoinEffectGiveRandomItem = true,
                CoinEffectGrantDamageImmunity = true,
                CoinEffectDropCurrentItem = true,
                CoinEffectSwapWithRandomPlayer = true,
                CoinEffectTeleportToRandomRoom = true,
                CoinEffectBoostDamageOutput = true,
                CoinEffectPullNearbyPlayers = true,
                CoinEffectDisguisePlayer = true,
                CoinEffectSwapInventoryWithRandom = true,
                CoinEffectCreateDecoyClone = true,
                CoinEffectShuffleAllPlayers = true,
                CoinEffectToggleWeapons = true,
                CoinEffectCreateForceField = true,
                CoinEffectTimeShiftPlayers = true,
                LobbySystemEnabled = true
            };
        }

        // Metoda do walidacji i naprawy konfiguracji
        public (PluginConfig fixedConfig, List<string> warnings) ValidateAndFix()
        {
            var defaultConfig = CreateDefault();
            var fixedConfig = new PluginConfig();
            var warnings = new List<string>();

            // Walidacja IsEnabled
            fixedConfig.IsEnabled = IsEnabled; // Wartość bool, zawsze poprawna

            // Walidacja RandomMessages
            fixedConfig.RandomMessagesEnabled = RandomMessagesEnabled; // Wartość bool, zawsze poprawna
            fixedConfig.RandomMessagesMinIntervalSeconds = RandomMessagesMinIntervalSeconds >= 1
                ? RandomMessagesMinIntervalSeconds
                : defaultConfig.RandomMessagesMinIntervalSeconds;
            fixedConfig.RandomMessagesMaxIntervalSeconds = RandomMessagesMaxIntervalSeconds >= fixedConfig.RandomMessagesMinIntervalSeconds
                ? RandomMessagesMaxIntervalSeconds
                : defaultConfig.RandomMessagesMaxIntervalSeconds;

            // Walidacja DoorLockdown
            fixedConfig.DoorLockdownEnabled = DoorLockdownEnabled; // Wartość bool, zawsze poprawna
            fixedConfig.DoorLockdownMinIntervalSeconds = DoorLockdownMinIntervalSeconds >= 1
                ? DoorLockdownMinIntervalSeconds
                : defaultConfig.DoorLockdownMinIntervalSeconds;
            fixedConfig.DoorLockdownMaxIntervalSeconds = DoorLockdownMaxIntervalSeconds >= fixedConfig.DoorLockdownMinIntervalSeconds
                ? DoorLockdownMaxIntervalSeconds
                : defaultConfig.DoorLockdownMaxIntervalSeconds;
            fixedConfig.DoorLockdownDurationSeconds = DoorLockdownDurationSeconds >= 1
                ? DoorLockdownDurationSeconds
                : defaultConfig.DoorLockdownDurationSeconds;

            // Walidacja LateJoin
            fixedConfig.LateJoinEnabled = LateJoinEnabled; // Wartość bool, zawsze poprawna
            fixedConfig.LateJoinTimeSeconds = LateJoinTimeSeconds >= 1
                ? LateJoinTimeSeconds
                : defaultConfig.LateJoinTimeSeconds;

            // Walidacja Coins
            fixedConfig.CoinsEnabled = CoinsEnabled; // Wartość bool, zawsze poprawna
            fixedConfig.CoinEffectTeleportPlayer = CoinEffectTeleportPlayer;
            fixedConfig.CoinEffectHealPlayer = CoinEffectHealPlayer;
            fixedConfig.CoinEffectChangePlayerClass = CoinEffectChangePlayerClass;
            fixedConfig.CoinEffectGiveRandomItem = CoinEffectGiveRandomItem;
            fixedConfig.CoinEffectGrantDamageImmunity = CoinEffectGrantDamageImmunity;
            fixedConfig.CoinEffectDropCurrentItem = CoinEffectDropCurrentItem;
            fixedConfig.CoinEffectSwapWithRandomPlayer = CoinEffectSwapWithRandomPlayer;
            fixedConfig.CoinEffectTeleportToRandomRoom = CoinEffectTeleportToRandomRoom;
            fixedConfig.CoinEffectBoostDamageOutput = CoinEffectBoostDamageOutput;
            fixedConfig.CoinEffectPullNearbyPlayers = CoinEffectPullNearbyPlayers;
            fixedConfig.CoinEffectDisguisePlayer = CoinEffectDisguisePlayer;
            fixedConfig.CoinEffectSwapInventoryWithRandom = CoinEffectSwapInventoryWithRandom;
            fixedConfig.CoinEffectCreateDecoyClone = CoinEffectCreateDecoyClone;
            fixedConfig.CoinEffectShuffleAllPlayers = CoinEffectShuffleAllPlayers;
            fixedConfig.CoinEffectToggleWeapons = CoinEffectToggleWeapons;
            fixedConfig.CoinEffectCreateForceField = CoinEffectCreateForceField;
            fixedConfig.CoinEffectTimeShiftPlayers = CoinEffectTimeShiftPlayers;

            // Walidacja LobbySystem
            fixedConfig.LobbySystemEnabled = LobbySystemEnabled; // Wartość bool, zawsze poprawna

            return (fixedConfig, warnings);
        }
    }
}