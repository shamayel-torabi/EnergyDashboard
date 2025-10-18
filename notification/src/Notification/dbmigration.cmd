rmdir /S /Q Data\Migrations
dotnet-ef migrations add InitialAppDb --context MessageDbContext -o Data/Migrations
rem dotnet-ef migrations script -o script.sql -c MessageDbContext
pause