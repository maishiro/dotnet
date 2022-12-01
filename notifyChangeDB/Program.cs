using System.Configuration;


var appSettings = ConfigurationManager.AppSettings;
var app = ConfigurationManager.AppSettings["test"];
var dbType = ConfigurationManager.AppSettings["DBType"];
var conString = ConfigurationManager.AppSettings["ConnectionString"];


DbAccess db = DbAccess.CreateDB(dbType, conString);
db.Open();
db.Run();
