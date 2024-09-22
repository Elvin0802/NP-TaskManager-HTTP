using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace TaskManagerClientUI;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext=this;
		Processes = new();

	}
	public ObservableCollection<string> Processes { get; set; }

	private void GetBtn_Click(object sender, RoutedEventArgs e)
	{
		GetBtn.IsEnabled = false;

		Task.Run(GetProcessAsync);

		GetBtn.IsEnabled = true;
	}

	public async Task GetProcessAsync()
	{
		HttpClient client = new HttpClient();

		var message = new HttpRequestMessage
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri(@"http://localhost:27001/"),
		};

		message.Headers.Add("Accept", "application/json");

		var response = await client.SendAsync(message);

		Processes.Clear();

		var json = await response.Content.ReadAsStringAsync();


		var ps = JsonSerializer.Deserialize<List<string>>(json);
		//Processes = new ObservableCollection<string>(ps);

		//ProcessLB.Items.Add("asdasda");

		Dispatcher.Invoke(() => { LB.Items.Clear(); });
		Dispatcher.Invoke(() => {
			foreach (var item in ps)
			{
				LB.Items.Add(item);
			}
		});
		//for (int i = 0; i < ps.Count; i++)
		//{
		//	ProcessLB.Items.Add(ps[i].ToString());
		//}
		//MessageBox.Show($"{ps?.Count}");

		
	}

	private void PostBtn_Click(object sender, RoutedEventArgs e)
	{
		RunProcessWindow win = new RunProcessWindow();

		win.ShowDialog();
	}

	private void KillBtn_Click(object sender, RoutedEventArgs e)
	{
		Task.Run(KillProcessAsync);
		Task.Run(GetProcessAsync);
	}

	public async Task KillProcessAsync()
	{
		try
		{
			string? name="";

			Dispatcher.Invoke(() => {
				name = LB.SelectedItem.ToString();
			});

			HttpClient client = new HttpClient();

			var content = new StringContent($"KILL={name}", Encoding.UTF8, "text/plain");

			var response = await client.PostAsync(new Uri(@"http://localhost:27001/"), content);

			var result = await response.Content.ReadAsStringAsync();

			Thread.Sleep(1000);

			MessageBox.Show(result, "Window Result.");
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Exception: {ex.Message}", "Error");
		}
	}
}