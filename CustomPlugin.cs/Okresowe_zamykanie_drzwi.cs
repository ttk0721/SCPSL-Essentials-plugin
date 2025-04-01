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
        private readonly System.Random rng = new System.Random(); // Lokalna instancja rng
        private bool isDoorLockdownActive = false;

        public OkresoweZamykanieDrzwi(CustomPlugin plugin)
        {
            this.plugin = plugin;
        }

        // Zdarzenie startu rundy, aby uruchomić harmonogram zamykania drzwi
        [PluginEvent(ServerEventType.RoundStart)]
        private void OnRoundStart()
        {
            StartDoorLockdownScheduler();
        }

        // Harmonogram zamykania drzwi
        private async void StartDoorLockdownScheduler()
        {
            while (true) // Pętla działa przez całą rundę
            {
                // Losowy czas oczekiwania między 180 a 300 sekund (3–5 minut)
                int delaySeconds = rng.Next(180, 301); // Używamy lokalnego rng
                await Task.Delay(delaySeconds * 1000); // Konwersja na milisekundy

                // Sprawdzenie, czy runda nadal trwa
                if (!Round.IsRoundStarted)
                    break;

                // Uruchomienie procedury zamykania drzwi
                await ExecuteDoorLockdown();
            }
        }

        // Procedura zamykania i blokowania wszystkich drzwi
        private async Task ExecuteDoorLockdown()
        {
            if (isDoorLockdownActive)
                return; // Zapobiega nakładaniu się procedur

            isDoorLockdownActive = true;
            try
            {
                // Log dla debugowania, aby upewnić się, że funkcja jest wywoływana
                Log.Info("Rozpoczynanie procedury lockdown: zamykanie i blokowanie wszystkich drzwi.");

                // Komunikat C.A.S.S.I.E. z alarmem, efektami glitch i wyświetleniem na ekranie
                Log.Info("Wysyłanie komunikatu C.A.S.S.I.E...");
                Cassie.Message("Facility activated lockdown protocol all doors closed", true, true, true);

                // Pobranie wszystkich drzwi na mapie
                var doors = DoorVariant.AllDoors;

                // Iteracja po wszystkich drzwiach: zamykanie otwartych i blokowanie wszystkich
                foreach (var door in doors)
                {
                    if (door == null)
                        continue;

                    // Zamknięcie drzwi, jeśli są otwarte
                    if (door.TargetState) // TargetState == true oznacza, że drzwi są otwarte
                    {
                        door.NetworkTargetState = false; // Zamknij drzwi
                    }

                    // Zablokowanie drzwi, niezależnie od ich stanu
                    door.ServerChangeLock(DoorLockReason.AdminCommand, true);
                }

                // Log dla debugowania
                Log.Info("Wszystkie drzwi zostały zamknięte i zablokowane w ramach procedury lockdown.");

                // Odblokowanie drzwi po 15 sekundach
                await Task.Delay(15000); // 15 sekund opóźnienia

                foreach (var door in doors)
                {
                    if (door == null)
                        continue;

                    // Odblokowanie drzwi
                    door.ServerChangeLock(DoorLockReason.AdminCommand, false);
                }

                Log.Info("Wszystkie drzwi zostały odblokowane po 15 sekundach.");
            }
            catch (System.Exception ex)
            {
                Log.Error($"Błąd podczas zamykania drzwi: {ex.Message}");
            }
            finally
            {
                isDoorLockdownActive = false;
            }
        }

        // Zdarzenie interakcji z drzwiami
        [PluginEvent(ServerEventType.PlayerInteractDoor)]
        private void OnDoorInteract(Player player, DoorVariant door, bool canOpen)
        {
            if (rng.Next(100) < 5 && door.RequiredPermissions.RequiredPermissions != KeycardPermissions.None) // Używamy lokalnego rng
            {
                door.ServerChangeLock(DoorLockReason.AdminCommand, true);
                door.NetworkTargetState = !door.TargetState;
                player.SendBroadcast("Drzwi zostały losowo przełączone!", 3);
            }
        }
    }
}