using BancoPopular.Servicios.Servicio;
using BancoPopular.Solicitudes.BantotalServices.BTS;
using MicroservicioProductosCorresponsal.APP.CuentasAPP;
using MicroservicioProductosCorresponsal.DAA.CuentasDA;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI de tu app
builder.Services.AddScoped<ICuentasAPP, CuentasAPP>();
builder.Services.AddScoped<ICuentasDA, CuentasDA>();

// Dependencias externas (usa las implementaciones reales de tu solución)
builder.Services.AddScoped<IBantotalServices, BantotalServices>();
builder.Services.AddScoped<IServicio, Servicio>();

var app = builder.Build();

// Para pruebas, lo dejamos siempre habilitado.
// Si prefieres, luego lo vuelves a limitar a Development.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroservicioProductosCorresponsal API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();