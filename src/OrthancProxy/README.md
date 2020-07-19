# configuration
You will need to add an appsettings.{Environment}.json file

This settings file sets up Trusted Urls for CORS.

Here is a sample:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "TrustedUrls" : [
    "{your trusted url here}"
  ]
}
```

# requests

The Web API Currently supports GET requests with Basic Auth.

To make a programatic request to this proxy server, you must include BaseUrl in the headers.
Including Authorization, will cause the proxy server to forward that header in the request to the target orhtanc server.

# typescript sample
```
let auth = 'XYZ'; // Your Basic Auth here
let baseUrl = 'https://{target orthanc url here}';
let headers = new HttpHeaders();
headers = headers.append("Authorization", "Basic " + auth);
headers = headers.append("BaseUrl", baseUrl);

let orthancProxyUrl = 'https://localhost:5001/orthanc'; // Your OrhtancProxy url here.
this.http.get(orthancProxyUrl + '/patients?expand', { headers });
```
