#pragma once

#include "pch.h"
#ifdef POWERBEARRENDERCPPDLL_EXPORTS
#define Func_API __declspec(dllexport)
#else
#define Func_API __declspec(dllimport)
#endif
// 上部分供，不是DLL导出的程序使用。在本项目，属性页面，c/c++中，预处理器中，显示声明了POWERBEARRENDERCPPDLL_EXPORTS

extern "C" Func_API int TestDLL(
	const unsigned long long a, const unsigned long long b);