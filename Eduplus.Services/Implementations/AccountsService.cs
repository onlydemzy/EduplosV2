using Eduplos.Domain.BurseryModule;
using Eduplos.DTO.BursaryModule;
using KS.Core;
using KS.Domain.AccountsModule;
using KS.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KS.Services.Implementation
{
    public class AccountsService:IAccountsService
    {
        IUnitOfWork _unitOfWork;
        public AccountsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region ACCOUNTING CHART OPERATIONS
        public List<Accounts> AllAccounts()
        {
            var accts = _unitOfWork.AccountsRepository.GetAll()
                .OrderBy(a => a.AccountType).OrderBy(a => a.AccountCode);
            return accts.ToList();
        }
        public List<Accounts> FetchBankAccounts()
        {
            var accts = _unitOfWork.AccountsRepository.GetFiltered(a => a.Description == "Bank").ToList();
            return accts;
        }

        public Accounts SaveAccount(Accounts account, string userId)
        {
            var dbAccount = _unitOfWork.AccountsRepository.Get(account.AccountCode);
            if (dbAccount == null)//fresh account, add
            {
                _unitOfWork.AccountsRepository.Add(account);
                _unitOfWork.Commit(userId);
                return account;
            }
            dbAccount.AccountType = account.AccountType;
            dbAccount.Active = account.Active;
            dbAccount.CurrentBalance = account.CurrentBalance;
            dbAccount.Description = account.Description;
            dbAccount.OpeningBalance = account.OpeningBalance;
            dbAccount.Title = account.Title;

            _unitOfWork.Commit(userId);
            return account;
        }
        #endregion

        #region ACCOUNTS TRANSACTION
        public string CreditAccount(TransactionDTO transaction)
        {
            //Credit Income Account
            TransMaster tm = new TransMaster();
            tm.AccountCode = transaction.AccountCode;
            tm.Amount = transaction.Amount;
             
            tm.Particulars = transaction.Particulars;
            tm.TellerNo = transaction.VoucherNumber;
            tm.TransDate = transaction.PayDate;
             
            tm.PayMethod = transaction.PayMethod;
            tm.TransType = "Credit";
            _unitOfWork.TransMasterRepository.Add(tm);

            //Credit Bank Account

            TransMaster tm1 = new TransMaster();
            tm1.AccountCode = transaction.BankAccountCode;
            tm1.Amount = transaction.Amount;
            tm1.Particulars = transaction.Particulars;
            tm1.TellerNo = transaction.VoucherNumber;
            tm1.TransDate = transaction.PayDate;
             
            tm1.PayMethod = transaction.PayMethod;
            tm1.TransType = "Credit";
            _unitOfWork.TransMasterRepository.Add(tm1);

            _unitOfWork.Commit(transaction.InputtedBy);

            return "Transaction added successfully";

        }
        public string TransferMoneyToBankAccounts(string sourceAccount,string destinationAccount,double amount,string authorisedBy,
            DateTime date,string voucherNo,string depositor,string transferMode,string inputtedBy)
        {
            //debit source account
            
            TransMaster tm = new TransMaster();
            tm.AccountCode = sourceAccount;
            tm.Amount = amount;
             
            tm.Particulars = "Money deposited by"+depositor+", authorised by "+authorisedBy;
            tm.TellerNo = voucherNo;
            tm.TransDate = date;
            
            tm.PayMethod = transferMode;
            tm.TransType = "Debit";
            _unitOfWork.TransMasterRepository.Add(tm);

            //Credit Destination Account

            TransMaster tm1 = new TransMaster();
            tm1.AccountCode = destinationAccount;
            tm1.Amount = amount;
             
            tm1.Particulars = "Money deposited by" + depositor + ", authorised by " + authorisedBy;
            tm1.TellerNo = voucherNo;
            tm1.TransDate = date;
             
            tm1.PayMethod = transferMode;
            tm1.TransType = "Credit";
            _unitOfWork.TransMasterRepository.Add(tm1);

            _unitOfWork.Commit(inputtedBy);
            return "Transaction completed successfully";
        }


        
        #endregion
    }
}
