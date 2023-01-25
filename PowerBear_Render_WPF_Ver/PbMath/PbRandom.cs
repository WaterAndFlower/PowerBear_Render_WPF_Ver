using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.PbMath {
    public class PbRandom {
        /// <summary>
        /// 返回一个[0,1)的double类型随机数
        /// </summary>
        /// <returns>返回[0,1)</returns>
        public static double Random_Double() {
            Random random = new Random();
            return random.NextDouble();
        }
        /// <summary>
        /// 返回一个[0,1)的double类型随机数
        /// </summary>
        /// <param name="seed">随机数种子</param>
        /// <returns>返回[0,1)</returns>
        public static double Random_Double(int seed) {
            Random random = new Random(seed);
            return random.NextDouble();
        }
        /// <summary>
        /// 取随机数返回[min,max)
        /// </summary>
        /// <param name="minn">最小</param>
        /// <param name="maxn">最大</param>
        /// <returns></returns>
        public static double Random_Double(double minn, double maxn) {
            Random random = new Random();
            return minn + (maxn - minn) * random.NextDouble();
        }
        public static double Random_Double(double minn, double maxn, int seed) {
            Random random = new Random(seed);
            return minn + (maxn - minn) * random.NextDouble();
        }
        public static int Random_int(int min, int max) {
            Random random = new Random();
            return (int)(Random_Double(min, max + 1));
        }
    }
}
