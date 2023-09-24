namespace APC.Kernel;

public enum ApcVariable {
  // RabbitMQ message bus
  APC_RABBIT_MQ_HOST,
  APC_RABBIT_MQ_USER,
  APC_RABBIT_MQ_PASS,
  
  // Redis cache
  APC_REDIS_HOST,
  APC_REDIS_USER,
  APC_REDIS_PASS,
  
  // OTEL collector telemetry
  APC_OTEL_HOST,
  
  APC_API_HOST,
  
  // LEGACY: Place on disk to store artifacts, only required for ACM.Git
  APC_ACM_DIR,
  
  // MongoDb connection string
  APC_MONGO_STR,
  
  // ACM.Http configuration
  ACM_HTTP_DELTA,
  ACM_HTTP_MODE,
  
  // S3
  ACM_S3_ACCESS_KEY,
  ACM_S3_SECRET_KEY,
  ACM_S3_REGION,
  ACM_S3_ENDPOINT,
  ACM_S3_BUCKET,
  
  // Registry proxy for S3 
  ACM_CONTAINER_REGISTRY
}