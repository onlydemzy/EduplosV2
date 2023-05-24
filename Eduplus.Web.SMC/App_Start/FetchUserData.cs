using Eduplus.Domain.CoreModule;
using Eduplus.Services.Contracts;
using Eduplus.Services.Implementations;
using KS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//mike-sumoh
//    untom

namespace Eduplus.Web.SMC
{
    public class FetchUserData
    {
       
        public static UserData CreateUserDataForCache()
        {

            UnitOfWork uow = new UnitOfWork();
            var udata = uow.UserData.Single();

            return udata;
        }
    }
}