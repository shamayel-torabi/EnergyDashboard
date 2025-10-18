# rmdir /S /Q Todo.Infrastructure\Data\Migrations
dotnet ef migrations add `
    --project Todo.Infrastructure\ProjectManagment.Infrastructure.csproj `
    --startup-project Todo.Api\ProjectManagment.Api.csproj `
    --context ProjectManagment.Infrastructure.Data.ApplicationDbContext `
    --configuration Debug Initial --output-dir Data\Migrations
pause