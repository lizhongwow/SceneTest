using SceneTest;
using SceneTestLib.Confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneTestLib;

namespace SceneTest
{
    public class pet_mon
    {
        public int mid { get; set; }
        public long fintm { get; set; }
    }


    public class pinfo : IMapUnit
    {
        public Dictionary<int, int> addResist { get; set; }

        public int atkcdtm { get; set; }

        public atking atking { get; set; }

        public int atkrange { get; set; }

        public atk_type atktp { get; set; }

        public int atk_dmg_mul { get; set; }

        public int atk_max { get; set; }

        public int atk_min { get; set; }

        public int atk_rate { get; set; }

        public List<BState> bstates { get; set; }

        public casting casting { get; set; }

        public bool cfmv { get; set; }

        public int cid { get; set; }

        public int clanid { get; set; }

        public int criatk { get; set; }

        public int criatk_debuffs { get; set; }

        public int cridmg { get; set; }

        public int cur_kp { get; set; }

        public int def { get; set; }

        public IMapUnit DefaultMapUnit { get; set; }

        public Dictionary<int, long> defend_tm_map { get; set; }

        public int def_red { get; set; }

        public int dmg_red { get; set; }

        public int dobrate { get; set; }

        public int dp { get; set; }

        public int exatk { get; set; }

        public int exatk_debuffs { get; set; }

        public int exdmg { get; set; }

        public int exp { get; set; }

        public int exper_add { get; set; }

        public follow follow { get; set; }

        public int full_hp_rate { get; set; }

        public int full_mp_rate { get; set; }

        public bool ghost { get; set; }

        public int grid_x { get; set; }

        public int grid_y { get; set; }

        public holding holding { get; set; }

        public int hp { get; set; }

        public int hpsuck { get; set; }

        public int hpsuck_dmgmul { get; set; }

        public int igatk_dmg_mul { get; set; }

        public int igdef_rate { get; set; }

        public int igdef_rate_debuffs { get; set; }

        public int igdmg_red { get; set; }

        public int iid { get; set; }

        public int invisible { get; set; }

        public bool in_pczone { get; set; }

        public bool isdie { get; set; }

        public jumping jumping { get; set; }

        public int kp { get; set; }

        public long last_atk_tm { get; set; }

        public List<Point2D> last_mvpts { get; set; }

        public int level { get; set; }

        public int line { get; set; }

        public int llid { get; set; }

        public int lmpid { get; set; }

        public int lvlsideid { get; set; }

        public double lx { get; set; }

        public double ly { get; set; }

        public int map_id { get; set; }

        public int matk_max { get; set; }

        public int matk_min { get; set; }

        public int max_dp { get; set; }

        public int max_hp { get; set; }

        public int max_mp { get; set; }

        public int mid { get; set; }

        public int miss_rate { get; set; }

        public moving moving { get; set; }

        public int mp { get; set; }

        public int observer { get; set; }

        public int org_init_x { get; set; }

        public int org_init_y { get; set; }

        public int owner_cid { get; set; }

        public int pkatk { get; set; }

        public int pkatkrate { get; set; }

        public int pkatk_dmg_mul { get; set; }

        public int pkdef { get; set; }

        public int pkdmg_red { get; set; }

        public int pkigdp_rate { get; set; }

        public int pkigdp_rate_debuffs { get; set; }

        public int pkmatk { get; set; }

        public int pkmisrate { get; set; }

        public pk_state_type pk_state { get; set; }

        public int pk_v { get; set; }

        public rednm_type rednm { get; set; }

        public int rev_atk { get; set; }

        public int rev_dmg_mul { get; set; }

        public int sid { get; set; }

        public int size { get; set; }

        public Dictionary<int, SkillData> skill { get; set; }

        public Dictionary<int, int> skill_cd { get; set; }

        public long skill_gen_cd { get; set; }

        public long skill_gen_cdst { get; set; }

        public int speed { get; set; }

        public UnitState states { get; set; }

        public int teamid { get; set; }

        public teleping teleping { get; set; }

        public double x { get; set; }

        public double y { get; set; }

        public long pk_v_revtm { get; set; }

        public Dictionary<int, SkillData> skills { get; set; }
    }


    public class Player : IBaseUnit
    {
        public bool kp_log = true; //保存角色记录
        public string session_ip = "";

        public pinfo pinfo = null;

        public pet_mon pet_mon = new pet_mon();

        //pdb = null;
        //cha_db = null;
        //item_db = null;
        //wpnrnk_db = null;
        //user_info = null;
        //cha_info = null;
        //blacklist_db = null;
        //foes_db = null;
        //buddy_db = null;
        //mail_db = null;
        //petpk_db = null;
        //petrnk_db = null;
        //whrnk_db = null;
        //lotlog_db = null;
        //lvlmisrnk_db = null;
        //rnkact_db = null;
        //last_rnkact_data = null;
        public long db_last_update_tm = 0;
        public long last_notify_fcm_tm = 0; // 反沉迷消息提示时间
        public double fcm_earn_rate = 1.0;      // 反沉迷收益率
        public bool is_in_lvl = false;      // 是否在副本中
        public Level in_lvl = null;          // 副本对象
        public bool dying = false;
        public long shout_tm = 0;           // 上次世界聊天时间
        public long sort_pkg_tm = 0;        // 上次整理仓库的时间

        //last_act_tm = 0;        // 上次行动的时间

        public long last_move_tm = 0; //上次移动的时间

        public long check_tmmis_tm = 0;     // 检查限时任务时间

        public long check_ol_tm = 0;        // 检查在线时间更新

        public long last_colarea_tm = 0;    // 最后一次区域采集时间

        public long last_pet_ladder_tm = 0; // 最后一次取宠物天梯时间

        public long last_query_auc_tm = 0;  // 最后一次查询挂售道具列表的时间

        public long last_query_marry_seek_tm = 0; // 最后一次查询征婚表的时间

        public long last_query_bs_ply_info = 0; // 最后一次查询跨服战的时间

        public long next_change_line_tm = 0; //下次可换线时间

        public long last_cal_log_day_tm = 0;    //记录上次玩家是否进入第二天


        //public bcast_achive_ary = null; // 广播上线的角色成就id数组

        //itm_cards = null;       // 道具卡、礼包

        public bool wait_join_map = false;

        public grid_map gmap = null;
        public long respawn_tm = 0;
        public long allow_respawn_tm = 0;   // 死亡后复活等待时间
        public long last_trace_target_tm = 0;
        public bool bwantchangmap = false;
        //acupup_conf = null;
        public bool brejointeam = false;

        public bool ignore_team = false;    // 强制pk队友
        public bool ignore_clan = false;    // 强制pk帮派成员

        public bool kp_asist_rec = false;   // 是否需要记录助攻
        public long lastdmgtm = 0;          // 最近被攻击时间



        public long allow_respawn_tm_s = 0; // 死亡后需等待复活的时间，原地复活忽略该时间限制，单位秒

        public long leave_vip_line_tm_s = 0; // 离开vip线的时间

        public IBaseUnit petmon_inst = null; // 战斗宠物实例


        public void modify_hp(int hp_add)
        {
            pinfo.hp += hp_add;
            if (pinfo.hp > pinfo.max_hp) pinfo.hp = pinfo.max_hp;

            // broadcast hpchange msg
            //this.broad_cast_zone_msg_and_self(26, { iid = pinfo.iid, hpchange ={ hpadd = hp_add, die = false} });
        }
        public int lotlog_sort_func(int cid_a, int cid_b)
        {
            if (cid_a > cid_b)
            {
                return -1;
            }
            else if (cid_a < cid_b)
            {
                return 1;
            }

            return 0;
        }

        public static g_world_lvl_info g_world_lvl_info = new g_world_lvl_info();


        public Player()
        {
            pinfo = new pinfo();
            db_last_update_tm = 0;
            gmap = null;
            respawn_tm = 0;
            last_trace_target_tm = 0;
            bwantchangmap = false;
            //acupup_conf = null;
            leave_vip_line_tm_s = 0;
            //last_act_tm = sys.time();
            last_move_tm = 0;
            check_ol_tm = DateTime.Now.ToBinary();
        }

        public void release_petmon(bool dead = false, bool clearinfo = true)
        {
            //if (clearinfo)
            //{
            //    this.pinfo.pet_mon.mid = 0;
            //    this.pinfo.pet_mon.fintm = 0;
            //}

            if (petmon_inst == null)
            {
                return;
            }

            if (!dead)
            {
                petmon_inst.die(null, false);
            }

            petmon_inst.get_pack_data().owner_cid = 0;
            petmon_inst.owner_ply = null;
            this.gmap.release_pet_mon(petmon_inst);
            petmon_inst = null;
        }



        public void call_petmon(int mid, long fintm)
        {
            // 创建出战战斗宠物

            if (this.gmap == null)
            {
                return;
            }

            //相同 战宠   已招则取消召唤
            if (petmon_inst != null && petmon_inst.get_pack_data().mid == mid)
            {
                release_petmon();
                return;
            }

            release_petmon(false, false);

            map_mon_conf monconf = new map_mon_conf() { mid = mid, x = 0, y = 0, r_x = 8, r_y = 8, spwan_time = 0 };
            petmon_inst = this.gmap.new_pet_mon(this, monconf);
            if (null == petmon_inst)
            {
                return;
            }

            this.pet_mon.mid = mid;
            this.pet_mon.fintm = fintm;

            IMapUnit mondata = petmon_inst.get_pack_data();

            mondata.follow = new follow()
            {
                start_tm = Utility.time(),
                tar_iid = pinfo.iid,
                trace_tm_left = 0,
                frang = 78400,
                trang = 8100,
                do_ai = true
            };//280*280
        }



        public void on_make_direct_dmg()
        {
            if (pinfo.full_hp_rate > 0 && pinfo.hp < pinfo.max_hp)
            {
                int judg = Utility.random(0, 1000);
                //sys.trace( sys.SLT_DETAIL, "pinfo.full_hp_rate =" + pinfo.full_hp_rate + "  judg=" + judg + "\n" );
                if (judg < pinfo.full_hp_rate)
                {
                    int hpadd = pinfo.max_hp - pinfo.hp;
                    pinfo.hp = pinfo.max_hp;
                    //this.broad_cast_zone_msg_and_self(26, { iid = pinfo.iid, hpchange ={ hpadd = hpadd, die = (pinfo.hp <= 0)} } );
                }
            }
            if (pinfo.full_mp_rate > 0 && pinfo.mp < pinfo.max_mp)
            {
                int judg = Utility.random(0, 1000);
                //sys.trace( sys.SLT_DETAIL, "pinfo.full_mp_rate =" + pinfo.full_mp_rate + "  judg=" + judg + "\n" );
                if (judg < pinfo.full_mp_rate)
                {
                    int mpadd = pinfo.max_mp - pinfo.mp;
                    pinfo.mp = pinfo.max_mp;
                    //::send_rpc(pinfo.sid, 32, { mpadd = mpadd} );
                }
            }
        }


        public void set_pk_state(pk_state_type pk_state)
        {
            //if (this.pinfo.level < game_conf.pk_safe_lvl) return;

            if (pk_state >= pk_state_type.PKST_MAX) return;

            this.pinfo.pk_state = pk_state;

            if (this.pinfo.pk_state == pk_state_type.PKST_PEACE)
            {
                //this.pinfo.pk_tar_iid = 0;  
                if (pinfo.atking != null)
                {//停止 自动攻击是玩家
                    IBaseUnit target = this.gmap.get_sprite_by_iid(this.pinfo.atking.tar_iid);
                    if (target != null)
                    {
                        if (target.get_sprite_type() == map_sprite_type.MstPlayer)
                            this.cancel_atk();
                    }
                    else
                        this.cancel_atk();

                }
            }
        }

        public void add_pk_v(int add)
        {
            int old_pk_v = pinfo.pk_v;
            int new_pk_v = old_pk_v + add;
            //if (curr_pklvl_conf.lvl == rednm_type.RNT_DEVIL)
            //{//pk_v最大值 为魔头最大值 
            //    if (new_pk_v > curr_pklvl_conf.max)
            //    {
            //        pinfo.pk_v = curr_pklvl_conf.max;
            //    }
            //    else
            //    {
            //        pinfo.pk_v = new_pk_v;
            //    }
            //    // 广播红名等级变化消息
            //    if (old_pk_v != pinfo.pk_v)
            //    {
            //        this.broad_cast_zone_msg_and_self(3, pinfo);
            //    }
            //    return;
            //}

            pinfo.pk_v = new_pk_v;
            //sys.trace( sys.SLT_ERR, "pinfo.pk_v =  " + pinfo.pk_v + "\n");
            //if (pinfo.rednm == rednm_type.RNT_NORMAL)
            //{
            //    if (pinfo.pk_v > 0)
            //    {
            //        pinfo.pk_v_revtm = sys.time();
            //    }
            //}
            //if (new_pk_v > curr_pklvl_conf.max)
            //{//调整红名等级
            //    curr_pklvl_conf = get_general_pklvl_conf_by_pkv(new_pk_v);
            //    if (curr_pklvl_conf)
            //    {//已经到了最大等级
            //        pinfo.rednm = curr_pklvl_conf.lvl;
            //        //sys.trace( sys.SLT_ERR, "pinfo.rednm =  " + pinfo.rednm + "\n");           

            //        if (pinfo.rednm == rednm_type.RNT_DEVIL)
            //        {//大侠需要广播                                          
            //            _broad_cast_sys_msg({ cid = pinfo.cid, name = pinfo.name, tp = bcast_msg_tp.REDNM_CHANGE, par = pinfo.rednm });
            //        }
            //    }
            //    else
            //    {
            //        sys.trace(sys.SLT_ERR, "add_pk_v(), curr_pklvl_conf is null  pk_v = " + pinfo.pk_v + "\n");
            //    }
            //}
            //// 广播红名等级变化消息
            //this.broad_cast_zone_msg_and_self(3, pinfo);
        }

        //是否可以为负值be_neg   可以使用道具降低PK值
        public void sub_pk_v(int sub, bool be_neg = false)
        {
            //if (!curr_pklvl_conf) return;

            int old_pk_v = pinfo.pk_v;
            int new_pk_v = old_pk_v - sub;
            //if (curr_pklvl_conf.lvl == rednm_type.RNT_SHERO)
            //{//pk_v最小值 为大侠最小值 
            //    if (new_pk_v < curr_pklvl_conf.min)
            //    {
            //        pinfo.pk_v = curr_pklvl_conf.min;
            //    }
            //    else
            //    {
            //        pinfo.pk_v = new_pk_v;
            //    }

            //    // 广播红名等级变化消息
            //    if (old_pk_v != pinfo.pk_v)
            //    {
            //        this.broad_cast_zone_msg_and_self(3, pinfo);
            //    }
            //    return;
            //}

            //sys.trace( sys.SLT_ERR, "pinfo.pk_v =  " + pinfo.pk_v + "\n");
            if (new_pk_v < 0 && !be_neg) new_pk_v = 0;

            pinfo.pk_v = new_pk_v;

            //if (new_pk_v < curr_pklvl_conf.min)
            //{//调整红名等级
            //    curr_pklvl_conf = get_general_pklvl_conf_by_pkv(new_pk_v);
            //    if (curr_pklvl_conf)
            //    {//已经到了最小等级
            //        pinfo.rednm = curr_pklvl_conf.lvl;
            //        //sys.trace( sys.SLT_ERR, "pinfo.rednm =  " + pinfo.rednm + "\n");      

            //        if (pinfo.rednm == rednm_type.RNT_SHERO)
            //        {//大侠需要广播                                          
            //            _broad_cast_sys_msg({ cid = pinfo.cid, name = pinfo.name, tp = bcast_msg_tp.REDNM_CHANGE, par = pinfo.rednm });
            //        }
            //    }
            //    else
            //    {
            //        sys.trace(sys.SLT_ERR, "sub_pk_v(), curr_pklvl_conf is null  pk_v = " + pinfo.pk_v + "\n");
            //    }
            //}
            //// 广播红名等级变化消息
            //this.broad_cast_zone_msg_and_self(3, pinfo);
        }

        //----------------------------------------------------------------------------
        public void vip_lvl_changed(bool re_calc = true)
        {
            //this._calc_vip_data();

            //local old_vip_attpt = pinfo.vip_attpt;
            ////sys.trace( sys.SLT_DETAIL, "pinfo.vip =" + pinfo.vip + "  old_vip_attpt=" + pinfo.vip_attpt + "\n" );
            //this._re_calc_vip_attpt();
            ////sys.trace( sys.SLT_DETAIL, "pinfo.vip_attpt" + pinfo.vip_attpt + "\n" );
            //local change_attpt = pinfo.vip_attpt - old_vip_attpt;
            //if (change_attpt != 0)
            //{
            //    if (change_attpt > 0)
            //    {
            //        pinfo.att_pt += change_attpt;
            //    }
            //    else if (change_attpt < 0)
            //    {
            //        if (pinfo.att_pt + change_attpt > 0)
            //        {
            //            pinfo.att_pt += change_attpt;
            //        }
            //        else
            //        {//洗点
            //            pinfo.att_pt = pinfo.att_pt + pinfo.strpt + pinfo.conpt + pinfo.intept + pinfo.agipt + pinfo.wispt + change_attpt;
            //            if (pinfo.att_pt < 0)
            //            {
            //                sys.trace(sys.SLT_ERR, "vip_lvl_changed  pinfo.cid = " + pinfo.cid + "pinfo.level = " + pinfo.level + "pinfo.resetlvl = " + pinfo.resetlvl + "\n");
            //                sys.trace(sys.SLT_ERR, "vip_lvl_changed  old_vip_attpt = " + old_vip_attpt + " new_vip_attpt = " + pinfo.vip_attpt + "\n");
            //                pinfo.att_pt = 0;
            //            }

            //            pinfo.strpt = 0;
            //            pinfo.conpt = 0;
            //            pinfo.intept = 0;
            //            pinfo.agipt = 0;
            //            pinfo.wispt = 0;
            //        }
            //    }

            //    if (re_calc)
            //    {
            //    // send detail_info_change msg
            //    ::send_rpc(pinfo.sid, 40, { pinfo ={ att_pt = pinfo.att_pt} });
            //    }
            //}

            //if (re_calc)
            //{
            //    this.re_calc_cha_data();
            //}
        }


        public void broad_cast_zone_msg(int cmd, Variant data)
        {
            // 向在区域中、且能看到自己的用户角色广播消息
            if (pinfo.invisible > 0)
            {
                var obplys = this.get_observer_plys();
                //return svr.mul_snd_rpc(obplys.sids, obplys.count, cmd, data);
            }
            else
            {
                //return svr.mul_snd_rpc(this.get_inz_ply_sids(), this.get_inz_ply_cnt(), cmd, data);
            }
        }

        public void broad_cast_zone_msg_and_self(int cmd, Variant data)
        {
            //::send_rpc(pinfo.sid, cmd, data);

            broad_cast_zone_msg(cmd, data);
        }

        public void broad_cast_non_observer_msg(int cmd, Variant data)
        {
            if (pinfo.invisible > 0)
            {
                // 向在区域中、但是看不到自己的用户角色广播消息
                var nobplys = this.get_non_observer_plys();
                //sys.dumpobj(nobplys);
                //svr.mul_snd_rpc(nobplys.sids, nobplys.count, cmd, data);
            }
        }

        public void broad_cast_zone_msg_igview(int cmd, Variant data)
        {
            // 向在区域中所有用户(包含不能看到自己的用户)角色广播rpc消息
            //return svr.mul_snd_rpc(this.get_inz_ply_sids(), this.get_inz_ply_cnt(), cmd, data);
        }

        // 向在区域中所有用户(包含不能看到自己的用户)广播type package消息
        public void broad_cast_zone_tpkg_igview(int cmd, Variant data)
        {
            //return svr.mul_snd_tpkg(this.get_inz_ply_sids(), this.get_inz_ply_cnt(), cmd, data);
        }
        public void broad_cast_zone_tpkg_igview_self(int cmd, Variant data)
        {
            //svr.send_type_pkg(pinfo.sid, cmd, data);
            //return svr.mul_snd_tpkg(this.get_inz_ply_sids(), this.get_inz_ply_cnt(), cmd, data);
        }


        public void visible_change(int invisible)
        {
            if (invisible == pinfo.invisible)
            {
                return;
            }

            IMapUnit pl = pinfo;
            if (invisible > pl.invisible)
            {
                // broad sprite_invisible msg
                //this.broad_cast_zone_msg_and_self(30, { iid = pl.iid,invisible = invisible});

                // invisible change
                pl.invisible = invisible;
                this.set_invisible(pl.invisible);

                // broadcast leave zone msg
                //this.broad_cast_non_observer_msg(56, { iidary =[pl.iid]});
            }
            else
            {
                // broadcast enter zone msg
                //this.broad_cast_non_observer_msg(54, { pary =[pl]});

                // invisible change
                pl.invisible = invisible;
                this.set_invisible(pl.invisible);

                // broad sprite_invisible msg
                //this.broad_cast_zone_msg_and_self(30, { iid = pl.iid,invisible = invisible});
            }
        }

        public class get_visible_change_sprite_res
        {
            public List<IBaseUnit> plys = new List<IBaseUnit>();
            public List<IBaseUnit> mons = new List<IBaseUnit>();
        }

        public get_visible_change_sprite_res get_visible_change_sprite(int observer)
        {
            return new get_visible_change_sprite_res();
        }

        public void observer_change(int observer)
        {
            if (observer == pinfo.observer)
            {
                return;
            }

            IMapUnit pl = pinfo;
            var chsprs = this.get_visible_change_sprite(observer);

            if (observer > pl.observer)
            {
                //local vbc_zone_plys = [];
                //local vbc_zone_mons = [];

                //foreach (var val in chsprs.plys)
                //{
                //    vbc_zone_plys.push(val.pinfo);
                //}
                //foreach (var val in chsprs.mons)
                //{
                //    vbc_zone_mons.push(val.mondata);
                //}

                //if (vbc_zone_plys.len() > 0)
                //{
                //// send player enter zone msg
                //::send_rpc(pinfo.sid, 54, { pary = vbc_zone_plys});
                //}

                //if (vbc_zone_mons.len() > 0)
                //{
                //// send monster enter zone msg
                //::send_rpc(pinfo.sid, 55, { monsters = vbc_zone_mons});
                //}
            }
            else
            {
                //local vbc_sprite_iids = [];
                //foreach (val in chsprs.plys)
                //{
                //    vbc_sprite_iids.push(val.pinfo.iid);
                //}
                //foreach (val in chsprs.mons)
                //{
                //    vbc_sprite_iids.push(val.mondata.iid);
                //}

                //if (vbc_sprite_iids.len() > 0)
                //{
                //// send leave zone msg
                //::send_rpc(pinfo.sid, 56, { iidary = vbc_sprite_iids});
                //}
            }

            //sys.trace(sys.SLT_DETAIL, "sid:["+pinfo.sid+"] observer:["+observer+"]\n");

            pl.observer = observer;
            this.set_observer(observer);
        }

        public int get_iid()
        {
            return pinfo.iid;
        }
        public map_sprite_type get_sprite_type()
        {
            return map_sprite_type.MstPlayer;
        }
        public IMapUnit get_pack_data()
        {
            return pinfo;
        }
        //public void get_pk_tar_iid()
        //{
        //    return pinfo.pk_tar_iid;
        //}
        public int get_atk_cd()
        {
            int cd_tm = pinfo.atkcdtm;
            if (pinfo.states != null)
            {
                cd_tm = (int)(cd_tm / pinfo.states.atk_spd_mul);
            }
            return cd_tm;
        }
        public void cancel_atk()
        {
            pinfo.atking = null;
        }

        public void _trig_dying_act(long cur_clock_tm)
        {
            if (gmap != null)
            {
                this.gmap.on_ply_dying(this, cur_clock_tm);
            }
        }

        public void die(IBaseUnit killer)
        {
            if (this.dying)
            {
                return;
            }
            this.dying = true;

            long cur_clock_tm = DateTime.Now.ToBinary();
            // 尝试触发死亡ai动作
            this._trig_dying_act(cur_clock_tm);

            // TO DO : player die

            // 中断打坐
            //this.try_stop_recover();

            this.respawn_tm = cur_clock_tm + 30000; // 30 秒后安全复活
            this.pinfo.isdie = true;
            pinfo.atking = null;
            pinfo.moving = null;

            if (pinfo.states != null)
            {
                bool _remark_pl_state = false;
                if (this.pinfo.states.state_par.Count > 0)
                {
                    for (var idx = 0; idx < this.pinfo.states.state_par.Count; ++idx)
                    {
                        var val = this.pinfo.states.state_par[idx];
                        if (val.desc.die_keep == 1)
                            continue;

                        this.pinfo.states.state_par.RemoveAt(idx);
                        --idx;
                        _remark_pl_state = true;
                    }
                }

                //pinfo.states.state = 0;
                //pinfo.states.mov_spd_mul = 1.0;
                //pinfo.states.atk_spd_mul = 1.0;
                //pinfo.states.mp2hp = 0;
                //pinfo.states.dmglnkper = 0;
                //re_calc_cha_data();
                if (_remark_pl_state)
                    Skill._remark_pl_state(this, pinfo);
            }

            if (pinfo.jumping != null)
            {
                // 跳跃中死亡，落在目标点
                pinfo.x = pinfo.jumping.dest_x;
                pinfo.y = pinfo.jumping.dest_y;

                if (!this.is_in_lvl)
                {
                    pinfo.lx = pinfo.x;
                    pinfo.ly = pinfo.y;
                }

                pinfo.jumping = null;
            }

            if (gmap != null)
            {
                if (gmap.blvlmap)
                {
                    //if (this.gmap.worldsvr.is_lvlmis_lvl())
                    //{
                    //    this.allow_respawn_tm = sys.clock_time() + 6000; // 侠客行副本中，必须6秒后复活
                    //}

                    this.gmap.worldsvr.on_ply_die(this);
                }
                else
                {
                    if (gmap.pk_seting != map_pk_setting_type.MPST_PK)
                    {//经验惩罚                       

                    }
                }

                if (killer != null && killer.get_sprite_type() == map_sprite_type.MstMonster)
                {
                    if (killer.owner_ply != null)
                    {
                        killer = killer.owner_ply; // 宠物击杀相当于玩家角色击杀
                    }
                }

                if (killer != null && killer.get_sprite_type() == map_sprite_type.MstPlayer)
                {
                    IMapUnit killer_pl = killer.get_pack_data();
                    //local pkdp = true;
                    if (!gmap.blvlmap)
                    {
                        // 添加仇敌
                        //_add_foes(this, killer.pinfo);

                        //计算pk值 
                        //是否为中立地图
                        if (gmap.pk_seting == map_pk_setting_type.MPST_NORMAL)
                        {
                            if (!this.can_atk_direct())
                            {
                                //自己为正义玩家
                                //sys.dumpobj( defend_tm_map );
                                //sys.trace( sys.SLT_ERR, "killer.pinfo.cid = " + killer.pinfo.cid + "sys.time()" + sys.time() + "\n")
                                int killer_cid = killer_pl.cid;
                                long defend_en_tm = 0;
                                long now_s = DateTime.Now.ToBinary();
                                defend_tm_map.TryGetValue(killer_cid, out defend_en_tm);
                                if (defend_en_tm > 0)
                                {
                                    //自己是正当防卫 则
                                    if (defend_en_tm >= now_s)
                                    {
                                        //增加对方pk值 
                                        int add_val = 1;
                                        int pkv = killer_pl.pk_v;
                                        if (pkv < 0)
                                        {//击杀者 有正义值 则清除
                                            add_val = 1 - pkv;
                                        }
                                        killer.add_pk_v(add_val);
                                    }
                                    else
                                    {
                                        defend_tm_map.Remove(killer_cid);
                                    }
                                }
                            }
                            else
                            {//可直接攻击的玩家
                                if (killer.is_justice())
                                {//击杀者 为正义玩家 降低PK值
                                    killer.sub_pk_v(1, true);
                                }
                            }
                        }
                        this.check_tsk_attribute(true);
                    }
                    else
                    {
                        // 在副本中
                        // 增加击杀值
                        killer_pl.kp++;
                        killer_pl.cur_kp++;

                        this.gmap.worldsvr.on_kp(killer, this);
                    }
                }

                // remove atking from atker
                // TO DO : 增加被锁定列表，死亡后清理被锁定列表中的对象的攻击信息即可
                foreach (var val in gmap.map_sprites.Values)
                {
                    val.on_spr_die(pinfo.iid);
                }
            }

            // 清理助攻列表
            if (this.kp_asist_rec)
            {
                this.beatk_ply_cids.Clear();
            }
        }
        public void atk_by(IBaseUnit atker)
        {
            // attack back
            // TO DO : be attack
        }
        public void onhate(IBaseUnit atker, int dmg)
        {
            // monster特有函数，player不做处理
            if (petmon_inst != null)
            {
                petmon_inst.onhate(atker, dmg);
            }
        }
        public void ondmg(IBaseUnit atker, int dmg)
        {
            int atker_cid = atker.get_pack_data().cid;
            long now_s = DateTime.Now.ToBinary();
            // 被攻击
            if (this.kp_asist_rec)
            {
                // 需要记录助攻
                if (atker.get_sprite_type() == map_sprite_type.MstMonster)
                {
                    return;
                }

                this.beatk_ply_cids[atker_cid] = now_s;  // 记录攻击者cid

                this.lastdmgtm = now_s; // 记录最后一次受伤害时间
            }

            if (gmap.pk_seting == map_pk_setting_type.MPST_NORMAL)
            {
                if (atker.get_sprite_type() == map_sprite_type.MstPlayer)
                {
                    if (atker_cid != pinfo.cid && !this.can_atk_direct())
                    {
                        if (!atker.defend_tm_map.ContainsKey(pinfo.cid))
                        {
                            //是否被正当防御 
                            //是否第一次记录
                            defend_tm_map[atker_cid] = now_s + 10000;

                        }
                    }
                }
            }
        }
        public bool isaily(IBaseUnit spr)
        {
            if (spr.get_sprite_type() == map_sprite_type.MstMonster && spr.owner_ply != null)
                spr = spr.owner_ply; // 用主人代替宠物检查敌我关系

            IMapUnit pl = spr.get_pack_data();
            if (pl.iid == this.pinfo.iid)
                return true;

            if (pl.lvlsideid != 0 && pinfo.lvlsideid == pl.lvlsideid)
                return true;

            if (pl.teamid == 0 || pinfo.teamid == 0)
                return false;

            if (pl.teamid == pinfo.teamid)
                return true;

            if (pl.clanid == 0 || pinfo.clanid == 0)
                return false;

            if (pl.clanid == pinfo.clanid)
                return true;

            return false;
        }

        public bool can_atk(IBaseUnit spr)
        {
            if (spr.get_sprite_type() == map_sprite_type.MstMonster && spr.owner_ply != null)
                spr = spr.owner_ply; // 用主人代替宠物检查敌我关系

            IMapUnit pl = spr.get_pack_data();
            if (pl.iid == this.pinfo.iid)
                return false;

            //if (spr.get_sprite_type() == map_sprite_type.MstPlayer && pl.level < game_conf.pk_safe_lvl)
            //{
            //    return false;
            //}

            if (pl.lvlsideid != 0)
            {
                if (pinfo.lvlsideid != pl.lvlsideid)
                    return true;

                if ((gmap != null && !gmap.ignore_side) || spr.get_sprite_type() == map_sprite_type.MstMonster)
                    return false;
            }

            bool cannt_pk = false;
            switch (pinfo.pk_state)
            {
                case pk_state_type.PKST_PEACE:
                    cannt_pk = spr.get_sprite_type() == map_sprite_type.MstPlayer;
                    break;
                case pk_state_type.PKST_PK_ALL:
                    cannt_pk = false;
                    break;
                case pk_state_type.PKST_PK_TEAM:
                    if (pl.teamid == 0 || pinfo.teamid == 0)
                        cannt_pk = false;
                    else
                        cannt_pk = pl.teamid == pinfo.teamid;
                    break;
                case pk_state_type.PKST_PK_CLAN:

                    if (pl.clanid == 0 || pinfo.clanid == 0)
                        cannt_pk = false;
                    else
                        cannt_pk = pl.clanid == pinfo.clanid;
                    break;
            }

            return !cannt_pk;
        }

        //厌恶的
        public bool is_hateable()
        {
            return (pinfo.rednm == rednm_type.RNT_RASCAL) || (pinfo.rednm == rednm_type.RNT_EVIL) || (pinfo.rednm == rednm_type.RNT_DEVIL);
        }
        //正义的
        public bool is_justice()
        {
            return (pinfo.rednm == rednm_type.RNT_NORMAL) || (pinfo.rednm == rednm_type.RNT_RIGHT) || (pinfo.rednm == rednm_type.RNT_HERO) || (pinfo.rednm == rednm_type.RNT_SHERO);
        }
        //是否可以直接攻击
        public bool can_atk_direct()
        {
            return (pinfo.rednm == rednm_type.RNT_EVIL) || (pinfo.rednm == rednm_type.RNT_DEVIL);
        }

        public void check_tsk_attribute(bool bcast = false)
        {
            // 是否已接护送任务

            //    local old_misatt_len = pinfo.misplatts.len();
            //    pinfo.misplatts.clear();

            //    local old_follow = 0;
            //    if ("follow" in pinfo)
            //{
            //        old_follow = pinfo.follow;
            //    }

            //    local follow = 0;
            //    pinfo.followka = 0;
            //    foreach (mis in pinfo.misacept)
            //    {
            //        local conf = get_mis_conf(mis.misid);
            //        if (!conf)
            //        {
            //            continue;
            //        }

            //        if ("follow" in conf.goal)
            //    {
            //            follow = conf.goal.follow;
            //        }

            //        if (("kilmon" in conf.goal) && ("km" in mis))
            //    {
            //            foreach (val in conf.goal.kilmon)
            //            {
            //                if (!("follow" in val))
            //            {
            //                    continue;
            //                }

            //                foreach (km in mis.km)
            //                {
            //                    if (km.monid != val.monid)
            //                    {
            //                        continue;
            //                    }

            //                    if (km.cnt < val.cnt)
            //                    {
            //                        continue;
            //                    }

            //                    follow = val.follow;
            //                }

            //                break;
            //            }
            //        }

            //        if ("kiler_awd" in conf.goal)
            //    {
            //            pinfo.followka = 1;
            //        }

            //        if ("attmod" in conf.accept)
            //    {
            //            foreach (att in conf.accept.attmod)
            //            {
            //                pinfo.misplatts.push(att);
            //            }
            //        }
            //    }

            //    if (follow > 0)
            //    {
            //        pinfo.follow < -follow;

            //        if (bcast && old_follow != follow)
            //        {
            //            // broad cast attchange msg
            //            broad_cast_zone_msg(26, { iid = pinfo.iid, follow = pinfo.follow});
            //        }
            //    }
            //    else if (old_follow > 0)
            //    {
            //        delete pinfo.follow;
            //        if (bcast)
            //        {
            //            // broad cast attchange msg
            //            broad_cast_zone_msg(26, { iid = pinfo.iid, follow = 0});
            //        }
            //    }

            //if(pinfo.followka > 0)
            //{
            //    // 有击杀奖励，强制打开pk开关

            //    if(pinfo.pk_state != pk_state_type.PKST_PK)
            //    {
            //        pinfo.pk_state = pk_state_type.PKST_PK;

            //        if(bcast)
            //        {
            //            // send pk_state_change msg
            //            this.broad_cast_zone_msg_and_self(2, pinfo);
            //        }
            //    }
            //}
            //else if(pinfo.pk_state != pk_state_type.PKST_PEACE)
            //{

            //}

            //if (old_misatt_len != pinfo.misplatts.len())
            //{
            //    // 有属性调整变化，重新计算属性

            //    if (bcast)
            //    {
            //        this.re_calc_cha_data();
            //    }
            //}
        }
        public bool has_follow()
        {
            return pinfo.follow != null;
        }

        public void on_spr_die(int iid)
        {
            //if ( pinfo.pk_tar_iid == iid )
            //{
            //    pinfo.pk_tar_iid = 0;
            //}
            if (pinfo.atking == null)
                return;

            if (pinfo.atking.tar_iid == iid)
                pinfo.atking = null;
        }
        public bool isdie()
        {
            // TO DO : is die
            return this.pinfo.isdie;
        }
        public bool isghost()
        {
            return this.pinfo.ghost;
        }
        public bool has_state(int state_val)
        {
            return Skill.has_pl_state(state_val, pinfo);
        }
        public bool ignore_dmg()
        {
            return false;
        }
        public int get_state_par(int state_val)
        {
            return Skill.get_pl_state_par(state_val, pinfo);
        }
        public long get_state_end_tm(int state_val)
        {
            return Skill.get_pl_state_end_tm(state_val, pinfo);
        }
        public SkillData get_skil_data(int skid)
        {
            SkillData skil = null;

            this.pinfo.skill.TryGetValue(skid, out skil);

            return skil;
        }



        public bool modify_level(int add_lvl, bool empty_exp = true)
        {
            if (add_lvl <= 0)
                return false;

            int new_lvl = pinfo.level + add_lvl;

            if (new_lvl <= pinfo.level)
            {// 满级了，无法再升级
                return false;
            }

            if (empty_exp)
            {//清空当前经验
                pinfo.exp = 0;
            }

            _level_up(new_lvl - pinfo.level, 0);
            return true;
        }


        //外部 需要 确保在 有效等级范围内
        public void _level_up(int add_lvl, int mod_exp)
        {
            // 升级
            int old_lvl = pinfo.level;
            pinfo.level += add_lvl;

            re_calc_cha_data();

            pinfo.hp = pinfo.max_hp;
            pinfo.mp = pinfo.max_mp;
            pinfo.dp = pinfo.max_dp;
        }

        public void modify_exp(int exp_added, bool bstate_add = false)
        {
            if (exp_added == 0)
                return;

            int expmul = Skill._get_bstate_exp_mul(this);   //玩家经验印章等经验祝福状态的加成结算
            //expmul += pinfo.ach_exp_mul;                //玩家称号的经验加成属性的结算
            //expmul += pinfo.eqp_exp_mul;                //玩家装备经验值加成结算

            if (bstate_add && expmul > 0)
            {
                exp_added = (exp_added * (100 + expmul) / 100);
            }

            pinfo.exp += exp_added;
            if (pinfo.exp < 0)
            {
                pinfo.exp = 0;
            }
        }

        public void modify_mp(int mp_add)
        {
            pinfo.mp += mp_add;
            if (pinfo.mp > pinfo.max_mp) pinfo.mp = pinfo.max_mp;

            // send mpchange msg
            //::send_rpc(pinfo.sid, 32, { mpadd = mp_add});
        }

        public void modify_dp(int dp_add)
        {
            pinfo.dp += dp_add;
            if (pinfo.dp > pinfo.max_dp) pinfo.dp = pinfo.max_dp;

            // send mpchange msg
            //::send_rpc(pinfo.sid, 32, { dpadd = dp_add});
        }
        public void re_calc_cha_data(bool add_change_atts = false)
        {
            int old_speed = pinfo.speed;
            int old_max_hp = pinfo.max_hp;
            int old_max_mp = pinfo.max_mp;
            int old_max_dp = pinfo.max_dp;

            double hp_percent = ((double)this.pinfo.hp) / this.pinfo.max_hp;
            double mp_percent = ((double)this.pinfo.mp) / this.pinfo.max_mp;
            double dp_percent = ((double)this.pinfo.dp) / this.pinfo.max_dp;

            Utility.calc_cha_data(this, pinfo, false);

            if (this.pinfo.max_hp < 0)
            {
                this.pinfo.max_hp = 1;
            }
            if (this.pinfo.max_mp < 0)
            {
                this.pinfo.max_mp = 1;
            }
            if (this.pinfo.max_dp < 0)
            {
                this.pinfo.max_dp = 1;
            }

            if (old_max_hp != pinfo.max_hp)
            {
                this.pinfo.hp = (int)(this.pinfo.max_hp * hp_percent);
            }
            //避免 old_max_mp == pinfo.max_mp时  计算mp_percent 带来的误差 不同步到客户端
            if (old_max_mp != pinfo.max_mp)
            {
                this.pinfo.mp = (int)(this.pinfo.max_mp * mp_percent);
            }
            if (old_max_dp != pinfo.max_dp)
            {
                this.pinfo.dp = (int)(this.pinfo.max_dp * dp_percent);
            }

            //sys.trace(sys.SLT_DETAIL, "old_max_mp [" + old_max_mp +"] this.pinfo.mp["+this.pinfo.mp+"] this.pinfo.max_mpd["+this.pinfo.max_mp+"] \n");


            //sys.trace(sys.SLT_DETAIL, "pl.iid [" + pinfo.iid +"] old_speed["+old_speed+"] pinfo.speed["+pinfo.speed+"] \n");

            //sys.trace(sys.SLT_DETAIL, "pl.iid [" + pinfo.iid +"] old_max_hp["+old_max_hp+"] pinfo.max_hp["+pinfo.max_hp+"] \n");

        }

        public void respawn(int percent, bool back_town, bool checkallow = false)
        {
            if (gmap == null)
            {
                return;
            }

            long now_s = DateTime.Now.ToBinary();

            if (checkallow && this.allow_respawn_tm > now_s)
            {
                // 还不能复活
                return;
            }

            //::g_debug( "respawn" );

            this.pinfo.isdie = false;
            this.dying = false;
            this.pinfo.hp = this.pinfo.max_hp * percent / 100;
            this.pinfo.mp = this.pinfo.max_mp * percent / 100;
            this.pinfo.dp = this.pinfo.max_dp * percent / 100;
            this.respawn_tm = 0;
            this.allow_respawn_tm = 0;
            this.allow_respawn_tm_s = 0;

            pinfo.atking = null;
            pinfo.moving = null;
            //if("states" in this.pinfo)
            //{
            //    delete this.pinfo.states;
            //}

            if (back_town)
            {
                // 回城
                if (this.is_in_lvl)
                {
                    // 在副本中，回出生点
                    Level lvl = gmap.worldsvr;
                    lvl.player_respawn(this);
                }
                else
                {
                    // 在大世界中
                    var born_pos = gmap.get_born_area();

                    // 跳转至指定出生点
                    pinfo.x = Utility.random_section(born_pos.x, born_pos.w);
                    pinfo.y = Utility.random_section(born_pos.y, born_pos.h);
                    pinfo.map_id = born_pos.mpid;
                    pinfo.last_mvpts = null;

                    pinfo.lmpid = pinfo.map_id;
                    pinfo.lx = pinfo.x;
                    pinfo.ly = pinfo.y;

                    //::g_dump( "跳转至指定出生点 born_area:", born_area );



                    var game_ref = gmap.worldsvr;
                    //if (game_ref.is_map_in_thr(pinfo.map_id))
                    {
                        gmap.rmv_player_from_map(game_ref, pinfo.sid);
                        game_ref.maps[pinfo.map_id].add_player_to_map(game_ref, pinfo.sid, this);
                    }
                    //else
                    //{
                    //    gmap.rmv_player_from_map(game_ref, pinfo.sid);

                    //    this.flush_db_data(false, false, false);
                    //    svr.readd_session(pinfo.sid);
                }
            }


            // send respawn msg
            //gmap.broadcast_map_rpc(21, this.pinfo);
            //broad_cast_zone_msg_and_self(21, this.pinfo);
        }

        public bool goto_worldmap(int mpid, double x, double y)
        {
            // 跳转至大世界目标地图（地图必须是大世界地图）
            if (gmap == null)
            {
                return false;
            }

            // TO DO : 判断mpid是否为大世界中的地图
            if (!gmap.worldsvr.is_map_exist(mpid))
            {
                return false;
            }

            pinfo.x = x;
            pinfo.y = y;
            pinfo.map_id = mpid;
            pinfo.last_mvpts = null;

            pinfo.moving = null;

            if (this.is_in_lvl)
            {
                // 在副本中
                gmap.rmv_player_from_map(gmap.worldsvr, pinfo.sid);

                //this.flush_db_data(false, false, false);
                Level.enter_world(pinfo.sid, pinfo.line);
            }
            else
            {
                pinfo.lmpid = pinfo.map_id;
                pinfo.lx = pinfo.x;
                pinfo.ly = pinfo.y;

                // 在大世界中

                var game_ref = gmap.worldsvr;

                //if (game_ref.is_map_in_thr(pinfo.map_id))
                {
                    gmap.rmv_player_from_map(game_ref, pinfo.sid);
                    game_ref.maps[pinfo.map_id].add_player_to_map(game_ref, pinfo.sid, this);
                }
                //else
                {
                    //gmap.rmv_player_from_map(game_ref, pinfo.sid);

                    //this.flush_db_data(false, false, false);
                    //svr.readd_session(pinfo.sid);
                }
            }

            return true;
        }


        public void update(IService game_ref, long cur_clock_tm, long tm_elasped_s)
        {
            //local game_conf = get_general_game_conf();

            long cur_tm_s = Utility.time(); // TO DO : use time manager

            if (pinfo.pk_v > 0)
            {
                if (cur_tm_s - pinfo.pk_v_revtm > 3600) // 1小时减少1点
                {
                    pinfo.pk_v_revtm = cur_tm_s;
                    this.sub_pk_v(1);
                }
            }

            //正当防卫 更新
            List<int> removed_defend_tm_map = new List<int>();
            foreach (KeyValuePair<int, long> pair in this.defend_tm_map)
            {
                if (pair.Value <= cur_tm_s)
                {
                    removed_defend_tm_map.Add(pair.Key);


                    //::send_rpc(pinfo.sid, 2, { defend = false,cid = atker_cid});
                    //local be_defend_sid = svr.get_sid_by_cid(atker_cid);
                    //if (be_defend_sid > 0)
                    //{
                    // ::send_rpc(be_defend_sid, 2, { be_defend = false,cid = pinfo.cid});
                    //}
                }
            }

            foreach (var did in removed_defend_tm_map)
                this.defend_tm_map.Remove(did);


            if (this.allow_respawn_tm_s > 0)
            {
                this.allow_respawn_tm_s -= tm_elasped_s;
                if (this.allow_respawn_tm_s < 0) this.allow_respawn_tm_s = 0;
            }


            if (respawn_tm > 0)
            {

                //sys.trace(sys.SLT_ERR, "player["+this.pinfo.iid+"] die res tm[" + respawn_tm +"] cur tm[" +cur_clock_tm+"]\n");

                // dead
                if (respawn_tm < cur_clock_tm)
                {
                    respawn(100, true);
                }

                return;
            }

            if (gmap == null)
            {
                return;
            }

            if (gmap.map_fined)
            {
                Utility.trace_err("err:gmap finished player cid=[" + this.pinfo.cid + "] mapid=[" + this.gmap.mapid + "]\n");
                return;
            }

            gmap.update_skill_casting(this, pinfo, cur_clock_tm);
            gmap.update_skill_holding(this, pinfo, cur_clock_tm);
            gmap.update_jumping(this, pinfo, cur_clock_tm);
            gmap.update_teleping(this, pinfo, cur_clock_tm);

            if (gmap.is_player_in_pc_zone(this) != this.pinfo.in_pczone)
            {
                this.pinfo.in_pczone = !this.pinfo.in_pczone;
                if (this.pinfo.in_pczone && this.pinfo.pk_state != pk_state_type.PKST_PEACE)
                {
                    set_pk_state(pk_state_type.PKST_PEACE);
                }
                // 通知客户端进入、离开和平区域
                // send attchange msg
                //this.broad_cast_zone_msg_and_self(26, { iid = pinfo.iid, in_pczone = this.pinfo.in_pczone});
            }

            Skill.update_pl_state(cur_clock_tm, this);
            Skill._update_pl_bstate(cur_tm_s, this);

            bool is_moving_before = false;
            if (pinfo.moving != null)
                is_moving_before = true;

            int tm_left = grid_map.update_pl_move(this, cur_clock_tm);

            if (pinfo.moving == null)
            {
                if (pinfo.atking != null)
                // player stoped move
                {
                    // need trace target
                    //this.pinfo.atking.trace_tm_left = tm_left;
                    //update_pl_atk_tracing(this, cur_clock_tm);
                    //update_pl_move(this, cur_clock_tm); // update move again
                }
                //else if(bwantchangmap && is_moving_before)
                //{
                //    local grid = gmap.get_grid_by_pt(pinfo.x, pinfo.y);
                //    if(grid != null)
                //    {
                //        //sys.trace(sys.SLT_ERR, "player stand on x[" + grid.x +"] y[" +grid.y+"] ptx["+pinfo.x+"] pty["+pinfo.y+"]\n");
                //        local link = gmap.get_link(grid.x, grid.y);
                //        if(link != null)
                //        {
                //            sys.trace(sys.SLT_ERR, "find map link to["+link.to+"] to_x[" + link.to_x +"] to_y[" +link.to_y+"]\n");
                //            // player change map
                //            // TO DO : 考虑目标地图人过多的情况
                //            //if(link.to in game_ref.maps)
                //            //{
                //                sys.trace(sys.SLT_ERR, "add player sid["+pinfo.sid+"] to map[" + link.to +"]\n");

                //                gmap.rmv_player_from_map(game_ref, pinfo.sid);

                //                pinfo.x = link.to_x*game_const.map_grid_pixel+ 16;
                //                pinfo.y = link.to_y*game_const.map_grid_pixel+ 16;
                //                pinfo.map_id = link.to;

                //                if(game_ref.is_map_in_thr(link.to))
                //                {
                //                    game_ref.maps[link.to].add_player_to_map(game_ref, pinfo.sid, this);
                //                }
                //                else
                //                {
                //                    this.flush_db_data(false, false);
                //                    svr.readd_session(pinfo.sid);
                //                }

                //                bwantchangmap = false;

                //                return true;
                //            //}
                //        }
                //    }
                //}
            }

            grid_map.update_pl_atk(this, cur_clock_tm);

            //if("atking" in this.pinfo)
            //{
            //    this.pinfo.atking.trace_tm_left = 0;
            //    update_pl_atk_tracing(this, cur_clock_tm);
            //}

            //meri_update_pl_acupup(this, cur_tm_s);
            //skil_update_pl_skilup(this, cur_tm_s);
            //trade_update_trade_req(this, cur_tm_s);
            //pet_update_pet(this, cur_tm_s);

            //if(this.pinfo.recover > 0)
            //{
            //    // 打坐中
            //    if(this.pinfo.next_recover_tm < cur_tm_s)
            //    {
            //        // +血
            //        if(this.pinfo.hp < this.pinfo.max_hp)
            //        {
            //            local hp_add = (this.pinfo.max_hp * game_conf.recover_hp_per / 1000).tointeger();
            //            this.pinfo.hp += hp_add;
            //            if(this.pinfo.hp > this.pinfo.max_hp) this.pinfo.hp = this.pinfo.max_hp;

            //            // broadcast hpchange msg
            //            this.broad_cast_zone_msg_and_self(26, {iid=pinfo.iid, hpchange={hpadd=hp_add, die=false}});
            //        }

            //        // +魔
            //        if(this.pinfo.mp < this.pinfo.max_mp)
            //        {
            //            local mp_add = (this.pinfo.max_mp * game_conf.recover_mp_per / 1000).tointeger();
            //            this.pinfo.mp += mp_add;
            //            if(this.pinfo.mp > this.pinfo.max_mp) this.pinfo.mp = this.pinfo.max_mp;

            //            // send mpchange msg
            //            ::send_rpc(pinfo.sid, 32, {mpadd=mp_add});
            //        }

            //        // +经验
            //        local exp_add = (game_conf.recover_exp_base + game_conf.recover_exp_perlvl * this.pinfo.level).tointeger();
            //        if(exp_add > 0)
            //        {

            //            if("vip_conf" in this.pinfo)
            //            {
            //                if("recov_expmul" in this.pinfo.vip_conf)
            //                {
            //                    exp_add += (exp_add*this.pinfo.vip_conf.recov_expmul/100).tointeger();
            //                }
            //            }

            //            this.modify_exp(exp_add);
            //        }

            //        this.pinfo.next_recover_tm = cur_tm_s + game_conf.recover_tm;
            //    }

            //    if(this.pinfo.next_recover_skpttm < cur_tm_s)
            //    {
            //        // +气海值
            //        local skpt = (game_conf.recover_skpt_base + game_conf.recover_skpt_perlvl * (this.pinfo.level - game_conf.recover_skpt_minlvl)).tointeger();

            //        //sys.trace(sys.SLT_SYS, "skpt["+skpt+"]\n");
            //        if(skpt > 0)
            //        {
            //            this.modify_skexp(skpt);
            //        }

            //        this.pinfo.next_recover_skpttm = cur_tm_s + game_conf.recover_skpt_tm;
            //    }
            //}

            //if (this.pinfo.next_hpmpr_tm < cur_tm_s)
            //{
            //    if (!pinfo.isdie)
            //    {
            //        if (pinfo.hpr > 0 || pinfo.dprv > 0)
            //        {
            //            local hpchange = { };
            //            // +血
            //            if (pinfo.hpr > 0 && this.pinfo.hp < this.pinfo.max_hp)
            //            {
            //                local hp_add = (this.pinfo.max_hp * pinfo.hpr / 1000.0).tointeger();
            //                this.pinfo.hp += hp_add;
            //                if (this.pinfo.hp > this.pinfo.max_hp) this.pinfo.hp = this.pinfo.max_hp;

            //                hpchange.hpadd < -hp_add;
            //                hpchange.die < -false;
            //            }

            //            // +防护值
            //            if (pinfo.dprv > 0 && this.pinfo.dp < this.pinfo.max_dp)
            //            {
            //                local dp_add = pinfo.dprv;
            //                this.pinfo.dp += dp_add;
            //                if (this.pinfo.dp > this.pinfo.max_dp) this.pinfo.dp = this.pinfo.max_dp;

            //                hpchange.dpadd < -dp_add;
            //            }

            //            if (hpchange.len() > 0)
            //            {
            //                // broadcast hpchange msg
            //                this.broad_cast_zone_msg_and_self(26, { iid = pinfo.iid, hpchange = hpchange});
            //            }
            //        }

            //        if (pinfo.mpr > 0)
            //        {
            //            // +魔
            //            if (this.pinfo.mp < this.pinfo.max_mp)
            //            {
            //                local mp_add = (this.pinfo.max_mp * pinfo.mpr / 1000.0).tointeger();
            //                this.pinfo.mp += mp_add;
            //                if (this.pinfo.mp > this.pinfo.max_mp) this.pinfo.mp = this.pinfo.max_mp;

            //            // send mpchange msg
            //            ::send_rpc(pinfo.sid, 32, { mpadd = mp_add});
            //            }
            //        }
            //    }
            //    this.pinfo.next_hpmpr_tm = cur_tm_s + game_conf.hpmpr_tm;
            //}







            if (this.pet_mon.mid > 0)
            {
                if (this.pet_mon.fintm <= cur_tm_s)
                {
                    // 战斗宠物的时间到了
                    release_petmon(false);
                }
            }
        }



        //进入副本地图
        public bool goto_levelmap(int mpid, double x, double y)
        {
            if (gmap == null)
            {
                // send begin chang map res
                //::send_rpc(pinfo.sid, 57, { res = game_err_code.PARAMETER_ERR});
                return false;
            }

            if (!this.is_in_lvl)
            {
                // send begin chang map res
                //::send_rpc(pinfo.sid, 57, { res = game_err_code.PARAMETER_ERR});
                return false;
            }

            if (bwantchangmap)
            {
                return false;
            }

            if (!this.in_lvl.is_map_exist(mpid))
            {
                return false;
            }

            long cur_clock_tm = Utility.time();
            grid_map.update_pl_move(this, cur_clock_tm);

            // allow change

            // TO DO : 考虑目标地图人过多的情况

            // 停止移动
            this.pinfo.moving = null;

            gmap.rmv_player_from_map(gmap.worldsvr, pinfo.sid);

            pinfo.x = x;
            pinfo.y = y;
            pinfo.map_id = mpid;
            pinfo.last_mvpts = null;

            // bwantchangmap = true;

            in_lvl.maps[pinfo.map_id].add_player_to_map(this.in_lvl, pinfo.sid, this);

            return true;
        }

        public void on_hurt(IBaseUnit tar_sprite)
        {   //当攻击到别的目标时 触发的被动
            // if(!this.has_wing()) return;

            //    foreach (pos,state in pinfo.wingholes.actstates)
            //{
            //        local state_conf = get_wing_stat_conf(state.sid);
            //        if (!state_conf)
            //        {
            //        ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});
            //            return;
            //        }
            //        if (("atk" in state_conf ) && state_conf.atk > 0 )
            //    {   //攻击类型状态技能
            //            on_mount_st_trig(state, tar_sprite, state_conf, false);
            //        }
            //    }
        }


        public void on_be_hurt(IBaseUnit frm_sprite)
        {   //当被攻击时 触发的被动  
            // if(!this.has_wing()) return;

            //    foreach (pos,state in pinfo.wingholes.actstates)
            //{
            //        local state_conf = get_wing_stat_conf(state.sid);
            //        if (!state_conf)
            //        {
            //        ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});
            //            return;
            //        }
            //        if (("def" in state_conf ) && state_conf.def > 0 )
            //    {   //防御类型状态技能
            //            on_mount_st_trig(state, frm_sprite, state_conf, true);
            //        }
            //    }
        }

        public void begin_change_map(IService game_ref, int gto)
        {
            if (gmap == null)
            {
                // send begin chang map res
                //::send_rpc(pinfo.sid, 57, { res = game_err_code.PARAMETER_ERR});
                return;
            }

            if (bwantchangmap)
            {
                return;
            }

            var cur_clock_tm = Utility.time();
            //sys.trace(sys.SLT_ERR, "cur server_tm [" + cur_clock_tm +"]\n");
            grid_map.update_pl_move(this, cur_clock_tm);

            var link = gmap.get_link_by_gto(gto);
            if (link == null)
            {
                // 遍历查找动态添加链接点
                foreach (var dlink in gmap.add_links.Values)
                {
                    if (dlink.gto != gto)
                    {
                        continue;
                    }
                    if (Math.Abs(dlink.x * game_const.map_grid_pixel + 16 - pinfo.x) > 300 ||
                        Math.Abs(dlink.y * game_const.map_grid_pixel + 16 - pinfo.y) > 300)
                    {
                        continue;
                    }
                    link = dlink;
                }

                if (link == null)
                {
                    // send begin chang map res
                    //::send_rpc(pinfo.sid, 57, { res = game_err_code.MAP_LINK_NOT_EXIST});
                    return;
                }
            }
            int tomap = link.gto;
            bool quit_lvl = false;

            pinfo.moving = null;
            // 停止移动            

            gmap.rmv_player_from_map(game_ref, pinfo.sid);

            //    if ("rand_pos" in link )
            //{
            //        local rand_pos = _n_choose_1(link.rand_pos);
            //        pinfo.x = rand_pos.to_x * game_const.map_grid_pixel + 16;
            //        pinfo.y = rand_pos.to_y * game_const.map_grid_pixel + 16;
            //    }
            //else
            {
                pinfo.x = link.to_x * game_const.map_grid_pixel + 16;
                pinfo.y = link.to_y * game_const.map_grid_pixel + 16;
            }

            pinfo.map_id = tomap;
            pinfo.last_mvpts = null;

            if (!this.is_in_lvl || quit_lvl)
            {
                pinfo.lmpid = pinfo.map_id;
                pinfo.lx = pinfo.x;
                pinfo.ly = pinfo.y;
            }

            bwantchangmap = true;

            // send begin chang map res
            //::send_rpc(pinfo.sid, 57, { res = 1});
        }


        public void end_change_map(IService game_ref)
        {
            if (!bwantchangmap)
            {
                return;
            }

            //if (game_ref.is_map_in_thr(pinfo.map_id))
            //{
            game_ref.maps[pinfo.map_id].add_player_to_map(game_ref, pinfo.sid, this);
            //}
            //else
            //{
            //    if (!game_ref.is_map_exist(pinfo.map_id))
            //    {
            //        if (this.is_in_lvl)
            //        {
            //            //this.in_lvl.remove_player(pinfo.sid);
            //            this.flush_db_data(false, false, false);
            //            level.enter_world(pinfo.sid, pinfo.line);
            //        }
            //        else
            //        {
            //            sys.trace(sys.SLT_ERR, "ply change to map[" + pinfo.map_id + "] not exist!\n");
            //        }
            //    }
            //    else
            //    {
            //        this.flush_db_data(false, false, false);
            //        svr.readd_session(pinfo.sid);
            //    }
            //}

            bwantchangmap = false;
        }


        public void change_map(IService game_ref)
        {
            //local cur_clock_tm =  DateTime.Now.ToBinary();
            //sys.trace(sys.SLT_ERR, "cur server_tm [" + cur_clock_tm +"]\n");
            //update_pl_move(this, cur_clock_tm);

            //local grid = gmap.get_grid_by_pt(pinfo.x, pinfo.y);
            //if(grid != null)
            //{
            //    sys.trace(sys.SLT_ERR, "player stand on x[" + grid.x +"] y[" +grid.y+"] ptx["+pinfo.x+"] pty["+pinfo.y+"]\n");
            //    local link = gmap.get_link(grid.x, grid.y);
            //    if(link != null)
            //    {
            //        sys.trace(sys.SLT_ERR, "find map link to["+link.to+"] to_x[" + link.to_x +"] to_y[" +link.to_y+"]\n");
            //        // player change map
            //        // TO DO : 考虑目标地图人过多的情况
            //        //if(link.to in game_ref.maps)
            //        //{
            //            sys.trace(sys.SLT_ERR, "add player sid["+pinfo.sid+"] to map[" + link.to +"]\n");

            //            gmap.rmv_player_from_map(game_ref, pinfo.sid);

            //            pinfo.x = link.to_x*game_const.map_grid_pixel+ 16;
            //            pinfo.y = link.to_y*game_const.map_grid_pixel+ 16;
            //            pinfo.map_id = link.to;

            //            if(game_ref.is_map_in_thr(link.to))
            //            {
            //                game_ref.maps[link.to].add_player_to_map(game_ref, pinfo.sid, this);
            //            }
            //            else
            //            {
            //                this.flush_db_data(false, false);
            //                svr.readd_session(pinfo.sid);
            //            }

            //            bwantchangmap = false;

            //            return true;
            //        //}
            //        //else
            //        //{

            //        //}
            //    }
            //    else
            //    {
            //        bwantchangmap = true;
            //    }
            //}

            //return false;
        }
        public MapUnitState get_state_exsit(int state_id)
        {
            if (pinfo.states == null || pinfo.states.state_par.Count <= 0)
                return null;

            foreach (var sta in pinfo.states.state_par)
            {
                if (sta.id == state_id)
                    return sta;
            }

            return null;
        }



        //--------------------------------------翅膀技能 end----------------------------------------------------------------------------

    }
}