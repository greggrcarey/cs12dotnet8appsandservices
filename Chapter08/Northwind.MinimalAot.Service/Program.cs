using Northwind.Serialization;
using Packt.Extensions; // To use MapGets().


var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, NorthwindJsonSerializerContext.Default);
});

var app = builder.Build();

app.MapGets();

app.Run();

