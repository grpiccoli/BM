sqlcmd -S localhost -d BiblioMit -U SA -P 34erdfERDF -Q "BULK INSERT dbo.EnsayoFitos FROM '/media/guillermo/WD3DNAND-SSD-1TB/BiblioMit/BiblioMit/Data/DIGEST/EntryFito.tsv'"
sqlcmd -S localhost -d BiblioMit -U SA -P 34erdfERDF -Q "BULK INSERT dbo.Groups FROM '/media/guillermo/WD3DNAND-SSD-1TB/BiblioMit/BiblioMit/Data/DIGEST/Groups.tsv'"
sqlcmd -S localhost -d BiblioMit -U SA -P 34erdfERDF -Q "BULK INSERT dbo.Phytoplanktons FROM '/media/guillermo/WD3DNAND-SSD-1TB/BiblioMit/BiblioMit/Data/DIGEST/Phytoplankton.tsv'"
sqlcmd -S localhost -d BiblioMit -U SA -P 34erdfERDF -Q "BULK INSERT dbo.Spawnings FROM '/media/guillermo/WD3DNAND-SSD-1TB/BiblioMit/BiblioMit/Data/SEMAFORO/Spawning.tsv'"
sqlcmd -S localhost -d BiblioMit -U SA -P 34erdfERDF -Q "BULK INSERT dbo.RepStages FROM '/media/guillermo/WD3DNAND-SSD-1TB/BiblioMit/BiblioMit/Data/SEMAFORO/RepStage.tsv'"
sqlcmd -S localhost -d BiblioMit -U SA -P 34erdfERDF -Q "BULK INSERT dbo.Species FROM '/media/guillermo/WD3DNAND-SSD-1TB/BiblioMit/BiblioMit/Data/SEMAFORO/Specie.tsv'"
sqlcmd -S localhost -d BiblioMit -U SA -P 34erdfERDF -Q "BULK INSERT dbo.SpecieSeeds FROM '/media/guillermo/WD3DNAND-SSD-1TB/BiblioMit/BiblioMit/Data/SEMAFORO/SpecieSeed.tsv'"
sqlcmd -S localhost -d BiblioMit -U SA -P 34erdfERDF -Q "BULK INSERT dbo.Seeds FROM '/media/guillermo/WD3DNAND-SSD-1TB/BiblioMit/BiblioMit/Data/SEMAFORO/Seed.tsv'"
sqlcmd -S localhost -d BiblioMit -U SA -P 34erdfERDF -Q "BULK INSERT dbo.Tallas FROM '/media/guillermo/WD3DNAND-SSD-1TB/BiblioMit/BiblioMit/Data/SEMAFORO/Talla.tsv'"

sudo chown mssql:mssql /var/opt/mssql/data/BiblioMit*
sudo chmod 640 /var/opt/mssql/data/BiblioMit*
sqlcmd -S localhost -U sa
CREATE DATABASE [db_name] ON PRIMARY (FILENAME='/var/opt/mssql/data/BiblioMit.mdf') LOG ON (FILENAME='/var/opt/mssql/data/BiblioMit_log.ldf') FOR ATTACH;
GO
exit