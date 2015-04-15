using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Demo.Console.WebThumbnail
{
	class Program
	{
		static void Main(string[] args)
		{
			CreateThumbnail();
		}

		private static async Task CreateThumbnail()
		{
			//string url = string.Format("http://{0}", "weitzlux.com");
			string url = "http://www.charith.gunasekara.web-sphere.co.uk/2010/09/how-to-convert-html-page-to-image-using.html";
			int thumbnailWidth = 800;
			int thumbnailHeight = 600;
			Thumbnail thumbnail = new Thumbnail(url, 800, 600, thumbnailWidth, thumbnailHeight);
			Bitmap image = await thumbnail.GenerateThumbnailAsync();
			image.Save(@"c:\temp\Thumbnail.jpg", ImageFormat.Jpeg);
		}
	}
}
