#pragma once

#include "pch.h"
#ifdef POWERBEARRENDERCPPDLL_EXPORTS
#define Func_API __declspec(dllexport)
#else
#define Func_API __declspec(dllimport)
#endif
// �ϲ��ֹ�������DLL�����ĳ���ʹ�á��ڱ���Ŀ������ҳ�棬c/c++�У�Ԥ�������У���ʾ������POWERBEARRENDERCPPDLL_EXPORTS

extern "C" Func_API int TestDLL(
	const unsigned long long a, const unsigned long long b);