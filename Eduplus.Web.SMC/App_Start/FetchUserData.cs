using Eduplos.Domain.CoreModule;
using Eduplos.Services.Contracts;
using Eduplos.Services.Implementations;
using KS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//mike-sumoh
//    untom

namespace Eduplos.Web.SMC
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