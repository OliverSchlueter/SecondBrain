using System;
using System.IO;

namespace SecondBrain
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public const string Version = "v0.0.3 [DEV]";
        public const string DataFolderPath = "data/";
        
        [STAThread]
        public static void Main()
        {
            if (!Directory.Exists(DataFolderPath))
            {
                Directory.CreateDirectory(DataFolderPath);
            }
            
            var application = new App();
            application.InitializeComponent();
            application.Run();
        }
    }
}