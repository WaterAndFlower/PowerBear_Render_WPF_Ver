#include "pch.h"
#include "imageFunc.h"
#include "opencv2/imgcodecs.hpp"
#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"

uchar* Mat_to_array(cv::Mat input);


void doDeNoise(BYTE inptImg[], int width, int height, BYTE* outPtr) {
	//https://blog.csdn.net/iiinoname/article/details/127600364
	cv::Mat imageMat(height, width, CV_8UC3, inptImg, width * 3);
	//cv::imshow("Display 读入", imageMat);
	cv::Mat result = cv::Mat::zeros(imageMat.rows, imageMat.cols, imageMat.type());
	cv::bilateralFilter(imageMat, result, 15, 20, 50);
	cv::imshow("Display 降噪", result);
	cv::imwrite("C:/Users/PowerBear/source/repos/PowerBear_Render_WPF_Ver/PowerBear_Render_WPF_Ver/bin/Debug/net6.0-windows/Tmp/Write_DeNoise.png", result);
	Mat_to_array(result, outPtr);
}
void doCanny() {
	cv::Mat Img = cv::imread("C:/Users/PowerBear/source/repos/PowerBear_Render_WPF_Ver/PowerBear_Render_WPF_Ver/bin/Debug/net6.0-windows/Tmp/Read.png", cv::ImreadModes::IMREAD_COLOR);
	cv::Mat dstImage, grayImage, edge, cannyImage;//复制的待变换图，灰度图，边缘图
	//创建原图同类型同大小的矩阵
	dstImage.create(Img.size(), Img.type());
	cannyImage.create(Img.size(), Img.type());
	//转灰度
	cvtColor(Img, grayImage, cv::COLOR_BGR2GRAY);
	//高斯滤波降噪
	blur(grayImage, edge, cv::Size(3, 3));
	Canny(edge, cannyImage, 3, 9, 3);
	imshow("3-9-3 canny算子", cannyImage);

	blur(grayImage, edge, cv::Size(3, 3));
	Canny(edge, cannyImage, 5, 200, 3);
	imshow("5-200-3 canny算子", cannyImage);

	cv::imwrite("C:/Users/PowerBear/source/repos/PowerBear_Render_WPF_Ver/PowerBear_Render_WPF_Ver/bin/Debug/net6.0-windows/Tmp/WriteCanny.png", cannyImage);

	std::vector<std::vector<cv::Point>> contours;
	cv::Mat outImg;
	outImg.create(Img.size(), Img.type());
	cv::threshold(grayImage, outImg, 127, 255, cv::THRESH_BINARY);
	//cv::findContours(outImg, contours, cv::ContoursMode, 0);
	//https://www.jb51.net/article/220037.htm
	//https://www.w3cschool.cn/opencv/opencv-p8ze2dhc.html
}



uchar* Mat_to_array(cv::Mat input)
{
	auto height = input.rows;
	auto width = input.cols;
	int value = 0;
	uchar* pRga = new uchar[height * width * 3];
	int k = 0;
	for (int i = 0; i < height; i++)  //BGR
	{
		for (int j = 0; j < width; j++)
		{
			pRga[k] = input.at<cv::Vec3b>(i, j)[0];
			pRga[height * width + k] = input.at<cv::Vec3b>(i, j)[1];
			pRga[height * width * 2 + k] = input.at<cv::Vec3b>(i, j)[2];
			k++;
		}
	}
	return pRga;
}

void Mat_to_array(cv::Mat input, BYTE* nptr)
{
	auto height = input.rows;
	auto width = input.cols;
	int value = 0;
	uchar* &pRga = nptr;
	int k = 0;
	for (int i = 0; i < height; i++)  //BGR
	{
		for (int j = 0; j < width; j++)
		{
			pRga[k] = input.at<cv::Vec3b>(i, j)[0];
			pRga[height * width + k] = input.at<cv::Vec3b>(i, j)[1];
			pRga[height * width * 2 + k] = input.at<cv::Vec3b>(i, j)[2];
			k++;
		}
	}
}
