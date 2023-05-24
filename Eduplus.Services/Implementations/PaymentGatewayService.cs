using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.CoreModule;
using Eduplus.DTO.CoreModule;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Eduplus.Services.Implementations
{
    public class PaymentGatewayService
    {
        
        public  string GenerateGatewayTransReference(string gatewayname,PaymentGateways gateway)
        {
            string refs="";
            switch (gatewayname)
            {
                case "Remita":
                    refs="Remita";
                    break;
            }
            return refs;
        }
        public static remitaRRRGenResponse GenerateRemitaRRR(string transId,string dept,string session,string prog,ProgrammeTypes progType,
            string particulars,string payer,string email,string phone,string payType,string regno, double amount,double providerCharge)
        {
            var gateway = progType.PaymentGateWays.Where(a => a.IsDefault == true).SingleOrDefault();
            var mapData = MapInvoiceData2RemitaObject(amount, transId,particulars, payer,progType, email, phone,providerCharge, payType);
            RemitaPaymentDataWithLineItems remWithLines = new RemitaPaymentDataWithLineItems();
            remitaRRRGenResponse rrrGen = new remitaRRRGenResponse();
            List<CustomFields> cf = new List<CustomFields>();
            //custom fields
            GeneralDutiesService gen = new GeneralDutiesService();
            cf.Add(new CustomFields { name = "RegNo", value = regno == null ? "NIL" : regno, type = "ALL" });
            cf.Add(new CustomFields { name = "Session", value =session, type = "ALL" });
            cf.Add(new CustomFields { name = "Department", value = dept, type = "ALL" });
            cf.Add(new CustomFields { name = "Programme", value = prog, type = "ALL" });
            cf.Add(new CustomFields { name = "ProgrammeType", value = progType.Type, type = "ALL" });
            cf.Add(new CustomFields { name = "PaymentType", value = payType, type = "ALL" });
            string hash = gen.Sha512Hasher(gateway.MerchantCode + gateway.ServiceId + transId + mapData.amount + gateway.MerchantKey);

            HttpResponseMessage response = null;
            string rrrGeneratorUrl = gateway.TransRefGenUrl;
            string requestHeader = "remitaConsumerKey=" + gateway.MerchantCode + ",remitaConsumerToken=" + hash;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", requestHeader);
            if (mapData.lineItems.Count() > 0)
            {
                mapData.customFields = cf;
                response = client.PostAsJsonAsync<RemitaPaymentDataWithLineItems>(rrrGeneratorUrl, mapData).Result;
            }
            else
            {
                RemitaPaymentDataWithNoLineItems noLines = new RemitaPaymentDataWithNoLineItems();
                noLines.amount = mapData.amount;
                noLines.customFields = mapData.customFields;
                noLines.description = mapData.description;
                noLines.orderId = mapData.orderId;
                noLines.payerEmail = mapData.payerEmail;
                noLines.payerName = mapData.payerName;
                noLines.payerPhone = mapData.payerPhone;
                noLines.serviceTypeId = mapData.serviceTypeId;
                noLines.customFields = cf;
                response = client.PostAsJsonAsync<RemitaPaymentDataWithNoLineItems>(rrrGeneratorUrl, noLines).Result;
            }

            if (!response.IsSuccessStatusCode)
            {
                rrrGen.statuscode = "OO";
                rrrGen.statusMessage= response.ReasonPhrase;
                
                return rrrGen;
            }
            var read = response.Content.ReadAsStringAsync().Result;
            var nr = read.Replace("jsonp (", "");
            var nr1 = nr.Replace(")", "");

            var responseData = JsonConvert.DeserializeObject<remitaRRRGenResponse>(nr1);

            if (responseData.statuscode == "025") //Update paymentinvoice
            {
                
                return responseData;

            }
            
            else
            {
                rrrGen.statuscode = "00";
                rrrGen.statusMessage = responseData.status+"-"+responseData.statusMessage;
                return rrrGen;
            }

        }
   
        static RemitaPaymentDataWithLineItems MapInvoiceData2RemitaObject(double amount,string transId,string particulars,string payer,
            ProgrammeTypes progtype, string email, string phone,double providerCharge,string paytype)
        {

            var gateway = progtype.PaymentGateWays.Where(a => a.IsDefault == true).SingleOrDefault();
            RemitaPaymentDataWithLineItems rem = new RemitaPaymentDataWithLineItems();
            List<CustomFields> customFields = new List<CustomFields>();
            List<lineItems> lineItems = new List<lineItems>();
            rem.orderId = transId;
            rem.description = particulars;
            rem.payerEmail = email;
            rem.payerName = payer;
            rem.serviceTypeId = gateway.ServiceId;
            rem.payerPhone = phone;

            rem.amount = ((providerCharge-gateway.GatewayCharge) + amount).ToString();

            if ((progtype.ApplyMajorCharge==true&&paytype=="School Fee") ||
                (progtype.ApplyMinorCharge == true && paytype != "School Fee"))//Provider service charge applied add lines
            {
                lineItems.Add(new lineItems
                {
                    lineItemsId = "itemId1",
                    bankCode = gateway.ProviderBankCode,
                    beneficiaryAccount = gateway.ProviderAccountNumber,
                    beneficiaryAmount = (providerCharge - gateway.GatewayCharge).ToString(),
                    beneficiaryName = gateway.ProviderAccountName,
                    deductFeeFrom = "0"
                });

                lineItems.Add(new lineItems
                {
                    lineItemsId = "itemId2",
                    bankCode = gateway.ClientBankCode,
                    beneficiaryAccount = gateway.ClientAccountNumber,
                    beneficiaryAmount = amount.ToString(),
                    beneficiaryName = gateway.ClientAccountName,
                    deductFeeFrom = "1"
                });

            }

            rem.lineItems = lineItems;
            return rem;
        }
    }
}
