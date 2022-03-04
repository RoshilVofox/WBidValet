using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Bidvalet.Model;

namespace Bidvalet.Business
{
	public class StateManagement
	{
		public void UpdateWBidStateContent()
		{

			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			if (GlobalSettings.Lines != null && GlobalSettings.Lines.Count > 0)
			{
				wBidStateContent.TagDetails = new TagDetails();
				wBidStateContent.TagDetails.AddRange(GlobalSettings.Lines.ToList().Where(x => x.Tag != null && x.Tag.Trim() != string.Empty).Select(y => new Tag { Line = y.LineNum, Content = y.Tag }));


				int toplockcount = GlobalSettings.Lines.Where(x => x.TopLock == true).ToList().Count;
				int bottomlockcount = GlobalSettings.Lines.Where(x => x.BotLock == true).ToList().Count;
				//save the top and bottom lock
				wBidStateContent.TopLockCount = toplockcount;
				wBidStateContent.BottomLockCount = bottomlockcount;

				//Get the line oreder
				List<int> lineorderlist = GlobalSettings.Lines.Select(x => x.LineNum).ToList();
				LineOrders lineOrders = new LineOrders();
				int count = 1;
				lineOrders.Orders = lineorderlist.Select(x => new LineOrder() { LId = x, OId = count++ }).ToList();
				lineOrders.Lines = lineorderlist.Count;
				wBidStateContent.LineOrders = lineOrders;

				//save the state of the Reserve line to botttom or blank line to bottom
				//wBidStateContent.ForceLine.IsBlankLinetoBottom = IsBlankLineToBotttom;
				// wBidStateContent.ForceLine.IsReverseLinetoBottom = IsReserveLineLineToBotttom;



				//string currentsortmethod = GetCurrentSortMetod();
				//if (currentsortmethod != "SelectedColumn")
				//{
				//    wBidStateContent.SortDetails.SortColumn = currentsortmethod;

				//}
				// Set the status of the Menu bar check box to the state file.
				SetMenuBarButtonStatusToStateFile(wBidStateContent);

				if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue && GlobalSettings.FAEOMStartDate != DateTime.MinValue.ToUniversalTime()))
					wBidStateContent.FAEOMStartDate = GlobalSettings.FAEOMStartDate;
				else
					wBidStateContent.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
			}

			//return wBidStateContent;

		}
		/// <summary>
		/// Set the status of the Menu bar check box to the state file.
		/// </summary>
		private void SetMenuBarButtonStatusToStateFile(WBidState wBidStateContent)
		{
			if (wBidStateContent != null && wBidStateContent.MenuBarButtonState != null)
			{
				wBidStateContent.MenuBarButtonState.IsVacationCorrection = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
				wBidStateContent.MenuBarButtonState.IsVacationDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
				wBidStateContent.MenuBarButtonState.IsOverlap = GlobalSettings.MenuBarButtonStatus.IsOverlap;
				wBidStateContent.MenuBarButtonState.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;
				wBidStateContent.MenuBarButtonState.IsMIL = GlobalSettings.MenuBarButtonStatus.IsMIL;
			}
		}
		public void ReloadDataFromStateFile()
		{
			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			////set constraints
			//ConstraintCalculations.CalculateAllConstraints(Lines);
			////Calculate weights
			//WeightCalculation.CalculateAllWieghts(Lines);

			// setForceLinefromStateFile();
			//Set  Lines order based on the line order saved in the state file
			SortLineOrderBasedOnStateFileLineOrder(wBidStateContent);

			//Set the Bottom and Top lock items in the line list from state file values
			SetTopandBottomLockBasedOnStateFile(wBidStateContent);

			//Set sort columns information state file
			// SetSortColumnInformationFromStateFile(wBidStateContent);

			ConstraintCalculations constaintcalculation = new ConstraintCalculations();
			constaintcalculation.ApplyAllConstraints();

			WeightCalculation weightCalculation = new WeightCalculation();
			weightCalculation.ApplyAllWeights();

			SortCalculation sort = new SortCalculation();
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
			{
				sort.SortLines(wBidStateContent.SortDetails.SortColumn);
			}




			if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (wBidStateContent.FAEOMStartDate != null && wBidStateContent.FAEOMStartDate != DateTime.MinValue && wBidStateContent.FAEOMStartDate != DateTime.MinValue.ToUniversalTime()))
				GlobalSettings.FAEOMStartDate = wBidStateContent.FAEOMStartDate;
			else
				GlobalSettings.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
			//SetMenuBarButtonStatusFromStateFile(wBidStateContent);
		}

		public void ReloadLineDetailsBasedOnPreviousState(bool isNeedToRecalculateLineProp)
		{
			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBidStateContent != null)
			{


				GlobalSettings.CompanyVA = GlobalSettings.WBidStateCollection.CompanyVA;
				SetMenuBarButtonStatusFromStateFile(wBidStateContent);
				//Setting  status to Global variables
				SetVacationOrOverlapExists(wBidStateContent);

				if (isNeedToRecalculateLineProp) {

					RecalculateLineProperties(wBidStateContent);
				}

				ReloadStateContent(wBidStateContent);

			}
		}


		public void RecalculateLineProperties(WBidState wBidStateContent)
		{
			WBidCollection.GenarateTempAbsenceList();

//			if (wBidStateContent.MenuBarButtonState.IsOverlap)
//			{
//				ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection();
//				reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection(GlobalSettings.Lines.ToList(), true);
//			}
//			else
//			{
				RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties();
				RecalcalculateLineProperties.CalcalculateLineProperties();

			//}
			if (!GlobalSettings.MenuBarButtonStatus.IsMIL) {
				PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
				prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
			}

		}


		public void ReloadStateContent(WBidState wBidStateContent)
		{

			//St the line order based on the state file.
			SortLineOrderBasedOnStateFileLineOrder(wBidStateContent);

			SetTopandBottomLockBasedOnStateFile(wBidStateContent);

			ConstraintCalculations constaintcalculation = new ConstraintCalculations();
			constaintcalculation.ApplyAllConstraints();

			WeightCalculation weightCalculation = new WeightCalculation();
			weightCalculation.ApplyAllWeights();



			SortCalculation sort = new SortCalculation();
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
			{
				sort.SortLines(wBidStateContent.SortDetails.SortColumn);
			}

		}
		public void ApplyCSW(WBidState wBidStateContent)
		{



			ConstraintCalculations constaintcalculation = new ConstraintCalculations();
			constaintcalculation.ApplyAllConstraints();

			WeightCalculation weightCalculation = new WeightCalculation();
			weightCalculation.ApplyAllWeights();



			SortCalculation sort = new SortCalculation();
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
			{
				sort.SortLines(wBidStateContent.SortDetails.SortColumn);
			}

		}

		/// <summary>
		/// Check whether we need to recalculate the line properties
		/// </summary>
		/// <returns></returns>
		public bool CheckLinePropertiesNeedToRecalculate(WBidState wBidStateContent)
		{
			bool status = false;
			//  if (wBidStateContent != null && wBidStateContent.MenuBarButtonState != null)
			// {
			//TRUE means they are having same Menu Button state
			status = (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection == wBidStateContent.MenuBarButtonState.IsVacationCorrection &&
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop == wBidStateContent.MenuBarButtonState.IsVacationDrop &&
				GlobalSettings.MenuBarButtonStatus.IsOverlap == wBidStateContent.MenuBarButtonState.IsOverlap &&
				GlobalSettings.MenuBarButtonStatus.IsEOM == wBidStateContent.MenuBarButtonState.IsEOM &&
				GlobalSettings.MenuBarButtonStatus.IsMIL == wBidStateContent.MenuBarButtonState.IsMIL);
			status = !status;
			// }
			return status;
		}

		public void SetMenuBarButtonStatusFromStateFile(WBidState wBidStateContent)
		{
			if (wBidStateContent.MenuBarButtonState != null)
			{
				if (GlobalSettings.MenuBarButtonStatus == null)
					GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus();
                GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = wBidStateContent.MenuBarButtonState.IsVacationCorrection || GlobalSettings.IsFVVacation;
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop = wBidStateContent.MenuBarButtonState.IsVacationDrop;
				GlobalSettings.MenuBarButtonStatus.IsOverlap = wBidStateContent.MenuBarButtonState.IsOverlap;
				GlobalSettings.MenuBarButtonStatus.IsEOM = wBidStateContent.MenuBarButtonState.IsEOM;
				GlobalSettings.MenuBarButtonStatus.IsMIL = wBidStateContent.MenuBarButtonState.IsMIL;
			}
		}

		public void SetVacationOrOverlapExists(WBidState wBidStateContent)
		{
			GlobalSettings.IsOverlapCorrection = wBidStateContent.IsOverlapCorrection;
			GlobalSettings.IsVacationCorrection = wBidStateContent.IsVacationOverlapOverlapCorrection;
			if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (wBidStateContent.FAEOMStartDate != null && wBidStateContent.FAEOMStartDate != DateTime.MinValue && wBidStateContent.FAEOMStartDate != DateTime.MinValue.ToUniversalTime()))
				GlobalSettings.FAEOMStartDate = wBidStateContent.FAEOMStartDate;
			else
				GlobalSettings.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
		}

		/// <summary>
		/// Set  Lines order based on the line order saved in the state file
		/// </summary>
		private void SortLineOrderBasedOnStateFileLineOrder(WBidState wBidStateContent)
		{
			LineOrders lineOrders = wBidStateContent.LineOrders;
			var orderedlines = GlobalSettings.Lines.ToList().OrderBy(x => lineOrders.Orders.Select(y => y.LId).ToList().IndexOf(x.LineNum));
			GlobalSettings.Lines = new ObservableCollection<Line>(orderedlines);
		}
		/// <summary>
		/// Set the Bottom and Top lock items in the line list from state file values
		/// </summary>
		private void SetTopandBottomLockBasedOnStateFile(WBidState wBidStateContent)
		{
			int toplockcount = wBidStateContent.TopLockCount;
			int bottomlockcount = wBidStateContent.BottomLockCount;

			foreach (Line line in GlobalSettings.Lines)
			{
				line.TopLock = false;
			}
			foreach (Line line in GlobalSettings.Lines)
			{
				line.BotLock = false;
			}

			var toplockedlines = GlobalSettings.Lines.ToList().Take(toplockcount);
			foreach (Line line in toplockedlines)
			{
				line.TopLock = true;
			}
			var bottomlockedlines = GlobalSettings.Lines.ToList().Skip(GlobalSettings.Lines.Count - bottomlockcount).ToList();
			foreach (Line line in bottomlockedlines)
			{
				line.BotLock = true;
			}
		}

		private void SetSortColumnInformationFromStateFile(WBidState wBidStateContent)
		{

		}
	}
}


