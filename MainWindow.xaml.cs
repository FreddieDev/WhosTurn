using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using KeysConverter = System.Windows.Forms.KeysConverter;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using ColorTranslator = System.Drawing.ColorTranslator;

namespace WhosTurn
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Settings
        private int keyEllipseSizes = 50;
        private int keyEllipseMargin = 8;
        private int keyDisplayPadding = 15;


        [DllImport("shlwapi.dll")]
        private static extern int ColorHLSToRGB(int H, int L, int S);

        /// <summary>
        /// Converts HLS colour to RGB
        /// </summary>
        /// <param name="hue">Hue ranged between 0-240</param>
        /// <param name="light">Light ranged between 0-240 (120 is normal)</param>
        /// <param name="saturation">Saturation ranged between 0-240</param>
        /// <returns>System.Media.Color from HLS parameters</returns>
        private static Color HLSToRGB(int hue, int light, int saturation)
        {
            int win32Color = ColorHLSToRGB(hue, light, saturation); // Get win32 colour from HLS
            System.Drawing.Color sysColor = ColorTranslator.FromWin32(win32Color); // Convert to system color
            
            return Color.FromArgb(sysColor.A, sysColor.R, sysColor.G, sysColor.B); // Convert to color
        }

        // Static form. Null if no form created yet.
        private static MainWindow form = null;
        private Game whosTurn;

        public MainWindow()
        {
            InitializeComponent();
            whosTurn = new Game();
            form = this;
        }

        private static string GetKeyLetterFromNumber(int letterNumber)
        {
            KeysConverter kc = new KeysConverter();
            return kc.ConvertToString(letterNumber);
        }

        public static void SetCountdownText(String countdownString)
        {
            form.L_Countdown.Content = countdownString;
        }

        internal static void DisplayWinner(int winningKey)
        {
            form.L_Countdown.Content = "";
            form.L_Status.Content = "It's " + GetKeyLetterFromNumber(winningKey) + "'s turn!";
        }

        public static void SetCountdownVisibility(bool countdownVisibility)
        {
            form.L_Countdown.Visibility = countdownVisibility ? Visibility.Visible : Visibility.Hidden;
        }

        public static void StaticUpdateKeysDisplay()
        {
            form.UpdateKeysDisplay();
        }

        private void UpdateKeysDisplay()
        {
            String keysText = "Held keys: ";
            int counter = 0;
            foreach (int i in whosTurn.KeysInGame)
            {
                // Create an Ellipse    
                Ellipse blueEllipse = new Ellipse
                {
                    Height = keyEllipseSizes,
                    Width = keyEllipseSizes
                };

                TextBlock textBlock = new TextBlock
                {
                    Width = keyEllipseSizes,
                    Text = GetKeyLetterFromNumber(i),
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 19,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 12, 0, 0) // Vertically center text
                };

                // Create a blue and a black Brush
                Random random = new Random();
                SolidColorBrush blueBrush = new SolidColorBrush
                {
                    // Generate Hue randomly
                    // Lightness is exactly 50% to show full colour
                    // Saturation is reduced to dim colour intensities
                    Color = HLSToRGB(random.Next(0, 240), 120, 150)
                };
                SolidColorBrush blackBrush = new SolidColorBrush
                {
                    Color = Colors.Black
                };


                // Set Ellipse's width and color    
                blueEllipse.StrokeThickness = 2;
                blueEllipse.Stroke = blackBrush;

                // Fill rectangle with blue color    
                blueEllipse.Fill = blueBrush;

                int ellipseXPos = keyEllipseSizes + keyEllipseMargin;
                Canvas.SetTop(blueEllipse, keyDisplayPadding);
                Canvas.SetLeft(blueEllipse, counter * ellipseXPos + keyDisplayPadding);
                C_Game.Children.Add(blueEllipse);

                Canvas.SetLeft(textBlock, counter * ellipseXPos + keyDisplayPadding);
                Canvas.SetTop(textBlock, keyDisplayPadding);
                C_Game.Children.Add(textBlock);


                keysText += i + " ";
                counter++;
            }

            //L_Status.Content = keysText;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);


            int asciiCode = KeyInterop.VirtualKeyFromKey(e.Key);
            if (GetKeyLetterFromNumber(asciiCode).Length > 4) return; // Ignore keys with big names

            whosTurn.AddKey(asciiCode);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);

            int asciiCode = KeyInterop.VirtualKeyFromKey(e.Key);
            whosTurn.RemoveKey(asciiCode);
        }
    }
}
