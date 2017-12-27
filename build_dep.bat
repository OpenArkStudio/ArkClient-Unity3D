@echo off

echo Building dependencies...
cd source
cd Assets
cd _NF

REM ######################################################################################################
echo Building ArkGameClient...

if exist ArkGameClient (rd ArkGameClient /q /s)
git clone https://github.com/ArkGame/ArkGameClient.git

cd ArkGameClient
call build_dep.bat

