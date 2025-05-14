using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace ChatbotApp
{
    static class Program
    {
        public static MainScreenForm mainScreenForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            mainScreenForm = new MainScreenForm();
            Application.Run(mainScreenForm);

            //var mainForm = new MainForm();
            //Application.Run(mainForm);
        }
    }
}