using SNMPLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CrawlerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    //prtMarkerSuppliesEntry
    //1.3.6.1.2.1.43.11.1.1
    [SNMPDataNode("1.3.6.1.2.1.43.11.1.1")]
    public class PrinterSupply
    {

    }
    [SNMPDeviceData]
    public class PrintingDeviceData
    {

        [SNMPValue("1.3.6.1.2.1.1.1")]
        public string PrinterName { get; set; }
        [SNMPValue("1.3.6.1.2.1.43.11.1.1.6")]
        public string SupplyDescription { get; set; }

        [SNMPValue("1.3.6.1.2.1.43.11.1.1.8")]
        public int SupplyMaxLevel { get; set; }
        [SNMPValue("1.3.6.1.2.1.43.11.1.1.9")]
        public int SupplyLevel { get; set; }

        [SNMPValue("1.3.6.1.2.1.43.11.1.1.5")]
        public int SupplyType { get; set; }

        [SNMPValue("1.3.6.1.2.1.43.10.2.1.4")]
        public int LifeCount { get; set; }

        [SNMPValue("1.3.6.1.2.1.43.10.2.1.5")]
        public int GetPoweredOnHours { get; set; }

        [SNMPValue("1.3.6.1.2.1.43.18.1.1.2")]
        public int AlertSecurityLevel { get; set; }
    }

    public partial class MainWindow : Window
    {
        PrintersStatusFactory<PrintingDeviceData> PrintingFactory = null;
        DispatcherTimer timer = null;
        public MainWindow()
        {
            PrintingFactory = new PrintersStatusFactory<PrintingDeviceData>();
         
     
            InitializeComponent();

            Task.Run(() => Initalizer());


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }
        public async Task Initalizer()
        {
            PrintingFactory.Initialize();
          
            await PrintingFactory.LoadDevicesFromJson();
            PrintingFactory.Initialized = true;
            await UpdateDeviceGrid();

        }
        public async Task UpdateDeviceGrid()
        {
            /*< StackPanel HorizontalAlignment = "Center" VerticalAlignment = "Center" >
                < Label FontSize = "20" HorizontalAlignment = "Center" > Dekstop </ Label >
                < Label > Printer name: Dekstop </ Label >
                < Label > Printer name: Dekstop </ Label >
                < Label > Printer name: Dekstop </ Label >
            </ StackPanel >
            */
            var result = await PrintingFactory.GetDevicesStatus(0);
            
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                devicesGrid.Children.Clear();
                foreach (var device in PrintingFactory.GetDeviceList())
                {
                    var stackPanel = new StackPanel();
                    {
                        stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                        stackPanel.VerticalAlignment = VerticalAlignment.Center;
                    }
                    {
                        for (int i = 0; i < PrintingFactory._VariableProps.Count; i++)
                        { 
                            System.Reflection.PropertyInfo property = PrintingFactory._VariableProps[i];

                            var displayNameLabel = new Label();
                            {
                                displayNameLabel.Content = property.Name + " : "  + property.GetValue(device.Data);
                                displayNameLabel.FontSize = 16;
                                displayNameLabel.HorizontalAlignment = HorizontalAlignment.Center;
                                displayNameLabel.VerticalAlignment = VerticalAlignment.Center;
                            }
                            stackPanel.Children.Add(displayNameLabel);
                        }

                        
                    }
                    devicesGrid.Children.Add(stackPanel);
                }
            }));
            
        }
        private async void Timer_Tick(object sender, EventArgs e)
        {
            // Disable the timer while the operation is in progress
            timer.IsEnabled = false;

   
            await Task.Run(async () => await UpdateDeviceGrid());

            // Re-enable the timer after the operation is complete
            timer.IsEnabled = true;
        }
    }
}