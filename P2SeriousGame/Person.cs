﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2SeriousGame
{
    public class Person
    {
        public string firstname { get; set; }
        public string lastname { get; set; }

        public Person (string firstname, string lastname)
        {
            this.firstname = firstname;
            this.lastname = lastname;
        }
    }
}
