A crude example of a docker compose running a complete infrastructure.

## Recommendations
Split vars.env into multiple files where only each necessary var is mounted into each container.

Provide production configured instances of HAProxy, Redis, PGSQL, Keycloak, RabbitMQ, MongoDB, and the entire telemetry suite.

Overall there is still work to be done on each APC service, however, this will provide you with a "rough" outline of how to run the APC services.

Configuration of individual services (ex. HAProxy) can be provided upon request.