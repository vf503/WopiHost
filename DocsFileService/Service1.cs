using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WopiCobaltHost;

namespace DocsFileService
{
    public partial class Service1 : ServiceBase
    {
        CobaltServer svr = new CobaltServer();

        public Service1()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            svr.Start();
        }

        protected override void OnStop()
        {
            svr.Stop();
        }
    }
}
