using Microsoft.Win32;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace TaskManagerClientUI;

public partial class RunProcessWindow : Window
{
	public RunProcessWindow()
	{
		InitializeComponent();
	}

	public void Ok(object sender, RoutedEventArgs e)
	{
		Task.Run(PostProcessAsync);
	}

	public async Task PostProcessAsync()
	{
		try
		{
			string? name = "";

			Dispatcher.Invoke(() => { name = AppNameTBox.Text; });

			HttpClient client = new HttpClient();

			var content = new StringContent($"RUN={name}", Encoding.UTF8, "text/plain");

			var response = await client.PostAsync(new Uri(@"http://localhost:27001/"), content);

			var result = await response.Content.ReadAsStringAsync();

			Thread.Sleep(1000);

			MessageBox.Show(result, "Window Result.");

			Dispatcher.Invoke(() => { AppNameTBox.Text = ""; });
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Exception: {ex.Message}", "Error");
		}
		finally
		{
			Cancel(null, null);
		}
	}

	public async void Cancel(object sender, RoutedEventArgs e)
	{
		try
		{
			Dispatcher.Invoke(() => { this.Close(); });
		}
		catch { MessageBox.Show("Error in Cancel Command.", "Window Error."); }
	}

	public void Browse(object sender, RoutedEventArgs e)
	{
		try
		{
			var fileDialog = new OpenFileDialog();

			fileDialog.Filter = "(*.exe)|*.exe";

			fileDialog.ShowDialog();

			AppNameTBox.Text = fileDialog.FileName;
		}
		catch { MessageBox.Show("Error in Browse Command.", "Window Error."); }
	}

}
