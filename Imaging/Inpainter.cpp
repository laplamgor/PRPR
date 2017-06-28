#include "pch.h"
#include "Inpainter.h"
#include <opencv2\core.hpp>
#include <opencv2\imgcodecs.hpp>
#include <opencv2\imgproc.hpp>
#include <opencv2\imgproc.hpp>
#include "opencv2\imgcodecs.hpp"
#include "opencv2\imgproc.hpp"
#include "opencv2\photo.hpp"

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

Inpainter::Inpainter()
{
}

cv::CascadeClassifier Inpainter::cascade;

Windows::Storage::Streams::IBuffer^ Inpainter::Detect()
{

	Mat img, mask, inpainted;
	img = imread("12.jpg");
	mask = imread("12.png", 0);
	inpaint(img, mask, inpainted, 3, INPAINT_NS);


	BitmapImage ^bitImg = ref new BitmapImage();
	WriteableBitmap;
	WriteableBitmap^ wbmp = ref new WriteableBitmap(inpainted.cols, inpainted.rows);
	IBuffer^ buffer = wbmp->PixelBuffer;

	return buffer;
}
