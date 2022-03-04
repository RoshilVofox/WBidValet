﻿using System;
using System.Xml.Serialization;

namespace Bidvalet
{
	public class Constraints
	{
		public Constraints()
		{

		}
		public Constraints(Constraints constraints)
		{
			Hard = constraints.Hard;
			Blank = constraints.Blank;
			Reserve = constraints.Reserve;
			Ready = constraints.Ready;
			International = constraints.International;
			NonConus = constraints.NonConus;
			AircraftChanges =new Cx2Parameter(constraints.AircraftChanges);
			BlockOfDaysOff = new Cx2Parameter(constraints.BlockOfDaysOff);
			DeadHeads = new Cx4Parameters(constraints.DeadHeads);
			CL=new CxCommutableLine(constraints.CL);
			DaysOfWeek = new Cx3Parameters(constraints.DaysOfWeek);
			DeadHeadFoL = new Cx3Parameters(constraints.DeadHeadFoL);
			DutyPeriod = new Cx2Parameter(constraints.DutyPeriod);
			EQUIP = new Cx3Parameters(constraints.EQUIP);
			FlightMin = new Cx2Parameter(constraints.FlightMin);
			GroundTime = new Cx3Parameter(constraints.GroundTime);
			InterConus = new Cx2Parameters(constraints.InterConus);
			LegsPerDutyPeriod = new Cx2Parameter(constraints.LegsPerDutyPeriod);
			LegsPerPairing = new Cx2Parameter(constraints.LegsPerPairing);
			NumberOfDaysOff = new Cx2Parameter(constraints.NumberOfDaysOff);
			OverNightCities = new Cx3Parameters(constraints.OverNightCities);
			PDOFS = new Cx4Parameters(constraints.PDOFS);
			Position = new Cx3Parameters(constraints.Position);
			StartDayOftheWeek = new Cx3Parameters(constraints.StartDayOftheWeek);
			Rest = new Cx3Parameters(constraints.Rest);
			PerDiem = new Cx2Parameter(constraints.PerDiem);
			TripLength = new Cx3Parameters(constraints.TripLength);
			WorkBlockLength = new Cx3Parameters(constraints.WorkBlockLength);
			MinimumPay = new Cx2Parameter(constraints.MinimumPay);
			No3On3Off = new Cx2Parameter(constraints.No3On3Off);
			//            NoOverLap = new Cx2Parameter(constraints.NoOverLap);
			LrgBlkDayOff = new Cx2Parameter(constraints.LrgBlkDayOff);
			DaysOfMonth = new DaysOfMonthCx(constraints.DaysOfMonth);
			WorkDay = new Cx2Parameter(constraints.WorkDay);
			BulkOvernightCity = new BulkOvernightCityCx(constraints.BulkOvernightCity);



		}

		/// <summary>
		/// PURPOSE : Hard
		/// </summary>
		[XmlElement("Hard")]
		public bool Hard { get; set; }


		/// <summary>
		/// PURPOSE : Blank
		/// </summary>
		[XmlElement("Blank")]
		public bool Blank { get; set; }


		/// <summary>
		/// PURPOSE : Reserve
		/// </summary>
		[XmlElement("Reserve")]
		public bool Reserve { get; set; }

		/// <summary>
		/// PURPOSE : Ready
		/// </summary>
		[XmlElement("Ready")]
		public bool Ready { get; set; }

		/// <summary>
		/// PURPOSE : International
		/// </summary>
		[XmlElement("International")]
		public bool International { get; set; }

		/// <summary>
		/// PURPOSE : NonConus
		/// </summary>
		[XmlElement("NonConus")]
		public bool NonConus { get; set; }


		/// <summary>
		/// Purpose :Aircraft Changes
		/// </summary>
		[XmlElement("AircraftChanges")]
		public Cx2Parameter AircraftChanges { get; set; }

		///// <summary>
		///// PURPOSE : AM/PM 
		///// </summary>
		//[XmlElement("AM-PM")]
		//public AMPMConstriants AM_PM { get; set; }



		/// <summary>
		/// PURPOSE : BlockOfDaysOff
		/// </summary>
		[XmlElement("BODO")]
		public Cx2Parameter BlockOfDaysOff { get; set; }



		[XmlElement("DeadHeads")]
		public Cx4Parameters DeadHeads { get; set; }


		public CxCommutableLine CL { get; set; }

		/// <summary>
		/// PURPOSE : DaysOfWeek
		/// </summary>
		[XmlElement("DOW")]
		public Cx3Parameters DaysOfWeek { get; set; }

		[XmlElement("DeadHeadFoL")]
		public Cx3Parameters DeadHeadFoL { get; set; }


		/// <summary>
		/// PURPOSE : DutyPeriod
		/// </summary>
		[XmlElement("DP")]
		public Cx2Parameter DutyPeriod { get; set; }


		/// <summary>
		/// PURPOSE : Equipment Type
		/// </summary>
		[XmlElement("EQUIP")]
		public Cx3Parameters EQUIP { get; set; }

		/// <summary>
		/// PURPOSE : FlightMin
		/// </summary>
		[XmlElement("FLTMIN")]
		public Cx2Parameter FlightMin { get; set; }


		/// <summary>
		/// PURPOSE : Equipment Type
		/// </summary>
		[XmlElement("GroundTime")]
		public Cx3Parameter GroundTime { get; set; }


		[XmlElement("InterConus")]
		public Cx2Parameters InterConus { get; set; }

		/// <summary>
		///Legs Per DutyPeriod
		/// </summary>
		[XmlElement("LEGS")]
		public Cx2Parameter LegsPerDutyPeriod { get; set; }

		/// <summary>
		///Legs Per Pairing
		/// </summary>
		[XmlElement("LPP")]
		public Cx2Parameter LegsPerPairing { get; set; }


		[XmlElement("NODO")]
		public Cx2Parameter NumberOfDaysOff { get; set; }

		/// <summary>
		///Over Night Cities
		/// </summary>
		[XmlElement("RON")]
		public Cx3Parameters OverNightCities { get; set; }

		/// <summary>
		/// Property PartialDaysOff
		/// </summary>
		public Cx4Parameters PDOFS { get; set; }

		/// <summary>
		/// Position
		/// </summary>
		[XmlElement("Position")]
		public Cx3Parameters Position { get; set; }


		/// <summary>
		///Start Days of the week
		/// </summary>
		[XmlElement("SDOW")]
		public Cx3Parameters StartDayOftheWeek { get; set; }

		/// <summary>
		/// Property Rest
		/// </summary>
		public Cx3Parameters Rest { get; set; }

		/// <summary>
		///Time Away From Base
		/// </summary>
		[XmlElement("PerDiem")]
		public Cx2Parameter PerDiem { get; set; }

		/// <summary>
		///Trip length
		/// </summary>
		[XmlElement("TL")]
		public Cx3Parameters TripLength { get; set; }


		/// <summary>
		///Work Block length
		/// </summary>
		[XmlElement("WB")]
		public Cx3Parameters WorkBlockLength { get; set; }

		/// <summary>
		/// Minimum Pay
		/// </summary>
		[XmlElement("MinPay")]
		public Cx2Parameter MinimumPay { get; set; }


		/// <summary>
		/// No 3-On 3-off
		/// </summary>
		[XmlElement("No3on3of")]
		public Cx2Parameter No3On3Off { get; set; }

		/// <summary>
		/// NoOverLap
		/// </summary>
		[XmlElement("NoOverLap")]
		public Cx2Parameter NoOverLap { get; set; }
		//====================================================================


		[XmlElement("LrgBlkDayOff")]
		public Cx2Parameter LrgBlkDayOff { get; set; }

		/// <summary>
		/// PURPOSE : DaysOfMonth
		/// </summary>
		[XmlElement("SDO")]
		public DaysOfMonthCx  DaysOfMonth { get; set; }


		[XmlElement("WorkDay")]
		public Cx2Parameter WorkDay { get; set; }



		[XmlElement("BulkOC")]
		public BulkOvernightCityCx  BulkOvernightCity { get; set; }

























	
}
}

