///////////////////////////////////////////////////////////////
// LoadXMLDB.cs - define noSQL database                       //
// Ver 1.1                                                  //
// Application: Project2 for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Thinkpad T540p, Core-i7, Windows 10            //
// Author:      Ravichandra Malapati, CST 4-187, Syracuse University  //
//              (315) 706-3437, rmalapat@syr.edu            //
//Initial Release 07-Oct-2015//
//  Author: Ravichandra Malapati  Source: Dr: JimFawcett      //
///////////////////////////////////////////////////////////////

/*
Public Interfaces:
public bool insert(Key key, Value val)
public bool getValue(Key key, out Value val)
public bool Delete(Key key)
public Value ChildQuery(Key key)
public bool Modify(Key key, Value val)
public void PersistDB(string input_path)
public List<string> getSetKeys(Key key1, bool default_or_nodata)
public List<string> getKeyswithStringValues(string value_input)
public List<string> getKeysWithTimeStampRange(DateTime time_Input)
public void createImmutableDatabase(Key key1)
public void clone_dbstore(string input_path, DBEngine<string, PackageElement> database_clone)
public IEnumerable<Key> Keys()

/*
 * Package Operations:
 * -------------------
 * This package implements DBEngine<Key, Value> where Value
 * is the DBElement<key, Data> type.
 *
 * This class is a starter for the DBEngine package you need to create.
 * It doesn't implement many of the requirements for the db, e.g.,
 * It doesn't remove elements, doesn't persist to XML, doesn't retrieve
 * elements from an XML file, and it doesn't provide hook methods
 * for scheduled persistance.
 */

///////////////////////////////////////////////////////////////
// LoadXMLDB.cs - define noSQL database                       //
// Ver 1.0                                                  //
// Application: Project2 for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo Thinkpad T540p, Core-i7, Windows 10            //
// Author:      Ravichandra Malapati, CST 4-187, Syracuse University  //
//              (315) 706-3437, rmalapat@syr.edu            //
// Initial release 07-October-2015
// Author: Ravichandra Malapati                             //
///////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////
// DBEngine.cs - define noSQL database                       //
// Ver 1.2                                                   //
// Application: Demonstration for CSE687-OOD, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Dell XPS2700, Core-i7, Windows 10            //
// Original Author:      Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements DBEngine<Key, Value> where Value
 * is the DBElement<key, Data> type.
 Public interfaces:

 *
 */
/*
* This class is a starter for the DBEngine package you need to create.
* It doesn't implement many of the requirements for the db, e.g.,
* It doesn't remove elements, doesn't persist to XML, doesn't retrieve
* elements from an XML file, and it doesn't provide hook methods
* for scheduled persistance.
*/
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, and
 *                 UtilityExtensions.cs only if you enable the test stub
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.2 : 24 Sep 15
 * - removed extensions methods and tests in test stub
 * - testing is now done in DBEngineTest.cs to avoid circular references
 * ver 1.1 : 15 Sep 15
 * - fixed a casting bug in one of the extension methods
 * ver 1.0 : 08 Sep 15
 * - first release
 *
 */
//todo add placeholders for Shard
//todo add reference to class text XML content

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Threading;

namespace Project4Starter
{
    /// <summary>
    /// Class for DBEngine which exposes various methods for 
    /// adding and editing elements,removing elements, geting value for the specific key,
    /// to retreive all keys and all values
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam
    public class DBEngine<Key, Value>
    {
        private Dictionary<Key, Value> dbStore;
        public DBEngine()
        {
            dbStore = new Dictionary<Key, Value>();
        }
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool insert(Key key, Value val)
        {
            if (dbStore.Keys.Contains(key))
            {
                //Console.WriteLine("The database already contains the key {0} you are trying to insert, Please use modify", key);
                return false;
            }
            else
            {
                //Console.WriteLine("Please wait while the key {0} is being inserted in database ", key);
                dbStore[key] = val;
                Console.WriteLine("key {0} is inserted", key);
                return true;
            }

        }
        /*To get the value from the database*/
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool getValue(Key key, out Value val)
        {
            if (dbStore.Keys.Contains(key))
            {
                val = dbStore[key];
                return true;
            }
            else
            {
                //Console.WriteLine("The database doesnot contain the key {0} you are requesting", key);
                val = default(Value);
                return false;
            }

        }

        /*To delete the data from the database*/
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Delete(Key key)
        {
            if (dbStore.Keys.Contains(key))
            {
                dbStore.Remove(key);
                //Console.WriteLine("The Key {0} is removed succesfully from the database", key);
                return true;
            }
            else
            {
                //Console.WriteLine("The key is not present in database, could you please recheck and Query");
                return false;
            }

        }
        /*To query the childs of a key data into the database*/
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Value ChildQuery(Key key)
        {
            if (dbStore.Keys.Contains(key))
            {
                //Console.WriteLine("Please wait while the key {0} is being Queried from database ", key);
                Value val = dbStore[key];
                return val;
                
            }
            else
            {
                // Console.WriteLine("The database doesnot contain the key {0} you are requesting", key);
                Value val = default(Value);
                return val;
            }

        }
        /*To modify the values and metadata of a key data into the database*/
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Modify(Key key, Value val)
        {
            if (dbStore.Keys.Contains(key))
            {
                //Console.WriteLine("Please wait while the key {0} is being modified in the database ", key);
                dbStore[key] = val;
                return true;
            }
            else
            {
                //Console.WriteLine("The database doesnot contain the key {0} you are requesting", key);
                return false;
            }

        }
        /*To persist the database into XML */
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public void PersistDB(string input_path)
        {
            string xmlString = null;
            var fields = dbStore.Values.ToList();
            Console.WriteLine("Persisting The Database..... Please wait");
            Thread.Sleep(2000);
            XmlSerializer xmlSerializer = new XmlSerializer(fields.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, fields);
                memoryStream.Position = 0;
                xmlString = new StreamReader(memoryStream).ReadToEnd();
            }
            XElement xElement = XElement.Parse(xmlString);
            xElement.Save(input_path);
        }
        /*To Query a set of keys based on the patter matching*/
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public List<string> getSetKeys(Key key1, bool default_or_nodata)
        {
            List<string> keyList = new List<string>();
            DBElement<string, string> item = new DBElement<string, string>();
            foreach (var i in dbStore.Keys)
            {
                if (default_or_nodata)
                {
                    Value v = dbStore[i];
                    item = v as DBElement<string, string>;
                    keyList.Add(item.PackageID);
                    //Console.WriteLine("The key is: {0}", item.PackageID);
                }
                else if (i.ToString().Contains(key1.ToString()))
                {
                    Value v = dbStore[i];
                    
                    item = v as DBElement<string, string>;
                    keyList.Add(item.PackageID.ToString());
                    Console.WriteLine("The key is: {0}", item.PackageID);
                }
            }
            return keyList;
        }
        /*To Query a set of keys based on the patter matching in Values either in metadata or payload*/
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public List<string> getKeyswithStringValues(string value_input)
        {
            List<string> keyList = new List<string>();
            DBElement<string, string> item = new DBElement<string, string>();
            foreach (var i in dbStore.Keys)
            {
                Value v = dbStore[i];
                item = v as DBElement<string, string>;
                if (item.Author.Contains(value_input) || item.Description.Contains(value_input) || item.Query.Contains(value_input)
                    || item.Name.Contains(value_input) || item.Country.Contains(value_input) || item.Payload.ToString().Contains(value_input))
                {
                    Console.WriteLine("The customer Id is: {0}", item.PackageID);
                    keyList.Add(item.PackageID);
                }
                
            }
            return keyList;
        }
        /*to Query the keys whose date time stamps are in a range*/
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public List<string> getKeysWithTimeStampRange(DateTime time_Input)
        {
            List<string> listKeys = new List<string>();
            DBElement<string, string> cust_item = new DBElement<string, string>();
            foreach (var i in dbStore.Keys)
            {
                Value val = dbStore[i];
                cust_item = val as DBElement<string, string>;
                int result_compare = DateTime.Compare(time_Input, cust_item.Time);
                if (result_compare <= 0)
                {
                    listKeys.Add(cust_item.PackageID);
                    Console.WriteLine("time stamp of the key {0} is between the input time stamp  which is {1} and present time which is {2}", cust_item.PackageID, time_Input, cust_item.Time);
                }
                    
            }
            return listKeys;
        }
        /*to create a immutable database*/
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public void createImmutableDatabase(Key key1)
        {
            var desiredResults = new Dictionary<Key, string>();

            Console.WriteLine("Creating immutable database ======\n");
            DBElement<string, string> item = new DBElement<string, string>();
            foreach (var i in dbStore.Keys)
            {
                if (i.ToString().Contains(key1.ToString()))
                {
                    Value v = dbStore[i];
                    item = v as DBElement<string, string>;
                    Console.WriteLine("Adding key {0} to immutable database:", item.PackageID);
                    desiredResults.Add(i, item.Name);
                    Console.WriteLine("he key in immutable database is: {0} ", item.PackageID);


                }
            }
            Console.WriteLine("The database can not be edited and it is immutable database......");
        }
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public void clone_dbstore(string input_path, DBEngine<string, PackageElement> database_clone)
        {

            foreach (Key x in dbStore.Keys)//(KeyValuePair<Key, Value> entry in dbStore)
            {
                string key1 = x.ToString();
                int i = 0;
                PackageElement value1 = new PackageElement();
                DBElement<string, string> value_o = dbStore[x] as DBElement<string, string>;
                value1.PackageID = value_o.PackageID;
                value1.Category = value_o.Category;
                value1.Description = value_o.Description;
                value1.Query = value_o.Query;
                value1.Name = value_o.Name;
                value1.Author = value_o.Author;
                value1.Country = value_o.Country;
                value1.Time = value_o.Time;
                value1.Time2 = value_o.Time2;
                value1.Payload = value_o.Payload;
                value1.Payload = value_o.Payload;
                foreach (var y in value_o.children)
                {
                    value1.children[i].ChildID = value_o.children[i].ChildID;
                    i++;
                }


                database_clone.insert(key1, value1);
                // do something with entry.Value or entry.Key
            }
            database_clone.PersistDB(input_path);
        }
        /*To insert data into the database*/
        /// <summary>
        ///Database operation function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public IEnumerable<Key> Keys()
        {
            return dbStore.Keys;
        }

        /*
         * More functions to implement here
         */
    }

#if (TEST_DBENGINE)

    class TestDBEngine
  {
    static void Main(string[] args)
    {
         
        {
                Write("\n  All testing of DBEngine class moved to DBEngineTest package.");
                Write("\n  This allow use of Utilty package without circular dependencies.");
                Write("\n\n");
            }
        }
  }
#endif
}
