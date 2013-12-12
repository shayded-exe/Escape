using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Reflection;
using Escape.Exceptions;

namespace Escape
{
    class Program
    {
        #region Declarations
        // The gloabal Random instance, possibly should be moved into an instance for repeatable plays,
        // but that wasn't possible before either.
        public static Random Random = new Random();

        //The width and height of the console
        private const int width = 73;
        private const int height = 30;

        //Where save data will be stored
        private const string saveFile = "escape.sav";

        //Sets if the game has an error or notification to display, and what it is
        private static bool isError = false;
        private static List<string> errors = new List<string>();

        private static bool isNotification = false;
        private static List<string> notifications = new List<string>();
        #endregion

        #region Main
        public static void Main()
        {
            //Set the console width and height
            Console.WindowWidth = width;
            Console.WindowHeight = height;

            Console.BufferWidth = width;
            Console.BufferHeight = height;

        // You could use a loop here to avoid the goto but I think it's fine in this case.
        // C# does validation and prevents common errors if you follow rules like not initializing variables unnecessarily.
        gameStart:
            try
            {
                // The control flow with inner loops represents how the game works a bit better.
                // It's also much easier to follow.
                StartState();
            }
            catch (StartNewGameException)
            {
                goto gameStart;
            }
            catch (QuitGameException)
            {
                return;
            }
        }
        #endregion

        #region GameState Methods

        private static void StartState()
        {
            // Create a new world and set the player's location
            var player = new Player();
            player.LoadHandler = () =>
                {
                    Player loadedPlayer;
                    if (Load(out loadedPlayer))
                    {
                        loadedPlayer.LoadHandler = player.LoadHandler;
                        loadedPlayer.Died += ply => GameOverState();
                    }
                };
            player.Died += ply => GameOverState();

            // I'm not sure about how the player is initialized, it's a bit ugly.
            var world = new World();
            player.Location = world.StartLocation;
            player.Attacks.AddRange(world.PlayerAttacks);
            // World is not referenced after this point.
            // Not having global item lists could make debugging (cheat commands) difficult, so I need to think about that some more.

            if (isError)
            {
                DisplayError();
            }

            //Get the player's name and set the game to the playing state
            Text.WriteLine("Hello adventurer! What is your name?");
            player.Name = Text.SetPrompt("> ");
            Text.Clear();
            PlayingState(player);
        }

        private static void PlayingState(Player player)
        {
            while (true)
            {
                if (isError)
                {
                    DisplayError();
                }
                //Display any notifications
                if (isNotification)
                {
                    DisplayNotification();
                }

                //Render the main hUD
                World.LocationHUD(player);

                //Get input from the player
                string temp = Text.SetPrompt("[" + player.Location.Name + "] > ");
                Text.Clear();
                player.DoPlaying(temp);
            }
        }

        // Maybe this should be moved into the BattleCore class...
        public static void BattleState(BattleCore battleCore)
        {
            bool isEnd = false;
            while (isEnd == false)
            {
                if (isError)
                {
                    DisplayError();
                }
                //Display any notifications
                if (isNotification)
                {
                    DisplayNotification();
                }

                //Render the battle HUD, start the next turn, and check if the battle ended
                battleCore.BattleHUD();
                battleCore.NextTurn();
                battleCore.CheckResults(out isEnd);
            }
        }

        public static void QuitState()
        {
            if (isError)
            {
                DisplayError();
            }
            //Render the quit HUD
            Console.Clear();
            Text.WriteColor("`r`/-----------------------------------------------------------------------\\", false);
            Text.WriteColor("|`w`                 Are you sure you want to quit? (y/n)                  `r`|", false);
            Text.WriteColor("\\-----------------------------------------------------------------------/`w`", false);

            //If the player pressed 'y', quit. Otherwise, reset the GameState to playing
            char quitKey = Text.SetKeyPrompt();

            if (quitKey == 'y')
            {
                throw new QuitGameException("Quit from QuitState().");
            }
            else
            {
                Text.Clear();
            }
        }

        private static void GameOverState()
        {
            if (isError)
            {
                DisplayError();
            }

            //Render the game over HUD
            Console.Clear();
            Text.WriteColor("`r`/-----------------------------------------------------------------------\\", false);
            Text.WriteColor("|`w`                              Game Over!                               `r`|", false);
            Text.WriteColor("|`w`                           Try again? (y/n)                            `r`|", false);
            Text.WriteColor("\\-----------------------------------------------------------------------/`w`", false);

            //If the player pressed 'y', restart the game. Otherwise, exit the game
            ConsoleKeyInfo quitKey = Console.ReadKey();

            if (quitKey.KeyChar == 'y')
            {
                throw new StartNewGameException("Restart from GameOverState()");
            }
            throw new QuitGameException("Quit from GameOverState().");
        }
        #endregion

        #region Notification Handling
        //Displays any notifications
        private static void DisplayNotification()
        {
            //Set the cursor to one line above the bottom of the console and draw the top of the notification box
            Console.CursorTop = Console.WindowHeight - 1;
            Text.WriteColor("`g`/-----------------------------------------------------------------------\\", false);

            //Cycle through each notification in the list and display it
            foreach (string notification in notifications)
            {
                //Split each notification into multiple lines so it fits within the display box
                List<string> notificationLines = Text.Limit(string.Format("`g`Alert: `w`" + notification), Console.WindowWidth - 4);

                foreach (string line in notificationLines)
                {
                    Text.WriteColor("| `w`" + line + Text.BlankSpaces(Console.WindowWidth - Regex.Replace(line, @"`.`", "").Length - 4, true) + "`g` |", false);
                }
            }

            Text.Write("\\-----------------------------------------------------------------------/");

            //Set the cursor back to the top of the console and mark all notifications as displayed
            Console.SetCursorPosition(0, 0);
            UnsetNotification();
        }

        //Sets a message as a notification
        public static void SetNotification(string message)
        {
            notifications.Add(message);
            isNotification = true;
        }

        //Clears all notifications out of the list
        private static void UnsetNotification()
        {
            notifications.Clear();
            isNotification = false;
        }
        #endregion

        #region Error Handling
        //99% of this works the same as notifications, just adjusted slightly to be errors instead.
        private static void DisplayError()
        {
            Console.CursorTop = Console.WindowHeight - 1;
            Text.WriteColor("`r`/-----------------------------------------------------------------------\\", false);

            foreach (string error in errors)
            {
                List<string> errorLines = Text.Limit(string.Format("`r`Error: `w`" + error), Console.WindowWidth - 4);

                foreach (string line in errorLines)
                {
                    Text.WriteColor("| `w`" + line + Text.BlankSpaces(Console.WindowWidth - Regex.Replace(line, @"`.`", "").Length - 4, true) + "`r` |", false);
                }
            }

            Text.Write("\\-----------------------------------------------------------------------/");

            Console.SetCursorPosition(0, 0);
            UnsetError();
        }

        public static void SetError(string message)
        {
            errors.Add(message);
            isError = true;
        }

        private static void UnsetError()
        {
            errors.Clear();
            isError = false;
        }
        #endregion

        #region Save and Load
        //This saves the game (wow really?)
        public static void Save(Player player)
        {
            //Creates a new SaveGame object that captures all the current variables that need to be saved. See SaveGame.cs for more.
            SaveGame saveGame = new SaveGame(player);

            try
            {
                //Serializes saveGame into a save file called escape.sav
                using (Stream stream = File.Open(saveFile, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, saveGame);
                }

                Program.SetNotification("Save Successful!");
            }
            //Outputs some possible errors
            catch (AccessViolationException)
            {
                Program.SetError("Save Failed! File access denied.");
            }
            catch (Exception)
            {
                Program.SetError("Save Failed! An unspecified error occurred.");
            }
        }

        //This basically does the exact opposite of save (whodda thunk it!)
        public static bool Load(out Player player)
        {
            try
            {
                using (Stream stream = File.Open(saveFile, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    SaveGame saveGame = (SaveGame)bin.Deserialize(stream);
                    player = saveGame.Load();
                }
                Program.SetNotification("Load Successful!");
                return true;
            }
            catch (FileNotFoundException)
            {
                Program.SetError("No savegame exists!");
            }
            catch (AccessViolationException)
            {
                Program.SetError("Load failed! File access denied.");
            }
            catch (Exception)
            {
                Program.SetError("Load failed! An unspecified error occurred.");
            }
            player = default(Player);
            return false;
        }
        #endregion
    }
}