using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

public partial class CustomPlugin
{
    // Zmienne stanu pluginu
    private bool roundInProgress = false;
    private bool spyAssigned = false;
    private static System.Random rng = new System.Random();
    private readonly ItemType Scp035ItemType = ItemType.KeycardO5;
    private DateTime roundStartTime;
    private readonly TimeSpan lateJoinTime = TimeSpan.FromMinutes(1);
    private bool lateJoinEnabled = true;
    private bool voiceChatEnabled = false;
    private Dictionary<Player, DateTime> playerJoinTimes = new Dictionary<Player, DateTime>();

    [PluginEntryPoint("CustomPlugin", "2.0.1", "Rozbudowany plugin z dodatkowymi funkcjami", "Autor:ttk0721")]
    private void OnLoaded()
    {
        EventManager.RegisterEvents(this);
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
        playerJoinTimes[ev.Player] = DateTime.Now;
        if (voiceChatEnabled)
        {
            ev.Player.GameObject.AddComponent<VoiceModulator>().SetRandomVoice();
        }
    }
}