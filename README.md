# SCPSL-Essentials-plugin

Wtyczka do gry [SCP: Secret Laboratory](https://scpslgame.com/), której celem jest dodanie przydatnych ulepszeń do gry.

> ⚠️ Projekt znajduje się w **fazie alpha**. Wszystkie funkcje są we wczesnej fazie rozwoju i ich działanie lub wygląd mogą ulec zmianie.

## ✅ Aktualnie zaimplementowane funkcje

- **Late Join System** – umożliwia dołączanie graczy do rozpoczętej rundy przez określony czas.
- **Lobby System** – poczekalnia przed startem rozgrywki.
- **Losowe komunikaty C.A.S.S.I.E.** – fabularne komunikaty odtwarzane w czasie gry, dodające immersji.
- **System monetek** – rzut monetą może aktywować losowy efekt (można je wyłączać).
- **Lockdown Protocol** – co jakiś czas automatycznie zamyka drzwi w placówce na ustalony czas.
- **System naprawczy pliku konfiguracyjnego** – automatycznie naprawia błędne wpisy w `config.yml`.

## 🔧 Przykład konfiguracji
<details>

```yaml
# Sekcja: Ogólne ustawienia wtyczki
# ---------------------------------
# Czy wtyczka jest włączona?
# Dozwolone wartości: true, false
IsEnabled: true

# Sekcja: Komunikaty fabularne (LosoweKomunikaty)
# ---------------------------------------------
# Czy komunikaty fabularne są włączone?
# Dozwolone wartości: true, false
RandomMessagesEnabled: true
# Minimalny czas między komunikatami (w sekundach)
# Dozwolone wartości: liczba całkowita >= 1
RandomMessagesMinIntervalSeconds: 60
# Maksymalny czas między komunikatami (w sekundach)
# Dozwolone wartości: liczba całkowita >= RandomMessagesMinIntervalSeconds
RandomMessagesMaxIntervalSeconds: 480

# Sekcja: Okresowe zamykanie drzwi (OkresoweZamykanieDrzwi)
# ------------------------------------------------------
# Czy okresowe zamykanie drzwi jest włączone?
# Dozwolone wartości: true, false
DoorLockdownEnabled: true
# Minimalny czas między lockdownami (w sekundach)
# Dozwolone wartości: liczba całkowita >= 1
DoorLockdownMinIntervalSeconds: 180
# Maksymalny czas między lockdownami (w sekundach)
# Dozwolone wartości: liczba całkowita >= DoorLockdownMinIntervalSeconds
DoorLockdownMaxIntervalSeconds: 300
# Czas trwania lockdownu (w sekundach)
# Dozwolone wartości: liczba całkowita >= 1
DoorLockdownDurationSeconds: 15

# Sekcja: System Late Join (LateJoinSystem)
# ---------------------------------------
# Czy system ""late join"" jest włączony?
# Dozwolone wartości: true, false
LateJoinEnabled: true
# Czas na dołączenie do rundy (w sekundach)
# Dozwolone wartości: liczba całkowita >= 1
LateJoinTimeSeconds: 60

# Sekcja: Mechanika Monetek (Monetki)
# -----------------------------------
# Czy mechanika monetek jest włączona?
# Dozwolone wartości: true, false
CoinsEnabled: true

# Ustawienia poszczególnych efektów monetek
# Dozwolone wartości dla wszystkich efektów: true, false
CoinEffectTeleportPlayer: true        # Teleportuje gracza w losowe miejsce
CoinEffectHealPlayer: true            # Leczy gracza do pełnego zdrowia
CoinEffectChangePlayerClass: true     # Zmienia klasę gracza na losową
CoinEffectGiveRandomItem: true        # Daje graczowi losowy przedmiot
CoinEffectGrantDamageImmunity: true   # Daje graczowi odporność na obrażenia na 20 sekund
CoinEffectDropCurrentItem: true       # Upuszcza aktualny przedmiot gracza
CoinEffectSwapWithRandomPlayer: true  # Zamienia miejscami z losowym graczem
CoinEffectTeleportToRandomRoom: true  # Teleportuje gracza do losowego pomieszczenia
CoinEffectBoostDamageOutput: true     # Zwiększa obrażenia gracza na 15 sekund
CoinEffectPullNearbyPlayers: true     # Przyciąga pobliskich graczy
CoinEffectDisguisePlayer: true        # Przebranie gracza na 30 sekund
CoinEffectSwapInventoryWithRandom: true  # Wymienia ekwipunek z losowym graczem
CoinEffectCreateDecoyClone: true      # Tworzy klona gracza
CoinEffectShuffleAllPlayers: true     # Przetasowuje pozycje wszystkich graczy
CoinEffectToggleWeapons: true         # Włącza/wyłącza broń gracza
CoinEffectCreateForceField: true      # Tworzy pole siłowe wokół gracza
CoinEffectTimeShiftPlayers: true      # Przesuwa czas dla wszystkich graczy

# Sekcja: Lobby
# -----------------------------------
# Czy Lobby przed grą ma być włączone?
# Dozwolone wartości: true, false
LobbySystemEnabled: true
```
</details>

Cały szablon konfiguracyjny znajdziesz w pliku `config.yml` generowanym przy pierwszym uruchomieniu pluginu.

📜 Changelog
------------

[Zobacz najnowsze wydanie aby sprawdzić listę zmian.](https://github.com/ttk0721/SCPSL-Essentials-plugin/releases/latest) 

📄 Licencja
-----------

Projekt objęty licencją [GPL-3.0](https://github.com/ttk0721/SCPSL-Essentials-plugin?tab=GPL-3.0-1-ov-file#).
