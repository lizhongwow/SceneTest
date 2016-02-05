
using SceneTestLib;
using SceneTestLib.Confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace SceneTest
{
    public class damage_attr
    {

    }

    public static class ExpandedFunc
    {
        public static string pop(this DateTime dt)
        {
            return dt.ToString();
        }

        public static T pop<T>(this List<T> list)
        {
            if (list == null || list.Count <= 0)
                return default(T);

            int l = list.Count;
            T last = list[l - 1];
            list.RemoveAt(l - 1);

            return last;
        }

        public static List<T> push<T>(this List<T> list, T element)
        {
            if (list == null)
                return null;

            list.Add(element);

            return list;
        }

        public static List<T> slice<T>(this List<T> list, int start_index)
        {
            if (list.Count <= start_index)
                return new List<T>();

            return list.Skip(start_index).ToList();
        }
    }
    public class statics
    {



    }




    public class BState
    {
        public bstate_conf conf { get; set; }

        public int par { get; set; }

        public long end_tm { get; set; }

        public int id { get; set; }

        public long ticktm { get; set; }

    }
    public class MapUnitState
    {
        public int frm_iid { get; set; }
        public state_conf desc { get; set; }

        public long end_tm { get; set; }
        public long start_tm { get; set; }


        public int absorbed { get; set; }
        public int maxdmg { get; set; }

        public int atkedcnt { get; set; }

        public int per { get; set; }
        public int par { get; set; }

        public int id { get; set; }

        public long tm_elapsed { get; set; }

        public Dictionary<string, int> attadj { get; set; }

        public MapUnitState()
        {
            atkedcnt = -1;
        }
    }

    public class UnitState
    {
        public List<MapUnitState> state_par { get; set; }
        public int state { get; set; }
        public double mov_spd_mul { get; set; }
        public double atk_spd_mul { get; set; }
        public int mp2hp { get; set; }
        public int dmglnkper { get; set; }
    }



    public class Skill_Cd
    {
        public int skill_id { get; set; }
        public long last_tm { get; set; }
        public long cd_end_tm { get; set; }
    }


    public enum map_sprite_type
    {
        None = 0,
        MstMonster = 1,
        MstPlayer = 2,
    }
    public class Tracing
    {
        public int tar_iid { get; set; }
        public int trace_tm_left { get; set; }
        public int start_tm { get; set; }

        public int frang { get; set; }
        public int trang { get; set; }

        public bool do_ai { get; set; }
    }

    public static class StaticStuff
    {
        public const int GRID_WIDTH = 32;
    }

    public class map_item
    {
        public long left_tm { get; set; }
        public long dis_tm { get; set; }
    }


    public interface ISprite
    {
        IMapUnit get_pack_data();

        long last_trace_target_tm { get; set; }

        void remove_from_hurt_from_iids(int iid);

        //ISprite Owner_ply { get; set; }

        map_sprite_type get_sprite_type();

        bool isdie();

        bool isghost();

        void on_spr_die(int iid);

        bool Is_Player();

        bool Is_Monster();

        bool Is_Hero();

        bool ignore_dmg();

        void calc_weapon_dura(double dmg);
        void on_make_direct_dmg();
        void calc_eqp_dura(double dmg);

        void die(IBaseUnit frm_sprite, bool clear_owner = false);

        bool is_in_lvl { get; set; }

        void on_pos_change(double x, double y);

        void re_calc_cha_data();

        void modify_hp(int hp_add);

        void modify_mp(int mp_add);

        void set_invisible(int invisible);
        void set_observer(int observer);

        /// <summary>
        /// 正当防卫的对手cid列表，key是cid，value是到期的时间戳
        /// </summary>
        Dictionary<int, long> defend_tm_map { get; set; }

        void add_pk_v(int add_val);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sub_val"></param>
        /// <param name="sub_to_negtive">pk_v 是否可减至负值</param>
        void sub_pk_v(int sub_val, bool sub_to_negtive = false);

        bool is_justice();

        bool can_atk_direct();

        /// <summary>
        /// 被攻击玩家cid列表
        /// </summary>
        Dictionary<int, long> beatk_ply_cids { get; set; }

        void release_petmon(bool dead = false, bool clearinfo = true);

        List<IBaseUnit> get_inz_plys();
        List<IBaseUnit> get_inz_mons();

        /// <summary>
        /// 玩家sid 离开this的视野
        /// </summary>
        /// <param name="sid"></param>
        void rmv_zone_player(int sid);

        void respawn(int hp_percent, bool backtotown);

        Level in_lvl { get; set; }

        bool kp_asist_rec { get; set; }

        bool ignore_team { get; set; }
        bool ignore_clan { get; set; }

        long db_last_update_tm { get; set; }

        long allow_respawn_tm_s { get; set; }

        void update(IService service, long cur_time, long time_elapsed);
    }

    public class IBaseUnit : ISprite
    {
        //public atking atking { get; set; }
        //public moving moving { get; set; }
        //public jumping jumping { get; set; }

        public grid_map gmap { get; set; }

        public bool is_in_lvl { get; set; }

        public int iid
        {
            get
            {
                return get_pack_data().iid;
            }
        }


        public Dictionary<int, Skill_Cd> skill_cd { get; set; }


        public IMapUnit get_pack_data()
        {
            return null;
        }

        public bool has_state(int state)
        {
            return false;
        }

        public bool has_state(pl_state_type stp)
        {
            return ((this.get_pack_data().states.state | (int)stp) == (int)stp);
        }

        public int collect_tar { get; set; }

        public bool can_atk(IBaseUnit target)
        {
            return false;
        }

        public bool can_atk_direct()
        {
            return false;
        }

        public void atk_by(IBaseUnit from)
        {

        }

        public long last_trace_target_tm { get; set; }

        public void remove_from_hurt_from_iids(int iid)
        {

        }

        public IBaseUnit owner_ply { get; set; }

        public List<IBaseUnit> petmon_insts { get; set; }

        public void flush_db_data(bool a, bool b)
        {

        }

        public List<IBaseUnit> get_inz_plys()
        {
            IMapUnit unit_pl = this.get_pack_data();
            List<IBaseUnit> inz_plys = new List<IBaseUnit>();
            List<IBaseUnit> all_players = this.gmap.map_players_bycid.Values.ToList();
            foreach (var p in all_players)
            {
                IMapUnit pl = p.get_pack_data();

                if (pl.isdie || pl.ghost)
                    continue;

                if (Math.Abs(pl.grid_x - unit_pl.grid_x) > 10 || Math.Abs(pl.grid_y - unit_pl.grid_y) > 10)
                    continue;

                inz_plys.Add(p);
            }

            return inz_plys;
        }

        public List<IBaseUnit> get_observer_plys()
        {
            throw new NotImplementedException();
        }

        public List<IBaseUnit> get_non_observer_plys()
        {
            throw new NotImplementedException();
        }

        public List<IBaseUnit> get_inz_mons()
        {
            IMapUnit unit_pl = this.get_pack_data();
            List<IBaseUnit> inz_plys = new List<IBaseUnit>();
            List<IBaseUnit> all_players = this.gmap.map_mons.Values.ToList();
            foreach (var p in all_players)
            {
                IMapUnit pl = p.get_pack_data();

                if (pl.isdie || pl.ghost)
                    continue;

                if (Math.Abs(pl.grid_x - unit_pl.grid_x) > 10 || Math.Abs(pl.grid_y - unit_pl.grid_y) > 10)
                    continue;

                inz_plys.Add(p);
            }

            return inz_plys;
        }

        public Dictionary<int, long> defend_tm_map { get; set; }

        public Dictionary<int, long> beatk_ply_cids { get; set; }

        public Level in_lvl { get; set; }

        public bool kp_asist_rec { get; set; }

        public bool ignore_team { get; set; }

        public bool ignore_clan { get; set; }

        public long db_last_update_tm { get; set; }

        public long allow_respawn_tm_s { get; set; }

        public virtual bool isdie()
        {
            return false;
        }


        public map_sprite_type get_sprite_type()
        {
            return map_sprite_type.None;
        }


        public bool isaily(IBaseUnit spr)
        {
            return false;
        }




        public void on_spr_die(int iid)
        {
            throw new NotImplementedException();
        }


        public bool isghost()
        {
            throw new NotImplementedException();
        }

        public SkillData get_skil_data(int skill_id)
        {
            throw new NotImplementedException();
        }


        public bool Is_Player()
        {
            throw new NotImplementedException();
        }

        public bool Is_Monster()
        {
            throw new NotImplementedException();
        }

        public bool Is_Hero()
        {
            throw new NotImplementedException();
        }

        public bool Is_Skill_In_CD(int skill_id, long time_now)
        {
            Skill_Cd cd = null;
            this.skill_cd.TryGetValue(skill_id, out cd);
            if (null == cd)
                return false;

            return cd.cd_end_tm >= time_now;
        }

        public void Set_Skill_CD(int skill_id, long now, long end_tm)
        {
            Skill_Cd cd = null;
            this.skill_cd.TryGetValue(skill_id, out cd);
            if (null == cd)
            {
                skill_cd[skill_id] = new Skill_Cd() { cd_end_tm = end_tm, last_tm = now, skill_id = skill_id };
            }
            else
            {
                cd.cd_end_tm = end_tm;
                cd.last_tm = now;
            }
        }


        public bool ignore_dmg()
        {
            throw new NotImplementedException();
        }

        public void broad_cast_zone_msg_and_self(int cmd, Variant data)
        {
        }

        public void _move_to(double x, double y)
        {

        }

        public int get_atk_cd()
        {
            return 1000;
        }

        public void cancel_atk()
        {

        }

        public void on_hurt(IBaseUnit tar_sprite)
        {

        }

        public void on_be_hurt(IBaseUnit frm_sprite)
        {

        }

        public void onhate(IBaseUnit frm_sprite, double dmg)
        {

        }

        public void ondmg(IBaseUnit frm_sprite, double dmg)
        {

        }





        public void calc_weapon_dura(double dmg)
        {
            throw new NotImplementedException();
        }

        public void on_make_direct_dmg()
        {
            throw new NotImplementedException();
        }


        public void calc_eqp_dura(double dmg)
        {
            throw new NotImplementedException();
        }


        public void die(IBaseUnit frm_sprite, bool clear_owner = false)
        {
            throw new NotImplementedException();
        }

        public virtual void on_pos_change(double x, double y)
        {

        }

        public virtual void set_lvlside(int sideid)
        {

        }

        public virtual void set_conf_respawn_tm(long respawn_tm)
        {

        }

        public void re_calc_cha_data()
        {
            throw new NotImplementedException();
        }

        public void modify_hp(int hp_add)
        {
            throw new NotImplementedException();
        }

        public void modify_mp(int mp_add)
        {
            throw new NotImplementedException();
        }

        public void set_invisible(int invisible)
        {
            throw new NotImplementedException();
        }

        public void set_observer(int observer)
        {
            throw new NotImplementedException();
        }

        public void add_pk_v(int add_val)
        {
            throw new NotImplementedException();
        }

        public void sub_pk_v(int sub_val)
        {
            throw new NotImplementedException();
        }

        public void sub_pk_v(int sub_val, bool sub_to_negtive = false)
        {
            throw new NotImplementedException();
        }

        public bool is_justice()
        {
            throw new NotImplementedException();
        }

        public void release_petmon(bool dead = false, bool clearinfo = true)
        {
            throw new NotImplementedException();
        }

        public void rmv_zone_player(int sid)
        {
            throw new NotImplementedException();
        }

        public void respawn(int hp_percent, bool backtotown)
        {
            throw new NotImplementedException();
        }

        public void update(IService service, long cur_time, long time_elapsed)
        {
            throw new NotImplementedException();
        }
    }

    public interface IMapUnit : IPoint2D
    {
        Dictionary<int,SkillData> skills { get; set; }
        rednm_type rednm { get; set; }
        int grid_x { get; set; }
        int grid_y { get; set; }

        int pk_v { get; set; }

        int map_id { get; set; }
        //double x { get; set; }
        //double y { get; set; }

        int org_init_x { get; set; }
        int org_init_y { get; set; }

        double lx { get; set; }
        double ly { get; set; }
        int lmpid { get; set; }

        int sid { get; set; }
        int iid { get; set; }
        int line { get; set; }

        int cid { get; set; }

        int mid { get; set; }

        int owner_cid { get; set; }

        int lvlsideid { get; set; }

        int invisible { get; set; }

        bool in_pczone { get; set; }

        int size { get; set; }
        int atkrange { get; set; }

        Dictionary<int, SkillData> skill { get; set; }


        UnitState states { get; set; }

        List<BState> bstates { get; set; }

        atking atking { get; set; }

        moving moving { get; set; }
        jumping jumping { get; set; }

        holding holding { get; set; }

        casting casting { get; set; }

        teleping teleping { get; set; }

        follow follow { get; set; }

        int speed { get; set; }

        long skill_gen_cd { get; set; }
        long skill_gen_cdst { get; set; }

        int observer { get; set; }

        List<Point2D> last_mvpts { get; set; }

        long last_atk_tm { get; set; }
        int pkatkrate { get; set; }
        int pkmisrate { get; set; }
        int level { get; set; }
        int exp { get; set; }
        int atk_rate { get; set; }
        int miss_rate { get; set; }

        int def { get; set; }
        int pkdef { get; set; }
        int def_red { get; set; }

        int igdef_rate { get; set; }
        int igdef_rate_debuffs { get; set; }

        Dictionary<int, int> addResist { get; set; }

        Dictionary<int, int> skill_cd { get; set; }

        atk_type atktp { get; set; }

        int matk_min { get; set; }
        int matk_max { get; set; }

        int pkmatk { get; set; }

        int criatk { get; set; }
        int criatk_debuffs { get; set; }
        int exatk { get; set; }
        int exatk_debuffs { get; set; }

        int exper_add { get; set; }
        int exdmg { get; set; }
        int cridmg { get; set; }

        int atk_min { get; set; }
        int atk_max { get; set; }

        int pkatk { get; set; }

        int dmg_red { get; set; }
        int igdmg_red { get; set; }
        int pkdmg_red { get; set; }

        int hpsuck { get; set; }

        int hp { get; set; }
        int max_hp { get; set; }
        int dp { get; set; }
        int max_dp { get; set; }

        int mp { get; set; }
        int max_mp { get; set; }

        int hpsuck_dmgmul { get; set; }

        int atk_dmg_mul { get; set; }

        int igatk_dmg_mul { get; set; }

        int pkatk_dmg_mul { get; set; }

        int dobrate { get; set; }

        int pkigdp_rate { get; set; }
        int pkigdp_rate_debuffs { get; set; }

        int rev_dmg_mul { get; set; }
        int rev_atk { get; set; }

        /// <summary>
        /// 是否能被强制移动
        /// </summary>
        bool cfmv { get; set; }

        int full_hp_rate { get; set; }
        int full_mp_rate { get; set; }

        pk_state_type pk_state { get; set; }

        int atkcdtm { get; set; }

        bool isdie { get; set; }

        bool ghost { get; set; }

        int kp { get; set; }
        int cur_kp { get; set; }

        int teamid { get; set; }
        int clanid { get; set; }

        int llid { get; set; }

        long respawn_tm { get; set; }
    }

    public class monsterconf_att
    {
        public int lv { get; set; }
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

    public class monsterconf_ai
    {
        public int mover { get; set; }
        public int thinktm { get; set; }
        public int aggres { get; set; }
        public int defrang { get; set; }
        public int tracerang { get; set; }
        public int running_time { get; set; }
        public int hate_add { get; set; }
        public int hatp { get; set; }
        public int clhor { get; set; }

        public List<monsterconf_cond> cond = new List<monsterconf_cond>();
    }

    public class monsterconf_cond
    {
        public int die { get; set; }
        public int minlvl { get; set; }
        public int respawn { get; set; }

        public List<monstarconf_cond_act> act = new List<monstarconf_cond_act>();
    }

    public class monstarconf_cond_act
    {
        public int cnt { get; set; }
        public int actkey { get; set; }
        public int r { get; set; }
        public int castsk { get; set; }
        public List<int> trig { get; set; }

        public int x { get; set; }
        public int y { get; set; }

        public int suicide { get; set; }

        public int seltar { get; set; }
    }

    public class monsterconf
    {
        public static monsterconf default_value = new monsterconf();

        public int mid { get; set; }
        public int hp { get; set; }
        public int max_hp { get; set; }
        public int constitution { get; set; }
        public int strength { get; set; }
        public int intelligence { get; set; }
        public int physics { get; set; }
        public int magic { get; set; }
        public int physics_def { get; set; }
        public int magic_def { get; set; }
        public int critical_damage { get; set; }
        public int critical_def { get; set; }
        public int ice_att { get; set; }
        public int fire_att { get; set; }
        public int thunder_att { get; set; }
        public int ice_def { get; set; }
        public int fire_def { get; set; }
        public int thunder_def { get; set; }
        public int size { get; set; }

        public monsterconf_att att = new monsterconf_att();
        public monsterconf_ai ai = new monsterconf_ai();


    }

    public interface IConf
    {

    }

    public class BattleAttrs
    {
        public int max_hp { get; set; }
        public int constitution { get; set; }
        public int strength { get; set; }
        public int intelligence { get; set; }
        public int physics { get; set; }
        public int magic { get; set; }
        public int physics_def { get; set; }
        public int magic_def { get; set; }
        public int critical_damage { get; set; }
        public int critical_def { get; set; }
        public int ice_att { get; set; }
        public int fire_att { get; set; }
        public int thunder_att { get; set; }
        public int ice_def { get; set; }
        public int fire_def { get; set; }
        public int thunder_def { get; set; }
    }

    #region IMapUnit property class

    public class holding
    {
        public int sid { get; set; }
        public int tar_tp { get; set; }
        public Variant rpc { get; set; }
        public long end_tm { get; set; }
        public long start_tm { get; set; }
    }
    public class atking
    {
        public int tar_iid { get; set; }
        public long start_tm { get; set; }
        public long trace_tm_left { get; set; }

        public Variant ToVariant()
        {
            Variant v = new Variant();
            v["tar_iid"] = this.tar_iid;
            v["start_tm"] = this.start_tm;
            v["trace_tm_left"] = this.trace_tm_left;

            return v;
        }
    }

    public class casting
    {
        public int tar_tp { get; set; }

        public long end_tm { get; set; }

        public Variant rpc { get; set; }
    }

    public class moving
    {
        public long start_tm { get; set; }
        public List<Point2D> pts { get; set; }
        public double to_x { get; set; }
        public double to_y { get; set; }
        public double float_x { get; set; }
        public double float_y { get; set; }

        public double max_r { get; set; }

        public double stx { get; set; }
        public double sty { get; set; }

        public double ori_cos { get; set; }
        public double ori_sin { get; set; }

        public List<Point2D> ppts { get; set; }


    }

    public class jumping
    {
        public int dest_x { get; set; }
        public int dest_y { get; set; }

        public long start_tm { get; set; }
        public long during_tm { get; set; }
        public long end_tm { get; set; }
        public jump_conf jump { get; set; }
        public Variant rpc { get; set; }
        public int percent { get; set; }

        public int tar_iid { get; set; }
    }

    public class patroling
    {
        public moveto movto { get; set; }
        public Tracing tracing = null;

        public List<moveto> path { get; set; }

        public int idx { get; set; }
        public int cnt { get; set; }
        public int init_cnt { get; set; }
    }

    /// <summary>
    /// 移动，x y width height 共同确定了移动的终点，即终点是一个矩形区域
    /// 
    /// </summary>
    public class moveto
    {
        /// <summary>
        /// 移动的初始点，像素值
        /// </summary>
        public double trinitx { get; set; }
        /// <summary>
        /// 移动的初始点，像素值
        /// </summary>
        public double trinity { get; set; }

        public bool tracing { get; set; }

        /// <summary>
        /// 表示grid_x
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// 表示grid_y
        /// </summary>
        public int y { get; set; }

        public int width { get; set; }
        public int height { get; set; }

        /// <summary>
        /// 区域的宽度，像素值
        /// </summary>
        public int rngx { get; set; }

        /// <summary>
        /// 区域的高度，像素值
        /// </summary>
        public int rngy { get; set; }

        /// <summary>
        /// 在此点上的等待时间
        /// </summary>
        public long wait { get; set; }

        /// <summary>
        /// 在此点上停留的时间终点
        /// </summary>
        public long cont_tm { get; set; }
    }

    public class follow
    {
        public long start_tm { get; set; }
        public int tar_iid { get; set; }
        public int trace_tm_left { get; set; }
        public int frang { get; set; }
        public int trang { get; set; }
        public bool do_ai { get; set; }

        public Variant ToVariant()
        {
            Variant v = new Variant();
            v["tar_iid"] = this.tar_iid;
            v["start_tm"] = this.start_tm;
            v["trace_tm_left"] = this.trace_tm_left;
            v["frang"] = this.frang;
            v["trang"] = this.trang;
            v["do_ai"] = this.do_ai;

            return v;
        }
    }

    public class teleping
    {
        public long start_tm { get; set; }
        public long end_tm { get; set; }
        public teleport_conf telep { get; set; }
        public Variant rpc { get; set; }
        public int percent { get; set; }
        public int dest_x { get; set; }
        public int dest_y { get; set; }
    }


    #endregion

    public class NetMsgData
    {

    }

    public class SkillData
    {
        public int skid { get; set; }
        public int sklvl { get; set; }
    }

    public class gameWorld
    {

    }

    public class cast_skill_res
    {
        public game_err_code res { get; set; }
        public skill_conf skill_data { get; set; }
        public Variant rpc_data { get; set; }
        public skill_lv_conf skill_level_conf { get; set; }

        public cast_skill_res(game_err_code code = game_err_code.RES_OK)
        {
            res = code;
        }

        public int percent { get; set; }
    }


    #region enums
    //英雄品质
    enum hero_quality
    {
        YouLiang = 0,//良好
        JieChu = 1,//杰出
        WanMei = 2,//完美
        WuShuang = 3,//无双
    }

    //英雄性格
    enum hero_character
    {
        Zhong = 0,//忠
        Yong = 1,//勇
        Yi = 2,//义
    }


    public class game_const
    {
        /// <summary>
        /// 整型上限
        /// </summary>
        public const int max_int = 0x7fffffff; //
        /// <summary>
        /// 格子相素
        /// </summary>
        public const int max_gold = 0x7fffffff;   //
        /// <summary>
        /// 格子相素
        /// </summary>
        public const int map_grid_pixel = 32; //

        public const int vision_distance_grid = 10;
    };

    //连接状态
    enum bsc_join_state
    {
        BSCJS_NONE = 0,    //无
        BSCJS_CONNECT = 1,   //已进入bs outgame_service  
        BSCJS_LOGIN = 2,   //已进入bs outgame_service 并登入了  
        BSCJS_JOIN_WORLD = 3,    //已进入bs world_service 
        BSCJS_JOIN_LEVEL = 4, //已进入bs level_service
    }

    //执行状态
    enum bsc_do_state
    {
        BSCDO_NONE = 0,    //无
        BSCDO_LOGIN = 1,   // 
        BSCDO_CREAT_CHAR = 2, //创建bs角色   
        BSCDO_SELECT_CHAR = 3,//选择bs角色
        BSCDO_CHECK_IN = 4, //CHECK IN 中。。。。
        BSCDO_ENTER_BATTLE = 5, //进入战斗中。。。。
    }

    enum game_msg
    {
        new_game_ack = 1,    // new game result
        join_pgame_ack = 2,    // join persist game result
        join_game_ack = 3,    // join user game reult
        new_player_join = 4,    // a new player is join the game
        player_leave = 5,    // a player has leave the game

        chat_msg = 100,  // chat message
    };

    enum session_do_cmd
    {
        sdc_start = 1024,

        sdc_fcm_change = 1025, // fcm change
        sdc_add_itmcard = 1026, // 新增道具卡
        sdc_dragpl = 1027, // 被抓到指定地点
        sdc_set_fobtalk = 1028, // 角色被禁言
        sdc_set_seal = 1029, // 角色被封号
        sdc_add_yb = 1030, // 增加元宝
        sdc_add_bndyb = 1031, // 增加礼券
        sdc_add_gld = 1032, // 增加游戏币
        sdc_add_petpt = 1033, // 增加宠物天梯积分
        sdc_set_vip = 1034, // 设置vip
        sdc_add_ybpt = 1035, // 增加积分
        sdc_pvip_change = 1036, // 平台vip变化
        sdc_buy_itmex = 1036, // 通过平台币直接购买道具
        sdc_get_pos_info = 1037, // 获取当前位置信息
        sdc_fcm_earn_rate_change = 1038, // 角色反沉迷收益率变化
        sdc_add_pvip_pow = 1039, // 黄钻能量增加
        sdc_calc_cha_data = 1040,  //刷新当前属性
        sdc_add_buddy = 1041,  //新增好友
        sdc_delete_buddy = 1042,  //删除好友
        sdc_add_buddy_apply = 1043, //增加好友申请

        sdc_ach_family_level = 2000, //家族升级时，检查在线玩家的成就
        sdc_ach_family_join = 2001, //加入家族时，检查成就

        sdc_buddy_add = 2100,       //加好友时，检查成就
    }

    enum svr_task_cmd
    {
        stc_kick_idle_ply = 1,   // 踢出发呆用户
        stc_comb_data_base = 2,	 // 合服操作
        stc_http_timer_tsk = 3,   // http定时任务
        stc_robot_do = 4,   // 机器人操作
        stc_modrob_conf = 5,   // 调整机器人配置
        stc_world_army = 6,   // 黄金部队
        stc_verify_rnkact_tm = 10,  // 修改开服时间，合服时间等
        stc_verify_market_conf = 11,  // 修改市场配置
        stc_verify_itemdrop_conf = 12,  // 修改掉落配置
        stc_verify_monster_conf = 13,  // 修改怪物配置
        stc_pvip_power_act = 14,  // 修改黄钻能量活动开始结束时间
        stc_verify_server_prop = 15,  // 修改服务器属性
        stc_clan_lvl_chg = 16,  // 帮派等级变化
        stc_aprov_join_clan = 17,  // 同意他人加入帮派
        stc_rmv_achive = 18,  // 移除称号 
        stc_verify_items_conf = 19,  // 修改物品配置   
        stc_acti_time = 20,  // 修改活动时间 
    }

    enum robot_cmd
    {
        rc_quit = 1,    // 退出游戏
        rc_kick = 2,    // 踢出游戏    
    }

    enum rpc_cmd
    {
        join_pgame_ack = 1,
        new_player_join = 2,
        player_leave = 3,
    };

    //enum map_sprite_type
    //{
    //    MST_PLAYER = 1,
    //    MST_MONSTER = 2,
    //};

    public enum skill_type
    {
        ST_SELF = 0,
        ST_TARGET = 1,
        ST_GROUND = 2,
    };



    public enum pl_state_type
    {
        PST_CANT_MOVE = 1,
        PST_CANT_ATTACK = 2,
        PST_CANT_CAST_SKILL = 4,
        PST_CANT_USE_ITEM = 8,
        PST_MOVE_SPEED_MUL = 16,
        PST_ATK_SPEED_MUL = 32,
        PST_MOVE_SPEED_ADD = 64,
        PST_ATK_SPEED_ADD = 128,
        PST_HP_TIME_ADD = 256,
        PST_MP_TIME_ADD = 512,
        PST_SKILL_AVOID = 1024,
        PST_ATK_AVOID = 2048,
        PST_MP2HP = 4096,
        PST_CANT_CTMV = 8192,
        PST_DMGLNK_M = 16384,
        PST_DMGLNK_S = 32768,
        PST_COMA = 65536,
    };

    public enum atk_type
    {
        ATKTP_ATK = 1,    // 外功攻击
        ATKTP_MATK = 2,    // 内功攻击
    }

    enum skiltp
    {
        SKILTP_NONE = 0,    // 其他
        SKILTP_CARR_SKILL = 1,    // 门派技能
        SKILTP_LIFE_SKILL = 2,    // 生活技能
    }

    public enum pk_state_type
    {
        PKST_PEACE = 0,    // 和平
        PKST_PK_ALL = 1,    // PK 全部
        PKST_PK_TEAM = 2,    // PK 组队
        PKST_PK_CLAN = 3,    // PK 帮派
        PKST_MAX = 4,    // 
    }

    public enum rednm_type
    {
        RNT_NORMAL = 0,    // 普通
        RNT_RASCAL = 1,    // 无赖
        RNT_EVIL = 2,  //恶人
        RNT_DEVIL = 3, //魔头
        RNT_RIGHT = 4, //义士
        RNT_HERO = 5, //英雄
        RNT_SHERO = 6, //大侠
    }

    public enum map_pk_setting_type
    {
        MPST_PEACE = 0,    // 和平地图，不能pk
        MPST_PK = 1,    // PK地图，随意pk，无惩罚
        MPST_NORMAL = 2,    // 中立地图，开了pk状态可pk，视情况有无惩罚
    }

    enum level_multi_type
    {
        LMT_SINGLE = 1,    // 单人副本
        LMT_TEAM = 2,    // 组队副本
        LMT_CLAN = 3,    // 帮派副本（弃用）
        LMT_MULTI = 4,    // 多人副本
    }

    enum level_checkin_type
    {
        LCT_USER_CREATE = 1,    // 用户自主创建
        LCT_AUTO_CREATE = 2,    // 服务器创建
        LCT_AUTO_MATCH = 3,    // 自动匹配创建
    }

    enum level_pk_type
    {
        LPT_PVE = 1,    // PVE副本
        LPT_PVP = 2,    // PVP副本
    }

    public enum level_state_type
    {
        LST_NONE = 1,    // 
        LST_PROCEED = 2,    // 副本进行中
        LST_FINED = 3,    // 副本已结束，等待用户退出中
    }

    enum tm_acc_tp
    {
        TAT_MERI = 0,    // 经脉修炼加速
        TAT_SKIL = 1,    // 技能领悟加速
    }

    enum mail_flag_tp
    {
        MFT_READED = 0x01, // 是否已读
        MFT_LOCKED = 0x02, // 是否锁定
        MFT_ITEMEXIST = 0x04, // 是否有附件
    }

    enum server_const
    {
        GLOBAL_DATA_LOCK_TM = 10,   // 全局数据锁超时时间, 10秒钟
    }

    enum bcast_msg_tp // 系统公告类型
    {
        KILL_MON_GAIN_ITM = 1,    // 杀怪掉落道具
        EQP_FORGE_SUCC = 2,    // 成功锻造
        STN_MERGE_SUCC = 3,    // 成功合成宝石
        SKIL_SPTUP_MAX = 4,    // 技能强化到了最大值
        MERI_ACUP_ALL_ACTIVE = 5,    // 开通了整条经脉
        MERGE_EQP_SUCC = 6,    // 成功打造装备
        EQP_PERFECT_TRANS = 7,    // 完美萃取装备锻造等级
        LVL_PVP_COMBO_KP = 8,    // 副本连杀广播
        PZ_PVP_COMBO_KP = 9,    // 擂台连杀广播
        LVL_AWD_ITM = 10,   // 副本(含侠客行)通关奖励广播
        MAP_TRIGER_MSG = 11,   // 地图触发器触发消息
        LOT_GET_ITEM = 12,   // 抽奖获得道具
        HUNT_WH = 13,   // 猎魂获得武魂
        ARENA_CHLVL_FIN = 14,   // 竞技场冠军决赛结果 
        ACHIVE_PLY_ONLINE = 15,   // 牛b成就玩家上线了
        CHIEF_CARR_PLY = 16,   // 玩家在首席大弟子副本中胜出
        OPEN_PKG_GAIN_ITM = 17,   // 使用道具（开宝箱、礼包）获得道具
        DMIS_FIN = 18,   // 完成了很多每日必做任务
        ACP_MIS = 19,   // 接受了某个牛b任务 
        CLANTER_LVL_FIN = 20,   // 帮派领地争夺战结束
        MIS_KILL_AWD = 21,   // 可截任务被截（运镖被截）
        FETCH_FC_AWD = 22,   // 领取礼包广播
        MIS_COMPLETE = 23,   // 完成某特殊任务
        USE_BUYVIP_ITM = 24,   // 使用vip道具卡
        WORLD_BOSS_RESPAWN = 25,   // 世界boss刷新
        MERGE_ITM_SUCC = 26,   // 成功合成装备
        PET_QUAL_UP = 27,   // 宠物品阶提升广播
        PET_ATTPER_UP = 28,   // 宠物资质提升广播
        GET_LOTPT_ITM = 29,   // 领取修行祝福奖励
        EQP_UP_SUCC = 30,   // 橙装升级广播
        LVLMIS_FIN = 31,   // 侠客行副本通关
        BTL_FIN = 32,   // 排行奖励战场结束广播
        CARRLVL_UP = 33,   // 玩家转职广播
        REDNM_CHANGE = 34,   // 玩家红名广播
        WORLD_ARMY_RESPAWN = 35,   // 黄金部队刷新广播
        MARRY_SUCC = 36,   // 结缘成功广播
        MARRY_DIALOG = 37,   // 结缘互动广播
        WORLD_BOSS_KILLED = 38,   // 世界boss击杀广播
        BUY_MARKET_ITEM = 39,   // 购买商城物品
        OPEN_PKG_ITM = 40,   // 开宝箱、礼包道具广播
        BUY_LVL_BUFF = 41,   // 购买副本BUFF
        CLTER_COST_AWD = 42,   // 领取攻城战消耗奖励广播
        CLTER_CLAN_REQUEST = 43,   // 申请攻城战广播
        CLTER_CLAN_KILL = 44,   // 攻城战帮派击杀广播
        LVL_MON_RESPAWN = 45,   // 副本怪刷新广播
        LVL_MON_KILLED = 46,   // 副本怪击杀广播
        LVL_EQP_VERI_EXATT = 47,   // 鉴定卓越属性广播
        BUY_RANDSTORE_ITEM = 48,   // 购买随机商城道具广播
        CLIENT_BOARDCAST = 49,   // 客户端广播
        NOBILITY_BOARDCAST = 50,   // 爵位广播
        GOT_ACHIVE = 51,   // 获得称号广播
        GOT_AWDACT_AWD = 52,   // 成功获得领奖活动奖励广播
        NOBILITY_PLY_ONLINE = 53,   // 特定爵位玩家上线了
        DO_WORSHIP_BCAST = 54,   // 膜拜广播
        DO_FESTACT_BCAST = 55,   // 节日活动广播
        RESET_LVL = 56,   // 转生成功
        PLY_SEND_RED_PAPER = 57,   // 发放红包广播
        PLY_FETCH_RED_PAPER = 58,   // 领取红包广播
    }

    enum bstate_eff_type
    {
        BET_ADD_HP = 1,    // 定时回复生命祝福
        BET_ADD_MP = 2,    // 定时回复法力祝福
        BET_EXP_MUL = 3,    // 经验加倍祝福
        BET_RATE_MUL = 4,   // 掉率加倍祝福
    }

    enum gdctrl_type
    {
        GT_DPITM_CTRL = 1,    // 掉落数据控制器
        GT_MKTITM_CTRL = 2,    // 商城打折道具控制器
        GT_CLAN_LIST = 3,    // 帮派列表
        GT_GLOBAL_LOTLOGS = 4,    // 修行抽奖记录
        GT_ITMCARD_CONF = 5,    // 道具卡配置信息
        GT_BULLETIN_INFO = 6,    // 滚动公告消息信息
        GT_WRDBOSS_MGR = 7,    // 世界boss管理器

        GT_RNK_ACHIVES = 8,    // 排行榜称号管理器


        GT_RANK_PLY_LVL = 10,  // 角色等级排行板
        GT_RANK_WEALTH = 11,  // 角色财富排行板
        GT_RANK_MERI = 12,  // 经脉修为排行板
        GT_RANK_SKIL = 13,  // 武学修为排行板
        GT_RANK_PET = 14,  // 宠物排行板
        GT_RANK_WEAPON = 15,  // 武器排行板
        GT_RANK_EQUIP = 16,  // 防具排行板
        GT_RANK_DECORATION = 17,  // 饰品排行板
        GT_RANK_RIDE = 18,  // 坐骑排行板
        GT_RANK_FASHION = 19,  // 时装排行板
        GT_RANK_WPN_KCNT = 20,  // 利器（武器击杀）排行板
        GT_RANK_WNP_BCNT = 21,  // 神兵（击碎武器）排行板
        GT_RANK_KM_CNT = 22,  // 杀怪数排行板
        GT_RANK_PK_V = 23,  // 罪恶值排行板
        GT_RANK_LVL = 24,  // 副本排行
        GT_RANK_LVLMIS = 25,  // 侠客行排行
        GT_RANK_WH = 26,  // 武魂排行
        GT_RANK_ARENA_PT = 27,  // 竞技场积分排行
        GT_RANK_ARENA_TPT = 28,  // 竞技场总积分排行
        GT_RANK_ARENA_CHPL = 29,  // 竞技场历史冠军
        GT_RANK_COMBPT = 30,  // 总战斗力排行榜
        GT_RANK_HEXPT = 31,  // 荣誉排行榜
        GT_RANK_MARRY = 32, //结缘排行榜
        GT_RANK_NOBILITY = 33, //爵位排行榜
        GT_RANK_ACHIEVE = 34, //成就排行榜
        GT_RANK_DEFEND = 35, //守护排行榜
        GT_RANK_END = 36,

        GT_MARRYSEEK_MGR = 50,    // 征婚管理器



        GT_BS_RANK_START = 60, //用于whsever缓存 跨服战排行榜信息
        GT_BS_RANK_OFF = 50, //用于把rank tp 偏移到GT_BS_RANK_START
        GT_BS_RANK_END = 87,
        GT_ROOM_MGR = 90, //房间管理器

        GT_BLOCK_BEGIN = 100,
        GT_BLOCK_MSG = 100,  // 消息开关
        GT_BLOCK_LVL = 101,  // 副本开关
        GT_BLOCK_MIS = 102,  // 任务开关
        GT_BLOCK_END = 103,

        GT_BS_PLY_DATA = 150, //用于whsever缓存 跨服战角色信息

        GT_RANK = 200,//排行榜
        GT_TOURNAMENT = 210, //比武场
    }

    enum itmrnk_tp
    {
        IT_EQP = 1,    // 防具
        IT_WPN = 2,    // 武器
        IT_RIDE = 3,    // 坐骑
        IT_FASHION = 4,    // 时装
        IT_DECORA = 5,    // 饰品
        IT_DEFEND = 6,    // 守护

        ITMRNK_TP_END = 100,
    }

    enum clan_c_tp
    {
        CCT_NONE = 0,    // 帮众
        CCT_VP = 1,    // 副帮主
        CCT_P = 2,    // 帮主
    }

    enum clan_log_tp
    {
        CLANLT_CLANC_CHANGE = 1,    // 帮派成员职务变动
        CLANLT_LVL_UP = 2,    // 帮派升级
        CLANTECH_1_UP = 3,    // 升级建筑类帮派科技
        CLANTECH_2_UP = 4,    // 升级加点类帮派科技
        CLANTECH_2_PROC = 5,    // 帮派科技加点
        CLAN_CHL = 6,    // 帮派宣战
        CLAN_CONQ_WAR = 7,    // 帮派从属战斗结果
        CLAN_CONQ_CHANGE = 8,    // 帮派从属关系变化
        CLAN_TER_WAR = 9,    // 帮派领地争夺战结果
    }

    enum clan_conq_chang_tp
    {
        CCCT_DISMIS = 0,    // 取消了下属关系
        CCCT_LOSE = 1,    // 失去了下属
        CCCT_CONQ = 2,    // 征服了对方，成为了上级
        CCCT_BECONQ = 3,    // 被对方征服，成为了下属
        CCCT_BEGRAB = 4,    // 下属被其他帮派抢走
    }

    enum itm_card_functp
    {
        ICT_NONE = 0,
        ICT_NEW_GUY = 1,    // 新手礼包
        ICT_FIRST_CHARGE = 2,    // 首次充值
        ICT_DALY_CARD = 3,    // 每日可领取的卡片
        ICT_COMPENSTION = 4,    //补偿卡片

        ICT_NOTIFY_CLIENT = 5,    // 需要通知客户端已领取的道具卡类型阀值


        ICT_NORMAL_CARD = 100,  // 普通卡阀值
    }
    enum itm_card_state
    {
        ICST_NONE = 0,
        ICST_NEWADD = 1,    // 新发放
        ICST_FETCHED = 2,    // 已领取
    }

    enum rnkact_type
    {
        RAT_RESERVE = 10,
        RAT_LVLUP = 11,   // 升级排行活动
        RAT_ADDGOLD = 12,   // 获得游戏币排行活动
        RAT_MERIV = 13,   // 经脉修炼排行活动
        RAT_LVLMIS = 14,   // 侠客行排行活动
        RAT_PVPHEXP = 15,   // 荣誉值排行活动
        RAT_BRKWPN = 16,   // 击碎武器排行活动
        RAT_FLVL = 17,   // 锻造排行活动
        RAT_LVLFIN = 18,   // 副本通关排行活动
        RAT_SKILSPTUP = 19,   // 技能修练排行活动
        RAT_SKILV = 20,   // 技能修为值排行活动
        RAT_WHEXP = 21,   // 武魂吞噬排行活动

        RAT_LVLPT_TYPE = 50,//50以后为 副本积分排行活动  
    }

    enum clan_rnkact_type
    {
        CLAN_RAT_RANK = 1, //帮派排行活动
        CLAN_RAT_HOLDCITY = 2,//功城战排行活动
    }

    enum user_vipact
    {
        UVAT_LOT = 1,    // vip抽奖
    }

    enum item_change_msg_flag
    {
        ICMF_NO_NOTIFY = 1,    // 无道具变化内容道具变化消息，需客户端重新获取相应背包、仓库等道具列表，已弃用
        ICMF_COL_ITEM = 2,    // 采集获得道具
        ICMF_DECOMP_EQP = 3,    // 分解装备消耗道具
        ICMF_DELAY_EXPIRE = 4,    // 延长装备时效消耗道具
        ICMF_LVL_PRIZE = 5,    // 领取副本通关奖励获得道具
        ICMF_OPEN_PKG = 6,    // 开启礼包获得道具
        ICMF_FGUESS_RETURN = 7,    // 划拳失败后返回获得的游戏币
        ICMF_LVL_TM_COST = 8,    // 计时副本消耗
    }

    enum db_glbdata_datatype
    {
        DBGDT_PET_FIGHT_LADDER = 1,    // 人宠合体回合制pk天梯
        DBGDT_PET_FIGHT_REC = 2,    // 最近宠物pk战斗记录信息
        DBGDT_CHLVL_CRT_MGR = 3,    // 竞技场决赛创建管理器
        DBGDT_CARR_CHIEF = 4,    // 首席大弟子形象管理器
        DBGDT_CLAN_TER = 5,    // 帮派领地管理器
        DBGDT_MKT_INFO = 6,    // 商城打折信息
        DBGDT_LVLMIS_RNK_INFO = 7,    // 侠客行每关霸主、首破、最近通关玩家信息
        DBGDT_CLANCHALENGE_INFO = 8,    // 帮派宣战信息
        DBGDT_GLBMAIL_INFO = 9,    // 全服公告邮件
        DBGDT_W_HEXP_RNK_INFO = 10,   // 每周荣誉排行榜管理器
        DBGDT_BLT_RNK_INFO = 11,   // 战场排行榜奖励数据管理器
        DBGDT_RANDSTORE_BUY_INFO = 12,   // 神秘商店道具购买记录
        DBGDT_FGUESS_BUY_INFO = 13,   // 猜拳道具购买记录
        DBGDT_AWDACT_INFO = 14,   // 领奖活动信息记录
        DBGDT_CLANACT_INFO = 15,   // 帮派活动信息记录
        DBGDT_DAY_ONLINE_INFO = 16,   //每日在线信息 记录
        DBGDT_YBRACT_INFO = 17,   //充值活动信息 记录
        DBGDT_WORSHIP_INFO = 18,   //膜拜信息 记录
        DBGDT_ACTONLINE_INFO = 19,   //后台配置在线活动管理器

        DBGDT_CONSUME_RACT_INFO = 21,   //消费活动信息 记录
        DBGDT_RED_PAPER = 22,   //红包信息 记录    
        DBGDT_REDPAPER_RNK_INFO = 23,   //红包行榜数据管理器

        DBGDT_INVEST_LOG_INFO = 25, //投资 记录
    }

    enum clan_teritory_type
    {
        CTT_DEF_TERRITORY = 1,    // 默认帮派自有领地
        CTT_WAR_TERRITORY = 2,    // 帮派争夺领地
    }

    enum lvl_kumite_state
    {
        LKS_WAIT = 1,    // 等待挑战中
        LKS_FIGHTING = 2,    // 挑战中
        LKS_DIED = 3,    // 挑战失败了
    }

    enum resist_type
    {
        RT_ICE = 0,
        RT_POISON = 1,
        RT_LIGHTNING = 2,
        RT_FIRE = 3,
        RT_EARTH = 4,
        RT_WIND = 5,
        RT_WATER = 6,
    }

    enum item_att_flag_type
    {
        IAFT_LUCK = 1,    // 幸运装备
        IAFT_SKIL = 2,    // 带装备技能
        IAFT_ATT380 = 4,    // 380级激活属性
    }

    enum equip_pos_type
    {
        EPT_WEAPON = 6,    //武器
        EPT_EQP_POS_RIDE = 10,   //坐骑
        EQP_POS_WING = 11,    //翅膀
    }

    enum cardgame_card_type
    {
        CCT_LVL = 1,    //副本
        CCT_MARRY = 2,    // 结缘
    }

    enum client_data_type
    {
        CDT_BOARDCAST = 1,    //广播
    }

    enum activity_sign
    {
        TCYB_LOTT = 5,        //充值抽奖活动
        COST_YB_LOTT = 6,        //消费抽奖活动
        FETCH_RED_PAPER = 7,        //抢红包活动
    }
    #endregion








    public class IBaseUnit_State
    {
        public int id { get; set; }
        public int par { get; set; }
        public long start_tm { get; set; }
        public long end_tm { get; set; }
    }

    public class g_world_lvl_info
    {
        public int tm = 0;
        public int lvl = 0;
    }

    //public class player_skill_data
    //{
    //    public int skill_id { get; set; }
    //    public int skill_level { get; set; }
    //}

}
