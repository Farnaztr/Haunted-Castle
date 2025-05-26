using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using NAudio.Wave;

namespace HauntedCastle
{
    class Program
    {
        static volatile bool isPaused = false;
        static volatile bool speedUp = false;
        static void HandleKeyPress(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Spacebar)
                        isPaused = !isPaused;
                    else if (key == ConsoleKey.Enter)
                        speedUp = true;
                    else if (key == ConsoleKey.Backspace)
                        speedUp = false;
                }

                Thread.Sleep(50);
            }
        }

        static void TypeWrite(string text, int normalDelay = 60, int fastDelay = 10)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Thread inputThread = new Thread(() => HandleKeyPress(cts.Token));
            inputThread.Start();

            foreach (char c in text)
            {
                while (isPaused)
                    Thread.Sleep(90);

                Console.Write(c);
                Thread.Sleep(speedUp ? fastDelay : normalDelay);
            }

            cts.Cancel();
            inputThread.Join();
        }



        static void PlayMusic(string filePath)
        {
            try
            {
                while (true)
                {


                    using (var reader = new Mp3FileReader(filePath))
                    using (var waveOut = new WaveOutEvent())
                    {
                        waveOut.Init(reader);
                        waveOut.Play();
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                            Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error playing music: " + ex.Message);
            }
        }

        static void WriteBoxedText(string[] lines)
        {
            int maxLen = 0;
            foreach (string line in lines)
                if (line.Length > maxLen)
                    maxLen = line.Length;

            string top = "╔" + new string('═', maxLen + 2) + "╗";
            string bottom = "╚" + new string('═', maxLen + 2) + "╝";

            Console.WriteLine(top);
            foreach (string line in lines)
            {
                string padded = line.PadRight(maxLen);
                Console.Write("║ ");
                TypeWrite(padded, 30, 10);
                Console.WriteLine(" ║");
                Thread.Sleep(200);
            }
            Console.WriteLine(bottom);
        }

        static void EndGame()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            TypeWrite("\nYour answer was wrong...");
            Thread.Sleep(1500);
            TypeWrite("\nThe pages of the book slam shut and vanish into smoke.");
            Thread.Sleep(2000);
            TypeWrite("\nYou failed to uncover the secrets.");
            Console.ResetColor();
            Console.ReadKey();
        }

        static void Main()
        {
            string musicUrl = "https://www.fesliyanstudios.com/download-link.php?src=i&id=186";
            string filePath = "haunted.mp3";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    var audioBytes = client.GetByteArrayAsync(musicUrl).Result;
                    File.WriteAllBytes(filePath, audioBytes);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                return;
            }

            Thread musicThread = new Thread(() => PlayMusic(filePath));
            musicThread.Start();

            Console.CursorVisible = false;
            bool isWhite = false;
            int count = 8, change = 100, blink = 0;

            while (count > 0)
            {
                Console.BackgroundColor = isWhite ? ConsoleColor.Black : ConsoleColor.White;
                Console.ForegroundColor = isWhite ? ConsoleColor.White : ConsoleColor.Black;
                Console.Clear();

                Console.SetCursorPosition(61, 14);
                Console.WriteLine("** H A U N T E D   C A S T L E **");
                Console.SetCursorPosition(59, 22);
                Console.WriteLine($"  The game will start in {count} seconds...");

                Thread.Sleep(change);
                isWhite = !isWhite;
                blink++;

                if (blink >= 8)
                {
                    count--;
                    blink = 0;
                }
            }

            Console.Clear();
            Console.SetCursorPosition(61, 20);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" Welcome to the Haunted Castle Game!");
            Console.SetCursorPosition(65, 29);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" press any key to continue..");
            Console.ReadKey();
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(0, 3);
            Console.WriteLine("STORY:");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nSpacebar->pause, Enter->SpeedUp (Backspace->normal)");
            Console.ForegroundColor = ConsoleColor.White;

            TypeWrite("\nYou step into a forest that everyone is afraid of.\nThey say no one has ever walked through it and come out alive.\nThe wind blowing through the dry branches sounds like strange, whispering voices in your ears.\nYou keep walking, and suddenly the sound of rustling leaves makes your heart beat faster.\nYou feel like someone is following you, but when you turn around, no one is there.\nYour flashlight is getting weak, and the darkness is growing. Your eyes catch a cold, blue glow in the distance.\nAs you get closer, you find an old wooden cabin with a faint light coming from inside.\nYou open the door and hear slow, heavy breathing.\nInside, there's a wooden table with a large, ancient book on it.\nOn the cover, it says:\n\n\n                                                              *Secrets of the Forbidden Forest*\n\n\nYou touch the book, and a whispering voice says :\n\n\n\n                       (If you're looking for the truth, open this book... but be careful! each page holds a riddle you must solve...)");
            Console.SetCursorPosition(61, 33);
            TypeWrite("\nNow...");
            Console.Clear();

            TypeWrite("\n You slowly reach out and place your hand on the book...");
            Thread.Sleep(1000);
            TypeWrite("\n Suddenly, the light in the cabin flickers.");
            Thread.Sleep(1000);
            TypeWrite("\n You open the first page, and a glowing blue symbol appears...");
            Thread.Sleep(1000);
            TypeWrite("\n A whispering voice echoes through the room:");
            Thread.Sleep(1500);
            TypeWrite("\n \"Solve the riddles... and learn the truth of the haunted castle.\"");
            Thread.Sleep(2000);
            Console.Clear();

            TypeWrite("\n RIDDLE 1 ");
            Thread.Sleep(1000);
            TypeWrite("\nI breathe without lungs,\r\nI feed on memories,\r\nI echo your fears,\r\nyet I'm chained beneath your ribs.");
            TypeWrite("\n\nType your answer: ");
            string answer1 = Console.ReadLine().ToLower().Trim();

            if (!answer1.Contains("soul"))
            {
                EndGame();
                return;
            }

            Console.Clear();

         
            TypeWrite("\n RIDDLE 2 ");
            Thread.Sleep(1000);
            TypeWrite("\nI show you who you are,\r\nBut twist the truth behind the eyes.\r\nYou look to me for answers,\r\nBut only find yourself disguised.\r\nWhat am I?\r\n\r\n");
            TypeWrite("\n\nType your answer: ");
            string answer2 = Console.ReadLine().ToLower().Trim();

            if (!answer2.Contains("mirror"))
            {
                EndGame();
                return;
            }

            Console.Clear();

          
            TypeWrite("\n RIDDLE 3 ");
            Thread.Sleep(1000);
            TypeWrite("\nI exist between heartbeat and breath,\r\nA shadow in light, a spark in death.\r\nCall me, and I am gone.\r\nForget me, and I remain.\r\nWhat am I?\r\n\r\n");
            TypeWrite("\n\nType your answer: ");
            string answer3 = Console.ReadLine().ToLower().Trim();

            if (!answer3.Contains("thought"))
            {
                EndGame();
                return;
            }

            Console.Clear();

            string[] story = new string[]
            {
                "The truth behind the cursed castle is finally revealed...",
                "Centuries ago, the castle belonged to Altherion, a sorcerer obsessed with eternal knowledge.",
                "But knowledge came at a price.",
                "In his greed, Altherion opened a forbidden portal — a gateway to the realm of forgotten spirits.",
                "Darkness poured in. The castle twisted. Reality bent.",
                "To stop the spread, Altherion trapped his own soul in a magical tome, locking it with 3 ancient riddles.",
                "Only a mind sharp enough to solve them would be worthy to uncover the truth — and possibly set him free.",
                "",
                "You were chosen by the book.",
                "Your answers awakened the castle's last memory.",
                "The riddles weren’t just tests — they were the keys to unlocking the sorcerer’s past.",
                "And now you know the truth:",
                "",
                "The castle isn’t haunted...",
                "It’s alive.",
                "",
                "And as you stare at the glowing map, you realize...",
                "You were never meant to escape the riddles — you were meant to continue them.",
                "Because the book is closed for now...",
                "But the castle is still watching."
            };

            WriteBoxedText(story);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\nTHE END.");
            Console.ReadKey();
            Console.ResetColor();

            musicThread.Join();
        }
    }
}
