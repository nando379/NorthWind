namespace NorthWind.Membership.Frontend.RazorViews.AuthenticationStateProviders;

public class JWTAuthenticationStateProvider(ITokenStorage storage) :
    AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsIdentity Identity = new ClaimsIdentity();
        var Tokens = await storage.GetTokensAsync();
        if (Tokens != null)
        {
            var Handler = new JsonWebTokenHandler();
            var Token = Handler.ReadJsonWebToken(Tokens.AccessToken);
            Identity = new ClaimsIdentity(
                Token.Claims,
                nameof(JWTAuthenticationStateProvider));
        }
        return new AuthenticationState(new ClaimsPrincipal(Identity));
    }

    public async ValueTask LoginAsync(TokensDto tokens)
    {
        await storage.StoreTokensAsync(tokens);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async ValueTask LogoutAsync()
    {
        await storage.RemoveTokensAsync();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
