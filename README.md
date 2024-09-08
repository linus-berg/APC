# Artifact Processor Complex
:warning: Warning: This project is still under heavy development, to be considered a beta, at best.

The APC is used to collect and store a varied and large amounts of artifacts, for example npm, nuget...

Primary use-case is transferring to airgapped / high security networks for offline consumption via an artifact repository.

The complex keeps track of already collected artifacts, dependencies, and versions.

The APC's collection of artifacts is greedy, it will by default collect every version and every dependency version, recursively, 
until every item is collected, artifact filters can be applied to reduce the amount collected.

It builds daily deltas to reduce the day-to-day load on the transfer device.

The design is microservice oriented and intended to minimize the amount of work needed to build a new "Processor", for example maven.

Processors can safely rely on the core infrastructure of the APC, the core infrastructure handles the memorization of metadata, 
and the collection via standard protocols, for example http, git, or docker.
## Deploy
An early stage helm chart is available at
https://github.com/linus-berg/helm-charts/tree/main/charts/apc

https://linus-berg.github.io/helm-charts/

See the `values.examples.yaml` file for configuration.
The auxiliary services such as mongodb still needs to be provided by the administrator.

## Recommended minimum requirements (single node)
| Resource | Req                              |
|----------|----------------------------------|
| RAM      | 16GB                             |
| CPU      | 8 cores                          |
| Storage  | 2TB * APM (depending on usage)   |
| Network  | >= 1 Gbps                        |

The APC is designed to be horizontally scalable and suitable for kubernetes, however, kubernetes deployments have not been tested thoroughly.

The following services are required to run the complete suite of APC capabilities.
A batteries included starter pack is included under /examples and /Compose, however, these are not production ready configurations, and are only intended as an development/test environment.


| Service                 | Requirement                            |
|-------------------------|--------------------------------------|
| Keycloak                | Required (only for API and GUI)        |
| RabbitMq                | Required                               |
| Minio                   | Required                               |
| MongoDb                 | Required                               |
| Redis                   | Required                               |
| Container Registry      | Required (only for ACM.Container)      |
| OpenTelemetry Collector | Recommended but required for Telemetry | 
| Grafana                 | Recommended                            | 
| Prometheus              | Recommended                            | 
| Tempo                   | Recommended                            | 


## Modules
| Tag | Name                        | Description                                                                   |
|-----|-----------------------------|-------------------------------------------------------------------------------|
| APC | Artifact Processing Complex | The name of the entire suite.                                                 |
| ATM | Artifact Tracking Module    | The ATM handles the tracking of artifacts to request an update by the APC.    |
| APM | Artifact Processing Module  | The APM handles the processing of a defined artifact-type.                    |
| ACM | Artifact Collector Module   | The ACM handles the collection of artifact-types based on standard protocols. |

## Environment Variables
| Name                   | Default   | Modules               |
|------------------------|-----------|-----------------------|
| APC_OTEL_HOST          | -         | Ingest, API, APM, ACM |
| APC_RABBIT_MQ_HOST     | localhost | Ingest, API, APM, ACM |
| APC_RABBIT_MQ_USER     | guest     | Ingest, API, APM, ACM |
| APC_RABBIT_MQ_PASS     | guest     | Ingest, API, APM, ACM |
| APC_REDIS_HOST         | localhost | Ingest, API           |
| APC_REDIS_USER         | -         | Ingest, API           |
| APC_REDIS_PASS         | -         | Ingest, API           |
| APC_MONGO_STR          | -         | Ingest, API           |
| APC_API_HOST           | localhost | GUI, ATM, API         |
| APC_API_PORT           | 4000      | GUI, ATM, API         |
| APC_ACM_DIR            | /data/    | ACM.Git               |
| ACM_HTTP_DELTA         | true      | ACM.Http              |
| ACM_HTTP_MODE          | lake      | ACM.Http              |
| ACM_S3_ACCESS_KEY      | -         | ACM                   |
| ACM_S3_SECRET_KEY      | -         | ACM                   |
| ACM_S3_REGION          | -         | ACM                   |
| ACM_S3_ENDPOINT        | -         | ACM                   |
| ACM_S3_BUCKET          | -         | ACM                   |
| ACM_CONTAINER_REGISTRY | -         | ACM.Container         |
