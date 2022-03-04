using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Bidvalet.Model;

namespace Bidvalet
{
    public class UserInformation
    {

        /// <summary>
        /// First Name
        /// </summary>
        /// 
        [XmlAttribute("FirstName")]
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name
        /// </summary>
        [XmlAttribute("LastName")]
        public string LastName { get; set; }
        /// <summary>
        /// Employee Number
        /// </summary>
        [XmlAttribute("EmpNo")]
        public string EmpNo { get; set; }

        /// <summary>
        /// Employee Number
        /// </summary>
        [XmlAttribute("RemoteEmpNo")]
        public int RemoteEmpNo { get; set; }

        /// <summary>
        /// Domicile
        /// </summary>
        [XmlAttribute("Domicile")]
        public string Domicile { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        [XmlAttribute("Position")]
        public string Position { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        [XmlAttribute("RemotePosition")]
        public int RemotePosition { get; set; }
        /// <summary>
        /// Seniority Number
        /// </summary>
        [XmlAttribute("SeniorityNumber")]
        public int SeniorityNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [XmlAttribute("Email")]
        public string Email { get; set; }

        /// <summary>
        /// To check male or femaie.true if it is female.
        /// </summary>
        [XmlAttribute("isFemale")]
        public bool IsFemale { get; set; }

        /// <summary>
        /// To CellNumber
        /// </summary>
        [XmlAttribute("CellNumber")]
        public string CellNumber { get; set; }

        /// <summary>
        /// CellCarrier
        /// </summary>
        [XmlAttribute("CellCarrier")]
        public int CellCarrier { get; set; }

        /// <summary>
        /// isAcceptMail
        /// </summary>
        [XmlAttribute("isAcceptMail")]
        public bool isAcceptMail { get; set; }

        /// <summary>
        /// isAcceptUserTermsAndCondition
        /// </summary>
        [XmlAttribute("isAcceptUserTermsAndCondition")]
        public bool isAcceptUserTermsAndCondition { get; set; }

        /// <summary>
        /// PaidUntil Date
        /// </summary>
        [XmlAttribute("PaidUntilDate")]
        public DateTime? PaidUntilDate { get; set; }

        /// <summary>
        /// UserAccountDateTime Date
        /// </summary>
        [XmlAttribute("UserAccountDateTime")]
        public DateTime UserAccountDateTime { get; set; }


        [XmlAttribute("BidBase")]
        public string BidBase { get; set; }

        [XmlAttribute("BidSeat")]
        public string BidSeat { get; set; }


        [XmlAttribute("IsFree")]
        public bool IsFree { get; set; }


        [XmlAttribute("IsYearly")]
        public bool IsYearlySubscribed { get; set; }

        [XmlAttribute("IsMonthly")]
        public bool IsMonthlySubscribed { get; set; }

        [XmlAttribute("TopLine")]
        public string TopSubscriptionLine { get; set; }

        [XmlAttribute("SecondLine")]
        public string SecondSubscriptionLine { get; set; }

        [XmlAttribute("ThirdLine")]
        public string ThirdSubscriptionLine { get; set; }


        [XmlAttribute("LastSyncInfo")]
        public string LastSyncInfo { get; set; }

        [XmlAttribute("IsCBYearly")]
        public bool IsCBYearlySubscribed { get; set; }

        [XmlAttribute("IsCBMonthly")]
        public bool IsCBMonthlySubscribed { get; set; }

        /// <summary>
        /// Holds the recent file
        /// </summary>
        [XmlAttribute("RecentFiles")]
        public RecentFiles RecentFiles { get; set; }
        /// <summary>
        /// Holds the Bid data file names
        /// </summary>
        [XmlElement(ElementName= "AppDataBidFiles")] 
       // [XmlAttribute("AppDataBidFiles")]
        public List<AppDataBidFileNames> AppDataBidFiles { get; set; }

    }


    public class ServerUserInformation
    {


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int EmpNum { get; set; }
        public string Email { get; set; }
        public string CellPhone { get; set; }
        public DateTime? UserAccountDateTime { get; set; }
        public string CellCarrier { get; set; }
        public int Position { get; set; }
        public int CarrierNum { get; set; }
        public bool AcceptEmail { get; set; }
        public string Password { get; set; }

        public DateTime? CBExpirationDate { get; set; }

        public DateTime? WBExpirationDate { get; set; }

        //public string BidBase { get; set; }
        //public string BidSeat { get; set; }

        public int AppNum { get; set; }

        public bool IsFree { get; set; }


        public bool IsYearlySubscribed { get; set; }


        public bool IsMonthlySubscribed { get; set; }

        public bool IsCBYearlySubscribed { get; set; }


        public bool IsCBMonthlySubscribed { get; set; }

        public string TopSubscriptionLine { get; set; }


        public string SecondSubscriptionLine { get; set; }


        public string ThirdSubscriptionLine { get; set; }
    }
}
