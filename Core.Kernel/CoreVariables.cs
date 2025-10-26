namespace Core.Kernel;

public enum CoreVariables {
  // RabbitMQ message bus
  BP_RABBIT_MQ_API,
  BP_RABBIT_MQ_HOST,
  BP_RABBIT_MQ_USER,
  BP_RABBIT_MQ_PASS,

  // Redis cache
  BP_REDIS_HOST,
  BP_REDIS_USER,
  BP_REDIS_PASS,

  // OTEL collector telemetry
  BP_OTEL_HOST,

  BP_API_HOST,

  // LEGACY: Place on disk to store artifacts, only required for Collector.Git
  BP_COLLECTOR_DIRECTORY,

  // MongoDb connection string
  BP_MONGO_STR,

  // S3
  BP_S3_ACCESS_KEY,
  BP_S3_SECRET_KEY,
  BP_S3_REGION,
  BP_S3_ENDPOINT,
  BP_S3_BUCKET,

  // Collector.Http configuration
  BP_COLLECTOR_HTTP_DELTA,
  BP_COLLECTOR_HTTP_MODE,

    
  // Registry proxy for S3 
  BP_COLLECTOR_CONTAINER_REGISTRY
}