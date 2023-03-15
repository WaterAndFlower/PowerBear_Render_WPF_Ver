#include "pch.h"
#include "imageFunc.h"
#include "opencv2/imgcodecs.hpp"
#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"

uchar* Mat_to_array(cv::Mat input);
void Mat_to_array(cv::Mat input, BYTE* nptr);

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
void doCanny(BYTE inptImg[], int width, int height) {
	//auto imageMat = cv::imread("C:\\Users\\PowerBear\\Desktop\\1.png");
	cv::Mat imageMat(height, width, CV_8UC3, inptImg, width * 3);
	cv::Mat grayImage, contour_Mat, binImg, cannyImage, cannyImg;//复制的待变换图，灰度图，边缘图

	grayImage.create(imageMat.size(), imageMat.type());
	binImg.create(imageMat.size(), imageMat.type());
	cvtColor(imageMat, grayImage, cv::COLOR_BGR2GRAY);
	//cv::threshold(grayImage, binImg, 0, 128, cv::ThresholdTypes::THRESH_OTSU);
	binImg = grayImage;
	//转灰度
	cv::imshow("Display 灰度图", binImg);
	cv::waitKey(0);
	cv::Mat edge;
	cv::Canny(grayImage, edge, 0, 200);
	cv::imshow("Display Edge", edge);

	std::vector<std::vector<cv::Point>> contour_vec;
	std::vector<cv::Vec4i> hierarchy;
	cv::findContours(edge, contour_vec, hierarchy, cv::RETR_CCOMP, cv::ContourApproximationModes::CHAIN_APPROX_SIMPLE);

	contour_Mat.create(imageMat.size(), imageMat.type());
	cv::drawContours(imageMat, contour_vec, -1, cv::Scalar(0), 2);

	cv::imshow("轮廓图", imageMat);
	//cv::findContours(outImg, contours, cv::ContoursMode, 0);
	//https://www.jb51.net/article/220037.htm
	//https://www.w3cschool.cn/opencv/opencv-p8ze2dhc.html
	//https://blog.csdn.net/qq_30460949/article/details/124626347
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
			pRga[k++] = input.at<cv::Vec3b>(i, j)[0];
			pRga[k++] = input.at<cv::Vec3b>(i, j)[1];
			pRga[k++] = input.at<cv::Vec3b>(i, j)[2];
		}
	}
	return pRga;
}

void Mat_to_array(cv::Mat input, BYTE* nptr) // 数组指针
{
	auto height = input.rows;
	auto width = input.cols;
	int value = 0;
	uchar*& pRga = nptr;
	int k = 0;
	for (int i = 0; i < height; i++)  //RGB
	{
		for (int j = 0; j < width; j++) //RGB
		{
			pRga[k++] = input.at<cv::Vec3b>(i, j)[2];// R == R 在mat里面【2】才是R因为是BGR存储的
			pRga[k++] = input.at<cv::Vec3b>(i, j)[1];
			pRga[k++] = input.at<cv::Vec3b>(i, j)[0];
		}
	}
}
