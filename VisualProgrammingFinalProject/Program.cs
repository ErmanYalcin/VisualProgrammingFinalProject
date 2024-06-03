using System;
using System.Windows.Forms;

namespace VisualProgrammingFinalProject
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            loginform loginForm = new loginform();
            Application.Run(loginForm);
        }
    }
}
