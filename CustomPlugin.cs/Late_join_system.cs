using System;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PlayerRoles;

namespace CustomPlugin
{
    public class LateJoinSystem
    {
        private readonly CustomPlugin plugin;
        private readonly PluginConfig config;

        public LateJoinSystem(CustomPlugin plugin, PluginConfig config)
        {
            this.plugin = plugin;
            this.config = config;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        private void OnPlayerJoined(PlayerJoinedEvent ev)
        {
            if (!config.LateJoinEnabled)
                return;

            Player player = ev.Player;

            // Sprawdzamy, czy runda się rozpoczęła
            if (!Round.IsRoundStarted)
            {
                // Runda się nie rozpoczęła, nie przypisujemy roli
                return;
            }

            // Sprawdzamy, czy gracz dołączył w czasie dozwolonym na "late join"
            double secondsSinceRoundStart = (DateTime.Now - plugin.roundStartTime).TotalSeconds;
            if (secondsSinceRoundStart <= config.LateJoinTimeSeconds)
            {
                // Przypisujemy losową rolę (np. Class-D lub Scientist, w zależności od ustawień)
                RoleTypeId newRole = UnityEngine.Random.Range(0, 2) == 0 ? RoleTypeId.ClassD : RoleTypeId.Scientist;
                player.Role = newRole;
                player.SendBroadcast($"Dołączyłeś do rundy jako {newRole}!", 5);
                Log.Info($"[LateJoinSystem] Gracz {player.Nickname} dołączył jako {newRole}.\n");
            }
            else
            {
                // Gracz dołączył za późno, przypisujemy rolę Spectator
                player.Role = RoleTypeId.Spectator;
                player.SendBroadcast("Dołączyłeś za późno, aby wziąć udział w rundzie. Jesteś obserwatorem.", 5);
                Log.Info($"[LateJoinSystem] Gracz {player.Nickname} dołączył za późno i został obserwatorem.\n");
            }
        }
    }
}