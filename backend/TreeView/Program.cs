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
	    
	    using (var scope = app.Services.CreateScope())
            {
              var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

              var retries = 10;
              while (retries > 0)
              {
                try
                {
                  dbContext.Database.Migrate();
                  break;
                }
                catch
                {
                  retries--;
                  Thread.Sleep(5000);
                }
            }
}
	    
            app.Run();
        }
    }
}
