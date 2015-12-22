///////////////////////////////////////////////////////////////
// Server.cs - Server programs for Query processing for remote database implmentation//
// Ver 2.3                                                  //
// Application: Project2 for CSE681-SMA, Project#4      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Thinkpad T540p, Core-i7, Windows 10            //
// Author:      Ravichandra Malapati, CST 4-187, Syracuse University  //
//              (315) 706-3437, rmalapat@syr.edu            //
//New Release 21-Nov-2015//
// Original Author: Ravichandra Malapati                             //
///////////////////////////////////////////////////////////////
/* 
Pckage operations:
------------------------
Public Interfaces: no public interfaces
This implements the requirmnets that are required to process the incoming requests. 

 * Maintenance History:
 * --------------------
 */
/////////////////////////////////////////////////////////////////////////
// Server.cs - CommService server                                      //
// ver 2.2                                                             //
// Original author: Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - This server now receives and then sends back received messages.
 */
/*
 * Plans:
 * - Add message decoding and NoSqlDb calls in performanceServiceAction.
 * - Provide requirements testing in requirementsServiceAction, perhaps
 *   used in a console client application separate from Performance 
 *   Testing GUI.
 */
/*
 
 * ver 2.3 : 29 Oct 2015
 * - added handling of special messages: 
 *   "connection start message", "done", "closeServer"
 * ver 2.2 : 25 Oct 2015
 * - minor changes to display
 * ver 2.1 : 24 Oct 2015
 * - added Sender so Server can echo back messages it receives
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 2.0 : 20 Oct 2015
 * - Defined Receiver and used that to replace almost all of the
 *   original Server's functionality.
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project4Starter
{
    using Util = Utilities;

    class Server
    {
        LoadXMLDB load_back = new LoadXMLDB();
        private DBEngine<string, DBElement<string, string>> database_Live = new DBEngine<string, DBElement<string, string>>();
        private DBEngine<string, PackageElement> database_clone = new DBEngine<string, PackageElement>();
        public List<PackageElement> packageList_clone = new List<PackageElement>();
        public List<DBElement<string, string>> dbelement_clone = new List<DBElement<string, string>>();

        string address { get; set; } = "localhost";
        string port { get; set; } = "8080";
        //----< quick way to grab ports and addresses from commandline >-----

        public void ProcessCommandLine(string[] args)
        {
            if (args.Length > 0)
            {
                port = args[0];
            }
            if (args.Length > 1)
            {
                address = args[1];
            }
        }
        static void Main(string[] args)
        {
            Util.verbose = false;
            Server srvr = new Server();
            srvr.ProcessCommandLine(args);
            MessageMaker makeMsg = new MessageMaker();
            Console.Title = "Server";
            Console.Write(String.Format("\n  Starting CommService server listening on port {0}", srvr.port));
            Console.Write("\n ====================================================\n");
            srvr.start_server();
            Sender sndr = new Sender(Util.makeUrl(srvr.address, srvr.port));
            Receiver rcvr = new Receiver(srvr.port, srvr.address);
            Action serviceAction = () =>
            {
                Message msg = null;
                while (true)
                {
                    msg = rcvr.getMessage();   // note use of non-service method to deQ messages
                    Console.Write("\n  Received message:");
                    Console.Write("\n  sender is {0}", msg.fromUrl);
                    Console.Write("\n  content is {0}\n", msg.content);

                    if (msg.content == "connection start message")
                    {
                        continue; // don't send back start message
                    }
                    else if (msg.content == "done")
                    {
                        Message msg2 = makeMsg.replyMakeMessage(msg.toUrl, msg.fromUrl);
                        msg2.content = "done";
                        Console.WriteLine("sending End message");
                        sndr.sendMessage(msg2);
                        Console.Write("\n  client has finished\n");
                        continue;
                    }
                    else if (msg.content == "closeServer")
                    {
                        Console.Write("received closeServer");
                        break;
                    }
                    else if (msg.content == "DBwork")
                    {
                        Message sndMessage = makeMsg.replyMakeMessage(msg.toUrl, msg.fromUrl);
                        Message infoMessage = makeMsg.replyMakeMessage(msg.toUrl, msg.fromUrl);
                        Console.WriteLine("Received DB query : {0}", msg.query);
                        PackageElement check = new PackageElement();
                        {   
                            process1(msg, check);
                            infoMessage.query = "info";
                            infoMessage.content = "Requirment 8:processing  Query is: " + check.Query;
                            sndr.sendMessage(infoMessage);
                            //srvr.processDBQuery(check, msg.packageList[i], sndMessage);
                            srvr.processDBQuery(check, msg.package, sndMessage, srvr, sndr);
                            sndr.sendMessage(sndMessage);
                        }
                    }
                    msg.content = "received " + msg.content + " from " + msg.fromUrl;

              // swap urls for outgoing message
              Util.swapUrls(ref msg);

#if (TEST_WPFCLIENT)
              //int count = 0;
              //      for (int i = 0; i < 5; ++i)
              //      {
              //          Message testMsg = new Message();
              //          testMsg.toUrl = msg.toUrl;
              //          testMsg.fromUrl = msg.fromUrl;
              //          testMsg.content = String.Format("test message #{0}", ++count);
              //          Console.Write("\n  sending testMsg: {0}", testMsg.content);
              //          sndr.sendMessage(testMsg);
              //      }
#else
          /////////////////////////////////////////////////
          // Use the statement below for normal operation
          sndr.sendMessage(msg);
#endif
          }
            };

            if (rcvr.StartService())
            {
                rcvr.doService(serviceAction); // This serviceAction is asynchronous,
            }                                // so the call doesn't block.
            Util.waitForUser();
        }

        private static void process1(Message msg, PackageElement check)
        {
            check.PackageID = msg.package.PackageID;//msg.packageList[i].PackageID;/
            check.Category = msg.package.Category;//msg.packageList[i].Category;//
            check.Description = msg.package.Description; //msg.packageList[i].Description;//
            check.Query = msg.package.Query; //msg.packageList[i].Query;//
            check.Name = msg.package.Name;//msg.packageList[i].Name;//
            check.Author = msg.package.Author;//msg.packageList[i].Author;//msg.package.Author;
            check.Country = msg.package.Country;//msg.packageList[i].Country;//msg.package.Country;
            check.Time = msg.package.Time;// msg.packageList[i].Time;//msg.package.Time;
            check.Time2 = msg.package.Time2;//msg.packageList[i].Time2;//msg.package.Time2;
            check.Payload = msg.package.Payload.ToString(); //msg.packageList[i].Payload.ToString();//
            check.children = msg.package.children; //msg.packageList[i].children;//
            check.searchMeta = msg.package.searchMeta.ToString();
            check.searchTime = msg.package.searchTime;
        }

        public bool start_server()
        {
            
            database_Live = load_back.LoadXML2Object(database_Live, @"Package_details_persist.xml", database_clone);
            return true;
        }
        public bool persist_server()
        {
            database_Live.clone_dbstore(@"Package_details_persist.xml", database_clone);
            return true;
        }
        public void processDBQuery(PackageElement list, DBElement<string, string> list_clone, Message msg, Server srvr, Sender sndr)
        {
            if(list.Query == "Insert"|| list.Query == "Delete"|| list.Query == "Modify"|| list.Query == "Query")
            {
                processDBQuery1(list, list_clone, msg);
            }
            if (list.Query == "ChildQuery" || list.Query == "QuerysetKey" || list.Query == "TimeStampKey" || 
                list.Query == "QueryKeyMetadata"|| list.Query == "Persist" || list.Query == "Restore")
            {
                 if (list.Query == "ChildQuery")
                {
                    DBElement<string, string> cust4 = new DBElement<string, string>();
                    database_Live.getValue(list.PackageID, out cust4);
                    msg.childList = cust4.children.ToList();
                    msg.package = cust4;
                    msg.query = "ChildQuery";
                    msg.content = "Child Query for : " + list.PackageID.ToString();
                }
                else if (list.Query == "QuerysetKey")
                {
                    msg.keyList = database_Live.getSetKeys(list.PackageID, false);
                    msg.query = "QuerysetKey";
                    msg.content = "QuerysetKey Query for : " + list.PackageID;

                }
                else if (list.Query == "TimeStampKey")
                {
                    msg.keyList = database_Live.getKeysWithTimeStampRange(list.Time);
                    msg.query = "TimeStampKey";
                    msg.content = "TimeStampKey Query for : " + list.Time.ToString();
                }
                else if (list.Query == "QueryKeyMetadata")
                {

                    msg.keyList = database_Live.getKeyswithStringValues(list.searchMeta.ToString());
                    msg.query = "QueryKeyMetadata";
                    msg.content = "QueryKeyMetadata Query for : " + list.searchMeta;
                }
                else if (list.Query == "Persist")
                {
                    database_clone.PersistDB(@"Package_details_persist.xml");
                    msg.content = "Database Persisted";
                    msg.query = "Persist";
                }
                else if (list.Query == "Restore")
                {   srvr.start_server();
                    msg.query = "Restore"; msg.content = "Database Persisted";
                }
            }
        }

        private void processDBQuery1(PackageElement list, DBElement<string, string> list_clone, Message msg)
        {
            if (list.Query == "Insert")
            {
                database_Live.insert(list.PackageID, list_clone);
                database_clone.insert(list.PackageID, list);
                msg.query = "Insert";
                msg.content = "Insert for :" + list.PackageID;
            }
            else if (list.Query == "Delete")
            {
                database_Live.Delete(list.PackageID);
                database_clone.Delete(list.PackageID);
                msg.query = "Delete";
                msg.content = "Deletefor :" + list.PackageID;
            }
            else if (list.Query == "Modify")
            {
                database_Live.Modify(list.PackageID, list_clone);
                database_clone.Modify(list.PackageID, list);
                msg.query = "Modify";
                msg.content = "Modify for :" + list.PackageID;
            }
            else if (list.Query == "Query")
            {
                DBElement<string, string> cust4 = new DBElement<string, string>();
                database_Live.getValue(list.PackageID, out cust4);
                msg.package = cust4;
                msg.query = "Query";
                msg.content = "Query for : " + list.PackageID;
            }
        }

        public void create_list_clone(List<DBElement<string, string>> packageList)
        {
            PackageElement pck_elem = new PackageElement();
            for (var i = 0; i < packageList.Count; i++)
            {
                pck_elem.PackageID = packageList[i].PackageID;
                pck_elem.Category = packageList[i].Category;
                pck_elem.Description = packageList[i].Description;
                pck_elem.Query = packageList[i].Query;
                pck_elem.Name = packageList[i].Name;
                pck_elem.Author = packageList[i].Author;
                pck_elem.Country = packageList[i].Country;
                pck_elem.Time = packageList[i].Time;
                pck_elem.Time2 = packageList[i].Time2;
                pck_elem.Payload = packageList[i].Payload.ToString();
                pck_elem.children = packageList[i].children;
                packageList_clone.Add(pck_elem);
                //i++;
            }
        }
        public void create_listdb_clone(List<DBElement<string, string>> packageList)
        {
            DBElement<string, string> pck_elem = new DBElement<string, string>();
            //List<DBElement<string, string>> packageList_clone = new List<DBElement<string, string>>();
            for (var i = 0; i < packageList.Count; i++)
            {
                pck_elem.PackageID = packageList[i].PackageID;
                pck_elem.Category = packageList[i].Category;
                pck_elem.Description = packageList[i].Description;
                pck_elem.Query = packageList[i].Query;
                pck_elem.Name = packageList[i].Name;
                pck_elem.Author = packageList[i].Author;
                pck_elem.Country = packageList[i].Country;
                pck_elem.Time = packageList[i].Time;
                pck_elem.Time2 = packageList[i].Time2;
                pck_elem.Payload = packageList[i].Payload.ToString();
                pck_elem.children = packageList[i].children;
                dbelement_clone.Add(pck_elem);
            }
        }
    }
}
