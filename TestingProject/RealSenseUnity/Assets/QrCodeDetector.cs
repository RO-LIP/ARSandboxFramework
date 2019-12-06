
    using UnityEngine;
    using System.Collections;
    using OpenCvSharp;
    using UnityEngine.UI;
    using Intel.RealSense;

    public class QrCodeDetector : MonoBehaviour
    {

        public RsProcessingPipe processingPipe;

        void OnNewSample(Frame frame)
        {
        using (var frameSet = frame.AsFrameSet())
            {
           
                using (var colorFrame = frameSet.ColorFrame)
                {
                Debug.Log(colorFrame);
                if (colorFrame == null) return;
                
                //Debug.Log(depthFrame);
                //Gray scale image
                int cfWidth = colorFrame.Width;
                    int cfHeight = colorFrame.Height;
                    Mat image = new Mat (cfWidth, cfHeight, MatType.CV_16UC1, colorFrame.Data);
                    var obj = new QRCodeDetector();
                    bool detected = obj.Detect(image, out var points);
                    Debug.Log(detected);

                }

            }

        }





        private void Start()
        {
            processingPipe.OnNewSample += OnNewSample;
        }

        private void Update()
        {

        }



    }


