using System;
using System.Windows.Forms;
using DMR;

internal static class MainWindow
{
	[STAThread]
	private static void Main(string[] args)
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.Run(new MainForm(args));
	}

	private static void smethod_0(Exception exception_0)
	{
		MessageBox.Show(exception_0.Message + "\r\n" + exception_0.StackTrace, "");
	}
}
