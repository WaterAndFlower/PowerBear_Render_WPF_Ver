using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;

namespace PowerBear_Render_WPF_Ver.Render {
    public interface IRayColor {
        /// <summary>
        /// 绘制图元的主要逻辑，获得光线求交后的色彩颜色值
        /// </summary>
        /// <param name="ray">光线</param>
        /// <param name="world">场景</param>
        /// <param name="depth">光线碰撞次数</param>
        /// <returns></returns>
        public abstract Vector3d Ray_Color(Ray ray, HitAble world, int depth);

        // 在渲染之前或者之后进行一些操作

        public virtual void BeforeRender() { }
        public virtual void AfterRender() { }
    }
}
