# AzuriteTestContainer

## Build:

```
docker build . --no-cache -t azuritetestcontainer:dev -f AzuriteTestcontainer/Dockerfile
```

## Run/test:
```
docker run -v /var/run/docker.sock:/var/run/docker.sock azuritetestcontainer:dev
```