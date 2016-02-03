using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SceneTestLib.Confs
{
    public class map_conf
    {
        public const string xml_node_name = "map";

        public int id { get; set; }
        public int name { get; set; }
        public int tile_size { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int tile_set { get; set; }
        public int pk { get; set; }

        public int immrespawn { get; set; }

        public List<map_mon_conf> map_mon { get; set; }
        public List<pk_zone_conf> pk_zone { get; set; }

        public map_grd_conf map_grd { get; set; }

        public map_conf()
        {
            this.map_mon = new List<map_mon_conf>();
            this.pk_zone = new List<pk_zone_conf>();
        }
    }

    public class pk_zone_conf
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class map_grd_conf
    {
        public string file { get; set; }
    }

    public class map_mon_conf
    {
        public const string xml_node_name = "map_mon";

        public int mid { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int r_x { get; set; }
        public int r_y { get; set; }
        public int spwan_time { get; set; }

        public int sideid { get; set; }
    }

    public class born_pos
    {
        public int x { get; set; }
        public int y { get; set; }
        public int mpid { get; set; }

        public int w { get; set; }

        public int h { get; set; }
    }

    public class mon_conf
    {
        public const string xml_node_name = "mon_conf";
        public int id { get; set; }
        public string name { get; set; }
        public string obj { get; set; }
        public int size { get; set; }
        public int drop { get; set; }
        public int shkil { get; set; }
        public int exp { get; set; }
        public int boss { get; set; }

        public mon_att mon_att { get; set; }

        public mon_ai mon_ai { get; set; }
    }

    public class mon_att
    {
        public const string xml_node_name = "mon_att";
        public int level { get; set; }
        public int hp { get; set; }
        public int mp { get; set; }
        public int atk_min { get; set; }
        public int atk_max { get; set; }
        public int matk_min { get; set; }
        public int matk_max { get; set; }
        public int def { get; set; }
        public int mdef { get; set; }
        public int atktp { get; set; }
        public int atk_rate { get; set; }
        public int miss_rate { get; set; }
        public int atkcdtm { get; set; }
        public int speed { get; set; }
        public int atkrange { get; set; }
        public int exatk { get; set; }
        public int exdmg { get; set; }
    }

    public class mon_ai
    {
        public const string xml_node_name = "mon_ai";
        public int thinktm { get; set; }
        public int mover { get; set; }
        public int aggres { get; set; }
        public int revange { get; set; }
        public int defrang { get; set; }
        public int tracerang { get; set; }
        public int running_tm { get; set; }
    }
}
