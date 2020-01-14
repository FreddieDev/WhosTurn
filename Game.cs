using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ColorTranslator = System.Drawing.ColorTranslator;

namespace WhosTurn
{
    public class Game
    {
        // Settings
        private readonly int CountdownDuration = 2;

        // Vars
        private DispatcherTimer dispatcherTimer;
        public List<GameKey> KeysInGame { get; set; }
        private int TimeUntilStart;
        private bool GameOver;


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


        public Game()
        {
            KeysInGame = new List<GameKey>();
        }

        private void UpdateCountdownText()
        {
            MainWindow.SetCountdownText("Game starting in " + TimeUntilStart + " seconds...");
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeUntilStart--;

            if (TimeUntilStart < 1)
            {
                GameOver = true; // Update flag to stop leaving players clearing results screen

                var random = new Random();
                int winnerIndex = random.Next(KeysInGame.Count);
                MainWindow.DisplayWinner(KeysInGame[winnerIndex].ButtonChar);

                dispatcherTimer.Stop();
            } else
            {
                UpdateCountdownText();
            }
        }

        public void StartCountdown()
        {
            StopCountdown();
            TimeUntilStart = CountdownDuration;
            UpdateCountdownText();

            //  DispatcherTimer setup
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void StopCountdown()
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
            }
        }

        public void UpdateGame()
        {
            MainWindow.StaticUpdateKeysDisplay();

            // (re)start game countdown if more than 1 person is playing
            bool shouldStartGame = KeysInGame.Count > 1;
            if (shouldStartGame)
            {
                StartCountdown();
            }
            else
            {
                StopCountdown();
            }
            MainWindow.SetCountdownVisibility(shouldStartGame);
        }

        public void AddKey(int asciiCode, string keyStr)
        {
            if (!KeysInGame.Any(key => key.ASCIICode == asciiCode))
            {
                Random random = new Random();
                GameKey gameKey = new GameKey(
                    keyStr,
                    asciiCode,

                    // Generate Hue randomly
                    // Lightness is boosted (from 50% or 120) to add pastel color effect
                    // Saturation is reduced to dim colour intensities
                    HLSToRGB(random.Next(0, 240), 140, 160)
                );
                KeysInGame.Add(gameKey);

                GameOver = false;
                UpdateGame();
            }
        }

        public void RemoveKey(int asciiCode)
        {
            KeysInGame.RemoveAll(key => key.ASCIICode == asciiCode);
            if (!GameOver) UpdateGame();
        }
    }

}
