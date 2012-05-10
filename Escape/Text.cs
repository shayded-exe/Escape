using System;
using System.Collections.Generic;

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
		
		public static void Clear()
		{
			Console.Clear();
		}
		#endregion
	}
}