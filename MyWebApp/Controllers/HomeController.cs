using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace MyWebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            ViewBag.ImageDiagram = await GetImageFromByteArray("webappimages", "DiagramAssignment3_S00184418.PNG");
            ViewBag.ImageHeader = await GetImageFromByteArray("webappimages", "HomeImg.jpg");
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ViewBag.Message = ip.ToString();
                }
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Address.";

            return View();
        }
        public async Task<string> GetImageFromByteArray(string folder, string file)
        {
            byte[] byteData = await ReadObjectData(folder, file);
            string imreBase64Data = Convert.ToBase64String(byteData);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);

            return imgDataURL;
        }
        public async Task<byte[]> ReadObjectData(string key, string fileName)
        {
            try
            {
                using (var client = new AmazonS3Client(System.Web.Configuration.WebConfigurationManager.AppSettings["AwsKey"].ToString(), System.Web.Configuration.WebConfigurationManager.AppSettings["AwsSecret"].ToString(), RegionEndpoint.EUWest1))
                {
                    var request = new GetObjectRequest
                    {
                        BucketName = "assignment-bucket-s00184418",
                        Key = key + "/" + fileName
                    };

                    using (var getObjectResponse = await client.GetObjectAsync(request))
                    {
                        using (var responseStream = getObjectResponse.ResponseStream)
                        {
                            var stream = new MemoryStream();
                            await responseStream.CopyToAsync(stream);
                            stream.Position = 0;
                            return stream.ToArray();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Read object operation failed.", exception);
            }
        }
    }
}