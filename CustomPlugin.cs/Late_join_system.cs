using System;
using System.Collections.Generic;
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
        private readonly TimeSpan lateJoinTime = TimeSpan.FromMinutes(1); // 1 minuta na dołączenie
        private bool lateJoinEnabled = true; // Czy system "late join" jest włączony
        private Dictionary<Player, DateTime> playerJoinTimes = new Dictionary<Player, DateTime>(); // Czas dołączenia graczy
        private DateTime roundStartTime; // Czas rozpoczęcia rundy

        public LateJoinSystem(CustomPlugin plugin)
        {
            this.plugin = plugin;
        }

        // Zdarzenie rozpoczęcia rundy
        [PluginEvent(ServerEventType.RoundStart)]
        private void OnRoundStart(RoundStartEvent ev)
        {
            if (!lateJoinEnabled)
                return;

            roundStartTime = DateTime.Now; // Zapisujemy czas rozpoczęcia rundy
            Log.Info("System Late Join włączony. Gracze mają 1 minutę na dołączenie.");
        }

        // Zdarzenie dołączenia gracza
        [PluginEvent(ServerEventType.PlayerJoined)]
        private void OnPlayerJoined(PlayerJoinedEvent ev)
        {
            if (!lateJoinEnabled)
                return;

            Player player = ev.Player;
            playerJoinTimes[player] = DateTime.Now; // Zapisujemy czas dołączenia gracza

            // Obliczamy, ile czasu minęło od rozpoczęcia rundy
            TimeSpan timeSinceRoundStart = playerJoinTimes[player] - roundStartTime;

            // Sprawdzamy, czy gracz dołączył w ciągu pierwszej minuty
            if (timeSinceRoundStart <= lateJoinTime)
            {
                // Gracz dołączył na czas
                player.SendBroadcast("Late join sytem aktywny! Witaj w rundzie.", 5);
                Log.Info($"{player.Nickname} dołączył na czas ({timeSinceRoundStart.TotalSeconds} sekund od startu rundy).");

                // Opcjonalnie: Przypisz rolę dla graczy, którzy dołączyli na czas
                // Na przykład: Class-D
                player.Role = RoleTypeId.ClassD;
            }
            else
            {
                // Gracz spóźnił się
                //player.SendBroadcast("Spóźniłeś się! Runda już trwa.", 5);
                //Log.Info($"{player.Nickname} spóźnił się ({timeSinceRoundStart.TotalSeconds} sekund od startu rundy).");

                // Opcjonalnie: Przypisz inną rolę dla spóźnionych graczy
                // Na przykład: Spectator (obserwator)
                player.Role = RoleTypeId.Spectator;
            }
        }

        // Zdarzenie opuszczenia gry przez gracza
        [PluginEvent(ServerEventType.PlayerLeft)]
        private void OnPlayerLeft(PlayerLeftEvent ev)
        {
            // Usuwamy gracza z listy po opuszczeniu gry
            if (playerJoinTimes.ContainsKey(ev.Player))
            {
                playerJoinTimes.Remove(ev.Player);
            }
        }
    }
}