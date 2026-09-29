using Microsoft.EntityFrameworkCore;
using InventoryAPI.Models;

namespace InventoryAPI.Data;

public class InventoryContext : DbContext
{
    public InventoryContext(DbContextOptions<InventoryContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}