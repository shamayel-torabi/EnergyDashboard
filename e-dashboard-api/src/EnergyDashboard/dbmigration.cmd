rmdir /S /Q Data\Migrations
dotnet-ef migrations add InitialAppDb --context AppDbContext -o Data/Migrations/AppDb
rem dotnet-ef migrations add InitialIdentity --context IdentityDbContext -o Data/Migrations/IdentityDb
rem dotnet ef migrations add Added --context AppDbContext -o Data/Migrations
rem dotnet-ef migrations script -o Data/Migrations/EnergydashboardDb.sql
rem dotnet ef database update -c AppDbContext
pause