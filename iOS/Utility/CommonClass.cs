using Bidvalet.iOS;
using CoreGraphics;
using Foundation;
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
        public static bool SaveFormattedBidReceiptForPilot(string submitresult)
        {
            try
            {
                string lineString = string.Empty;
                string employeename = submitresult.Substring(0, submitresult.IndexOf("\n"));
                string fileName = employeename + "Rct.pdf";
                string footer = string.Empty;
                var lists = submitresult.Split('*').ToList();
                lists.RemoveAt(0);
                foreach (var item in lists)
                {
                    footer += item;
                }

                StringBuilder resultHeadingString = new StringBuilder();

                List<StringBuilder> content = new List<StringBuilder>();

                List<PrintBidReceipt> lstPrintBidReceipt = new List<PrintBidReceipt>();
                var submit = submitresult.Split('\n').ToList();
                submit.RemoveAt(0);
                int count = 1;
                var lines = GlobalSettings.Lines;
                foreach (string item in submit)
                {
                    if (item.Contains('*'))
                        break;
                    lstPrintBidReceipt.Add(new PrintBidReceipt() { LineOrder = count++, LineNum = item });

                }
                resultHeadingString.Append(lineString + Environment.NewLine + Environment.NewLine);

                List<PDFPage> lstPDfPages = new List<PDFPage>();

                PDFPage objPDFPage = new PDFPage();
                objPDFPage.Page = new List<SingleLine>();
                SingleLine objSingleLine = new SingleLine();
                objSingleLine.lstContent = new List<Content>();
                lstPDfPages.Add(objPDFPage);



                int startvalu = 0;
                int index = 0;
                int itemPercolumn = 65;


                StringBuilder singlePageContent = new StringBuilder();

                objSingleLine.lstContent.Add(new Content { Data = "WBidMax Formatted Bid Receipt " });
                objPDFPage.Page.Add(objSingleLine);

                objSingleLine = new SingleLine();
                objSingleLine.lstContent = new List<Content>();
                objSingleLine.lstContent.Add(new Content { Data = "Employee Number : " + employeename });
                objPDFPage.Page.Add(objSingleLine);

                //objSingleLine = new SingleLine();
                //objSingleLine.lstContent = new List<Content>();
                //objSingleLine.lstContent.Add(new Content { Data = "Bid Receipt File :" + WBidHelper.GetAppDataPath() + "\\" + fileName });
                //objPDFPage.Page.Add(objSingleLine);

                objSingleLine = new SingleLine();
                objSingleLine.lstContent = new List<Content>();
                objSingleLine.lstContent.Add(new Content { Data = "Receipt File Dated :" + DateTime.Now.ToLongDateString() + "(Local)" });
                objPDFPage.Page.Add(objSingleLine);

                objSingleLine = new SingleLine();
                objSingleLine.lstContent = new List<Content>();
                objSingleLine.lstContent.Add(new Content { Data = footer });
                objPDFPage.Page.Add(objSingleLine);

                Content objContent = new Content();

                objContent = new Content();

                string lineStr = string.Empty;
                int bidReceiptIndex;
                while (index + startvalu < lstPrintBidReceipt.Count)
                {
                    objSingleLine = new SingleLine();
                    objSingleLine.lstContent = new List<Content>();
                    for (int cnt = 0; cnt < 7; cnt++)
                    {
                        bidReceiptIndex = startvalu + index + (cnt * itemPercolumn);

                        if (bidReceiptIndex < lstPrintBidReceipt.Count)
                        {
                            lineStr = lstPrintBidReceipt[bidReceiptIndex].LineOrder.ToString().PadLeft(3, ' ') + ". " + lstPrintBidReceipt[bidReceiptIndex].LineNum.PadRight(3, ' ');


                            //singlePageContent.Append(lineStr.PadRight(15, ' '));
                            objContent = new Content();
                            objContent.Data = lineStr.PadRight(15, ' ');
                            int lineNUm = Convert.ToInt32(lstPrintBidReceipt[bidReceiptIndex].LineNum);
                            var liendata = lines.FirstOrDefault(x => x.LineNum == lineNUm);


                            if (liendata != null)
                            {
                                objContent.Blank = liendata.BlankLine;
                                objContent.Reserve = liendata.ReserveLine;

                            }

                            objSingleLine.lstContent.Add(objContent);
                        }
                        else
                        {
                            break;
                        }
                    }
                    objPDFPage.Page.Add(objSingleLine);


                    index++;
                    if (index == itemPercolumn)
                    {
                        index = 0;
                        startvalu = startvalu + index + (7 * itemPercolumn);
                        itemPercolumn = 62;



                        objPDFPage = new PDFPage();
                        objPDFPage.Page = new List<SingleLine>();
                        lstPDfPages.Add(objPDFPage);

                    }


                }

                if (singlePageContent.ToString().Trim() != string.Empty)
                {
                    lstPDfPages.Add(objPDFPage);
                    // content.Add(singlePageContent);

                }

                PDfParams pdfParams = new PDfParams
                {
                    Author = "WBidMax",
                    Creator = "WBidmax",
                    FileName = WBidHelper.GetAppDataPath() + "/" + employeename + "Rct.pdf",
                    Subject = "Bid Receipt",
                    Title = "Bid Receipt"
                };
                if (lstPDfPages.LastOrDefault() != null && lstPDfPages.LastOrDefault().Page.Count == 0)
                {
                    lstPDfPages.Remove(lstPDfPages.LastOrDefault());
                }
                CreatePDF(pdfParams, lstPDfPages);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
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
        public static void CreatePDF(PDfParams PDfParams, List<PDFPage> lstData)
        {
            string pdfPath = PDfParams.FileName;
            UIGraphics.BeginPDFContext(pdfPath, CGRect.Empty, new Foundation.NSDictionary());

            var ReserveAttribute = new UIStringAttributes();
            var BlankAttributes = new UIStringAttributes();
            var NormalAttribute = new UIStringAttributes();

            ReserveAttribute.BackgroundColor = UIColor.Red;
            ReserveAttribute.ForegroundColor = UIColor.White;
            ReserveAttribute.Font = UIFont.FromName("Courier", 9);

            BlankAttributes.BackgroundColor = UIColor.Blue;
            BlankAttributes.ForegroundColor = UIColor.White;
            BlankAttributes.Font = UIFont.FromName("Courier", 9);

            NormalAttribute.BackgroundColor = UIColor.White;
            NormalAttribute.Font = UIFont.FromName("Courier", 9);
            NormalAttribute.ForegroundColor = UIColor.Black;

            string test = string.Empty;
            foreach (var item in lstData)
            {
                NSMutableAttributedString content = new NSMutableAttributedString("", NormalAttribute);
                foreach (var singleLine in item.Page)
                {
                    foreach (var single in singleLine.lstContent)
                    {

                        if (single.Blank)
                        {
                            content.Append(new NSAttributedString(single.Data, BlankAttributes));
                            test += single.Data;
                        }
                        else if (single.Reserve)
                        {
                            content.Append(new NSAttributedString(single.Data, ReserveAttribute));
                            test += single.Data;
                        }
                        else
                        {
                            content.Append(new NSAttributedString(single.Data, NormalAttribute));
                            test += single.Data;
                        }

                    }
                    content.Append(new NSAttributedString("\n"));
                    test += "\n";
                }


                //var text = "";
                UIGraphics.BeginPDFPage(new CGRect(0, 0, 612, 792), new Foundation.NSDictionary());
                UIFont font = UIFont.FromName("Courier", 9);
                //CGSize stringSize = text.StringSize(font, new CGSize(550, 700), UILineBreakMode.WordWrap);

                CGRect renderingRect = new CGRect(20, 20, 550, 670);
                //test.DrawString(renderingRect, font, UILineBreakMode.WordWrap);
                content.DrawString(renderingRect);

            }
            UIGraphics.EndPDFContent();


            //foreach (var pg in ls)
            //{
            //	var text = pg.ToString();
            //	UIGraphics.BeginPDFPage(new CGRect(0, 0, 612, 792), new Foundation.NSDictionary());
            //	UIFont font = UIFont.FromName("Courier", 9);
            //	CGSize stringSize = text.StringSize(font, new CGSize(550, 700), UILineBreakMode.WordWrap);
            //	CGRect renderingRect = new CGRect(20, 20, 550, stringSize.Height);
            //	text.DrawString(renderingRect, font, UILineBreakMode.WordWrap);
            //}


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
        public class PDFPage
        {
            public List<SingleLine> Page { get; set; }
        }
        public class SingleLine
        {
            public List<Content> lstContent { get; set; }

        }

        public class Content
        {
            public string Data { get; set; }
            public bool Blank { get; set; }
            public bool Reserve { get; set; }
        }
    }
}
