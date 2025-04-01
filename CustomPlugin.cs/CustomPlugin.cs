using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace CustomPlugin
{
    public partial class CustomPlugin
    {
        // Zmienne stanu pluginu
        private bool roundInProgress = false;
        private bool spyAssigned = false;
        private static System.Random rng = new System.Random();
        private readonly ItemType Scp035ItemType = ItemType.KeycardO5;
        private DateTime roundStartTime;
        private bool voiceChatEnabled = false;

        [PluginEntryPoint("CustomPlugin", "2.0.2", "Rozbudowany plugin z dodatkowymi funkcjami", "Autor:ttk0721")]
        private void OnLoaded()
        {
            EventManager.RegisterEvents(this);
            EventManager.RegisterEvents(this, new Monetki(this)); // Rejestracja Monetki
            EventManager.RegisterEvents(this, new LosoweKomunikaty(this)); // Rejestracja LosoweKomunikaty
            EventManager.RegisterEvents(this, new LosoweAwarieSwiatla(this)); // Rejestracja LosoweAwarieSwiatla
            EventManager.RegisterEvents(this, new OkresoweZamykanieDrzwi(this)); // Rejestracja OkresoweZamykanieDrzwi
            EventManager.RegisterEvents(this, new LosoweAwarieWind(this)); // Rejestracja LosoweAwarieWind
            EventManager.RegisterEvents(this, new TeslaPersonelPlacowki(this)); // Rejestracja TeslaPersonelPlcowki
            EventManager.RegisterEvents(this, new LateJoinSystem(this)); // Rejestracja LateJoinSystem
            Log.Info("CustomPlugin 2.0 załadowany!");
            InitializeVoiceChat();
            SetupCustomItems();
        }

        // Inicjalizacja czatu głosowego
        private void InitializeVoiceChat()
        {
            voiceChatEnabled = true;
            Log.Info("Inicjalizacja czatu głosowego...");
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
            if (voiceChatEnabled)
            {
                ev.Player.GameObject.AddComponent<VoiceModulator>().SetRandomVoice();
            }
        }
    }
}