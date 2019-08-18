﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public sealed class Bootstrap
    {
        #region Singleton
        private static volatile Bootstrap instance;
        private static object syncRoot = new object();

        public Bootstrap() { }

        public static Bootstrap Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Bootstrap();
                    }
                }
                return instance;
            }
        }
        #endregion

        public IServiceProvider Services { get; set; }

        public bool InitApplication()
        {
            OnSetupServices();
            return true;
        }

        private void OnSetupServices()
        {
            IServiceCollection service = new ServiceCollection();
            service.AddSingleton<Globalize.ILocalize, Globalize.Localize>();
            service.AddSingleton<Internal.ILogger, Internal.Logger>();
            service.AddSingleton<Internal.ISettings, Internal.Settings>();

            service.AddSingleton<DAL.IDBContext, DAL.DBContext>();

            Services = service.BuildServiceProvider();
        }
    }
}
