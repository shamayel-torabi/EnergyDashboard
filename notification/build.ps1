$appVersion ="8.0.0"
$appName = "notification"
$publish = ".\publish"

if(Test-Path $publish) { Remove-Item $publish -Force -Recurse }

dotnet restore -r linux-x64
#dotnet publish -c Release -o $publish -r linux-x64 --self-contained true  -p:PublishTrimmed=true --no-restore
#dotnet publish -c Release -o $publish -r linux-x64 --self-contained true  --no-restore
dotnet publish -c Release -o .\publish -r linux-x64 -p:PublishTrimmed=true --no-restore

docker build -t registry.hrec.local/e-dashboard/${appName}:${appVersion} .
# docker login registry.hrec.local
# docker push registry.hrec.local/e-dashboard/${appName}:${appVersion}

# helm uninstall $appName
# helm upgrade --install $appName ./deploy --set image.tag=$appVersion
