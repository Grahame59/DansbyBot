using System;
using System.Windows.Forms;

namespace ChatbotApp
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

            var mainScreenForm = new MainScreenForm();
            Application.Run(mainScreenForm);

            //var mainForm = new MainForm();
            //Application.Run(mainForm);
        }
    }
}