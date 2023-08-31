using Eduplus.Domain.BurseryModule;
using Eduplus.DTO.BursaryModule;
using System;
using System.Collections.Generic;

namespace Eduplus.Services.Contracts
{
    public interface IStudentsAccountsService
    {
        
          
        string DebitNewStudent(string studentId,string userId);
        string DebitStudentAccount(TransactionDTO debit, string userId);
        PaymentInvoice GenerateSchoolFeesPaymentInvoice(string regNo, int sessionId, int installmentId,int level, string userId, out string flag);
        PaymentInvoice GenerateStudentPaymentInvoice(string regNo, string chargeTitle,int sessionId, string userId, out string flag);
        string GenerateTranscriptInvoice(string transcriptNo, int otherChargeId, int sessionId, string userId, out string flag);
        double GenerateExpectedSchoolFeesPayment(string regNo, int sessionId, int installmentId, int payLevel, out string flag);
        string SubmitGenerateExpectedSchoolFeesPayment(string regNo, int sessionId, double amount, int installmentId, string userId, string username, out string flag);
        List<StudentTransactionDTO> StudentPayments(string studentId, out string balance);
        StudentAccountStatementDTO StudentAccountStatement(string studentId);
        
        StudentAccountSummaryDTO StudentAccountSummary(string matricNumber);
        
        List<StudentsBalancesDTO> StudentPayments(DateTime fromDate, DateTime toDate);
        List<StudentsBalancesDTO> StudentPayments(int sessionId, string progCode);
        bool CheckPreviousSessionDebt(string studentId, int sessionId,out string amt);
        List<PaymentInvoiceDTO> PaymentInvoices(string studentId);
        int CheckAdmissionFeePayment(string studentId);
        int CheckAcceptanceFeePayment(string studentId);
        int CheckIfOtherFeePaymentExist(string studentId, string paymentType);
         
        PaymentInvoiceDTO GetStudentPaymentInvoice(string transactionId);
        PaymentInvoiceDTO GetStudentPaymentInvoice(string transactionId, string payref);
        string ProcessSuccessfulEPayments(string transId, DateTime completionDate, string confirmChannel, string userId = null);
        string GenerateStudentInvoiceToCreditStudentAccount(TransactionDTO debit, string userId,out string flag);
        PaymentInvoiceDTO GetStudentPaymentInvoiceForManualConfirmation(string transactionId);

        //Fee exemptions
        void AddFeeException(FeesExceptions exception);
        List<FeeExempListDTO> StudentsForExemption(string progCode);
        void AddBulkExemptions(List<FeeExempListDTO> exemptions, string user);
        IEnumerable<FeesExceptions> FetchFeeExceptions(int semesterId);
        FeesExceptions StudentInException(string studentId, int semesterId);
        List<FeesExceptions> ApprovedExcemptionList(List<int> exceptionIds, string approvedBy, string userId);
        List<FeesExceptions> AllSemesterExcemptions(int semesterId);
        List<PaymentInvoiceDTO> FetchCompletePay4Confirmation(DateTime startDate);
        List<PaymentInvoiceDTO> FetchInvoiceforConfirmationProcess();
        void UPdateManualInvoiceMethd(List<TransIdsProcessMthdDTO> proc);
        //Fetch Invoices
         

    }
}
