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

output "template_table_table_arn" {
  value = module.template_dynamodb_table.table_arn
}

output "template_table_policy" {
  value = module.template_dynamodb_table.policy
}

output "entry_table_table_arn" {
  value = module.entry_dynamodb_table.table_arn
}

output "entry_table_policy" {
  value = module.entry_dynamodb_table.policy
}