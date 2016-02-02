using SceneTestLib.Confs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneTestLib
{
    public class Grd
    {
        public Point2D[] grd_ary = null;
        public Point2D[] walkable_grd = null;

        public int width = 0;
        public int height = 0;
        public int length = 0;


        public const string file_folder = "";// @"D:\PrimaryServer\whserver\\";

        public Grd(map_conf map)
        {
            this.width = map.width;
            this.height = map.height;

            FileStream fs = new FileStream(file_folder + map.map_grd.file, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            var ba = sr.ReadToEnd().ToCharArray();

            length = ba.Length / 2;
            grd_ary = new Point2D[length];

            List<Point2D> walkables = new List<Point2D>();

            for (int i = 0; i < length; i++)
            {
                short ba1 = (short)(((short)ba[i * 2]) << 8);
                short ba2 = (short)ba[i * 2 + 1];
                //short g_walkable = (short)(((short)ba[i * 2]) << 8 + ba[i * 2 + 1]);
                int g_walkable = ba1 + ba2;
                Point2D grid = new Point2D(i / this.width, i % this.width);
                grid.walkable = g_walkable;
                grid.distance = int.MaxValue;
                grid.index = i;
                grd_ary[i] = grid;

                if (g_walkable == 0)
                    walkables.Add(grid);
            }

            walkable_grd = walkables.ToArray();

            sr.Close();
            fs.Close();
        }


        private void clear_distance()
        {
            foreach (var g in walkable_grd)
                g.distance = int.MaxValue;
        }

        public Point2D get_grid(int g_x, int g_y)
        {
            int idx = g_y * this.width + g_x;

            if (idx >= this.grd_ary.Length)
                return null;

            return this.grd_ary[idx];
        }

        public bool is_grid_walkable(int g_x, int g_y)
        {
            Point2D p = get_grid(g_x, g_y);
            if (null == p)
                return false;

            return (p.walkable == 0);
        }

        /// <summary>
        /// 寻路（线程不安全）
        /// </summary>
        /// <param name="s_x"></param>
        /// <param name="s_y"></param>
        /// <param name="d_x"></param>
        /// <param name="d_y"></param>
        /// <returns></returns>
        public List<Point2D> find_Path(int s_x, int s_y, int d_x, int d_y)
        {
            clear_distance();

            int index_src = s_x * this.width + s_y;
            int index_dst = d_x * this.width + d_y;

            if (index_src >= this.grd_ary.Length || index_dst >= this.grd_ary.Length)
                return null;

            Dictionary<int, Point2D> processed = new Dictionary<int, Point2D>();
            processed[index_src] = grd_ary[index_src];
            grd_ary[index_src].distance = 0;

            Dictionary<int, Point2D> new_processed1 = new Dictionary<int, Point2D>();
            new_processed1.Add(index_src, grd_ary[index_src]);


            Dictionary<int, Point2D> new_processed2 = new Dictionary<int, Point2D>();

            int step = 1;
            //TODO 增加条件，防止死循环
            bool not_reached = true;
            while (not_reached)
            {
                if (step > 1000)
                    throw new Exception("Grd.find_path,step to big,check if the destination is reachable");

                new_processed2.Clear();

                foreach (Point2D p in new_processed1.Values)
                {
                    int p_index = p.index;
                    //if (processed.ContainsKey(p_index))
                    //    continue;

                    Point2D p_check = null;
                    int p_dis = 0;

                    int idx = p_index - 1;
                    if (idx >= 0 && idx < length)
                    {
                        if (!processed.ContainsKey(idx))
                        {
                            p_check = grd_ary[idx];
                            p_dis = p_check.distance;
                            if (step < p_dis)
                            {
                                p_check.distance = step;
                                new_processed2.Add(idx, p_check);
                                processed.Add(idx, p_check);

                                if (idx == index_dst)
                                {
                                    not_reached = false;
                                    break;
                                }
                            }
                        }
                    }

                    idx = p_index + 1;
                    if (!processed.ContainsKey(idx))
                        if (idx >= 0 && idx < length)
                        {
                            p_check = grd_ary[idx];
                            p_dis = p_check.distance;
                            if (step < p_dis)
                            {
                                p_check.distance = step;
                                new_processed2.Add(idx, p_check);
                                processed.Add(idx, p_check);

                                if (idx == index_dst)
                                {
                                    not_reached = false;
                                    break;
                                }
                            }
                        }

                    idx = p_index - width;
                    if (!processed.ContainsKey(idx))
                        if (idx >= 0 && idx < length)
                        {
                            p_check = grd_ary[idx];
                            p_dis = p_check.distance;
                            if (step < p_dis)
                            {
                                p_check.distance = step;
                                new_processed2.Add(idx, p_check);
                                processed.Add(idx, p_check);

                                if (idx == index_dst)
                                {
                                    not_reached = false;
                                    break;
                                }
                            }
                        }

                    idx = p_index + width;
                    if (!processed.ContainsKey(idx))
                        if (idx >= 0 && idx < length)
                        {
                            p_check = grd_ary[idx];
                            p_dis = p_check.distance;
                            if (step < p_dis)
                            {
                                p_check.distance = step;
                                new_processed2.Add(idx, p_check);
                                processed.Add(idx, p_check);

                                if (idx == index_dst)
                                {
                                    not_reached = false;
                                    break;
                                }
                            }
                        }
                }

                step++;

                new_processed1.Clear();

                var list = new_processed1;
                new_processed1 = new_processed2;
                new_processed2 = list;

                not_reached = !processed.ContainsKey(index_dst);
            }

            List<Point2D> path = new List<Point2D>();
            path.Add(grd_ary[index_dst]);
            int c_p = index_dst;
            step -= 2;
            while (step >= 0)
            {
                int c_p_idx = c_p - 1;
                if (processed.ContainsKey(c_p_idx))
                {
                    if (processed[c_p_idx].distance == step)
                    {
                        path.Add(grd_ary[c_p_idx]);
                        step--;
                        c_p = c_p_idx;
                    }
                }

                c_p_idx = c_p + 1;
                if (processed.ContainsKey(c_p_idx))
                {
                    if (processed[c_p_idx].distance == step)
                    {
                        path.Add(grd_ary[c_p_idx]);
                        step--;
                        c_p = c_p_idx;
                    }

                }

                c_p_idx = c_p - this.width;
                if (processed.ContainsKey(c_p_idx))
                {
                    if (processed[c_p_idx].distance == step)
                    {
                        path.Add(grd_ary[c_p_idx]);
                        step--;
                        c_p = c_p_idx;
                    }

                }

                c_p_idx = c_p + this.width;
                if (processed.ContainsKey(c_p_idx))
                {
                    if (processed[c_p_idx].distance == step)
                    {
                        path.Add(grd_ary[c_p_idx]);
                        step--;
                        c_p = c_p_idx;
                    }

                }
            }

            path.Reverse();

            return path;
        }        
    }
}
