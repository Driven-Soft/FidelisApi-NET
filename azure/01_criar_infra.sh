#!/bin/bash
set -e

RESOURCE_GROUP=$1
LOCATION=$2
APP_SERVICE_PLAN=$3
WEBAPP_NAME=$4

echo "=== Criando infraestrutura Fidelis ==="

echo "Resource Group: $RESOURCE_GROUP"
echo "Location: $LOCATION"
echo "App Service Plan: $APP_SERVICE_PLAN"
echo "Web App: $WEBAPP_NAME"

# Resource Group
if ! az group show --name "$RESOURCE_GROUP" >/dev/null 2>&1; then
    echo "Criando Resource Group..."
    az group create \
        --name "$RESOURCE_GROUP" \
        --location "$LOCATION"
else
    echo "Resource Group já existe."
fi

# App Service Plan - Windows
if ! az appservice plan show \
    --name "$APP_SERVICE_PLAN" \
    --resource-group "$RESOURCE_GROUP" >/dev/null 2>&1; then

    echo "Criando App Service Plan..."
    az appservice plan create \
        --name "$APP_SERVICE_PLAN" \
        --resource-group "$RESOURCE_GROUP" \
        --location "$LOCATION" \
        --sku F1
else
    echo "App Service Plan já existe."
fi

# Web App
if ! az webapp show \
    --name "$WEBAPP_NAME" \
    --resource-group "$RESOURCE_GROUP" >/dev/null 2>&1; then

    echo "Criando Web App..."
    az webapp create \
        --name "$WEBAPP_NAME" \
        --resource-group "$RESOURCE_GROUP" \
        --plan "$APP_SERVICE_PLAN" \
        --runtime "DOTNETCORE:10.0"
else
    echo "Web App já existe."
fi

echo "=== Infraestrutura pronta ==="

az webapp show \
    --name "$WEBAPP_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --query "{nome:name,url:defaultHostName,state:state}" \
    --output table