﻿using System;
namespace Bidvalet.Model
{
    public class MissedTripResponseModel
    {
        public MissedTripResponseModel()
        {
        }
        public System.Collections.Generic.List<Trip> JsonTripData { get; set; }
        public string FileName { get; set; }
        public string Message { get; set; }

    }
}
