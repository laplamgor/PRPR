#include <opencv2/objdetect.hpp>

namespace Imaging
{
	public ref class Inpainter sealed
	{
	private:
		int frameWidth;
		int frameHeight;
		Windows::Foundation::Collections::IVector<Windows::Foundation::Rect>^ rects;
		static cv::CascadeClassifier cascade;


	public:
		Inpainter();

		Windows::Foundation::IAsyncOperation<Windows::Storage::Streams::InMemoryRandomAccessStream^>^ Detect();
		Windows::Foundation::IAsyncOperation<Windows::Storage::Streams::InMemoryRandomAccessStream^>^ InpaintMS();
	};
}
