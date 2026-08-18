#!/bin/bash

# Script para ejecutar el Sistema de Inventario con Docker (Linux/Mac)
# Uso: ./run-docker.sh

echo ""
echo "╔════════════════════════════════════════╗"
echo "║   Sistema de Gestión de Inventario    ║"
echo "║        PYME - Clean Architecture       ║"
echo "║        Inicializando con Docker...     ║"
echo "╚════════════════════════════════════════╝"
echo ""

# Verificar que estamos en la carpeta correcta
if [ ! -f "docker-compose.yml" ]; then
    echo "ERROR: docker-compose.yml no encontrado"
    echo "Debes estar en la carpeta del proyecto"
    exit 1
fi

# Verificar .env
if [ ! -f ".env" ]; then
    echo "Creando archivo .env..."
    cat > .env << EOF
SA_PASSWORD=YourPassword123!
ACCEPT_EULA=Y
EOF
    echo "✅ Archivo .env creado"
fi

echo ""
echo "Verificando Docker..."
if ! command -v docker &> /dev/null; then
    echo "ERROR: Docker no está instalado"
    echo "Descarga Docker Desktop desde: https://www.docker.com/products/docker-desktop"
    exit 1
fi

echo "✅ Docker está instalado"

echo ""
echo "Compilando proyecto..."
if ! dotnet build > /dev/null 2>&1; then
    echo "ERROR: Compilación fallida"
    echo "Ejecuta: dotnet build"
    exit 1
fi

echo "✅ Compilación exitosa"

echo ""
echo "Ejecutando tests..."
if ! dotnet test > /dev/null 2>&1; then
    echo "⚠️ Algunos tests fallaron (esto es normal si no tienes BD)"
else
    echo "✅ Todos los tests pasaron"
fi

echo ""
echo "Levantando contenedores con Docker Compose..."
echo "(Esto puede tomar 30 segundos la primera vez)"
echo ""

docker-compose up --env-file .env
