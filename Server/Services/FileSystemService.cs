using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TreasureHunt.Models;

namespace TreasureHunt.Services;
public static class FileSystemService
{
  private static string basePath = "c:\\Projects";
  private static string imageName = "Image";
  /**
   * Takes in a request with a file attached with the key "Image", a User id
   * and either a Hunt id or huntObject id and saves it to the filesystem.
   * The file is saved with the following path {{BasePath}}/userGUID/itemGUID/file.FileName
   */
  public static async Task<string> SaveImageToLocalFileSystem(HttpRequest req, string userGUID, string itemGUID)
  {
    var file = req.Form.Files["Image"];
    if (file == null)
    {
      return null;
    }
    // myBlob = file.OpenReadStream();
    // MemoryStream myBlob = new MemoryStream();

    // To remove you would search c:\\Projects\\UserGUID\\Hunt-GUID or Huntobject-GUID then the only file it finds??
    string extension = System.IO.Path.GetExtension(file.FileName);
    // TODO Only allow specified extensions
    string fileName = basePath + "\\" + userGUID + "\\" + itemGUID + "\\" + imageName + extension;
    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
    FileStream savedFile = new FileStream(fileName, FileMode.Create, FileAccess.Write);
    file.CopyTo(savedFile);
    savedFile.Close();

    return file.FileName;
  }

  public static string DeleteImageFromLocalFileSystem(string userGUID, string itemGUID)
  {
    System.IO.DirectoryInfo di = new DirectoryInfo(basePath + "\\" + userGUID + "\\" + itemGUID);
    try
    {
      di.Delete(true);
    }
    catch (Exception)
    {
      return null;
    }
    return "Deleted " + itemGUID;
  }

  public static string GetImagePathFromLocalFileSystem(string userGUID, string itemGUID)
  {
    string path = basePath + "\\" + userGUID + "\\" + itemGUID;
    if (Directory.Exists(path))
    {
      // Process the list of files found in the directory.
      return Directory.GetFiles(path)[0];
    }
    return null;
  }
}