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
        private readonly PluginConfig config; // Dodajemy konfigurację
        private readonly System.Random rng = new System.Random();
        private readonly Timer timer;
        private readonly List<string> komunikatyFabularne;

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

        public LosoweKomunikaty(CustomPlugin plugin, PluginConfig config)
        {
            this.plugin = plugin;
            this.config = config;

            komunikatyFabularne = new List<string>(komunikaty);

            timer = new Timer();
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;
            SetRandomInterval();
        }

        private void SetRandomInterval()
        {
            int delaySeconds = rng.Next(config.RandomMessagesMinIntervalSeconds, config.RandomMessagesMaxIntervalSeconds + 1);
            timer.Interval = delaySeconds * 1000;
            Log.Info($"[LosoweKomunikaty] Ustawiono nowy interwał dla komunikatu: {delaySeconds} sekund.\n");
        }

        [PluginEvent(ServerEventType.RoundStart)]
        private void OnRoundStart(RoundStartEvent ev)
        {
            if (!config.RandomMessagesEnabled)
            {
                Log.Info($"[LosoweKomunikaty] Komunikaty fabularne są wyłączone w konfiguracji.\n");
                return;
            }

            timer.Start();
            Log.Info($"[LosoweKomunikaty] Rozpoczęto wysyłanie losowych komunikatów fabularnych.\n");
        }

        [PluginEvent(ServerEventType.RoundEnd)]
        private void OnRoundEnd(RoundEndEvent ev)
        {
            timer.Stop();
            Log.Info($"[LosoweKomunikaty] Zatrzymano wysyłanie losowych komunikatów fabularnych.\n");
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (komunikatyFabularne.Count == 0) return;

            string komunikat = komunikatyFabularne[rng.Next(komunikatyFabularne.Count)];
            RespawnEffectsController.PlayCassieAnnouncement(komunikat, false, false);
            Log.Info($"[LosoweKomunikaty] Wysłano komunikat fabularny: {komunikat}\n");

            SetRandomInterval();
            timer.Start();
        }
    }
}