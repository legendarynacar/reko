// switch.c
// Generated by decompiling switch.dll
// using Reko decompiler version 0.7.1.0.

#include "switch.h"

// 10071000: Register (ptr char) get(Stack uint32 n)
char * get(uint32 n)
{
	if (n > ~0x01)
		return "other";
	else
	{
		switch (n + 0x01)
		{
		case 0x00:
			return "zero";
		case 0x01:
			return "one";
		case 0x02:
			return "two";
		}
	}
}

// 10071080: Register Eq_13 DllMain(Stack Eq_14 hModule, Stack Eq_15 dwReason, Stack Eq_16 lpReserved)
BOOL DllMain(HANDLE hModule, DWORD dwReason, LPVOID lpReserved)
{
	return 0x01;
}

