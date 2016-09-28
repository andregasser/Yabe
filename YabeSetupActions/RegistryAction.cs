using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Configuration.Install;

namespace YabeSetupActions
{
    [System.ComponentModel.RunInstaller(true)]
    public class RegistryAction : Installer
    {
        private string eventSourceName = "Yabe";
        
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            
            if (!EventLog.SourceExists(eventSourceName, "."))
            {
                EventSourceCreationData eventLogData = new EventSourceCreationData(eventSourceName, "Application");
                eventLogData.MachineName = ".";
                EventLog.CreateEventSource(eventLogData);
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            
            if (EventLog.SourceExists(eventSourceName, "."))
            {
                EventLog.DeleteEventSource(eventSourceName, ".");
            }
        }

        public override void Rollback(System.Collections.IDictionary savedState)
        {
            base.Rollback(savedState);
            
            if (EventLog.SourceExists(eventSourceName, "."))
            {
                EventLog.DeleteEventSource(eventSourceName, ".");
            }
        }
    }
}
