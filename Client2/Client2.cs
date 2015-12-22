
//////////////////////////////////////////////////////////////
// Cleint2.cs - define noSQL database                       //
// Ver 1.1                                                  //
// Application: Project2 for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Thinkpad T540p, Core-i7, Windows 10            //
// Author:      Ravichandra Malapati, CST 4-187, Syracuse University  //
//              (315) 706-3437, rmalapat@syr.edu            //
//Initial Release 21-Oct-2015//
//  Author: Ravichandra Malapati      Source: Dr: JimFawcett                            //
///////////////////////////////////////////////////////////////

/*
This is the client which acts as write client to insert any elements. 
Public Interfaces:
no public interfaces defined
interfaces inported are as follows:
read_ip_client()
HiResTimer()
Sender()
Receiver()
CommService()
WPFCommunication client()
*/

/////////////////////////////////////////////////////////////////////////
// Client2.cs - CommService client sends and receives messages         //
// ver 2.1                                                             //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added using System.Threading
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - in this incantation the client has Sender and now has Receiver to
 *   retrieve Server echo-back messages.
 * - If you provide command line arguments they should be ordered as:
 *   remotePort, remoteAddress, localPort, localAddress
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.1 : 29 Oct 2015
 * - fixed bug in processCommandLine(...)
 * - added rcvr.shutdown() and sndr.shutDown() 
 * ver 2.0 : 20 Oct 2015
 * - replaced almost all functionality with a Sender instance
 * - added Receiver to retrieve Server echo messages.
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Project4Starter
{
    using Util = Utilities;

    ///////////////////////////////////////////////////////////////////////
    // Client class sends and receives messages in this version
    // - commandline format: /L http://localhost:8085/CommService 
    //                       /R http://localhost:8080/CommService
    //   Either one or both may be ommitted
    /// <summary>
    /// class for demonstrating the project 2 requirments
    /// 
    /// </summary>
    class Client
    {
        HiResTimer d = new HiResTimer();
        read_ip_client ip_client = new read_ip_client();
        string localUrl { get; set; } = "http://localhost:8082/CommService";
        string remoteUrl { get; set; } = "http://localhost:8080/CommService";
        string remoteUrlWpf = "http://localhost:8089/CommService";
        //----< retrieve urls from the CommandLine if there are any >--------
        bool display_process_query = true;
        bool loggingCheck { get; set; } = true;
        public void processCommandLine(string[] args)
        {
            if (args.Length == 0)
                return;
            bool logging = true;
            localUrl = Util.processCommandLineForLocal(args, localUrl, ref logging);
            loggingCheck = logging;
           // remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
        }
        static void Main(string[] args)
        {
            Console.Write("\n  starting CommService client");
            Console.Write("\n =============================\n");

            Console.Title = "Write Client";

            Client clnt = new Client();
            clnt.processCommandLine(args);
            //Sender WpfSender = new Sender(clnt.remoteUrlWpf);
            Sender sndr = new Sender(clnt.localUrl);
            string localPort = Util.urlPort(clnt.localUrl);
            string localAddr = Util.urlAddress(clnt.localUrl);
            Receiver rcvr = new Receiver(localPort, localAddr);
            Action serviceAction = () =>
            {
                if (Util.verbose)
                    Console.Write("\n  starting Receiver.defaultServiceAction");
                Message msg1 = null;
                while (true)
                {
                    msg1 = rcvr.getMessage();   // note use of non-service method to deQ messages
                    Console.Write("\n  Received message:");
                    Console.Write("\n  sender is {0}", msg1.fromUrl);

                    if (msg1.content != "DBwork")
                        Console.Write("\n  The sender says {0}, {1}\n", msg1.content, msg1.query);
                    if (msg1.content == "done")
                    {
                        Client_Handling_forWrite(clnt, sndr, msg1);
                    }
                    clnt.serverProcessMessage(msg1);
                    if (msg1.content == "closeReceiver")
                        break;
                }
            };
            if (rcvr.StartService())
            {
                rcvr.doService(serviceAction);
            }
            Client_Handling1(clnt, sndr, rcvr);
        }

        private static void Client_Handling1(Client clnt, Sender sndr, Receiver rcvr)
        {
            Message msg = new Message();
            Message testmsg = new Message();
            msg.fromUrl = clnt.localUrl;
            msg.toUrl = clnt.remoteUrl;
            testmsg.toUrl = clnt.remoteUrl;
            testmsg.fromUrl = clnt.localUrl;
            Console.Write("\n  sender's url is {0}", msg.fromUrl);
            Console.Write("\n  attempting to connect to {0}\n", msg.toUrl);

            if (!sndr.Connect(msg.toUrl))
            {
                Console.Write("\n  could not connect in {0} attempts", sndr.MaxConnectAttempts);
                sndr.shutdown();
                rcvr.shutDown();
                return;
            }
            Console.Write("\n =============================\n");
            Console.Write("\n Reading input from XML provided by the user\n");
            testmsg.packageList = clnt.ip_client.ReadInput_client(@"Addition.xml");
            testmsg.content = "DBwork";
            Console.WriteLine("starting timer............");
            clnt.d.Start();
            //clnt.TestR1(msg, testmsg, sndr);
            clnt.TestR2(msg, testmsg, sndr);
            //clnt.TestR2(msg, testmsg, sndr);
            //clnt.TestR3(msg, testmsg, sndr);

            msg.content = "done";
            sndr.sendMessage(msg);

            // Wait for user to press a key to quit.
            // Ensures that client has gotten all server replies.
            Util.waitForUser();
            rcvr.shutDown();
            sndr.shutdown();
            Console.Write("\n\n");
        }

        private static void Client_Handling_forWrite(Client clnt, Sender sndr, Message msg1)
        {
            clnt.d.Stop();
            Console.WriteLine("====================================================");
            Console.WriteLine("The elapsed time is :{0} microseconds", clnt.d.ElapsedMicroseconds);
            Console.WriteLine("The elapsed time is :{0} seconds", clnt.d.ElapsedMicroseconds / 1000000);
            Console.WriteLine("====================================================");
            Message wpfmsg = new Message();
            wpfmsg.fromUrl = msg1.toUrl;
            wpfmsg.toUrl = clnt.remoteUrlWpf;
            wpfmsg.content = "The time taken for client " + msg1.toUrl + "is : " + clnt.d.ElapsedMicroseconds / 1000000;
            wpfmsg.content = "The time taken for client " + msg1.toUrl + "is : " + clnt.d.ElapsedMicroseconds / 1000000 + "seconds" + " \n Throughput is :  " + clnt.d.ElapsedMicroseconds / 100000 + "micro seconds";
            sndr.sendMessage(wpfmsg);
        }

        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void TestR1(Message msg, Message testmsg, Sender sndr)
        {
            int counter = 0;
            Console.WriteLine("==================================================");
            Console.WriteLine("Demonstrating Requirmetn1");
            Console.WriteLine("==================================================");
            testmsg.packageList = ip_client.ReadInput_client(@"TestR1.xml");
            Console.WriteLine("Starting and Restoring the remote server from XML stored file");
            Console.WriteLine("Persisting the database Please check the Server console");
            while (true)
            {
                msg.content = "DBwork";
                msg.package = testmsg.packageList[counter];
                msg.query = testmsg.packageList[counter].Query;
                Console.Write("\n  sending {0}", msg.content);
                Console.Write("\n sending query {0}", msg.query);
                for (var i = 0; i < msg.package.count; i++)
                {
                    Console.WriteLine(" \ncount is : {0}", i);
                    if (!sndr.sendMessage(msg))
                        return;
                }
                //if (!sndr.sendMessage(msg))
                //  return;
                Thread.Sleep(100);
                counter++;
                if (counter >= testmsg.packageList.Count)
                    break;
            }
            Console.WriteLine("**************************************************");
            Console.WriteLine("Demonstrating Requirmetn1 ends here");
            Console.WriteLine("**************************************************");

        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void TestR2(Message msg, Message testmsg, Sender sndr)
        {
            int counter = 0;
            Console.WriteLine("==================================================");
            Console.WriteLine("Demonstrating Requirmetn2");
            Console.WriteLine("==================================================");
            testmsg.packageList = ip_client.ReadInput_client(@"TestR2.xml");
            Console.WriteLine("Sending and Receiving the data from server");
            while (true)
            {
                msg.content = "DBwork";
                msg.package = testmsg.packageList[counter];
                msg.query = testmsg.packageList[counter].Query;
                Console.Write("\n  sending {0}", msg.content);
                Console.Write("\n sending query {0}", msg.query);
                for (var i = 0; i < msg.package.count; i++)
                {
                    Console.WriteLine(" \ncount is : {0}", i);
                    if (!sndr.sendMessage(msg))
                        return;
                }
                //if (!sndr.sendMessage(msg))
                //  return;
                Thread.Sleep(100);
                counter++;
                if (counter >= testmsg.packageList.Count)
                    break;
            }
            Console.WriteLine("**************************************************");
            Console.WriteLine("Demonstrating Requirmetn2 ends here");
            Console.WriteLine("**************************************************");
        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void TestR3(Message msg, Message testmsg, Sender sndr)
        {
            int counter = 0;
            Console.WriteLine("==================================================");
            Console.WriteLine("Demonstrating Requirmetn3");
            Console.WriteLine("==================================================");
            testmsg.packageList = ip_client.ReadInput_client(@"TestR3.xml");
            while (true)
            {
                msg.content = "DBwork";
                msg.package = testmsg.packageList[counter];
                msg.query = testmsg.packageList[counter].Query;
                Console.Write("\n  sending {0}", msg.content);
                Console.Write("\n sending query {0}", msg.query);
                for (var i = 0; i < msg.package.count; i++)
                {
                    Console.WriteLine(" \ncount is : {0}", i);
                    if (!sndr.sendMessage(msg))
                        return;
                }
                //if (!sndr.sendMessage(msg))
                //  return;
                Thread.Sleep(100);
                counter++;
                if (counter >= testmsg.packageList.Count)
                    break;
            }
            Console.WriteLine("**************************************************");
            Console.WriteLine("Demonstrating Requirmetn3 ends here");
            Console.WriteLine("**************************************************");
        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void TestR4(Message msg, Message testmsg, Sender sndr)
        {
            int counter = 0;
            Console.WriteLine("==================================================");
            Console.WriteLine("Demonstrating Requirmetn4");
            Console.WriteLine("==================================================");
            testmsg.packageList = ip_client.ReadInput_client(@"TestR4.xml");
            while (true)
            {
                msg.content = "DBwork";
                msg.package = testmsg.packageList[counter];
                msg.query = testmsg.packageList[counter].Query;
                Console.Write("\n  sending {0}", msg.content);
                Console.Write("\n sending query {0}", msg.query);
                for (var i = 0; i < msg.package.count; i++)
                {
                    Console.WriteLine(" \ncount is : {0}", i);
                    if (!sndr.sendMessage(msg))
                        return;
                }
                //if (!sndr.sendMessage(msg))
                //  return;
                Thread.Sleep(100);
                counter++;
                if (counter >= testmsg.packageList.Count)
                    break;
            }
            Console.WriteLine("**************************************************");
            Console.WriteLine("Demonstrating Requirmetn4 ends here");
            Console.WriteLine("**************************************************");
        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void TestR5(Message msg, Message testmsg, Sender sndr)
        {
            int counter = 0;
            Console.WriteLine("==================================================");
            Console.WriteLine("Demonstrating Requirmetn5");
            Console.WriteLine("==================================================");
            testmsg.packageList = ip_client.ReadInput_client(@"TestR5.xml");
            while (true)
            {
                msg.content = "DBwork";
                msg.package = testmsg.packageList[counter];
                msg.query = testmsg.packageList[counter].Query;
                Console.Write("\n  sending {0}", msg.content);
                Console.Write("\n sending query {0}", msg.query);
                for (var i = 0; i < msg.package.count; i++)
                {
                    Console.WriteLine(" \ncount is : {0}", i);
                    if (!sndr.sendMessage(msg))
                        return;
                }
                //if (!sndr.sendMessage(msg))
                //  return;
                Thread.Sleep(100);
                counter++;
                if (counter >= testmsg.packageList.Count)
                    break;
            }
            Console.WriteLine("**************************************************");
            Console.WriteLine("Demonstrating Requirmetn5 ends here");
            Console.WriteLine("**************************************************");
        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void TestR6(Message msg, Message testmsg, Sender sndr)
        {
            int counter = 0;
            Console.WriteLine("==================================================");
            Console.WriteLine("Demonstrating Requirmetn6");
            Console.WriteLine("==================================================");
            testmsg.packageList = ip_client.ReadInput_client(@"TestR6.xml");
            while (true)
            {
                msg.content = "DBwork";
                msg.package = testmsg.packageList[counter];
                msg.query = testmsg.packageList[counter].Query;
                Console.Write("\n  sending {0}", msg.content);
                Console.Write("\n sending query {0}", msg.query);
                for (var i = 0; i < msg.package.count; i++)
                {
                    Console.WriteLine(" \ncount is : {0}", i);
                    if (!sndr.sendMessage(msg))
                        return;
                }
                //if (!sndr.sendMessage(msg))
                //  return;
                Thread.Sleep(100);
                counter++;
                if (counter >= testmsg.packageList.Count)
                    break;
            }
            Console.WriteLine("**************************************************");
            Console.WriteLine("Demonstrating Requirmetn6 ends here");
            Console.WriteLine("**************************************************");
        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void TestR7(Message msg, Message testmsg, Sender sndr)
        {
            int counter = 0;
            Console.WriteLine("==================================================");
            Console.WriteLine("Demonstrating Requirmetn7");
            Console.WriteLine("==================================================");
            testmsg.packageList = ip_client.ReadInput_client(@"TestR7.xml");
            while (true)
            {
                msg.content = "DBwork";
                msg.package = testmsg.packageList[counter];
                msg.query = testmsg.packageList[counter].Query;
                Console.Write("\n  sending {0}", msg.content);
                Console.Write("\n sending query {0}", msg.query);
                for (var i = 0; i < msg.package.count; i++)
                {
                    Console.WriteLine(" \ncount is : {0}", i);
                    if (!sndr.sendMessage(msg))
                        return;
                }
                //if (!sndr.sendMessage(msg))
                //  return;
                Thread.Sleep(100);
                counter++;
                if (counter >= testmsg.packageList.Count)
                    break;
            }
            Console.WriteLine("**************************************************");
            Console.WriteLine("Demonstrating Requirmetn7 ends here");
            Console.WriteLine("**************************************************");
        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void TestR8(Message msg, Message testmsg, Sender sndr)
        {
            int counter = 0;
            Console.WriteLine("==================================================");
            Console.WriteLine("Demonstrating Requirmetn8");
            Console.WriteLine("==================================================");
            testmsg.packageList = ip_client.ReadInput_client(@"TestR7.xml");
            while (true)
            {
                msg.content = "DBwork";
                msg.package = testmsg.packageList[counter];
                msg.query = testmsg.packageList[counter].Query;
                Console.Write("\n  sending {0}", msg.content);
                Console.Write("\n sending query {0}", msg.query);
                for (var i = 0; i < msg.package.count; i++)
                {
                    Console.WriteLine(" \ncount is : {0}", i);
                    if (!sndr.sendMessage(msg))
                        return;
                }
                //if (!sndr.sendMessage(msg))
                //  return;
                Thread.Sleep(100);
                counter++;
                if (counter >= testmsg.packageList.Count)
                    break;
            }
            Console.WriteLine("**************************************************");
            Console.WriteLine("Demonstrating Requirmetn8 ends here");
            Console.WriteLine("**************************************************");
        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void TestR9(Message msg, Message testmsg, Sender sndr)
        {
            int counter = 0;
            Console.WriteLine("==================================================");
            Console.WriteLine("Demonstrating Requirmetn9");
            Console.WriteLine("==================================================");
            testmsg.packageList = ip_client.ReadInput_client(@"TestR8.xml");
            while (true)
            {
                msg.content = "DBwork";
                msg.package = testmsg.packageList[counter];
                msg.query = testmsg.packageList[counter].Query;
                Console.Write("\n  sending {0}", msg.content);
                Console.Write("\n sending query {0}", msg.query);
                for (var i = 0; i < msg.package.count; i++)
                {
                    Console.WriteLine(" \ncount is : {0}", i);
                    if (!sndr.sendMessage(msg))
                        return;
                }
                //if (!sndr.sendMessage(msg))
                //  return;
                Thread.Sleep(100);
                counter++;
                if (counter >= testmsg.packageList.Count)
                    break;
            }
            Console.WriteLine("**************************************************");
            Console.WriteLine("Demonstrating Requirmetn9 ends here");
            Console.WriteLine("**************************************************");
        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual void serverProcessMessage(Message msg)
        {
            string strprint = "The request " + msg.query + "is successful";
            Console.WriteLine("{0}", strprint);
            if (msg.query == "Query")
            {
                Console.WriteLine("The query sent was:{0} for {1} ", msg.query, msg.package.PackageID);
                display_package(msg);
            }
            else if (msg.query == "ChildQuery")
            {
                Console.WriteLine("{0}", msg.content);
                display_children(msg);
            }
            else if (msg.query == "QuerysetKey")
            {
                Console.WriteLine("{0}", msg.content);
                display_keylist(msg);
            }
            else if (msg.query == "TimeStampKey")
            {
                Console.WriteLine("{0}", msg.content);
                display_keylist(msg);
            }
            else if (msg.query == "QueryKeyMetadata")
            {
                display_keylist(msg);
            }
            else if (msg.query == "Persist")
            {
                Console.WriteLine("The Persist is successful");
            }
            else if (msg.query == "Restore")
            {
                Console.WriteLine("The Restore is successful");
            }
            else if (msg.query == "info")
            {
                if (display_process_query)
                    Console.WriteLine("{0}", msg.content);

            }
        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void display_package(Message msg)
        {
            List<Child> child_test;
            child_test = msg.package.children.ToList();
            Console.WriteLine("The key is: {0}", msg.package.PackageID);
            Console.WriteLine("The Category is: {0}", msg.package.Category);
            Console.WriteLine("The Description is: {0}", msg.package.Description);
            Console.WriteLine("The Name is: {0}", msg.package.Name);
            Console.WriteLine("The Author is: {0}", msg.package.Author);
            Console.WriteLine("The Country is: {0}", msg.package.Country);
            Console.WriteLine("The Time is: {0}", msg.package.Time);
            Console.WriteLine("The Time2 is: {0}", msg.package.Time2);
            Console.WriteLine("The Payload is: {0}", msg.package.Payload);
            for (int i = 0; i < child_test.Count; i++)
                Console.WriteLine("priting childs of the Key Value pair : Key is {0}, one of the childs is {1}", msg.package.PackageID, child_test[i].ChildID);
        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void display_children(Message msg)
        {

            List<Child> child_test;
            child_test = msg.childList.ToList();
            for (int i = 0; i < child_test.Count; i++)
                Console.WriteLine("priting childs of the Key Value pair : Key is {0}, one of the childs is {1}", msg.package.PackageID, child_test[i].ChildID);

        }
        /// <summary>
        /// Function to execute the requirments
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="testmsg"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void display_keylist(Message msg)
        {
            if (msg.query == "ChildQuery")
            {
                for (int i = 0; i < msg.keyList.Count; i++)
                {
                    Console.WriteLine("One of the Key is having required criteria ChildQuery is: {0}", msg.keyList[i]);
                }
            }
            else if (msg.query == "QuerysetKey")
            {
                for (int i = 0; i < msg.keyList.Count; i++)
                {
                    Console.WriteLine("One of the Key is having required criteria QuerysetKey is: {0}", msg.keyList[i]);
                }
            }
            else if (msg.query == "TimeStampKey")
            {
                for (int i = 0; i < msg.keyList.Count; i++)
                {
                    Console.WriteLine("One of the Key is having required criteria TimeStampKey is: {0}", msg.keyList[i]);
                }
            }
            else if (msg.query == "QueryKeyMetadata")
            {
                for (int i = 0; i < msg.keyList.Count; i++)
                {
                    Console.WriteLine("One of the Key is having required criteria QueryKeyMetadata is: {0}", msg.keyList[i]);
                }
            }
            else
            {
                Console.WriteLine("your query does not matach any criteria");
            }
        }

    }
}

