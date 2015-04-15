using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo.Console.WebThumbnail
{
	/// <remarks>
	/// <see cref="http://www.charith.gunasekara.web-sphere.co.uk/2010/09/how-to-convert-html-page-to-image-using.html"/>
	/// </remarks>
	public class Thumbnail
	{
		public string Url { get; set; }
		public Bitmap ThumbnailImage { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int BrowserWidth { get; set; }
		public int BrowserHeight { get; set; }

		public Thumbnail(string url, int browserWidth, int browserHeight, int thumbnailWidth, int thumbnailHeight)
		{
			Url = url;
			BrowserWidth = browserWidth;
			BrowserHeight = browserHeight;
			Height = thumbnailHeight;
			Width = thumbnailWidth;
		}

		//public Bitmap GenerateThumbnail()
		//{
		//	Thread thread = new Thread(GenerateThumbnailInteral);
		//	thread.SetApartmentState(ApartmentState.STA);
		//	thread.Start();
		//	thread.Join();
		//	return ThumbnailImage;
		//}

		public async Task<Bitmap> GenerateThumbnailAsync()
		{
			// This is not good because a thread should run on STA model.
			//return await Task.Run(() =>
			//{
			//	GenerateThumbnailInteral();
			//	return ThumbnailImage;
			//});

			return await StartStaTask(() =>
			{
				GenerateThumbnailInteral();
				return ThumbnailImage;
			});
		}

		/// <remarks><see cref="http://stackoverflow.com/a/16722767/4035"/></remarks>
		private static Task<T> StartStaTask<T>(Func<T> func)
		{
			var tcs = new TaskCompletionSource<T>();
			Thread thread = new Thread(() =>
			{
				try
				{
					tcs.SetResult(func());
				}
				catch (Exception e)
				{
					tcs.SetException(e);
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			return tcs.Task;
		}

		private void GenerateThumbnailInteral()
		{
			WebBrowser webBrowser = new WebBrowser {ScrollBarsEnabled = false};
			webBrowser.Navigate(Url);
			webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;

			while (webBrowser.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();

			webBrowser.Dispose();
		}


		private static int counter = 0;
		private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			WebBrowser webBrowser = (WebBrowser)sender;
			webBrowser.ClientSize = new Size(BrowserWidth, BrowserHeight);
			webBrowser.ScrollBarsEnabled = false;
			ThumbnailImage = new Bitmap(webBrowser.Bounds.Width, webBrowser.Bounds.Height);
			webBrowser.BringToFront();
			webBrowser.DrawToBitmap(ThumbnailImage, webBrowser.Bounds);
			ThumbnailImage = (Bitmap)ThumbnailImage.GetThumbnailImage(Width, Height, null, IntPtr.Zero);
			System.Console.WriteLine("Counter: {0}", counter);
			counter++;
		}
	}
}
