# Artifact Processor Complex



## Modules
| Tag | Name                        | Description                                                                   |
|-----|-----------------------------|-------------------------------------------------------------------------------|
| APC | Artifact Processing Complex | The name of the entire suite.                                                 |
| ATM | Artifact Tracking Module    | The ATM handles the tracking of artifacts to request an update by the APC.    |
| APM | Artifact Processing Module  | The APM handles the processing of a defined artifact-type.                    |
| ACP | Artifact Collector Module   | The ACM handles the collection of artifact-types based on standard protocols. |

## Environment Variables
| Name | Default |
|--------------------|-----------|
| APC_RABBIT_MQ_HOST | localhost |
| APC_RABBIT_MQ_USER | guest     |
| APC_RABBIT_MQ_PASS | guest     |
| APC_REDIS_HOST     | localhost |
| APC_REDIS_USER     | -         |
| APC_REDIS_PASS     | -         |
| APC_API_HOST       | -         |
