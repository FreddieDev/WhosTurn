using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Threading;
using ColorTranslator = System.Drawing.ColorTranslator;

namespace WhosTurn
{
    public class Game
    {
        // Settings
        private readonly int CountdownDuration = 3;

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
                int loserIndex = random.Next(KeysInGame.Count);
                MainWindow.StaticDisplayLoser(KeysInGame[loserIndex]);

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
                int keyPos = KeysInGame.Count;

                if (keyPos > 0)
                {
                    // Create min and max position from current key positions
                    int min = 0, max = KeysInGame.Max(key => key.Pos) + 2;

                    // Create full list from min to max
                    IEnumerable<int> filterableKeysInGame = Enumerable.Range(min, max);

                    // Create list of current key positions
                    IEnumerable<int> keyPositions = KeysInGame.Select(key => key.Pos).ToList();

                    // Find the first gap in keyPositions
                    keyPos = filterableKeysInGame.Except(keyPositions).First();
                }

                Random random = new Random();
                GameKey gameKey = new GameKey(
                    keyPos,
                    keyStr,
                    asciiCode,

                    // Generate Hue randomly
                    // Lightness is boosted (from 50% or 120) to add pastel color effect
                    // Saturation is reduced to dim colour intensities
                    HLSToRGB(random.Next(0, 240), 140, 160)
                );
                KeysInGame.Add(gameKey);

                GameOver = false;
                MainWindow.StaticRenderKey(gameKey);
                UpdateGame();
            }
        }

        public void RemoveKey(int asciiCode)
        {
            GameKey keyToRemove = KeysInGame.Find(key => key.ASCIICode == asciiCode);
            MainWindow.StaticClearKey(keyToRemove);
            KeysInGame.Remove(keyToRemove);
            if (!GameOver) UpdateGame();
        }
    }
}
