#include "pch.h"
#include "AnimeFaceDetector.h"

using namespace concurrency;
using namespace cv;
using namespace std;
using namespace Imaging;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Imaging;
using namespace Windows::Storage::Streams;

AnimeFaceDetector::AnimeFaceDetector()
{
}

cv::CascadeClassifier AnimeFaceDetector::cascade;

IAsyncOperation<IVector<Windows::Foundation::Rect>^>^ AnimeFaceDetector::Detect(Windows::Foundation::Uri^ uri, double scaleFactor, int minNeighbors, Windows::Foundation::Size minSize)
{

	return create_async([this, uri, scaleFactor, minNeighbors, minSize]() {
		RandomAccessStreamReference^ streamRef = RandomAccessStreamReference::CreateFromUri(uri);

		return create_task(streamRef->OpenReadAsync()).then([](IRandomAccessStreamWithContentType^ fileStream)
		{
			return BitmapDecoder::CreateAsync(fileStream);
		}).then([](task<BitmapDecoder^> thisTask)
		{
			BitmapDecoder^ decoder = thisTask.get();
			return decoder->GetFrameAsync(0);
		}).then([this, scaleFactor, minNeighbors, minSize](BitmapFrame^ frame)
		{
			// Save some information as fields
			frameWidth = frame->PixelWidth;
			frameHeight = frame->PixelHeight;

			return frame->GetPixelDataAsync();
		}).then([this, scaleFactor, minNeighbors, minSize](PixelDataProvider^ pixelProvider)
		{
			Platform::Array<byte>^ srcPixels = pixelProvider->DetachPixelData();
			Mat im = cv::Mat(frameHeight, frameWidth, CV_8UC4);
			memcpy(im.data, srcPixels->Data, 4 * frameWidth*frameHeight);


			if (cascade.empty())
			{
				cascade.load("lbpcascade_animeface.xml");
			}

			std::vector<cv::Rect> faces;
			cascade.detectMultiScale(im, faces, scaleFactor, minNeighbors, CV_HAAR_SCALE_IMAGE, cv::Size(minSize.Width, minSize.Height));
			vector<cv::Rect>::const_iterator r = faces.begin();
			IVector<Windows::Foundation::Rect>^ facesOutput = ref new Platform::Collections::Vector<Windows::Foundation::Rect>();
			for (; r != faces.end(); ++r) {
				Windows::Foundation::Rect rect = Windows::Foundation::Rect(r->x, r->y, r->width, r->height);
				facesOutput->Append(rect);
			}

			return facesOutput;
		});
	});
}



IAsyncOperation<IVector<Windows::Foundation::Rect>^>^ AnimeFaceDetector::DetectBitmap(BitmapFrame^ frame, double scaleFactor, int minNeighbors, Windows::Foundation::Size minSize)
{

	return create_async([this, frame, scaleFactor, minNeighbors, minSize]() {
		return create_task([this, frame]()
		{
			// Save some information as fields
			frameWidth = frame->PixelWidth;
			frameHeight = frame->PixelHeight;

			return frame->GetPixelDataAsync();
		}).then([this, scaleFactor, minNeighbors, minSize](PixelDataProvider^ pixelProvider)
		{
			Platform::Array<byte>^ srcPixels = pixelProvider->DetachPixelData();
			Mat im = cv::Mat(frameHeight, frameWidth, CV_8UC4);
			memcpy(im.data, srcPixels->Data, 4 * frameWidth*frameHeight);


			if (cascade.empty())
			{
				cascade.load("lbpcascade_animeface.xml");
			}

			std::vector<cv::Rect> faces;
			cascade.detectMultiScale(im, faces, scaleFactor, minNeighbors, CV_HAAR_SCALE_IMAGE, cv::Size(minSize.Width, minSize.Height));
			vector<cv::Rect>::const_iterator r = faces.begin();
			IVector<Windows::Foundation::Rect>^ facesOutput = ref new Platform::Collections::Vector<Windows::Foundation::Rect>();
			for (; r != faces.end(); ++r) {
				Windows::Foundation::Rect rect = Windows::Foundation::Rect(r->x, r->y, r->width, r->height);
				facesOutput->Append(rect);
			}

			return facesOutput;
		});
	});
}