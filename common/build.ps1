$Version="8.0.0"
$publish = ".\publish"

if(Test-Path $publish) { Remove-Item $publish -Force -Recurse }

dotnet pack -c Release -o $publish -p:PackageVersion=$Version .\Domain.Common\Domain.Common.csproj
dotnet pack -c Release -o $publish -p:PackageVersion=$Version .\Infrastructure.Common\Infrastructure.Common.csproj
dotnet pack -c Release -o $publish -p:PackageVersion=$Version .\EnergyDashboard.Common\EnergyDashboard.Common.csproj
dotnet pack -c Release -o $publish -p:PackageVersion=$Version .\HealthCheck\HealthCheck.csproj
dotnet pack -c Release -o $publish -p:PackageVersion=$Version .\JobSchedule\JobSchedule.csproj
dotnet pack -c Release -o $publish -p:PackageVersion=$Version .\Application.Common\Application.Common.csproj
dotnet pack -c Release -o $publish -p:PackageVersion=$Version .\MessageBus.Redis\MessageBus.Redis.csproj

# dotnet nuget push .\publish\Domain.Common.$Version.nupkg --source gitlab
# dotnet nuget push .\publish\Infrastructure.Common.$Version.nupkg --source gitlab
# dotnet nuget push .\publish\EnergyDashboard.Common.$Version.nupkg --source gitlab
# dotnet nuget push .\publish\HealthCheck.$Version.nupkg --source gitlab
# dotnet nuget push .\publish\JobSchedule.$Version.nupkg --source gitlab
# dotnet nuget push .\publish\Application.Common.$Version.nupkg --source gitlab
# dotnet nuget push .\publish\MessageBus.Redis.$Version.nupkg --source gitlab`