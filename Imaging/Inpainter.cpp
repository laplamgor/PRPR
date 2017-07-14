
#include "utils.hpp"

#include "pch.h"
#include "Inpainter.h"
#include <opencv2\core.hpp>
#include <opencv2\imgcodecs.hpp>
#include <opencv2\imgproc.hpp>
#include <opencv2\imgproc.hpp>
#include "opencv2\imgcodecs.hpp"
#include "opencv2\imgproc.hpp"
#include "opencv2\photo.hpp"
#include <robuffer.h>

using namespace concurrency;
using namespace cv;
using namespace std;
using namespace Imaging;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Imaging;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml::Media::Imaging;


using namespace Windows::UI::Xaml::Data;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage::Streams;
using namespace Microsoft::WRL;

Inpainter::Inpainter()
{
}

cv::CascadeClassifier Inpainter::cascade;

IAsyncOperation<Windows::Storage::Streams::InMemoryRandomAccessStream^>^ Inpainter::Detect()
{
	return create_async([this](){
		InMemoryRandomAccessStream^ inStream = ref new InMemoryRandomAccessStream();

		Mat img, img2, img3, mask, inpainted;
		img = imread("12.jpg");
		GaussianBlur(img, img2, cv::Size(3,3), 0, 0);

		bilateralFilter(img2, img3, 7, 80, 80);


		mask = imread("12.png", 0);
		cv::inpaint(img3, mask, inpainted, 7, INPAINT_TELEA);

		Mat inpainted4 = Mat(inpainted.rows, inpainted.cols, CV_8UC4);
		int from_to[] = { 0,0, 1,1, 2,2 };
		cv::mixChannels(&inpainted, 1, &inpainted4, 1, from_to, 3);


		DataWriter^ writer = ref new DataWriter();
		Array<byte>^ arr = ref new Array<byte>(inpainted4.data, inpainted4.step[0] * inpainted4.rows);
		writer->WriteBytes(arr);

		IBuffer^ buffer = writer->DetachBuffer();
		
		return create_task([inStream, buffer] ()
		{
			return inStream->WriteAsync(buffer);
		}).then([inStream](int tt){
			
			return inStream;
		});
	});
}

IAsyncOperation<Windows::Storage::Streams::InMemoryRandomAccessStream^>^ Inpainter::InpaintMS()
{
	return create_async([this]() {
		InMemoryRandomAccessStream^ inStream = ref new InMemoryRandomAccessStream();


		Mat img;
		img = imread("12.jpg");

		Mat inpainted12 = inpaint();

		Mat inpainted;
		inpainted12.convertTo(inpainted, CV_8U, 255.0 , 0.0f);


		Mat inpainted4 = Mat(inpainted.rows, inpainted.cols, CV_8UC4);
		int from_to[] = { 0,0, 1,1, 2,2 };

		cv::mixChannels(&inpainted, 1, &inpainted4, 1, from_to, 3);


		DataWriter^ writer = ref new DataWriter();
		Array<byte>^ arr = ref new Array<byte>(inpainted4.data, inpainted4.step[0] * inpainted4.rows);
		writer->WriteBytes(arr);

		IBuffer^ buffer = writer->DetachBuffer();

		return create_task([inStream, buffer]()
		{
			return inStream->WriteAsync(buffer);
		}).then([inStream](int tt) {

			return inStream;
		});
	});
}
