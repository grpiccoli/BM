#update
systemctl stop kestrel-bibliomit.service
sudo rsync -auv /media/sf_E_DRIVE/WebProjects/BiblioMit guillermo@190.13.148.78:/media/guillermo/WD3dNAND-SSD-1TB/
rm -R obj/ bin/ Migrations/
dotnet restore
dotnet ef database drop
dotnet ef migrations add Initial
dotnet ef database update
dotnet run
dotnet publish -r linux-x64 -c Release
sudo rm -R /var/bibliomit/
sudo mkdir /var/bibliomit/
sudo rsync -auv bin/Release/netcoreapp2.1/linux-x64/publish/* /var/bibliomit/
sudo chown -R guillermo /var/bibliomit/
systemctl start kestrel-bibliomit.service

#first
sudo rsync -auv /media/sf_E_DRIVE/WebProjects/BiblioMit guillermo@190.13.148.78:~/media/guillermo/WD3dNAND-SSD-1TB/
rm -R obj/ bin/ Migrations/ Data/Migrations/
dotnet restore
dotnet ef migrations add Initial
dotnet ef database update
dotnet run
dotnet publish -r linux-x64 -c Release
sudo mkdir /var/bibliomit/
sudo rsync -auv bin/Release/netcoreapp2.1/linux-x64/publish/* /var/bibliomit/
sudo chown -R guillermo /var/bibliomit/

#service
sudo tee << EOL /etc/systemd/system/kestrel-bibliomit.service >/dev/null
[Unit]
Description=BiblioMit

[Service]
WorkingDirectory=/root/webapps/bibliomit
ExecStart=/usr/bin/dotnet BiblioMit.dll
Restart=always
RestartSec=10
SyslogIdentifier=dotnet-bibliomit
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOL

systemctl daemon-reload

sudo tee << EOL /etc/nginx/sites-available/bibliomit >/dev/null
server {
    listen 80;
    server_name bibliomit.cl www.bibliomit.cl;

    location / {
        proxy_pass              http://localhost:5008;
        proxy_http_version      1.1;
        proxy_set_header        Upgrade \$http_upgrade;
        proxy_set_header        Connection keep-alive;
        proxy_set_header        Host \$host;
        proxy_cache_bypass      \$http_upgrade;
        proxy_set_header        X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header        X-Forwarded-Proto \$scheme;
    }

    location /chatHub {
        proxy_pass              http://localhost:5008;
        proxy_http_version      1.1;
        proxy_set_header        Upgrade \$http_upgrade;
        proxy_set_header        Connection "upgrade";
        proxy_set_header        Host \$host;
        proxy_cache_bypass      \$http_upgrade;
    }

    location /entryHub {
        proxy_pass              http://localhost:5008;
        proxy_http_version      1.1;
        proxy_set_header        Upgrade \$http_upgrade;
        proxy_set_header        Connection "upgrade";
        proxy_set_header        Host \$host;
        proxy_cache_bypass      \$http_upgrade;
    }

    location /progressHub {
        proxy_pass              http://localhost:5008;
        proxy_http_version      1.1;
        proxy_set_header        Upgrade \$http_upgrade;
        proxy_set_header        Connection "upgrade";
        proxy_set_header        Host \$host;
        proxy_cache_bypass      \$http_upgrade;
    }

    if(\$host = bibliomit.cl) {
        return 301 https://www.\$host\$request_uri;
    }
}
EOL

sudo ln -s /etc/nginx/sites-available/bibliomit /etc/nginx/sites-enabled/bibliomit

sudo /usr/sbin/nginx -s reload

#backup database
sqlcmd -S 127.0.0.1 -s "^" -U SA -P 34erdfERDF -d BiblioMit -W -h-1 -k -r1 -Q 'SELECT * FROM dbo.EnsayoFito' | tr '^' '\t' | grep '^[1-9]' | sed "s/NULL//g" > EnsayoFito.tsv
scp 190.13.148.78:~/EnsayoFito.tsv /media/sf_E_DRIVE/WebProjects/BiblioMit/BiblioMit/Data/DIGEST/
sqlcmd -S 127.0.0.1 -s "^" -U SA -P 34erdfERDF -d BiblioMit -W -h-1 -k -r1 -Q 'SELECT * FROM dbo.Phytoplankton' | tr '^' '\t' | grep '^[1-9]' | sed "s/NULL//g" > Phytoplankton.tsv
scp 190.13.148.78:~/Phytoplankton.tsv /media/sf_E_DRIVE/WebProjects/BiblioMit/BiblioMit/Data/DIGEST/
