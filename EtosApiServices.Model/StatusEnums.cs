using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Model
{
    public class StatusEnums
    {

    }

    public enum HotelSearchStatus
    {
        successful = 1,
        error = 0
    }
    public enum HotelDetailStatus
    {
        successful = 1,
        error = 0
    }

    public enum BookingStatus
    {
        None = 0,
        ConformationPending = 1,
        BookingSuccessful = 2,
        BookingCancel = 3,
        BookingOnRequest = 4,
        ErrorInBooking = 5,
        ErrorInCancellation = 6,
    }
    public enum Status
    {
        successful = 1,
        error = 0
    }


    public enum BookingPaxType
    {
        Adult = 1,
        Child = 2,
        Infant = 3
    }

    public enum BookingPaxTitle
    {
        Mr = 1,
        Mrs = 2,
        Miss = 3,
        Mstr = 4,
        Master = 5
    }
}
