@echo off
:: EasySave - Open Docker Log Server Port 9000
:: Run this script as Administrator to allow friends to send logs to your Docker server

echo ========================================
echo  EasySave - Opening port 9000 for Docker
echo ========================================
echo.

:: Check for admin rights
net session >nul 2>&1
if %errorLevel% NEQ 0 (
    echo ERROR: Please right-click this file and select "Run as administrator"
    pause
    exit /b 1
)

:: Remove existing rule if present
netsh advfirewall firewall delete rule name="EasySave Docker Log" >nul 2>&1

:: Add new rule
netsh advfirewall firewall add rule name="EasySave Docker Log" dir=in action=allow protocol=TCP localport=9000

if %errorLevel% EQU 0 (
    echo.
    echo SUCCESS! Port 9000 is now open.
    echo Your friend can now connect to your Docker log server.
    echo.
    echo Your local IP: 
    for /f "tokens=2 delims=:" %%a in ('ipconfig ^| findstr /i "IPv4"') do echo    %%a
) else (
    echo ERROR: Failed to open port. Try running as administrator.
)

echo.
pause
