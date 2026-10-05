using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class RawHotelRoomSHA
    {
        public RawHotelRoomSHA()
        {
            this.CheckInDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            this.CheckOutDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            this.AdultStr = string.Empty;
            this.ChildStr = string.Empty;
            this.RoomCount = string.Empty;
            this.HotelID = string.Empty;
            this.HotelName = string.Empty;
            this.RoomName = string.Empty;
            this.RoomMeal = string.Empty;
            this.IsCancellation = string.Empty;
            this.IsFreeCancellation = string.Empty;
            this.FreeCancellationDate = string.Empty;
            this.IsOffer = string.Empty;
            this.OfferName = string.Empty;
            this.RoomIndex = "0";
        }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public string AdultStr { get; set; }
        public string ChildStr { get; set; }
        public string RoomCount { get; set; }
        public string HotelID { get; set; }
        public string HotelName { get; set; }
        public string RoomName { get; set; }
        public string RoomMeal { get; set; }
        public string IsCancellation { get; set; }
        public string IsFreeCancellation { get; set; }
        public string FreeCancellationDate { get; set; }
        public string IsOffer { get; set; }
        public string OfferName { get; set; }
        public string RoomIndex { get; set; }
        public int IsHotelPackage { get; set; }
        public int PackageId { get; set; }
        public string IsExtraServiceNonMandantory { get; set; }
        public string IsExtraServiceMandantory { get; set; }
        public string GetConcateString()
        {
            string Raw_RoomSHA = string.Empty;
            try
            {
                //CheckInDate!CheckOutDate!AdultStr!ChildStr!RoomCount!HotelID!HotelName!RoomName!RoomMeal!IsCancellation!IsFreeCancellation!FreeCancellationDate
                string strRaw_RoomSHA = string.Format("{0}!{1}!{2}!{3}!{4}!{5}!{6}!{7}!{8}!{9}!{10}!{11}!{12}!{13}!{14}!{15}"
                    , this.CheckInDate
                    , this.CheckInDate
                    , this.AdultStr
                    , this.ChildStr
                    , this.RoomCount
                    , this.HotelID
                    , this.HotelName
                    , this.RoomName
                    , this.RoomMeal
                    , this.IsCancellation
                    , this.IsFreeCancellation
                    , this.FreeCancellationDate
                    , this.IsOffer
                    , this.OfferName
                    , this.RoomIndex
                    , this.IsHotelPackage, this.PackageId
                    );

                Raw_RoomSHA = Hash(strRaw_RoomSHA);
            }
            catch (Exception ex)
            {

            }
            return Raw_RoomSHA;

        }
        private string Hash(string input)
        {
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2")); // Uppercase hex, use "x2" for lowercase
                }

                return sb.ToString();
            }
        }


    }
}
