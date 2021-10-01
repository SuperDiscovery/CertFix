using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CertFix
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}
		private void 导入缺失证书_Click(object sender, RoutedEventArgs e)
		{
			int errorLevel;
			string certtext;
			ProcessStartInfo processInfo;
			Process process;

			this.Cursor = Cursors.Wait;

			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "CertFix.isrg-root.cer";

			try
			{
				using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				using (StreamReader reader = new StreamReader(stream))
				{
					certtext = reader.ReadToEnd();
				}
				// 			using (var reader = new StreamReader(assembly.GetManifestResourceStream(resourceName),
				//    Encoding.ASCII))
				// {
				// 	// Encoding encod = Encoding.UTF8;
				// 	// byte[] bts = encod.GetBytes(reader.ReadToEnd());
				// 	// certtext = Encoding.UTF8.GetString(bts);
				// 	certtext = reader.ReadToEnd();
				// }
				//Pass the filepath and filename to the StreamWriter Constructor
				StreamWriter sw = new StreamWriter("isrg.cer");
				// StreamWriter sw = new StreamWriter(new FileStream("isrg.cer", FileMode.Open, FileAccess.ReadWrite), Encoding.ASCII);
				//Write a line of text
				sw.Write(certtext);
				sw.Close();
				// MessageBox.Show(Directory.GetCurrentDirectory() + certtext);

				processInfo = new ProcessStartInfo("powershell.exe", "Import-Certificate -Filepath isrg.cer -CertStoreLocation cert:\\LocalMachine\\Root");
				processInfo.CreateNoWindow = true;
				processInfo.UseShellExecute = false;

				process = Process.Start(processInfo);
				process.WaitForExit();

				errorLevel = process.ExitCode;
				process.Close();

				this.Cursor = Cursors.Arrow;

				MessageBox.Show("导入成功！");
			}
			catch (Exception err)
			{
				MessageBox.Show("Exception: " + err.Message);
			}
			finally
			{
				Console.WriteLine("Executing finally block.");
			}
		}
	}
}
