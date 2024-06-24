using Microsoft.OpenApi.Models;
using PizzaStore.DB;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PizzaStore API",
        Description = "Making the Pizzas you love",
        Version = "v1"
    });
});
builder.Services.AddDbContext<PizzaDb>(options => options.UseInMemoryDatabase("items"));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI(c =>
   {
      c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
   });
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/pizzas/{id}", (int id) => PizzaDB.GetPizza(id));
app.MapGet("/pizzas", () => PizzaDB.GetPizzas());
app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());
app.MapPost("/pizza", async (PizzaDb db, PizzaStore.Models.Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});
app.MapPost("/pizzas", (PizzaStore.DB.Pizza pizza) => PizzaDB.CreatePizza(pizza));
app.MapPut("/pizzas", (PizzaStore.DB.Pizza pizza) => PizzaDB.UpdatePizza(pizza));
app.MapDelete("/pizzas/{id}", (int id) => PizzaDB.RemovePizza(id));

app.Run();