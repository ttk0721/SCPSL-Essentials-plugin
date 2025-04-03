using System.Collections;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PlayerRoles;

namespace CustomPlugin
{
    public class Lobby
    {
        private readonly CoroutineRunner coroutineRunner; // Referencja do MonoBehaviour
        private readonly PluginConfig config;
        private bool isWaitingForPlayers;

        public Lobby(CoroutineRunner coroutineRunner, PluginConfig config)
        {
            this.coroutineRunner = coroutineRunner;
            this.config = config;
            this.isWaitingForPlayers = false;
        }

        [PluginEvent]
        private void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            if (!config.LobbySystemEnabled)
            {
                Log.Info("[Lobby] Mechanika lobby jest wyłączona w konfiguracji (LobbyEnabled: false).");
                return;
            }

            isWaitingForPlayers = true;
            foreach (Player player in Player.GetPlayers())
            {
                SetPlayerToLobby(player);
            }
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        private void OnPlayerJoined(PlayerJoinedEvent ev)
        {
            if (!config.LobbySystemEnabled)
            {
                Log.Info($"[Lobby] Mechanika lobby jest wyłączona w konfiguracji (LobbyEnabled: false). Gracz {ev.Player.Nickname} nie zostanie ustawiony w lobby.");
                return;
            }

            if (isWaitingForPlayers)
            {
                SetPlayerToLobby(ev.Player);
            }
        }

        [PluginEvent]
        private void OnRoundStart(RoundStartEvent ev)
        {
            isWaitingForPlayers = false;
            // Używamy CoroutineRunner do uruchomienia korutyny
            coroutineRunner.StartCoroutine(ResetPlayersCoroutine());
        }

        private IEnumerator ResetPlayersCoroutine()
        {
            foreach (Player player in Player.GetPlayers())
            {
                if (player.Role == RoleTypeId.Tutorial)
                {
                    player.Role = RoleTypeId.Spectator;
                }
            }
            Log.Info("[Lobby] Zmieniono role graczy na obserwatora przed przydzieleniem nowych ról.");
            yield break;
        }


        private void SetPlayerToLobby(Player player)
        {
            player.Role = RoleTypeId.Tutorial;
            Vector3 escapeFinalPosition = new Vector3(130f, 989f, 20f);
            player.Position = escapeFinalPosition + Vector3.up * 2f;
            Log.Info($"[Lobby] Gracz {player.Nickname} ustawiony na rolę Tutorial i teleportowany do escape_final (współrzędne: {player.Position}).");
        }
    }
}
