using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = builder.Configuration.GetValue<string>("KeyVault:Uri");
builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());

var dbConnection1 = builder.Configuration.GetValue<string>("DemoDb1");
var dbConnection2 = builder.Configuration.GetValue<string>("DemoDb2");

var app = builder.Build();

app.Run();