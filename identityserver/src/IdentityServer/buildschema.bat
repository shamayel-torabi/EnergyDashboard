rmdir /S /Q "Data/Migrations"

dotnet-ef migrations add Users -c IdentityDbContext -o Data/Migrations
pause
