using PowerBear_Render_WPF_Ver.CameraObj;
using PowerBear_Render_WPF_Ver.DAO;
using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.Pages;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using PowerBear_Render_WPF_Ver.Textures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Color = System.Drawing.Color;
using sDrawing = System.Drawing;
using sysColor = System.Windows.Media.Color;

namespace PowerBear_Render_WPF_Ver {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static MainWindow? Instance { get; private set; }
        /// <summary>
        /// UI界面的数据
        /// </summary>
        public class RenderDetails {
            public string BitMapSize { get; set; } = "尚未开始";
            public string Mytest { get; set; } = "testvalue";
            public string UICpus = "1 Core";
            public bool uAllowRenderPreview { get; set; } = true; //允许，是否启动像素级预览功能，移动物体完毕，将会立即渲染画面（启动像素着色器预览功能）
            public sysColor _backColor1 { get; set; } = sysColor.FromRgb(182, 210, 234);
            public sysColor _backColor2 { get; set; } = sysColor.FromRgb(255, 255, 255);
            public int _backColorType { get; set; } = 0;//0 单 1 双 2 HDRI
            public string _backImgPath { get; set; } = "C:\\Users\\PowerBear\\Desktop\\Doc\\大创渲染器\\中间过程演示\\HDRI\\sky_hdri.png";
            public bool _AllowDoDeNoise { get; set; } = false;
            public bool _AllowPhongModel { get; set; } = false;
            public void Refush() {
                MainWindow.Instance.LabelRenderSize.Content = BitMapSize;
                MainWindow.Instance.CpusLabel.Content = UICpus;
            }
        }

        public RenderDetails renderDetails = new RenderDetails();
        System.Timers.Timer timertimer = new System.Timers.Timer();
        public MainWindow() {
            InitializeComponent();

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
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AfterRender);

            uObjectsListBox.ItemsSource = GobVar.fnObjects;
            InitRenderSettings();
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
        //！！！渲染主线程！！！
        public void RunThread(object sender, DoWorkEventArgs e) {

            var data = (ToRenderDispterData)e.Argument;

            BaseRenderDispter renderDsp = new RenderDispter(data);

            renderDsp.DoRender();
            e.Result = renderDsp;


            //this.renderDetails.Refush();
            //timertimer.Stop();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (e.ProgressPercentage == 100) {
                try {
                    var timeSpan = (TimeSpan)e.UserState;
                    TimeUseLabel.Dispatcher.BeginInvoke(new Action(() => {
                        TimeUseLabel.Content = $"完成时间：{DateTime.Now}|总耗时（秒）：{timeSpan.TotalSeconds}|总耗时（分）:{timeSpan.TotalMinutes}";
                        RenderProcess.Value = 100;
                    }));
                }
                catch (Exception ex) {
                    System.Windows.MessageBox.Show(ex.Message);
                }
                return;
            }//已经渲染完毕了

            RenderProcess.Value = e.ProgressPercentage;
            var val = (Tuple<int, int, byte[]>)e.UserState;
            TimeUseLabel.Content = val.Item1.ToString() + " " + val.Item2.ToString() + " " + e.ProgressPercentage.ToString();

            if (GobVar.AllowPreview) {
                GobVar.wBitmap1 = GobVar.BitmapWritePixels(GobVar.wBitmap1, val.Item3);
                MainImage.Source = GobVar.wBitmap1;
            }
        }
        // 线程运行完毕回调函数
        public void AfterRender(object? sender, RunWorkerCompletedEventArgs e) {
            var wb = (RenderDispter)e.Result;

            //更新主UI
            this.Dispatcher.Invoke((Action)delegate () {
                MainImage.Source = PbIO.CreateWriteableBitMap_BGRA(wb.pixelColorBytes, wb.width, wb.height);
            });
            timertimer.Stop();

            // 渲染完毕，将数据保存到Tmp文件夹中【渲染完毕处理】
            GobVar.SaveBitmp(GobVar.appStartupPath, $"/Tmp/Read.png", MainImage.Source);
            try {
                var mwidth = GobVar.wBitmap1.PixelWidth;
                var mheight = GobVar.wBitmap1.PixelHeight;
                byte[] inpt = PbIO.BGRA_TO_BGR(wb.pixelColorBytes, mheight, mwidth);
                byte[] resRGB = new byte[mwidth * mheight * 3];

                // 渲染完毕后做的事情
                if (renderDetails._AllowDoDeNoise) {
                    CppFunction.DoDeNoise(inpt, mwidth, mheight, resRGB);
                    this.Dispatcher.Invoke((Action)delegate () {
                        MainImage.Source = PbIO.CreateWriteableBitMap_RGB(resRGB, wb.width, wb.height);
                    });
                }
                if (renderDetails._AllowPhongModel) {
                    CppFunction.DoCanny(inpt, mwidth, mheight);
                }

            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
            //Task.Delay(1000);
            // MainImage.Source = new BitmapImage(new Uri(GobVar.appStartupPath + "/Tmp/Write.png"));
        }
        void InitRenderSettings() {
            // 设置相机
            var lookFrom = new Vector3d(double.Parse(CameraPosXTextBox.Text), double.Parse(CameraPosYTextBox.Text), double.Parse(CameraPosZTextBox.Text));
            var lookAt = new Vector3d(uCameraViewXSlider.Value, uCameraViewYSlider.Value, uCameraViewZSlider.Value);
            lookAt = lookFrom + lookAt.Normalized();
            GobVar.renderWidth = int.Parse(RenderWidth.Text);
            GobVar.renderHeight = int.Parse(RenderHeight.Text);
            GobVar.mCamera = new(GobVar.renderWidth, GobVar.renderHeight, uFovSlider.Value, lookFrom, lookAt, new Vector3d(0, 1, 0));

            // 设置天空盒
            var _bcakColor1 = renderDetails._backColor1;
            var _bcakColor2 = renderDetails._backColor2;
            Sphere _BackObj = new Sphere(new(0, 0, 0), 10000);
            switch (renderDetails._backColorType) {
                case 0: {
                        _BackObj.mat = new SkyMat(new Solid_Color(_bcakColor1.R / 255d, _bcakColor1.G / 255d, _bcakColor1.B / 255d));
                        break;
                    }
                case 1: {
                        _BackObj.mat = new SkyMat(new LinerColor(_bcakColor1, _bcakColor2));
                        break;
                    }
                case 2: {
                        _BackObj.mat = new SkyMat(new ImageTexture(renderDetails._backImgPath));
                        break;
                    }
            }
            GobVar.skyObject = _BackObj;
            GobVar._BackColor = new(_bcakColor1.R / 255d, _bcakColor1.G / 255d, _bcakColor1.B / 255d);
        }
        void BeforeRender() { // 在DoRender之前，进行一次预处理
            timertimer.Start();
            GC.Collect();
            //GC.WaitForFullGCComplete();
            //设定参数 在这里进行 一些预先的设置
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
            GobVar.InitRender(); // 小熊渲染管线
        }
        public void DoRender() {
            try {
                BeforeRender();

                // 开始渲染
                if (backgroundWorker.IsBusy) { backgroundWorker.CancelAsync(); } else {
                    //启动渲染线程
                    InitRenderSettings();
                    // 设定CPU显示UI
                    var t = 1;
                    switch (CPUs_Combox.SelectedIndex) {
                        case 0:
                        t = 1;
                        renderDetails.UICpus = "1 Core";
                        break;
                        case 1:
                        t = 2;
                        break;
                        case 2:
                        t = 4;
                        break;
                    }
                    renderDetails.UICpus = $"{t * 2}C{t * 2 * 2}T";

                    renderDetails.Refush();

                    GobVar.RenderDispData = new ToRenderDispterData() { width = GobVar.renderWidth, height = GobVar.renderHeight, mCamera = GobVar.mCamera, cpus = t, sample_depth = 0, sample_pixel_level = MSAA_Combox.SelectedIndex, _BackWorker = backgroundWorker };
                    backgroundWorker.RunWorkerAsync(GobVar.RenderDispData);
                }
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show("不能进行渲染，可能填写数字的地方有误：\n" + ex.Message);
            }
        }
        //开始渲染部分（UI信号）
        private void Button_Click(object sender, RoutedEventArgs e) {
            GobVar.stopAtRenderColor = false;
            DoRender();
        }

        void SaveWtableBmp(Object sender, RoutedEventArgs e) {
            String appStartupPath = System.IO.Directory.GetCurrentDirectory() + @"\Out";
            var saveName = appStartupPath + $"\\saveData{DateTime.Now.ToString("yy-MM-dd hh\\mm\\ss")}.png";

            GobVar.SaveBitmp(appStartupPath, $"\\saveData{DateTime.Now.ToString("yy-MM-dd hh\\mm\\ss")}.png", MainImage.Source);

            var btnRes = System.Windows.MessageBox.Show(saveName + "\n\n点击YES打开目录", "保存成功", MessageBoxButton.YesNo);
            if (btnRes.Equals(MessageBoxResult.Yes)) {
                Process ExplorerWindowProcess = new Process();
                ExplorerWindowProcess.StartInfo.FileName = "explorer.exe";
                ExplorerWindowProcess.StartInfo.Arguments = appStartupPath; //saveName 或 appStartupPath
                ExplorerWindowProcess.Start();
            }
        }

        private void uObjectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void Button_Click_RemoveObject(object sender, RoutedEventArgs e) {
            try {
                GobVar.fnObjects.Remove(GobVar.fnObjects[uObjectsListBox.SelectedIndex]);
            }
            catch (Exception ex) { System.Windows.MessageBox.Show("删除不了，原因：\n" + ex.Message); }
        }

        private void Button_Click_AddObject(object sender, RoutedEventArgs e) {
            AddObjectWindow adWin = new AddObjectWindow();
            adWin.Owner = this;
            adWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            adWin.ShowDialog();

        }

        private void Button_Click_About(object sender, RoutedEventArgs e) {
            AboutWindow abWin = new();
            abWin.Owner = this;
            abWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            abWin.ShowDialog();
        }

        private void CheckBox_Click_uRenderLIst(object sender, RoutedEventArgs e) {
            GobVar.Render_Preview();
        }

        private void Button_Click_SaveTransform(object sender, RoutedEventArgs e) {
            GobVar.Render_Preview();
        }

        private void Button_Click_RenderPreview(object sender, RoutedEventArgs e) {
            GobVar.AllowPreview = true;
            GobVar.Render_Preview();
        }

        private void Button_Click_PushNetWork(object sender, RoutedEventArgs e) {
            InitRenderSettings();
            PbIO.JsonEncode();
        }
    }
}
