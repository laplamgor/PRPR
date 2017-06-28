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

		Windows::Storage::Streams::IBuffer^ Detect();
	};
}
