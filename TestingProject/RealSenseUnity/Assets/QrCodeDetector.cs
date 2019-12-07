
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
                // set isDetecting true to avoid crashes 
                isDetecting = true;
                int cfWidth = colorFrame.Width;
                int cfHeight = colorFrame.Height;
                // transform realsense frame into cv mat format
                using (var image = new Mat(cfHeight, cfWidth, MatType.CV_8UC3, colorFrame.Data))
                {
                    using (qrCodeDetector = new QRCodeDetector())
                    {
                        // Show generated Image in separate window
                        // Window.ShowImages(image);
                        // check frame if qrcode is existing
                        bool detected = qrCodeDetector.Detect(image, out var points);

                        if (detected)
                        {
                            using (var straightQrCode = new Mat())
                            {
                                qrCodeDetector.Decode(image, points);
                                var decodedString = qrCodeDetector.Decode(image, points, straightQrCode);
                                Debug.Log(decodedString);
                            }
                        }
                        isDetecting = false;
                    }
                }
            }
        }
    }

    private void Start()
    {
        processingPipe.OnNewSample += OnNewSample;
    }
}


