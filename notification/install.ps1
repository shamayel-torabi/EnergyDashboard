$Version="8.0.0"

# Remove-Item .\publish
# dotnet restore -r linux-x64
# dotnet publish -c Release -o .\publish -r linux-x64 --self-contained false --no-restore

# docker build -t registry.hrec.local/e-dashboard/notification:$Version .
# docker login registry.hrec.local
# docker push registry.hrec.local/e-dashboard/notification:$Version

helm uninstall notification
helm upgrade --install notification ./deploy --set image.tag=$Version
