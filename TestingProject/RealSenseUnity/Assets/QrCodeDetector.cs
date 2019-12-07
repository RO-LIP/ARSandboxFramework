
    using UnityEngine;
    using System.Collections;
    using OpenCvSharp;
    using UnityEngine.UI;
    using Intel.RealSense;

    public class QrCodeDetector : MonoBehaviour
    {
    private QRCodeDetector qrCodeDetector;

        public RsProcessingPipe processingPipe;

        void OnNewSample(Frame frame)
        {
        using (var frameSet = frame.AsFrameSet())
            {
           
                using (var colorFrame = frameSet.ColorFrame)
                {
                if (colorFrame == null) return;
                
                //Debug.Log(depthFrame);
                //Gray scale image
                int cfWidth = colorFrame.Width;
                    int cfHeight = colorFrame.Height;
                    Mat image = new Mat (cfWidth, cfHeight, MatType.CV_16UC1, colorFrame.Data);
                    bool detected = qrCodeDetector.Detect(image, out var points);
                    Debug.Log(detected);
            }

            }

        }





        private void Start()
        {
        using (qrCodeDetector = new QRCodeDetector())
        {
            processingPipe.OnNewSample += OnNewSample;

        }

        }

        private void Update()
        {

        }



    }


