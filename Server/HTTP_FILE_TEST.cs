using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TreasureHunt.Models;
using TreasureHunt.Services;

// using Azure.Storage.Blobs;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Azure.WebJobs;
// using Microsoft.Azure.WebJobs.Extensions.Http;
// using Microsoft.Extensions.Logging;
// using System;
// using System.IO;
// using System.Threading.Tasks;

namespace TreasureHunt.Function;
public class FILE_TEST
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _IViewModelService; // TODO is this being used? remove iv not
  public FILE_TEST(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _IViewModelService = viewModelService;
  }

  [FunctionName("File_Test")]
  public async Task<IActionResult> FileTest(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "test")] HttpRequest req, ILogger log)
  {
    // Check if we have authentication info.
    ValidateJWT auth = new ValidateJWT(req);
    if (!auth.IsValid)
    {
      return new UnauthorizedResult(); // No authentication info.
    }

    // string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
    // string containerName = Environment.GetEnvironmentVariable("ContainerName");
    // MemoryStream myBlob = new MemoryStream();
    string result = await FileSystemService.SaveImageToLocalFileSystem(req, "path", "path2");
    // var file = req.Form.Files["File"];
    if (result == null)
    {
      return new BadRequestResult();
    }
    // myBlob = file.OpenReadStream();
    // MemoryStream myBlob = new MemoryStream();
    // FileStream savedFile = new FileStream("c:\\Projects\\" + file.FileName, FileMode.Create, FileAccess.Write);
    // myBlob.WriteTo(savedFile);
    // file.CopyTo(savedFile);
    // savedFile.Close();
    // myBlob.Close();
    // var blobClient = new BlobContainerClient(Connection, containerName);
    // var blob = blobClient.GetBlobClient(file.FileName);
    // await blob.UploadAsync(myBlob);

    //     using System;
    //     using System.Text;
    //     using System.Windows.Forms;
    //     using System.IO;
    // namespace WindowsFormsApplication2
    // {
    //   public partial class Form1 : Form
    //   {
    //     public Form1()
    //     {
    //       InitializeComponent();
    //     }
    //     private void button1_Click(object sender, EventArgs e)
    //     {
    //       string memString = "Memory test string !!";
    //       // convert string to stream
    //       byte[] buffer = Encoding.ASCII.GetBytes(memString);
    //       MemoryStream ms = new MemoryStream(buffer);
    //       //write to file
    //       FileStream file = new FileStream("d:\\file.txt", FileMode.Create, FileAccess.Write);
    //       ms.WriteTo(file);
    //       file.Close();
    //       ms.Close();
    //     }
    //   }
    // }


    string responce;
    if (!String.IsNullOrEmpty(result))
    {
      responce = "Filename is " + result;
    }
    else
    {
      responce = "Something went wrong";
    }
    return new OkObjectResult(responce);

    // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    // dynamic data = JsonConvert.DeserializeObject(requestBody);
    // if (!data.ContainsKey("Title"))
    // {
    //   return new BadRequestResult();
    // }
    // string title = data.Title;
    // IActionResult result = await _databaseService.CreateHunt(auth.UserId, title);
    // if (!result.GetType().Equals(typeof(OkObjectResult)))
    // {
    //   return result;
    // }
    // OkObjectResult resultObject = result as OkObjectResult;
    // Hunt resultHunt = resultObject.Value as Hunt;
    // return new OkObjectResult(await _IViewModelService.To_Hunt_ViewModel(resultHunt));
  }
}

// public static class FileUpload
// {
//   [FunctionName("FileUpload")]
//   public static async Task<IActionResult> Run(
//       [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
//   {
//     string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
//     string containerName = Environment.GetEnvironmentVariable("ContainerName");
//     Stream myBlob = new MemoryStream();
//     var file = req.Form.Files["File"];
//     myBlob = file.OpenReadStream();
//     var blobClient = new BlobContainerClient(Connection, containerName);
//     var blob = blobClient.GetBlobClient(file.FileName);
//     await blob.UploadAsync(myBlob);
//     return new OkObjectResult("file uploaded successfylly");
//   }
// }