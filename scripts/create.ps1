$servicePath = Join-Path $PSScriptRoot "bin\Release\net8.0\EmailToTelegram.exe"
sc.exe create "EmailToTelegram" binPath= $servicePath start=auto obj="NT AUTHORITY\LocalService"