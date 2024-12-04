using System;
using System.Net;
using System.Timers;

namespace DMR;

public class WebClientAsync : WebClient
{
	private int timeoutMS;

	private Timer timer;

	public WebClientAsync(int timeoutSeconds)
	{
		WebClientAsync webClientAsync = this;
		timeoutMS = timeoutSeconds * 1000;
		timer = new Timer(timeoutMS);
		ElapsedEventHandler handler = null;
		handler = delegate
		{
			webClientAsync.CancelAsync();
			webClientAsync.timer.Stop();
			webClientAsync.timer.Elapsed -= handler;
		};
		timer.Elapsed += handler;
		timer.Enabled = true;
	}

	protected override WebRequest GetWebRequest(Uri address)
	{
		WebRequest webRequest = base.GetWebRequest(address);
		webRequest.Timeout = timeoutMS;
		((HttpWebRequest)webRequest).ReadWriteTimeout = timeoutMS;
		return webRequest;
	}

	protected override void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
	{
		base.OnDownloadProgressChanged(e);
		timer.Stop();
		timer.Start();
	}
}
