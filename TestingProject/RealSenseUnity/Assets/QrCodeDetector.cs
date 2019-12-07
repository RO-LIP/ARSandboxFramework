
    using UnityEngine;
    using System.Collections;
    using OpenCvSharp;
    using UnityEngine.UI;
    using Intel.RealSense;

    public class QrCodeDetector : MonoBehaviour
    {
    private QRCodeDetector qrCodeDetector;
    private bool isDetecting = false;
        public RsProcessingPipe processingPipe;
        
        void OnNewSample(Frame frame)
        {
        if (isDetecting) return;

        using (var frameSet = frame.AsFrameSet())
            {
           
                using (var colorFrame = frameSet.ColorFrame)
                {
                if (colorFrame == null) return;
                isDetecting = true;
                int cfWidth = colorFrame.Width;
                    int cfHeight = colorFrame.Height;
                using (var image = new Mat(cfWidth, cfHeight, MatType.CV_8U, colorFrame.Data))
                {
                    using (qrCodeDetector = new QRCodeDetector())
                    {
                        bool detected = qrCodeDetector.Detect(image, out var points);
                        
                        isDetecting = false;
                        Debug.Log(detected);
                        if (detected)
                        {
                            Debug.Log(detected);
                            using (var straightQrCode = new Mat())
                            {
                                qrCodeDetector.Decode(image, points);
                                var decodedString = qrCodeDetector.Decode(image, points, straightQrCode);
                                Debug.Log(decodedString);
                            }
                        }
                        

                    }

                }
                    
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


