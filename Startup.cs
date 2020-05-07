using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fisher.Bookstore.Data;
using Microsoft.EntityFrameworkCore;
using Fisher.Bookstore.Services;

namespace Fisher.Bookstore
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
            services.AddControllers();
            services.AddCors();
            services.AddDbContext<BookstoreContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("BookstoreContext"))
            );
            services.AddScoped<IBooksRepository, BooksRepository>();
            services.AddSingleton<IBooksRepository, TestBooksRepository>();
            //services.AddScoped<IAuthorsRepository, AuthorsRepository>();
            services.AddSingleton<IAuthorsRepository, TestAuthorsRepository>();

            string domain = $"https://{COnfiguration["Auth0:Domain"]}/";

            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChellengeScheme = JqtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["Auth0:ApiIdentifier"];
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:messages", policy => policy.Requirements.Add(new HasScopeRequirement("read:message", domain)));
            });
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(b => b.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseAuthorization();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
