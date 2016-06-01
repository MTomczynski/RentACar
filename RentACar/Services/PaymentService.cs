using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RentACar.Services
{
    public class PaymentService
    {
        public PaymentService DoPayment()
        {
            return null;
        }
        public string DotpayId => System.Configuration.ConfigurationManager.AppSettings["Dotpay_Id"];
        public string DotpayPin => System.Configuration.ConfigurationManager.AppSettings["Dotpay_Pin"];

        public bool CheckResponseFromDotpay(string signature, string fullParameters)
        {
            string hashedLocalParameters = "";
            using (SHA256 hash = SHA256.Create())
            {
               hashedLocalParameters =  String.Join("", hash
                .ComputeHash(Encoding.UTF8.GetBytes(fullParameters))
                .Select(item => item.ToString("x2")));
            }
            
            if (hashedLocalParameters == signature)
            {
                return true;
            }
            return false;
        }

        public string CreateUrl(decimal amount, string description, string control, string firstName, string lastName, string email)
        {
            var currency = "PLN";
            var apiVersion = "dev";
            var id = DotpayId;
            //var URL = "PASTE_URL_HERE/payment/thanks";
            var type = "0";
            var buttontext = "Back to the Rent A Car";

            var url = $"https://ssl.dotpay.pl/test_payment/?api_version={apiVersion}&id={id}&amount={amount}&currency={currency}&description={description}&control={control}&URL={URL}&firstname={firstName}&lastname={lastName}&type={type}&buttontext={buttontext}&email={email}";
            return url;
        }
    }
}