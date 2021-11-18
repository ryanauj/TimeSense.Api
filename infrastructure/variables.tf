variable "region" {
  type = string
  default = "us-east-2"
}

variable "account_id" {
  type = string
  default = "048603927394"
}

variable "environment" {
  type = string
  default = "test"
}

variable "app_name"{
  type = string
  default = "test-app"
  description = "The name of the app, which will be used in the name set for created resources. Please use hyphens (-) to delineate spaces, like 'test-app'."
}

variable "lambda_handler" {
  type = string
  default = "DotNetLambda::DotNetLambda.LambdaEntryPoint::FunctionHandlerAsync"
  description = "The name of the lambda handler function - this is the syntax for a dotnet handler using the AWS lambda server package."
}

variable "lambda_runtime" {
  type = string
  default = "dotnetcore3.1"
  description = "The runtime environment for the lambda."
}

variable "lambda_filename" {
  type = string
  default = "lambda.zip"
  description = "The name of the packaged lambda function."
}

variable "public_routes" {
  type = map
  default = {}
  description = "An object containing all of the public routes and the corresponding HTTP method. This should be setup with the { 'route': { 'http_method': http_method_value } } schema."
}

variable "time_sense_connection_string" {
  type = string
  description = "The connection string for the mongodb TimeSense database."
}