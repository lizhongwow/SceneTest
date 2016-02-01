using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SceneTestLib.Confs
{
    public enum skill_aff_type
    {
        SAT_SELF = 1,
        SAT_ALLY = 2,
        SAT_ENERMY = 4,
        SAT_MID = 8,//玩家：不是盟友的玩家   怪物：不是盟友的怪物
    };

    public class bstate_conf
    {
        public const string xml_node_name = "bstate";

        public int id { get; set; }
        public string name { get; set; }
        public int uni_tp { get; set; }
        public int merg_tm { get; set; }

        public int tm { get; set; }

        public eff_conf eff { get; set; }

        public List<s_conf> s { get; set; }

        public bstate_conf()
        {
            s = new List<s_conf>();
        }
    }

    public class eff_conf
    {
        public const string xml_node_name = "eff";

        public int tp { get; set; }

        public int par { get; set; }

        public int maxpar { get; set; }

        public int ticktm { get; set; }

        public int add { get; set; }
    }

    public class state_conf
    {
        public const string xml_node_name = "state";

        public int id { get; set; }
        public int tp { get; set; }
        public int max_cnt { get; set; }
        public int gb { get; set; }
        public int off_rec { get; set; }
        public int die_keep { get; set; }
        public string name { get; set; }
        public string eff { get; set; }
        public int mvaction { get; set; }
        public int hate { get; set; }

        public unique_conf unique { get; set; }

        public state_atkcnt_conf atkcnt { get; set; }

        public timer_conf timer { get; set; }

        public absorbdmg_conf absorbdmg { get; set; }

        public List<s_conf> s_states { get; set; }

        public List<att_conf> att { get; set; }

        public state_conf()
        {
            s_states = new List<s_conf>();
            att = new List<att_conf>();
        }
    }

    public class att_conf
    {
        public const string xml_node_name = "att";

        public int con_add { get; set; }
        public int hpsteal { get; set; }

        public int str_add { get; set; }
        public int inte_add { get; set; }
        public int agi_add { get; set; }
        public int wis_add { get; set; }
        public int mpsteal { get; set; }
        public int ice_add { get; set; }
        public int poi_add { get; set; }
        public int lit_add { get; set; }

        public int fir_add { get; set; }
        public int eth_add { get; set; }

        public int wnd_add { get; set; }
        public int wat_add { get; set; }
        public int castspd { get; set; }
        public int atkcd_add { get; set; }
        public int movspd_add { get; set; }
    }

    public class dmg_conf
    {
        public const string xml_node_name = "dmg";

        public int aff { get; set; }
        public int rate { get; set; }
        public int dmg_min { get; set; }
        public int dmg_max { get; set; }

        public int atk_dmg { get; set; }
        public int matk_dmg { get; set; }
        public int nag_dmg { get; set; }
        public int pos_dmg { get; set; }
        public int voi_dmg { get; set; }
        public int poi_dmg { get; set; }
        public int hp_dmg { get; set; }
        public int mp_dmg { get; set; }
        public int agry_dmg { get; set; }

        public damage convert2damage()
        {
            return new damage();
        }

        public tres_conf convert2tres()
        {
            return new tres_conf(0);
        }
    }

    public class damage
    {
        public int dmg_min { get; set; }
        public int dmg_max { get; set; }

        public int noratk { get; set; }

        public int plyatk { get; set; }

        public int attr = -1;

        public int hp_dmg { get; set; }

        public int mp_dmg { get; set; }
    }

    public class timer_conf
    {
        public const string xml_node_name = "timer";

        public int interval { get; set; }
        public dmg_conf dmg { get; set; }

        public tres_conf add_stat { get; set; }

        public rang_conf rang { get; set; }
    }

    public class tres_conf
    {
        public const string xml_node_name = "tres";

        public tres_conf(int state_id)
        {
            state_tm = 10;
            tar_state = state_id;
        }
        public int state_par { get; set; }
        public int aff { get; set; }
        public int tar_state { get; set; }
        public int state_tm { get; set; }

        public int no_dmg { get; set; }

        public int rate { get; set; }

        public int stat_rat { get; set; }

        public rang_conf rang { get; set; }

        public int rmv_stat { get; set; }

        public int rmv_1stat { get; set; }

        public List<cd_red_conf> cd_red { get; set; }

        public force_move_violent_conf fmv { get; set; }

        public damage convert2damage()
        {
            return new damage();
        }
        

        public tres_conf()
        {
            cd_red = new List<cd_red_conf>();
        }
    }

    public class skill_state_add_conf
    {
        public int stid { get; set; }
        public int spt { get; set; }
    }

    public class force_move_violent_conf
    {
        public int dir { get; set; }
        public int rang { get; set; }

        public int rate { get; set; }
    }

    public class cd_red_conf
    {
        public const string xml_node_name = "cd_red";
        public int sktp { get; set; }
        public int red_tm { get; set; }
    }

    public class rang_conf
    {
        public const string xml_node_name = "rang";

        public int maxi { get; set; }
        public int cirang { get; set; }

        public ray_conf ray { get; set; }

        public fan_conf fan { get; set; }
    }

    public class ray_conf
    {
        public const string xml_node_name = "ray";

        public int dist { get; set; }
        public int width { get; set; }
    }

    public class fan_conf
    {
        public const string xml_node_name = "fan";

        public int angle { get; set; }
        public int rang { get; set; }

        public List<decay_conf> decay { get; set; }

        public fan_conf()
        {
            decay = new List<decay_conf>();
        }
        
    }

    public class decay_conf
    {
        public const string xml_node_name = "decay_conf";
        public int min { get; set; }
        public int max { get; set; }

        public int per { get; set; }
    }


    public class absorbdmg_conf
    {
        public const string xml_node_name = "absorbdmg_conf";
        public int per { get; set; }
        public int maxdmg { get; set; }
    }

    public class unique_conf
    {
        public const string xml_node_name = "unique";
        public int uni_tp { get; set; }
        public int uni_prior { get; set; }

        public unique_conf()
        {

        }
    }

    public class state_atkcnt_conf
    {
        public int cnt { get; set; }
        public int hitaction { get; set; }

        public int add_stat { get; set; }
        public int dmg { get; set; }

    }

    public class s_conf
    {
        public const string xml_node_name = "s";

        public int tp { get; set; }
        public int par { get; set; }
    }

    public class skill_lv_conf
    {
        public const string xml_node_name = "skill_lv";

        public int lv { get; set; }
        public jump_conf jump { get; set; }
        public teleport_conf teleport { get; set; }
        public rang_conf rang { get; set; }
        public List<tres_conf> tres { get; set; }
        public tres_conf sres { get; set; }
        public int cd { get; set; }

        public skill_lv_conf()
        {
            tres = new List<tres_conf>();
        }
    }

    public class jump_conf
    {
        public const string xml_node_name = "jump_conf";
        public int jump_rang { get; set; }
        public int speed { get; set; }

        public List<tres_conf> tres { get; set; }
        public rang_conf rang { get; set; }

        public jump_conf()
        {
            tres = new List<tres_conf>();
        }
    }

    public class teleport_conf
    {
        public const string xml_node_name = "teleport_conf";
        public int tm { get; set; }
        public int rang { get; set; }
    }

    public class skill_conf
    {
        public int id { get; set; }
        public bool igccstate { get; set; }
        public int tar_tp { get; set; }
        public int aff { get; set; }
        public int cd_tm { get; set; }
        public int cast_rang { get; set; }
        public int cast_tm { get; set; }
        public int hold_tm { get; set; }
        public List<skill_lv_conf> skill_lv { get; set; }

        public skill_conf()
        {
            skill_lv = new List<skill_lv_conf>();
        }

        public bool Is_Target_Enermy()
        {
            return skill_target_type_validation(this.aff, skill_aff_type.SAT_ENERMY);
        }

        public bool Is_Target_SELF()
        {
            return skill_target_type_validation(this.aff, skill_aff_type.SAT_SELF);
        }

        public bool Is_Target_ALAI()
        {
            return skill_target_type_validation(this.aff, skill_aff_type.SAT_ALLY);
        }

        public bool Is_Target_MID()
        {
            return skill_target_type_validation(this.aff, skill_aff_type.SAT_MID);
        }

        public skill_lv_conf GetSkillLevelConf(int level)
        {
            return new skill_lv_conf();
        }

        public static bool skill_target_type_validation(int skill_aff, skill_aff_type check_aff_type)
        {
            return ((skill_aff & (int)check_aff_type) == (int)check_aff_type);
        }
    }
}
