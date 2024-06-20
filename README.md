# KingICT Akademija 2024

Za potrebe prijemnog testa na KingICT akademiju kreiran je middleware koji omogućava komunikaciju sa REST API-om dummyjson.com.<br>
Za pokretanje projekta potrebno je pokrenuti projekt putem Visual Studia ili korištenjem ```dotnet run``` metode.<br>
Za pokretanje jediničnih testova potrebno je pokrenuti testove putem Visual Studia ili korištenjem ```dotnet test``` metode.<br>
Funkcionalnosti projekta testiraju se putem Swagger UI-a ili ručno.<br>

Osnovni račun za prijavu u sustav i pravilan rad je ```emilys | emilyspass```. 

## Funkcionalnosti 

Ovaj projekt pokriva funkcionalnosti autentifikacije i autorizacije i funkcionalnosti rada s proizvodima. 

### Auth
#### GET /api/Auth
Dohvat liste korisničkih podataka za prijavu s dummyjson.com API-a. 
```
curl -X 'GET' \
  'https://localhost:7157/api/Auth' \
  -H 'accept: application/json'
```

#### POST /api/Auth
Prijava u sustav koristeći dummyjson.com API.
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
Odjava iz sustava. Metoda dostupna samo za prijavljene korisnike. 
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
