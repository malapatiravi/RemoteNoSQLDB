///////////////////////////////////////////////////////////////////////
///  Test.cs    -  Measure times to scan files for Project #1       ///
///  ver 1.0                                                        ///
///                                                                 ///
///  Language:     Visual C#                                        ///
///  Platform:     Dell Dimension 8100, Windows Pro 2000, SP2       ///
///  Application:  CSE681 Example                                   ///
///  Author:       Jim Fawcett, CST 2-187, Syracuse Univ.           ///
///                (315) 443-3948, jfawcett@twcny.rr.com            ///
///////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections;

namespace HRTimer
{
  // class to test operations required for 
  // Dependency Analysis in Project #1

  class OpTimer
  {
    Hashtable table;
    ulong totalTime = 0;
    ulong openTime  = 0;
    ulong parseTime = 0;
    bool verbose = false;

    //----< constructor >----------------------------------------------

    public OpTimer()
    {
      table = new Hashtable();
    }
    //----< function never called, used to test parser >---------------

    public bool tryCommas(int first,bool second, double third)
    {
      if(first*third != 0)
        return false;
      return second;
    }
    //----< display contents of parse table >--------------------------

    public void showTable()
    {
      IDictionaryEnumerator enumer = table.GetEnumerator();
      while(enumer.MoveNext())
      {
        Console.Write("\n  {0}",enumer.Value);
        Console.Write("\t{0}",enumer.Key);
      }
    }
    //----< verbose property - display parsing details? >--------------

    public bool Verbose
    {
      get
      { return verbose; }
      set
      { verbose = value; }
    }
//
    //----< extract identifier from token string >---------------------
    //
    //  - Eliminates {}[]{};.
    //  - extracts words from comma separated list, e.g.,
    //      int first,bool second, double third
    //
    public string extractIdent(ref string compositeToken)
    {
      string temp = "";
      try
      {
        int len;
        len = compositeToken.IndexOf(")");
        if(len != -1)
          compositeToken = compositeToken.Remove(len,compositeToken.Length-len);
        len = compositeToken.LastIndexOf("(");
        if(len != -1)
          compositeToken = compositeToken.Remove(0,len+1);
        len = compositeToken.IndexOf("}");
        if(len != -1)
          compositeToken = compositeToken.Remove(len,compositeToken.Length-len);
        len = compositeToken.LastIndexOf("{");
        if(len != -1)
          compositeToken = compositeToken.Remove(0,len+1);
        len = compositeToken.IndexOf("]");
        if(len != -1)
          compositeToken = compositeToken.Remove(len,compositeToken.Length-len);
        len = compositeToken.LastIndexOf("[");
        if(len != -1)
          compositeToken = compositeToken.Remove(0,len+1);
        len = compositeToken.LastIndexOf(".");
        if(len != -1)
          compositeToken = compositeToken.Remove(0,len+1);
        len = compositeToken.LastIndexOf(";");
        if(len != -1)
          compositeToken = compositeToken.Remove(len,1);
        len = compositeToken.IndexOf(",");
        if(len != -1)
        {
          temp = compositeToken.Remove(len,compositeToken.Length-len);
          compositeToken = compositeToken.Remove(0,len+1);
        }
        else
        {
          temp = compositeToken;
          compositeToken = "";
        }
      }
      catch(Exception e)
      {
        Console.Write("\n\n--- {0} ---",e.Message);
      }
      return temp;
    }
    //
    //----< parse file into identifiers and store in Hashtable >-------

    public void ParseFile(string fileName)
    {
      HiResTimer total = new HiResTimer();
      HiResTimer open = new HiResTimer();
      HiResTimer parse = new HiResTimer();

      try
      {
        total.Start();
        open.Start();
        StreamReader fs = new StreamReader(fileName);
        open.Stop();
        openTime = open.ElapsedMicroseconds;

        parse.Start();
        int size = 0;
        string line;
        while((line = fs.ReadLine()) != null)
        {
          string[] tokens = line.Split();
          foreach(string token in tokens)
          {
            if(token == "\n" | token.Length == 0)
              continue;
            if(Verbose)
              Console.Write("\n    {0}",token);
            string tok, compositeToken = token;
            while(compositeToken != "")
            {
              tok = extractIdent(ref compositeToken);
              if(tok == "")
                continue;
              if(Verbose)
                Console.Write("\n      {0}",tok);
              if(table.Contains(tok))
                table[tok] = 1 + (int)table[tok];
              else
              {
                table[tok] = 1;
                size += tok.Length;
              }
            }
          }
        }
        parse.Stop();
        parseTime = parse.ElapsedMicroseconds;
        total.Stop();
        totalTime = total.ElapsedMicroseconds;

//
        Console.Write("\n   Open time: {0} microsec",openTime); 
        Console.Write("\n  Parse time: {0} microsec",parseTime); 
        Console.Write("\n  total time: {0} microsec",totalTime); 
        Console.Write("\n   Hash size: {0} bytes",size);
      }
      catch
      {
        Console.Write("\n  Could not open file \"{0}\"\n\n",fileName);
      }
    }
    //----< lookup a word in Hashtable >-------------------------------

    public bool HashLookUp(string word)
    {
      return table.ContainsKey(word);
    }
    //----< run timing tests for Project #1 >--------------------------

    [STAThread]
    static void Main(string[] args)
    {
      Console.Write("{0}{1}",
        "\n  Time Parsing Operations ",
        "\n =========================\n"
      );

      OpTimer ot = new OpTimer();
      ot.Verbose = false;
      string fileName = "../../Test.cs";
      ot.ParseFile(fileName);

      HiResTimer hrt = new HiResTimer();
      hrt.Start();
      int N = 1000;
      for(int i=0; i<N; ++i)
        ot.HashLookUp("class");
      hrt.Stop();
      ulong lookUpTime = hrt.ElapsedMicroseconds;
      Console.Write("\n   {0} lookups took {1} microseconds",N,lookUpTime);
      Console.Write("\n\n");
      ot.showTable();
      Console.Write("\n\n");
    }
  }
}
