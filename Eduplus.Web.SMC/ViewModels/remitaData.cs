using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Eduplus.Web.SMC.ViewModels
{
    public class remitaData
    {
        public string orderId { get; set; }
        public string payerName { get; set; }
        public string payerEmail { get; set; }
        public string payerPhone { get; set; }
        public string description { get; set; }
        public string amount { get; set; }
        public string serviceTypeId { get; set; }
         
        //public string hash { get; set; }
        public List<customFields> customFields { get; set; }
        public List<lineItems> lineItems { get; set; }
       
    }
    public class remitaDataNoLines 
    {
        public string orderId { get; set; }
        public string payerName { get; set; }
        public string payerEmail { get; set; }
        public string payerPhone { get; set; }
        public string description { get; set; }
        public string amount { get; set; }
        public string serviceTypeId { get; set; }

        //public string hash { get; set; }
        public List<customFields> customFields { get; set; }
       
    }
    
    public class customFields
    {
        public string name { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
    public class lineItems
    {
       public string lineItemsId { get; set; }
        public string beneficiaryName { get; set; }
        public string beneficiaryAccount { get; set; }
        public string bankCode { get; set; }
        public string beneficiaryAmount { get; set; }
        public string deductFeeFrom { get; set; }
    }
    public class remitaRRRGenResponse
    {
        public string RRR { get; set; }
        public string status { get; set; }//Payment Reference generated
        public string statuscode { get; set; } 
        public string statusMessage { get; set; }
    }

    public class remitaPayConfirmationData
    {
        public string RRR { get; set; }
        public string status { get; set; }//Payment Reference generated
        public string message { get; set; }
        public string merchantId { get; set; }
        public DateTime transactiontime { get; set; }
        public DateTime paymentDate { get; set; }
        public string orderId { get; set; }
        public double amount { get; set; }
    }

    public class remitaBankBranchResponseData
    {
        public string rrr { get; set; }
        public string channel { get; set; }
        public double amount { get; set; }
        public DateTime transactiondate { get; set; }
        public DateTime debitdate { get; set; }
        public string bank { get; set; }
        public string branch { get; set; }
        public string serviceTypeId { get; set; }
        public DateTime dateRequested { get; set; }
        public string orderRef { get; set; }
        public string payerName { get; set; }
        public string payerPhoneNumber { get; set; }
        public string payerEmail { get; set; }
        public string uniqueIdentifier { get; set; }
    }
     
}