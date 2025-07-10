using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.IO;

namespace CustomPlugin
{
    public partial class CustomPlugin
    {
        // Zmienne stanu pluginu
        private bool roundInProgress = false;
        private bool spyAssigned = false;
        private static System.Random rng = new System.Random();
        private readonly ItemType Scp035ItemType = ItemType.KeycardO5;
        public DateTime roundStartTime;
        private PluginConfig config; // Obiekt konfiguracji

        [PluginEntryPoint("CustomPlugin", "0.0.5-alpha", "Rozbudowany plugin z dodatkowymi funkcjami", "Autor:ttk0721")]
        private void OnLoaded()
        {
            // Ścieżka bezwzględna do folderu wtyczek
            string pluginsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SCP Secret Laboratory", "PluginAPI", "plugins");

            // Ścieżka do folderu global
            string globalDirectory = Path.Combine(pluginsDirectory, "global");
            if (!Directory.Exists(globalDirectory))
            {
                Directory.CreateDirectory(globalDirectory);
                Log.Info($"[CustomPlugin] Utworzono folder global: {globalDirectory}\n");
            }

            // Ścieżka do folderu wtyczki wewnątrz folderu global
            string pluginDirectory = Path.Combine(globalDirectory, "CustomPlugin");
            if (!Directory.Exists(pluginDirectory))
            {
                Directory.CreateDirectory(pluginDirectory);
                Log.Info($"[CustomPlugin] Utworzono folder wtyczki w folderze global: {pluginDirectory}\n");
            }

            // Ścieżka do pliku konfiguracyjnego
            string configPath = Path.Combine(pluginDirectory, "config.yml");

            // Wczytanie i weryfikacja konfiguracji za pomocą IsConfigValid
            config = IsConfigValid.LoadAndValidateConfig(configPath);

            // Sprawdzenie, czy wtyczka jest włączona
            if (!config.IsEnabled)
            {
                Log.Info($"[CustomPlugin] Wtyczka jest wyłączona w konfiguracji.\n");
                return;
            }

            // Rejestracja zdarzeń
            EventManager.RegisterEvents(this);
            EventManager.RegisterEvents(this, new Monetki(this, config));
            EventManager.RegisterEvents(this, new LosoweKomunikaty(this, config));
            EventManager.RegisterEvents(this, new LosoweAwarieSwiatla(this));
            EventManager.RegisterEvents(this, new OkresoweZamykanieDrzwi(this, config));
            EventManager.RegisterEvents(this, new LosoweAwarieWind(this));
            EventManager.RegisterEvents(this, new LateJoinSystem(this, config));
            //EventManager.RegisterEvents(this, new Lobby(this, config)); // Dodajemy rejestrację klasy Lobby

            Log.Info($"[CustomPlugin] CustomPlugin 0.0.5-alpha załadowany!\n");
            SetupCustomItems();

            // Uzyskujemy lub tworzymy obiekt CoroutineRunner
            CoroutineRunner coroutineRunner = GameObject.FindObjectOfType<CoroutineRunner>();
            if (coroutineRunner == null)
            {
                GameObject go = new GameObject("CoroutineRunner");
                UnityEngine.Object.DontDestroyOnLoad(go); // Zapobiega niszczeniu obiektu przy przeładowaniu sceny
                coroutineRunner = go.AddComponent<CoroutineRunner>();
            }

            // Rejestrujemy eventy dla Lobby, przekazując CoroutineRunner
            EventManager.RegisterEvents(this, new Lobby(coroutineRunner, config));

            Log.Info($"[CustomPlugin] CustomPlugin 0.0.5-alpha załadowany!\n");
            Log.Warning($"[CustomPlugin] Uwaga: Wtyczka w fazie alpha i może zawierać błędy.\n");
            SetupCustomItems();

        }

        // Funkcje pomocnicze
        private void SetupCustomItems()
        {
            EventManager.RegisterEvents<CustomItemHandler>(this);
        }

        // Zdarzenie dołączenia gracza
        [PluginEvent(ServerEventType.PlayerJoined)]
        private void OnPlayerJoined(PlayerJoinedEvent ev)
        {
            Player player = ev.Player;
            Log.Info($"[CustomPlugin] Gracz {player.Nickname} dołączył do gry.\n");

            // Sprawdzamy, czy runda się rozpoczęła (dla przyszłych potrzeb)
            if (!Round.IsRoundStarted)
            {
                // Powiadomienie usunięte na żądanie
            }
        }

        // Zdarzenie rozpoczęcia rundy
        [PluginEvent(ServerEventType.RoundStart)]
        private void OnRoundStart(RoundStartEvent ev)
        {
            roundInProgress = true;
            roundStartTime = DateTime.Now;
            Log.Info($"[CustomPlugin] Runda rozpoczęta o {roundStartTime}.\n");
        }

        // Zdarzenie zakończenia rundy
        [PluginEvent(ServerEventType.RoundEnd)]
        private void OnRoundEnd(RoundEndEvent ev)
        {
            roundInProgress = false;
            spyAssigned = false;
            Log.Info($"[CustomPlugin] Runda zakończona.\n");
        }
    }
}