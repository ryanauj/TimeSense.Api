output "table_arn" {
  value = aws_dynamodb_table.entry_table.arn
}

output "policy" {
  value = aws_iam_policy.dynamo_table.policy
}