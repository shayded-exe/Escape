using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Escape
{
	static class Text
	{
		#region Declarations
		
		#endregion
		
		#region Public Methods
		public static string SetPrompt(string aString)
		{
			Text.Write(String.Format(aString));
			
			return Console.ReadLine();
		}

		public static char SetKeyPrompt(string aString = "")
		{
			Text.Write(string.Format(aString));

			return Console.ReadKey().KeyChar;
		}
		
		public static void Write(string aString)
		{			
			Console.Write(aString);
		}
		
		public static void WriteLine(string aString, bool keepIndent = true)
		{
			int curX = Console.CursorLeft;
			
			Console.WriteLine(aString);
			
			if (keepIndent)
			{
				Console.CursorLeft = curX;
			}
		}
	    
		public static void WriteColor(string aString, bool newLine = true, bool keepIndent = true)
		{
			int curX = Console.CursorLeft;
			string[] segments = aString.Split('`');
			for (int i = 0; i < segments.Length; i++)
			{
				if (i % 2 == 0)
				{
					Text.Write(segments[i]);
				}
				else
				{
					switch(segments[i])
					{
						case "r":
							Console.ForegroundColor = ConsoleColor.Red;
							break;
						case "dr":
							Console.ForegroundColor = ConsoleColor.DarkRed;
							break;
						case "m":
							Console.ForegroundColor = ConsoleColor.Magenta;
							break;
						case "dm":
							Console.ForegroundColor = ConsoleColor.DarkMagenta;
							break;
						case "y":
							Console.ForegroundColor = ConsoleColor.Yellow;
							break;
						case "dy":
							Console.ForegroundColor = ConsoleColor.DarkYellow;
							break;
						case "c":
							Console.ForegroundColor = ConsoleColor.Cyan;
							break;
						case "dc":
							Console.ForegroundColor = ConsoleColor.DarkCyan;
							break;
						case "b":
							Console.ForegroundColor = ConsoleColor.Blue;
							break;
						case "db":
							Console.ForegroundColor = ConsoleColor.DarkBlue;
							break;
						case "g":
							Console.ForegroundColor = ConsoleColor.Green;
							break;
						case "dg":
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							break;
						case "gr":
							Console.ForegroundColor = ConsoleColor.Gray;
							break;
						case "dgr":
							Console.ForegroundColor = ConsoleColor.DarkGray;
							break;
						case "bl":
							Console.ForegroundColor = ConsoleColor.Black;
							break;
						case "w":
							Console.ForegroundColor = ConsoleColor.Gray;
							break;
						default:
							Text.Write(segments[i]);
							break;
					}
				}
			}
			if (newLine)
			{
				Text.WriteLine("", false);
				
				if (keepIndent)
				{
					Console.CursorLeft = curX;
				}
			}
		}
		
		public static void WriteAt(string aString, int x, int y, bool fromOrigin = false, bool color = false)
		{
		    int origX = Console.CursorLeft;
		  	int origY = Console.CursorTop;
		  	
		  	if (fromOrigin)
		  	{
		  		Console.SetCursorPosition(0 + x, 0 + y);
		  	}
		  	else
		  	{
		    	Console.SetCursorPosition(origX + x, origY + y);
		  	}
		    
		    if (color)
		    {
		    	Text.WriteColor(aString, false);
		    }
		    else
		    {
		    	Text.Write(aString);
		    }
		    
		    Console.SetCursorPosition(origX, origY);
	    }
	    
		public static void BlankLines(int num = 1)
		{		
			string temp = "";
			
			for (int i = 0; i < num; i++)
			{
				temp += "\n";
			}
			
			Text.Write(temp);
		}
		
		public static string BlankSpaces(int num = 1, bool returnOnly = true)
		{
			string temp = "";
			
			for (int i = 0; i < num; i++)
			{
				temp += " ";
			}
			
			if (returnOnly)
			{
				return temp;
			}
			
			Text.Write(temp);
			return temp;
		}
		
		public static List<string> Limit(string aString, int limit)
		{
			string[] words = aString.Split(' ');
			List<string> newSentence = new List<string>();
			string line = "";
			
			foreach (string word in words)
			{
				string newWord = Regex.Replace(word, @"`.`", "");
				string newLine = Regex.Replace(line, @"`.`", "");
			    if ((newLine + newWord).Length > limit)
			    {
			        newSentence.Add(line);
			        line = "";
			    }
			
			    line += string.Format("{0} ", word);
			}
			
			if (line.Length > 0)
			{
				newSentence.Add(line);
			}
			
			return newSentence;
		}
		
		public static string ToBar(int num, decimal maxNum, int length)
		{
			string barString = ("");
			int barNum = 0;
			
			for (decimal i = num; i > 0; i -= maxNum / length)
			{
				if (i < maxNum / length)
				{
					barString += ("=");
					
					if (i < maxNum / (length * 2))
					{
						barString += ("\b-");
					}
					
					barNum++;
					break;
				}
				
				barString += ("≡");
				barNum++;
			}
			
			for (int i = barNum; i < length; i++)
			{
				barString += (" ");
			}
			
			return barString;
		}

		public static string AorAn(string word)
		{
			if (Text.isVowel(word[0]))
			{
				return "an " + word;
			}
			else
			{
				return "a " + word;
			}
		}
		
		public static void Clear()
		{
			Console.Clear();
		}
		#endregion

		#region Helper Methods
		private static bool isVowel(char letter)
		{
            // There is a good amount of words where this doesn't hold,
            // u can be a combined consonant-vovel sound and y can start with a vovel.
            // It's probably a good idea to just have fixed variants for names (or a name class that stores relevant information on a case-by-case basis).
			return "aeiouAEIOU".IndexOf(letter) >= 0;
		}
		#endregion
	}
}