using EtosApiServices.Model;
using EtosApiServices.Model.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class EHotelBookingRequest
    {
        public EHotelBookingRequest()
        {
        }
        public EHotelBookingRequest(int ClientID_)
        {
            this.ClientID = ClientID_;
        }
        public int ClientID { get; set; }
        public int BookingID { get; set; }
        public string HRDID { get; set; }
        public int RoomIndex { get; set; }
        public List<RoomWisePaxDetails> RoomWisePaxInfo { get; set; }
        public string SpecialRequest { get; set; }
        public string NationalityCode { get; set; }
        public string ApiKey { get; set; }


        [SwaggerExclude]
        public int PlayerId { get; set; }

        [SwaggerExclude]
        public string? DBConnectioString { get; set; }
    }

    public class RoomWisePaxDetails
    {
        public RoomWisePaxDetails()
        {

        }

        public RoomWisePaxDetails(bool IsMain, int paxcount, int Age)
        {
            this.PaxType = BookingPaxType.Adult;
            this.RoomNo = 1;
            this.Priority = IsMain;
            this.Gender = 1;
            this.Title = BookingPaxTitle.Mr;
            this.FirstName = "FTest" + paxcount;
            this.LastName = "LTest" + paxcount;
            this.MiddleName = "MTest" + paxcount;
            this.DOB = DateTime.Now.AddDays(-Age).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            this.DateofExpiry = DateTime.Now.AddDays(15).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            this.Email = "Bhupendra@puratech.in";
            this.MobileCode = "91";
            this.MobileNo = "987654321";
            this.Age = Age;
        }
        public BookingPaxType PaxType { get; set; }//Enum
        public int RoomNo { get; set; }
        public bool Priority { get; set; }
        public int Gender { get; set; }
        public BookingPaxTitle Title { get; set; }//Enum
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string PassportNo { get; set; }
        public string DateofExpiry { get; set; }
        public string PlaceofIssue { get; set; }
        public string Email { get; set; }
        public string MobileCode { get; set; }
        public string MobileNo { get; set; }
        public int Age { get; set; }
    }

    public class EHotelBookingResponse
    {
        public EHotelBookingResponse(EHotelBookingRequest Req_)
        {
            this.HotelBookingRequest = Req_;
        }

        public EHotelBookingRequest HotelBookingRequest { get; set; }
        public string EBtoken { get; set; }
        public string ConfirmationNo { get; set; }

        public string SubConfirmationNo { get; set; }
        public decimal BookingCost { get; set; }
        public B2BEnum.BookingStatus BookingStatus { get; set; }
        public Dictionary<string, string> RequestLog { get; set; }
        public Dictionary<string, string> ResponseLog { get; set; }
        public string SupplierInfo { get; set; }

    }
}
