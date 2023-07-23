using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AspNetCore.API
{
    public class Startup
    {
        public IConfiguration configRoot
        {
            get;
        }
        public Startup(IConfiguration configuration)
        {
            configRoot = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var issuer = configRoot.GetValue<string>("JWT:Issuer");
            var Key = configRoot.GetValue<string>("JWT:Key");
            var expiresInMins = configRoot.GetValue<double>("JWT:ExpireInMinutes");

            var SPEntityId = configRoot.GetValue<string>("Saml:SPEntityId");
            var IDPEntityId = configRoot.GetValue<string>("Saml:IDPEntityId");
            var CertificateFileName = configRoot.GetValue<string>("Saml:CertificateFileName");
            var MetadataUrl = configRoot.GetValue<string>("Saml:MetadataUrl");

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = ApplicationSamlConstants.Application;
                o.DefaultSignInScheme = ApplicationSamlConstants.External;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromMinutes(expiresInMins),
                        ValidIssuer = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key))
                    };
                })
                .AddSaml2(options =>
                {
                    options.SPOptions.EntityId = new EntityId(SPEntityId);
                    options.IdentityProviders.Add(
                        new IdentityProvider(
                            new EntityId(IDPEntityId), options.SPOptions)
                        {
                            LoadMetadata = true,
                            MetadataLocation = MetadataUrl,
                        });

                    options.SPOptions.ServiceCertificates.Add(new X509Certificate2(CertificateFileName));
                });

            services.AddRazorPages();
        }
        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            //app.MapRazorPages();
            app.Run();
        }
    }
}
