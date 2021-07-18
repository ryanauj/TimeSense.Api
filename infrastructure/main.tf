provider "aws" {
  region = var.region
}

# For the zipped lambda function
provider "archive" {}

module "cognito_pools" {
  source = "git::https://github.com/ryanauj/cognito-pools.git?ref=0.0.2"
  name = "${var.app_name}-${var.environment}"
}

module "lambda_api" {
  source = "git::https://github.com/ryanauj/lambda-api.git?ref=0.0.17"
  name = "${var.app_name}-${var.environment}"
  environment = var.environment
  lambda_filename = var.lambda_filename
  lambda_handler = var.lambda_handler
  lambda_runtime = var.lambda_runtime
  public_routes = var.public_routes
  
  variables = {
    ASPNETCORE_ENVIRONMENT = var.environment
  }
}

module "sensed_time_dynamodb_table" {
  source = "./modules/sensed_time_dynamodb_table"
  environment = var.environment
  role = module.lambda_api.lambda_iam_role
}
