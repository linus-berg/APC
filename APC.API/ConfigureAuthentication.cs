using APC.Kernel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace APC.API;

public static class ConfigureAuthentication {
  public static IServiceCollection AddOidcAuthentication(
    this IServiceCollection services) {
    services.AddAuthentication(options => {
      options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme =
        OpenIdConnectDefaults.AuthenticationScheme;
    }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
      options.Cookie.Name = "APC_TOKEN";
      options.Cookie.SameSite = SameSiteMode.Lax;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }).AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => {
      options.Authority = Configuration.GetApcVar(ApcVariable.APC_OIDC_HOST);
      options.ClientId =
        Configuration.GetApcVar(ApcVariable.APC_OIDC_CLIENT_ID);
      options.ClientSecret =
        Configuration.GetApcVar(ApcVariable.APC_OIDC_CLIENT_SECRET);

      options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

      options.ResponseType = OpenIdConnectResponseType.Code;

      options.Scope.Clear();
      options.Scope.Add("openid");
      options.Scope.Add("profile");
      options.Scope.Add("email");
      options.Scope.Add("apc_roles");
      options.SaveTokens = true;
      options.GetClaimsFromUserInfoEndpoint = true;
      options.TokenValidationParameters.NameClaimType = "name";
    });

    return services;
  }
}