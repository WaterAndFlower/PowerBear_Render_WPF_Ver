#pragma once
#include "pch.h"
#include "opencv2/opencv.hpp"

#ifdef POWERBEARRENDERCPPDLL_EXPORTS
#define Func_API __declspec(dllexport)
#else
#define Func_API __declspec(dllimport)
#endif

extern "C" Func_API void doDeNoise(BYTE, int, int, BYTE*);
extern "C" Func_API void doCanny();