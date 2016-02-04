using SceneTest.confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
    public class Player : IBaseUnit
    {
        public bool kp_log = true; //保存角色记录
        public string session_ip = "";

        public pinfo pinfo = null;

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

            delete petmon_inst.mondata.owner_cid;
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
            if (petmon_inst && petmon_inst.get_pack_data().mi == mid)
            {
                release_petmon();
                return;
            }

            release_petmon(false, false);

            local monconf = { mid = mid, x = 0, y = 0, r_x = 8, r_y = 8, spwan_time = 0 };
            petmon_inst = this.gmap.new_pet_mon(this, monconf);
            if (!petmon_inst)
            {
                return;
            }

            this.pinfo.pet_mon.mid = mid;
            this.pinfo.pet_mon.fintm = fintm;

            petmon_inst.mondata.follow < - { start_tm = sys.clock_time(), tar_iid = pinfo.iid, trace_tm_left = 0, frang = 78400, trang = 8100, do_ai = true};//280*280
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

        public void add_pk_v(add )
        {
            if (!curr_pklvl_conf) return;

            local old_pk_v = pinfo.pk_v;
            local new_pk_v = old_pk_v + add;
            if (curr_pklvl_conf.lvl == rednm_type.RNT_DEVIL)
            {//pk_v最大值 为魔头最大值 
                if (new_pk_v > curr_pklvl_conf.max)
                {
                    pinfo.pk_v = curr_pklvl_conf.max;
                }
                else
                {
                    pinfo.pk_v = new_pk_v;
                }
                // 广播红名等级变化消息
                if (old_pk_v != pinfo.pk_v)
                {
                    this.broad_cast_zone_msg_and_self(3, pinfo);
                }
                return;
            }

            pinfo.pk_v = new_pk_v;
            //sys.trace( sys.SLT_ERR, "pinfo.pk_v =  " + pinfo.pk_v + "\n");
            if (pinfo.rednm == rednm_type.RNT_NORMAL)
            {
                if (pinfo.pk_v > 0)
                {
                    pinfo.pk_v_revtm = sys.time();
                }
            }
            if (new_pk_v > curr_pklvl_conf.max)
            {//调整红名等级
                curr_pklvl_conf = get_general_pklvl_conf_by_pkv(new_pk_v);
                if (curr_pklvl_conf)
                {//已经到了最大等级
                    pinfo.rednm = curr_pklvl_conf.lvl;
                    //sys.trace( sys.SLT_ERR, "pinfo.rednm =  " + pinfo.rednm + "\n");           

                    if (pinfo.rednm == rednm_type.RNT_DEVIL)
                    {//大侠需要广播                                          
                        _broad_cast_sys_msg({ cid = pinfo.cid, name = pinfo.name, tp = bcast_msg_tp.REDNM_CHANGE, par = pinfo.rednm });
                    }
                }
                else
                {
                    sys.trace(sys.SLT_ERR, "add_pk_v(), curr_pklvl_conf is null  pk_v = " + pinfo.pk_v + "\n");
                }
            }
            // 广播红名等级变化消息
            this.broad_cast_zone_msg_and_self(3, pinfo);
        }

        //是否可以为负值be_neg   可以使用道具降低PK值
        public void sub_pk_v(sub, be_neg = false)
        {
            if (!curr_pklvl_conf) return;

            local old_pk_v = pinfo.pk_v;
            local new_pk_v = old_pk_v - sub;
            if (curr_pklvl_conf.lvl == rednm_type.RNT_SHERO)
            {//pk_v最小值 为大侠最小值 
                if (new_pk_v < curr_pklvl_conf.min)
                {
                    pinfo.pk_v = curr_pklvl_conf.min;
                }
                else
                {
                    pinfo.pk_v = new_pk_v;
                }

                // 广播红名等级变化消息
                if (old_pk_v != pinfo.pk_v)
                {
                    this.broad_cast_zone_msg_and_self(3, pinfo);
                }
                return;
            }

            //sys.trace( sys.SLT_ERR, "pinfo.pk_v =  " + pinfo.pk_v + "\n");
            if (new_pk_v < 0 && !be_neg) new_pk_v = 0;

            pinfo.pk_v = new_pk_v;

            if (new_pk_v < curr_pklvl_conf.min)
            {//调整红名等级
                curr_pklvl_conf = get_general_pklvl_conf_by_pkv(new_pk_v);
                if (curr_pklvl_conf)
                {//已经到了最小等级
                    pinfo.rednm = curr_pklvl_conf.lvl;
                    //sys.trace( sys.SLT_ERR, "pinfo.rednm =  " + pinfo.rednm + "\n");      

                    if (pinfo.rednm == rednm_type.RNT_SHERO)
                    {//大侠需要广播                                          
                        _broad_cast_sys_msg({ cid = pinfo.cid, name = pinfo.name, tp = bcast_msg_tp.REDNM_CHANGE, par = pinfo.rednm });
                    }
                }
                else
                {
                    sys.trace(sys.SLT_ERR, "sub_pk_v(), curr_pklvl_conf is null  pk_v = " + pinfo.pk_v + "\n");
                }
            }
            // 广播红名等级变化消息
            this.broad_cast_zone_msg_and_self(3, pinfo);
        }

        //----------------------------------------------------------------------------
        public void vip_lvl_changed(re_calc= true)
        {
            this._calc_vip_data();

            local old_vip_attpt = pinfo.vip_attpt;
            //sys.trace( sys.SLT_DETAIL, "pinfo.vip =" + pinfo.vip + "  old_vip_attpt=" + pinfo.vip_attpt + "\n" );
            this._re_calc_vip_attpt();
            //sys.trace( sys.SLT_DETAIL, "pinfo.vip_attpt" + pinfo.vip_attpt + "\n" );
            local change_attpt = pinfo.vip_attpt - old_vip_attpt;
            if (change_attpt != 0)
            {
                if (change_attpt > 0)
                {
                    pinfo.att_pt += change_attpt;
                }
                else if (change_attpt < 0)
                {
                    if (pinfo.att_pt + change_attpt > 0)
                    {
                        pinfo.att_pt += change_attpt;
                    }
                    else
                    {//洗点
                        pinfo.att_pt = pinfo.att_pt + pinfo.strpt + pinfo.conpt + pinfo.intept + pinfo.agipt + pinfo.wispt + change_attpt;
                        if (pinfo.att_pt < 0)
                        {
                            sys.trace(sys.SLT_ERR, "vip_lvl_changed  pinfo.cid = " + pinfo.cid + "pinfo.level = " + pinfo.level + "pinfo.resetlvl = " + pinfo.resetlvl + "\n");
                            sys.trace(sys.SLT_ERR, "vip_lvl_changed  old_vip_attpt = " + old_vip_attpt + " new_vip_attpt = " + pinfo.vip_attpt + "\n");
                            pinfo.att_pt = 0;
                        }

                        pinfo.strpt = 0;
                        pinfo.conpt = 0;
                        pinfo.intept = 0;
                        pinfo.agipt = 0;
                        pinfo.wispt = 0;
                    }
                }

                if (re_calc)
                {
                // send detail_info_change msg
                ::send_rpc(pinfo.sid, 40, { pinfo ={ att_pt = pinfo.att_pt} });
                }
            }

            if (re_calc)
            {
                this.re_calc_cha_data();
            }
        }


        public void broad_cast_zone_msg(cmd, data)
        {
            // 向在区域中、且能看到自己的用户角色广播消息
            if (pinfo.invisible > 0)
            {
                local obplys = this.get_observer_plys();
                return svr.mul_snd_rpc(obplys.sids, obplys.count, cmd, data);
            }
            else
            {
                return svr.mul_snd_rpc(this.get_inz_ply_sids(), this.get_inz_ply_cnt(), cmd, data);
            }
        }

        public void broad_cast_zone_msg_and_self(cmd, data)
        {
        ::send_rpc(pinfo.sid, cmd, data);

            return broad_cast_zone_msg(cmd, data);
        }

        public void broad_cast_non_observer_msg(cmd, data)
        {
            if (pinfo.invisible > 0)
            {
                // 向在区域中、但是看不到自己的用户角色广播消息
                local nobplys = this.get_non_observer_plys();
                //sys.dumpobj(nobplys);
                return svr.mul_snd_rpc(nobplys.sids, nobplys.count, cmd, data);
            }
        }

        public void broad_cast_zone_msg_igview(cmd, data)
        {
            // 向在区域中所有用户(包含不能看到自己的用户)角色广播rpc消息
            return svr.mul_snd_rpc(this.get_inz_ply_sids(), this.get_inz_ply_cnt(), cmd, data);
        }

        // 向在区域中所有用户(包含不能看到自己的用户)广播type package消息
        public void broad_cast_zone_tpkg_igview(cmd, data)
        {
            return svr.mul_snd_tpkg(this.get_inz_ply_sids(), this.get_inz_ply_cnt(), cmd, data);
        }
        public void broad_cast_zone_tpkg_igview_self(cmd, data)
        {
            svr.send_type_pkg(pinfo.sid, cmd, data);
            return svr.mul_snd_tpkg(this.get_inz_ply_sids(), this.get_inz_ply_cnt(), cmd, data);
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
            if (petmon_inst)
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
        public void is_hateable()
        {
            return (pinfo.rednm == rednm_type.RNT_RASCAL) || (pinfo.rednm == rednm_type.RNT_EVIL) || (pinfo.rednm == rednm_type.RNT_DEVIL);
        }
        //正义的
        public void is_justice()
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

            local old_misatt_len = pinfo.misplatts.len();
            pinfo.misplatts.clear();

            local old_follow = 0;
            if ("follow" in pinfo)
        {
                old_follow = pinfo.follow;
            }

            local follow = 0;
            pinfo.followka = 0;
            foreach (mis in pinfo.misacept)
            {
                local conf = get_mis_conf(mis.misid);
                if (!conf)
                {
                    continue;
                }

                if ("follow" in conf.goal)
            {
                    follow = conf.goal.follow;
                }

                if (("kilmon" in conf.goal) && ("km" in mis))
            {
                    foreach (val in conf.goal.kilmon)
                    {
                        if (!("follow" in val))
                    {
                            continue;
                        }

                        foreach (km in mis.km)
                        {
                            if (km.monid != val.monid)
                            {
                                continue;
                            }

                            if (km.cnt < val.cnt)
                            {
                                continue;
                            }

                            follow = val.follow;
                        }

                        break;
                    }
                }

                if ("kiler_awd" in conf.goal)
            {
                    pinfo.followka = 1;
                }

                if ("attmod" in conf.accept)
            {
                    foreach (att in conf.accept.attmod)
                    {
                        pinfo.misplatts.push(att);
                    }
                }
            }

            if (follow > 0)
            {
                pinfo.follow < -follow;

                if (bcast && old_follow != follow)
                {
                    // broad cast attchange msg
                    broad_cast_zone_msg(26, { iid = pinfo.iid, follow = pinfo.follow});
                }
            }
            else if (old_follow > 0)
            {
                delete pinfo.follow;
                if (bcast)
                {
                    // broad cast attchange msg
                    broad_cast_zone_msg(26, { iid = pinfo.iid, follow = 0});
                }
            }

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

            if (old_misatt_len != pinfo.misplatts.len())
            {
                // 有属性调整变化，重新计算属性

                if (bcast)
                {
                    this.re_calc_cha_data();
                }
            }
        }
        public bool has_follow()
        {
            return pinfo.follow != null;
        }
        public void has_mission_buf()
        {
            return (pinfo.misplatts.len() > 0);
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
        public void get_skil_data(skid)
        {
            if (!(skid in pinfo.skil.skills))
        {
                return null;
            }

            return pinfo.skil.skills[skid];
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
        //

        //public void modify_baseatt(name, att_add)
        //{
        //    // TO DO : 判断是否可修改属性，目前可修改属性有：stra,cona,intea,agia,wisa

        //    if (name in pinfo)
        //{
        //        //sys.trace(sys.SLT_DETAIL, "mod character baseatt["+name+"] att_add["+att_add+"]\n");

        //        pinfo[name] += att_add;

        //        //// send detail_info_change msg
        //        if (name == "att_pt")
        //        {
        //        ::send_rpc(pinfo.sid, 40, { pinfo ={ att_pt = pinfo.att_pt} } );
        //        }
        //        else
        //        {
        //            // 重新计算角色数据
        //            re_calc_cha_data( [name] );
        //        }
        //        return true;
        //    }

        //    return false;
        //}

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
        public void re_calc_cha_data(add_change_atts = null)
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
                    born_pos born_pos = gmap.get_born_area();

                    // 跳转至指定出生点
                    pinfo.x = Utility.random_section(born_pos.x, born_pos.w);
                    pinfo.y = Utility.random_section(born_pos.y, born_pos.h);
                    pinfo.map_id = born_pos.mpid;
                    pinfo.last_mvpts = null;

                    pinfo.lmpid = pinfo.map_id;
                    pinfo.lx = pinfo.x;
                    pinfo.ly = pinfo.y;

                    //::g_dump( "跳转至指定出生点 born_area:", born_area );


                    

                    if (game_ref.is_map_in_thr(pinfo.map_id))
                    {
                        gmap.rmv_player_from_map(game_ref, pinfo.sid);
                        game_ref.maps[pinfo.map_id].add_player_to_map(game_ref, pinfo.sid, this);
                    }
                    else
                    {
                        gmap.rmv_player_from_map(game_ref, pinfo.sid);

                        this.flush_db_data(false, false, false);
                        svr.readd_session(pinfo.sid);
                    }
                }
            }

            // send respawn msg
            //gmap.broadcast_map_rpc(21, this.pinfo);
            broad_cast_zone_msg_and_self(21, this.pinfo);
        }

        public void goto_worldmap(mpid, x, y)
        {
            // 跳转至大世界目标地图（地图必须是大世界地图）
            if (!gmap)
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

            if ("moving" in pinfo)
        {
                delete pinfo.moving;
            }

            if (this.is_in_lvl)
            {
                // 在副本中
                gmap.rmv_player_from_map(gmap.worldsvr, pinfo.sid);

                this.flush_db_data(false, false, false);
                level.enter_world(pinfo.sid, pinfo.line);
            }
            else
            {
                pinfo.lmpid = pinfo.map_id;
                pinfo.lx = pinfo.x;
                pinfo.ly = pinfo.y;

                // 在大世界中

                local game_ref = gmap.worldsvr;

                if (game_ref.is_map_in_thr(pinfo.map_id))
                {
                    gmap.rmv_player_from_map(game_ref, pinfo.sid);
                    game_ref.maps[pinfo.map_id].add_player_to_map(game_ref, pinfo.sid, this);
                }
                else
                {
                    gmap.rmv_player_from_map(game_ref, pinfo.sid);

                    this.flush_db_data(false, false, false);
                    svr.readd_session(pinfo.sid);
                }
            }

            return true;
        }

        //进入副本地图
        public void goto_levelmap(mpid, x, y)
        {
            if (!gmap)
            {
            // send begin chang map res
            ::send_rpc(pinfo.sid, 57, { res = game_err_code.PARAMETER_ERR});
                return false;
            }

            if (!this.is_in_lvl)
            {
            // send begin chang map res
            ::send_rpc(pinfo.sid, 57, { res = game_err_code.PARAMETER_ERR});
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

            local cur_clock_tm = DateTime.Now.ToBinary();
            update_pl_move(this, cur_clock_tm);

            // allow change

            // TO DO : 考虑目标地图人过多的情况

            // 停止移动
            if ("moving" in pinfo) delete pinfo.moving;

            gmap.rmv_player_from_map(gmap.worldsvr, pinfo.sid);

            pinfo.x = x;
            pinfo.y = y;
            pinfo.map_id = mpid;
            pinfo.last_mvpts = null;

            // bwantchangmap = true;

            in_lvl.maps[pinfo.map_id].add_player_to_map(this.in_lvl, pinfo.sid, this);

            return true;
        }

        public void check_att(attchk, att_adjust={ })
    {
        // TO DO : check player attribute
        if(!("name" in attchk))
        {
            return game_err_code.CONFIG_ERR;
        }

        if( attchk.name=="carr")
        {
            // 检查职业需求  每4位一个职业（其中低1位为需求职业，高3位为最少需求职业等级）
            local carr = (attchk.and >> ((pinfo.carr - 1) * 4)) & 0xf;
            if( carr & 0x1 )
            {
                if ( (carr>>1) <= pinfo.carrlvl )
                {
                    return game_err_code.RES_OK;
                }       
            }
            return game_err_code.QUALIFY_CARR_ERR;
        }
        else if(attchk.name=="achive")
        {
            // 检查是否有某个成就
            foreach (achive in pinfo.achives)
            {
                if(achive == attchk.have)
                {
                    return game_err_code.RES_OK;
                }
            }

            return game_err_code.QUALIFY_ACHIVE_ERR;
        }
        else if(attchk.name=="finmis")
        {// 检查是否完成某个任务
            local fined_mis = this.get_fined_mis(attchk.equal);
            if( fined_mis )
            {       
                return game_err_code.RES_OK;
            }

            return game_err_code.QUALIFY_FINMIS_ERR;
        }

        else if(!(attchk.name in pinfo))
        {
            return game_err_code.QUALIFY_SOME_ATT_ERR;
        }

        local pass = true;
local val = 0;
        if("equal" in attchk)
        {
            if((attchk.equal).tointeger() != pinfo[attchk.name])
            {
                pass = false;
            }
        }
        else
        {
            if("min" in attchk)
            {
                val = (attchk.min).tointeger();
                if( attchk.name in att_adjust )
                {
                    local adjust = att_adjust[attchk.name];
val = (val* adjust.mul/100.0).tointeger() + adjust.add;
                    //sys.trace( sys.SLT_DETAIL, "attchk.name =" + attchk.name + " val = " + val + "\n");
                }
                if( val > pinfo[attchk.name])
                {
                    pass = false;
                }
            }
            if("max" in attchk)
            {
                if((attchk.max).tointeger() < pinfo[attchk.name])
                {
                    pass = false;
                }
            }
        }
        if(!pass)
        {
            if(attchk.name=="sex")
            {
                return game_err_code.QUALIFY_SEX_ERR;
            }
            else if(attchk.name=="level")
            {
                return game_err_code.QUALIFY_LEVEL_ERR;
            }
            else if(attchk.name=="str")
            {
                return game_err_code.QUALIFY_STR_ERR;
            }
            else if(attchk.name=="con")
            {
                return game_err_code.QUALIFY_CON_ERR;
            }
            else if(attchk.name=="inte")
            {
                return game_err_code.QUALIFY_INTE_ERR;
            }
            else if(attchk.name=="agi")
            {
                return game_err_code.QUALIFY_AGI_ERR;
            }
            else if(attchk.name=="carrlvl")
            {
                return game_err_code.QUALIFY_CARRLVL_ERR;
            }
            else if(attchk.name=="resetlvl")
            {
                return game_err_code.QUALIFY_RESETLVL_ERR;
            }
            else if(attchk.name=="wis")
            {
                return game_err_code.QUALIFY_WIS_ERR;
            }
            else if(attchk.name=="lotcnt")
            {
                return game_err_code.QUALIFY_LOTCNT_ERR;
            }
            else if(attchk.name=="mlmisid")
            {
                return game_err_code.QUALIFY_MLMIS_ERR;
            }
            else
            {
                return game_err_code.QUALIFY_SOME_ATT_ERR;
            }
        }
        return game_err_code.RES_OK;
    }

public void check_pet(petchk)
{
    switch (petchk.name)
    {
        case "have": // 检查拥有宠物
            {
                if (petchk.pid == 0) // 检查是否拥有任意宠物
                {
                    if (pinfo.pets.pet.len() <= 0)
                    {
                        return game_err_code.NOT_OWN_ANY_PET;
                    }
                }
                else
                {
                    local own_pet = false;
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.tpid == petchk.pid)
                        {
                            own_pet = true;
                            break;
                        }
                    }

                    if (!own_pet)
                    {
                        return game_err_code.NOT_OWN_SUCH_PET;
                    }
                }
            }
            break;
        case "sklvl": // 检查宠物技能等级
            {
                // TO DO (sxx) : 实现检查宠物技能等级功能
                local petsklvl_chk = false;

                if (petchk.pid == 0) // 检查任意宠物
                {
                    // petchk.sklvl
                    foreach (pet in pinfo.pets.pet)
                    {
                        foreach (pskid in pet.skil)
                        {
                            local psk_conf = get_psk_conf(pskid);
                            // psk_conf.sklvl
                            if (psk_conf.sklvl >= petchk.sklvl)
                            {
                                petsklvl_chk = true;
                                return game_err_code.RES_OK;//直接返回跳出2层循环
                            }
                        }
                    }
                }
                else // 检查指定宠物
                {
                    //foreach(pet in pinfo.pets.pet)
                    //{
                    //}
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.tpid == petchk.pid)
                        {
                            foreach (pskid in pet.skil)
                            {
                                local psk_conf = get_psk_conf(pskid);
                                // psk_conf.sklvl
                                if (psk_conf.sklvl >= petchk.sklvl)
                                {
                                    petsklvl_chk = true;
                                    break;
                                }
                            }
                        }
                        if (petsklvl_chk)
                        {
                            break;
                        }
                    }
                }
                if (!petsklvl_chk)
                {
                    return game_err_code.PET_NO_REACH_SKILL_LVL;
                }
            }
            break;
        case "attper": // 检查宠物资质
            {
                // TO DO (sxx) : 实现检查宠物资质功能
                local petattper_chk = false;
                if (petchk.pid == 0) // 检查任意宠物
                {
                    // petchk.attper
                    foreach (pet in pinfo.pets.pet)
                    {
                        // pet.attper
                        if (pet.attper >= petchk.attper)
                        {
                            petattper_chk = true;
                            break;
                        }
                    }
                }
                else // 检查指定宠物
                {
                    //foreach(pet in pinfo.pets.pet)
                    //{
                    //}
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.tpid == petchk.pid)
                        {
                            if (pet.attper >= petchk.attper)
                            {
                                petattper_chk = true;
                                break;
                            }
                        }
                    }
                }
                if (!petattper_chk)
                {
                    return game_err_code.PET_ATTPER_REACH_REQ;
                }
            }
            break;
        case "plvl": // 检查宠物等级
            {
                if (petchk.pid == 0) // 检查任意宠物
                {
                    local pet_jude = false;
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.lvl >= petchk.lvl)
                        {
                            pet_jude = true;
                            break;
                        }
                    }

                    if (!pet_jude)
                    {
                        return game_err_code.PET_LVL_TOO_LOW;
                    }
                }
                else
                {
                    local pet_jude = false;
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.tpid == petchk.pid && pet.lvl >= petchk.lvl)
                        {
                            pet_jude = true;
                            break;
                        }
                    }

                    if (!pet_jude)
                    {
                        return game_err_code.PET_LVL_TOO_LOW;
                    }
                }
            }
            break;
        case "skils": // 检查宠物技能数
            {
                if (petchk.pid == 0) // 检查任意宠物
                {
                    local pet_jude = false;
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.skil.len() >= petchk.cnt)
                        {
                            pet_jude = true;
                            break;
                        }
                    }

                    if (!pet_jude)
                    {
                        return game_err_code.PET_SKIL_CNT_NOT_ENOUGH;
                    }
                }
                else
                {
                    local pet_jude = false;
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.tpid == petchk.pid && pet.skil.len() >= petchk.cnt)
                        {
                            pet_jude = true;
                            break;
                        }
                    }

                    if (!pet_jude)
                    {
                        return game_err_code.PET_SKIL_CNT_NOT_ENOUGH;
                    }
                }
            }
            break;
        case "rar": // 检查宠物稀有度
            {
                if (petchk.pid == 0) // 检查任意宠物
                {
                    local pet_jude = false;
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.rarity >= petchk.rar)
                        {
                            pet_jude = true;
                            break;
                        }
                    }

                    if (!pet_jude)
                    {
                        return game_err_code.PET_RARITY_NOT_ENOUGH;
                    }
                }
                else
                {
                    local pet_jude = false;
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.tpid == petchk.pid && pet.rarity >= petchk.rar)
                        {
                            pet_jude = true;
                            break;
                        }
                    }

                    if (!pet_jude)
                    {
                        return game_err_code.PET_RARITY_NOT_ENOUGH;
                    }
                }
            }
            break;
        case "qual": // 检查宠物品阶
            {
                if (petchk.pid == 0) // 检查任意宠物
                {
                    local pet_jude = false;
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.qual >= petchk.qual)
                        {
                            pet_jude = true;
                            break;
                        }
                    }

                    if (!pet_jude)
                    {
                        return game_err_code.PET_QUAL_NOT_ENOUGH;
                    }
                }
                else
                {
                    local pet_jude = false;
                    foreach (pet in pinfo.pets.pet)
                    {
                        if (pet.tpid == petchk.pid && pet.qual >= petchk.qual)
                        {
                            pet_jude = true;
                            break;
                        }
                    }

                    if (!pet_jude)
                    {
                        return game_err_code.PET_QUAL_NOT_ENOUGH;
                    }
                }
            }
            break;
    }

    return game_err_code.RES_OK;
}

public void check_skil(skilchk)
{
    switch (skilchk.name)
    {
        case "cnt": // 技能的数量
            {
                if (skilchk.carr == 0) // 拥有秘籍数量
                {
                    local cnt = 0;
                    foreach (skil in pinfo.skil.skills)
                    {
                        local skil_conf = get_skil_skill_desc(skil.skid);
                        if (!skil_conf)
                        {
                            continue;
                        }

                        if (("carr_id" in skil_conf) && skil_conf.carr_id != 0)
                        {
                            continue;
                        }
                        ++cnt;
                    }

                    if (cnt < skilchk.cnt)
                    {
                        return game_err_code.SKIL_SECSKIL_NOT_ENOUGH;
                    }
                }
                else // 拥有门派技能数量
                {
                    local cnt = 0;
                    foreach (skil in pinfo.skil.skills)
                    {
                        local skil_conf = get_skil_skill_desc(skil.skid);
                        if (!skil_conf)
                        {
                            continue;
                        }

                        if (!("carr_id" in skil_conf) || skil_conf.carr_id == 0)
                        {
                            continue;
                        }

                        ++cnt;
                    }

                    if (cnt < skilchk.cnt)
                    {
                        return game_err_code.SKIL_CARRSKIL_NOT_ENOUGH;
                    }
                }
            }
            break;
        case "have": // 是否拥有某个技能
            {
                if (skilchk.skid == 0) // 拥有任意技能
                {
                    if (skilchk.carr == 0) // 拥有任意密集
                    {
                        local skil_judg = false;
                        foreach (skil in pinfo.skil.skills)
                        {
                            local skil_conf = get_skil_skill_desc(skil.skid);
                            if (!skil_conf)
                            {
                                continue;
                            }

                            if (("carr_id" in skil_conf) && skil_conf.carr_id != 0)
                            {
                                continue;
                            }

                            skil_judg = true;

                            break;
                        }

                        if (!skil_judg)
                        {
                            return game_err_code.SKIL_NOT_LEARN_ANY_SECSKIL;
                        }
                    }
                    else // 拥有任意门派技能
                    {
                        local skil_judg = false;
                        foreach (skil in pinfo.skil.skills)
                        {
                            local skil_conf = get_skil_skill_desc(skil.skid);
                            if (!skil_conf)
                            {
                                continue;
                            }

                            if (!("carr_id" in skil_conf) || skil_conf.carr_id == 0)
                            {
                                continue;
                            }

                            skil_judg = true;

                            break;
                        }

                        if (!skil_judg)
                        {
                            return game_err_code.SKIL_NOT_LEARN_ANY_CARRSKIL;
                        }
                    }
                }
                else
                {
                    local skil_judg = false;
                    foreach (skil in pinfo.skil.skills)
                    {
                        if (skil.skid == skilchk.skid)
                        {
                            skil_judg = true;
                            break;
                        }
                    }

                    if (!skil_judg)
                    {
                        return game_err_code.SKIL_NOT_LEARN_SUCH_SKIL;
                    }
                }
            }
            break;
        case "spt": // 某个技能的强化值
            {
                if (skilchk.skid == 0) // 任意技能
                {
                    if (skilchk.carr == 0) // 任意密集
                    {
                        local skil_judg = false;
                        foreach (skil in pinfo.skil.skills)
                        {
                            local skil_conf = get_skil_skill_desc(skil.skid);
                            if (!skil_conf)
                            {
                                continue;
                            }

                            if (("carr_id" in skil_conf) && skil_conf.carr_id != 0)
                            {
                                continue;
                            }

                            if ((skil.spt / 10).tointeger() >= skilchk.spt)
                            {
                                skil_judg = true;
                                break;
                            }
                        }

                        if (!skil_judg)
                        {
                            return game_err_code.SKIL_SPT_NOT_ENOUGH;
                        }
                    }
                    else // 任意门派技能
                    {
                        local skil_judg = false;
                        foreach (skil in pinfo.skil.skills)
                        {
                            local skil_conf = get_skil_skill_desc(skil.skid);
                            if (!skil_conf)
                            {
                                continue;
                            }

                            if (!("carr_id" in skil_conf) || skil_conf.carr_id == 0)
                            {
                                continue;
                            }

                            if ((skil.spt / 10).tointeger() >= skilchk.spt)
                            {
                                skil_judg = true;
                                break;
                            }
                        }

                        if (!skil_judg)
                        {
                            return game_err_code.SKIL_SPT_NOT_ENOUGH;
                        }
                    }
                }
                else
                {
                    local skil_judg = false;
                    foreach (skil in pinfo.skil.skills)
                    {
                        if (skil.skid == skilchk.skid && skil.spt >= skilchk.spt)
                        {
                            skil_judg = true;
                            break;
                        }
                    }

                    if (!skil_judg)
                    {
                        return game_err_code.SKIL_SPT_NOT_ENOUGH;
                    }
                }
            }
            break;
        case "sklvl": // 某个技能的等级
            {
                if (skilchk.skid == 0) // 任意技能
                {
                    if (skilchk.carr == 0) // 任意密集
                    {
                        local skil_judg = false;
                        foreach (skil in pinfo.skil.skills)
                        {
                            local skil_conf = get_skil_skill_desc(skil.skid);
                            if (!skil_conf)
                            {
                                continue;
                            }

                            if (("carr_id" in skil_conf) && skil_conf.carr_id != 0)
                            {
                                continue;
                            }

                            if (skil.sklvl >= skilchk.lvl)
                            {
                                skil_judg = true;
                                break;
                            }
                        }

                        if (!skil_judg)
                        {
                            return game_err_code.SKIL_SKLVL_NOT_ENOUGH;
                        }
                    }
                    else // 任意门派技能
                    {
                        local skil_judg = false;
                        foreach (skil in pinfo.skil.skills)
                        {
                            local skil_conf = get_skil_skill_desc(skil.skid);
                            if (!skil_conf)
                            {
                                continue;
                            }

                            if (!("carr_id" in skil_conf) || skil_conf.carr_id == 0)
                            {
                                continue;
                            }

                            if (skil.sklvl >= skilchk.lvl)
                            {
                                skil_judg = true;
                                break;
                            }
                        }

                        if (!skil_judg)
                        {
                            return game_err_code.SKIL_SKLVL_NOT_ENOUGH;
                        }
                    }
                }
                else
                {
                    local skil_judg = false;
                    foreach (skil in pinfo.skil.skills)
                    {
                        if (skil.skid == skilchk.skid && skil.sklvl >= skilchk.lvl)
                        {
                            skil_judg = true;
                            break;
                        }
                    }

                    if (!skil_judg)
                    {
                        return game_err_code.SKIL_SKLVL_NOT_ENOUGH;
                    }
                }
            }
            break;
    }

    return game_err_code.RES_OK;
}

public void have_petcol(ptype)
{
    foreach (ptp in pinfo.petcol.pets)
    {
        if (ptype == ptp)
        {
            return true;
        }
    }
    return false;
}
public void have_petcolawd(id)
{
    foreach (awdid in pinfo.petcol.awds)
    {
        if (id == awdid)
        {
            return true;
        }
    }
    return false;
}

public void change_map(game_ref)
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

public void begin_change_map(game_ref, gto)
{
    if (!gmap)
    {
            // send begin chang map res
            ::send_rpc(pinfo.sid, 57, { res = game_err_code.PARAMETER_ERR});
        return;
    }

    if (bwantchangmap)
    {
        return;
    }

    local cur_clock_tm = DateTime.Now.ToBinary();
    //sys.trace(sys.SLT_ERR, "cur server_tm [" + cur_clock_tm +"]\n");
    update_pl_move(this, cur_clock_tm);

    local link = gmap.get_link_by_gto(gto);
    if (!link)
    {
        // 遍历查找动态添加链接点
        foreach (dlink in gmap.add_links)
        {
            //if(dlink.gto == gto)
            //{
            //    link  = dlink;
            //    break;
            //}

            if (dlink.gto != gto)
            {
                continue;
            }
            if (abs(dlink.x * game_const.map_grid_pixel + 16 - pinfo.x) > 300 || abs(dlink.y * game_const.map_grid_pixel + 16 - pinfo.y) > 300)
            {
                continue;
            }
            link = dlink;
        }

        if (!link)
        {
                // send begin chang map res
                ::send_rpc(pinfo.sid, 57, { res = game_err_code.MAP_LINK_NOT_EXIST});
            return;
        }
    }
    local tomap = link.to;
    local quit_lvl = false;
    //if(("lvl" in link) && (link.lvl > pinfo.level))
    //{
    //    ::send_rpc(pinfo.sid, 57, {res=game_err_code.MAP_LINK_LEVEL_REQUIRE});
    //    return;
    //}

    /*   
    if(abs(link.x*game_const.map_grid_pixel+16 - pinfo.x) > 600 || abs(link.y*game_const.map_grid_pixel+16 - pinfo.y) > 600)
    {
        // send begin chang map res
        ::send_rpc(pinfo.sid, 57, {res=game_err_code.MAP_LINK_NOT_NEAR_BY});
        return;
    }

    local tomap = link.to;
    local quit_lvl = false; 
    if(!game_ref.is_map_exist(tomap))
    {
        if(this.is_in_lvl)
        {
            quit_lvl = true;
        }
        else
        {   // send begin chang map res
            ::send_rpc(pinfo.sid, 57, {res=game_err_code.TARGET_MAP_NOT_EXIST});
            return;
        }
    }
    if( !check_map( this, tomap, 57 ) ) return;
    // allow change
    */
    // TO DO : 考虑目标地图人过多的情况

    // 停止移动
    if ("moving" in pinfo) delete pinfo.moving;

    gmap.rmv_player_from_map(game_ref, pinfo.sid);

    if ("rand_pos" in link )
        {
        local rand_pos = _n_choose_1(link.rand_pos);
        pinfo.x = rand_pos.to_x * game_const.map_grid_pixel + 16;
        pinfo.y = rand_pos.to_y * game_const.map_grid_pixel + 16;
    }
        else
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
        ::send_rpc(pinfo.sid, 57, { res = 1});
}


public void end_change_map(game_ref)
{
    if (!bwantchangmap)
    {
        return;
    }

    if (game_ref.is_map_in_thr(pinfo.map_id))
    {
        game_ref.maps[pinfo.map_id].add_player_to_map(game_ref, pinfo.sid, this);
    }
    else
    {
        if (!game_ref.is_map_exist(pinfo.map_id))
        {
            if (this.is_in_lvl)
            {
                //this.in_lvl.remove_player(pinfo.sid);
                this.flush_db_data(false, false, false);
                level.enter_world(pinfo.sid, pinfo.line);
            }
            else
            {
                sys.trace(sys.SLT_ERR, "ply change to map[" + pinfo.map_id + "] not exist!\n");
            }
        }
        else
        {
            this.flush_db_data(false, false, false);
            svr.readd_session(pinfo.sid);
        }
    }

    bwantchangmap = false;
}

public void is_got_awdact_awd(actid, tid )
{
    if ("awdact" in pinfo )
        {
        local key = actid.tostring();
        if (key in pinfo.awdact )
            {
            local got_tids = pinfo.awdact[key];
            foreach (got_tid in got_tids)
            {
                if (got_tid == tid) return true;
            }
        }
    }
    return false;
}

public void on_got_awdact_awd(actid, tid )
{
    local key = actid.tostring();
    if (!("awdact" in pinfo) )
        {
        pinfo.awdact < - {[key]=[tid]};
        }
        else
        {
            if( key in pinfo.awdact )
            {
                pinfo.awdact[key].push(tid);
            }
            else
            {
                pinfo.awdact[key] <- [tid];
            }
        }   
    }

    public void on_divorce()
{
    pinfo.marryid = 0;
    pinfo.mate_cid = 0;
    this.marry_lvl_conf = null;
    set_mate_iid(0);
}

public void on_petpk_rec(id)
{
    // pkrec: {cid, tcid, turn, vic, tm, rec}
    local rec = db_obj.select_data_obj("petpk", "id", id.tostring(), null);
    if (rec && rec.len() > 0)
    {
        _add_petpk_rec(rec[0]);
    }
    else
    {
        // 数据库中未找到对应id的数据
        sys.trace(sys.SLT_ERR, "petpk record not found, id:%d\n", id);
    }
}

public void _add_petpk_rec(petpk_db_data)
{
    local info = petpk_db_data.get_data();
    this.petpk_db[info.id] < - { info = info, data = petpk_db_data};

    // 删除超出的历史记录
    //local game_conf = get_general_game_conf();
    if (this.petpk_db.len() > game_conf.petf_self_rec_max)
    {
        local diff = this.petpk_db.len() - game_conf.petf_self_rec_max;
        //sys.trace(sys.SLT_DETAIL, "to delete:"+diff+"\n");
        local i;
        for (i = 0; i < diff; ++i)
        {
            local min_id = 0;
            local min_idx = 0;
            foreach (idx, val in this.petpk_db)
                {
            if (!min_id)
            {
                min_id = val.info.id;
                min_idx = idx;
                continue;
            }
            if (min_id > val.info.id)
            {
                min_id = val.info.id;
                min_idx = idx;
            }
        }
        this.petpk_db[min_idx].data.db_delete();
        delete this.petpk_db[min_idx];
    }
}

// 通知客户端
// 取角色名
//local dnm = _get_name_by_cid(info.tcid);

local rec;
        if(info.turn == 0)
        {
            rec = {fid=info.id, scid=info.cid, dcid=info.tcid, snm=info.rec.sinfo.name, dnm=info.rec.dinfo.name, rst=info.vic, rnk=info.rnk};
        }
        else
        {
            rec = {fid=info.id, scid=info.tcid, dcid=info.cid, snm=info.rec.sinfo.name, dnm=info.rec.dinfo.name, rst=info.vic, rnk=info.rnk};
        }

        ::send_rpc(this.pinfo.sid, 178, { tp = 3, list =[rec]});
    }

    //public void try_stop_recover()
    //{
    //    if(this.pinfo.recover > 0)
    //    {
    //        this.pinfo.recover = 0;
            
    //        // send recover_res to client
    //        this.broad_cast_zone_msg_and_self(47, this.pinfo);
    //    }
    //}

    //public void _update_act_v(cur_tm_s)
    //{
    //    local tm_dist = cur_tm_s - pinfo.act_v_add_tm;
    //    if(game_conf.act_v_add_tm <= tm_dist)
    //    {
    //        if(pinfo.act_v < game_conf.max_act_v)
    //        {
    //            pinfo.act_v += (tm_dist / game_conf.act_v_add_tm).tointeger();
    //            if(pinfo.act_v > game_conf.max_act_v) pinfo.act_v = game_conf.max_act_v;

    //            pinfo.act_v_add_tm = cur_tm_s - (tm_dist%game_conf.act_v_add_tm);

    //            return true;
    //        }
    //        else
    //        {
    //            pinfo.act_v_add_tm = cur_tm_s;
    //        }
    //    }

    //    return false;
    //}

    public void _try_update_w_hexp(cur_tm_s)
{
    if (pinfo.hexpt <= pinfo.last_w_hexpt)
    {
        return;
    }

    local cur_local_tm = sys.trans_local_time(cur_tm_s);
    local cw_hexpt_local_tm = sys.trans_local_time(pinfo.cw_hexpt_tm);

    if (!_is_same_week(cur_local_tm, cw_hexpt_local_tm))
    {
        pinfo.cw_hexpt = 0;
        pinfo.last_w_hexpt = pinfo.hexpt;
        pinfo.cw_hexpt_tm = cur_tm_s;
        return;
    }

    local hexp_add = pinfo.hexpt - pinfo.last_w_hexpt;
    pinfo.last_w_hexpt = pinfo.hexpt;

    pinfo.cw_hexpt += hexp_add;

    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_W_HEXP_RNK_INFO, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local whexp_data = _get_w_hexp_rnk_data();
    if (whexp_data)
    {
        local whexp_info = whexp_data.get_data();
        if (whexp_info)
        {
            local changed = false;

            local idx = whexp_info.data.cw_info.len() - 1;
            for (; idx >= 0; --idx)
            {
                local whexp = whexp_info.data.cw_info[idx];
                if (whexp.hexpt > pinfo.cw_hexpt)
                {
                    break;
                }

                if (whexp.cid == pinfo.cid)
                {
                    whexp_info.data.cw_info.remove(idx);
                    continue;
                }
            }

            if (idx < game_conf.w_hexp_rnk_cnt - 1)
            {
                whexp_info.data.cw_info.insert(idx + 1, { cid = pinfo.cid, nm = pinfo.name, hexpt = pinfo.cw_hexpt});
                changed = true;
            }

            while (whexp_info.data.cw_info.len() > game_conf.w_hexp_rnk_cnt)
            {
                whexp_info.data.cw_info.pop();
                changed = true;
            }

            if (changed)
            {
                whexp_data.mod_data(whexp_info);
                //whexp_data.db_update();
            }
        }
    }
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_W_HEXP_RNK_INFO);  // 解锁
}


public void update(game_ref, cur_clock_tm, tm_elasped_s)
{
    //local game_conf = get_general_game_conf();

    local cur_tm_s = sys.time(); // TO DO : use time manager

    if (cur_tm_s - last_cal_log_day_tm > 600)
    {   //每小时更新一次
        last_cal_log_day_tm = cur_tm_s;
        local cur_tm_obj = sys.trans_local_time(cur_tm_s);
        //重置竞技连胜次数
        if (pinfo.oldday != cur_tm_obj.wd)
        {   //当前已经不是同一天了，更新
            pinfo.oldday = cur_tm_obj.wd;
            //重置竞技连胜次数
            pinfo.arena_cwin_y = pinfo.arena_cwin;
            pinfo.bs_arena_pt_y = pinfo.bs_arena_pt;

            pinfo.arena_cwin = 0;
            pinfo.arena_cwin_gotawd.clear();
        }
    }

    if (pinfo.bsinfo && pinfo.bsinfo.do_tp == bsc_do_state.BSCDO_NONE && pinfo.bsinfo.state != bsc_join_state.BSCJS_NONE)
    {
        if (pinfo.bsinfo.waitdo.len() <= 0)
        {
            if (!("dotm" in pinfo.bsinfo) || (pinfo.bsinfo.dotm + 120) < cur_tm_s )
                {//没有参加过跨服战的
                this.reset_bsinfo();
                bsclient.del_client(pinfo.sid);
            }
        }
    }

    if (pinfo.pk_v > 0)
    {
        if (cur_tm_s - pinfo.pk_v_revtm > game_conf.pk_v_revtm) // 1小时减少1点
        {
            pinfo.pk_v_revtm = cur_tm_s;
            this.sub_pk_v(1);
        }
    }

    //正当防卫 更新
    foreach (atker_cid, tm in defend_tm_map )
        {
        if (tm <= cur_tm_s)
        {
            delete defend_tm_map[atker_cid];


                ::send_rpc(pinfo.sid, 2, { defend = false,cid = atker_cid});
            local be_defend_sid = svr.get_sid_by_cid(atker_cid);
            if (be_defend_sid > 0)
            {
                     ::send_rpc(be_defend_sid, 2, { be_defend = false,cid = pinfo.cid});
            }
        }
    }

    // 统计在线奖励领取时间
    if (pinfo.olawd.tmleft > 0)
    {
        pinfo.olawd.tmleft -= tm_elasped_s;
        if (pinfo.olawd.tmleft < 0) pinfo.olawd.tmleft = 0;
    }

    if (this.allow_respawn_tm_s > 0)
    {
        this.allow_respawn_tm_s -= tm_elasped_s;
        if (this.allow_respawn_tm_s < 0) this.allow_respawn_tm_s = 0;
    }

    // 判断vip是否过期
    this._check_vip_expire(cur_tm_s);

    // TO DO : 优化队伍、帮派信息同步方式，无需频繁轮询调用

    // 同步队伍信息
    local tid = team.get_ply_teamid(pinfo.cid);
    if (tid > 0)
    {
        if (brejointeam)
        {
            // 下线后上线重新加入队伍

            // 发送队伍信息
            local members = team.get_members(tid);
            local idx = 0;
            for (idx = 0; idx < members.len(); ++idx)
            {
                if (members[idx].cid == pinfo.cid)
                {
                    members.remove(idx);
                    break;
                }
            }
            local reqs = team.get_join_reqs(tid);
            local leader_cid = team.get_leader_cid(tid);
            //local dir_join = team.is_dir_join(tid);
            //local memb_inv = team.is_memb_inv(tid);
            local tinfo = team.get_team_info(tid);
            if (!tinfo)
            {
                tinfo = { dir_join = false, memb_inv = memb_inv, pubed_ltpid = pubed_ltpid, pubed_ldiff = pubed_ldiff};
            }
                ::send_rpc(pinfo.sid, 133, { plys = members, join_reqs = reqs, leader_cid = leader_cid, tid = tid, dir_join = tinfo.dir_join, memb_inv = tinfo.memb_inv, ltpid = tinfo.pubed_ltpid, ldiff = tinfo.pubed_ldiff});

            brejointeam = false;
        }

        if (!("teamid" in pinfo) || pinfo.teamid != tid)
            {
            // 新加入队伍

            // 队内广播新用户加入消息
            team_bcast_rpc_except(tid, 134, pinfo, pinfo.sid);

            // 发送队伍信息
            local members = team.get_members(tid);
            local idx = 0;
            for (idx = 0; idx < members.len(); ++idx)
            {
                if (members[idx].cid == pinfo.cid)
                {
                    members.remove(idx);
                    break;
                }
            }
            local reqs = team.get_join_reqs(tid);
            local leader_cid = team.get_leader_cid(tid);
            //local dir_join = team.is_dir_join(tid);
            //local memb_inv = team.is_memb_inv(tid);
            local tinfo = team.get_team_info(tid);
                ::send_rpc(pinfo.sid, 133, { plys = members, join_reqs = reqs, leader_cid = leader_cid, tid = tid, dir_join = tinfo.dir_join, memb_inv = tinfo.memb_inv, ltpid = tinfo.pubed_ltpid, ldiff = tinfo.pubed_ldiff});

            // 广播队伍变化消息
            this.broad_cast_zone_msg(136, { cid = pinfo.cid, teamid = tid});

            this.set_team_id(tid);
            pinfo.teamid < -tid;

            // 隐身队友入队
            team_invisible_ply_join(tid, this);
        }
    }
    else if ("teamid" in pinfo && pinfo.teamid > 0)
        {
        // 离开队伍
        local old_tid = pinfo.teamid;

            // 通知客户端离开队伍
            ::send_rpc(pinfo.sid, 124, pinfo);

        // 队内广播用户离开消息
        team_bcast_rpc(pinfo.teamid, 124, pinfo);

        // 广播队伍变化消息
        this.broad_cast_zone_msg(136, { cid = pinfo.cid, teamid = 0});

        this.set_team_id(0);
        delete pinfo.teamid;

        if (this.is_in_lvl)
        {
            // 在副本中
            local linfo = level.get_lvl_info(pinfo.llid);
            if (linfo && linfo.lmtp == level_multi_type.LMT_TEAM)
            {
                // 组队副本, 需把用户从副本中踢出
                local lvl = this.gmap.worldsvr;
                lvl.remove_player(pinfo.sid);
                level.enter_world(pinfo.sid, pinfo.line);
            }
        }

        // 隐身队友离队
        team_invisible_ply_leave(old_tid, this);
    }

    // 同步帮派信息
    local clanid = clanmgr.get_pl_clanid(pinfo.cid);
    if (clanid > 0 && (!("clanid" in pinfo) || pinfo.clanid != clanid))
        {
        pinfo.clanid < -clanid;
        ++pinfo.rev;
    }
        else if (clanid == 0 && ("clanid" in pinfo))
        {
        delete pinfo.clanid;
        ++pinfo.rev;
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

    if (!gmap)
    {
        return;
    }

    if (gmap.map_fined)
    {
        sys.trace(sys.SLT_ERR, "err:gmap finished player cid=[" + this.pinfo.cid + "] mapid=[" + this.gmap.mapid + "]\n");
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
        this.broad_cast_zone_msg_and_self(26, { iid = pinfo.iid, in_pczone = this.pinfo.in_pczone});
    }

    update_pl_state(cur_clock_tm, this);
    _update_pl_bstate(cur_tm_s, this);

    local is_moving_before = false;
    if ("moving" in this.pinfo)
        {
        is_moving_before = true;
    }

    local tm_left = update_pl_move(this, cur_clock_tm);
    if (!("moving" in this.pinfo))
        {
        // player stoped move
        if ("atking" in this.pinfo)
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

    update_pl_atk(this, cur_clock_tm);

    //if("atking" in this.pinfo)
    //{
    //    this.pinfo.atking.trace_tm_left = 0;
    //    update_pl_atk_tracing(this, cur_clock_tm);
    //}

    //meri_update_pl_acupup(this, cur_tm_s);
    skil_update_pl_skilup(this, cur_tm_s);
    trade_update_trade_req(this, cur_tm_s);
    pet_update_pet(this, cur_tm_s);

    // 更新角色行动力值
    //local act_v_updated = _update_act_v(cur_tm_s);
    //if(act_v_updated)
    //{
    //    // 改变了
    //    ::send_rpc(pinfo.sid, 32, {act_v=pinfo.act_v});
    //}

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

    if (this.pinfo.next_hpmpr_tm < cur_tm_s)
    {
        if (!pinfo.isdie)
        {
            if (pinfo.hpr > 0 || pinfo.dprv > 0)
            {
                local hpchange = { };
                // +血
                if (pinfo.hpr > 0 && this.pinfo.hp < this.pinfo.max_hp)
                {
                    local hp_add = (this.pinfo.max_hp * pinfo.hpr / 1000.0).tointeger();
                    this.pinfo.hp += hp_add;
                    if (this.pinfo.hp > this.pinfo.max_hp) this.pinfo.hp = this.pinfo.max_hp;

                    hpchange.hpadd < -hp_add;
                    hpchange.die < -false;
                }

                // +防护值
                if (pinfo.dprv > 0 && this.pinfo.dp < this.pinfo.max_dp)
                {
                    local dp_add = pinfo.dprv;
                    this.pinfo.dp += dp_add;
                    if (this.pinfo.dp > this.pinfo.max_dp) this.pinfo.dp = this.pinfo.max_dp;

                    hpchange.dpadd < -dp_add;
                }

                if (hpchange.len() > 0)
                {
                    // broadcast hpchange msg
                    this.broad_cast_zone_msg_and_self(26, { iid = pinfo.iid, hpchange = hpchange});
                }
            }

            if (pinfo.mpr > 0)
            {
                // +魔
                if (this.pinfo.mp < this.pinfo.max_mp)
                {
                    local mp_add = (this.pinfo.max_mp * pinfo.mpr / 1000.0).tointeger();
                    this.pinfo.mp += mp_add;
                    if (this.pinfo.mp > this.pinfo.max_mp) this.pinfo.mp = this.pinfo.max_mp;

                        // send mpchange msg
                        ::send_rpc(pinfo.sid, 32, { mpadd = mp_add});
                }
            }
        }
        this.pinfo.next_hpmpr_tm = cur_tm_s + game_conf.hpmpr_tm;
    }

    // 检查定时任务
    if (check_tmmis_tm < cur_tm_s)
    {
        local changed = false;
        for (local mis_idx = 0; mis_idx < this.pinfo.misacept.len(); ++mis_idx)
        {
            local acept_mis = this.pinfo.misacept[mis_idx];
            // 检查任务时间是否超过
            if ("fintm" in acept_mis)
                {
            if (acept_mis.fintm < cur_tm_s)
            {
                        // 放弃任务

                        // send abord_mis_res msg
                        ::send_rpc(pinfo.sid, 115, { res = game_err_code.RES_OK, misid = acept_mis.misid});

                if ("cntleft" in acept_mis)
                        {
                    local fined_mis = this.get_fined_mis(acept_mis.misid);
                    if (fined_mis)
                    {
                        fined_mis.cntleft = acept_mis.cntleft;
                    }
                    else
                    {
                        if ("km" in acept_mis) delete acept_mis.km;
                        if ("kp" in acept_mis) delete acept_mis.kp;
                        if ("pzkp" in acept_mis) delete acept_mis.pzkp;
                        if ("pzckp" in acept_mis) delete acept_mis.pzckp;
                        if ("fintm" in acept_mis) delete acept_mis.fintm;
                        // 记录任务完成情况
                        this.pinfo.misfined[acept_mis.misid] < -acept_mis;
                    }
                }

                local mis_conf = get_mis_conf(acept_mis.misid);

                // 删除任务保险
                if (_rmv_mis_insure(pinfo, mis_conf.id))
                {
                            // send mis insure msg
                            ::send_rpc(pinfo.sid, 108, { tp = 6, insure_misids = pinfo.mis_insures});
                }

                _rmv_mis_appawd(pinfo, mis_conf.id);

                // 移除已接任务
                this.pinfo.misacept.remove(mis_idx);
                --mis_idx;

                changed = true;
            }
        }
    }

    if (changed)
    {
        // 检查护送任务
        this.check_tsk_attribute(true);
    }

    // 检查时效称号是否超时
    foreach (achive_id, tm in pinfo.achivetm)
            {
        if (tm < cur_tm_s)
        {
            // 当前称号
            if (this.pinfo.achbytp.len() > 0)
            {
                this.unact_achive(achive_id);
            }

            // 移除称号
            delete pinfo.achivetm[achive_id];
            foreach (idx, val in this.pinfo.achives)
                    {
                if (val == achive_id)
                {
                    //sys.trace(sys.SLT_DETAIL, "achive["+achive_id+"] timeout!\n");
                    this.pinfo.achives.remove(idx);
                    break;
                }
            }
                    ::send_rpc(this.pinfo.sid, 35, { rmv_achive = achive_id});   // send remove achive msg
        }
    }

    // 检查帮派状态是否超时
    local calc_chadata = false;
    for (local idx = 0; idx < this.pinfo.clanstat.len(); ++idx)
    {
        local clstat = this.pinfo.clanstat[idx];
        if (clstat.end_tm > cur_tm_s)
        {
            continue;
        }

        // 到时间，结束状态
        this.pinfo.clanstat.remove(idx);
        --idx;

        calc_chadata = true;

                // send self_attchange msg
                ::send_rpc(this.pinfo.sid, 32, { rmv_clstat = clstat.id});
}

            //时效性装备
            for(local idx = this.pinfo.expire_eqp.len() - 1; idx>=0 ; --idx)
            {
                local eqp = this.pinfo.expire_eqp[idx];
                if ( eqp.expire<cur_tm_s )
                {
                    calc_chadata = true;
                    this.pinfo.expire_eqp.remove( idx );
}
            }             

            if(calc_chadata)
            {
                // 重新计算角色属性
                this.re_calc_cha_data();
}

check_tmmis_tm = cur_tm_s + 2;  // 每2秒检查一次
        }

        if(cur_tm_s > check_ol_tm + 60) // 60 秒一次
        {
            // 尝试更新每日必做任务
            _try_up_dmis(this, "oltm", cur_tm_s-check_ol_tm);
check_ol_tm = cur_tm_s;

            // 尝试更新每周荣誉排行榜
            _try_update_w_hexp(cur_tm_s);
        }

        if(this.bcast_achive_ary.len() > 0)
        {
    // 广播具备牛b成就玩家上线消息
    _broad_cast_sys_msg({ cid = this.pinfo.cid, name = this.pinfo.name, tp = bcast_msg_tp.ACHIVE_PLY_ONLINE, achives = this.bcast_achive_ary});
    this.bcast_achive_ary.clear();
}

        if( this.bcast_ol_noblv > 0 )
        {
    // 广播具备牛b成就玩家上线消息
    _broad_cast_sys_msg({ cid = this.pinfo.cid, name = this.pinfo.name, tp = bcast_msg_tp.NOBILITY_PLY_ONLINE, par = bcast_ol_noblv});
    this.bcast_ol_noblv = 0;
}

        if(this.kp_asist_rec)
        {
    if (cur_tm_s - this.lastdmgtm > 3)
    {
        // 3秒内未被攻击，清理助攻列表
        this.beatk_ply_cids = { };
    }
}

        if(pinfo.pet_mon.mid > 0)
        {
    if (pinfo.pet_mon.fintm <= cur_tm_s)
    {
        // 战斗宠物的时间到了
        release_petmon(false);
    }
}
}


public void get_resetlvl_pt_add_total()
{
    local total_pt = 0;

    for (local i = 1; i <= pinfo.resetlvl; i++)
    {
        if (!(i in game_data_conf.general.resetlvl ) ) continue;
    local resetlvl_conf = game_data_conf.general.resetlvl[i];
    total_pt += resetlvl_conf.attpt_add;
}
        return total_pt;
    }

    public void pvip_power_refresh()
{
    local tminfo = pvip_pow_activity_tm();
    if (is_pvip_pow_activity())
    {
        if (pinfo.pvip_power.expire < tminfo.st)
        {//原有数据处理 过期  
            pvip_power_reset();
        }
    }
    else
    { //不在活动时间           
        pvip_power_reset();
    }
    pinfo.pvip_power.sttm = tminfo.st;
    pinfo.pvip_power.expire = tminfo.ed;
}

public void pvip_power_reset()
{//黄钻能量默认数据
    pinfo.pvip_power < - {
        pt = 0,
            pt_curr = 0,
            sttm = 0,
            expire = 0,
            recored = [],
        }
    user_info.pvip_power < -pinfo.pvip_power;
}

//--------------------------------------翅膀技能 start----------------------------------------------------------------------------
public void set_wing_state_sel(pos, state )
{
    pinfo.wingholes.actstates[pos] < -state;
}

public void on_hurt(tar_sprite )
{   //当攻击到别的目标时 触发的被动
    // if(!this.has_wing()) return;

    foreach (pos,state in pinfo.wingholes.actstates)
        {
        local state_conf = get_wing_stat_conf(state.sid);
        if (!state_conf)
        {
                ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});
            return;
        }
        if (("atk" in state_conf ) && state_conf.atk > 0 )
            {   //攻击类型状态技能
            on_mount_st_trig(state, tar_sprite, state_conf, false);
        }
    }
}

public void on_be_hurt(frm_sprite )
{   //当被攻击时 触发的被动  
    // if(!this.has_wing()) return;

    foreach (pos,state in pinfo.wingholes.actstates)
        {
        local state_conf = get_wing_stat_conf(state.sid);
        if (!state_conf)
        {
                ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});
            return;
        }
        if (("def" in state_conf ) && state_conf.def > 0 )
            {   //防御类型状态技能
            on_mount_st_trig(state, frm_sprite, state_conf, true);
        }
    }
}

public void on_mount_st_trig(state, trigger_sprite, state_conf, be_hurt )
{//当攻击到别的目标时 触发的被动        

    local curr_svr_tm = sys.time();
    local cur_clock_tm = DateTime.Now.ToBinary();

    if (("expire" in state ) )
        {//cd中
        if (sys.time() < state.expire)
        {//cd中
            return;
        }
    }

    local judg = Utility.random(0, 1000);

    if (judg > state_conf.rate) return;


    local add_self = true;
    if (("add_trigger" in state_conf) && state_conf.add_trigger > 0 )
        {
        add_self = false;
    }
    if (add_self)
    {
        if (get_state_exsit(state_conf.tar_state)) return;
    }
    else
    {
        if (trigger_sprite.get_state_exsit(state_conf.tar_state)) return;
    }

    local res = null;
    local iid = 0;
    local add_sprite = null;
    if (!add_self)
    {
        if (trigger_sprite)
        {
            add_sprite = trigger_sprite;
            res = add_state_to_pl(cur_clock_tm, trigger_sprite, state_conf, this, 1000, false);
            if (trigger_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
            {
                iid = trigger_sprite.pinfo.iid;
            }
            else
            {
                iid = trigger_sprite.mondata.iid;
            }
        }
    }
    else
    {
        add_sprite = this;
        res = add_state_to_pl(cur_clock_tm, this, state_conf, this, 1000, false);
        iid = pinfo.iid;
    }

    if ("cd" in state_conf )
        {
        state.expire < -(curr_svr_tm + (state_conf.cd * 100) / 1000);
    }

    if (res)
    {
        if (add_sprite) add_sprite.re_calc_cha_data();
        broad_cast_zone_msg_and_self(24, { iid = iid,  states =[res] } );
    }
}

public void get_state_exsit(sid )
{
    if (!("states" in pinfo ) || !("state_par" in pinfo.states) ) 
        {
        return null;
    }
    foreach (state in pinfo.states.state_par)
    {
        if (state.id == sid)
        {
            return state;
        }
    }
    return null;
}



    //--------------------------------------翅膀技能 end----------------------------------------------------------------------------

}