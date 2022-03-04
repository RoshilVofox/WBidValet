using Bidvalet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidvalet.Business.BusinessLogic
{
    public class LineOperations
    {

        /// <summary>
        /// Move the selected line above the selected line number
        /// </summary>
        /// <param name="SelectedRows"></param>
        /// <param name="lineNumberToMove"></param>
        public static bool MoveSelectedLineAbove(List<int> SelectedRows, int lineNumberToMove)
        {
           // var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            List<Line> PromotedLines = new List<Line>();
            foreach (int linenum in SelectedRows)
            {
                if (linenum != lineNumberToMove)
                {

                    PromotedLines.Add(GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenum));
                }
            }
            if (PromotedLines != null && PromotedLines.Count > 0)
            {
                PromotedLines = PromotedLines.OrderBy(x => GlobalSettings.Lines.Select(y => y.LineNum).ToList().IndexOf(x.LineNum)).ToList();
                int currentIndex = GlobalSettings.Lines.IndexOf(PromotedLines[0]);
                currentIndex += PromotedLines.Count;
                if (currentIndex >= GlobalSettings.Lines.Count)
                    currentIndex = GlobalSettings.Lines.Count - 1;

                if (PromotedLines != null && PromotedLines.Count > 0)
                {

                    //  bool isMovelineSelected = false;

                    for (int i = 0; i < PromotedLines.Count; i++)
                    {
                        //if (PromotedLines[i].LineNum != lineNumberToMove)
                        //{
                        GlobalSettings.Lines.Remove(PromotedLines[i]);
                        //}
                    }

                    Line lineToMove = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == lineNumberToMove);


                    int lineindex = GlobalSettings.Lines.IndexOf(lineToMove);
                    bool toplock = false;
                    bool bottomlock = false;

                    if (lineToMove.TopLock)
                        toplock = true;
                    else if (lineToMove.BotLock)
                        bottomlock = true;

                    foreach (Line promoteline in PromotedLines)
                    {
                        promoteline.TopLock = toplock;
                        promoteline.BotLock = bottomlock;
                        GlobalSettings.Lines.Insert(lineindex, promoteline);
                        lineindex++;
                    }
                }
            }
            //if (wBidStateContent.ForceLine.IsBlankLinetoBottom == true && PromotedLines.Any(x => x.BlankLine))
            //{
            //    wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
            //    return true;

            //    //wBidStateContent.ForceLine.IsBlankLinetoBottom
            //    //IsBlankLineToBotttom = false;
            //    //if(CSWViewModelInstance!=null)
            //    //	CSWViewModelInstance.IsBlankLineToBotttom = false;
            //    //Xceed.Wpf.Toolkit.MessageBox.Show("Blank Lines are no longer at the bottom, you have moved a blank line(s) out of the bottom.!", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            //}
            return false;
        }



        public static bool MoveSelectedLineBelow(List<int> selectedRows, int lineNumberToMove)
        {
            try
            {
              //  var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                List<Line> TrashedLines = new List<Line>();
                foreach (int linenum in selectedRows)
                {
                    if (linenum != lineNumberToMove)
                    {
                        TrashedLines.Add(GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenum));
                    }
                }
                if (TrashedLines != null && TrashedLines.Count > 0)
                {
                    TrashedLines = TrashedLines.OrderBy(x => GlobalSettings.Lines.Select(y => y.LineNum).ToList().IndexOf(x.LineNum)).ToList();

                    int currentIndex = GlobalSettings.Lines.IndexOf(TrashedLines[0]);
                    if (currentIndex >= GlobalSettings.Lines.Count)
                        currentIndex = GlobalSettings.Lines.Count - 1;
                    if (TrashedLines != null && TrashedLines.Count > 0)
                    {
                        for (int i = 0; i < TrashedLines.Count; i++)
                        {
                            GlobalSettings.Lines.Remove(TrashedLines[i]);
                        }

                        Line lineToMove = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == lineNumberToMove);
                        int lineindex = GlobalSettings.Lines.IndexOf(lineToMove);
                        bool toplock = false;
                        bool bottomlock = false;

                        if (lineToMove.TopLock)
                            toplock = true;
                        else if (lineToMove.BotLock)
                            bottomlock = true;
                        foreach (Line trashedline in TrashedLines)
                        {
                            trashedline.TopLock = toplock;
                            trashedline.BotLock = bottomlock;
                            GlobalSettings.Lines.Insert(lineindex + 1, trashedline);
                            lineindex++;
                        }
                    }
                }
                //if (wBidStateContent.ForceLine.IsBlankLinetoBottom == true && TrashedLines.Any(x => x.BlankLine))
                //{
                //    wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
                //    return true;


                //}
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
