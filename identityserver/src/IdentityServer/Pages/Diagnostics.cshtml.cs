using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace IdentityServer.Pages
{

    [Authorize]
    public class DiagnosticsModel : PageModel
    {
        public IEnumerable<string>? Clients { get; private set; } = new List<string>();
        public AuthenticateResult? AuthResult { get; private set; }
        public async Task OnGetAsync()
        {
            AuthResult = await HttpContext.AuthenticateAsync();

            if (AuthResult.Succeeded)
            {
                if (AuthResult.Properties.Items.ContainsKey("client_list"))
                {
                    var encoded = AuthResult.Properties.Items["client_list"];
                    var bytes = Base64Url.Decode(encoded);
                    var value = Encoding.UTF8.GetString(bytes);

                    Clients = JsonSerializer.Deserialize<string[]>(value);
                }
            }
        }
    }
}
