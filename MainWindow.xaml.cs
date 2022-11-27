using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
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

namespace myWPFPrj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CancellationTokenSource cancellationTokenSource;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            log.AppendText("작업 시작\r\n");
            var t1 = task1(cancellationTokenSource);
            var result = await t1.ConfigureAwait(false);
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { log.AppendText(result); }));
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource();
            log.Clear();
        }

        public async Task<string> task1(CancellationTokenSource cancellationTokenSource)
        {
            try 
            {
                for (int i = 0; i < 100; i++)
                {
                    await Task.Run(()=> SelfDelay(int.MaxValue, cancellationTokenSource.Token));
                    //await Task.Delay(100, cancellationTokenSource.Token);
                }

                return "Task1 is done\r\n";
            }
            catch (OperationCanceledException ex)
            {
                return "Task1 is cancelled\r\n";
            }
        }

        public Task<bool> SelfDelay(int millisecond, CancellationToken token)
        {
            int i = 0;
            while (true)
            {
                i++;
                if (i > millisecond)
                    break;
                if (token.IsCancellationRequested)
                    break;
            }

            if (token.IsCancellationRequested)
                return Task.FromResult(false);
            else
                return Task.FromResult(true);
        }
    }
}
