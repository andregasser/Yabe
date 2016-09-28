using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Yabe.Mailer;

namespace Yabe
{
    class YabeMain
    {
        static YabeConfig yabeConfig = new YabeConfig();
        
        #region Main
        static void Main(string[] args)
        {
            // Print Banner
            Console.WriteLine(" ___ ___ _______ _______  _______");
            Console.WriteLine("|   Y   |   _   |   _   \\|   _   |");
            Console.WriteLine("|   1   |.  1   |.  1   /|.  1___|");
            Console.WriteLine(" \\_   _/|.  _   |.  _   \\|.  __)_");
            Console.WriteLine("  |:  | |:  |   |:  1    |:  1   |");
            Console.WriteLine("  |::.| |::.|:. |::.. .  |::.. . |");
            Console.WriteLine("  `---' `--- ---`-------'`-------'");
            Console.WriteLine("");      
            Console.WriteLine("YABE 0.2");
            Console.WriteLine("Yet Another Batch Engine");
            Console.WriteLine("This tool is free.");
            Console.WriteLine("Written by A. Gasser 2010");
            Console.WriteLine("Mail: andre.gasser@gmx.ch");
            Console.WriteLine("-----");
            
            // Prepare YabeConfig structure
            yabeConfig.successMailRecipients = new Dictionary<string, string>();
            yabeConfig.errorMailRecipients = new Dictionary<string, string>();

            // Read commands from text file
            ReadXMLConfig();
            Console.WriteLine("Config loaded.");

            // Process Actions
            ProcessActions();
        }
        #endregion

        #region ProcessActions
        private static void ProcessActions()
        {
            try
            {
                foreach (YabeAction action in yabeConfig.actions)
                {
                    switch (action.type)
                    {
                        case YabeActionType.ShellCommand: ExecuteShellCommand(action); break;
                    }

                }

                // Create event log entry
                if (yabeConfig.eventLogEnabled)
                {
                    CreateEventLogEntrySuccess("Batch job finished without errors.");
                }
                
                // Send success email
                if ((yabeConfig.SMTPHost != "") && (yabeConfig.SMTPHost != null))
                {
                    Mail mail = new Mail();
                    foreach (KeyValuePair<string, string> recipient in yabeConfig.successMailRecipients)
                    {
                        mail.AddRecipient(MailRecipientType.To, recipient.Key, recipient.Value);
                    }
                    mail.SetFrom(yabeConfig.successMailSenderEmail, yabeConfig.successMailSenderName);
                    mail.Host = yabeConfig.SMTPHost;
                    mail.Username = "";
                    mail.Password = "";
                    mail.Port = yabeConfig.SMTPPort;
                    mail.Priority = MailPriority.Normal;
                    mail.Subject = "[" + yabeConfig.hostName + "] " + yabeConfig.successMailSubject;
                    mail.SetPlaintextBody(yabeConfig.successMailBody);
                    mail.Send();
                }
            }
            catch (ActionFailedException ex)
            {
                // Create event log entry
                if (yabeConfig.eventLogEnabled)
                {
                    CreateEventLogEntryFailure("Batch job finished with errors.\n"
                    + "Action id = " + ex.actionId + "\n" 
                    + "Error message = " + ex.errorMessage);
                }
                
                // Send error email
                if ((yabeConfig.SMTPHost != "") && (yabeConfig.SMTPHost != null))
                {
                    Mail mail = new Mail();
                    foreach (KeyValuePair<string, string> recipient in yabeConfig.errorMailRecipients)
                    {
                        mail.AddRecipient(MailRecipientType.To, recipient.Key, recipient.Value);
                    }
                    mail.SetFrom(yabeConfig.errorMailSenderEmail, yabeConfig.errorMailSenderName);
                    mail.Host = yabeConfig.SMTPHost;
                    mail.Username = "";
                    mail.Password = "";
                    mail.Port = yabeConfig.SMTPPort;
                    mail.Priority = MailPriority.High;
                    mail.Subject = "[" + yabeConfig.hostName + "] " + yabeConfig.errorMailSubject;
                    string errorMailBody = "";
                    errorMailBody += yabeConfig.errorMailBody;
                    errorMailBody += "\n\n";
                    errorMailBody += "-----\n";
                    errorMailBody += "The following exception was thrown:\n";
                    errorMailBody += "Action Error Message:\n" + ex.errorMessage + "\n\n";
                    errorMailBody += "Failed Action Id:\n" + ex.actionId + "\n\n";
                    errorMailBody += "Message:\n" + ex.Message + "\n\n";
                    errorMailBody += "Inner Exception:\n" + ex.InnerException + "\n\n";
                    errorMailBody += "Stack Trace:\n" + ex.StackTrace + "\n\n";
                    mail.SetPlaintextBody(errorMailBody);
                    mail.Send();
                }
            }
        }
        #endregion

        #region CreateEventLogEntrySuccess
        private static void CreateEventLogEntrySuccess(string message)
        {
            EventLog eventLog = new EventLog();
            eventLog.Source = "Yabe";
            eventLog.WriteEntry(message, EventLogEntryType.Information);
        }
        #endregion

        #region CreateEventLogEntryFailure
        private static void CreateEventLogEntryFailure(string message)
        {
            EventLog eventLog = new EventLog();
            eventLog.Source = "Yabe";
            eventLog.WriteEntry(message, EventLogEntryType.Error);
        }
        #endregion

        #region ExecuteShellCommand
        private static void ExecuteShellCommand(YabeAction action)
        {
            Console.WriteLine("----------");
            Console.WriteLine(DateTime.Now.ToString() + " Executing: " + action.shellcommand);

            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd.exe", "/c " + action.shellcommand);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            string exitCode = proc.ExitCode.ToString();

            Console.WriteLine("Result = " + result);
            Console.WriteLine("Exit Code = " + exitCode);

            if (!action.successExitCodes.Contains(exitCode))
            {
                ActionFailedException ex = new ActionFailedException();
                ex.errorMessage = "Action " + action.id + " failed during execution.";
                ex.actionId = action.id;
                throw ex;
            }
        }
        #endregion

        #region ReadXMLConfig
        private static void ReadXMLConfig()
        {
            yabeConfig.actions = new List<YabeAction>();
            
            XmlTextReader reader = new XmlTextReader("config.xml");
            while (reader.Read())
            {
                // If node type us a declaration
                if (reader.NodeType == XmlNodeType.Element)
                {
                    //Console.WriteLine(reader.Name.ToString());
                    switch (reader.Name)
                    {
                        case "host": SetupHost(reader.ReadSubtree()); break;
                        case "actions": SetupActions(reader.ReadSubtree()); break;
                        case "notification": SetupNotification(reader.ReadSubtree()); break;
                    }
                }
            }
            reader.Close();
        }
        #endregion

        #region SetupHost
        private static void SetupHost(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "hostname")
                {
                    string hostName = reader.ReadElementContentAsString();
                    yabeConfig.hostName = hostName;
                }
            }
        }
        #endregion

        #region SetupActions
        private static void SetupActions(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "action")
                {
                    YabeAction action = new YabeAction();
                    action.successExitCodes = new List<string>();
                    action.successRegexPatterns = new List<string>();
                    
                    int actionId = Convert.ToInt32(reader.GetAttribute("id"));
                    
                    YabeActionType actionType = YabeActionType.ShellCommand;
                    switch (reader.GetAttribute("type"))
                    {
                        case "shellcommand": actionType = YabeActionType.ShellCommand; break;
                    }
                    
                    action.id = actionId;
                    action.type = actionType;

                    // Extract Shellcommand data
                    if (actionType == YabeActionType.ShellCommand)
                    {
                        XmlReader reader2 = reader.ReadSubtree();
                        while (reader2.Read())
                        {
                            if (reader2.NodeType == XmlNodeType.Element && reader2.Name == "shellcommand")
                            {
                                action.shellcommand = reader2.ReadElementContentAsString();
                            }
                            if (reader2.NodeType == XmlNodeType.Element && reader2.Name == "exitcodesuccess")
                            {
                                action.successExitCodes.Add(reader2.ReadElementContentAsString());
                            }
                            if (reader2.NodeType == XmlNodeType.Element && reader2.Name == "regexsuccess")
                            {
                                action.successRegexPatterns.Add(reader2.ReadElementContentAsString());
                            }
                        }
                    
                        // Add ShellCommand action to action list
                        yabeConfig.actions.Add(action);
                    }
                }
            }
        }
        #endregion

        #region SetupNotification
        private static void SetupNotification(XmlReader reader)
        {
            bool successMail = false;
            
            while (reader.Read())
            {
                // Read email configuration block
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "email")
                {
                    XmlReader reader2 = reader.ReadSubtree();
                    while (reader2.Read())
                    {
                        if (reader2.NodeType == XmlNodeType.Element && reader2.Name == "smtphost")
                        {
                            yabeConfig.SMTPHost = reader2.ReadElementContentAsString();
                        }
                        if (reader2.NodeType == XmlNodeType.Element && reader2.Name == "smtpport")
                        {
                            yabeConfig.SMTPPort = Convert.ToInt32(reader2.ReadElementContentAsString());
                        }
                        if (reader2.NodeType == XmlNodeType.Element && reader2.Name == "mailmessage")
                        {
                            // Set mail event
                            switch (reader2.GetAttribute("event"))
                            {
                                case "onsuccess": successMail = true; break;   // Success email
                                case "onerror": successMail = false; break;    // Error email
                            }

                            XmlReader reader3 = reader2.ReadSubtree();
                            while (reader3.Read())
                            {
                                if (reader3.NodeType == XmlNodeType.Element && reader3.Name == "sender")
                                {
                                    if (successMail)
                                    {
                                        yabeConfig.successMailSenderName = reader3.GetAttribute("name");
                                        yabeConfig.successMailSenderEmail = reader3.GetAttribute("email");
                                    }
                                    else
                                    {
                                        yabeConfig.errorMailSenderName = reader3.GetAttribute("name");
                                        yabeConfig.errorMailSenderEmail = reader3.GetAttribute("email");
                                    }
                                }
                                if (reader3.NodeType == XmlNodeType.Element && reader3.Name == "subject")
                                {
                                    if (successMail)
                                    {
                                        yabeConfig.successMailSubject = reader3.ReadElementContentAsString();
                                    }
                                    else
                                    {
                                        yabeConfig.errorMailSubject = reader3.ReadElementContentAsString();
                                    }
                                }
                                if (reader3.NodeType == XmlNodeType.Element && reader3.Name == "body")
                                {
                                    if (successMail)
                                    {
                                        yabeConfig.successMailBody = reader3.ReadElementContentAsString();
                                    }
                                    else
                                    {
                                        yabeConfig.errorMailBody = reader3.ReadElementContentAsString();
                                    }
                                }
                                if (reader3.NodeType == XmlNodeType.Element && reader3.Name == "recipients")
                                {
                                    XmlReader reader4 = reader3.ReadSubtree();
                                    while (reader4.Read())
                                    {
                                        if (reader4.NodeType == XmlNodeType.Element && reader4.Name == "recipient")
                                        {
                                            if (successMail)
                                            {
                                                yabeConfig.successMailRecipients.Add(
                                                    reader4.GetAttribute("email"),
                                                    reader4.GetAttribute("name")
                                                );
                                            }
                                            else
                                            {
                                                yabeConfig.errorMailRecipients.Add(
                                                    reader4.GetAttribute("email"),
                                                    reader4.GetAttribute("name")
                                                );
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Read event log configuration block
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "eventlog")
                {
                    string eventLogEnabledVal = reader.GetAttribute("enabled");
                    
                    if (eventLogEnabledVal == "true")
                    {
                        yabeConfig.eventLogEnabled = true;
                    }
                    else
                    {
                        yabeConfig.eventLogEnabled = false;
                    }
                }
            }
        }
        #endregion
    }
}
