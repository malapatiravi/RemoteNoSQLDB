

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
namespace Project4Starter
{
    /// <summary>
    /// This will hold the cild list of a DB element
    /// 
    /// </summary>
    public class Child
    {
        public string ChildID { get; set; }
        public DateTime ChildDate { get; set; }

    }
    /// <summary>
    ///DB element structure for Database operations
    /// 
    /// </summary>
    public class DBElement<Key, Data>
    {
        public string PackageID { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Query { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Country { get; set; }
        public DateTime Time { get; set; }
        public DateTime Time2 { get; set; }
        public string Payload { get; set; }
        public Child[] children { get; set; }
        public int count { get; set; }
        public string searchMeta { get; set; }
        public DateTime searchTime { get; set; }

    }
    /// <summary>
    ///DB element structure for Database operations
    /// 
    /// </summary>
    public class PackageElement
    {
        public string PackageID { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Query { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Country { get; set; }
        public DateTime Time { get; set; }
        public DateTime Time2 { get; set; }
        public string Payload { get; set; }
        public Child[] children { get; set; }       // data        |
        public int count { get; set; }
        public string searchMeta { get; set; }
        public DateTime searchTime { get; set; }
        //public DBElement(string name = "unnamed", string Descr = "undescribed")
        //{
        //    Name = name;
        //    Description = Descr;
        //    Time = DateTime.Now;


        //}
    }
#if (TEST_DBELEMENT)

    /// <summary>
    /// class for demonstrating the project 2 requirments
    /// 
    /// </summary>
    class TestDBElement
    {

        static void Main(string[] args)
        {
            TestDBElement td = new TestDBElement();
            WriteLine("Testing DBElement Package");
           
            Write("\n --- Test DBElement<int,string> ---");
           

            //Inserting the first element
            DBElement<int, string> elem2 = new DBElement<int, string>();
            elem2.Payload = "The Empire strikes back!";
          

            //Inserting the second element
            var elem3 = new DBElement<int, string>();
          
            elem3.Payload = "X-Wing fighter in swamp - Oh oh!";
            
        }

        private static void WriteLine(string v)
        {
            throw new NotImplementedException();
        }
    }
#endif
}
