﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2SeriousGame
{
    static class Program
    {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main()
        {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Handler FirstLevelHandler = new Handler();
			Map FirstLevel = new Map(FirstLevelHandler, 20, 1);
			Application.Run(FirstLevelHandler);
		}
	}
}
