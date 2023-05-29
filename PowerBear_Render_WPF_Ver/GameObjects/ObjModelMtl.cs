using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using PowerBear_Render_WPF_Ver.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    public class ObjModelMtl : HitTable {
        [XmlIgnore]
        public BVH_Tree? mBVH; // 为了XML序列化后退出

        public List<HitTable> mTriangle = new List<HitTable>();
        public List<Vector3d> vertexsPos = new();
        public List<Vector3d> vertexsNormal = new();
        public List<Tuple<double, double>> vertexsUV = new();
        public List<ObjData> faceData = new();
        public string objPath = ""; // obj模型文件的路径
        public string mtlPath = ""; // mtl材质纹理文件的路径
        public Dictionary<String, Material> matNames = new();// "材质名字"，"索引编号"
        String basedPath = ""; // 本模型文件的主要文件夹

        public ObjModelMtl() { }
        public ObjModelMtl(string objPath, string? mtlPath = null) {
            // 无论怎么说，都应该先获得材质数组的信息
            var resPath = Path.GetDirectoryName(objPath);
            try {
                if (resPath == null) throw new Exception("非法路径");
                basedPath = resPath;
                Console.WriteLine("ObjModelMtl: READ文件根目录" + basedPath);
                // 以objPath文件里面保存的mtl贴图信息为准
                using StreamReader sr = new StreamReader(objPath);
                var line = sr.ReadLine();
                while (line != null) {
                    if (line.StartsWith("mtllib ")) {
                        var res = line.Split(' ');
                        mtlPath = res[1];
                        break;
                    }
                    line = sr.ReadLine();
                }
                // 检验模型文件有无mtl贴图文件
                if (mtlPath == null) { throw new Exception("NO MTL FILE!"); }
                // 开始创建材质贴图
                MatsBuild(Path.Combine(basedPath, mtlPath));
                // 构建模型数据
                ObjBuild(objPath);
            }
            catch (Exception ex) { Console.WriteLine("ObjModelMtl: READ FAILED | " + ex.Message, Console.Error); }
        }
        void AddNewMat(Material mat, String matName) {
            matNames[matName] = mat;
        }
        // 材质数组构建
        void MatsBuild(string mtlPath) {
            try {
                Console.WriteLine("ObjModelMtl: 读取材质 " + mtlPath);
                using StreamReader sr = new StreamReader(mtlPath);
                var line = sr.ReadLine();
                // new mat info
                String? mtl_name = null;
                String? image_name = null;
                double kd_r = 1, kd_g = 1, kd_b = 1;//有些文件没有kd，默认为1
                while (line != null) {
                    if (line.StartsWith("newmtl ")) {
                        var res = line.Split(' ');

                        if (mtl_name != null) {
                            Material material = new Lambertian(kd_r, kd_g, kd_b);
                            if (image_name != null) {
                                material.mTexture = new ImageTexture(image_name);
                            }
                            AddNewMat(material, mtl_name);
                        }

                        mtl_name = res[1]; // 初始化，为了下次的读入
                        image_name = null;
                        kd_r = 1; kd_g = 1; kd_b = 1;
                    } else if (line.StartsWith("Kd ")) {
                        var res = line.Split(' ');
                        kd_r = double.Parse(res[1]); kd_g = double.Parse(res[2]); kd_b = double.Parse(res[3]);
                    } else if (line.StartsWith("map_Kd ")) {
                        var res = line.Split(' ');
                        image_name = Path.Combine(basedPath, res[1]);
                    }
                    line = sr.ReadLine();
                }

                if (mtl_name != null) {
                    Material material = new Lambertian(kd_r, kd_g, kd_b);
                    if (image_name != null) {
                        material.mTexture = new ImageTexture(image_name);
                    }
                    AddNewMat(material, mtl_name);
                }
            }
            catch (Exception ex) { Console.WriteLine("ObjModelMtl 读取mtl出错: " + ex.Message, Console.Error); }
        }
        Hittable_List hittable_List = new Hittable_List();
        // Obj模型文件构建
        void ObjBuild(string objPath) {
            Console.WriteLine("ObjModelMtl: 读取模型数据 " + objPath);
            //使得下标都是从1开始
            vertexsPos.Add(Vector3d.Zero);
            vertexsNormal.Add(Vector3d.Zero);
            vertexsUV.Add(new(0, 0));
            faceData.Add(new());

            //从文件IO流进行读入
            try {
                using StreamReader sr = new StreamReader(objPath);
                var line = sr.ReadLine();
                Material nowMat = new Lambertian(0.5d, 0.5d, 0.5d);
                while (line != null) {
                    if (line.StartsWith("usemtl ")) { // 往下的三角面应该使用此材质数据
                        var res = line.Split(' ');
                        nowMat = matNames[res[1]];
                    } else if (line.StartsWith("v ")) { // 读入顶点
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
                        var triangleObject = new Triangle(vertexsPos[faceData[stIndex].vertIdex], vertexsPos[faceData[stIndex + 1].vertIdex], vertexsPos[faceData[stIndex + 2].vertIdex], stIndex, stIndex + 1, stIndex + 2);

                        triangleObject.mat = nowMat;

                        mTriangle.Add(triangleObject);
                        // hittable_List.Add(triangleObject);
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
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {

            var tp = mBVH.Hit(ray, t_min, t_max, out hitResult);

            //var tp = hittable_List.Hit(ray, t_min, t_max, out hitResult);


            if (tp == false) { return false; }
            // 从三角形局部坐标，转为整个Obj的UV（使用三角形重心表示法，利用u*P0+v*P1+(1-u-v)*P2）
            var recObj = hitResult.hitObj as Triangle;//都是三角面
            // 根据读入Obj的顶点法线，线性插值，法线方向
            var resOutNormal = hitResult.u * vertexsNormal[faceData[recObj.p1Index].NorIdex] + hitResult.v * vertexsNormal[faceData[recObj.p2Index].NorIdex] + (1 - hitResult.u - hitResult.v) * vertexsNormal[faceData[recObj.p0Index].NorIdex];
            //按照比例混合U和V
            hitResult.u = hitResult.u * vertexsUV[faceData[recObj.p1Index].TexIdex].Item1 + hitResult.v * vertexsUV[faceData[recObj.p2Index].TexIdex].Item1 + (1 - hitResult.u - hitResult.v) * vertexsUV[faceData[recObj.p0Index].TexIdex].Item1;

            hitResult.v = hitResult.u * vertexsUV[faceData[recObj.p1Index].TexIdex].Item2 + hitResult.v * vertexsUV[faceData[recObj.p2Index].TexIdex].Item2 + (1 - hitResult.u - hitResult.v) * vertexsUV[faceData[recObj.p0Index].TexIdex].Item2;

            hitResult.Set_Face_Normal(ray, resOutNormal);

            return tp;
        }
        public override bool Bounding_Box(out AABB? output_box) {
            //Console.WriteLine("DEBUG三角形求交：" + hittable_List.objects.Count);
            //return hittable_List.Bounding_Box(out output_box);
            return mBVH.Bounding_Box(out output_box);
        }
        public override object Clone() {
            return this;
            var res = new ObjModelMtl();
            foreach (var item in mTriangle) res.mTriangle.Add(item);
            foreach (var item in vertexsPos) res.vertexsPos.Add(item);
            foreach (var item in vertexsNormal) res.vertexsNormal.Add(item);
            foreach (var item in vertexsUV) res.vertexsUV.Add(item);
            foreach (var item in faceData) res.faceData.Add(item);
            foreach (var item in matNames) res.matNames.Add(item.Key, item.Value);
            // foreach (var item in hittable_List.objects) res.hittable_List.Add(item);
            res.mBVH = new(res.mTriangle);
            return res;
        }
    }
}
// obj格式解析 https://blog.csdn.net/xiaxl/article/details/76893165
// mtl格式解析 https://blog.csdn.net/weixin_33937913/article/details/94031011
// https://zhuanlan.zhihu.com/p/516546645?utm_id=0