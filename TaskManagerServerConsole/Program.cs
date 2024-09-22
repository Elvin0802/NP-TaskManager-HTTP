using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

HttpListener listener = new HttpListener();
listener.Prefixes.Add("http://localhost:27001/");
listener.Start();
Console.WriteLine("\t\tListening...\n");

try
{
	while (true)
	{
		HttpListenerContext context = listener.GetContext();
		HttpListenerRequest request = context.Request;

		string actionType = request.HttpMethod;

		using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
		{
			string content = reader.ReadToEnd();
			Console.WriteLine($"\t\tReceived: {content}\n");

			string responseMessage = "\t\tOperation failed.";

			switch (actionType)
			{
				case "POST":
					var res = content.Split('=');

					if (res[0] == "KILL")
					{
						try
						{
							var lst = Process.GetProcessesByName(res[1]).ToList();

							foreach (var i in lst)
							{
								i.Kill();
							}

							responseMessage = $"\t\tKilled process: {content}\n";
						}
						catch (Exception ex)
						{
							responseMessage = $"\t\tError starting process: {ex.Message}\n";
						}
					}
					else if (res[0] == "RUN")
					{
						try
						{
							Process.Start(res[1]);
							responseMessage = $"\t\tStarted process: {content}\n";
						}
						catch (Exception ex)
						{
							responseMessage = $"\t\tError starting process: {ex.Message}\n";
						}
					}
					break;

				case "GET":
					var processNames = Process.GetProcesses().Select((process) => process.ProcessName).ToList();

					responseMessage = JsonSerializer.Serialize(processNames);

					

					Console.WriteLine("\t\tSent running process list.\n");
					break;

				default:
					responseMessage = "\t\tUnknown request.";
					break;
			}
			StreamWriter w = new StreamWriter(context.Response.OutputStream);
			w.Write(responseMessage);
			w.Close();
		}
	}
}
catch (Exception ex)
{
	Console.WriteLine($"\n\n\t\tERROR: {ex.Message}");
}
finally
{
	Console.WriteLine("\n\n\t\tServer shut down.");
}
