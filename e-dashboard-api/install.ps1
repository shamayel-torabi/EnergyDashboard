$appVersion ="7.0.9"
$appName = "e-dashboard-api"
$publish = ".\publish"

docker build -t registry.hrec.local/e-dashboard/${appName}:${appVersion} .
docker login registry.hrec.local
docker push registry.hrec.local/e-dashboard/${appName}:${appVersion}

helm uninstall $appName
helm upgrade --install $appName ./deploy --set image.tag=$appVersion