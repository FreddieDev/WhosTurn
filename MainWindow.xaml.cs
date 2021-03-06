﻿using System;
using System.Windows;
using System.Windows.Controls;
using KeysConverter = System.Windows.Forms.KeysConverter;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Generic;

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

        private readonly string awaitingMatchText = "Press a key to start a game...";
        private readonly string matchBeginningText = "Hold any key to join!";
        private readonly FontFamily statusDefaultFont = new FontFamily("Segoe UI SemiBold");
        private readonly FontFamily statusLoserFont = new FontFamily("Segoe UI Black");

        private readonly String[] bannedKeyNames = new String[] {
            "Oem",
            "None",
            "LMen",
            "RMen",
        };

        private GameKey LoserKey;
        private bool readyForGame = true;

        private static MainWindow form = null;
        private readonly Game whosTurn;
        private DispatcherTimer keyUpTimer;
        public List<Key> KeysToRemove { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            whosTurn = new Game();
            form = this;
            KeysToRemove = new List<Key>();

            TB_Status.Text = awaitingMatchText;
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

        private void BuildGameKey(GameKey key)
        {
            // Create an Ellipse    
            key.Ellipse = new Ellipse
            {
                Height = keyEllipseSizes,
                Width = keyEllipseSizes,
                Stroke = new SolidColorBrush(Colors.LightGray),
                Fill = new SolidColorBrush(key.Color)
            };

            // Create button label
            key.TextBlock = new TextBlock
            {
                Width = keyEllipseSizes,
                Text = key.ButtonChar,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 19,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 12, 0, 0) // Vertically center text
            };
        }

        internal static void StaticDisplayLoser(GameKey loserKey)
        {
            form.DisplayLoser(loserKey);
        }
        private void DisplayLoser(GameKey loserKey)
        {
            readyForGame = false;
            LoserKey = (GameKey)loserKey.Clone();
            foreach (GameKey key in whosTurn.KeysInGame)
            {
                ClearKey(key);
            }
            RenderKey(LoserKey);
            LoserKey.Ellipse.StrokeThickness = 3;
            LoserKey.Ellipse.Stroke = new SolidColorBrush(Colors.DarkRed);

            L_Countdown.Content = "";
            TB_Status.Text = "PLAYER " + loserKey.ButtonChar + " LOSES!";
            TB_Status.FontFamily = statusLoserFont;
        }

        public static void SetCountdownVisibility(bool countdownVisibility)
        {
            form.L_Countdown.Visibility = countdownVisibility ? Visibility.Visible : Visibility.Hidden;
        }

        internal static void StaticRenderKey(GameKey key)
        {
            form.RenderKey(key);
        }
        private void RenderKey(GameKey key)
        {
            BuildGameKey(key);

            int ellipseXPos = keyEllipseSizes + keyEllipseMargin;
            int xPos = key.Pos * ellipseXPos + keyDisplayPadding;
            int yPos = keyDisplayPadding;

            Canvas.SetTop(key.Ellipse, yPos);
            Canvas.SetLeft(key.Ellipse, xPos);
            C_Game.Children.Add(key.Ellipse);

            Canvas.SetLeft(key.TextBlock, xPos);
            Canvas.SetTop(key.TextBlock, yPos);
            C_Game.Children.Add(key.TextBlock);
        }

        public static void StaticClearKey(GameKey key)
        {
            if (!form.readyForGame) return;
            form.ClearKey(key);
        }
        private void ClearKey(GameKey key)
        {
            C_Game.Children.Remove(key.Ellipse);
            C_Game.Children.Remove(key.TextBlock);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Don't allow a new game to start until the last game is cleared
            if (!readyForGame) return;

            int asciiCode = KeyInterop.VirtualKeyFromKey(e.Key);
            string keyStr = GetKeyLetterFromNumber(asciiCode);
            if (keyStr.Length > 5) return; // Ignore keys with big names
            if (bannedKeyNames.Any(keyStr.Contains)) return; // Ignore keys with ugly names

            TB_Status.Text = matchBeginningText;
            whosTurn.AddKey(asciiCode, keyStr);
        }

        private void Window_ActualKeyUp(List<Key> keysToRemove)
        {
            keyUpTimer.Stop();

            foreach (Key key in keysToRemove)
            {
                int asciiCode = KeyInterop.VirtualKeyFromKey(key);

                // Check if the key is in the game
                if (whosTurn.KeysInGame.FindAll(Key => Key.ASCIICode == asciiCode).Count == 0) continue;


                // Don't allow keys to be removed by other keyboards (check key is still down)
                if (Keyboard.IsKeyDown(key)) continue;

                whosTurn.RemoveKey(asciiCode);

                if (whosTurn.KeysInGame.Count == 0)
                {
                    if (!readyForGame)
                    {
                        ClearKey(LoserKey);
                        LoserKey = null;
                        readyForGame = true;
                    }

                    TB_Status.FontFamily = statusDefaultFont;
                    TB_Status.Text = awaitingMatchText;
                }
            }

            keysToRemove.Clear();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);

            KeysToRemove.Add(e.Key);

            //  DispatcherTimer setup
            keyUpTimer = new DispatcherTimer();
            keyUpTimer.Tick += (_, __) => { Window_ActualKeyUp(KeysToRemove); };
            keyUpTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            keyUpTimer.Start();
        }
    }
}
