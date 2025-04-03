/*using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PlayerRoles;
using System.Collections.Generic;
using PlayerStatsSystem;
using System;

namespace CustomPlugin
{
    public class TeslaPersonelPlacowki
    {
        private readonly PluginConfig config;
        private readonly List<RoleTypeId> ignoredRoles;

        public TeslaPersonelPlacowki(CustomPlugin plugin, PluginConfig config)
        {
            this.config = config;

            // Konwertujemy listę stringów z config.yml na listę RoleTypeId
            ignoredRoles = new List<RoleTypeId>();
            foreach (var roleName in config.TeslaGateIgnoredRoles)
            {
                if (Enum.TryParse<RoleTypeId>(roleName, true, out var roleType))
                {
                    ignoredRoles.Add(roleType);
                }
                else
                {
                    Log.Warning($"[TeslaPersonelPlacowki] Nieprawidłowa rola w TeslaGateIgnoredRoles: {roleName}. Pomijam.");
                }
            }
        }

        // Zdarzenie wywoływane, gdy gracz otrzymuje obrażenia (po obliczeniu)
        [PluginEvent(ServerEventType.PlayerHurt)]
        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            // Sprawdzamy, czy gracz i źródło obrażeń istnieją
            Player player = ev.Player;
            if (player == null || ev.DamageHandler == null)
                return;

            // Sprawdzamy, czy obrażenia pochodzą od Tesli
            // W wersji 13.1.5.0 używamy StandardDamageHandler i Type
            if (ev.DamageHandler is StandardDamageHandler handler && handler.Type == DamageType.Tesla)
            {
                // Sprawdzamy, czy rola gracza jest na liście ignorowanych
                if (ignoredRoles.Contains(player.Role))
                {
                    // Anulujemy obrażenia, ustawiając Damage na 0
                    ev.Damage = 0f;
                    Log.Info($"[TeslaPersonelPlacowki] Obrażenia od Tesli anulowane dla {player.Nickname} (rola: {player.Role})");
                }
                else
                {
                    // Pozwalamy na obrażenia dla innych ról
                    Log.Info($"[TeslaPersonelPlacowki] Obrażenia od Tesli zadane {player.Nickname} (rola: {player.Role})");
                }
            }
        }
    }
}*/