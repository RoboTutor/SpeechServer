/**
 * This Application opens streams of provided Axis camera IP Addresses and does 
 * Image Processing to detect hand raised events.
 * If an event is detected, then it sends the coordinates of the detected event
 * to the RoboTutor application server.
 */


#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <iostream>

#include <opencv\cv.h>
#include <opencv\highgui.h>
#include "FrameDifferenceBGS.h"
#include <Windows.h>
#include <process.h>

using namespace cv;
using namespace std;


extern void sendTCPMessage(const char *msg, const char *addr, const char *port);
extern void init();

// IMPORTANT CONSTANTS
const double THRESHOLD = 0.01;
const double MAX_VALUE = 0.47;
const int BODY_LENGTH = 25;
//#define USE_AXIS_CAMERA


/** Face Detection */
 const String face_cascade_name = "haarcascade_frontalface_alt.xml";
 const String eyes_cascade_name = "haarcascade_eye_tree_eyeglasses.xml";
 CascadeClassifier face_cascade;
 CascadeClassifier eyes_cascade;
 const string window_name = "Capture - Face detection";
 RNG rng(12345);


/**
 * Function to perform fast template matching with gaussian image pyramid
 */
void fastMatchTemplate(cv::Mat& srca,  // The reference image
                       cv::Mat& srcb,  // The template image
                       cv::Mat& dst,   // Template matching result
                       int maxlevel)   // Number of levels
{
    std::vector<cv::Mat> refs, tpls, results;

    // Build Gaussian pyramid
    cv::buildPyramid(srca, refs, maxlevel);
    cv::buildPyramid(srcb, tpls, maxlevel);

    cv::Mat ref, tpl, res;

    // Process each level
    for (int level = maxlevel; level >= 0; level--)
    {
        ref = refs[level];
        tpl = tpls[level];
        res = cv::Mat::zeros(ref.size() + cv::Size(1,1) - tpl.size(), CV_32FC1);

        if (level == maxlevel)
        {
            // On the smallest level, perform regular template matching
            cv::matchTemplate(ref, tpl, res, CV_TM_CCORR_NORMED);
        }
        else
        {
            // On the next layers, template matching is performed on pre-defined 
            // ROI areas.  We define the ROI using the template matching result 
            // from the previous layer.

            cv::Mat mask;
            cv::pyrUp(results.back(), mask);

            cv::Mat mask8u;
            mask.convertTo(mask8u, CV_8U);

            // Find matches from previous layer
            std::vector<std::vector<cv::Point> > contours;
            cv::findContours(mask8u, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_NONE);

            // Use the contours to define region of interest and 
            // perform template matching on the areas
            for (int i = 0; i < contours.size(); i++)
            {
                cv::Rect r = cv::boundingRect(contours[i]);
                cv::matchTemplate(
                    ref(r + (tpl.size() - cv::Size(1,1))), 
                    tpl, 
                    res(r), 
                    CV_TM_CCORR_NORMED
                );
            }
        }

        // Only keep good matches
        cv::threshold(res, res, THRESHOLD, 1., CV_THRESH_TOZERO);
        results.push_back(res);
    }

    res.copyTo(dst);
}

/** @function detectAndDisplay */
void detectFace( Mat frame )
{
  std::vector<Rect> faces;
  Mat frame_gray;

  cvtColor( frame, frame_gray, CV_BGR2GRAY );
  equalizeHist( frame_gray, frame_gray );

  CascadeClassifier face_cascade;
  face_cascade.load( "haarcascade_frontalface_alt.xml" );
    
  //-- Detect faces
  face_cascade.detectMultiScale( frame_gray, faces, 1.1, 3, 0|CV_HAAR_SCALE_IMAGE, Size(15, 15) );

  for( size_t i = 0; i < faces.size(); i++ )
  {
    Point center( faces[i].x + faces[i].width*0.5, faces[i].y + faces[i].height*0.5 );
    ellipse( frame, center, Size( faces[i].width*0.5, faces[i].height*0.5), 0, 0, 360, Scalar( 255, 0, 255 ), 4, 8, 0 );

    Mat faceROI = frame_gray( faces[i] );
    std::vector<Rect> eyes;

   /* //-- In each face, detect eyes
    eyes_cascade.detectMultiScale( faceROI, eyes, 1.1, 2, 0 |CV_HAAR_SCALE_IMAGE, Size(30, 30) );

    for( size_t j = 0; j < eyes.size(); j++ )
     {
       Point center( faces[i].x + eyes[j].x + eyes[j].width*0.5, faces[i].y + eyes[j].y + eyes[j].height*0.5 );
       int radius = cvRound( (eyes[j].width + eyes[j].height)*0.25 );
       circle( frame, center, radius, Scalar( 255, 0, 0 ), 4, 8, 0 );
     }*/
  }
  //-- Show o/p
  //imshow( window_name, frame );
 }

/*
 * Structure contains information about the time and the Location of the latest detection
 */
struct DetectionFilter
{
	SYSTEMTIME systime;
	cv::Point beginPoint;
	cv::Point endPoint;
	bool shown;
};

/*
 * Structure contains information about the information required to open the 
 * video stream.
 */
struct Parameters
{
	int cam;
	DetectionFilter currentDectection;
	DetectionFilter oldDectection;
	String cameraIpAddress;
	String username;
	String password;
	String computerIp;
	String socketNumber;
};

/*
 * Returns the current time in milliseconds.
 */
long long getTimeInMilli(SYSTEMTIME tym)
{
	return (long long)(tym.wMilliseconds + tym.wSecond*1000 + tym.wMinute*60*1000 + tym.wHour*60*60*1000);
}

void runVideoCaptureAndProcessing(void *param)
{
	CvCapture *capture = 0;
	SYSTEMTIME st;
	Parameters* par = (Parameters*)param; 
	DetectionFilter currentDectection = par -> currentDectection;
	DetectionFilter oldDectection = par -> oldDectection;
	char* front_window = "Front";
	char* back_window = "Back";
	std::string videoStreamAddress;// example stream = "http://root:passcode123@192.168.0.108/mjpg/video.mjpg";
	VideoCapture cap;
  
	int camera = par -> cam;

	videoStreamAddress = "http://" + par -> username + ":" + par -> password + "@" 
			+ par -> cameraIpAddress +"/mjpg/video.mjpg";

	//capture = cvCaptureFromCAM(0); // To capture from laptop's webcam 
	if(camera == 1){
		namedWindow(front_window, CV_WINDOW_AUTOSIZE);
		
#ifdef USE_AXIS_CAMERA
		 //open the video stream and make sure it's opened
		if(!cap.open(videoStreamAddress)) {
			std::cout << "Error opening Camera - 1 stream" << std::endl;
			std::cout << "Type 'q' to quit application" << std::endl;
			return;
		}
		std::cout << "Started Camera - 1" << std::endl;
#else
		std::cout << "Using Video File 1" << std::endl;
		capture = cvCaptureFromAVI("class/vid/front2.wmv"); // To load from a file
#endif

	}else if(camera == 2){
		namedWindow(back_window, CV_WINDOW_AUTOSIZE);

#ifdef USE_AXIS_CAMERA
		//open the video stream and make sure it's opened
		if(!cap.open(videoStreamAddress)) {
			std::cout << "Error opening Camera - 2 stream" << std::endl;
			std::cout << "Type 'q' to quit application" << std::endl;
			return;
		}
		std::cout << "Started Camera - 2" << std::endl;
#else
		std::cout << "Using Video File 2" << std::endl;
		capture = cvCaptureFromAVI("class/vid/back2.wmv");
#endif
		
	}
  
	Mat img_input;

	// Load Templates
	cv::Mat temp = imread("class/templates/t2.png", CV_LOAD_IMAGE_GRAYSCALE);
 
	FrameDifferenceBGS* bgs = new FrameDifferenceBGS;
	
	int key = 0;
	while(key != 'q')
	{
#ifdef USE_AXIS_CAMERA
		if(cap.isOpened())
			cap >> img_input;

		if(img_input.empty())
			break;
#else	
		if(!capture)
			break;
		IplImage *frame = cvQueryFrame(capture);
		if(!frame){
			break;
		}
		cv::Mat img_input(frame,true);

#endif

		cv::resize(img_input,img_input,cv::Size(800, 600));
		//cv::imshow("input", img_input);
    
		cv::Mat img_mask;
		cv::Mat img_dummy;
	
		bgs->process(img_input, img_mask, img_dummy); // automatically shows the foreground mask image
    
		if(!img_mask.empty())
		{
			try
			{
				cv::Mat result;
				result.create( img_mask.cols, img_mask.rows, CV_32FC1 );
			
				fastMatchTemplate(img_mask, temp, result, 0);
				while (true)
				{
					double minval, maxval;
					cv::Point minloc, maxloc;
					cv::minMaxLoc(result, &minval, &maxval, &minloc, &maxloc);
					GetSystemTime(&st);
					if (maxval >= MAX_VALUE) // Filter only raising hand events
					{
						GetSystemTime(&(currentDectection.systime));						
											
						currentDectection.beginPoint = maxloc;
						currentDectection.endPoint = cv::Point(maxloc.x + temp.cols, maxloc.y + temp.rows);
						currentDectection.shown = false;
						
						cv::floodFill(
							result, maxloc, 
							cv::Scalar(0), 0, 
							cv::Scalar(.1), 
							cv::Scalar(1.)
						);
						
					}
					else
						break;
				}

				// Draw red rectangle around detected regions
				if(getTimeInMilli(st) - getTimeInMilli(currentDectection.systime) > 500 && !currentDectection.shown)
				{
					if(camera == 1)
						detectFace(img_input);
					cv::rectangle(
						img_input, currentDectection.beginPoint, 
						currentDectection.endPoint, 
						Scalar(0,0,255),  2, 8, 0 
					 );
						
					currentDectection.shown = true;					
					GetSystemTime(&(currentDectection.systime));	

					// Calculate co-ordinates to be sent to Nao
					
					int x = currentDectection.beginPoint.x + abs((currentDectection.beginPoint.x - currentDectection.endPoint.x)/2);
					switch(camera){
						case 1 :
							if(x >= (img_input.cols/2))
								x = -(x - (img_input.cols /2));
							else if(x < (img_input.cols/2))
								x = (img_input.cols /2) - x;
							break;
						case 2:
							if(x < (img_input.cols/2))
								x = -((img_input.cols /2) - x);
							else if(x >= (img_input.cols/2))
								x = x - (img_input.cols /2);
							break;
						default:
							break;
					}
					
					int y = currentDectection.beginPoint.y + abs((currentDectection.beginPoint.y - currentDectection.endPoint.y)/2);

					string location;
					stringstream timestream, xstream, ystream, camstream;
					camstream << camera;
					location = camstream.str();

					timestream << getTimeInMilli(currentDectection.systime);
					location = location + "," + timestream.str();

					xstream << x;
					location = location + "," + xstream.str();

					ystream << y;
					location = location + "," + ystream.str();

					// Send co-ordinates to Nao
					sendTCPMessage(location.c_str(), par->computerIp.c_str(), par->socketNumber.c_str());

				}
				
				if(camera == 1){
					cv::imshow(front_window, img_input);
				}else if(camera == 2){
					cv::imshow(back_window, img_input);
				}
			}
			catch(Exception ex)
			{
				cout << ex.err << endl;
			}
		}
		key = cvWaitKey(1);
	  }

	  delete bgs;

	  cvDestroyAllWindows();
	  cvReleaseCapture(&capture);
  
	  return;
}

/**
 * Starting point of the Program
 */
int main( int argc, char** argv )
{
	Parameters cam1Par, cam2Par;
	
	if(argc != 9){
		cout << "Invalid Arguments received." << endl;
		cout << "Arguments: <Camera1-IPAddress> <username> <password> <Camera2-IPAddress> \n<username> <password> <Computer-IPAddress> <port>" << endl;
		cout << "Press any key to quit ..." << endl;
		getchar();
		return 1;
	}

	/*for (int i = 0; i < argc; ++i) { // To print the arguments received by the app
        std::cout << argv[i] << std::endl;
    }*/

	cam1Par.cameraIpAddress = argv[1];
	cam1Par.username = argv[2];
	cam1Par.password = argv[3];
	cam2Par.cameraIpAddress = argv[4];
	cam2Par.username = argv[5];
	cam2Par.password = argv[6];
	cam1Par.computerIp = cam2Par.computerIp = argv[7];
	cam1Par.socketNumber = cam2Par.socketNumber = argv[8];

	  int cam1 = 1, cam2 = 2;
	  
	  init();
	  cam1Par.cam = cam1;
	  uintptr_t t1 = _beginthread( runVideoCaptureAndProcessing, 0, (void*)&cam1Par);
	  cam2Par.cam = cam2;
	  uintptr_t t2 = _beginthread( runVideoCaptureAndProcessing, 0, (void*)&cam2Par);
	  int key;
	  while((key = getchar()) != 'q');
	  return 0;
}


// ------------------------------------------------------------------------
// -----------------------------------------------------------------------
