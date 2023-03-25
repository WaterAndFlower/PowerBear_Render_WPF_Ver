// dllmain.cpp : 定义 DLL 应用程序的入口点。
// 小熊渲染器，C++部分实现
// DLL文件输出位置：在WPF的bin文件夹下，Debug
// https://learn.microsoft.com/ZH-cn/cpp/build/walkthrough-creating-and-using-a-dynamic-link-library-cpp?view=msvc-160
#include "pch.h"
#include <opencv2/core/utils/logger.hpp>
BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
) {
	switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		break;
	}
	// 这里设置一些初始的东西
	cv::utils::logging::setLogLevel(cv::utils::logging::LOG_LEVEL_SILENT);// 取消opencv调试信息提示
	return TRUE;
}

//  ..\PowerBear_Render_WPF_Ver\bin\Release\net6.0-windows\publish\win-x64\
//  ..\PowerBear_Render_WPF_Ver\bin\Debug\net6.0-windows
//  ..\PowerBear_Render_WPF_Ver\bin\Release\net6.0-windows
//  $(SolutionDir)\PowerBear_Render_WPF_Ver\bin\$(Configuration)\net6.0-windows\

//  .\x64\Publish

// 打包时候必须注意，要将 C++ 的运行时的 DLL 也要打包进入到文件夹内，这样才能使得用户成功运行，防止缺少dll文件
// WPF 调试 C++ 的DLL 需要在WPF里面开启 调试外部代码 即可