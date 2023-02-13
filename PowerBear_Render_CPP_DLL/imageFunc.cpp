#include "pch.h"
#include "imageFunc.h"

void doDeNoise() {
	cv::Mat imageMat = cv::imread("C:/Users/PowerBear/source/repos/PowerBear_Render_WPF_Ver/PowerBear_Render_WPF_Ver/bin/Debug/net6.0-windows/Tmp/Read.png", cv::ImreadModes::IMREAD_COLOR);
	cv::Mat result = cv::Mat::zeros(imageMat.rows, imageMat.cols, imageMat.type());
	cv::bilateralFilter(imageMat, result, 15, 20, 50);
	cv::imshow("Display Ωµ‘Î", result);
	cv::imwrite("C:/Users/PowerBear/source/repos/PowerBear_Render_WPF_Ver/PowerBear_Render_WPF_Ver/bin/Debug/net6.0-windows/Tmp/Write.png",result);
}