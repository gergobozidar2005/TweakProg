@echo off
echo Starting TweakProg Development Environment...
echo.

echo [1/3] Starting Backend API...
start "TweakProg Backend" cmd /k "cd TweakManagerBackend && dotnet run"

echo [2/3] Waiting for backend to start...
timeout /t 5 /nobreak > nul

echo [3/3] Starting Client Application...
start "TweakProg Client" cmd /k "cd TweakAppClient && dotnet run"

echo.
echo Development environment started!
echo - Backend API: https://localhost:7223
echo - Admin Dashboard: https://localhost:7223/Admin
echo - Client Application: Running in separate window
echo.
echo Press any key to exit...
pause > nul