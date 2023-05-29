using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    /// <summary>
    /// 一个通用的Object类型，可以执行Transform的变换操作
    /// </summary>
    [Serializable]
    public class NormalObject : HitTable, ISerializable {
        public HitTable srcObj { get; set; }
        public Vector3d offset { get; set; } = new Vector3d(0, 0, 0);
        public double angleY { get; set; } = 0;
        public NormalObject() { }
        public NormalObject(HitTable srcObj) {
            this.srcObj = srcObj;
        }

        HitTable actualObj;

        public override void BeforeRendering() {
            var __GUID = srcObj._GUID;
            actualObj = srcObj.Clone() as HitTable;
            actualObj = new Rotate_Y(actualObj, angleY);
            actualObj = new Translate(actualObj, offset);
            actualObj._GUID = __GUID;
        }

        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            return actualObj.Hit(ray, t_min, t_max, out hitResult);
        }
        
        public override bool Bounding_Box(out AABB? output_box) {
            return actualObj.Bounding_Box(out output_box);
        }

        public override object Clone() {
            return new NormalObject((HitTable)srcObj.Clone());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
        }
    }
}
