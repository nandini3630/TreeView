using Microsoft.EntityFrameworkCore;
using TreeView.Data;

namespace TreeView
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://0.0.0.0:7295");
            // Add services to the container.
            builder.Services.AddCors(builder =>
            {
                builder.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            }); 
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            
            app.UseCors("AllowAllOrigins");
            app.UseAuthorization();

            app.MapControllers();
	    
	    // Run database migrations at startup
            using (var scope = app.Services.CreateScope())
            {
              var services = scope.ServiceProvider;
              var dbContext = services.GetRequiredService<ApplicationDbContext>();
              var logger = services.GetRequiredService<ILogger<Program>>();

              var retries = 10;
              while (retries > 0)
              {
                try
                {
                  logger.LogInformation("Attempting database migration...");
                  dbContext.Database.Migrate();
                  logger.LogInformation("Database migration completed successfully.");
                  break;
                }
                catch (Exception ex)
                {
                  retries--;
                  logger.LogError($"Database migration failed (retries left: {retries}). Error: {ex.Message}");
                  if (retries > 0)
                    Thread.Sleep(5000);
                }
              }
              if (retries == 0)
              {
                logger.LogError("Database migration failed after all retries. Application may not function correctly.");
              }
            }
	    
            app.Run();
        }
    }
}
