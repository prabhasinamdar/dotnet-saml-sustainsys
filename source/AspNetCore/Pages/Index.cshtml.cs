using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace AspNetCore.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration _configuration;
    public string? ResponseFromAuthenticatedAPI { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public void OnGet()
    {
        if(HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
        {
            var authResult = HttpContext.AuthenticateAsync().GetAwaiter().GetResult();
            var token = this.CreateJwtSecurityToken(authResult);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            

            //Call the API passing the JWT Token
            ResponseFromAuthenticatedAPI = InvokeApi("GET", "https://localhost:7081/", "api/ValidateApi/UsingSamlToken", "", jwtToken).GetAwaiter().GetResult();
        }
    }

    private JwtSecurityToken CreateJwtSecurityToken(AuthenticateResult authenticateResult)
    {
        var claimsIdentity = new ClaimsIdentity(ApplicationSamlConstants.Application);
        var idpIssuer = _configuration.GetValue<string>("Saml:IDPEntityId");
        var audience = _configuration.GetValue<string>("Saml:IDPEntityId");
        var encryptkey = _configuration.GetValue<string>("Saml:EncryptKey");


        if (authenticateResult != null && authenticateResult.Principal != null) {
            var userClaim = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier);

            if (userClaim != null && !string.IsNullOrEmpty(userClaim.Value))
            {
                claimsIdentity.AddClaim(userClaim);

                var username = userClaim.Value.ToString();
                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, username),
            };
            
            //UPDATE THE key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptkey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //UPDATE THE TENANTID
            return new JwtSecurityToken(
                    idpIssuer,
                    audience,
                    claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials);

            }
            else { return new JwtSecurityToken(); };
        }
        else { return new JwtSecurityToken(); };
        
    }

    private async Task<string> InvokeApi(string action, string apiURL, string apiMethod, string JsonRequestContent, string authToken)
    {
        string _apiResponse = string.Empty;
        double _apiConnectionTimeOut = Convert.ToDouble(60);
        try
        {
            using (var client = new HttpClient())
            {
                client.Timeout = System.TimeSpan.FromSeconds(Convert.ToDouble(_apiConnectionTimeOut));
                client.BaseAddress = new Uri(apiURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authToken);
                StringContent theContent = new StringContent(JsonRequestContent, UnicodeEncoding.UTF8, "application/json");
                HttpResponseMessage response = new HttpResponseMessage();
                switch (action.ToUpper())
                {
                    case "POST":
                        response = await client.PostAsync(apiURL + apiMethod, theContent);
                        break;
                    case "GET":
                        response = await client.GetAsync(apiMethod);
                        break;
                }
                if (response != null && response.Content != null)
                {
                    _apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _apiResponse = "InvokeApi method: failed with message: " + ex.Message.ToString();
        }
        return _apiResponse;
    }
}
