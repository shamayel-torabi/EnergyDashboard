$appVersion ="7.0.9"
$appName = "identityserver"

# Remove-Item .\publish
# dotnet restore -r linux-x64
# dotnet publish -c Release -o .\publish -r linux-x64 --self-contained false --no-restore

# docker build -t registry.hrec.local/e-dashboard/identityserver:$Version .
# docker login registry.hrec.local
# docker push registry.hrec.local/e-dashboard/identityserver:$Version

helm uninstall $appName
helm upgrade --install $appName ./deploy --set image.tag=$appVersion
