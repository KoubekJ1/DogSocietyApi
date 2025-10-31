# DogSocietyAPI
Webové API napsané v C# pomocí ASP.NET určené pro evidenci spolků chovatelů psů.

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
