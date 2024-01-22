using Eduplos.Domain.BurseryModule;
using Eduplos.DTO.BursaryModule;
using KS.Domain.AccountsModule;
using System;
using System.Collections.Generic;

namespace Eduplos.Services.Contracts
{
    public interface IBursaryService
    {
        
        List<FeeScheduleDTO> FetchFeeSchedules();
        FeeScheduleDTO FetchFeeScheduleById(int scheduleId);
        string DeletefeeScheduleLineItem(FeeScheduleDetailsDTO schedule, string userID);
        FeeScheduleDetailsDTO SaveFeeScheduleLineItem(FeeScheduleDetailsDTO schedule, out string msg, string userId);
        FeeScheduleDTO FetchCurrentFeeSchedule(string facultyCode, string programmeType);
        string ApplyFeeSchedule(int scheduleId, string userId);
         
        string DebitStudentAccount(TransactionDTO debit, string userId);
        
        StudentFeeScheduleDTO PaymentReceipt(string paymentReference);
        
        StudentAccountStatementDTO StudentAccountStatement(string studentId);
        
        StudentAccountSummaryDTO StudentAccountSummary(string matricNumber);
        List<FeeOptions> GetFeeOptions();
        List<FeeOptions> StudentFeeOptionsByProgType(string studentId);
        FeeOptions SaveFeeOptions(FeeOptions option, string userId);
        List<StudentTransactionDTO> StudentPayments(string studentId, out string balance);
        FeesCollectionDTO StudentPayments(DateTime fromDate, DateTime toDate, string accountCode, string deptCode);
        FeesCollectionDTO StudentPayments(DateTime fromDate, DateTime toDate, string accountCode, string deptCode, string progType);
        FeesCollectionDTO SchoolFeePayments(int sessionId, string deptCode);
        FeesCollectionDTO SchoolFeePayments(int sessionId, string deptCode, string progType);
        FeesCollectionDTO SchoolFeePayments(DateTime fromDate, DateTime toDate, string deptCode, string progType);
        List<PaymentTypesCollectionSummaryDTO> DailyPaymentsByPayType(DateTime date);
        List<PaymentTypesCollectionSummaryDTO> DailyPaymentsByProgType(DateTime date);
        FeesCollectionDTO FeesCollectionSummary(DateTime from, DateTime to, string progType);
        List<FeeOptions> GetActiveFeeOptions();

        //void AddStudentTransaction(StudentPayments trans);
        OtherCharges FetchOtherChargesAmount(string otherCharge, string studentId);
        OtherCharges FetchTranscriptcharges(string deliveryArea, string studentId);
        OtherCharges FetchApplicationFeeAmount(string studentId);
        List<OtherCharges> FetchStudentOtherChargesByProgramType(string studentId);
        string SaveOtherCharge(OtherCharges otherCharge, string userId);
        List<OtherCharges> AllOtherCharges();
        Accounts FetchSingleAccount(string title);
        List<StudentsBalancesDTO> StudentBalancesByProgType(int sessionId, string progType);
        List<StudentsBalancesDTO> StudentBalancesByDept(int sessionId, string progType);
        List<AccountsDTO> RevenueAccountsSummaryCollectionsBySession(string sessionPaid);
        FeesCollectionDTO RevenueAccountCollectionsDetails(string sessionPaid, string accountCode);
        FeesCollectionDTO RevenueAccountCollectionsDetails(DateTime fromDate, DateTime toDate, string accountCode);
        FeesCollectionDTO RevenueAccountCollectionsDetailsAllFees(DateTime fromDate, DateTime toDate);
        double SessionTotalCollections(string session);
        List<PaymentInvoiceDTO> AllPendingTransactions();
        List<FeesCollectionDTO> SessionTotalCollectionsProgrammeType(string session);
        string ApplyFeeNewSchedule(string studentId, string userId);
        DailyCollectionDTO DailyRevenueCollectionsBulk(DateTime fromDate, DateTime toDate);
        DailyCollectionDTO DailyRevenueCollectionsFull(DateTime fromDate, DateTime toDate);


    }
}
