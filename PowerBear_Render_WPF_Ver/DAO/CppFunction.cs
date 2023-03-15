using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.DAO {
    public class CppFunction {
        /// <summary>
        /// 进行降噪算法的实现，使用C++的OpenCV库，提供了支持，这部分属于图像处理
        /// </summary>
        [DllImport("PowerBear_Render_CPP_DLL")]
        unsafe public extern static void doDeNoise(byte[] inptImg, int width, int height, Byte* outPtr);

        [DllImport("PowerBear_Render_CPP_DLL")]
        public extern static void doCanny(byte[] inptImg, int width, int height);

        public static void DoDeNoise(byte[] inptImg, int width, int height, Byte[] outPtr) {
            unsafe {
                fixed (Byte* ptr = outPtr)
                    doDeNoise(inptImg, width, height, ptr);
            }
        }
        public static void DoCanny(byte[] inptImg,int width,int height) {
            unsafe {
                doCanny(inptImg, width, height);
            }
        }
    }
}
