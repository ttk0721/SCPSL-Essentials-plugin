using System;
using System.Collections.Generic;
using System.Timers;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using Respawning;

namespace CustomPlugin
{
    public class LosoweKomunikaty
    {
        private readonly CustomPlugin plugin;
        private readonly System.Random rng = new System.Random();
        private readonly Timer timer;
        private readonly List<string> komunikatyFabularne;

        // Lista fabularnych komunikatów w języku angielskim, zgodnych z C.A.S.S.I.E.
        private readonly List<string> komunikaty = new List<string>
        {
            "system failure detected in CASSIE core",
            "anomaly detected in sector 3 immediate evacuation",
            "security systems in heavy containment zone breach has an error",
            "unauthorized access detected in containment area",
            "reactor in sector 5 critical temperature alert",
            "danger detected in secure system",
            "systems in light containment zone offline",
            "power failure detected in command sector",
            "security protocol breach detected in SCP 1 7 3 cell",
            "CASSIE system alert noise detected"
        };

        public LosoweKomunikaty(CustomPlugin plugin)
        {
            this.plugin = plugin;

            // Inicjalizacja listy komunikatów
            komunikatyFabularne = new List<string>(komunikaty);

            // Ustawienie timera z początkowym losowym interwałem (1-8 minut)
            timer = new Timer();
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false; // Wyłączamy automatyczne powtarzanie, bo będziemy ręcznie ustawiać nowy interwał
            SetRandomInterval(); // Ustawiamy początkowy losowy interwał
        }

        // Funkcja ustawiająca losowy interwał między 1 a 8 minut
        private void SetRandomInterval()
        {
            // Losowy czas między 1 minutą (60 sekund) a 8 minutami (480 sekund)
            int delaySeconds = rng.Next(60, 481); // 60 do 480 sekund
            timer.Interval = delaySeconds * 1000; // Konwersja na milisekundy
            Log.Info($"Ustawiono nowy interwał dla komunikatu: {delaySeconds} sekund.");
        }

        // Zdarzenie rozpoczęcia rundy
        [PluginEvent(ServerEventType.RoundStart)]
        private void OnRoundStart(RoundStartEvent ev)
        {
            timer.Start();
            Log.Info("Rozpoczęto wysyłanie losowych komunikatów fabularnych.");
        }

        // Zdarzenie zakończenia rundy
        [PluginEvent(ServerEventType.RoundEnd)]
        private void OnRoundEnd(RoundEndEvent ev)
        {
            timer.Stop();
            Log.Info("Zatrzymano wysyłanie losowych komunikatów fabularnych.");
        }

        // Funkcja wywoływana przez timer
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (komunikatyFabularne.Count == 0) return;

            // Losowy komunikat
            string komunikat = komunikatyFabularne[rng.Next(komunikatyFabularne.Count)];

            // Wysłanie komunikatu przez C.A.S.S.I.E.
            RespawnEffectsController.PlayCassieAnnouncement(komunikat, false, false);
            Log.Info($"Wysłano komunikat fabularny: {komunikat}");

            // Ustawienie nowego losowego interwału i ponowne uruchomienie timera
            SetRandomInterval();
            timer.Start();
        }
    }
}