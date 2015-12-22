

//////////////////////////////////////////////////////////////
// read input from the XML.cs - define noSQL database                       //
// Ver 1.1                                                  //
// Application: Project2 for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Thinkpad T540p, Core-i7, Windows 10            //
// Author:      Ravichandra Malapati, CST 4-187, Syracuse University  //
//              (315) 706-3437, rmalapat@syr.edu            //
//Initial Release 21-Oct-2015//
//  Author: Ravichandra Malapati                             //
///////////////////////////////////////////////////////////////

/*

This is used to read the XML file from the disk and pass to the clients. 
All the clients used this for reading XML purposes.
Public Interfaces:
public class read_ip_client();
interfaces inported are as follows:
No methods or classes are imported for this project. 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Project4Starter
{
    /// <summary>
    /// takes input and provides the list of XML elements as output
    /// </summary>
    /// <param name="input_path"></param>
    /// <returns></returns>
    public class read_ip_client
    {
        private static List<DBElement<string, string>> packageList;
        /// <summary>
        /// takes input and provides the list of XML elements as output
        /// </summary>
        /// <param name="input_path"></param>
        /// <returns></returns>
        public List<DBElement<string, string>> ReadInput_client(String input_path)
        {
            packageList = (
                   from e in XDocument.Load(input_path).
                             Root.Elements("PackageElement")
                   select new DBElement<string, string>
                   {
                       Category = (string)e.Element("Category"),
                       PackageID = (string)e.Element("PackageID"),
                       Description = (string)e.Element("Description"),
                       Query = (string)e.Element("Query"),
                       Name = (string)e.Element("Name"),
                       Author = (string)e.Element("Author"),
                       Country = (string)e.Element("Country"),
                       Time = (DateTime)e.Element("Time"),
                       Time2 = (DateTime)e.Element("Time2"),
                       Payload = (string)e.Element("Payload"),
                       count=(int)e.Element("count"),
                       searchMeta=(string)e.Element("searchMeta"),
                       
                       children = (
                           from o in e.Elements("children").Elements("Child")
                           select new Child
                           {
                               ChildID = (string)o.Element("ChildID"),
                               ChildDate = (DateTime)o.Element("ChildDate"),
                           })
                           .ToArray()


                   })
                   .ToList();

            return packageList;
        }



#if TEST_READIP
        static void Main(string[] args)
        {
            read_ip_client ip = new read_ip_client();
            string input_path = "@Additions.XML";
            ip.ReadInput_client(input_path);
        }
#endif
    }
}
