using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PLUploadDemo.Models
{
    public class FileUploadModel
    {
        [Required]
        public string fileFlag { get; set; }

        public string folder { get; set; }

        [Required]
        public HttpPostedFileBase postedFile { get; set; }


        public string name { get; set; }
        public int? chunk { get; set; }
        public int? chunks { get; set; }

    }
    public class FileUploadResultModel
    {
        public bool result { get; set; }
        public string result_text { get; set; }


        public string newFilePath { get; set; }

        public string newFileName { get; set; }
        public string oldFileName { get; set; }

    }


}