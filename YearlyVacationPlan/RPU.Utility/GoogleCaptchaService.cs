using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace RPU.Utility;

public class GoogleCaptchaService
{
    private readonly IOptionsMonitor<GoogleCaptcha> _captcha;

    public GoogleCaptchaService(IOptionsMonitor<GoogleCaptcha> captcha)
    {
        _captcha = captcha;
    }
    public async Task<bool> VerifyToken(string token)
    {
        try
        {
            var url = $"https://www.google.com/recaptcha/api/siteverify?secret={_captcha.CurrentValue.SecretKey}&response={token}";
            using (var client = new HttpClient())
            {
                var httpResult = await client.GetAsync(url);
                if (httpResult.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                var responseString = await httpResult.Content.ReadAsStringAsync();
                var googleResult = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(responseString);

                // the score for this request (0.0 - 1.0)
                return googleResult.success && googleResult.score >= 0.5;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
}
