using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

class Backup 
{
	private readonly IConfiguration _configuration;
	public Backup(IConfiguration configuration)
	{
		_configuration = configuration;
	}


	public void CreateBackup()
	{
		var sourceFolder = _configuration["sourcePath"];
		var targetFolder = _configuration["targetPath"];

		if (!Directory.Exists(sourceFolder))
		{
			Console.WriteLine($"Исходная папка '{sourceFolder}' не найдена.");
			return;
		}

		if (!Directory.Exists(targetFolder))
		{
			Directory.CreateDirectory(targetFolder);
		}

		var backupFolder = Path.Combine(targetFolder, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        Directory.CreateDirectory(backupFolder);

		try
		{
			foreach (var file in Directory.GetFiles(sourceFolder))
			{
				try
				{
					File.Copy(file, Path.Combine(backupFolder, Path.GetFileName(file)), true);
					Console.WriteLine($"Файл '{file}' скопирован успешно.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ошибка при копировании файла '{file}': {ex.Message}");
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Общая ошибка при копировании: {ex.Message}");
		}
	}
}

class Program
{
	static void Main()
	{
		string exePath = Assembly.GetExecutingAssembly().Location;
		string exeDir = Path.GetDirectoryName(exePath);
		var configurationPath = Path.Combine(exeDir, "config", "jsconfig1.json");
		var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(configurationPath)).AddJsonFile("jsconfig1.json", optional: false, reloadOnChange: true);

		IConfiguration configuration = builder.Build();

		var backup = new Backup(configuration);
		backup.CreateBackup();
		Console.Write("Press <Enter> to exit");
		Console.ReadLine();
	}
}