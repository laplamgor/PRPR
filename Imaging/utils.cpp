#include "utils.hpp"
#include "pch.h"
#include <opencv2\core.hpp>
#include <opencv2\imgcodecs.hpp>
#include <opencv2\imgproc.hpp>
#include <opencv2\imgproc.hpp>
#include "opencv2\imgcodecs.hpp"
#include "opencv2\imgproc.hpp"
#include "opencv2\photo.hpp"
#include <robuffer.h>

// utility functions needed for inpainting

using namespace concurrency;
using namespace cv;
using namespace std;
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

/* 
 * Return a % b where % is the mathematical modulus operator.
 */
int mod(int a, int b) {
    return ((a % b) + b) % b;
}

//
//typedef std::vector<std::vector<cv::Point>> contours_t;
//typedef std::vector<cv::Vec4i> hierarchy_t;
//typedef std::vector<cv::Point> contour_t;

// Patch raduius
#define RADIUS 4
// The maximum number of pixels around a specified point on the target outline
#define BORDER_RADIUS 4


/*
* Load the color, mask, grayscale images with a border of size
* radius around every image to prevent boundary collisions when taking patches
*/
void loadInpaintingImages(
	const std::string& colorFilename,
	const std::string& maskFilename,
	cv::Mat& colorMat,
	cv::Mat& maskMat,
	cv::Mat& grayMat)
{
	assert(colorFilename.length() && maskFilename.length());

	colorMat = cv::imread(colorFilename, 1); // color
	maskMat = 255 - cv::imread(maskFilename, 0);  // grayscale

	assert(colorMat.size() == maskMat.size());
	assert(!colorMat.empty() && !maskMat.empty());

	// convert colorMat to depth CV_32F for colorspace conversions
	colorMat.convertTo(colorMat, CV_32F);
	colorMat /= 255.0f;

	// add border around colorMat
	cv::copyMakeBorder(
		colorMat,
		colorMat,
		RADIUS,
		RADIUS,
		RADIUS,
		RADIUS,
		cv::BORDER_CONSTANT,
		cv::Scalar_<float>(0, 0, 0)
	);

	cv::cvtColor(colorMat, grayMat, CV_BGR2GRAY);
}

/*
* Extract closed boundary from mask.
*/
void getContours(const cv::Mat& mask,
	contours_t& contours,
	hierarchy_t& hierarchy
)
{
	assert(mask.type() == CV_8UC1);
	cv::findContours(mask, contours, hierarchy, CV_RETR_TREE, CV_CHAIN_APPROX_NONE);
}


/*
* Return the confidence of confidencePatch
*/
double computeConfidence(const cv::Mat& confidencePatch)
{
	return cv::sum(confidencePatch)[0] / (double)confidencePatch.total();
}



/*
* Get a patch of size RAIDUS around point p in mat.
*/
cv::Mat getPatch(const cv::Mat& mat, const cv::Point& p)
{
	assert(RADIUS <= p.x && p.x < mat.cols - RADIUS && RADIUS <= p.y && p.y < mat.rows - RADIUS);
	return  mat(
		cv::Range(p.y - RADIUS, p.y + RADIUS + 1),
		cv::Range(p.x - RADIUS, p.x + RADIUS + 1)
	);
}


// get the x and y derivatives of a patch centered at patchCenter in image
// computed using a 3x3 Scharr filter
void getDerivatives(const cv::Mat& grayMat, cv::Mat& dx, cv::Mat& dy)
{
	assert(grayMat.type() == CV_32FC1);

	cv::Sobel(grayMat, dx, -1, 1, 0, -1);
	cv::Sobel(grayMat, dy, -1, 0, 1, -1);
}


/*
* Get the unit normal of a dense list of boundary points centered around point p.
*/
cv::Point2f getNormal(const contour_t& contour, const cv::Point& point)
{
	int sz = (int)contour.size();

	assert(sz != 0);

	int pointIndex = (int)(std::find(contour.begin(), contour.end(), point) - contour.begin());

	assert(pointIndex != contour.size());

	if (sz == 1)
	{
		return cv::Point2f(1.0f, 0.0f);
	}
	else if (sz < 2 * BORDER_RADIUS + 1)
	{
		// Too few points in contour to use LSTSQ regression
		// return the normal with respect to adjacent neigbourhood
		cv::Point adj = contour[(pointIndex + 1) % sz] - contour[pointIndex];
		return cv::Point2f(adj.y, -adj.x) / cv::norm(adj);
	}

	// Use least square regression
	// create X and Y mat to SVD
	cv::Mat X(cv::Size(2, 2 * BORDER_RADIUS + 1), CV_32F);
	cv::Mat Y(cv::Size(1, 2 * BORDER_RADIUS + 1), CV_32F);

	assert(X.rows == Y.rows && X.cols == 2 && Y.cols == 1 && X.type() == Y.type()
		&& Y.type() == CV_32F);

	int i = mod((pointIndex - BORDER_RADIUS), sz);

	float* Xrow;
	float* Yrow;

	int count = 0;
	int countXequal = 0;
	while (count < 2 * BORDER_RADIUS + 1)
	{
		Xrow = X.ptr<float>(count);
		Xrow[0] = contour[i].x;
		Xrow[1] = 1.0f;

		Yrow = Y.ptr<float>(count);
		Yrow[0] = contour[i].y;

		if (Xrow[0] == contour[pointIndex].x)
		{
			++countXequal;
		}

		i = mod(i + 1, sz);
		++count;
	}

	if (countXequal == count)
	{
		return cv::Point2f(1.0f, 0.0f);
	}
	// to find the line of best fit
	cv::Mat sol;
	cv::solve(X, Y, sol, cv::DECOMP_SVD);

	assert(sol.type() == CV_32F);

	float slope = sol.ptr<float>(0)[0];
	cv::Point2f normal(-slope, 1);

	return normal / cv::norm(normal);
}

/*
* Iterate over every contour point in contours and compute the
* priority of path centered at point using grayMat and confidenceMat
*/
void computePriority(const contours_t& contours, const cv::Mat& grayMat, const cv::Mat& confidenceMat, cv::Mat& priorityMat)
{
	assert(grayMat.type() == CV_32FC1 &&
		priorityMat.type() == CV_32FC1 &&
		confidenceMat.type() == CV_32FC1
	);

	// define some patches
	cv::Mat confidencePatch;
	cv::Mat magnitudePatch;

	cv::Point2f normal;
	cv::Point maxPoint;
	cv::Point2f gradient;

	double confidence;

	// get the derivatives and magnitude of the greyscale image
	cv::Mat dx, dy, magnitude;
	getDerivatives(grayMat, dx, dy);
	cv::magnitude(dx, dy, magnitude);

	// mask the magnitude
	cv::Mat maskedMagnitude(magnitude.size(), magnitude.type(), cv::Scalar_<float>(0));
	magnitude.copyTo(maskedMagnitude, (confidenceMat != 0.0f));
	cv::erode(maskedMagnitude, maskedMagnitude, cv::Mat());

	assert(maskedMagnitude.type() == CV_32FC1);

	// for each point in contour
	cv::Point point;

	for (int i = 0; i < contours.size(); ++i)
	{
		contour_t contour = contours[i];

		for (int j = 0; j < contour.size(); ++j)
		{

			point = contour[j];

			confidencePatch = getPatch(confidenceMat, point);

			// get confidence of patch
			confidence = cv::sum(confidencePatch)[0] / (double)confidencePatch.total();
			assert(0 <= confidence && confidence <= 1.0f);

			// get the normal to the border around point
			normal = getNormal(contour, point);

			// get the maximum gradient in source around patch
			magnitudePatch = getPatch(maskedMagnitude, point);
			cv::minMaxLoc(magnitudePatch, NULL, NULL, NULL, &maxPoint);
			gradient = cv::Point2f(
				-getPatch(dy, point).ptr<float>(maxPoint.y)[maxPoint.x],
				getPatch(dx, point).ptr<float>(maxPoint.y)[maxPoint.x]
			);

			// set the priority in priorityMat
			priorityMat.ptr<float>(point.y)[point.x] = std::abs((float)confidence * gradient.dot(normal));
			assert(priorityMat.ptr<float>(point.y)[point.x] >= 0);
		}
	}
}


/*
* Transfer the values from patch centered at psiHatQ to patch centered at psiHatP in
* mat according to maskMat.
*/
void transferPatch(const cv::Point& psiHatQ, const cv::Point& psiHatP, cv::Mat& mat, const cv::Mat& maskMat)
{
	assert(maskMat.type() == CV_8U);
	assert(mat.size() == maskMat.size());
	assert(RADIUS <= psiHatQ.x && psiHatQ.x < mat.cols - RADIUS && RADIUS <= psiHatQ.y && psiHatQ.y < mat.rows - RADIUS);
	assert(RADIUS <= psiHatP.x && psiHatP.x < mat.cols - RADIUS && RADIUS <= psiHatP.y && psiHatP.y < mat.rows - RADIUS);

	// copy contents of psiHatQ to psiHatP with mask
	getPatch(mat, psiHatQ).copyTo(getPatch(mat, psiHatP), getPatch(maskMat, psiHatP));
}

/*
* Runs template matching with tmplate and mask tmplateMask on source.
* Resulting Mat is stored in result.
*
*/
cv::Mat computeSSD(const cv::Mat& tmplate, const cv::Mat& source, const cv::Mat& tmplateMask)
{
	assert(tmplate.type() == CV_32FC3 && source.type() == CV_32FC3);
	assert(tmplate.rows <= source.rows && tmplate.cols <= source.cols);
	assert(tmplateMask.size() == tmplate.size() && tmplate.type() == tmplateMask.type());

	cv::Mat result(source.rows - tmplate.rows + 1, source.cols - tmplate.cols + 1, CV_32F, 0.0f);

	cv::matchTemplate(source,
		tmplate,
		result,
		CV_TM_SQDIFF,
		tmplateMask
	);
	cv::normalize(result, result, 0, 1, cv::NORM_MINMAX);
	cv::copyMakeBorder(result, result, RADIUS, RADIUS, RADIUS, RADIUS, cv::BORDER_CONSTANT, 1.1f);

	return result;
}







cv::Mat inpaint() {
	// --------------- read filename strings ------------------
	std::string colorFilename, maskFilename;


	// ---------------- read the images ------------------------
	// colorMat     - color picture + border
	// maskMat      - mask picture + border
	// grayMat      - gray picture + border
	cv::Mat colorMat, maskMat, grayMat;

	loadInpaintingImages(
		"12.jpg",
		"12.bmp",
		colorMat,
		maskMat,
		grayMat
	);

	// confidenceMat - confidence picture + border
	cv::Mat confidenceMat;
	maskMat.convertTo(confidenceMat, CV_32F);
	confidenceMat /= 255.0f;

	// add borders around maskMat and confidenceMat
	cv::copyMakeBorder(maskMat, maskMat,
		RADIUS, RADIUS, RADIUS, RADIUS,
		cv::BORDER_CONSTANT, 255);
	cv::copyMakeBorder(confidenceMat, confidenceMat,
		RADIUS, RADIUS, RADIUS, RADIUS,
		cv::BORDER_CONSTANT, 0.0001f);

	// ---------------- start the algorithm -----------------

	contours_t contours;            // mask contours
	hierarchy_t hierarchy;          // contours hierarchy


									// priorityMat - priority values for all contour points + border
	cv::Mat priorityMat(
		confidenceMat.size(),
		CV_32FC1
	);  // priority value matrix for each contour point

	assert(
		colorMat.size() == grayMat.size() &&
		colorMat.size() == confidenceMat.size() &&
		colorMat.size() == maskMat.size()
	);

	cv::Point psiHatP;          // psiHatP - point of highest confidence

	cv::Mat psiHatPColor;       // color patch around psiHatP

	cv::Mat psiHatPConfidence;  // confidence patch around psiHatP
	double confidence;          // confidence of psiHatPConfidence

	cv::Point psiHatQ;          // psiHatQ - point of closest patch

	cv::Mat result;             // holds result from template matching
	cv::Mat erodedMask;         // eroded mask

	cv::Mat templateMask;       // mask for template match (3 channel)

								// eroded mask is used to ensure that psiHatQ is not overlapping with target
	cv::erode(maskMat, erodedMask, cv::Mat(), cv::Point(-1, -1), RADIUS);

	cv::Mat drawMat;


	// main loop
	const size_t area = maskMat.total();



	while (cv::countNonZero(maskMat) != area)   // end when target is filled
	{
		// set priority matrix to -.1, lower than 0 so that border area is never selected
		priorityMat.setTo(-0.1f);

		// get the contours of mask
		getContours((maskMat == 0), contours, hierarchy);

		//if (DEBUG) {
		//	drawMat = colorMat.clone();
		//}

		// compute the priority for all contour points
		computePriority(contours, grayMat, confidenceMat, priorityMat);

		// get the patch with the greatest priority
		cv::minMaxLoc(priorityMat, NULL, NULL, NULL, &psiHatP);
		psiHatPColor = getPatch(colorMat, psiHatP);
		psiHatPConfidence = getPatch(confidenceMat, psiHatP);

		cv::Mat confInv = (psiHatPConfidence != 0.0f);
		confInv.convertTo(confInv, CV_32F);
		confInv /= 255.0f;
		// get the patch in source with least distance to psiHatPColor wrt source of psiHatP
		cv::Mat mergeArrays[3] = { confInv, confInv, confInv };
		cv::merge(mergeArrays, 3, templateMask);
		result = computeSSD(psiHatPColor, colorMat, templateMask);

		// set all target regions to 1.1, which is over the maximum value possilbe
		// from SSD
		result.setTo(1.1f, erodedMask == 0);
		// get minimum point of SSD between psiHatPColor and colorMat
		cv::minMaxLoc(result, NULL, NULL, &psiHatQ);

		assert(psiHatQ != psiHatP);

		//if (DEBUG) {
			//cv::rectangle(drawMat, psiHatP - cv::Point(RADIUS, RADIUS), psiHatP + cv::Point(RADIUS + 1, RADIUS + 1), cv::Scalar(255, 0, 0));
			//cv::rectangle(drawMat, psiHatQ - cv::Point(RADIUS, RADIUS), psiHatQ + cv::Point(RADIUS + 1, RADIUS + 1), cv::Scalar(0, 0, 255));
			//showMat("red - psiHatQ", drawMat);
		//}


		// updates
		// copy from psiHatQ to psiHatP for each colorspace
		transferPatch(psiHatQ, psiHatP, grayMat, (maskMat == 0));
		transferPatch(psiHatQ, psiHatP, colorMat, (maskMat == 0));

		// fill in confidenceMat with confidences C(pixel) = C(psiHatP)
		confidence = computeConfidence(psiHatPConfidence);
		assert(0 <= confidence && confidence <= 1.0f);
		// update confidence
		psiHatPConfidence.setTo(confidence, (psiHatPConfidence == 0.0f));
		// update maskMat
		maskMat = (confidenceMat != 0.0f);

		int c = cv::countNonZero(maskMat);
	}

//	showMat("final result", colorMat, 0);
	return colorMat;
}