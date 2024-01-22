using Eduplos.Domain.BurseryModule;
using Eduplos.DTO.BursaryModule;
using KS.Domain.AccountsModule;
using System.Collections.Generic;

namespace KS.Services.Contract
{
    public interface IAccountsService
    {
        List<Accounts> AllAccounts();
        Accounts SaveAccount(Accounts account, string userId);
        List<Accounts> FetchBankAccounts();
       
    }
}
