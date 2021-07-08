#!/bin/bash
#Assuming in solution root dir and that the following tools are installed
#dotnet, zip, terraform, aws

PROJECT_NAME=TimeSense.Api
INFRASTRUCTURE_DIR=infrastructure

#Create zip file in solution root dir
echo "Checking for existing $PROJECT_NAME.zip file."
if test -f "$PROJECT_NAME.zip"; then
  echo "Removing existing $PROJECT_NAME.zip file."
  rm $PROJECT_NAME.zip
fi

dotnet publish /p:GenerateRuntimeConfigurationFiles=true src/$PROJECT_NAME/$PROJECT_NAME.csproj -c Release
pushd src/$PROJECT_NAME/bin/Release/netcoreapp3.1/publish/

echo "Creating new $PROJECT_NAME.zip file."
zip -rX "../../../../../../$PROJECT_NAME.zip" .
popd

#Initialize terraform modules
echo "Initializing new terraform modules."
pushd $INFRASTRUCTURE_DIR
terraform init
popd

#Deploy terraform
echo "Deploying service."
pushd $INFRASTRUCTURE_DIR
terraform apply -auto-approve
popd
