using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernApplicationFramework.Controls.Windows;

namespace ConsoleApp1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("test");
            Console.ReadLine();

            var window = new ModernChromeWindow();
            
            window.Show();


            window.PreviewKeyDown += Window_PreviewKeyDown;

            Console.ReadLine();
        }

        private static void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Trace.WriteLine(Keyboard.Modifiers);
        }
    }
}
