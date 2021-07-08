app_name = "timesense"
environment = "test"
lambda_filename = "../TimeSense.Api.zip"
lambda_handler = "TimeSense.Api::TimeSense.Api.LambdaEntryPoint::FunctionHandlerAsync"
public_routes = {
    "swagger": {
        http_method="GET"
    }
}
