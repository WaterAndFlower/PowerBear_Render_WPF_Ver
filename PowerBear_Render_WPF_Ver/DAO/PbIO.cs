using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using PowerBear_Render_WPF_Ver.Render;
using System.Windows;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PowerBear_Render_WPF_Ver.GameObjects;
using System.IO;
using System.ComponentModel;
using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.Textures;
using PowerBear_Render_WPF_Ver.CameraObj;
using System.Xml;

namespace PowerBear_Render_WPF_Ver.DAO {
    public class PbIO {
        public static Byte[] BitmapToBytes(BitmapSource bmp) {
            Int32 PixelHeight = bmp.PixelHeight; // 图像高度
            Int32 PixelWidth = bmp.PixelWidth;   // 图像宽度
            Int32 Stride = PixelWidth << 2;         // 扫描行跨距
            Byte[] Pixels = new Byte[PixelHeight * Stride];
            if (bmp.Format == PixelFormats.Bgr32 || bmp.Format == PixelFormats.Bgra32) {   // 拷贝像素数据
                bmp.CopyPixels(Pixels, Stride, 0);
            } else {   // 先进行像素格式转换，再拷贝像素数据
                new FormatConvertedBitmap(bmp, PixelFormats.Bgr32, null, 0).CopyPixels(Pixels, Stride, 0);
            }
            return Pixels;
        }
        public static Byte[] Bitmap_TO_BGR(BitmapSource bmp) {
            Int32 mheight = bmp.PixelHeight;
            Int32 mwidth = bmp.PixelWidth;
            byte[] inpt = BitmapToBytes(bmp);
            byte[] outp = new byte[mwidth * mheight * 3];
            // B G R
            for (int i = 0, ii = 0; i < mwidth * mheight * 3; i += 3, ii += 4) { outp[i] = inpt[ii]; outp[i + 1] = inpt[ii + 1]; outp[i + 2] = inpt[ii + 2]; }
            return outp;
        }
        public static Byte[] BGRA_TO_BGR(byte[] inpt, int mheight, int mwidth) {
            byte[] outp = new byte[mwidth * mheight * 3];
            // B G R
            for (int i = 0, ii = 0; i < mwidth * mheight * 3; i += 3, ii += 4) { outp[i] = inpt[ii]; outp[i + 1] = inpt[ii + 1]; outp[i + 2] = inpt[ii + 2]; }
            return outp;
        }
        /// <summary>
        /// 必须是BGRA格式的数组
        /// </summary>
        public static WriteableBitmap CreateWriteableBitMap_BGRA(Byte[] pixelColorBytes, int width, int height) {
            var b = RenderDispter.CreateWriteableBitMap(width, height);
            b.WritePixels(new Int32Rect(0, 0, (int)b.Width, (int)b.Height), pixelColorBytes, b.BackBufferStride, 0);
            return b;
        }
        /// <summary>
        /// 传入RGB数组
        /// </summary>
        public static WriteableBitmap CreateWriteableBitMap_RGB(Byte[] colorRGB, int width, int height) {
            byte[] toBytesBGRA = new byte[width * height * 4];
            for (int i1 = 0, i2 = 0; i1 < toBytesBGRA.Length && i2 < colorRGB.Length; i1 += 4, i2 += 3) {
                toBytesBGRA[i1] = colorRGB[i2 + 2];
                toBytesBGRA[i1 + 1] = colorRGB[i2 + 1];
                toBytesBGRA[i1 + 2] = colorRGB[i2];
                toBytesBGRA[i1 + 3] = 255;
            }
            return CreateWriteableBitMap_BGRA(toBytesBGRA, width, height);
        }

        // 序列化场景的物体
        public static NetworkJsonData JsonEncode() {
            string xml_fnObjects = "", xml_Camera = "", xml_SkyBox = "";

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BindingList<NormalObject>), new Type[] { typeof(HitTable), typeof(Material) });

            // 序列化 fnObjects 为 XML
            using MemoryStream mms = new();
            xmlSerializer.Serialize(mms, GobVar.fnObjects);
            mms.Position = 0;
            using StreamReader ssr = new(mms);
            Console.WriteLine(xml_fnObjects = ssr.ReadToEnd());

            // 序列化 Camera 参数为 XML
            xmlSerializer = new(typeof(Camera));
            using MemoryStream mss2 = new();
            xmlSerializer.Serialize(mss2, GobVar.mCamera);
            mss2.Position = 0;
            using StreamReader ssr2 = new(mss2);
            Console.WriteLine(xml_Camera = ssr2.ReadToEnd());

            // 序列化 天空盒 为 XML
            xmlSerializer = new(typeof(Sphere), new Type[] { typeof(Material), typeof(Texture) });
            using MemoryStream mss3 = new();
            xmlSerializer.Serialize(mss3, GobVar.skyObject);
            mss3.Position = 0;
            using StreamReader ssr3 = new(mss3);
            Console.WriteLine(xml_SkyBox = ssr3.ReadToEnd());

            //序列化 线程参数 为 XML



            NetworkJsonData jsonData = new NetworkJsonData() { xml_fnObjects = xml_fnObjects, xml_sky_box = xml_SkyBox, xml_camera = xml_Camera };

            return jsonData;
        }

        public static object? XmlDeserialize(string xml, Type type, Type[]? otherType = null) {
            try {
                using (StringReader sr = new StringReader(xml)) {
                    XmlSerializer serializer = new XmlSerializer(type, otherType);
                    return serializer.Deserialize(sr);
                }
            }
            catch (Exception e) {
                return null;
            }
        }

        public static void SaveBitmp(string path, string fileName, ImageSource se) {
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            var saveActualPath = path + "\\" + fileName;
            using (FileStream stream = new FileStream(saveActualPath, FileMode.Create)) {
                PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
                pngBitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)se));
                pngBitmapEncoder.Save(stream);
            }
        }
    }
}
