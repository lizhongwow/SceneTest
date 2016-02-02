using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneTestLib.Confs
{
    public class level_conf
    {
        public int tpid { get; set; }

        public int lmtp{get;set;}
        public int lctp { get; set; }
        public int lptp { get; set; }
        public int arenaid { get; set; }
        public int tm { get; set; }
        public int maxply { get; set; }
        public int npcentry { get; set; }
        public int gpmap_maxply { get; set; }
        public int rmv_leave_ply { get; set; }
        public int plyrespawn_tm { get; set; }
        public int immrespawn { get; set; }
        public int tm_out { get; set; }

        public int exitfin { get; set; }

        public List<level_map_conf> level_map { get; set; }

        public pvp_conf pvp { get; set; }

        public List<diff_lvl_conf> diff_lvl { get; set; }

        public fin_check_conf fin_check { get; set; }

        public List<score_conf> score { get; set; }

        //public born_pos born { get; set; }

        public level_conf()
        {
            this.diff_lvl = new List<diff_lvl_conf>();
            this.level_map = new List<level_map_conf>();
            score = new List<score_conf>();
        }

        public diff_lvl_conf get_diff_lvl_conf(int diff_level)
        {
            foreach (var lvl in diff_lvl)
                if (lvl.lv == diff_level)
                    return lvl;

            return null;
        }
    }

    public class score_conf
    {
        public int score { get; set; }
        public int tm { get; set; }
    }

    public class level_map_conf
    {
        public int id { get; set; }

        public born_pos born {get;set;}
    }



    public class death_match_conf
    {
        public int preptm { get; set; }
        public int die2ghost { get; set; }
        public List<side_conf> side { get; set; }

        public death_match_conf()
        {
            side = new List<side_conf>();
        }
    }

    public class side_conf
    {
        public int id { get; set; }
        public born_pos born { get; set; }
    }

    public class diff_lvl_conf
    {
        public int lv { get; set; }

        public win_awd_conf win_awd { get; set; }

        public born_pos born { get; set; }

        public List<level_map_conf> level_map { get; set; }

        public diff_lvl_conf()
        {
            this.level_map = new List<level_map_conf>();
        }

        public fin_check_conf fin_check { get; set; }
    }

    public class win_awd_conf
    {
        public int hexp { get; set; }
    }

    public class fin_check_conf
    {
        public int tm { get; set; }
        public List<km_conf> km { get; set; }
        public fin_check_conf()
        {
            this.km = new List<km_conf>();
        }
        
    }

    public class km_conf
    {
        public int mid { get; set; }
        public int cnt { get; set; }
    }

    public class pvp_conf
    {
        public int ignore_team { get; set; }
        public int ignore_clan { get; set; }
        public int plyside { get; set; }

        public death_match_conf death_match { get; set; }
        public death_pt_conf death_pt { get; set; }

        public List<round_conf> round { get; set; }
        public List<side_conf> side { get; set; }
        public pvp_conf()
        {
            round = new List<round_conf>();
            side = new List<side_conf>();
        }
    }

    public class round_conf
    {
        public int id { get; set; }
        public int tm { get; set; }
        
        public win_awd_conf win_awd { get; set; }

        public death_pt_conf death_pt { get; set; }

        public death_match_conf death_match { get; set; }

        public List<side_conf> side { get; set; }
        public round_conf()
        {
            side = new List<side_conf>();
        }
    }

    public class death_pt_conf
    {
        public int preptm { get; set; }
        public int wincnt { get; set; }
    }
}
