#update
systemctl stop kestrel-bibliomit.service
sudo rsync -auv /media/sf_E_DRIVE/WebProjects/BiblioMit guillermo@190.13.148.78:/media/guillermo/WD3dNAND-SSD-1TB/
rm -R obj/ bin/ Migrations/
dotnet restore
dotnet ef database drop
dotnet ef migrations add Initial
sqlcmd -S localhost -U SA -P 34erdfERDF -Q 'CREATE DATABASE BiblioMit'
dotnet ef database update
dotnet run
dotnet publish -r linux-x64 -c Release
dotnet publish -r linux-x64 -c Release
sudo rm -R /var/bibliomit/
sudo mkdir /var/bibliomit/
sudo rsync -auv bin/Release/netcoreapp2.1/linux-x64/publish/* /var/bibliomit/
sudo chown -R guillermo /var/bibliomit/
systemctl start kestrel-bibliomit.service

#first
openssl pkcs12 -export -out certificate.pfx -inkey private.key -in certificate.crt -certfile ca_bundle.crt
sudo rsync -auv /media/sf_E_DRIVE/WebProjects/BiblioMit guillermo@190.13.148.78:~/media/guillermo/WD3dNAND-SSD-1TB/
rm -R obj/ bin/ Migrations/ Data/Migrations/
dotnet restore
dotnet ef migrations add Initial
sqlcmd -S localhost -U SA -P 34erdfERDF -Q 'CREATE DATABASE BiblioMit'
dotnet ef database update
dotnet run
dotnet publish -r linux-x64 -c Release
sudo mkdir /var/bibliomit/
sudo rsync -auv bin/Release/netcoreapp2.1/linux-x64/publish/* /var/bibliomit/
sudo chown -R guillermo /var/bibliomit/

#backup database
sqlcmd -S 127.0.0.1 -s "^" -U SA -P 34erdfERDF -d BiblioMit -W -h-1 -k -r1 -Q 'SELECT * FROM dbo.EnsayoFito' | tr '^' '\t' | grep '^[1-9]' | sed "s/NULL//g" > EnsayoFito.tsv
scp 190.13.148.78:~/EnsayoFito.tsv /media/sf_E_DRIVE/WebProjects/BiblioMit/BiblioMit/Data/DIGEST/
sqlcmd -S 127.0.0.1 -s "^" -U SA -P 34erdfERDF -d BiblioMit -W -h-1 -k -r1 -Q 'SELECT * FROM dbo.Phytoplankton' | tr '^' '\t' | grep '^[1-9]' | sed "s/NULL//g" > Phytoplankton.tsv
scp 190.13.148.78:~/Phytoplankton.tsv /media/sf_E_DRIVE/WebProjects/BiblioMit/BiblioMit/Data/DIGEST/

#TEMP PATCH
#https://packages.microsoft.com/ubuntu/16.04/prod/pool/main/m/msodbcsql17/
#https://packages.microsoft.com/ubuntu/16.04/mssql-server-2017/pool/main/m/mssql-server/
#https://packages.microsoft.com/ubuntu/16.04/prod/pool/main/m/mssql-tools/
sudo apt purge msodbcsql17 mssql-server mssql-tools
sudo apt install msodbcsql17=17.3.1.1-1 mssql-server=14.0.3192.2-2 mssql-tools=17.3.0.1-1
sudo /opt/mssql/bin/mssql-conf setup
sudo apt purge aspnetcore-runtime-2.2 dotnet-runtime-2.2 dotnet-hostfxr-2.2 dotnet-host dotnet-runtime-deps-2.2 dotnet-sdk-2.2
sudo apt install aspnetcore-runtime-2.2=2.2.5-1 dotnet-runtime-2.2=2.2.5-1 dotnet-hostfxr-2.2=2.2.5-1 dotnet-host=2.2.5-1 dotnet-runtime-deps-2.2=2.2.5-1 dotnet-sdk-2.2=2.2.300-1

echo <<< EOL
[Unit]
Description=ConsultaMD

[Service]
WorkingDirectory=/media/guillermo/WD3DNAND-SSD-1TB/webapps/consultamd
ExecStart=/usr/bin/dotnet ConsultaMD.dll
Restart=always
RestartSec=10
SyslogIdentifier=dotnet-consultamd
User=guillermo
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOL >> /etc/systemd/system/kestrel-consultamd.service

echo <<< EOL
	upstream elochile{
		server 127.0.0.3:5050
	}

systemctl start kestrel-bibliomit.service