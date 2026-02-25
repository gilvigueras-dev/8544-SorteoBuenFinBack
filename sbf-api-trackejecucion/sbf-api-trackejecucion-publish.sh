#!/bin/bash

# Colores para output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # Sin color

# Configuración
RESOURCE_GROUP="rg-sat-flujos-dev"
APP_NAME="sbf-api-trackejecucion"
BUILD_CONFIG="Release"
PUBLISH_DIR="./publish"
ZIP_FILE="./publish/sbf-api-trackejecucion-publish.zip"

echo -e "${BLUE}🚀 Iniciando deployment de $APP_NAME...${NC}"

# Verificar si Azure CLI está instalado
if ! command -v az &> /dev/null; then
    echo -e "${RED}❌ Azure CLI no está instalado. Instálalo con: brew install azure-cli${NC}"
    exit 1
fi

# Verificar si está logueado en Azure
if ! az account show &> /dev/null; then
    echo -e "${YELLOW}⚠️  No estás logueado en Azure. Ejecutando 'az login'...${NC}"
    az login
fi

# Limpiar archivos anteriores
echo -e "${BLUE}🧹 Limpiando archivos anteriores...${NC}"
rm -rf $PUBLISH_DIR
rm -f $ZIP_FILE

# Limpiar y restaurar dependencias
echo -e "${BLUE}📦 Restaurando dependencias...${NC}"
dotnet clean
dotnet restore

# Compilar y publicar
echo -e "${BLUE}🔨 Compilando aplicación...${NC}"
dotnet publish -c $BUILD_CONFIG -o $PUBLISH_DIR

# Verificar que la compilación fue exitosa
if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Error en la compilación. Abortando deployment.${NC}"
    exit 1
fi

# Crear ZIP
echo -e "${BLUE}📁 Creando archivo ZIP...${NC}"
cd $PUBLISH_DIR
zip -r ../$ZIP_FILE . -x "*.pdb" "*.xml"
cd ..

# Verificar que el ZIP se creó correctamente
if [ ! -f $ZIP_FILE ]; then
    echo -e "${RED}❌ Error creando el archivo ZIP. Abortando deployment.${NC}"
    exit 1
fi

echo -e "${BLUE}📊 Tamaño del archivo: $(du -h $ZIP_FILE | cut -f1)${NC}"

# Subir a Azure
echo -e "${BLUE}☁️  Subiendo a Azure App Service...${NC}"
az webapp deploy \
  --resource-group $RESOURCE_GROUP \
  --name $APP_NAME \
  --src-path ./$ZIP_FILE \
  --type zip

# Verificar resultado del deployment
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Deployment completado exitosamente!${NC}"
    echo -e "${GREEN}🌐 URL: https://$APP_NAME-htbchrcmewcndtfh.eastus2-01.azurewebsites.net${NC}"
    
    # Preguntar si quiere abrir la URL
    read -p "¿Quieres abrir la aplicación en el navegador? (y/n): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        open "https://$APP_NAME-htbchrcmewcndtfh.eastus2-01.azurewebsites.net"
    fi
else
    echo -e "${RED}❌ Error durante el deployment.${NC}"
    exit 1
fi

# Limpiar archivos temporales
echo -e "${BLUE}🧹 Limpiando archivos temporales...${NC}"
rm -rf $PUBLISH_DIR
rm -f $ZIP_FILE

echo -e "${GREEN}🎉 Proceso completado!${NC}"