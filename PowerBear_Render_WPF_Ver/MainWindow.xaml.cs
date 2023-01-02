﻿using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using sDrawing = System.Drawing;

namespace PowerBear_Render_WPF_Ver {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static MainWindow? Instance { get; private set; }

        public class RenderDetails {
            public string BitMapSize { get; set; } = "尚未开始";
            public string Mytest { get; set; } = "testvalue";
            public void Refush() {
                MainWindow.Instance.LabelRenderSize.Content = BitMapSize;
            }
        }

        public RenderDetails renderDetails = new RenderDetails();
        System.Timers.Timer timertimer = new System.Timers.Timer();
        public MainWindow() {
            InitializeComponent();
            this.Title = "小熊CPU离线渲染器Ver1.6";

            var wb = new WriteableBitmap(500, 500, 96, 96, PixelFormats.Bgra32, null);
            byte[] buffer = new byte[500 * 500 * 4];
            for (int i = 0; i < buffer.Length; i++) { buffer[i] = (byte)255; }
            wb.WritePixels(new Int32Rect(0, 0, (int)wb.Width, (int)wb.Height), buffer, wb.BackBufferStride, 0);
            MainImage.Source = wb;

            this.DataContext = renderDetails;
            MainWindow.Instance = this;

            timertimer.Interval = 16; //60HZ 16
            timertimer.Elapsed += Timer_Tick;

            //https://blog.csdn.net/q465162770/article/details/103406564
            //https://blog.csdn.net/qq_40313232/article/details/124987701
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            backgroundWorker.DoWork += new DoWorkEventHandler(RunThread);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkDown);
        }
        //强制UI进行刷新
        public void UIRefresh() {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(delegate (object f) {
                    ((DispatcherFrame)f).Continue = false;
                    return null;
                }
                    ), frame);
            Dispatcher.PushFrame(frame);
        }
        //通知线程更新图像缓存
        delegate void VoidDelegate();
        public void Timer_Tick(object? sender, ElapsedEventArgs e) {
            GobVar.NeedFlush1 = true;
            this.UIRefresh();
        }
        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
        }
        //--多线程部分--
        BackgroundWorker backgroundWorker;
        //渲染主线程！！！
        private void RunThread(object sender, DoWorkEventArgs e) {
            var start = DateTime.Now;
            Tuple<int, int> data = (Tuple<int, int>)e.Argument;
            RenderDispter renderDsp = new RenderDispter(data.Item1, data.Item2);
            renderDsp._BackWorker = backgroundWorker;
            renderDsp.doRender();
            e.Result = renderDsp;
            var stop = DateTime.Now;
            try {
                TimeUseLabel.Dispatcher.BeginInvoke(new Action(() => {
                    TimeUseLabel.Content = $"完成时间：{DateTime.Now}|总耗时（秒）：{(stop - start).TotalSeconds}|总耗时（分）:{(stop - start).TotalMinutes}";
                    RenderProcess.Value = 100;
                }));
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.Message);
            }
            //this.renderDetails.Refush();
            //timertimer.Stop();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (e.UserState == null) return;//已经渲染完毕了
            RenderProcess.Value = e.ProgressPercentage;
            var val = (Tuple<int, int, byte[]>)e.UserState;
            TimeUseLabel.Content = val.Item1.ToString() + " " + val.Item2.ToString() + " " + e.ProgressPercentage.ToString();

            if(GobVar.AllowPreview) {
                GobVar.wBitmap1 = GobVar.BitmapWrPixels(GobVar.wBitmap1, val.Item3);
                MainImage.Source = GobVar.wBitmap1;
            }
        }

        public void RunWorkDown(object? sender, RunWorkerCompletedEventArgs e) {
            var wb = (RenderDispter)e.Result;

            //更新主UI
            this.Dispatcher.Invoke((Action)delegate () {
                MainImage.Source = RenderDispter.CreateWriteableBitMap(wb.pixelColorBytes, wb.width, wb.height);
            });
            timertimer.Stop();
        }
        //开始渲染部分
        private void Button_Click(object sender, RoutedEventArgs e) {
            timertimer.Start();
            GC.Collect();
            //GC.WaitForFullGCComplete();
            //设定参数
            this.renderDetails.BitMapSize = RenderWidth.Text + "*" + RenderHeight.Text;
            this.renderDetails.Refush();
            GobVar.AllowPreview = cbAllowPreview.IsChecked == true ? true : false;
            timertimer.Interval = slHzPreview.Value;

            this.MainImage.Width = int.Parse(RenderWidth.Text);
            this.MainImage.Height = int.Parse(RenderHeight.Text);
            //使用一张假图像显示结果
            GobVar.renderWidth = int.Parse(RenderWidth.Text);
            GobVar.renderHeight = int.Parse(RenderHeight.Text);
            GobVar.wBitmap1 = RenderDispter.CreateWriteableBitMap(int.Parse(RenderWidth.Text), int.Parse(RenderHeight.Text));
            this.MainImage.Source = GobVar.wBitmap1;
            if (backgroundWorker.IsBusy) { backgroundWorker.CancelAsync(); } else {
                //启动渲染线程
                backgroundWorker.RunWorkerAsync(new Tuple<int, int>(int.Parse(RenderWidth.Text), int.Parse(RenderHeight.Text)));
            }
        }
        private void BackGroundColor_MouseDown(object sender, MouseButtonEventArgs e) {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(colorDialog.Color);
                SolidColorBrush solidColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, sb.Color.R, sb.Color.G, sb.Color.B));
                this.RenderSettingsBackgroundColor.Background = solidColorBrush;
            }
        }
    }
}
