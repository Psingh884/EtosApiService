using EtosApiServices.Hotel.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Library
{
    public class MarkUpDetail
    {
    }
    public static class APIMarkupSection
    {
        private static bool IsMarkupPerRoomPerNight = true;
        public static string AP = "AP";
        public static string MK = "MK";
        public static string MK_PV = "MK_PV";//Mark up IsPer Or Value
        public static string MK_V = "MK_V"; //Value
        public static string MKV = "MKV"; //Value
        public static string MT = "MT";
        public static string MT1 = "SRT";
        public static string MT2 = "MT1";
        public static string TXO = "TXO_M";// Tax on Overall Or Markup
        public static string TX_V = "TX_V";//Tax value
        public static string TXV = "TXV";//Tax val
        public static string AR = "AR";
        public static string MKTable = "MKT";
        public static string TaxType = "TaxType";
        public static string TAX_P = "TAX_P";//Taxable percent
        public static string TAX_V = "TAX_V";//Taxable val
        //public static string MKTable = "MKRaw";
        public static string C = "APICurrency";
        public static string C_ID = "APICurrencyID";
        public static string MK_OA = "MK_OA";
        public static string TX_I = "TX_I";//Input Tax val
        public static string mutax = "MunicipalityTax";
        public static string Input_Tax = "INPUT_TAX";
        public static string Output_Tax = "OUTPUT_TAX";

        public static Dictionary<string, object> AddMarkupByRule(int APIID, decimal RawValue, MarkupClass MarkupClassObj, string APICurrency, int NoOfNight, int NoOfRooms)
        {
            Dictionary<string, object> MarkUpBreakUp = new Dictionary<string, object>();
            try
            {
                decimal Markupvalue = 0;
                decimal taxvalue = 0;
                decimal finalvalue = 0;
                MarkUpBreakUp.Add(AP, RawValue);
                MarkUpBreakUp.Add(C, APICurrency);

                if (MarkupClassObj != null)
                {
                    MarkUpBreakUp.Add(MK_PV, MarkupClassObj.IsPercentage);
                    MarkUpBreakUp.Add(MK_V, MarkupClassObj.MVal);
                    MarkUpBreakUp.Add(TXO, MarkupClassObj.TaxType);
                    MarkUpBreakUp.Add(TaxType, MarkupClassObj.TaxType);
                    MarkUpBreakUp.Add(TX_V, MarkupClassObj.TVal);
                    MarkUpBreakUp.Add(MK_OA, MarkupClassObj.IsOverAll);
                    if (MarkupClassObj.MVal > 0)
                    {
                        if (MarkupClassObj.IsPercentage)
                        {
                            Markupvalue = RawValue * MarkupClassObj.MVal / 100;
                        }
                        else
                        {
                            if (IsMarkupPerRoomPerNight)
                            {
                                Markupvalue = MarkupClassObj.MVal * NoOfNight * NoOfRooms;
                            }
                            else
                            {
                                Markupvalue = MarkupClassObj.MVal;
                            }
                        }
                    }
                    if (MarkupClassObj.TVal > 0)
                    {
                        if (MarkupClassObj.TaxType.Equals(1))
                        {
                            taxvalue = (RawValue + Markupvalue) * MarkupClassObj.TVal / 100;
                        }
                        if (MarkupClassObj.TaxType.Equals(2))
                        {
                            taxvalue = (Markupvalue) * MarkupClassObj.TVal / 100;
                        }
                    }
                }
                else
                {
                    MarkUpBreakUp.Add(MK_PV, 0);
                    MarkUpBreakUp.Add(MK_V, 0);
                    MarkUpBreakUp.Add(TXO, 0);
                    MarkUpBreakUp.Add(TX_V, 0);
                    MarkUpBreakUp.Add(TaxType, 0);
                }
                finalvalue = RawValue + Markupvalue + taxvalue;
                MarkUpBreakUp.Add(MKV, Markupvalue);
                MarkUpBreakUp.Add(TXV, taxvalue);
                MarkUpBreakUp.Add(Output_Tax, 0);
                MarkUpBreakUp.Add(MT2, 0);
                
                MarkUpBreakUp.Add(AR, finalvalue);

            }
            catch (Exception ex)
            {
                //Errorlog.InsertLogError(ex, 0);
            }
            return MarkUpBreakUp;
        }

        public static string AddPolicyMarkupByRule(int APIID, decimal RawValue, MarkupClass MarkupClassObj, string APICurrency, int NoOfNight, int NoOfRooms)
        {
            string MarkUpBreakUp = string.Empty;
            try
            {
                decimal Markupvalue = 0;
                decimal taxvalue = 0;
                decimal finalvalue = 0;

                if (MarkupClassObj != null)
                {
                    if (MarkupClassObj.MVal > 0)
                    {
                        if (MarkupClassObj.IsPercentage)
                        {
                            Markupvalue = RawValue * MarkupClassObj.MVal / 100;
                        }
                        else
                        {
                            if (IsMarkupPerRoomPerNight)
                            {
                                Markupvalue = MarkupClassObj.MVal * NoOfNight * NoOfRooms;
                            }
                            else
                            {
                                Markupvalue = MarkupClassObj.MVal;
                            }
                        }
                    }
                    if (MarkupClassObj.TVal > 0)
                    {
                        if (MarkupClassObj.TaxType.Equals(1))
                        {
                            taxvalue = (RawValue + Markupvalue) * MarkupClassObj.TVal / 100;
                        }
                        if (MarkupClassObj.TaxType.Equals(2))
                        {
                            taxvalue = (Markupvalue) * MarkupClassObj.TVal / 100;
                        }
                    }
                }
                else
                {

                }
                finalvalue = RawValue + Markupvalue + taxvalue;
                MarkUpBreakUp = finalvalue.ToString("0.00");

            }
            catch (Exception ex)
            {

            }
            return MarkUpBreakUp;
        }

    }
}
