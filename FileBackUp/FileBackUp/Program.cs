using System;
using System.Collections.Generic;
using System.IO;


namespace FileBackUp
{
	class Program
	{

		public static void Main(string[] args)
		{
			//get the users desktop info so we can save the output in a folder on it later
			var userDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			//Get user input for path to New and Old .txt files
			Console.Write("Please enter the full path of the OLD sha1 file you would like to check \n" +
				"for example if I wanted to look in a file on my desktop I would type \n" +
				"C:\\Users\\{user name}\\Desktop\\Old.sha1.txt.  The path to your desktop is " + userDesktop + ": \n");
			var newFileName = Console.ReadLine();

			//Here we use isValidPath to make sure we have a valid file or path since we are relying on user input.  If there is no
			//valid path or file, we will enter do while loop until we have a valid input.
			if (isValidPath(newFileName) != true)
			{
				do
				{
					Console.WriteLine("You did not enter a valid path to the file, please check the path and type again");
					newFileName = Console.ReadLine();
				}
				while (isValidPath(newFileName) != true);
			}

			Console.Write("\nPlease enter the full path of the NEW sha1 file you would like to check \n" +
				"for example if I wanted to look in a file on my desktop I would type \n" +
				"C:\\Users\\{userName}\\Desktop\\New.sha1.txt.  The path to your desktop is " + userDesktop + ": \n");
			var oldFileName = Console.ReadLine();

			//Here we use isValidPath to make sure we have a valid file or path since we are relying on user input.  If there is no
			//valid path or file, we will enter do while loop until we have a valid input.
			if (isValidPath(oldFileName) != true)
			{
				do
				{
					Console.WriteLine("You did not enter a valid path to the file, please check the path and type again");
					oldFileName = Console.ReadLine();
				}
				while (isValidPath(oldFileName) != true);
			}

			//create new folder on users desktop
			var newFolder = Path.Combine(userDesktop, "Backup Results");
			Directory.CreateDirectory(newFolder);

			//I am using a dictionary because the input file lines contain only 2 elements a SHA-1 hash(which will be the Key and will be unique)
			//and a filename(which will be the Value), a dictionary is an easily comparable data type
			Dictionary<string, string> newFileLine = new Dictionary<string, string>();
			Dictionary<string, string> oldFileLine = new Dictionary<string, string>();

			//Here we are using tthe ReadTheFile function to create the Dictionaries we need ot pass to the compare function
			Console.WriteLine("\nReading Files. \n");
			newFileLine = ReadTheFile(newFileName);
			oldFileLine = ReadTheFile(oldFileName);

			//Here we pass the two dictionaries into the function to compare, the order in which the dictionaries are put in the function
			//determines which file is getting written
			Console.WriteLine("Comparing Files. \n");
			var oldFileWrite = CompareFunction(oldFileLine, newFileLine);
			var newFileWrite = CompareFunction(newFileLine, oldFileLine);

			#region Save Files To Desktop
			//files are saved on the users desktop in the newly create Backup Results folder
			using (var writer = new StreamWriter(newFolder + "\\OldNotInNew.txt"))
			{
				foreach (string fileName in oldFileWrite)
				{
					writer.WriteLine(fileName);
				}
			}

			using (var writer = new StreamWriter(newFolder + "\\NewNotInOld.txt"))
			{
				foreach (string fileName in newFileWrite)
				{
					writer.WriteLine(fileName);
				}
			}
			#endregion

			#region Custom Functions
			//This function validates that the file path entered is a valid one.  If the file is not found or if the path is not
			//valid in any way, we will catch the exception and set boolean to false
			bool isValidPath(string filePath)
			{
				bool isValid = true;
				try
				{
					using (var reader = new StreamReader(filePath))
					{
						string line;

						line = reader.ReadLine();
					}
				}
				catch (Exception e)
				{
					isValid = false;

				}
				return isValid;
			}
			//This function reads the file, I made this a function because we read 2 different files
			Dictionary<string, string> ReadTheFile(string fileName)
			{
				Dictionary<string, string> fileLine = new Dictionary<string, string>();
				try
				{
					using (var reader = new StreamReader(fileName))
					{
						string line;

						while ((line = reader.ReadLine()) != null)
						{
							string key = string.Empty;
							string value = string.Empty;
							//since the SHA-1 is 40 characters long we can make the first 40 characters of a line the key
							for (int i = 0; i <= 39; i++)
							{
								key += line[i];
							}
							//after the space we pick up the with the 41st character which will be the first of the filename and loop
							//until the end of the line
							for (int i = 41; i <= line.Length - 1; i++)
							{
								value += line[i];
							}
							fileLine.Add(key, value);
						}
					}
					
				}
				catch (FileNotFoundException e)
				{
					Console.WriteLine("File Could not be found, please check the path and type again" + e.Message);
					fileName = Console.ReadLine();
					
				}

				return fileLine;
			}

			//The function that will compare the two Dictionairies and return the required information
			List<string> CompareFunction(Dictionary<string, string> areThese, Dictionary<string, string> inHere)
			{
				//I am using a List because the size does not need to be declared
				List<string> FileList = new List<string>();

				foreach (var key in areThese.Keys)
				{
					//is the current Key in the comparing dictionary if not add the value to the FileList
					if (!inHere.ContainsKey(key))
					{
						var value = areThese[key];
						FileList.Add(value);
					}
				}
				return FileList;
			}
			#endregion

			#region End of Program Message
			Console.WriteLine("\n The results are on your desktop in a file named Backup Results.  You may press any key to exit.");
			Console.ReadKey();
			#endregion
		}
	}
}
