## Changelog

### v2.0.2 (2025-04-01)

- **Okresowe Zamykanie Drzwi**:
  - Naprawiono problem z blokowaniem drzwi podczas lockdownu � teraz wszystkie drzwi (otwarte i zamkni�te) s� blokowane na czas trwania procedury.
  - Otwarte drzwi s� zamykane, a po 15 sekundach wszystkie drzwi s� odblokowywane.

- **Komunikaty C.A.S.S.I.E.**:
  - Zaktualizowano komunikaty fabularne, aby by�y zgodne z ograniczeniami C.A.S.S.I.E. (tylko angielskie s�owa, brak kropek, numery jako cyfry).
  - Zmieniono interwa� komunikat�w na losowy (1�8 minut).

- **Late Join System**:
  - Dodano system "late join" � gracze do��czaj�cy w ci�gu pierwszej minuty rundy otrzymuj� rol� Class-D, sp�nieni zostaj� obserwatorami (Spectator).
  - Przeniesiono logik� z `CustomPlugin.cs` do osobnego pliku `Late_join_system.cs`.