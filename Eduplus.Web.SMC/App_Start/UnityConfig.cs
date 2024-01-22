using Eduplos.Services.Contracts;
using Eduplos.Services.Implementations;
using KS.Core;
using KS.Data.Repositories;
using KS.Services.Contract;
using KS.Services.Implementation;
using System;
using System.Web.Http;
using Unity;

namespace Eduplos.Web.SMC
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<KS.Core.IUnitOfWork, KS.Data.UnitOfWork>();
            container.RegisterType(typeof(IRepository<>), typeof(KS.Data.Repositories.Repository<>));
            container.RegisterType<IAcademicAffairsService, AcademicAffairsService>();
            container.RegisterType<IAcademicProfileService, AcademicProfileService>();
            container.RegisterType<IAccountsService,AccountsService>();
            container.RegisterType<IAppImagesService,AppImagesService>();
            container.RegisterType<IArticleService,ArticleService>();
            container.RegisterType<IBursaryService,BursaryService>();
            container.RegisterType<ICommunicationService,CommunicationService>();
            container.RegisterType<IGeneralDutiesService,GeneralDutiesService>();
            container.RegisterType<IStaffService,StaffService>();
            container.RegisterType<IStudentsAccountsService,StudentsAccountsService>();
            container.RegisterType<IStudentService,StudentService>();
            container.RegisterType<IUserService,UserService>();

            //setting resolving for MCV5
            System.Web.Mvc.DependencyResolver.SetResolver(new Unity.AspNet.Mvc.UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.AspNet.WebApi.UnityDependencyResolver(container);

        }
    }
}