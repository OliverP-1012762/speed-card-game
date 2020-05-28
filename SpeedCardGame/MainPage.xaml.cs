using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SpeedCardGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //global variables
        public int size = 5;
        public int tutPg = 1;
        public MainPage()
        {
            this.InitializeComponent();

            tutorial.Width = Convert.ToInt32(Window.Current.Bounds.Width) / size;
            tutorial.Height = Convert.ToInt32(Window.Current.Bounds.Height) / size;
            tutorial.Source = new BitmapImage(new Uri($"ms-appx:///Assets/tutorialPages/tutPg{tutPg}.jpg"));
        }

        private void tutorialUpdate(String Up)
        {
            if (Up == "previous")
            {
                if (tutPg > 1) {
                    tutPg--;
                    tutorial.Source = new BitmapImage(new Uri($"ms-appx:///Assets/tutorialPages/tutPg{tutPg}.jpg"));
                }
            }
            else if (Up == "next")
            {
                if (tutPg < 10)
                {
                    tutPg++;
                    tutorial.Source = new BitmapImage(new Uri($"ms-appx:///Assets/tutorialPages/tutPg{tutPg}.jpg"));
                }

            }
            else if (Up == "smaller")
            {
                if (size > 1)
                {
                    size -= 1;
                    tutorial.Width = Convert.ToInt32(Window.Current.Bounds.Width) / size;
                    tutorial.Height = Convert.ToInt32(Window.Current.Bounds.Height) / size;
                }
            }
            else if (Up == "bigger")
            {
                if (size < 10)
                {
                    size += 1;
                    tutorial.Width = Convert.ToInt32(Window.Current.Bounds.Width) / size;
                    tutorial.Height = Convert.ToInt32(Window.Current.Bounds.Height) / size;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("unknown key");
            }
        }
        private void KeyPress(object sender, KeyRoutedEventArgs e)
        {
            if (Convert.ToString(e.Key) == "A"|| Convert.ToString(e.Key) == "Left")
            {
                this.tutorialUpdate("previous");
            }
            else if (Convert.ToString(e.Key) == "D"|| Convert.ToString(e.Key) == "Right")
            {
                this.tutorialUpdate("next");
            }
            else if (Convert.ToString(e.Key) == "W"||Convert.ToString(e.Key) == "Up")
            {
                this.tutorialUpdate("bigger");
            }
            else if (e.Key == Windows.System.VirtualKey.Down||e.Key == Windows.System.VirtualKey.S)
            {
                this.tutorialUpdate("smaller");
            }
        }
        private void spacePressed(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(gamePage), null);
        }
    }
}
