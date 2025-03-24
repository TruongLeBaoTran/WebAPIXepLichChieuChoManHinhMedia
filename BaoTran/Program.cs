using BaoTran.Data;
using BaoTran.Mappers;
using BaoTran.Repository;
using BaoTran.Services;
using BaoTran.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace BaoTran
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            //Đăng ký appsetting database
            IConfigurationRoot cf = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            builder.Services.AddDbContext<MyDbContext>(opt => opt.UseSqlServer(cf.GetConnectionString("MyDatabase")));

            //Đăng ký Repository
            builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

            //Đăng ký Service
            builder.Services.AddScoped<IMediaFileService, MediaFileService>();
            builder.Services.AddScoped<IPlaySchedualService, PlaySchedualService>();

            //Đăng ký AutoMap
            builder.Services.AddAutoMapper(typeof(Mapping));

            //Đăng ký Fluent Validator
            builder.Services.AddControllers().AddFluentValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<MediaFileValidator>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
