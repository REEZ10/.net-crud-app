using Microsoft.EntityFrameworkCore;
using InventoryAPI.Data;
using InventoryAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseSqlite("Data Source=inventory.db"));

// Allow requests from any frontend (Angular, React, Vue) during development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// GET all products
app.MapGet("/api/products", async (InventoryContext db) =>
    await db.Products.ToListAsync());

// GET single product
app.MapGet("/api/products/{id}", async (int id, InventoryContext db) =>
    await db.Products.FindAsync(id) is Product product
        ? Results.Ok(product)
        : Results.NotFound());

// POST create product
app.MapPost("/api/products", async (Product product, InventoryContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", product);
});

// PUT update product
app.MapPut("/api/products/{id}", async (int id, Product updated, InventoryContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    product.Name = updated.Name;
    product.Category = updated.Category;
    product.Quantity = updated.Quantity;
    product.Price = updated.Price;

    await db.SaveChangesAsync();
    return Results.Ok(product);
});

// DELETE product
app.MapDelete("/api/products/{id}", async (int id, InventoryContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();