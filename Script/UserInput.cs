using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PluScript.Script;

	public class UserInput
	{
		public static (string Name, string Password) GetUserInput()
		{
			string name = "";
			string password = "";

			if (File.Exists("profiles.txt"))
			{
				var data = new List<(string Name, string Password)>();
				var lines = File.ReadAllLines("profiles.txt");
				
				foreach (var line in lines)
				{
					var parts = line.Trim().Split(' ', 2);
					if (parts.Length >= 2)
					{
						data.Add((parts[0], parts[1]));
					}
				}

				Console.WriteLine("Dostępne profile:");
				Console.WriteLine("0. Utwórz nowy");
				
				for (int i = 0; i < data.Count; i++)
				{
					Console.WriteLine($"{i + 1}. {data[i].Name}");
				}

				Console.Write("Wybierz profil (numer): ");
				if (int.TryParse(Console.ReadLine(), out int choice))
				{
					if (choice == 0)
					{
						(name, password) = ReadLoginData();
					}
					else if (choice > 0 && choice <= data.Count)
					{
						(name, password) = data[choice - 1];
					}
					else
					{
						Console.WriteLine("Nieprawidłowy wybór. Używam domyślnego profilu.");
						(name, password) = data.First();
					}
				}
				else
				{
					Console.WriteLine("Nieprawidłowy wybór. Używam domyślnego profilu.");
					(name, password) = data.First();
				}
			}
			else
			{
				(name, password) = ReadLoginData();
			}

			return (name, password);
		}

		private static (string Name, string Password) ReadLoginData()
		{
			Console.Write("Podaj login lub email: ");
			string name = Console.ReadLine();
			
			Console.Write("Podaj hasło: ");
			string password = Console.ReadLine();
			
			Console.Write("Czy zapisać dane logowania? (t/n): ");
			string saveData = Console.ReadLine()?.Trim().ToLower();
			
			if (saveData == "t")
			{
				File.AppendAllText("profiles.txt", $"{name} {password}\n");
			}

			return (name, password);
		}
	}
