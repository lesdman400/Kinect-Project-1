using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;



namespace Kinect_codelist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        KinectSensor _sensor;
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];

                if (_sensor.Status == KinectStatus.Connected)
                {
                    _sensor.ColorStream.Enable();
                    _sensor.DepthStream.Enable();
                    _sensor.SkeletonStream.Enable();
                    _sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_sensor_AllFramesReady);
                    _sensor.Start();
                }
               
                   
            }

        }

        private byte[] GenerateColoredBytes(DepthImageFrame depthFrame)
        {
            short[] rawDepthData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(rawDepthData);

            Byte[] pixels = new byte[depthFrame.Height * depthFrame.Width * 4];

            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;
            const int AlphaIndex = 3;
            
          

            for (int depthIndex = 0, colorIndex = 0;
                depthIndex < rawDepthData.Length && colorIndex < pixels.Length;
                depthIndex++, colorIndex += 4)
            {
                int player = rawDepthData[depthIndex] & DepthImageFrame.PlayerIndexBitmask;
                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                if (depth <= 6000)
                {


                    pixels[colorIndex + BlueIndex] = 0;
                    pixels[colorIndex + GreenIndex] = 255;
                    pixels[colorIndex + RedIndex] = 0;
                    pixels[colorIndex + AlphaIndex] = 0;


                }

                //else if (depth < 1000)
                //{

                //    pixels[colorIndex + BlueIndex] = 100;
                //    pixels[colorIndex + GreenIndex] = 100;
                //    pixels[colorIndex + RedIndex] = 0;
                //    pixels[colorIndex + AlphaIndex] = 255;

                //}
                //else if (depth < 2000)
                //{

                //    pixels[colorIndex + BlueIndex] = 50;
                //    pixels[colorIndex + GreenIndex] = 100;
                //    pixels[colorIndex + RedIndex] = 50;
                //    pixels[colorIndex + AlphaIndex] = 255;

                //}
                //else if (depth < 3000)
                //{

                //    pixels[colorIndex + BlueIndex] = 100;
                //    pixels[colorIndex + GreenIndex] = 50;
                //    pixels[colorIndex + RedIndex] = 100;
                //    pixels[colorIndex + AlphaIndex] = 255;

                //}
                //else if (depth < 4000)
                //{

                //    pixels[colorIndex + BlueIndex] = 255;
                //    pixels[colorIndex + GreenIndex] = 0;
                //    pixels[colorIndex + RedIndex] = 20;
                //    pixels[colorIndex + AlphaIndex] = 255;

                //}
                //else if (depth < 5000)
                //{

                //    pixels[colorIndex + BlueIndex] = 70;
                //    pixels[colorIndex + GreenIndex] = 0;
                //    pixels[colorIndex + RedIndex] = 255;
                //    pixels[colorIndex + AlphaIndex] = 255;

                //}
                //else
                //{
                //    pixels[colorIndex + BlueIndex] = 0;
                //    pixels[colorIndex + GreenIndex] = 0;
                //    pixels[colorIndex + RedIndex] = 0 ;
                //    pixels[colorIndex + AlphaIndex] = 100;
                //}

                if (player == 1)
                {
                    if (depth < 1000)
                    {
                        pixels[colorIndex + BlueIndex] = 255;
                        pixels[colorIndex + GreenIndex] = 0;
                        pixels[colorIndex + RedIndex] = 0;
                        pixels[colorIndex + AlphaIndex] = 255;
                    }
                    else if (depth >= 1000 && depth < 2000)
                    {
                        pixels[colorIndex + BlueIndex] = 0;
                        pixels[colorIndex + GreenIndex] = 255;
                        pixels[colorIndex + RedIndex] = 0;
                        pixels[colorIndex + AlphaIndex] = 255;


                    }
                    else if (depth >= 2000 && depth < 3000)
                    {
                        pixels[colorIndex + BlueIndex] = 0;
                        pixels[colorIndex + GreenIndex] = 0;
                        pixels[colorIndex + RedIndex] = 255;
                        pixels[colorIndex + AlphaIndex] = 255;

                    }
                    else
                    {
                        pixels[colorIndex + BlueIndex] = 0;
                        pixels[colorIndex + GreenIndex] = 0;
                        pixels[colorIndex + RedIndex] = 0;
                        pixels[colorIndex + AlphaIndex] = 0;

                    }
                    //pixels[colorIndex + BlueIndex] = 0;
                    //pixels[colorIndex + GreenIndex] = 0;
                    //pixels[colorIndex + RedIndex] = 0;
                    //pixels[colorIndex + AlphaIndex] = 0;
                    ////byte[] frame2 = new byte[pixels.Length];




                    //    for (int idx = 0;
                    //        idx < frame2.Length;
                    //        idx += 4)
                    //    {

                    //frame2[idx + BlueIndex] = pixels[idx + BlueIndex];
                    //frame2[idx + GreenIndex] = pixels[idx + GreenIndex];
                    //frame2[idx + RedIndex] = pixels[idx + RedIndex];
                    //frame2[idx + AlphaIndex] = 255;
                    //    }
                    //    return frame2;

                }
            }

            return pixels;
            

        }

        private byte[] MessWithColor(byte[] pixels)
        {

            
            byte[] frame = new byte[pixels.Length];
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;
            const int AlphaIndex = 3; 

            //make it shades of red (remove b and g)
             
            for (int idx = 0;
                idx < frame.Length;
                idx += 4)
            {
                
                frame[idx + BlueIndex] = pixels[idx + BlueIndex];
                frame[idx + GreenIndex] = pixels[idx + GreenIndex];
                frame[idx + RedIndex] = pixels[idx + RedIndex];
                frame[idx + AlphaIndex] = 255;
            }
            return frame;
        }
        void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Stop();
                sensor.AudioSource.Stop();
            }
        }




        void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
  
            //throw new NotImplementedException();
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }

                byte[] pixels = new byte[colorFrame.PixelDataLength];

                //copy data out into our byte array
                colorFrame.CopyPixelDataTo(pixels);

                
                byte[] pixels2 = MessWithColor(pixels);
                int stride = colorFrame.Width * 4;

                image1.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height,
                    96, 96, PixelFormats.Bgra32, null, pixels2, stride);

                
            }


            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                {
                    return;
                }
            
                
                byte[] pixels = GenerateColoredBytes(depthFrame);
                int stride = depthFrame.Width * 4;

                image2.Source = BitmapSource.Create(depthFrame.Width, depthFrame.Height,
                    96, 96, PixelFormats.Bgra32, null, pixels, stride);


            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopKinect(_sensor);
        }

        private void image2_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }





    }
}



