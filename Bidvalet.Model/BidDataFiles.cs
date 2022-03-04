using System;
namespace Bidvalet.Model
{
    public class BidDataFiles
    {
        public BidDataFiles()
        {
        }
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public bool IsError { get; set; }
    }
}
