using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApi;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(opt => opt.AddPolicy("Policy", policy =>
{
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));
builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("Policy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
 {
     options.SerializeAsV2 = true;
 });
    app.UseSwaggerUI(options =>
  {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
      options.RoutePrefix = string.Empty;
  });
}
app.MapGet("/items", async (ToDoDbContext db) =>
    await db.Items.ToListAsync());
app.MapPost("/items", async (ToDoDbContext db, [FromBody] Item item) =>
{
    var item1 = new Item() { Name = item.Name, IsComplete = item.IsComplete };
    db.Items.Add(item1); await db.SaveChangesAsync(); return item1;
});
app.MapPut("/items", async (ToDoDbContext db, [FromBody] Item item) =>
{
    var item1 = await db.Items.FindAsync(item.Id);
    if (item1 == null)
        return Results.NotFound();
    item1.IsComplete = item.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok();
});
app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    if (item == null)
        return Results.NotFound();
    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();

});
app.Run();
