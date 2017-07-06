using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using WebAPI.Controllers;
using WebAPI.IoC;
using WebAPI.Dal;

[assembly: OwinStartup(typeof(WebAPI.Startup))]

namespace WebAPI
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();

            configuration.IncludeErrorDetailPolicy = GetErrorDetailPolicy();

            ConfigureJsonFormatter(configuration.Formatters.JsonFormatter);
            WebApiConfig.Register(configuration);

            var authenticationManager = new Authentication.AuthenticationManager();
            authenticationManager.ConfigureAuth(app, configuration);

            var container = CreateWindsorContainer();
            ConfigureDependencyResolving(configuration, container);
            
            app.UseWebApi(configuration);
        }

        protected virtual IncludeErrorDetailPolicy GetErrorDetailPolicy() => IncludeErrorDetailPolicy.LocalOnly;

        protected virtual WindsorContainer CreateWindsorContainer()
        {
            var container = new WindsorContainer();
            container.AddFacility<TypedFactoryFacility>();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));

            container.Register(
                Component.For<ItemsController>()
                    .ImplementedBy<ItemsController>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IItemsRepository>()
                    .ImplementedBy<ItemsRepository>()
                    .LifestyleSingleton());

            container.Register(
                Component.For<GroupController>()
                    .ImplementedBy<GroupController>()
                    .LifestyleTransient());

            container.Register(
                Component.For<IGroupRepository>()
                    .ImplementedBy<GroupRepository>()
                    .LifestyleTransient());

            return container;
        }

        private static void ConfigureDependencyResolving(HttpConfiguration configuration, WindsorContainer container)
        {
            configuration.DependencyResolver = new WindsorDependencyResolver(container);
            configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorHttpControllerActivator(container));
        }

        public static void ConfigureJsonFormatter(JsonMediaTypeFormatter formatter)
        {
            // Support response in JSON format in Web API controllers
            formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            formatter.SerializerSettings.Formatting = Formatting.Indented;
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

    }
}
