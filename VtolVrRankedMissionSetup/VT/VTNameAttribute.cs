﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VT
{
    public class VTNameAttribute : Attribute
    {
        public string Name { get; set; }

        public VTNameAttribute(string name)
        {
            Name = name;
        }
    }
}
