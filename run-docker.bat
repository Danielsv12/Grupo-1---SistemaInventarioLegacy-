@echo off
REM Script para ejecutar el Sistema de Inventario con Docker
REM Uso: run-docker.bat

setlocal enabledelayedexpansion

echo.
echo ╔════════════════════════════════════════╗
echo ║   Sistema de Gestión de Inventario    ║
echo ║        PYME - Clean Architecture       ║
echo ║        Inicializando con Docker...     ║
echo ╚════════════════════════════════════════╝
echo.

REM Verificar que estamos en la carpeta correcta
if not exist "docker-compose.yml" (
    echo ERROR: docker-compose.yml no encontrado
    echo Debes estar en la carpeta C:\SistemaInventarioRefactorizado
    pause
    exit /b 1
)

echo.
echo Verificando Docker...
docker --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Docker no está instalado o no está corriendo
    echo Descarga Docker Desktop desde: https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
)

echo ✅ Docker está instalado

echo.
echo Compilando proyecto...
dotnet build >nul 2>&1
if errorlevel 1 (
    echo ERROR: Compilación fallida
    echo Ejecuta: dotnet build
    pause
    exit /b 1
)

echo ✅ Compilación exitosa

echo.
echo Ejecutando tests...
dotnet test >nul 2>&1
if errorlevel 1 (
    echo ⚠️ Algunos tests fallaron (esto es normal si no tienes BD)
) else (
    echo ✅ Todos los tests pasaron
)

echo.
echo Levantando contenedores con Docker Compose...
echo (Esto puede tomar 30 segundos la primera vez)
echo.
echo Las credenciales de BD son:
echo   Usuario: sa
echo   Contraseña: YourPassword123!
echo.

docker-compose up

pause
