using PowerBear_Render_WPF_Ver.CameraObj;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PowerBear_Render_WPF_Ver.DAO {
    /// <summary>
    /// 传递给多线程初始化的参数
    /// </summary>
    public class ToRenderDispterData {
        public int width, height;
        public int cpus; // 使用CPU核心数
        public int sample_pixel_level = 1;
        public int sample_depth = 10;
        public Camera mCamera;
        public int startRow = 1;//[1,500] sample 1
        public int endRow = -1;//sample 500
        [XmlIgnore]
        public BackgroundWorker _BackWorker;
    }
}
