# Artifact Processor Complex

The APC is used to collect and store a varied and large amount of artifacts, for example npm, nuget...

Primary use-case is transferring to airgapped / high security networks for offline consumption via a artifact repository.

The complex keeps track of already collected artifacts, dependencies, and versions.

The APC's collection of artifacts is greedy, it will per default take every version and every dependency version, recursively, 
until every item is collected, artifacts can include a filter to reduce the amount collected.

It builds daily deltas to reduce the day-to-day load on the transfer device.

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
| APC_API_HOST       | localhost | GUI, ATM, API         |
| APC_API_PORT       | 4000      | GUI, ATM, API         |
| APC_ACM_DIR        | /data/    | ACM                   |
