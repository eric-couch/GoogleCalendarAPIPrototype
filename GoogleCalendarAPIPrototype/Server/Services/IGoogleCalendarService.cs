using GoogleCalendarAPIPrototype.Shared;

namespace GoogleCalendarAPIPrototype.Server.Services;

public interface IGoogleCalendarService
{
    string GetAuthCode();

    Task<GoogleTokenResponse> GetTokens(string code);
    string AddToGoogleCalendar(GoogleCalendarReqDTO googleCalendarReqDTO);
}
