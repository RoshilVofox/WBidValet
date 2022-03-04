using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidvalet.Model
{
     public class DaysOfMonth
    {

        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
              
            }
        }


        private string _day;
        public string Day
        {
            get
            {
                return _day;
            }
            set
            {
                _day = value;
              
            }
        }


        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
              
            }
        }


        private bool _isEnabled;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
              
            }
        }




        //  Null-Normal Day, true- Work day false-NonWorking

        private bool? _status;
        public bool? Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
              
            }
        }




   
}
}
