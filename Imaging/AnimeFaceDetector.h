#pragma once

#include <opencv2/objdetect.hpp>

namespace Imaging
{
	public ref class AnimeFaceDetector sealed
	{
	private:
		int frameWidth;
		int frameHeight;
		Windows::Foundation::Collections::IVector<Windows::Foundation::Rect>^ rects;
		static cv::CascadeClassifier cascade;


	public:
		AnimeFaceDetector();
		void LoadCascade();
		Windows::Foundation::IAsyncOperation<Windows::Foundation::Collections::IVector<Windows::Foundation::Rect>^>^ Detect(Windows::Foundation::Uri^ uri, double scaleFactor, int minNeighbors, Windows::Foundation::Size minSize);
		Windows::Foundation::IAsyncOperation<Windows::Foundation::Collections::IVector<Windows::Foundation::Rect>^>^ DetectBitmap(Windows::Graphics::Imaging::BitmapFrame^ frame, double scaleFactor, int minNeighbors, Windows::Foundation::Size minSize);
	};
}
