using JwtAuthenticationExample.Data;
using JwtAuthenticationExample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Dalla proprietà Configuration andiamo a recuperare la Connection String
            // che si chiama Default, cioè quella che abbiamo definito in appsettings.json
            var dbAddress = Configuration.GetConnectionString("Default");

            services.AddDbContext<DataContext>(
                opt => opt.UseMySql(
                    dbAddress,
                    ServerVersion.AutoDetect(dbAddress)
                )
            );

            services.AddScoped<ICharacterService, CharacterService>();
            services.AddScoped<IAuthService, AuthService>();

            // Abilitiamo il controllo dell'autenticazione
            // tramite il token JWT e le sue convenzioni
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    // Conviene esternalizzare la chiave segreta
                    var secret = "Super secret very very long long men pikachu";
                    // Questo secret è importante per validare il token dell'utente
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtAuthenticationExample", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JwtAuthenticationExample v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Dobbiamo mette SOPRA UseAuthorization
            // l'utilizzo dell'authentication!!!!
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
