output "user_pool_id" {
  value = module.cognito_pools.user_pool_id
}

output "user_pool_client_id" {
  value = module.cognito_pools.user_pool_client_id
}

output "identity_pool_id" {
  value = module.cognito_pools.identity_pool_id
}

output "invoke_url" {
  value = module.lambda_api.invoke_url
}

output "root_resource_id" {
  value = module.lambda_api.root_resource_id
}

output "proxy_resource_id" {
  value = module.lambda_api.proxy_resource_id
}

output "sensed_time_table_table_arn" {
  value = module.sensed_time_dynamodb_table.table_arn
}

output "sensed_time_table_policy" {
  value = module.sensed_time_dynamodb_table.policy
}
