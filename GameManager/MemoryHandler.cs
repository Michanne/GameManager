using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager
{
    public static class MemoryHandler
    {
        static BackgroundWorker worker = new BackgroundWorker();

        public static void CollectMemoryInformation()
        {

            worker.DoWork += (sender, e) =>
                {
                    
                    while(true)
                        Debug.WriteLine("Current Memory: " + DeviceStatus.ApplicationCurrentMemoryUsage + " / " + DeviceStatus.ApplicationMemoryUsageLimit);
                };

            worker.RunWorkerAsync();
        }
    }
}
