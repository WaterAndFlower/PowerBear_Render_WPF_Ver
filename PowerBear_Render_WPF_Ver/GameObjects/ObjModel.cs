using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    public struct ObjData { // OBJ格式的索引编号，FACE编号
        public int vertIdex, TexIdex, NorIdex;
        public ObjData(int v, int tex, int nor) { vertIdex = v; TexIdex = tex; NorIdex = nor; }
    }
    /// <summary>
    /// Obj格式模型解析，使用右手坐标系，导出时候，选择向上方向是y，向前是-z。
    /// </summary>
    public class ObjModel : HitTable {
        [XmlIgnore]
        public BVH_Tree mBVH;

        public List<HitTable> mTriangle = new List<HitTable>();
        public List<Vector3d> vertexsPos = new();
        public List<Vector3d> vertexsNormal = new();
        public List<Tuple<double, double>> vertexsUV = new();
        public List<ObjData> faceData = new();
        public new string objName = "";
        // Obj模型的材质，可以带贴图玩玩
        public Material mat = new Lambertian(0.5d, 0.5d, 0.5d);//此物体的材质

        // public Hittable_List tList = new Hittable_List(); // 由于Tree算法有点问题，这个是调试，最后记得注释掉
        void ObjBuild(string objPath) {
            //使得下标都是从1开始
            vertexsPos.Add(Vector3d.Zero);
            vertexsNormal.Add(Vector3d.Zero);
            vertexsUV.Add(new(0, 0));
            faceData.Add(new());
            //从文件IO流进行读入
            try {
                using StreamReader sr = new StreamReader(objPath);
                var line = sr.ReadLine();
                while (line != null) {
                    if (line.StartsWith("v ")) { // 读入顶点
                        var res = line.Split(' ');
                        vertexsPos.Add(new Vector3d(double.Parse(res[1]), double.Parse(res[2]), double.Parse(res[3])));
                    } else if (line.StartsWith("vn")) { // 读入Normal
                        var res = line.Split(' ');
                        vertexsNormal.Add(new Vector3d(double.Parse(res[1]), double.Parse(res[2]), double.Parse(res[3])));
                    } else if (line.StartsWith("vt")) { //顶点UV数据
                        var res = line.Split(' ');
                        vertexsUV.Add(new(double.Parse(res[1]), double.Parse(res[2])));
                    } else if (line.StartsWith("f")) { //面，读取到的是索引
                        int stIndex = faceData.Count;
                        var res = line.Split(' ');
                        for (int i = 1; i <= 3; i++) {
                            var temp = res[i];
                            var index = temp.Split('/');
                            faceData.Add(new ObjData(int.Parse(index[0]), int.Parse(index[1]), int.Parse(index[2])));
                        }

                        mTriangle.Add(new Triangle(vertexsPos[faceData[stIndex].vertIdex], vertexsPos[faceData[stIndex + 1].vertIdex], vertexsPos[faceData[stIndex + 2].vertIdex], stIndex, stIndex + 1, stIndex + 2));

                        //  tList.Add(mTriangle[mTriangle.Count - 1]);

                    } else if (line.StartsWith("o")) { //模型名字
                        var res = line.Split(' ');
                        objName = res[1];
                    }
                    line = sr.ReadLine();
                }
                //构建BVH树
                Console.WriteLine($"构建OBJ模型的BVH：{objName}");
                mBVH = new BVH_Tree(mTriangle);
            }
            catch (Exception e) {
                Console.WriteLine("创建Obj格式模型出错了\n" + objName + "：\n" + e.Message);
                MessageBox.Show($"读取Obj格式文件出错\n{e.Message}\nOBJ名称：{objName}");
                throw new Exception("模型创建失败");
            }
        }
        public ObjModel(string path, Material mat) {
            ObjBuild(path);
            this.mat = mat;
        }
        public ObjModel(string path) {
            ObjBuild(path);
        }
        public ObjModel() { }

        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {

            //var tp = tList.Hit(ray, t_min, t_max, out hitResult);

            var tp = mBVH.Hit(ray, t_min, t_max, out hitResult);

            if (tp == false) { return false; }
            // 从三角形局部坐标，转为整个Obj的UV（使用三角形重心表示法，利用u*P0+v*P1+(1-u-v)*P2）
            var recObj = hitResult.hitObj as Triangle;//都是三角面
            // 根据读入Obj的顶点法线，线性插值，法线方向
            var resOutNormal = hitResult.u * vertexsNormal[faceData[recObj.p1Index].NorIdex] + hitResult.v * vertexsNormal[faceData[recObj.p2Index].NorIdex] + (1 - hitResult.u - hitResult.v) * vertexsNormal[faceData[recObj.p0Index].NorIdex];
            //按照比例混合U和V
            hitResult.u = hitResult.u * vertexsUV[faceData[recObj.p1Index].TexIdex].Item1 + hitResult.v * vertexsUV[faceData[recObj.p2Index].TexIdex].Item1 + (1 - hitResult.u - hitResult.v) * vertexsUV[faceData[recObj.p0Index].TexIdex].Item1;

            hitResult.v = hitResult.u * vertexsUV[faceData[recObj.p1Index].TexIdex].Item2 + hitResult.v * vertexsUV[faceData[recObj.p2Index].TexIdex].Item2 + (1 - hitResult.u - hitResult.v) * vertexsUV[faceData[recObj.p0Index].TexIdex].Item2;

            hitResult.Set_Face_Normal(ray, resOutNormal);
            hitResult.mat = this.mat;

            return tp;
        }
        public override bool Bounding_Box(out AABB? output_box) {
            return mBVH.Bounding_Box(out output_box);
        }
        public override object Clone() {
            return this;
            var res = new ObjModel();
            foreach (var item in mTriangle) res.mTriangle.Add(item);
            foreach (var item in vertexsPos) res.vertexsPos.Add(item);
            foreach (var item in vertexsNormal) res.vertexsNormal.Add(item);
            foreach (var item in vertexsUV) res.vertexsUV.Add(item);
            foreach (var item in faceData) res.faceData.Add(item);
            res.mBVH = new(res.mTriangle);
            res.mat = this.mat;
            return res;
        }
    }
}
