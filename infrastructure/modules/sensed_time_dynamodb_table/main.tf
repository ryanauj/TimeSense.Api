resource "aws_dynamodb_table" "sensed_time_table" {
  name = "sensed-time-table-${var.environment}"
  billing_mode = "PAY_PER_REQUEST"
  hash_key = "UserId"
  range_key = "Id"
  
  attribute {
    name = "UserId"
    type = "S"
  }
  
  # Should change this to CreatedAt or Modified at and use a GSI for Id for lookups
  # since we are going to be mainly returning the most recent requests.
  # Could also just make a GSI with CreatedAt or ModifiedAt as the range key,
  # although would have to copy over most of the fields.
  attribute {
    name = "Id"
    type = "S"
  }
}

resource "aws_iam_policy" "dynamo_table" {
  name        = "sensed-time-table-policy"
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
      "Resource": "${aws_dynamodb_table.sensed_time_table.arn}",
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
