using System.Threading.Tasks;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;

namespace CustomPlugin
{
    public class OkresoweZamykanieDrzwi
    {
        private readonly CustomPlugin plugin;
        private readonly PluginConfig config; // Dodajemy konfigurację
        private readonly System.Random rng = new System.Random();
        private bool isDoorLockdownActive = false;

        public OkresoweZamykanieDrzwi(CustomPlugin plugin, PluginConfig config)
        {
            this.plugin = plugin;
            this.config = config;
        }

        [PluginEvent(ServerEventType.RoundStart)]
        private void OnRoundStart()
        {
            if (!config.DoorLockdownEnabled)
            {
                Log.Info($"[OkresoweZamykanieDrzwi] Okresowe zamykanie drzwi jest wyłączone w konfiguracji.\n");
                return;
            }

            StartDoorLockdownScheduler();
        }

        private async void StartDoorLockdownScheduler()
        {
            while (true)
            {
                int delaySeconds = rng.Next(config.DoorLockdownMinIntervalSeconds, config.DoorLockdownMaxIntervalSeconds + 1);
                await Task.Delay(delaySeconds * 1000);

                if (!Round.IsRoundStarted)
                    break;

                await ExecuteDoorLockdown();
            }
        }

        private async Task ExecuteDoorLockdown()
        {
            if (isDoorLockdownActive)
                return;

            isDoorLockdownActive = true;
            try
            {
                Log.Info($"[OkresoweZamykanieDrzwi] Rozpoczynanie procedury lockdown: zamykanie i blokowanie wszystkich drzwi.\n");
                Log.Info($"[OkresoweZamykanieDrzwi] Wysyłanie komunikatu C.A.S.S.I.E...\n");
                Cassie.Message("Facility activated lockdown protocol all doors closed", true, true, true);

                var doors = DoorVariant.AllDoors;

                foreach (var door in doors)
                {
                    if (door == null)
                        continue;

                    if (door.TargetState)
                    {
                        door.NetworkTargetState = false;
                    }

                    door.ServerChangeLock(DoorLockReason.AdminCommand, true);
                }

                Log.Info($"[OkresoweZamykanieDrzwi] Wszystkie drzwi zostały zamknięte i zablokowane w ramach procedury lockdown.\n");

                await Task.Delay(config.DoorLockdownDurationSeconds * 1000);

                foreach (var door in doors)
                {
                    if (door == null)
                        continue;

                    door.ServerChangeLock(DoorLockReason.AdminCommand, false);
                }

                Log.Info($"[OkresoweZamykanieDrzwi] Wszystkie drzwi zostały odblokowane po {config.DoorLockdownDurationSeconds} sekundach.\n");
            }
            catch (System.Exception ex)
            {
                Log.Error($"[OkresoweZamykanieDrzwi] Błąd podczas zamykania drzwi: {ex.Message}\n");
            }
            finally
            {
                isDoorLockdownActive = false;
            }
        }

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
    }
}