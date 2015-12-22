#include "stdafx.h"
#include <windows.h>
#include <iostream>
#include <iomanip>
#include "Dbghelp.h"

using namespace std;

void StackTrace()
{
    // Get handles to the current process and thread.
    HANDLE hCurProc = GetCurrentProcess();
    HANDLE hCurThread = GetCurrentThread();

    CONTEXT context;

    // Initialize the symbol handler functions.
    BOOL rslt = SymInitialize(hCurProc, NULL, TRUE);

    if (TRUE == rslt)
    {
        STACKFRAME stf;
        BOOL strslt = FALSE;

        // Get the context for the current thread.
        ZeroMemory(&context, sizeof(CONTEXT));
        context.ContextFlags = CONTEXT_FULL;
        rslt = GetThreadContext(hCurThread, &context);

        if (TRUE == rslt)
        {
            // Setup the STACKFRAME structure.  If you do not do this the
            // StackWalk api will not work correctly.
            ZeroMemory(&stf, sizeof(STACKFRAME));
            stf.AddrPC.Offset = context.Eip;
            stf.AddrPC.Mode   = AddrModeFlat;
            stf.AddrStack.Offset = context.Esp;
            stf.AddrStack.Mode   = AddrModeFlat;
            stf.AddrFrame.Offset = context.Ebp;
            stf.AddrFrame.Mode   = AddrModeFlat;

            do
            {						  
                // Keep calling the StackWalk function until the  
                // STACKFRAME.AddrReturn.Offset is equal to zero.
                strslt = StackWalk(IMAGE_FILE_MACHINE_I386, hCurProc,  
                                              hCurThread, &stf, &context,  
                                              (PREAD_PROCESS_MEMORY_ROUTINE) ReadProcessMemory,
                                              SymFunctionTableAccess, SymGetModuleBase, NULL);

                if (TRUE == strslt)
                {
                    cout << "RET " << hex << setw(8) << stf.AddrReturn.Offset \
                            << " EIP " << setw(8) << stf.AddrPC.Offset << " FRM "   \
                            << setw(8) << stf.AddrFrame.Offset << " STK "           \
                            << stf.AddrStack.Offset << endl;
                }

                if (stf.AddrReturn.Offset == 0x0)
                {
                    strslt = FALSE;
                }

            } while (TRUE == strslt);
        }

        // Close the symbol handler.
        SymCleanup(hCurProc);

    }
    else
    {
        cout << "SymInitialize failed." << endl;
    }
}