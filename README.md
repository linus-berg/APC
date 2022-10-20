# Artifact Processor Complex

Used to collect multiple package manager artifacts.

## Modules
| Tag | Name                        | Description                                                                   |
|-----|-----------------------------|-------------------------------------------------------------------------------|
| APC | Artifact Processing Complex | The name of the entire suite.                                                 |
| ATM | Artifact Tracking Module    | The ATM handles the tracking of artifacts to request an update by the APC.    |
| APM | Artifact Processing Module  | The APM handles the processing of a defined artifact-type.                    |
| ACM | Artifact Collector Module   | The ACM handles the collection of artifact-types based on standard protocols. |

## Environment Variables
| Name               | Default   | Modules               |
|--------------------|-----------|-----------------------|
| APC_RABBIT_MQ_HOST | localhost | Ingest, API, APM, ACM |
| APC_RABBIT_MQ_USER | guest     | Ingest, API, APM, ACM |
| APC_RABBIT_MQ_PASS | guest     | Ingest, API, APM, ACM |
| APC_REDIS_HOST     | localhost | Ingest, API           |
| APC_REDIS_USER     | -         | Ingest, API           |
| APC_REDIS_PASS     | -         | Ingest, API           |
| APC_PGSQL_STR      | -         | Ingest, API           |
| APC_API_HOST       | localhost | GUI, ATM, API         |
| APC_API_PORT       | 4000      | GUI, ATM, API         |
| APC_ACM_DIR        | /data/    | ACM                   |
