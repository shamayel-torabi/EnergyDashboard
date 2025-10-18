rem rmdir /S /Q Infrastructure\Persistence\Migrations
dotnet-ef migrations add UpdateNewAppDb --context ApplicationDbContext -o Infrastructure/Persistence/Migrations
dotnet-ef migrations script -o script.sql -c ApplicationDbContext
pause