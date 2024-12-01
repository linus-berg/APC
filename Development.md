# Development environment
### Docker compose
Run the external services in the Compose folder with
`docker compose up`.

This will setup the required external services required to run the APC framework.

### Keycloak (required for GUI & API)
Create a client in keycloak named `apc` with the following config.
```
Valid redirect URIs = *
Valid post logout redirect URIs = *
Web origins = *
```
Create a role for the client called `Administrator`.

Export the configuration and place it in the root of `APC.Api`.

The configuration should look something like this:
```json
{
  "realm": "master",
  "auth-server-url": "http://localhost:8090",
  "ssl-required": "external",
  "resource": "apc",
  "public-client": true,
  "verify-token-audience": false,
  "use-resource-role-mappings": true,
  "confidential-port": 0
}
```

### Minio (required for ACM development)
Create a bucket named `apc`.

Create an access key and secret key with the credentials `minio-apc` in both fields.