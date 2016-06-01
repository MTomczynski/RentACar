# RentACar_v2
Aplikacja symulujaca dzialanie wypozyczalni samochodowej.
http://car-rental-uek.azurewebsites.net/

1. Zaloz konto w aplikacji.
2. Wybierz z listy dostepnych samochodow interesujacy Cie model.
3. Wybierz date zwrotu.
4. Dokonaj platnosci.
  - Jesli przed dokonaniem platnosci uplynelo wiecej niz 3 minuty, 
  aplikacja anuluje rezerwacje oraz zwroci samochod do puli dostepnych do wypozyczenia.
  - Jesli platnosc zostanie odrzucona, aplikacja anuluje rezerwacje oraz zwroci
  samochod do puli dostepnych do wypozyczenia.
5. Sprawdz zakladke w Menu>My Rents status swojego wypozyczenia.

Dodatkowe czynnosci
- Mozna skorzystac z formularza kontaktowego do firmy.
- Po zalogowaniu sie na konto z uprawnieniami administratora mozna wejsc na strone
  http://car-rental-uek.azurewebsites.net/cars po czym dowolnie modyfikowac dostepne samochody,
  rowniez z opcja dodawania oraz usuwania.
- Jesli uzytkownik wypozycza samochod na dluzej niz 7 dni, przysluguje mu rabat w wysokosci 10%
- Jesli uzytkownik wypozyczyl wiecej niz 3 samochody w ostatnim miesiacu, przysluguje mu rabat
  w wysokosci 20% (Rabaty nie sumuja sie, wygrywa silniejszy).
