using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;

namespace WhosTurn
{
    public class Game
    {
        // Settings
        private int CountdownDuration = 2;

        // Vars
        private DispatcherTimer dispatcherTimer;
        public List<int> KeysInGame { get; protected set; }
        private int TimeUntilStart;
        private bool GameOver;

        public Game()
        {
            KeysInGame = new List<int>();
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
                MainWindow.DisplayWinner(KeysInGame[winnerIndex]);

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

        public void AddKey(int asciiCode)
        {
            if (!KeysInGame.Contains(asciiCode))
            {
                KeysInGame.Add(asciiCode);
                GameOver = false;
                UpdateGame();
            }
        }

        public void RemoveKey(int asciiCode)
        {
            KeysInGame.Remove(asciiCode);
            if (!GameOver) UpdateGame();
        }
    }

}
