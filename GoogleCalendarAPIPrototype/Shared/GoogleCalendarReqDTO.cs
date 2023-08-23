using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalendarAPIPrototype.Shared;

public class GoogleCalendarReqDTO
{
    public string Summary
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    public DateTime StartTime
    {
        get;
        set;
    }

    public DateTime EndTime
    {
        get;
        set;
    }

    public string CalendarId
    {
        get;
        set;
    }

    public string refreshToken
    {
        get;
        set;
    }
}