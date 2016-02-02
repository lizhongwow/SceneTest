using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneTestLib
{
    public interface IPoint2D
    {
        double x { get; set; }
        double y { get; set; }
    }
    public class Point2D : IPoint2D
    {
        public double x { get; set; }
        public double y { get; set; }

        /// <summary>
        /// 0 表示可走
        /// </summary>
        public int walkable { get; set; }
        /// <summary>
        /// 用于寻路时的计算
        /// </summary>
        public int distance { get; set; }

        public int index { get; set; }

        public Point2D() { }

        public Point2D(int _x, int _y)
        {
            this.x = _x;
            this.y = _y;
        }

        public Point2D(double _x, double _y)
        {
            this.x = _x;
            this.y = _y;
        }
    }
}
