using System.Text.Json;

namespace AMESWEB.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}

// Usage in Login Action (would be in your Controller):
/*
public class AccountController : Controller
{
    public IActionResult Login()
    {
        var permissions = new Dictionary<string, bool>
        {
            {"IsView", true},
            {"IsEdit", user.HasEditPermission},
            // etc
        };

        HttpContext.Session.SetObject("Permissions", permissions);

        return View();
    }
}
*/