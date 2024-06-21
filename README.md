# KingICT Akademija 2024

Za potrebe prijemnog testa na KingICT akademiju kreiran je middleware koji omogućava komunikaciju sa REST API-om dummyjson.com.<br>
Funkcionalnosti projekta testiraju se putem Swagger UI-a (preporuča se) ili ručno.<br>
Osnovni račun za prijavu u sustav i pravilan rad je ```emilys | emilyspass```. 

## Kloniranje repozitorija i pokretanje projekta
Kako bi se projekt mogao pokrenuti potrebno klonirati repozitorij te ga pokrenuti putem IDE-a ili terminala. <br>
Nužna je ovisnost o .NET 8 verziji. Dostupno na https://dotnet.microsoft.com/en-us/download 

### Kloniranje putem IDE-a (preporuča se Visual Studio / Visual Studio Code)
Potrebno je pozicionirati se na mjesto gdje želimo klonirati ovaj repozitorij i zalijepiti URL: ```https://github.com/tivanovic21/KingICT-akademija.git```. 

### Kloniranje putem terminala
Potrebno je pozicionirati se na mjesto gdje želimo klonirati ovaj repozitorij i upisati komandu:
 ``` git clone https://github.com/tivanovic21/KingICT-akademija.git ```

### Pokretanje projekta
Nakon što je projekt kloniran potrebno se je pozicionirati u direktoriji koji stvori pod imenom "KingICT-akademija".<br>
Za pokretanje projekta potrebno je pokrenuti projekt putem Visual Studia ili korištenjem ```dotnet run``` metode.<br>
Projekt se pokreće na adresi: https://localhost:7157/swagger/index.html ako nije drugačije specificirano. 

### Pokretanje jediničnih testova
Za pokretanje jediničnih testova potrebno je pokrenuti testove putem Visual Studia ili korištenjem ```dotnet test``` metode.<br>
Rezultati testova vidljivi su unutar Visual Studia ili terminala ovisno o metodi koja je korištena za pokretanje testova. 

## Funkcionalnosti 

Ovaj projekt pokriva funkcionalnosti autentifikacije i autorizacije i funkcionalnosti rada s proizvodima. <br>

### Auth
#### GET /api/Auth
Dohvat liste korisničkih podataka za prijavu s dummyjson.com API-a. Bilo koji račun dohvaćen ovom metodom može se koristiti za prijavu u sustav. 
```
curl -X 'GET' \
  'https://localhost:7157/api/Auth' \
  -H 'accept: application/json'
```

#### POST /api/Auth
Prijava u sustav koristeći dummyjson.com API. Osnovni račun za prijavu je ```emilys | emilyspass```.<br>
Prilikom prijave kreira se JWT token koji zaštićuje metode vezane za rad s proizvodima te se pohranjuje u HTTP cookie. 
```
curl -X 'POST' \
  'https://localhost:7157/api/Auth' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "emilys",
  "password": "emilyspass"
}'
```

#### POST /api/Auth/logout
Odjava iz sustava. Metoda dostupna samo za prijavljene korisnike.<br>
U procesu odjave uklanja se HTTP cookie s JWT tokenom te se isti dodaje na listu zabranjenih tokena što onemogućuje korištenje istog tokena nakon odjave. 
```
curl -X 'POST' \
  'https://localhost:7157/api/Auth/logout' \
  -H 'accept: */*' \
  -d ''
```

### Products
Sve metode u ovom dijelu su zaštićene te su dostupne samo prijavljenim korisnicima. 
#### GET /api/Products
Dohvat liste proizvoda s dummyjson.com API-a.
```
curl -X 'GET' \
  'https://localhost:7157/api/Products' \
  -H 'accept: application/json'
```
#### GET /api/Products/{id}
Dohvat pojedinačnog proizvoda s dummyjson.com API-a prema jedinstvenom Id-u. 
```
curl -X 'GET' \
  'https://localhost:7157/api/Products/45' \
  -H 'accept: application/json'
```
#### GET /api/Products/search
Dohvat liste proizvoda s dummyjson.com API-a prema riječi u naslovu proizvoda.
```
curl -X 'GET' \
  'https://localhost:7157/api/Products/search?title=lipstick' \
  -H 'accept: application/json'
```
#### GET /api/Products/filter
Dohvat liste proizvoda s dummyjson.com API-a filtriranim prema kategoriji i maksimalnoj cijeni.
```
curl -X 'GET' \
  'https://localhost:7157/api/Products/filter?category=beauty&price=10' \
  -H 'accept: application/json'
```

## Testovi
Projekt sadrži napisane jedinične testove za kontrolere koristeći xUnit i FakeItEasy.<br>
Da bi se testovi pokrenuli potrebno ih je pokrenuti putem Visual Studia ili putem komande ```dotnet test```. 
