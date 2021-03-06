﻿using System;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Microsoft.Owin.Testing;
using WebAPI.Controllers;
using WebAPI.Tests.Fakes;
using System.Web.Http;

namespace WebAPI.Tests.Infrastructure
{
    public class ApiServer : IDisposable
    {
        public TestServer TestServer { get; }
        private TestStartup Startup { get; }

        public ApiServer()
        {
            Startup = new TestStartup();
            TestServer = TestServer.Create(app =>
            {
                Startup.Configuration(app);
            });

        }

        public void Reset()
        {
            //Startup.ItemsRepository.Clear();
        }

        public void Dispose()
        {
            TestServer?.Dispose();
        }

        public IHttpRequester GetRequester()
        {
            var requester= new HttpRequester(TestServer.Handler, TestServer.BaseAddress);
            return requester;
        }
    }

    public class TestStartup : Startup
    {
        public ItemsRepositoryFake ItemsRepository { get; private set; }

        protected override WindsorContainer CreateWindsorContainer()
        {
            var container = base.CreateWindsorContainer();

            return container;
        }

        protected override IncludeErrorDetailPolicy GetErrorDetailPolicy() => IncludeErrorDetailPolicy.Always;
    }
}
