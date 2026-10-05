using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Model
{
    public class B2BEnum
    {
        public enum BookingStatus
        {
            None = 0,//None
            InProcess = 1,//In process
            BookingSuccessful = 2,//Confirmed
            BookingCancel = 3,//Cancelled
            Pending = 4,//Pending
            Rejected = 5,//Rejected
            Expired = 6,//Expired
            CancellationPending = 7,//Cancellation Pending
            Decline = 8,//Decline
            BookingOnRequest = 10,//On Request
            FlightBookCancel = 11,//Flight Book Cancel
            OnHold = 12,//On-Hold
            Failed = 13,//Failed
            Proceedtopay = 14,//Proceed to pay
            Refund = 15,//Refund
            NoRefund = 16,//No Refund
            PaymentSuccessful = 17,//No Refund
            NeedtoVerify = 18,//Need to Verify
            Paymentnotreceived = 19,//Need to Verify
            CreditLimit = 20,//Need to Verify
            BillPaymentReceived = 21,//Bill Payment Received
            CreditRefund = 22,//Credit Refund
            ErrorInBooking = 23,
            ProceedtoRefund = 24,//ProceedtoRefund
            PaymentAmountMismatch = 25,//Amount Mismatch
            ProceedToPaymentPage = 26,//Amount Mismatch
            OnPaymentVerification = 27,//On Payment Verification
            PaymentVerificationFailed = 28,//Payment Verification Failed
            PaymentVerificationDone = 29,//Payment Verification Done
            RejectBooking = 30,//Reject Booking
            AcceptBooking = 31,//Accept Booking
            PaymentNotRequired = 32,//,
            PaymentPGCancel = 33,//Payment Gateway Cancel
            TentativeBooking = 34,
            BankTransferConfirmationPending = 35,
            PreBookingProcessStart = 36,
            PreBookingProcessComplete = 37,
            NoAvailability = 38,
            PartialCancellation = 39,
            PartialPaymentReceived = 40,
            FullPaymentReceived = 41,
            AnyPaymentReceived = 42,
            PartialConfirmed = 43
        }

        public enum PaymentStatus
        {
            None = 0,
            ConformationPending = 1,
            Successful = 2,
            Fail = 3,
            Error = 4
        }

        public enum PaymentMode
        {
            None = 0,
            Credit = 1,
            Online = 4,
            Cash = 3,
            Paylater = 8,
            AgentCredit = 7,
            BankTransfer = 2,
            On_RequestPaylater = 9,
            WithoutPaymentMode = 10,
            Direct = 11,
            CreditorDebitCard = 12,
            PaySmart = 13
        }
    }
}
