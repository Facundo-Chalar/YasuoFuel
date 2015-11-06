using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;
using FuelSDK;
using glombardo.Helpers;

namespace YasuoApiConnector
{
    public class PollingJob:IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                
                   HelperDatabase conexionBD = new HelperDatabase("putyoursqlormysqlserverhere", "putyourdbnamehere", "user", "password", (int)HelperDatabase.typeConn.MYSQL,3306);
                   conexionBD.Connect();
                   var objResult = conexionBD.Execute("SELECT Value from Configuration where Name='LastPollTime'");
                   conexionBD.Disconnect();

                   if (objResult.error != null)
                   {
                        Console.WriteLine("Error retrieving last poll timestamp.");
                   }

            DateTime lastPollTimeStamp =DateTime.Parse (objResult.rows[0][0].ToString());
            
            ET_Client client = new ET_Client();
            Console.WriteLine("Establishing Connection...");
            
            ET_Send sendObject=new ET_Send();
            sendObject.AuthStub = client;

            sendObject.SearchFilter = new SimpleFilterPart() { Property = "SendDate", SimpleOperator = SimpleOperators.greaterThan, DateValue = new DateTime[] { lastPollTimeStamp } };
            
                //sendObject.Props = new string[] { "ID", "PartnerKey", "CreatedDate", "ModifiedDate", "Client.ID", "Client.PartnerClientKey", "Email.ID", "Email.PartnerKey", "SendDate", "FromAddress", "FromName", "Duplicates", "InvalidAddresses", "ExistingUndeliverables", "ExistingUnsubscribes", "HardBounces", "SoftBounces", "OtherBounces", "ForwardedEmails", "UniqueClicks", "UniqueOpens", "NumberSent", "NumberDelivered", "NumberTargeted", "NumberErrored", "NumberExcluded", "Unsubscribes", "MissingAddresses", "Subject", "PreviewURL", "SentDate", "EmailName", "Status", "IsMultipart", "SendLimit", "SendWindowOpen", "SendWindowClose", "IsAlwaysOn", "Additional", "BCCEmail", "EmailSendDefinition.ObjectID", "EmailSendDefinition.CustomerKey" };
            GetReturn response = sendObject.Get();

            Console.WriteLine("---Results obtained---");

            foreach (ET_Send send in response.Results)
            {
               

                conexionBD.Connect();
                var insertNewSendRecord = conexionBD.Execute("INSERT INTO SentData ");
                conexionBD.Disconnect();

                if (objResult.error != null){
                        Console.WriteLine("Error updating last poll timestamp.");
                }
                    string subject = send.Subject;
                    string status = send.Status;

                    if (!subject.StartsWith("Test Send") && status.Equals("Complete")){
                        string previewUrl = send.PreviewURL;
                        string domain = "placeholder";
                        string templateName = "placeholdertemplate";
                        DateTime sentDate = send.SendDate;
                        

                      
                        conexionBD.Connect();
                        var insertStatement = String.Format("INSERT INTO SentData(Url,Domain,Date,Subject,Template_Name) values ('{0}','{1}','{2}','{3}','{4}')", previewUrl, domain, sentDate.ToString("yyyy-MM-dd HH:mm:ss"), subject.Trim(), templateName);
                        var updatePollTimeResult = conexionBD.Execute(insertStatement);
                        conexionBD.Disconnect();
                        if (updatePollTimeResult.error != null)
                        {
                            Console.WriteLine("Error updating last poll timestamp.");
                        }
                    }
            }
            
         


            }
            catch (Exception ConnectionException){
                Console.WriteLine("Failed to do polling Job.");
                Console.WriteLine(ConnectionException.Message);
            }

        }


        private void  connectToDb()
        {
         
        }

        
    }
}
