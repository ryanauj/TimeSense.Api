provider "aws" {
  region = var.region
}

# For the zipped lambda function
provider "archive" {}

module "cognito_pools" {
  source = "git::https://github.com/ryanauj/cognito-pools.git?ref=0.0.10"
  name = "${var.app_name}-${var.environment}"
  username_attributes = ["email"]
}

module "lambda_api" {
  source = "git::https://github.com/ryanauj/lambda-api.git?ref=0.0.36"
  name = "${var.app_name}-${var.environment}"
  environment = var.environment
  lambda_filename = var.lambda_filename
  lambda_handler = var.lambda_handler
  lambda_runtime = var.lambda_runtime
  public_routes = var.public_routes
  
  integration_request_parameters = {
    "integration.request.header.X-Cognito-Id" = "context.identity.cognitoIdentityId"
  }
  
  
  
  variables = {
    ASPNETCORE_ENVIRONMENT = var.environment
    SensedTimesConfiguration__ConnectionString = var.time_sense_connection_string
    SensedTimesConfiguration__CollectionName = "SensedTimes"
    SensedTimesConfiguration__DatabaseName = "TimeSense"
  }
  
  provider_arns = [module.cognito_pools.user_pool_arn]
}

module "sensed_time_dynamodb_table" {
  source = "./modules/sensed_time_dynamodb_table"
  environment = var.environment
  role = module.lambda_api.lambda_iam_role
}