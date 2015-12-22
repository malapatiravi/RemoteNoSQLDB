///////////////////////////////////////////////////////////////
// DBEngineTest.cs - Test DBEngine and DBExtensions          //
// Ver 2.0                                                   //
// Application: Demonstration for CSE687-OOD, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Dell XPS2700, Core-i7, Windows 10            //
// Author:      Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package replaces DBEngine test stub to remove
 * circular package references.
 *
 * Now this testing depends on the class definitions in DBElement,
 * DBEngine, and the extension methods defined in DBExtensions.
 * We no longer need to define extension methods in DBEngine.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBEngineTest.cs,  DBElement.cs, DBEngine.cs,  
 *   DBExtensions.cs, UtilityExtensions.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 2.0 : 03 Oct 15
 * - replaced all payloads of type string and List<string>
 *   with PL_String and PL_ListOfString
 * ver 1.0 : 24 Sep 15
 * - first release
 *
 */
 //ToDo: 3 - add tests for remove(key) and clear()
 //ToDo: 4 - move test for ToXml<Data>() from DBExtensions to here

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project2Starter
{
  class Program
  {
    static void Main(string[] args)
    {
      "Testing DBEngine Package".title('=');
      WriteLine();

      Write("\n --- Test DBElement<int,PL_String> ---");
      DBElement<int, PL_String> elem1 = new DBElement<int, PL_String>();
      elem1.payload = new PL_String("a payload");
      Write(elem1.showElement<int, PL_String>());
      WriteLine();

      DBElement<int, PL_String> elem2 = new DBElement<int, PL_String>("Darth Vader", "Evil Overlord");
      elem2.payload = new PL_String("The Empire strikes back!");
      Write(elem2.showElement<int, PL_String>());
      WriteLine();

      var elem3 = new DBElement<int, PL_String>("Luke Skywalker", "Young HotShot");
      elem3.children.AddRange(new List<int> { 1, 5, 23 });
      elem3.payload = new PL_String("X-Wing fighter in swamp - Oh oh!");
      Write(elem3.showElement<int, PL_String>());
      WriteLine();

      Write("\n --- Test DBEngine<int,DBElement<int,PL_String>> ---");

      int key = 0;
      Func<int> keyGen = () => { ++key; return key; };  // anonymous function to generate keys

      DBEngine<int, DBElement<int, PL_String>> db = new DBEngine<int, DBElement<int, PL_String>>();
      bool p1 = db.insert(keyGen(), elem1);
      bool p2 = db.insert(keyGen(), elem2);
      bool p3 = db.insert(keyGen(), elem3);
      if (p1 && p2 && p3)
        Write("\n  all inserts succeeded");
      else
        Write("\n  at least one insert failed");
      db.showDB<int, DBElement<int, PL_String>, PL_String>();
      WriteLine();

      //db.toXml<PL_String>();

      Write("\n --- Test DBElement<string,PL_ListOfStrings> ---");
      DBElement<string, PL_ListOfStrings> newelem1 = new DBElement<string, PL_ListOfStrings>();
      newelem1.name = "newelem1";
      newelem1.descr = "test new type";
      newelem1.payload = new PL_ListOfStrings(new List<string> { "one", "two", "three" });
      Write(newelem1.showElement<string, PL_ListOfStrings>());
      WriteLine();

      Write("\n --- Test DBElement<string,PL_ListOfStrings> ---");
      DBElement<string, PL_ListOfStrings> newerelem1 = new DBElement<string, PL_ListOfStrings>();
      newerelem1.name = "newerelem1";
      newerelem1.descr = "better formatting";
      newerelem1.payload = new PL_ListOfStrings(new List<string> { "alpha", "beta", "gamma" });
      newerelem1.payload.theWrappedData.Add("delta");
      newerelem1.payload.theWrappedData.Add("epsilon");
      Write(newerelem1.showElement<string, PL_ListOfStrings>());
      WriteLine();

      DBElement<string, PL_ListOfStrings> newerelem2 = new DBElement<string, PL_ListOfStrings>();
      newerelem2.name = "newerelem2";
      newerelem2.descr = "better formatting";
      newerelem1.children.AddRange(new[] { "first", "second" });
      newerelem2.payload = new PL_ListOfStrings(new List<string> { "a", "b", "c" });
      newerelem2.payload.theWrappedData.Add("d");
      newerelem2.payload.theWrappedData.Add("e");
      Write(newerelem2.showElement<string, PL_ListOfStrings>());
      WriteLine();

      Write("\n --- Test DBEngine<string,DBElement<string,PL_ListOfStrings>> ---");

      int seed = 0;
      string skey = seed.ToString();
      Func<string> skeyGen = () => {
        ++seed;
        skey = "string" + seed.ToString();
        skey = skey.GetHashCode().ToString();
        return skey;
      };

      DBEngine<string, DBElement<string, PL_ListOfStrings>> newdb =
        new DBEngine<string, DBElement<string, PL_ListOfStrings>>();
      newdb.insert(skeyGen(), newerelem1);
      newdb.insert(skeyGen(), newerelem2);
      newdb.showDB<string, DBElement<string, PL_ListOfStrings>, PL_ListOfStrings>();
      WriteLine();

      "testing edits".title();
      db.showDB<int, DBElement<int, PL_String>, PL_String>();
      DBElement<int, PL_String> editElement = new DBElement<int, PL_String>();
      db.getValue(1, out editElement);
      editElement.showElement<int, PL_String>();
      editElement.name = "editedName";
      editElement.descr = "editedDescription";
      db.showDB<int, DBElement<int, PL_String>, PL_String>();
      Write("\n\n");
    }
  }
}
