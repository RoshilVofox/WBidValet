using Bidvalet.iOS;
using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Bidvalet.iOS.Utility
{
    public class CommonClass
    {
		public static BidAwardView objAward;

        public enum RequestTypes
        {
            DownnloadBid = 0,
            DownloadAward,
            SubmitBid,
            ScrapMissingTrip,
            FromReparse,
            DownnloadHostoricalBid,
            Other

        }

        public static void SaveFormatBidReceipt(string result)
        {

           
            string lineString = string.Empty;
            string employeename = result.Substring(0, result.IndexOf("\n"));
            string fileName = employeename + "Rct";
            string footer = string.Empty;
            List<string> lists = result.Split('*').ToList();
            lists.RemoveAt(0);
            foreach (var item in lists)
            {
                footer += item;
            }
            StringBuilder resultHeadingString = new StringBuilder();
            List<StringBuilder> content = new List<StringBuilder>();

            List<PrintBidReceipt> lstPrintBidReceipt = new List<PrintBidReceipt>();
            List<string> submit = result.Split('\n').ToList();
            submit.RemoveAt(0);
            int count = 1;
            foreach (string item in submit)
            {
                if (item.Contains("*"))
                    break;
                lstPrintBidReceipt.Add(new PrintBidReceipt() { LineOrder = count++, LineNum = item });

            }
            resultHeadingString.Append(lineString + Environment.NewLine + Environment.NewLine);

            int startvalu = 0;
            int index = 0;
            int itemPercolumn = 65;

            StringBuilder singlePageContent = new StringBuilder();
            singlePageContent.Append("BidValet Formatted Bid Receipt" + Environment.NewLine);
            singlePageContent.Append("Employee Number : " + employeename + Environment.NewLine);
            //singlePageContent.Append ("Bid Receipt File :" + WBidHelper.GetAppDataPath () + "/" + fileName + Environment.NewLine);
            singlePageContent.Append("Receipt File Dated :" + DateTime.Now + "(Local)" + Environment.NewLine);
            singlePageContent.Append(footer + Environment.NewLine);
            string lineStr = string.Empty;
            int bidReceiptIndex;
            while (index + startvalu < lstPrintBidReceipt.Count)
            {
                for (int cnt = 0; cnt < 7; cnt++)
                {
                    bidReceiptIndex = startvalu + index + (cnt * itemPercolumn);

                    if (bidReceiptIndex < lstPrintBidReceipt.Count)
                    {
                        lineStr = lstPrintBidReceipt[bidReceiptIndex].LineOrder.ToString().PadLeft(3, ' ') + ". " + lstPrintBidReceipt[bidReceiptIndex].LineNum.PadRight(3, ' ');
                        singlePageContent.Append(lineStr.PadRight(15, ' '));
                    }
                    else
                    {
                        break;
                    }
                }
                singlePageContent.Append(Environment.NewLine);

                index++;
                if (index == itemPercolumn)
                {
                    index = 0;
                    startvalu = startvalu + index + (7 * itemPercolumn);
                    itemPercolumn = 62;
                    content.Add(singlePageContent);
                    singlePageContent = new StringBuilder();
                }
            }

            if (singlePageContent.ToString().Trim() != string.Empty)
            {
                content.Add(singlePageContent);

            }

            PDfParams pdfParams = new PDfParams
            {
                Author = "WBidValet",
                Creator = "WBidValet",
                FileName = WBidHelper.GetAppDataPath() + "/" + employeename + "Rct.pdf",
                Subject = "Bid Receipt",
                Title = "Bid Receipt"
            };

            
            CreatePDF(pdfParams, content);

        }

        public static void CreatePDF(PDfParams PDfParams, List<StringBuilder> ls)
        {
           
            string pdfPath = PDfParams.FileName;
            UIGraphics.BeginPDFContext(pdfPath, CGRect.Empty, new Foundation.NSDictionary());

            foreach (var pg in ls)
            {
                var text = pg.ToString();
                UIGraphics.BeginPDFPage(new CGRect(0, 0, 612, 792), new Foundation.NSDictionary());
                UIFont font = UIFont.FromName("Courier", 9);
                CGSize stringSize = text.StringSize(font, new CGSize(550, 700), UILineBreakMode.WordWrap);
                CGRect renderingRect = new CGRect(20, 20, 550, stringSize.Height);
                text.DrawString(renderingRect, font, UILineBreakMode.WordWrap);
            }

            UIGraphics.EndPDFContent();

        }


        public class PrintBidReceipt
        {
            public int LineOrder { get; set; }

            public string LineNum { get; set; }

        }

        public class PDfParams
        {

            public string FileName { get; set; }

            public StringBuilder FileContent { get; set; }

            public string Title { get; set; }

            public string Subject { get; set; }

            public string Creator { get; set; }

            public string Author { get; set; }




        }
    }
}
