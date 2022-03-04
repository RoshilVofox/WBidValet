﻿using System;
using System.Collections.Generic;

namespace Bidvalet
{
	public sealed class SharedObject
	{
		private static readonly SharedObject instance = new SharedObject(){
			ListConstraint =  new List<object>()
		};

		private SharedObject(){}

		public static SharedObject Instance
		{
			get 
			{
				return instance; 
			}
		}

		public List<Object> ListConstraint;
	}
}

