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
        private readonly int keyEllipseSizes = 50;
        private readonly int keyEllipseMargin = 12;
        private readonly int keyDisplayPadding = 17;


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

        private void DrawGameKey(GameKey key, int xPos, int yPos)
        {
            // Create an Ellipse    
            Ellipse blueEllipse = new Ellipse
            {
                Height = keyEllipseSizes,
                Width = keyEllipseSizes
            };

            // Create button label
            TextBlock textBlock = new TextBlock
            {
                Width = keyEllipseSizes,
                Text = key.ButtonChar,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 19,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 12, 0, 0) // Vertically center text
            };


            // Set Ellipse's width and colours 
            blueEllipse.StrokeThickness = 2;
            blueEllipse.Stroke = new SolidColorBrush(Colors.LightGray);
            blueEllipse.Fill = new SolidColorBrush(key.Color);

            Canvas.SetTop(blueEllipse, yPos);
            Canvas.SetLeft(blueEllipse, xPos);
            C_Game.Children.Add(blueEllipse);

            Canvas.SetLeft(textBlock, xPos);
            Canvas.SetTop(textBlock, yPos);
            C_Game.Children.Add(textBlock);
        }

        internal static void DisplayWinner(string winningKey)
        {
            form.L_Countdown.Content = "";
            form.L_Status.Content = "\nLOSES";
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
            int counter = 0;
            foreach (GameKey key in whosTurn.KeysInGame)
            {
                int ellipseXPos = keyEllipseSizes + keyEllipseMargin;
                int xPos = counter * ellipseXPos + keyDisplayPadding;
                int yPos = keyDisplayPadding;
                DrawGameKey(key, xPos, yPos);

                counter++;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);

            int asciiCode = KeyInterop.VirtualKeyFromKey(e.Key);
            string keyStr = GetKeyLetterFromNumber(asciiCode);
            if (keyStr.Length > 4) return; // Ignore keys with big names

            whosTurn.AddKey(asciiCode, keyStr);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);

            int asciiCode = KeyInterop.VirtualKeyFromKey(e.Key);
            whosTurn.RemoveKey(asciiCode);
        }
    }
}
