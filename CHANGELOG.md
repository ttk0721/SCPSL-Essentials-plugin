## Changelog

### v2.0.2 (2025-04-01)

- **Okresowe Zamykanie Drzwi**:
  - Naprawiono problem z blokowaniem drzwi podczas lockdownu – teraz wszystkie drzwi (otwarte i zamkniête) s¹ blokowane na czas trwania procedury.
  - Otwarte drzwi s¹ zamykane, a po 15 sekundach wszystkie drzwi s¹ odblokowywane.

- **Komunikaty C.A.S.S.I.E.**:
  - Zaktualizowano komunikaty fabularne, aby by³y zgodne z ograniczeniami C.A.S.S.I.E. (tylko angielskie s³owa, brak kropek, numery jako cyfry).
  - Zmieniono interwa³ komunikatów na losowy (1–8 minut).

- **Late Join System**:
  - Dodano system "late join" – gracze do³¹czaj¹cy w ci¹gu pierwszej minuty rundy otrzymuj¹ rolê Class-D, spóŸnieni zostaj¹ obserwatorami (Spectator).
  - Przeniesiono logikê z `CustomPlugin.cs` do osobnego pliku `Late_join_system.cs`.