using System;

namespace HRTimer
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
      HiResTimer hrt = new HiResTimer();
      hrt.Start();
      int sum = 0;
      int N = 1000;
      for(int i=0; i<N; ++i)
      {
        sum += (i+1);
      }
      hrt.Stop();
      Console.Write(
        "\n  after {0} iterations, sum = {1}, computed in {2} microSecs\n",
        N,
        sum,
        hrt.ElapsedMicroseconds
      );
		}
	}
}
