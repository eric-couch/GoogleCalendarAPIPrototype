﻿using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using GoogleCalendarAPIPrototype.Shared;
using GoogleCalendarAPIPrototype.Server.Common;
using System.Text;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Configuration;

namespace GoogleCalendarAPIPrototype.Server.Services;

public class GoogleCalendarService : IGoogleCalendarService
{

    private readonly HttpClient _httpClient;

    public GoogleCalendarService()
    {
        _httpClient = new HttpClient();
    }

    public string GetAuthCode()
    {
        try
        {

            string scopeURL1 = "https://accounts.google.com/o/oauth2/auth?redirect_uri={0}&prompt={1}&response_type={2}&client_id={3}&scope={4}&access_type={5}";
            var redirectURL = "https://localhost:7272/auth/callback";
            string prompt = "consent";
            string response_type = "code";
            string clientId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["GoogleClientId"]!;
            string clientSecret = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["GoogleClientSecret"]!;
            string scope = "https://www.googleapis.com/auth/calendar";
            string access_type = "offline";
            string redirect_uri_encode = Method.urlEncodeForGoogle(redirectURL);
            var mainURL = string.Format(scopeURL1, redirect_uri_encode, prompt, response_type, clientId, scope, access_type);

            return mainURL;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    public async Task<GoogleTokenResponse> GetTokens(string code)
    {

        string clientId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["GoogleClientId"]!;
        string clientSecret = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["GoogleClientSecret"]!;
        var redirectURL = "https://localhost:7274/auth/callback";
        var tokenEndpoint = "https://accounts.google.com/o/oauth2/token";
        var content = new StringContent($"code={code}&redirect_uri={Uri.EscapeDataString(redirectURL)}&client_id={clientId}&client_secret={clientSecret}&grant_type=authorization_code", Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await _httpClient.PostAsync(tokenEndpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleTokenResponse>(responseContent);
            return tokenResponse;
        }
        else
        {
            // Handle the error case when authentication fails
            throw new Exception($"Failed to authenticate: {responseContent}");
        }
    }

    public string AddToGoogleCalendar(GoogleCalendarReqDTO googleCalendarReqDTO)
    {
        try
        {
            string clientId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["GoogleClientId"]!;
            string clientSecret = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["GoogleClientSecret"]!;

            var token = new TokenResponse
            {
                RefreshToken = googleCalendarReqDTO.refreshToken
            };
            var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
              new GoogleAuthorizationCodeFlow.Initializer
              {
                  ClientSecrets = new ClientSecrets
                  {
                      ClientId = clientId,
                      ClientSecret = clientSecret
                  }

              }), "user", token);

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
            });

            Event newEvent = new Event()
            {
                Summary = googleCalendarReqDTO.Summary,
                Description = googleCalendarReqDTO.Description,
                Start = new EventDateTime()
                {
                    DateTime = googleCalendarReqDTO.StartTime,
                    //TimeZone = Method.WindowsToIana();    //user's time zone
                },
                End = new EventDateTime()
                {
                    DateTime = googleCalendarReqDTO.EndTime,
                    //TimeZone = Method.WindowsToIana();    //user's time zone
                },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[] {

                new EventReminder() {
                    Method = "email", Minutes = 30
                  },

                  new EventReminder() {
                    Method = "popup", Minutes = 15
                  },

                  new EventReminder() {
                    Method = "popup", Minutes = 1
                  },
              }
                }

            };

            EventsResource.InsertRequest insertRequest = service.Events.Insert(newEvent, googleCalendarReqDTO.CalendarId);
            Event createdEvent = insertRequest.Execute();
            return createdEvent.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }
}
