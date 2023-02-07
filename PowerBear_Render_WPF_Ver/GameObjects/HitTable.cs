using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    public struct HitResult {
        public Vector3d p; // 击中的坐标点
        public Vector3d normal; // 击中的法线，通过下面Set_Face_Nomrmal设置
        public double t; // 射线碰撞后at(t)参数
        public bool front_face; //T:射线从表面的外面射入，F:射线从表面内面摄入
        public Material mat; // 击中之后，根据材质的表现，计算光之后的行为
        /// <summary>
        /// 击中点的u和v坐标
        /// </summary>
        public double u, v;
        public HitTable? hitObj; // 击中的物体是什么，可以为null
        public void Set_Face_Normal(Ray r, Vector3d outward_normal) {
            front_face = Vector3d.Dot(r.direction, outward_normal) < 0;
            //if (front_face !=true) { Console.WriteLine("No"); }
            normal = front_face ? outward_normal : -1.0d * outward_normal;//取反，便于计算
            normal = normal.Normalized();
        }
    };
    public abstract class HitTable : ICloneable {
        public string objName { get; set; } = "未命名";
        public bool needRender { get; set; } = true;
        /// <summary>
        /// 光线求交点
        /// </summary>
        /// <param name="ray">一个Ray光线</param>
        /// <param name="t_min">Ray光线t值裁剪[t_min,t_max)</param>
        /// <param name="t_max">Ray光线最大t值</param>
        /// <param name="hitResult">out 回调击中信息</param>
        /// <returns></returns>
        public abstract bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult);
        public virtual bool Bounding_Box(out AABB? output_box) { output_box = null; return false; }
        public bool needDebug = false;
        /// <summary>
        /// 小熊渲染器，渲染管线，在渲染前
        /// </summary>
        public virtual void BeforeRendering() { }
        /// <summary>
        /// 小熊渲染器，渲染管线，在渲染后
        /// </summary>
        public virtual void AfterRendering() { }
        public virtual object Clone() { throw new NotImplementedException(); }

    }
}
