# rmdir /S /Q Todo.Infrastructure\Data\Migrations

$Migrations = ".\Infrastructure\Persistence\Migrations"

if(Test-Path $Migrations) { Remove-Item $Migrations -Force -Recurse }

dotnet ef migrations add `
    --project Infrastructure\EnergyDashboard.Infrastructure.csproj `
    --startup-project EnergyDashboard\EnergyDashboard.csproj `
    --context EnergyDashboard.Infrastructure.Persistence.AppDbContext `
    --configuration Debug Initial --output-dir Persistence\Migrations
pause