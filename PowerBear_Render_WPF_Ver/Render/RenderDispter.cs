using PowerBear_Render_WPF_Ver.GameObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using syDrawing = System.Drawing;
namespace PowerBear_Render_WPF_Ver.Render {
    /// <summary>
    /// 多线程类
    /// </summary>
    public class RenderDispter {
        //Camera设定
        CameraObj cameraObj;
        //图像设定
        public int width, height;
        public Byte[]? pixelColorBytes; //BGRA32
        public Action? OnFinish, FreshImage;
        public BackgroundWorker _BackWorker;


        //方法组
        public RenderDispter() { }
        public RenderDispter(int width, int height) {
            this.Init(width, height);
        }
        ~RenderDispter() {
            GobVar.wBitmap1 = null;
            pixelColorBytes = null;
        }
        public void doRender() {
            try {
                for (int i = 1; i <= this.height; i++) {
                    for (int j = 1; j <= this.width; j++) {
                        this.SetColorToBytes(i, j, syDrawing.Color.FromArgb((int)(1.0f * i / this.height * 255), (int)(1.0f * j / width * 255), 0));

                        if (GobVar.NeedFlush1) {
                            _BackWorker.ReportProgress((int)(1.0f * ((i - 1) * height + j) / (this.height * this.width) * 100), new Tuple<int, int, byte[]>(i, j, this.pixelColorBytes));
                            GobVar.NeedFlush1 = false;
                        }

                    }
                }
                _BackWorker.ReportProgress(100);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public static WriteableBitmap CreateWriteableBitMap(int width, int height) {
            return new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        }
        /// <summary>
        /// 必须是BGRA格式的数组
        /// </summary>
        /// <param name="pixelColorBytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static WriteableBitmap CreateWriteableBitMap(Byte[] pixelColorBytes, int width, int height) {
            var b = RenderDispter.CreateWriteableBitMap(width, height);
            b.WritePixels(new Int32Rect(0, 0, (int)b.Width, (int)b.Height), pixelColorBytes, b.BackBufferStride, 0);
            return b;
        }

        void Init(int width, int height) {
            this.pixelColorBytes = new Byte[width * height * 4];
            this.width = width;
            this.height = height;
        }

        void SetColorToBytes(int x, int y, System.Drawing.Color color) {
            x--; y--;
            int point = x * width * 4 + y * 4;
            this.pixelColorBytes[point] = color.B;
            this.pixelColorBytes[point + 1] = color.G;
            this.pixelColorBytes[point + 2] = color.R;
            this.pixelColorBytes[point + 3] = 255;
        }
    }
}
