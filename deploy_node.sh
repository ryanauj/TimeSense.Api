#!/bin/bash
#Assuming in solution root dir and that the following tools are installed
#dotnet, zip, terraform, aws

INFRASTRUCTURE_DIR=infrastructure
PATH_TO_ZIP=node_code/index.js
ZIP_FILE_NAME=lambda
HANDLER_FUNCTION=index.handler

#Create zip file in solution root dir
echo "Checking for existing $ZIP_FILE_NAME.zip file."
if test -f "$ZIP_FILE_NAME.zip"; then
  echo "Removing existing $ZIP_FILE_NAME.zip file."
  rm $ZIP_FILE_NAME.zip
fi

echo "Creating new $ZIP_FILE_NAME.zip file."
zip -rX $PATH_TO_ZIP
popd

#Initialize terraform modules
echo "Initializing new terraform modules."
pushd $INFRASTRUCTURE_DIR
terraform init
popd

#Deploy terraform
echo "Deploying service."
pushd $INFRASTRUCTURE_DIR
terraform apply -auto-approve \
  -var="lambda_filename=\"../$PATH_TO_ZIP.zip\"" \
  -var="lambda_handler=\"$HANDLER_FUNCTION\""
popd
