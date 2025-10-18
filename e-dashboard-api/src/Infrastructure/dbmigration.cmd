rmdir /S /Q Persistence\Migrations
dotnet-ef migrations add UpdateAppDb --context AppDbContext -o Persistence/Migrations
pause