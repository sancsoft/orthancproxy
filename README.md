# orthancproxy
Proxy server for Orthanc PACS

## Deployment

OrthancProxy is run in a Docker Container on Linux. In our case, it is on the same server as the Orthanc Server.

### Building the dotnet application

Publish the proxy to a single application file using the Linux runtime. 

`dotnet publish -c Release -r linux-x64 /p:PublishSingleFile=true -o ./dist`

### Building the Docker Image

The Dockerfile is intended for use on the hosting server along with the contents of the dist folder, which contains
the published executable.

`docker build -t orthancproxy .`

### Running the Docker Container

Running the container is nothing special. Just pick a port that is available on the host side and tie it to the 
default dotnet HTTTP port.

  `docker container run -d -p 5002:5000 --restart always --name orthancproxy orthancproxy`

### Hosting on Nginx

Use a default ASP dotnet core nginx configuration.

```
server {
    server_name   orthancproxy.sancsoft.net;
    location / {
        proxy_pass         http://localhost:5002;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
    listen 80;
}

```

Certbot can be used to add SSL and configure redirection to HTTPS.
