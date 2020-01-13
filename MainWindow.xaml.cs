using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WhosTurn
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        private void UpdateKeysDisplay()
        {
            String keysText = "Held keys: ";
            foreach (int i in whosTurn.KeysInGame)
            {
                keysText += i + " ";
            }

            //L_Status.Content = keysText;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            int asciiCode = KeyInterop.VirtualKeyFromKey(e.Key);
            whosTurn.AddKey(asciiCode);

            UpdateKeysDisplay();
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyUp(e);

            int asciiCode = KeyInterop.VirtualKeyFromKey(e.Key);
            whosTurn.RemoveKey(asciiCode);

            UpdateKeysDisplay();
        }
    }
}
