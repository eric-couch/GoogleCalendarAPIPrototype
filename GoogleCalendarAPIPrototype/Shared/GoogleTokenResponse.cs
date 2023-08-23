using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalendarAPIPrototype.Shared;

public class GoogleTokenResponse
{
    public string access_type
    {
        get;
        set;
    }

    public long expires_in
    {
        get;
        set;
    }

    public string refresh_token
    {
        get;
        set;
    }

    public string scope
    {
        get;
        set;
    }

    public string token_type
    {
        get;
        set;
    }
}