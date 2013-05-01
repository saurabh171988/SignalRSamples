﻿using System;
using System.Reflection;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SignalRIoCScopeSample
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.MapHubs();
            GlobalHost.DependencyResolver = new AutofacDependencyResolver(RegisterServices(new ContainerBuilder()));
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            // InstancePerLifetimeScope: This will keep the instance till it's disposed

            builder.RegisterType<Broadcaster>().As<IBroadcaster>().InstancePerLifetimeScope();
            builder.RegisterType<Foo>().As<IFoo>();
            builder.RegisterType<Bar>().As<IBar>();

            builder.Register(c => serializer).As<JsonSerializer>();
            return builder.Build();
        }
    }
}