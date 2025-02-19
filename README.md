Enpacotar e publicar:
dotnet pack
dotnet nuget push bin/Debug/TechChallenge.Common.SDK.1.0.0.nupkg -k <API_KEY> -s https://api.nuget.org/v3/index.json

Referenciar na aplicação:
dotnet add package SharedLibrary

Configurar a injeção de dependencia:
builder.Services.AddDbContext<MyDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));
builder.Services.AddSingleton<RabbitMqService>(new RabbitMqService("localhost"));
