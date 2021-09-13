resource "aws_dynamodb_table" "sensed_time_metrics_table" {
  name = "sensed-time-metrics-table-${var.environment}"
  billing_mode = "PAY_PER_REQUEST"
  hash_key = "UserId"

  attribute {
    name = "UserId"
    type = "S"
  }
}

resource "aws_iam_policy" "dynamo_table" {
  name        = "sensed-time-metrics-table-policy"
  policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": [
        "dynamodb:List*",
        "dynamodb:DescribeReservedCapacity*",
        "dynamodb:DescribeLimits",
        "dynamodb:DescribeTimeToLive",
        "dynamodb:BatchGet*",
        "dynamodb:DescribeStream",
        "dynamodb:DescribeTable",
        "dynamodb:Get*",
        "dynamodb:Query",
        "dynamodb:Scan",
        "dynamodb:BatchWrite*",
        "dynamodb:CreateTable",
        "dynamodb:Delete*",
        "dynamodb:Update*",
        "dynamodb:PutItem"
      ],
      "Resource": "${aws_dynamodb_table.sensed_time_metrics_table.arn}",
      "Effect": "Allow"
    }
  ]
}
EOF
}

resource "aws_iam_role_policy_attachment" "dynamo_table" {
  role = var.role
  policy_arn = aws_iam_policy.dynamo_table.arn
}
