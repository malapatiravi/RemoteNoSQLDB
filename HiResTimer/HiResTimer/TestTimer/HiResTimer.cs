
// class HiResTimer
//
// by: Shawn Van Ness
// rev: 15 Mar 2002
//
using System;
using System.Runtime.InteropServices; // for DllImport attribute
using System.ComponentModel; // for Win32Exception class
using System.Threading; // for Thread.Sleep method

namespace HRTimer
{
   class HiResTimer
   {
      // Construction

      public HiResTimer()
      {
#if (!NOTIMER)
         a = b = 0UL;
         if ( QueryPerformanceFrequency( out f) == 0) 
            throw new Win32Exception();
#endif
      }

      // Properties

      public ulong ElapsedTicks
      {
#if (!NOTIMER)
         get
         { return (b-a); }
#else
      get
      { return 0UL; }
#endif
      }

      public ulong ElapsedMicroseconds
      {
#if (!NOTIMER)
         get
         { 
            ulong d = (b-a); 
            if (d < 0x10c6f7a0b5edUL) // 2^64 / 1e6
               return (d*1000000UL)/f; 
            else
               return (d/f)*1000000UL;
         }
#else
      get
      { return 0UL; }
#endif
      }

      public TimeSpan ElapsedTimeSpan
      {
#if (!NOTIMER)
         get
         { 
            ulong t = 10UL*ElapsedMicroseconds;
            if ((t&0x8000000000000000UL) == 0UL)
               return new TimeSpan((long)t);
            else
               return TimeSpan.MaxValue;
         }
#else
      get
      { return TimeSpan.Zero; }
#endif
      }

      public ulong Frequency
      {
#if (!NOTIMER)
         get
         { return f; }
#else
      get
      { return 1UL; }
#endif
      }

      // Methods

      public void Start()
      {
#if (!NOTIMER)
         Thread.Sleep(0);
         QueryPerformanceCounter( out a);
#endif
      }

      public ulong Stop()
      {
#if (!NOTIMER)
         QueryPerformanceCounter( out b);
         return ElapsedTicks;
#else
      return 0UL;
#endif
      }

      // Implementation

#if (!NOTIMER)
      [ DllImport("kernel32.dll", SetLastError=true) ]
      protected static extern 
         int QueryPerformanceFrequency( out ulong x);

      [ DllImport("kernel32.dll") ]
      protected static extern 
         int QueryPerformanceCounter( out ulong x);

      protected ulong a, b, f;
#endif
   }
}