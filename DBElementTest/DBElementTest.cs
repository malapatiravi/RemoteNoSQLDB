///////////////////////////////////////////////////////////////
// DBElementTest.cs - Test DBElement and DBExtensions        //
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
 * This package replaces DBElement test stub to remove
 * circular package references.
 *
 * Now this testing depends on the class definitions in DBElement
 * and the extension methods defined in DBExtensions.  We no longer
 * need to define extension methods in DBElement.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBElementTest.cs,  DBElement.cs, 
 *   DBExtensions.cs, UtilityExtensions.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 2.0 : 03 Oct 15
 * - all uses of string and List<string> have been replaced by
 *   wrapped types PL_String and PL_ListOfStrings
 * ver 1.0 : 24 Sep 15
 * - first release
 *
 */
 //ToDo: 5 - add tests for Clone and ToXml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project2Starter
{
  class DBElementTest
  {
    static void Main(string[] args)
    {
      "Testing DBElement Package".title('=');
      WriteLine();

      Write("\n --- Test DBElement<int,PL_String> ---");
      WriteLine();

      DBElement<int, PL_String> elem1 = new DBElement<int, PL_String>();
      Write(elem1.showElement<int, PL_String>());
      WriteLine();

      DBElement<int, PL_String> elem2 = new DBElement<int, PL_String>("Darth Vader", "Evil Overlord");
      elem2.payload = new PL_String("The Empire strikes back!");
      Write(elem2.showElement<int, PL_String>());
      WriteLine();

      var elem3 = new DBElement<int, PL_String>("Luke Skywalker", "Young HotShot");
      elem3.children = new List<int> { 1, 2, 7 };
      elem3.payload = new PL_String("X-Wing fighter in swamp - Oh oh!");
      Write(elem3.showElement<int, PL_String>());
      WriteLine();

      Write("\n --- Test ToString() ---");
      Write("\n  {0}", elem3.ToString());
      WriteLine();

      Write("\n --- Test ToXml() ---");
      Write("\n  {0}", elem3.ToXml());
      WriteLine();

      Write("\n --- Test DBElement<string,PL_ListOfStrings> ---");
      WriteLine();

      DBElement<string, PL_ListOfStrings> newelem1 = new DBElement<string, PL_ListOfStrings>();
      newelem1.name = "newelem1";
      newelem1.descr = "test new type";
      newelem1.payload = new PL_ListOfStrings( new List<string> { "one", "two", "three" });
      Write(newelem1.showElement<string, PL_ListOfStrings>());

      DBElement<string, PL_ListOfStrings> newerelem1 = new DBElement<string, PL_ListOfStrings>();
      newerelem1.name = "newerelem1";
      newerelem1.descr = "same stuff";
      newerelem1.children.Add("first_key");
      newerelem1.children.Add("second_key");
      newerelem1.payload = new PL_ListOfStrings(new List<string> { "alpha", "beta", "gamma" });
      newerelem1.payload.theWrappedData.AddRange(new List<string> { "delta", "epsilon" });
      Write(newerelem1.showElement<string, PL_ListOfStrings>());
      WriteLine();

      Write("\n --- Test ToString() ---");
      Write("\n  {0}", newerelem1.ToString());
      WriteLine();

      Write("\n --- Test ToXml() ---");
      Write("\n  {0}", newerelem1.ToXml());
      WriteLine();
      WriteLine();

      Write("\n --- Test FromXml() ---");
      try
      {
        DBElement<string, PL_ListOfStrings> el = newerelem1.FromXml("<nop></nop>") as DBElement<string, PL_ListOfStrings>;
      }
      catch(Exception ex)
      {
        Write(ex.Message);
      }

      Write("\n\n");
    }
  }
}
