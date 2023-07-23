using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var azureIdPIdentifier = configuration.GetValue<string>("Saml:IDPEntityId");
var entityID = configuration.GetValue<string>("Saml:SPEntityId");
var metadataUrl = configuration.GetValue<string>("Saml:MetadataUrl");
var CertificateFileName = configuration.GetValue<string>("Saml:CertificateFileName");

builder.Services.AddAuthentication(opt =>
{
    // Default scheme that maintains session is cookies.
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    // If there's a challenge to sign in, use the Saml2 scheme.
    opt.DefaultChallengeScheme = Saml2Defaults.Scheme;
})
    .AddCookie()
    .AddSaml2(opt =>
    {
        //Set up our EntityId, this is our application.
        opt.SPOptions.EntityId = new EntityId(entityID);

        // Single logout messages should be signed according to the SAML2 standard, so we need
        // to add a certificate for our app to sign logout messages with to enable logout functionality.
        opt.SPOptions.ServiceCertificates.Add(new X509Certificate2(CertificateFileName));

        // Add an identity provider.
        opt.IdentityProviders.Add(new IdentityProvider(
            // The identityprovider's entity id.
            new EntityId(azureIdPIdentifier),
            opt.SPOptions)
        {
            //App Federation Metadata Url
            MetadataLocation = metadataUrl,
            // Load config parameters from metadata, using the Entity Id as the metadata address.
            LoadMetadata = true
        });        
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.MapRazorPages();

app.Run();
