using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PluginAPI.Core;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace CustomPlugin
{
    public static class IsConfigValid
    {
        public static PluginConfig LoadAndValidateConfig(string configPath)
        {
            // Serializacja/deserializacja YAML
            var serializer = new SerializerBuilder().Build();
            var deserializer = new DeserializerBuilder().Build();

            PluginConfig config;
            bool configNeedsFix = false;

            try
            {
                if (!File.Exists(configPath))
                {
                    // Jeśli plik nie istnieje, tworzymy go z domyślnym szablonem
                    File.WriteAllText(configPath, PluginConfig.DefaultConfigTemplate);
                    Log.Info($"[CustomPlugin] Utworzono domyślny plik konfiguracyjny z komentarzami: {configPath}\n");
                    config = PluginConfig.CreateDefault();
                }
                else
                {
                    // Wczytanie pliku i deserializacja
                    string configText = File.ReadAllText(configPath);
                    try
                    {
                        config = deserializer.Deserialize<PluginConfig>(configText) ?? PluginConfig.CreateDefault();
                        Log.Info($"[CustomPlugin] Wczytano konfigurację z pliku: {configPath}\n");
                    }
                    catch (YamlException ex)
                    {
                        // Jeśli deserializacja się nie powiodła, próbujemy ręcznie sparsować plik
                        Log.Warning($"[CustomPlugin] Błąd podczas deserializacji pliku konfiguracyjnego: {ex.Message}. Próbuję odzyskać wartości...\n");
                        config = ParseConfigManually(configText);
                        configNeedsFix = true;
                    }

                    // Walidacja i naprawa konfiguracji wraz z logowaniem ostrzeżeń
                    var (fixedConfig, warnings) = config.ValidateAndFix();
                    foreach (var warning in warnings)
                    {
                        Log.Warning(warning);
                    }
                    if (!AreConfigsEqual(config, fixedConfig))
                    {
                        config = fixedConfig;
                        configNeedsFix = true;
                        Log.Info($"[CustomPlugin] Wykryto nieprawidłowe wartości w pliku konfiguracyjnym. Poprawiono konfigurację.\n");
                    }
                }
            }
            catch (Exception ex)
            {
                // W przypadku innych błędów (np. brak dostępu do pliku), tworzymy nowy plik
                Log.Error($"[CustomPlugin] Krytyczny błąd podczas wczytywania pliku konfiguracyjnego: {ex.Message}\n");
                File.WriteAllText(configPath, PluginConfig.DefaultConfigTemplate);
                Log.Info($"[CustomPlugin] Utworzono nowy domyślny plik konfiguracyjny z komentarzami: {configPath}\n");
                config = PluginConfig.CreateDefault();
            }

            // Jeśli konfiguracja wymaga poprawek, zapisujemy poprawiony plik
            if (configNeedsFix)
            {
                string updatedConfigText = PluginConfig.DefaultConfigTemplate;
                // Aktualizujemy wartości w szablonie, zachowując komentarze
                updatedConfigText = UpdateConfigText(updatedConfigText, config);
                File.WriteAllText(configPath, updatedConfigText);
                Log.Info($"[CustomPlugin] Zaktualizowano plik konfiguracyjny z poprawionymi wartościami: {configPath}\n");
            }

            return config;
        }

        // Metoda do ręcznego parsowania pliku YAML
        private static PluginConfig ParseConfigManually(string configText)
        {
            var config = PluginConfig.CreateDefault();
            var lines = configText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            bool inTeslaGateIgnoredRolesSection = false;
            var teslaGateIgnoredRoles = new List<string>();

            foreach (var line in lines)
            {
                // Pomijamy puste linie i komentarze
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                    continue;

                // Sprawdzamy, czy jesteśmy w sekcji TeslaGateIgnoredRoles
                if (line.Trim() == "TeslaGateIgnoredRoles:")
                {
                    inTeslaGateIgnoredRolesSection = true;
                    continue;
                }

                if (inTeslaGateIgnoredRolesSection)
                {
                    if (!line.StartsWith("  -"))
                    {
                        inTeslaGateIgnoredRolesSection = false;
                    }
                    else
                    {
                        // Parsujemy rolę z linii w formacie "  - FacilityGuard"
                        string role = line.Trim().Substring(2).Trim();
                        if (!string.IsNullOrEmpty(role))
                        {
                            teslaGateIgnoredRoles.Add(role);
                        }
                        continue;
                    }
                }

                // Parsowanie pozostałych pól
                var parts = line.Split(new[] { ':' }, 2, StringSplitOptions.None);
                if (parts.Length != 2)
                    continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                try
                {
                    switch (key)
                    {
                        case "IsEnabled":
                            if (bool.TryParse(value, out bool isEnabled))
                                config.IsEnabled = isEnabled;
                            break;

                        case "RandomMessagesEnabled":
                            if (bool.TryParse(value, out bool randomMessagesEnabled))
                                config.RandomMessagesEnabled = randomMessagesEnabled;
                            break;

                        case "RandomMessagesMinIntervalSeconds":
                            if (int.TryParse(value, out int minInterval) && minInterval >= 1)
                                config.RandomMessagesMinIntervalSeconds = minInterval;
                            break;

                        case "RandomMessagesMaxIntervalSeconds":
                            if (int.TryParse(value, out int maxInterval) && maxInterval >= config.RandomMessagesMinIntervalSeconds)
                                config.RandomMessagesMaxIntervalSeconds = maxInterval;
                            break;

                        case "DoorLockdownEnabled":
                            if (bool.TryParse(value, out bool doorLockdownEnabled))
                                config.DoorLockdownEnabled = doorLockdownEnabled;
                            break;

                        case "DoorLockdownMinIntervalSeconds":
                            if (int.TryParse(value, out int doorMinInterval) && doorMinInterval >= 1)
                                config.DoorLockdownMinIntervalSeconds = doorMinInterval;
                            break;

                        case "DoorLockdownMaxIntervalSeconds":
                            if (int.TryParse(value, out int doorMaxInterval) && doorMaxInterval >= config.DoorLockdownMinIntervalSeconds)
                                config.DoorLockdownMaxIntervalSeconds = doorMaxInterval;
                            break;

                        case "DoorLockdownDurationSeconds":
                            if (int.TryParse(value, out int doorDuration) && doorDuration >= 1)
                                config.DoorLockdownDurationSeconds = doorDuration;
                            break;

                        case "LateJoinEnabled":
                            if (bool.TryParse(value, out bool lateJoinEnabled))
                                config.LateJoinEnabled = lateJoinEnabled;
                            break;

                        case "LateJoinTimeSeconds":
                            if (int.TryParse(value, out int lateJoinTime) && lateJoinTime >= 1)
                                config.LateJoinTimeSeconds = lateJoinTime;
                            break;

                        case "CoinsEnabled":
                            if (bool.TryParse(value, out bool coinsEnabled))
                                config.CoinsEnabled = coinsEnabled;
                            break;

                        case "CoinEffectTeleportPlayer":
                            if (bool.TryParse(value, out bool teleportPlayer))
                                config.CoinEffectTeleportPlayer = teleportPlayer;
                            break;

                        case "CoinEffectHealPlayer":
                            if (bool.TryParse(value, out bool healPlayer))
                                config.CoinEffectHealPlayer = healPlayer;
                            break;

                        case "CoinEffectChangePlayerClass":
                            if (bool.TryParse(value, out bool changePlayerClass))
                                config.CoinEffectChangePlayerClass = changePlayerClass;
                            break;

                        case "CoinEffectGiveRandomItem":
                            if (bool.TryParse(value, out bool giveRandomItem))
                                config.CoinEffectGiveRandomItem = giveRandomItem;
                            break;

                        case "CoinEffectGrantDamageImmunity":
                            if (bool.TryParse(value, out bool grantDamageImmunity))
                                config.CoinEffectGrantDamageImmunity = grantDamageImmunity;
                            break;

                        case "CoinEffectDropCurrentItem":
                            if (bool.TryParse(value, out bool dropCurrentItem))
                                config.CoinEffectDropCurrentItem = dropCurrentItem;
                            break;

                        case "CoinEffectSwapWithRandomPlayer":
                            if (bool.TryParse(value, out bool swapWithRandomPlayer))
                                config.CoinEffectSwapWithRandomPlayer = swapWithRandomPlayer;
                            break;

                        case "CoinEffectTeleportToRandomRoom":
                            if (bool.TryParse(value, out bool teleportToRandomRoom))
                                config.CoinEffectTeleportToRandomRoom = teleportToRandomRoom;
                            break;

                        case "CoinEffectBoostDamageOutput":
                            if (bool.TryParse(value, out bool boostDamageOutput))
                                config.CoinEffectBoostDamageOutput = boostDamageOutput;
                            break;

                        case "CoinEffectPullNearbyPlayers":
                            if (bool.TryParse(value, out bool pullNearbyPlayers))
                                config.CoinEffectPullNearbyPlayers = pullNearbyPlayers;
                            break;

                        case "CoinEffectDisguisePlayer":
                            if (bool.TryParse(value, out bool disguisePlayer))
                                config.CoinEffectDisguisePlayer = disguisePlayer;
                            break;

                        case "CoinEffectSwapInventoryWithRandom":
                            if (bool.TryParse(value, out bool swapInventoryWithRandom))
                                config.CoinEffectSwapInventoryWithRandom = swapInventoryWithRandom;
                            break;

                        case "CoinEffectCreateDecoyClone":
                            if (bool.TryParse(value, out bool createDecoyClone))
                                config.CoinEffectCreateDecoyClone = createDecoyClone;
                            break;

                        case "CoinEffectShuffleAllPlayers":
                            if (bool.TryParse(value, out bool shuffleAllPlayers))
                                config.CoinEffectShuffleAllPlayers = shuffleAllPlayers;
                            break;

                        case "CoinEffectToggleWeapons":
                            if (bool.TryParse(value, out bool toggleWeapons))
                                config.CoinEffectToggleWeapons = toggleWeapons;
                            break;

                        case "CoinEffectCreateForceField":
                            if (bool.TryParse(value, out bool createForceField))
                                config.CoinEffectCreateForceField = createForceField;
                            break;

                        case "CoinEffectTimeShiftPlayers":
                            if (bool.TryParse(value, out bool timeShiftPlayers))
                                config.CoinEffectTimeShiftPlayers = timeShiftPlayers;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"[CustomPlugin] Błąd podczas parsowania linii '{line}': {ex.Message}. Używam wartości domyślnej.\n");
                }
            }

            // Walidacja i naprawa konfiguracji wraz z logowaniem ostrzeżeń
            var (fixedConfig, warnings) = config.ValidateAndFix();
            foreach (var warning in warnings)
            {
                Log.Warning(warning);
            }
            return fixedConfig;
        }

        // Metoda do porównania dwóch obiektów PluginConfig
        private static bool AreConfigsEqual(PluginConfig config1, PluginConfig config2)
        {
            return config1.IsEnabled == config2.IsEnabled &&
                   config1.RandomMessagesEnabled == config2.RandomMessagesEnabled &&
                   config1.RandomMessagesMinIntervalSeconds == config2.RandomMessagesMinIntervalSeconds &&
                   config1.RandomMessagesMaxIntervalSeconds == config2.RandomMessagesMaxIntervalSeconds &&
                   config1.DoorLockdownEnabled == config2.DoorLockdownEnabled &&
                   config1.DoorLockdownMinIntervalSeconds == config2.DoorLockdownMinIntervalSeconds &&
                   config1.DoorLockdownMaxIntervalSeconds == config2.DoorLockdownMaxIntervalSeconds &&
                   config1.DoorLockdownDurationSeconds == config2.DoorLockdownDurationSeconds &&
                   config1.LateJoinEnabled == config2.LateJoinEnabled &&
                   config1.LateJoinTimeSeconds == config2.LateJoinTimeSeconds &&
                   config1.CoinsEnabled == config2.CoinsEnabled &&
                   config1.CoinEffectTeleportPlayer == config2.CoinEffectTeleportPlayer &&
                   config1.CoinEffectHealPlayer == config2.CoinEffectHealPlayer &&
                   config1.CoinEffectChangePlayerClass == config2.CoinEffectChangePlayerClass &&
                   config1.CoinEffectGiveRandomItem == config2.CoinEffectGiveRandomItem &&
                   config1.CoinEffectGrantDamageImmunity == config2.CoinEffectGrantDamageImmunity &&
                   config1.CoinEffectDropCurrentItem == config2.CoinEffectDropCurrentItem &&
                   config1.CoinEffectSwapWithRandomPlayer == config2.CoinEffectSwapWithRandomPlayer &&
                   config1.CoinEffectTeleportToRandomRoom == config2.CoinEffectTeleportToRandomRoom &&
                   config1.CoinEffectBoostDamageOutput == config2.CoinEffectBoostDamageOutput &&
                   config1.CoinEffectPullNearbyPlayers == config2.CoinEffectPullNearbyPlayers &&
                   config1.CoinEffectDisguisePlayer == config2.CoinEffectDisguisePlayer &&
                   config1.CoinEffectSwapInventoryWithRandom == config2.CoinEffectSwapInventoryWithRandom &&
                   config1.CoinEffectCreateDecoyClone == config2.CoinEffectCreateDecoyClone &&
                   config1.CoinEffectShuffleAllPlayers == config2.CoinEffectShuffleAllPlayers &&
                   config1.CoinEffectToggleWeapons == config2.CoinEffectToggleWeapons &&
                   config1.CoinEffectCreateForceField == config2.CoinEffectCreateForceField &&
                   config1.CoinEffectTimeShiftPlayers == config2.CoinEffectTimeShiftPlayers &&
                   config1.LobbySystemEnabled == config2.LobbySystemEnabled;
        }

        // Metoda do aktualizacji wartości w szablonie YAML, zachowując komentarze
        private static string UpdateConfigText(string template, PluginConfig config)
        {
            var lines = template.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            var updatedLines = new List<string>();
            bool inTeslaGateIgnoredRolesSection = false;

            foreach (var line in lines)
            {
                string updatedLine = line;

                if (line.StartsWith("IsEnabled:"))
                    updatedLine = $"IsEnabled: {config.IsEnabled.ToString().ToLower()}";
                else if (line.StartsWith("RandomMessagesEnabled:"))
                    updatedLine = $"RandomMessagesEnabled: {config.RandomMessagesEnabled.ToString().ToLower()}";
                else if (line.StartsWith("RandomMessagesMinIntervalSeconds:"))
                    updatedLine = $"RandomMessagesMinIntervalSeconds: {config.RandomMessagesMinIntervalSeconds}";
                else if (line.StartsWith("RandomMessagesMaxIntervalSeconds:"))
                    updatedLine = $"RandomMessagesMaxIntervalSeconds: {config.RandomMessagesMaxIntervalSeconds}";
                else if (line.StartsWith("DoorLockdownEnabled:"))
                    updatedLine = $"DoorLockdownEnabled: {config.DoorLockdownEnabled.ToString().ToLower()}";
                else if (line.StartsWith("DoorLockdownMinIntervalSeconds:"))
                    updatedLine = $"DoorLockdownMinIntervalSeconds: {config.DoorLockdownMinIntervalSeconds}";
                else if (line.StartsWith("DoorLockdownMaxIntervalSeconds:"))
                    updatedLine = $"DoorLockdownMaxIntervalSeconds: {config.DoorLockdownMaxIntervalSeconds}";
                else if (line.StartsWith("DoorLockdownDurationSeconds:"))
                    updatedLine = $"DoorLockdownDurationSeconds: {config.DoorLockdownDurationSeconds}";
                else if (line.StartsWith("LateJoinEnabled:"))
                    updatedLine = $"LateJoinEnabled: {config.LateJoinEnabled.ToString().ToLower()}";
                else if (line.StartsWith("LateJoinTimeSeconds:"))
                    updatedLine = $"LateJoinTimeSeconds: {config.LateJoinTimeSeconds}";
                else if (line.StartsWith("CoinsEnabled:"))
                    updatedLine = $"CoinsEnabled: {config.CoinsEnabled.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectTeleportPlayer:"))
                    updatedLine = $"CoinEffectTeleportPlayer: {config.CoinEffectTeleportPlayer.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectHealPlayer:"))
                    updatedLine = $"CoinEffectHealPlayer: {config.CoinEffectHealPlayer.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectChangePlayerClass:"))
                    updatedLine = $"CoinEffectChangePlayerClass: {config.CoinEffectChangePlayerClass.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectGiveRandomItem:"))
                    updatedLine = $"CoinEffectGiveRandomItem: {config.CoinEffectGiveRandomItem.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectGrantDamageImmunity:"))
                    updatedLine = $"CoinEffectGrantDamageImmunity: {config.CoinEffectGrantDamageImmunity.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectDropCurrentItem:"))
                    updatedLine = $"CoinEffectDropCurrentItem: {config.CoinEffectDropCurrentItem.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectSwapWithRandomPlayer:"))
                    updatedLine = $"CoinEffectSwapWithRandomPlayer: {config.CoinEffectSwapWithRandomPlayer.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectTeleportToRandomRoom:"))
                    updatedLine = $"CoinEffectTeleportToRandomRoom: {config.CoinEffectTeleportToRandomRoom.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectBoostDamageOutput:"))
                    updatedLine = $"CoinEffectBoostDamageOutput: {config.CoinEffectBoostDamageOutput.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectPullNearbyPlayers:"))
                    updatedLine = $"CoinEffectPullNearbyPlayers: {config.CoinEffectPullNearbyPlayers.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectDisguisePlayer:"))
                    updatedLine = $"CoinEffectDisguisePlayer: {config.CoinEffectDisguisePlayer.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectSwapInventoryWithRandom:"))
                    updatedLine = $"CoinEffectSwapInventoryWithRandom: {config.CoinEffectSwapInventoryWithRandom.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectCreateDecoyClone:"))
                    updatedLine = $"CoinEffectCreateDecoyClone: {config.CoinEffectCreateDecoyClone.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectShuffleAllPlayers:"))
                    updatedLine = $"CoinEffectShuffleAllPlayers: {config.CoinEffectShuffleAllPlayers.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectToggleWeapons:"))
                    updatedLine = $"CoinEffectToggleWeapons: {config.CoinEffectToggleWeapons.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectCreateForceField:"))
                    updatedLine = $"CoinEffectCreateForceField: {config.CoinEffectCreateForceField.ToString().ToLower()}";
                else if (line.StartsWith("CoinEffectTimeShiftPlayers:"))
                    updatedLine = $"CoinEffectTimeShiftPlayers: {config.CoinEffectTimeShiftPlayers.ToString().ToLower()}";
                else if (line.StartsWith("LobbySystemEnabled:"))
                    updatedLine = $"LobbySystemEnabled: {config.LobbySystemEnabled.ToString().ToLower()}";

                if (inTeslaGateIgnoredRolesSection && line.StartsWith("  -"))
                {
                    continue; // Pomijamy stare role, już dodaliśmy nowe
                }
                else
                {
                    inTeslaGateIgnoredRolesSection = false;
                }

                updatedLines.Add(updatedLine);
            }

            return string.Join("\n", updatedLines);
        }
    }
}
