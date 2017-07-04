using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.Windsor;

namespace WebAPI.IoC
{
    /// <summary> 
    ///   windsor webApi controller factory 
    /// </summary> 
    public sealed class WindsorHttpControllerActivator : IHttpControllerActivator
    {
        private readonly IWindsorContainer _container;
        /// <summary> 
        ///   constructor 
        /// </summary> 
        /// <param name="container">windsor container</param> 
        public WindsorHttpControllerActivator(IWindsorContainer container)
        {
            this._container = container;
        }
        /// <summary> 
        ///   Constructor 
        /// </summary> 
        /// <param name="request">request</param> 
        /// <param name="controllerDescriptor">controller descriptor</param> 
        /// <param name="controllerType">controller type</param> 
        /// <returns>controller</returns> 
        public IHttpController Create(
          HttpRequestMessage request,
          HttpControllerDescriptor controllerDescriptor,
          Type controllerType)
        {
            var controller = (ApiController)this._container.Resolve(controllerType);
            request.RegisterForDispose(new Release(() => this._container.Release(controller)));
            return controller;
        }
        private class Release : IDisposable
        {
            private readonly Action _release;
            public Release(Action release)
            {
                _release = release;
            }
            public void Dispose()
            {
                _release();
            }
        }
    }
}