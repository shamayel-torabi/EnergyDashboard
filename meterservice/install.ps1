
$Version="7.0.9"
docker build -t registry.hrec.local/e-dashboard/meterservice:$Version .
docker login registry.hrec.local
docker push registry.hrec.local/e-dashboard/meterservice:$Version

helm uninstall meterservice
helm upgrade --install meterservice ./deploy --set image.tag=$Version