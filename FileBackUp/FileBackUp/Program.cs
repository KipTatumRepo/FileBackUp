using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FileBackUp
{
	class Program
	{

		public static void Main(string[] args)
		{
			//get the users desktop info so we can save the output in a folder on it later
			var userDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			//create file names
			var newFileName = "..\\..\\Resources\\New.sha1.txt";
			var oldFileName = "..\\..\\Resources\\Old.sha1.txt";

			//create new folder on users desktop
			var newFolder = Path.Combine(userDesktop, "Backup Results");
			Directory.CreateDirectory(newFolder);

			//I am using a dictionary because the input file lines contain only 2 elements a SHA-1 hash(which will be the Key)
			//and a filename(which will be the Value), a dictionary is an easily comparable data type
			Dictionary<string, string> newFileLine = new Dictionary<string, string>();
			Dictionary<string, string> oldFileLine = new Dictionary<string, string>();

			//Here we are using tthe ReadTheFile function to create the Dictionaries we need ot pass to the compare function
			newFileLine = ReadTheFile(newFileName);
			oldFileLine = ReadTheFile(oldFileName);

			//Here we pass the two dictionaries into the function to compare, the order in which the dictionaries are put in the function
			//determines which file is getting written
			var oldFileWrite = CompareFunction(oldFileLine, newFileLine);
			var newFileWrite = CompareFunction(newFileLine, oldFileLine);

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

			//This function reads the file, I made this a function because we read 2 different files
			Dictionary<string, string> ReadTheFile(string fileName)
			{
				Dictionary<string, string> fileLine = new Dictionary<string, string>();
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

			Console.WriteLine("The results are on your desktop in a file named Backup Results.  You may press any key to exit.");
			Console.ReadKey();
		}
	}
}
