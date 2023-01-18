using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PowerBear_Render_WPF_Ver.CameraObj {
    public class Camera {
        public Camera(int width, int height, double fov, Vector3d lookFrom, Vector3d lookAt, Vector3d vup) {
            this.width = width;
            this.height = height;
            this.aspect_radio = 1.0 * width / height;

            var theta = PbMath.PbMath.Degress_To_Radians(fov);
            var h = Math.Tan(theta / 2.0d);

            viewport_height = 2.0d * h;
            viewport_width = aspect_radio * viewport_height;

            var w = Vector3d.Unit_Vector(lookFrom - lookAt); //一个轴
            var u = Vector3d.Cross(vup, w); //根据vup和w做垂直
            var v = Vector3d.Cross(u, w);// 和w轴垂直的平面，u投影在上面，变成v

            origin = lookFrom;
            horizontal = u * viewport_width; // 方向向右
            vertical = v * viewport_height; // 方向向下
            upper_left_corner = origin - 0.5d * horizontal - 0.5d * vertical - w;
        }

        public Ray GetRay(double u, double v) {
            return new Ray(origin, upper_left_corner + u * horizontal + v * vertical - origin);
        }

        public double width, height;
        public double aspect_radio;
        public double forceLength = 1.0d;

        double viewport_height = 2.0d;
        double viewport_width;
        Vector3d origin = new(0d, 0d, 0d);
        Vector3d upper_left_corner; //画面左上角
        Vector3d horizontal;
        Vector3d vertical;
    }
}
