using PLUploadDemo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PLUploadDemo.Controllers
{
    public class PLDemoController : Controller
    {
        // GET: PLDemo
        public ActionResult Index()
        {
            return View();
        }


        #region PLUploader

        [HttpPost]
        public ActionResult PLUpload(FileUploadModel model)
        {
            FileUploadResultModel result = new FileUploadResultModel();
            result.result = true;

            HttpPostedFileBase file = model.postedFile;

            if (null != file && file.ContentLength > 0)
            {
                try
                {
                    if (model.chunks.HasValue && model.chunk.HasValue)
                    {
                        result = ChunkSaveFile(model, result);
                    }
                    else
                    {
                        result = SaveFile(model, result);
                    }
                }
                catch (Exception ex)
                {
                    result.result = false;
                    result.result_text = ex.Message;
                }
            }
            else
            {
                result.result = false;
                result.result_text = "无文件";
            }

            return Json(result);
        }
        [NonAction]
        private FileUploadResultModel SaveFile(FileUploadModel model, FileUploadResultModel result)
        {
            Stream uploadStream = null;
            FileStream fs = null;
            HttpPostedFileBase postFileBase = model.postedFile;
            uploadStream = postFileBase.InputStream;

            result = CreateNewFileName(model, result);
            string newFileFullPath = Server.MapPath(result.newFilePath) + result.newFileName;
            result.oldFileName = model.name;

            //文件上传，一次上传1M的数据，防止出现大文件无法上传  
            int bufferLen = 1024 * 1024;
            byte[] buffer = new byte[bufferLen];
            int contentLen = 0;

            // 保存到服务器文件夹
            fs = new FileStream(newFileFullPath, FileMode.Create, FileAccess.ReadWrite);
            while ((contentLen = uploadStream.Read(buffer, 0, bufferLen)) != 0)
            {
                fs.Write(buffer, 0, contentLen);
                fs.Flush();
            }
            fs.Close();
            fs.Dispose();
            //// 另外简单方式
            //file.SaveAs(newFileFullPath);

            return result;
        }
        [NonAction]
        private FileUploadResultModel ChunkSaveFile(FileUploadModel model, FileUploadResultModel result)
        {
            Stream uploadStream = null;
            FileStream fs = null;

            HttpPostedFileBase postFileBase = model.postedFile;
            uploadStream = postFileBase.InputStream;

            FileUploadResultModel currentFileUploadInfo = result;
            if (0 == model.chunk)
            {// 第一次创建文件名
                result = CreateNewFileName(model, result);
                Session[model.fileFlag] = result;
            }
            else
            {
                currentFileUploadInfo = Session[model.fileFlag] as FileUploadResultModel;
            }
            currentFileUploadInfo.oldFileName = model.name;

            string newFileFullPath = Server.MapPath(currentFileUploadInfo.newFilePath) + currentFileUploadInfo.newFileName;


            int bufferLen = postFileBase.ContentLength;
            byte[] buffer = new byte[bufferLen];
            int contentLen = 0;

            // 保存到服务器文件夹
            fs = new FileStream(newFileFullPath, FileMode.Append, FileAccess.Write);
            while ((contentLen = uploadStream.Read(buffer, 0, bufferLen)) != 0)
            {
                fs.Write(buffer, 0, contentLen);
                fs.Flush();
            }
            fs.Close();
            fs.Dispose();

            // 最后一次重命名
            if ((model.chunk.Value + 1) == model.chunks.Value)
            {
                FileInfo fi = new FileInfo(newFileFullPath);
                var ext = Path.GetExtension(model.name);
                var newName = newFileFullPath + ext;
                currentFileUploadInfo.newFileName += ext;
                fi.MoveTo(Path.Combine(newName));
            }

            return currentFileUploadInfo;
        }
        [NonAction]
        private FileUploadResultModel CreateNewFileName(FileUploadModel model, FileUploadResultModel result)
        {
            // 文件夹
            var fileFolder = "/Upload/Common/";
            if (!string.IsNullOrEmpty(model.folder))
            {
                fileFolder = model.folder;
            }

            // 文件路径
            string fileName = Path.GetFileName(model.postedFile.FileName);
            string imageBasePath = fileFolder + (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["filePath"]) ? DateTime.Now.ToString("yyyy/MM/dd/") : DateTime.Now.ToString(ConfigurationManager.AppSettings["filePath"]));
            result.newFilePath = imageBasePath;
            string imagePath = Server.MapPath(imageBasePath);
            if (!Directory.Exists(imagePath))
                Directory.CreateDirectory(imagePath);

            // 新文件名
            string newFileNameBase = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["newFileName"]) ? "yyyyMMddHHmmssfff" : ConfigurationManager.AppSettings["newFileName"];
            string newFileName = DateTime.Now.ToString(newFileNameBase) + "_o_" + Path.GetExtension(fileName);
            result.newFileName = newFileName;

            return result;
        }

        #endregion

    }
}