# DogSocietyAPI
**ASP.NET bohužel neumožňuje Swagger UI poskytovat na endpointu /api, proto se nachází na endpointu /swagger**
<p>Webové API napsané v C# pomocí ASP.NET určené pro evidenci spolků chovatelů psů.</p>

## Popis
### Použité technologie
- ASP.NET
- Entity Framework
- PostgreSQL

API je rozdělené na 4 různé sekce, každá sekce má několik endpointů
#### Address
<p>Fyzické adresy míst v České republice.</p>
<p> > /Address (GET) - Vrátí všechny adresy </p>
<p> > /Address/{id} (GET) - Vrátí adresu dle ID </p>
<p> > /Address/create (POST) - Vytvoří novou adresu </p>

#### Association
<p>Spolky chovatelů psů.</p>
<p> > /Association (GET) - Vrátí všechny spolky </p>
<p> > /Association/{id} (GET) - Vrátí spolek dle ID </p>
<p> > /Association/mine (GET) - Vrátí všechny spolky jejichž předsedou je přihlášený uživatel </p>
<p> > /Association/create (POST) - Vytvoří nový spolek </p>
<p> > /Association/statutes/{associationId} (GET) - Vrátí aktuálně platné stanovy pro daný spolek </p>
<p> > /Association/changestatute (POST) - Aktualizuje stanovy pro daný spolek </p>

#### Event
<p>Události, kterých se mohou spolky zúčastnit.</p>
<p> > /Event (GET) - Vrátí všechny události </p>
<p> > /Event/upcoming (GET) - Vrátí nadcházející události </p>
<p> > /Event/{id} (GET) - Vrátí událost dle ID </p>
<p> > /Event/participation/{associationId} (GET) - Vrátí události, kterých je zůčastněn daný spolek </p>
<p> > /Event/upcomingparticipation/{associationId} (GET) - Vrátí nadcházející události, kterých je zůčastněn daný spolek </p>
<p> > /Event/joinevent (POST) - Připojí daný spolek k dané události </p>
<p> > /Event/create (POST) - Vytvoří novou událost </p>

#### User
<p>Uživatelé systému.</p>
<p> > /User/register (POST) - Registruje nového uživatele </p>
<p> > /User/login (POST) - Přihlásí uživatele </p>

## Požadavky
- .NET verze 9.0.306 nebo vyšší na hostovacím zařízení
- Instance PostgreSQL databáze verze 18.0 nebo vyšší

## Nasazení
- Stáhněte si zdrojový kód z tohoto GitHub repozitáře.

### Příprava databáze
- Spusťte PostgreSQL databázi a poznamenejte si připojovací textový řetězec (Connection string)
- Stáhněte si EF Core tools pomocí následujícího příkazu v příkazové řádce: dotnet tool install --global dotnet-ef
- Do souboru "appsettings.Development" v sekci "ConnectionStrings" v řádku "localhost" (řádek 10) nahraďte text vaším připojovacím textovým řetězcem k databázi
- Vytvořte databázové schéma pomocí následujícího příkazu v příkazové řádce: dotnet ef database update

### Spuštění

#### Debug
- Ujistěte se, že v souboru "appsettings.Development" v sekci "ConnectionStrings" v řádku "localhost" (řádek 10) je Váš připojovací textový řetězec k databázi
- Aplikaci spusťe pomocí příkazu dotnet run
- Aplikace se spustí na TCP portu 5295 s protokolem HTTP
- **ASP.NET bohužel neumožňuje Swagger UI poskytovat na endpointu /api, proto se nachází na endpointu /swagger**
