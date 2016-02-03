using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
    public class group_map
    {
        public int gpid
        {
            get; set;
        }
        public Dictionary<int, grid_map> maps
        {
            get; set;
        }

        public Variant kmfin
        {
            get; set;
        }
        public bool tmfin { get; set; }
        public Dictionary<int, IBaseUnit> plys { get; set; }
        public int plycnt { get; set; }
        public bool fin { get; set; }
        public bool closed { get; set; }
        public int win { get; set; }

        public group_map()
        {
            maps = new Dictionary<int, grid_map>();
        }
    }
    public class Level : IService
    {
        public Dictionary<int, IBaseUnit> sgplayers = new Dictionary<int, IBaseUnit>();
        public Dictionary<int, IBaseUnit> sgplayersbycid = new Dictionary<int, IBaseUnit>();
        public Dictionary<int, IBaseUnit> sgplayersbyside = new Dictionary<int, IBaseUnit>();
        public Dictionary<int, grid_map> maps = new Dictionary<int, grid_map>();
        public void on_dmg(IBaseUnit frm_sprite, IBaseUnit tar_sprite, double dmg)
        {

        }



        function _calc_clantechmon_att(clanid, sideid)
        {
            this.clanmon_aff = true; // 需要计算帮派副本攻防加成
            this.clanmon_att_add = { clanid = clanid, sideid = sideid, atts ={ } };

            local clan_mgr = global_data_mgrs.clan;
            clan_mgr.lock_data(clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

            local clan_db_data = _get_clan_data(clanid);
            if (clan_db_data.Count <= 0)
            {
                clan_mgr.unlock_data(clanid); // 解锁
                return;
            }

            local clan_db_info = clan_db_data[0].get_data();

            clan_mgr.unlock_data(clanid); // 解锁

            foreach (cur_tech in clan_db_info.tech.techs)
            {
                if (cur_tech.lvl <= 0)
                {
                    continue;
                }

                local tech_conf = get_clantech_conf(cur_tech.id);
                if (!tech_conf)
                {
                    continue;
                }

                local tech_func_aff = tech_conf.func[0].aff[0];
                if (tech_func_aff.name != "att")
                {
                    continue;
                }

                if (!("clanmon" in tech_func_aff))
            {
                    continue;
                }

                local tech_lvl_conf = get_clantechlvl_conf(cur_tech.lvl);
                if (("clan_tech_lvl" in tech_conf) && (cur_tech.lvl in tech_conf.clan_tech_lvl))
            {
                    tech_lvl_conf = tech_conf.clan_tech_lvl[cur_tech.lvl];
                }
                if (!tech_lvl_conf)
                {
                    continue;
                }

                if (!(tech_func_aff.clanmon in this.clanmon_att_add.atts))
            {
                    this.clanmon_att_add.atts[tech_func_aff.clanmon] < - [];
                }
                this.clanmon_att_add.atts[tech_func_aff.clanmon].push({ att = tech_func_aff.att, per = 100 + tech_lvl_conf.affper});
            }
        }

        function _calc_clantechply_att(clanid)
        {
            if (!(clanid in this.clanply_att_add))
        {
                this.clanply_att_add[clanid] < - [];
            }

            local clan_mgr = global_data_mgrs.clan;
            clan_mgr.lock_data(clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

            local clan_db_data = _get_clan_data(clanid);
            if (clan_db_data.Count <= 0)
            {
                clan_mgr.unlock_data(clanid); // 解锁
                return null;
            }

            local clan_db_info = clan_db_data[0].get_data();

            clan_mgr.unlock_data(clanid); // 解锁

            foreach (cur_tech in clan_db_info.tech.techs)
            {
                if (cur_tech.lvl <= 0)
                {
                    continue;
                }

                local tech_conf = get_clantech_conf(cur_tech.id);
                if (!tech_conf)
                {
                    continue;
                }

                local tech_func_aff = tech_conf.func[0].aff[0];
                if (tech_func_aff.name != "att")
                {
                    continue;
                }

                if (!("plyact" in tech_func_aff) || tech_func_aff.plyact != 0)
            {
                    continue;
                }

                local tech_lvl_conf = get_clantechlvl_conf(cur_tech.lvl);
                if (("clan_tech_lvl" in tech_conf) && (cur_tech.lvl in tech_conf.clan_tech_lvl))
            {
                    tech_lvl_conf = tech_conf.clan_tech_lvl[cur_tech.lvl];
                }
                if (!tech_lvl_conf)
                {
                    continue;
                }

                this.clanply_att_add[clanid].push({ att = tech_func_aff.att, per = 100 + tech_lvl_conf.affper});
            }
        }



        function _on_triger_addmons(added_mon)
        {
            if (this.diff_lvl in this.lvl_conf.diff_lvl)
        {
                // 根据diff_lvl等级初始化新增怪物等级

                local diff_lvl_conf = this.lvl_conf.diff_lvl[this.diff_lvl];

                // 增加地图怪物等级
                if ("mon_lvl_add" in diff_lvl_conf && diff_lvl_conf.mon_lvl_add > 0)
            {
                    foreach (mon in added_mon)
                    {
                        mon.setlevel(mon.mondata.level + diff_lvl_conf.mon_lvl_add);
                    }
                }
            }

            if (this.clanmon_aff)
            {
                // 帮派怪物加成
                foreach (mon in added_mon)
                {
                    if (mon.mondata.lvlsideid != this.clanmon_att_add.sideid)
                    {
                        continue;
                    }

                    if (!("clanmon" in mon.monconf))
                {
                        continue;
                    }

                    if (!(mon.monconf.clanmon in this.clanmon_att_add.atts))
                {
                        continue;
                    }

                    mon.setclanatts(this.clanmon_att_add.atts[mon.monconf.clanmon]);
                }
            }
        }


        function player_respawn(ply )
        {
            local pinfo = ply.pinfo;
            if (pinfo.lvlsideid > 0)
            {
                // 在阵营中
                local side_conf = this.sides_conf[pinfo.lvlsideid];

                pinfo.map_id = side_conf.born[0].mpid;
                pinfo.x = sys.rand(side_conf.born[0].x, side_conf.born[0].width);
                pinfo.y = sys.rand(side_conf.born[0].y, side_conf.born[0].height);
                pinfo.last_mvpts = null;
            }
            else
            {
                pinfo.map_id = this.born_map_conf[0].mpid;
                pinfo.x = sys.rand(this.born_map_conf[0].x, this.born_map_conf[0].width);
                pinfo.y = sys.rand(this.born_map_conf[0].y, this.born_map_conf[0].height);
                pinfo.last_mvpts = null;
            }

            ply.gmap.rmv_player_from_map(this, pinfo.sid);
            if (this.gpmap_maxply > 0)
            {
                local gp_map = this.gp_maps[this.cid2gpid[pinfo.cid] - 1];
                gp_map.maps[pinfo.map_id].add_player_to_map(this, pinfo.sid, ply);
            }
            else
            {
                this.maps[pinfo.map_id].add_player_to_map(this, pinfo.sid, ply);
            }
        }




        function ply_change_map(ply, map)
        {
            if (map.mapid in this.maps)
        {
                if (this.tm_out && (ply.pinfo.cid in this.joininfo))
            {
                    local cur_tm_s = sys.time();
                    local info = this.joininfo[ply.pinfo.cid];
                    if (info.mapid != map.mapid)
                    {   //该玩家切换地图了
                        info.join_tm = cur_tm_s;
                        info.mapid = map.mapid;
                    }
                }
                if (map.mapid in enter_map_plys)
            {
                    local old_enter_plys = enter_map_plys[map.mapid];
                    local haspush = false;
                    foreach (oldply in old_enter_plys)
                    {
                        if (oldply.cid == ply.pinfo.cid)
                        {
                            haspush = true;
                            break;
                        }
                    }
                    if (!haspush)
                    {
                        enter_map_plys[map.mapid].push({ cid = ply.pinfo.cid,give = false});
                        _give_joinlvl_awd(ply);
                    }
                }
            else
            {
                    enter_map_plys[map.mapid] < - [{ cid = ply.pinfo.cid,give = false}];
                    _give_joinlvl_awd(ply);
                }
            }
        }

        function get_player_ghost_cnt(cid)
        {
            if (cid in this.ghost_players)
        {
                return this.ghost_players[cid];
            }
            return -1;
        }

        function is_arena()
        {
            return ("arenaid" in lvl_conf && lvl_conf.arenaid > 0);
        }
        function is_carrchief()
        {
            return ("carrchief" in lvl_conf && lvl_conf.carrchief > 0);
        }

        function is_lvlmis_lvl()
        {
            // 是否为侠客行副本
            return (null != get_lvlmis_conf(this.ltpid));
        }

      

    function lvl_finish(cur_tm_s)
{
    if (this.state == level_state_type.LST_FINED)
    {
        return;
    }

    this.state = level_state_type.LST_FINED;
    this.close_tm = cur_tm_s + 30;

    if (game_data_conf.is_battle_svr)
    {
        bsserver_do_lvl_finish(cur_tm_s, this);
    }
    else
    {
        _lvl_finish(cur_tm_s);
    }
}


local clanid = clanter_info.clanid;
            if( 0 == clanid )
            {
                Utility.trace_err( "cltwar clanid = "+clanid+"!\n");
            }
            else
            {
                local infos = [];
local clan_mgr = global_data_mgrs.clan;
clan_mgr.lock_data(clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
                local total_cnt = clanmgr.get_clanpl_cnt(clanid);
local clanpls = clanmgr.get_clanpls(clanid, 0, total_cnt);
                foreach(clanpl in clanpls)
                {
                    local clanpl_info = clanpl.data.get_data();
                    if(clanpl_info)
                    {
                        infos.push( {cid=clanpl_info.cid, carr=clanpl_info.carr, level=clanpl_info.lvl, name=clanpl_info.name, 
                        carrlvl=clanpl_info.carrlvl, awdtm=cur_tm_s} );
                    }
                }
                clan_mgr.unlock_data(clanid); // 解锁
                
                clanter_info.showinfo<- infos;
                foreach(info in infos)
                {
                    foreach(ply in sgplayersbycid)
                    {
                        local pinfo = ply.pinfo;
                        if(pinfo.cid == info.cid)
                        {
                            if(pinfo.sid > 0)
                            {   // 通知可领取奖励
                                ::send_rpc(pinfo.sid, 232, { tp = 2, clteid = lvl_conf.cltwarid, awdtm = cur_tm_s}); // send clter_res msg
                            }
                            break;
                        }
                    }
                }
                // 全服帮派领地争夺战结束公告
                _broad_cast_sys_msg({ tp = bcast_msg_tp.CLANTER_LVL_FIN, ltpid = this.ltpid, par = clanid, par1 = old_clanid});

                // 记录数据
                local glbdata_mgr = global_data_mgrs.glbdata;
glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CLAN_TER, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
                local gld_clanter_data = _get_clanter_data();
                if(gld_clanter_data)
                {
                    local glb_clanter_info = gld_clanter_data.get_data();
glb_clanter_info.data[lvl_conf.cltwarid] <- clanter_info;
                    gld_clanter_data.mod_data(glb_clanter_info);
                    gld_clanter_data.db_update();
                }
                glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
            }
        }
        else if(this.clcqwar)
        {
    // 帮派奴役战
    _calc_clanconq_res();
}

        if(this.rnk_batl)
        {
    // 有排名奖励的战场
    local kprec_ary = [];

    foreach (cid, sgply in this.sgplayersbycid)
            {
        if (!(cid in this.kprec))
                {
            local clanid = 0;
            if ("clanid" in sgply.pinfo)
                    {
                clanid = sgply.pinfo.clanid;
            }
            this.kprec[sgply.pinfo.cid] < - { cid = sgply.pinfo.cid, sideid = sgply.pinfo.lvlsideid, kp = 0, ckp = 0, dc = 0, ac = 0, lvl_hexp = 0, bcasted = 0, clanid = clanid}; ;
        }
    }

    foreach (cid, rec in this.kprec)
            {
        kprec_ary.push(rec);
    }

    local sort_func = function(a, b)
            {
        if (a.kp > b.kp)
        {
            return -1;
        }
        else if (a.kp == b.kp && b.ac <= a.ac)
        {
            return -1;
        }

        return 1;
    }
    kprec_ary.sort(sort_func);

    local champ_ply = { cid = 0, nm = null };
    local btlrec_ary = [];
    foreach (kpinfo in kprec_ary)
    {
        // cid=killer.pinfo.cid, sideid=killer.pinfo.lvlsideid, kp=1, ckp=1, dc=0, ac=0, lvl_hexp=hexp_add, bcasted=0, clanid=clanid
        local chainfo = null;
        if (kpinfo.cid in this.sgplayersbycid)
                {
            chainfo = this.sgplayersbycid[kpinfo.cid].pinfo;
        }
                else
                {
            chainfo = _query_cha_info(kpinfo.cid);
        }
        if (!chainfo)
        {
            continue;
        }
        btlrec_ary.push({ cid = chainfo.cid, kp = kpinfo.kp, ac = kpinfo.ac, nm = chainfo.name, camp = chainfo.camp});

        if (champ_ply.cid <= 0)
        {
            champ_ply.cid = chainfo.cid;
            champ_ply.nm = chainfo.name;
        }
    }

    // 记录排名数据对象
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_BLT_RNK_INFO, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local battle_data = _get_battle_rnk_data();
    if (battle_data)
    {
        local battle_info = battle_data.get_data();
        if (battle_info)
        {
            if (this.lvl_conf.tpid in battle_info.data.cinfo)
                    {
                battle_info.data.linfo[this.lvl_conf.tpid] < -battle_info.data.cinfo[this.lvl_conf.tpid]; // 记录上次战斗排行榜
            }

            battle_info.data.cinfo[this.lvl_conf.tpid] < - { tm = sys.time(), btlrec = btlrec_ary};

            battle_data.mod_data(battle_info);
            battle_data.db_update();
        }
    }
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_BLT_RNK_INFO);  // 解锁

    // 计算胜利阵营
    local win_camp = 0;
    if (this.sgplayersbyside.Count > 0)
    {
        if (this.win in this.sides_conf)
                {
            local side_conf = this.sides_conf[this.win];
            if ("camp" in side_conf)
                    {
                win_camp = side_conf.camp;
            }
        }
    }

    // 广播排行奖励战场结束消息
    local msg = { tp = bcast_msg_tp.BTL_FIN, ltpid = this.lvl_conf.tpid, par = win_camp };
    if (champ_ply.cid > 0)
    {
        msg.cid < -champ_ply.cid;
        msg.name < -champ_ply.nm;
    }
    _broad_cast_sys_msg(msg);
}

        if((this.lvl_conf.lmtp != level_multi_type.LMT_MULTI))
        {
    local ply_reses = [];
    foreach (cid, ply in sgplayersbycid)
            {
        local plyres = { cid = cid, has_prz = (cid in this.prize), score = score, achives =[]}
    if (cid in ply_achives)
                {
        plyres.achives = ply_achives[cid];
    }

    if (ltpid in ply.pinfo.lvlentried)
                {
        local lvlentried = ply.pinfo.lvlentried[ltpid];
        plyres.diff_lvl < -lvlentried.diff_lvl;
        if ("fin_diff" in lvlentried)
                    {
            plyres.fin_diff < -lvlentried.fin_diff;
        }
        if (lvlentried.besttm == 0 || lvlentried.besttm > cost_tm)
        {
            lvlentried.besttm = cost_tm;
            plyres.besttm < -cost_tm;
        }
    }
    ply_reses.push(plyres);
}

            // broad cast lvl_fin msg
            this.broad_cast_msg(245, { win = this.win, close_tm = this.close_tm, ply_res = ply_reses});
}
        else
        {
            // 多人副本人数较多，不设置成就、得分、难度等级等内容，不广播用户结果

            local winplycids = [];
            if(this.win< 0 && win_plys.Count > 0)
            {
    foreach (cid, ply in win_plys)
                {
        winplycids.push(cid);
    }
}

local ret_msg = { win = this.win, close_tm = this.close_tm};
            if(winplycids.Count > 0 )
            {
    ret_msg.winplycids < -winplycids;
}

            if(arena_ply_res)
            {
    ret_msg.aply_res < -arena_ply_res;
}
            if(cltw_ply_res)
            {
    ret_msg.cply_res < -cltw_ply_res;
}
            if(cltw_clan_res)
            {
    ret_msg.cclan_res < -cltw_clan_res;
}

            // broad cast lvl_fin msg
            this.broad_cast_msg(245, ret_msg);

// TO DO : 考虑通知是否拥有奖励，或者获胜方必有奖励
}

        foreach(cid, ply in this.sgplayersbycid)
        {
    // 尝试更新每日必做任务
    _try_up_dmis(ply, "lvlfin", { ltpid = ltpid, diff = diff_lvl});
}

// log lvl log
_log_lvl_log(this.lvl_conf, this.start_tm, cur_tm_s, this.sgplayersbycid.Count, this.win, ply_achives);
}

function _calc_clanconq_res()
{
    // 计算帮派奴役战争结果
    local cur_tm_s = sys.time();

    // 删除帮派宣战记录
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CLANCHALENGE_INFO, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local glbclanch_data = _get_clanchalenge_data();
    if (!glbclanch_data)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLANCHALENGE_INFO);  // 解锁
        return null;
    }
    local glbclanch_info = glbclanch_data.get_data();

    foreach (idx, clanch in glbclanch_info.data.chlist)
        {
        if (clanch.id == this.creator)
        {
            glbclanch_info.data.chlist.remove(idx);
            break;
        }
    }

    glbclanch_data.mod_data(glbclanch_info);
    glbclanch_data.db_update();

    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLANCHALENGE_INFO);  // 解锁

    // 全局广播战场结果消息
    local atkwin = 1;
    if (this.clcqwar.cntleft > 0)
    {
        atkwin = 0;
    }
    // bcast clan_msg msg
    svr.broadcast_rpc(226, { tp = 4, tclid = this.clcqwar.def_clanid, fclid = this.clcqwar.atk_clanid, atkwin = atkwin}, true);

    // 获取进攻方帮派信息
    local clan_mgr = global_data_mgrs.clan;
    clan_mgr.lock_data(this.clcqwar.atk_clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

    local atk_clan_db_data = _get_clan_data(this.clcqwar.atk_clanid);
    if (atk_clan_db_data.Count <= 0)
    {
        // 目标帮派不存在
        clan_mgr.unlock_data(this.clcqwar.atk_clanid); // 解锁
        return;
    }
    local atk_clan_db_info = atk_clan_db_data[0].get_data();

    clan_mgr.unlock_data(this.clcqwar.atk_clanid); // 解锁

    // 获取防守方帮派信息
    clan_mgr.lock_data(this.clcqwar.def_clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

    local def_clan_db_data = _get_clan_data(this.clcqwar.def_clanid);
    if (def_clan_db_data.Count <= 0)
    {
        // 目标帮派不存在
        clan_mgr.unlock_data(this.clcqwar.def_clanid); // 解锁
        return;
    }
    local def_clan_db_info = def_clan_db_data[0].get_data();

    clan_mgr.unlock_data(this.clcqwar.def_clanid); // 解锁

    local win_clan_info = null;
    local lose_clan_info = null;
    local conqed = false;

    if (this.clcqwar.cntleft > 0)
    {
        // 防守方获胜
        win_clan_info = def_clan_db_info;
        lose_clan_info = atk_clan_db_info;

        if (atk_clan_db_info.conq.supclid == 0)
        {
            // 进攻方(失败方)无上级帮派，防守方成功奴役进攻方
            conqed = true;
        }
    }
    else
    {
        // 进攻方获胜
        win_clan_info = atk_clan_db_info;
        lose_clan_info = def_clan_db_info;

        conqed = true;
    }

    // 记录帮派日志:帮派从属战结果
    _clan_log(win_clan_info.id, clan_log_tp.CLAN_CONQ_WAR, { win = 1, clname = lose_clan_info.clname}); // 胜利方
    _clan_log(lose_clan_info.id, clan_log_tp.CLAN_CONQ_WAR, { win = 0, clname = win_clan_info.clname}); // 失败方

    if (win_clan_info.conq.supclid == lose_clan_info.id)
    {
        // 胜利方的上级是失败方，解除奴役

        // 更新胜利方帮派数据
        local clan_mgr = global_data_mgrs.clan;
        clan_mgr.lock_data(win_clan_info.id, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

        local win_clan_db_data = _get_clan_data(win_clan_info.id);
        if (win_clan_db_data.Count <= 0)
        {
            // 目标帮派不存在
            clan_mgr.unlock_data(win_clan_info.id); // 解锁
            return;
        }
        win_clan_info = win_clan_db_data[0].get_data();

        win_clan_info.conq.supclid = 0;

        win_clan_db_data[0].mod_data(win_clan_info);
        win_clan_db_data[0].db_update();

        clan_mgr.unlock_data(win_clan_info.id); // 解锁

        // broad cast clan_info_change msg
        _broad_cast_clan_msg(win_clan_info.id, 216, { supclid = win_clan_info.conq.supclid}, clan_c_tp.CCT_NONE);

        // 记录帮派日志:帮派从属关系变化
        _clan_log(win_clan_info.id, clan_log_tp.CLAN_CONQ_CHANGE, { conqtp = clan_conq_chang_tp.CCCT_DISMIS, clname = lose_clan_info.clname}); // 取消了下属关系

        // 更新失败方帮派数据
        clan_mgr.lock_data(lose_clan_info.id, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

        local lose_clan_db_data = _get_clan_data(lose_clan_info.id);
        if (lose_clan_db_data.Count <= 0)
        {
            // 目标帮派不存在
            clan_mgr.unlock_data(lose_clan_info.id); // 解锁
            return;
        }
        lose_clan_info = lose_clan_db_data[0].get_data();

        foreach (idx, conqedclan in lose_clan_info.conq.clans)
            {
            if (conqedclan.id == win_clan_info.id)
            {
                lose_clan_info.conq.clans.remove(idx);
                break;
            }
        }

        lose_clan_db_data[0].mod_data(lose_clan_info);
        lose_clan_db_data[0].db_update();

        clan_mgr.unlock_data(lose_clan_info.id); // 解锁

        // broad cast clan_info_change msg
        _broad_cast_clan_msg(lose_clan_info.id, 216, { rmv_cqclan_id = win_clan_info.id}, clan_c_tp.CCT_NONE);

        // 记录帮派日志:帮派从属关系变化
        _clan_log(lose_clan_info.id, clan_log_tp.CLAN_CONQ_CHANGE, { conqtp = clan_conq_chang_tp.CCCT_LOSE, clname = win_clan_info.clname}); // 失去了下属
    }
    else
    {
        if (conqed)
        {
            // 胜利方成功奴役失败方

            // 更新胜利方帮派数据
            local clan_mgr = global_data_mgrs.clan;
            clan_mgr.lock_data(win_clan_info.id, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

            local win_clan_db_data = _get_clan_data(win_clan_info.id);
            if (win_clan_db_data.Count <= 0)
            {
                // 目标帮派不存在
                clan_mgr.unlock_data(win_clan_info.id); // 解锁
                return;
            }
            win_clan_info = win_clan_db_data[0].get_data();

            // 获取帮派奴役科技数据
            if (!(game_data_conf.clan_occ_tech_id in win_clan_info.tech.techs))
                {
                clan_mgr.unlock_data(win_clan_info.id); // 解锁
                return;
            }

            local occ_tech = win_clan_info.tech.techs[game_data_conf.clan_occ_tech_id];

            if (occ_tech.lvl <= 0)
            {
                clan_mgr.unlock_data(win_clan_info.id); // 解锁
                return;
            }

            local tech_conf = get_clantech_conf(occ_tech.id);
            if (!tech_conf)
            {
                Utility.trace_err( "clan tech id[" + occ_tech.id + "] config not exist\n");
                clan_mgr.unlock_data(win_clan_info.id); // 解锁
                return;
            }

            local tech_lvl_conf = get_clantechlvl_conf(occ_tech.lvl);
            if (("clan_tech_lvl" in tech_conf) && (occ_tech.lvl in tech_conf.clan_tech_lvl))
                {
                tech_lvl_conf = tech_conf.clan_tech_lvl[occ_tech.lvl];
            }
            if (!tech_lvl_conf)
            {
                Utility.trace_err( "clan tech id[" + occ_tech.id + "] lvl[" + occ_tech.lvl + "] lvl config not exist\n");
                clan_mgr.unlock_data(win_clan_info.id); // 解锁
                return;
            }

            local tech_func_aff = tech_conf.func[0].aff[0];
            local occ_cnt = (tech_func_aff.cnt * ((100 + tech_lvl_conf.affper).tofloat() / 100.0)).tointeger();

            if (win_clan_info.conq.clans.Count >= occ_cnt)
            {
                // 不能再增加下属帮派了
                clan_mgr.unlock_data(win_clan_info.id); // 解锁
                return;
            }

            local game_conf = get_general_game_conf();

            local new_conq_clan = { id = lose_clan_info.id, clname = lose_clan_info.clname, lvl = lose_clan_info.lvl, congld = 0, conyb = 0, expire_tm = cur_tm_s + game_conf.clanconqtm };
            // 新增从属帮派
            win_clan_info.conq.clans.push(new_conq_clan);

            win_clan_db_data[0].mod_data(win_clan_info);
            win_clan_db_data[0].db_update();

            clan_mgr.unlock_data(win_clan_info.id); // 解锁

            // broad cast clan_info_change msg
            _broad_cast_clan_msg(win_clan_info.id, 216, { conqclan = new_conq_clan}, clan_c_tp.CCT_NONE);

            // 记录帮派日志:帮派从属关系变化
            _clan_log(win_clan_info.id, clan_log_tp.CLAN_CONQ_CHANGE, { conqtp = clan_conq_chang_tp.CCCT_CONQ, clname = lose_clan_info.clname}); // 征服了对方，成为了上级

            // 更新失败方帮派数据
            clan_mgr.lock_data(lose_clan_info.id, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

            local lose_clan_db_data = _get_clan_data(lose_clan_info.id);
            if (lose_clan_db_data.Count <= 0)
            {
                // 目标帮派不存在
                clan_mgr.unlock_data(lose_clan_info.id); // 解锁
                return;
            }
            lose_clan_info = lose_clan_db_data[0].get_data();

            local old_supclid = lose_clan_info.conq.supclid;
            lose_clan_info.conq.supclid = win_clan_info.id;

            lose_clan_db_data[0].mod_data(lose_clan_info);
            lose_clan_db_data[0].db_update();

            clan_mgr.unlock_data(lose_clan_info.id); // 解锁

            // broad cast clan_info_change msg
            _broad_cast_clan_msg(lose_clan_info.id, 216, { supclid = lose_clan_info.conq.supclid}, clan_c_tp.CCT_NONE);

            // 记录帮派日志:帮派从属关系变化
            _clan_log(lose_clan_info.id, clan_log_tp.CLAN_CONQ_CHANGE, { conqtp = clan_conq_chang_tp.CCCT_BECONQ, clname = win_clan_info.clname}); // 被对方征服，成为了下属

            if (old_supclid != 0)
            {
                // 有原上级帮派，解除原上级帮派的从属关系
                clan_mgr.lock_data(old_supclid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

                local oldsup_clan_db_data = _get_clan_data(old_supclid);
                if (oldsup_clan_db_data.Count <= 0)
                {
                    // 目标帮派不存在
                    clan_mgr.unlock_data(old_supclid); // 解锁
                    return;
                }
                local oldsup_clan_info = oldsup_clan_db_data[0].get_data();

                foreach (idx, conqedclan in oldsup_clan_info.conq.clans)
                    {
                    if (conqedclan.id == lose_clan_info.id)
                    {
                        oldsup_clan_info.conq.clans.remove(idx);
                        break;
                    }
                }

                oldsup_clan_db_data[0].mod_data(oldsup_clan_info);
                oldsup_clan_db_data[0].db_update();

                clan_mgr.unlock_data(old_supclid); // 解锁

                // broad cast clan_info_change msg
                _broad_cast_clan_msg(oldsup_clan_info.id, 216, { rmv_cqclan_id = lose_clan_info.id}, clan_c_tp.CCT_NONE);

                // 记录帮派日志:帮派从属关系变化
                _clan_log(oldsup_clan_info.id, clan_log_tp.CLAN_CONQ_CHANGE, { conqtp = clan_conq_chang_tp.CCCT_BEGRAB, clname = lose_clan_info.clname, o_clname = win_clan_info.clname}); // 下属被其他帮派抢走
            }
        }
        else
        {
            // 从属关系无变化
        }
    }
}

function get_fin_awd()
{
    local awd = null;
    if ("fin_awd" in lvl_conf )
        {
        awd = lvl_conf.fin_awd;
    }
    if ((this.diff_lvl in lvl_conf) && "fin_awd" in lvl_conf[this.diff_lvl]  )
        {
        awd = lvl_conf[this.diff_lvl].fin_awd;
    }
    return awd;
}

function _give_lvlfin_awd(cur_tm_s, plys, awd_conf, score_awd_conf= null)
{
    local awd = {
            exp = 0,
            gld = 0,
            bndyb = 0,
            skexp = 0,
            nobpt = 0,
            meript = 0,
        };

    local fin_awd = get_fin_awd();

    if (fin_awd)
    {
        foreach (key, val in awd )
            {
            if (key in fin_awd )
                {
                awd[key] += fin_awd[key];
            }
        }
    }

    if (awd_conf)
    {
        foreach (key, val in awd )
            {
            if (key in awd_conf )
                {
                awd[key] += awd_conf[key];
            }
        }
    }
    if (score_awd_conf)
    {
        foreach (key, val in awd )
            {
            if (key in score_awd_conf )
                {
                awd[key] += score_awd_conf[key];
            }
        }
    }

    local exp = awd.exp;
    local gld = awd.gld;
    local bndyb = awd.bndyb;
    local skexp = awd.skexp;
    local nobpt = awd.nobpt;
    local meript = awd.meript;

    local real_gold = 0;
    local total_gold = 0;
    local item_change_msg;
    foreach (ply in plys)
    {
        if (exp > 0)
        {
            ply.modify_exp(exp);
        }
        item_change_msg = { };
        if (gld > 0)
        {
            real_gold = ply.add_gold(gld);
            if (real_gold > 0)
            {
                item_change_msg.gold < -real_gold;

                // 尝试更新排行活动值
                _rnkact_on_addgld(ply, real_gold);
                total_gold += real_gold;
            }
        }
        if (bndyb > 0)
        {
            // 获得礼券
            ply.add_bndyb(bndyb);
            item_change_msg.bndyb < -bndyb;

            // log bndyb change
            _log_bndyb_log(ply, cur_tm_s, bndyb_act_type.BAT_LVL_AWD, bndyb, 0, this.ltpid, 0);
        }
        if (skexp > 0)
        {
            ply.modify_skexp(skexp, 1); // 副本奖励气海值
        }
        if (nobpt > 0)
        {
            ply.add_nobpt(nobpt);
        }
        if (meript > 0)
        {
            ply.add_meript(meript);
        }

        if (item_change_msg.Count > 0)
        {
                // send item_change msg
                ::send_rpc(ply.pinfo.sid, 75, item_change_msg);
        }


        //物品奖励
        local to_add_itms = {
                exp = 0,        //不使用
                gld = 0,        //不使用
                bndyb = 0,      //不使用
                skexp = 0,      //不使用
                nobpt = 0,      //不使用
                meript = 0,     //不使用
                itm = [],
                eqp = [],
            };



        if (awd_conf)
        {
            if ("itm" in awd_conf )
                {
                to_add_itms.itm.extend(awd_conf.itm);
            }
            if ("eqp" in awd_conf )
                {
                to_add_itms.eqp.extend(awd_conf.eqp);
            }
        }

        if (score_awd_conf)
        {
            if ("itm" in score_awd_conf )
                {
                to_add_itms.itm.extend(score_awd_conf.itm);
            }
            if ("eqp" in score_awd_conf )
                {
                to_add_itms.eqp.extend(score_awd_conf.eqp);
            }
        }

        if (fin_awd)
        {
            if ("itm" in fin_awd )
                {
                to_add_itms.itm.extend(fin_awd.itm);
            }
            if ("eqp" in fin_awd )
                {
                to_add_itms.eqp.extend(fin_awd.eqp);
            }
        }


        //::g_dump( "lvl to_add_itms:", to_add_itms );

        local res = _check_add_item_to_pl(to_add_itms, ply);
        if (res == game_err_code.RES_OK)
        {

            local new_add_itm_ary = _add_item_to_pl(to_add_itms, ply, false);


            ply.flush_db_data(false, false); // 同步数据至管理器 

            // log itm
            if (new_add_itm_ary.Count > 0)
            {
                _log_itm_log(
                    ply,
                    cur_tm_s,
                    itm_act_type.IAT_LVL_AWD,
                    itm_flag_type.IFT_NEW_ADD,
                    ltpid,
                    new_add_itm_ary,

                    []
                );
            }
        }
        else
        {//发邮件
            local title = "lvl_mail#" + ltpid;
            _send_mail(
                0,
                    {
                tocid = ply.pinfo.cid, 
                        title = title, 
                        msg = ""
                    }, 
                    0, 
                    "", 
                    to_add_itms, 
                    null
                );

            //_send_mail(sid, rpc, frmcid, frmnm, itm, worldsvr, trans_svr=true, ply=null)
        }


    }

    if (total_gold > 0)
    {
        // log gold
        svr.add_gold_log(gold_act_type.GLDAT_LVL_AWD, total_gold, 0);
    }
}

function _get_gp_map_win_plys(gp_map, cur_tm_s )
{
    local win_plys = { };

    if (this.death_pts)
    {
        // 混战积分模式

        // 按积分排序
        local ply_ary = [];
        foreach (val in this.death_pts)
        {
            if (!(val.cid in gp_map.plys) ) continue;

            local idx = 0;
            for (; idx < ply_ary.Count; ++idx)
            {
                if (val.pt >= ply_ary[idx].pt)
                {
                    break;
                }
            }
            ply_ary.insert(idx, val);
        }

        local i = 0;
        for (; i < ply_ary.Count && i < this.death_ptconf.wincnt; ++i)
        {
            local cid = ply_ary[i].cid;
            win_plys[cid] < -gp_map.plys[cid];
        }
    }
    else if (gp_map.win >= 0)
    {
        if (death_match)
        {
            // 死亡模式
            if (this.sgplayersbyside.Count > 0 && gp_map.win > 0)
            {
                // 分阵营情况下，this.win为胜利方阵营id
                local win_side_players = this.sgplayersbyside[gp_map.win];
                foreach (ply in win_side_players)
                {
                    if (ply.pinfo.cid in gp_map.plys )
                        {
                        win_plys[ply.pinfo.cid] < -ply;
                    }
                }
            }
            else
            {
                // 混战情况下this.win为胜利者角色id
                if (gp_map.win in gp_map.plys )
                    {
                    win_plys[gp_map.win] < -gp_map.plys[gp_map.win];
                }
            }
        }
        else if (this.kumiteply)
        {
            // 车轮战模式，胜利者角色id
            if (gp_map.win in gp_map.plys)
                {
                win_plys[gp_map.win] < -gp_map.plys[gp_map.win];
            }
        }
        else
        {
            win_plys = gp_map.plys;
            if (gp_map.win > 0)
            {
                // 胜利者有阵营
                local win_side_players = this.sgplayersbyside[gp_map.win];
                foreach (ply in win_side_players)
                {
                    if (ply.pinfo.cid in gp_map.plys )
                        {
                        win_plys[ply.pinfo.cid] < -ply;
                    }
                }
            }
        }
    }

    return win_plys;
}


function gp_map_finish(gp_map, cur_tm_s )
{
    if (gp_map.fin)
    {
        return;
    }
    gp_map.fin = true;
    gp_map.close_tm < -cur_tm_s + 30;

    local ply_achives = { };
    local score = 0;
    local cost_tm = cur_tm_s - this.start_tm;

    local win_plys = _get_gp_map_win_plys(gp_map, cur_tm_s);

    // 计算荣誉值
    //if("pvp" in this.lvl_conf)
    //{
    //    local give_hexp = true;
    //    if("hexp_tmchk" in this.lvl_conf.pvp[0])
    //    {
    //        give_hexp = _check_lvl_tmchk( this.lvl_conf.pvp[0].hexp_tmchk ) == game_err_code.RES_OK;              
    //    }

    //    if(give_hexp)
    //    {
    //        foreach(cid, ply in this.sgplayersbycid)
    //        {
    //            local hexp_add = this.lvl_conf.pvp[0].hexplose;
    //            if(cid in win_plys)
    //            {
    //                hexp_add = this.lvl_conf.pvp[0].hexpwin;
    //            }

    //            if(hexp_add == 0)
    //            {
    //                continue;
    //            }

    //            // 增加荣誉值
    //            local real_add = ply.add_hexp(hexp_add);

    //            if(real_add > 0)
    //            {
    //                // 尝试更新排行活动值
    //                _rnkact_on_pvphexp(ply, real_add);
    //            }
    //        }
    //    }
    //}

    if (win_plys.Count > 0)
    {
        if ("score" in lvl_conf)
            {
            // 计算得分
            foreach (score_conf in lvl_conf.score)
            {
                if (score_conf.tm >= cost_tm && score_conf.score > score)
                {
                    score = score_conf.score;
                }
            }

            //sys.trace(sys.SLT_DETAIL, "score ["+score+"] upscore["+lvl_conf.upscore+"]\n");

            //if(score >= lvl_conf.upscore)
            //{
            // 更新角色关卡得分并升级

            foreach (cid, ply in win_plys)
                    {
                //sys.trace(sys.SLT_DETAIL, "ltpid ["+ltpid+"]\n");
                //sys.dumpobj(ply.pinfo.lvlentried);

                if (!(ltpid in ply.pinfo.lvlentried))
                        {
                    continue;
                }
                local lvlentried = ply.pinfo.lvlentried[ltpid];
                lvlentried.score = score;

                // 记录已通关难度
                if (!("fin_diff" in lvlentried))
                        {
                    lvlentried.fin_diff < -lvlentried.diff_lvl;
                }
                        else if (lvlentried.fin_diff < lvlentried.diff_lvl)
                {
                    lvlentried.fin_diff = lvlentried.diff_lvl;
                }

                if (diff_lvl >= lvlentried.diff_lvl)
                {
                    ++lvlentried.diff_lvl;

                    //sys.dumpobj(lvlentried);

                    if (!(lvlentried.diff_lvl in lvl_conf.diff_lvl))
                            {
                        --lvlentried.diff_lvl;
                    }
                            else
                            {
                        lvlentried.score = 0;
                    }
                }

                ++lvlentried.fcnt;
            }
            //}
        }

        local awd_itms = []; // 翻牌奖励
        local prize_itms = []; // 转盘奖励
        local total_rate = 0;
        if ("drop_itm" in lvl_conf)
            {
            foreach (val in lvl_conf.drop_itm)
            {
                prize_itms.push(val);
                total_rate += val.rate;
            }
        }

        if (diff_lvl in lvl_conf.diff_lvl)
            {
            local diff_lvl_conf = lvl_conf.diff_lvl[diff_lvl];

            if ("drop_itm" in diff_lvl_conf)
                {
                // 难度额外掉落
                foreach (val in diff_lvl_conf.drop_itm)
                {
                    prize_itms.push(val);
                    total_rate += val.rate;
                }
            }

            if ("awd_itm" in diff_lvl_conf)
                {
                awd_itms = diff_lvl_conf.awd_itm;
            }

            if ("achive" in  diff_lvl_conf)
                {
                // 获得称号
                foreach (achive_conf in diff_lvl_conf.achive)
                {
                    if (!("score" in achive_conf) || score >= achive_conf.score)
                        {
                        local boardachive = ("boardcast" in achive_conf) && achive_conf.boardcast != 0;
                        foreach (cid, ply in win_plys)
                            {
                            if (!ply.has_achive(achive_conf.id))
                            {
                                ply.pinfo.achives.push(achive_conf.id);
                            }

                            local ach_data = { achive = achive_conf.id };
                            // 记录成就时效
                            if ("tm" in achive_conf)
                                {
                                ach_data.tm < -cur_tm_s + achive_conf.tm;
                                ply.pinfo.achivetm[achive_conf.id] < -ach_data.tm;
                            }
                                ::send_rpc(ply.pinfo.sid, 5, achive_conf); // send gain achive msg

                            if (boardachive)
                            {
                                // 全服广播
                                _broad_cast_sys_msg({ tp = bcast_msg_tp.GOT_ACHIVE, cid = ply.pinfo.cid, name = ply.pinfo.name, par = achive_conf.id});
                            }

                            if (!(cid in ply_achives))
                                {
                                ply_achives[cid] < - [achive_conf.id];
                            }
                                else
                                {
                                local hasid = false;
                                foreach (achid in ply_achives[cid])
                                {
                                    if (achid == achive_conf.id)
                                    {
                                        hasid = true;
                                        break;
                                    }
                                }
                                if (!hasid) ply_achives[cid].push(achive_conf.id);
                            }
                        }
                    }
                    //else if()
                    //{
                    //    // 其他称号获得条件判断
                    //}
                }
            }
            local win_awd_conf = null;
            // 给胜利方奖励
            if ("win_awd" in diff_lvl_conf)
                {
                win_awd_conf = diff_lvl_conf.win_awd[0];
                //_give_lvlfin_awd( cur_tm_s, win_plys, diff_lvl_conf.win_awd[0] );
            }

            local score_awd_conf = get_score_awd_conf()

                _give_lvlfin_awd(cur_tm_s, win_plys, win_awd_conf, score_awd_conf);
        }

        if (awd_itms.Count > 0)
        {
            // 抽牌奖励
            foreach (cid, ply in win_plys)
                {
                this.prize[cid] < - { tp = 0, cnt = 0};
                ply.pinfo.lvlprize.push({ ltpid = this.ltpid, diff_lvl = diff_lvl});
            }

        }
        else if (prize_itms.Count > 0)
        {
            // 转盘奖励
            foreach (cid, ply in win_plys)
                {
                local judg = sys.rand_between(0, total_rate);
                local cur_rate = 0;

                foreach (prz in prize_itms)
                {
                    cur_rate += prz.rate;

                    if (judg < cur_rate)
                    {
                        // 根据道具掉落数量控制掉落结果
                        //local itms={itm=[{id=prz.itmid, cnt=prz.cnt}],eqp=[]}
                        //check_dpitm_ctrl(itms);
                        //if(itms.itm.Count <= 0)
                        //{
                        //    // 不能再掉落了
                        //    continue;
                        //}

                        if (!check_single_dpitm_ctrl(prz.itmid, prz.cnt))
                        {
                            // 道具不能掉落了
                            continue;
                        }

                        this.prize[cid] < - { tp = prz.itmid, cnt = prz.cnt};

                        local bnd = ("bnd" in prz) ? prz.bnd : 0;
                        ply.pinfo.lvlprize.push({ ltpid = this.ltpid, tp = prz.itmid, bnd = bnd, cnt = prz.cnt, diff_lvl = diff_lvl});
                        break;
                    }
                }
            }
        }

        if ("rank" in lvl_conf && lvl_conf.rank == 1)
            {
            // 需要排行

            local game_conf = get_general_game_conf();

            local rnk_data = _get_rank_info({ sub_tp = ltpid, rnk_tp = gdctrl_type.GT_RANK_LVL});

            local need_add = true;
            if (rnk_data.rnkary.Count >= game_conf.rnk_total_cnt)
            {
                // 判断是否达到上榜要求
                local idx = rnk_data.rnkary.Count - 1;
                if (idx >= 0)
                {
                    local rnk_info = rnk_data.rnkary[idx];
                    if (rnk_info.rnkv > diff_lvl)
                    {
                        need_add = false;
                    }
                    else if (rnk_info.rnkv == diff_lvl && rnk_info.besttm < cost_tm)
                    {
                        need_add = false;
                    }
                }
            }

            if (need_add)
            {
                local cnt = 0;
                local player_names = "";
                foreach (cid, ply in win_plys)
                    {
                    if (cnt > 0)
                    {
                        player_names += ",";
                    }
                    player_names += ply.pinfo.name;
                    ++cnt;
                    if (cnt >= game_conf.lvl_rank_names_cnt)
                    {
                        break;
                    }
                }

                // 增加副本排行记录
                db_obj.mod("insert into `rnklvl` (ltpid, difflvl, besttm, par) values(" + ltpid + "," + diff_lvl + "," + cost_tm + ",'" + player_names + "');");
            }
        }

        foreach (cid, ply in win_plys)
            {
            // 尝试更新排行活动值
            _rnkact_on_lvlfin(ply, this.lvl_conf, this.diff_lvl);

            // 尝试更新每日必做任务
            _try_up_dmis(ply, "lvlwin", { ltpid = ltpid, diff = diff_lvl});

            ply.flush_db_data(false, false); // 同步至内存全局数据管理器
        }
    }

    if ("lose_awd" in diff_lvl_conf )
        {
        local lose_plys = [];
        foreach (cid, ply in gp_map.plys)
            {
            if (cid in win_plys)
                {
                continue;
            }

            lose_plys.push(ply);
        }

        if (lose_plys.Count > 0)
        {
            _give_lvlfin_awd(cur_tm_s, lose_plys, diff_lvl_conf.lose_awd[0]);
        }
    }

    if ((this.lvl_conf.lmtp != level_multi_type.LMT_MULTI))
    {
        local ply_reses = [];
        foreach (cid, ply in gp_map.plys)
            {
            local plyres = { cid = cid, has_prz = (cid in this.prize), score = score, achives =[]}
        if (cid in ply_achives)
                {
            plyres.achives = ply_achives[cid];
        }

        if (ltpid in ply.pinfo.lvlentried)
                {
            local lvlentried = ply.pinfo.lvlentried[ltpid];
            plyres.diff_lvl < -lvlentried.diff_lvl;
            if ("fin_diff" in lvlentried)
                    {
                plyres.fin_diff < -lvlentried.fin_diff;
            }
            if (lvlentried.besttm == 0 || lvlentried.besttm > cost_tm)
            {
                lvlentried.besttm = cost_tm;
                plyres.besttm < -cost_tm;
            }
        }
        ply_reses.push(plyres);
    }

    // broad cast lvl_fin msg
    this.broad_cast_msg(245, { win = gp_map.win, close_tm = gp_map.close_tm, ply_res = ply_reses});
}
        else
        {
            // 多人副本人数较多，不设置成就、得分、难度等级等内容，不广播用户结果

            local winplycids = [];
            if(this.win< 0 && win_plys.Count > 0)
            {
    foreach (cid, ply in win_plys)
                {
        winplycids.push(cid);
    }
}

local ret_msg = { win = gp_map.win, close_tm = gp_map.close_tm};
            if(winplycids.Count > 0 )
            {
    ret_msg.winplycids < -winplycids;
}

            // broad cast lvl_fin msg
            this.broad_cast_msg(245, ret_msg);

// TO DO : 考虑通知是否拥有奖励，或者获胜方必有奖励
}

        foreach(cid, ply in gp_map.plys)
        {
            // 尝试更新每日必做任务
            _try_up_dmis(ply, "lvlfin", { ltpid = ltpid, diff = diff_lvl});
        }

        // log lvl log
        _log_lvl_log(this.lvl_conf, this.start_tm, cur_tm_s, gp_map.plycnt, gp_map.win, ply_achives, gp_map.gpid);
    }



function _init_stastic_lvl_cost(init_yb, init_gld )
{
    this.stastic_lvl_cost = { yb_cost ={ }, total_yb_cost = init_yb, gld_cost ={ }, total_gld_cost = init_gld };
}

function on_ply_cost_yb(ply, tp, cost )
{
    if (this.state == level_state_type.LST_FINED) return;

    if (this.stastic_lvl_cost && (ply.pinfo.sid in this.sgplayers) )
        {
        if (tp in this.stastic_lvl_cost.yb_cost )
            {
            this.stastic_lvl_cost.yb_cost[tp] += cost;
        }
            else
            {
            this.stastic_lvl_cost.yb_cost[tp] < -cost;
        }
        this.stastic_lvl_cost.total_yb_cost += cost;

        this.broad_cast_msg(233, { tp = 8, add_cost_yb = cost} );
    }
}

function on_ply_cost_gold(ply, tp, cost )
{
    if (this.state == level_state_type.LST_FINED) return;

    if (this.stastic_lvl_cost && (ply.pinfo.sid in this.sgplayers) )
        {
        if (tp in this.stastic_lvl_cost.gld_cost )
            {
            this.stastic_lvl_cost.gld_cost[tp] += cost;
        }
            else
            {
            this.stastic_lvl_cost.gld_cost[tp] < -cost;
        }
        this.stastic_lvl_cost.total_gld_cost += cost;

        this.broad_cast_msg(233, { tp = 8, add_cost_gold = cost} );
    }
}

function _trig_modatt(ply, modatt)
{
    local pl = null;
    if (ply)
    {
        pl = ply.get_pack_data();
        //sys.trace(sys.SLT_DETAIL, "ply.get_sprite_type()["+ply.get_sprite_type()+"] pl.lvlsideid["+pl.lvlsideid+"]\n");
    }

    local plys = null;
    if (modatt.sideadd == 1)
    {
        // 增加阵营的属性
        if (pl && pl.lvlsideid > 0)
        {
            // 分阵营
            //sys.trace(sys.SLT_DETAIL, "pl.lvlsideid["+pl.lvlsideid+"]\n");
            plys = this.sgplayersbyside[pl.lvlsideid];
        }
        else
        {
            if (ply && ply.get_sprite_type() == map_sprite_type.MST_PLAYER)
            {
                plys = this.sgplayersbycid;
            }
        }
    }
    else
    {
        if (ply)
        {
            if (ply.get_sprite_type() == map_sprite_type.MST_PLAYER)
            {
                plys = [ply];
            }
        }
        else
        {
            plys = this.sgplayersbycid;
        }
    }

    if (plys)
    {
        local exp = ("exp" in modatt) ? modatt.exp : 0;
        local gld = ("gld" in modatt) ? modatt.gld : 0;
        local skexp = ("skexp" in modatt) ? modatt.skexp : 0;
        local nobpt = ("nobpt" in modatt) ? modatt.nobpt : 0;
        local meript = ("meript" in modatt) ? modatt.meript : 0;

        local real_gold = 0;
        local total_gold = 0;
        foreach (ply in plys)
        {
            if (exp > 0)
            {
                ply.modify_exp(exp);
            }
            if (gld > 0)
            {
                real_gold = ply.add_gold(gld);
                if (real_gold > 0)
                {
                        // send item_change msg
                        ::send_rpc(ply.pinfo.sid, 75, { gold = real_gold});
                    // 尝试更新排行活动值
                    _rnkact_on_addgld(ply, real_gold);
                    // log gold
                    total_gold += real_gold
                    }
            }
            if (skexp > 0)
            {
                ply.modify_skexp(skexp, 1); // 副本奖励气海值
            }
            if (nobpt > 0)
            {
                ply.add_nobpt(nobpt);
            }
            if (meript > 0)
            {
                ply.add_meript(meript);
            }
        }

        if (total_gold > 0)
        {
            // log gold
            svr.add_gold_log(gold_act_type.GLDAT_LVL_TRIGER, total_gold, 0);
        }
    }
}
function _trig_addstat(spr, conf)
{
    local cur_clock_tm = sys.clock_time();
    local add_state_ply = { };

    local pl = spr.get_pack_data();

    foreach (addstat in conf)
    {
        local plys = [];
        if (addstat.sideadd == 1)
        {
            // 增加自己阵营的状态
            if (pl.lvlsideid > 0)
            {
                // 分阵营
                plys = this.sgplayersbyside[pl.lvlsideid];
            }
            else
            {
                plys = this.sgplayersbycid;
            }
        }
        else if (addstat.sideadd == 2)
        {
            // 增加自己敌对阵营的状态
            if (pl.lvlsideid > 0)
            {
                // 分阵营
                foreach (sideid, plys_byside in this.sgplayersbyside)
                    {
                    if (sideid == pl.lvlsideid)
                        continue;

                    foreach (p in plys_byside)
                    {
                        plys.push(p);
                    }
                }
            }
        }
        else if (spr.get_sprite_type() == map_sprite_type.MST_PLAYER)
        {
            plys.push(spr);
        }

        foreach (curply in plys)
        {
            local state_obj = add_state_to_pl(cur_clock_tm, curply, addstat, curply, 1000, false);
            if (state_obj != null)
            {
                local plystat = { ply = curply, iid = curply.pinfo.iid, states =[] };
                if (!(curply.pinfo.cid in add_state_ply))
                    {
                    add_state_ply[curply.pinfo.cid] < -plystat;
                }
                    else
                    {
                    plystat = add_state_ply[curply.pinfo.cid];
                }
                plystat.states.push(state_obj);
            }
        }
    }

    foreach (plystat in add_state_ply)
    {
        _remark_pl_state(plystat.ply, plystat.ply.pinfo);

        // broadcast add state msg;
        plystat.ply.broad_cast_zone_msg_and_self(24, plystat);
    }
}
function _trig_vkm(spr, vkm_confs, gmap)
{
    if (gmap)
    {
        // 尝试触发地图触发器
        foreach (vkm in vkm_confs)
        {
            if (!("trig" in vkm) || (vkm.trig <= 0))
                {
                // 无触发地图触发器属性
                continue;
            }

            gmap.on_km(spr, vkm.mid);
        }
    }

    if (!spr)
    {
        return;
    }

    local pl = spr.get_pack_data();

    if (pl.lvlsideid > 0)
    {
        // 分阵营

        if (pl.lvlsideid in this.kmfinbyside)
            {
            local kmside = this.kmfinbyside[pl.lvlsideid];

            foreach (vkm in vkm_confs)
            {
                if (vkm.mid in kmside)
                    {
                    local km = kmside[vkm.mid];

                    --km.cntleft;

                    // 通知客户端杀怪 lvl_km msg
                    if (spr.get_sprite_type() == map_sprite_type.MST_PLAYER)
                    {
                        this.broad_cast_msg(249, { sideid = pl.lvlsideid, cid = pl.cid, name = pl.name, mid = vkm.mid});
                    }
                    else
                    {
                        // 怪物杀死怪物
                        this.broad_cast_msg(249, { sideid = pl.lvlsideid, cid = 0, name = spr.monconf.name, mid = vkm.mid});
                    }

                    if (km.cntleft < 0)
                    {
                        km.cntleft = 0;
                    }

                    //_check_finish();
                }
            }
        }

        foreach (vkm in vkm_confs)
        {
            if (!("shmis" in vkm) || (vkm.shmis <= 0))
                {
                // 非共享任务击杀
                continue;
            }

            local mis_plys = { };
            if ((vkm.shmis == 2))
            {
                if (!(pl.lvlsideid in this.sgplayersbyside))
                    {
                    continue;
                }

                // 全阵营共享任务击杀
                mis_plys = this.sgplayersbyside[pl.lvlsideid];

            }
            else if (vkm.shmis == 1)
            {
                // 触发者任务击杀
                if (spr.get_sprite_type() == map_sprite_type.MST_PLAYER)
                {
                    mis_plys[spr.pinfo.cid] < -spr;
                }
            }

            foreach (ply in mis_plys)
            {
                foreach (mis in ply.pinfo.misacept)
                {
                    local conf = get_mis_conf(mis.misid);

                    local mis_goal = _get_mission_goal(ply.pinfo, conf);

                    // 更新角色任务杀怪数
                    if (!("km" in mis))
                        {
                        continue;
                    }

                    foreach (idx, km in mis.km)
                        {
                        if (km.monid != vkm.mid)
                        {
                            continue;
                        }

                        local old_km_cnt = km.cnt;
                        ++km.cnt;
                        if (mis_goal && "kilmon" in mis_goal && mis_goal.kilmon.Count > idx)
                            {
                            if (km.cnt > mis_goal.kilmon[idx].cnt)
                            {
                                km.cnt = mis_goal.kilmon[idx].cnt;

                                if ("follow" in mis_goal.kilmon[idx])
                                    {
                                    local follow = mis_goal.kilmon[idx].follow;

                                    local old_follow = 0;
                                    if ("follow" in ply.pinfo)
                                        {
                                        old_follow = ply.pinfo.follow;
                                    }

                                    if (follow > 0)
                                    {
                                        ply.pinfo.follow < -follow;

                                        if (old_follow != follow)
                                        {
                                            // broad cast attchange msg
                                            ply.broad_cast_zone_msg(26, { iid = ply.pinfo.iid, follow = ply.pinfo.follow});
                                        }
                                    }
                                    else if (old_follow > 0)
                                    {
                                        delete ply.pinfo.follow;

                                        // broad cast attchange msg
                                        ply.broad_cast_zone_msg(26, { iid = ply.pinfo.iid, follow = 0});
                                    }
                                }
                            }
                        }

                        if (old_km_cnt != km.cnt)
                        {
                                // send mis_data_modify msg
                                ::send_rpc(ply.pinfo.sid, 113, { misid = mis.misid, monid = km.monid, cnt = km.cnt});
                        }
                    }
                }
            }
        }
    }

}

function _trig_active_finchk(chk = true)
{
    this.active_finchk = chk;
}

function _trig_finlvl(spr, finlvl_conf, gmap)
{
    // 触发了结束副本触发器
    if (this.gpmap_maxply > 0)
    {
        local gmap_ply;
        if (spr && spr.get_sprite_type() == map_sprite_type.MST_PLAYER)
        {
            gmap_ply = spr;
        }
        else
        {
            foreach (ply in gmap.map_players)
            {
                gmap_ply = ply;
                break;
            }
        }

        if (gmap_ply)
        {
            //先处理不分阵营的情况
            this.win = 0;
            local gpid = this.cid2gpid[gmap_ply.pinfo.cid];
            this.gp_map_finish(this.gp_maps[gpid - 1], sys.time());
        }
    }
    else
    {
        this.lvl_finish(sys.time());
    }
}




function score_awd_tm(score_awds )
{
    local curr_tm = sys.time();
    local pass_tm = curr_tm - this.start_tm;
    local i = 0;
    for (i = 0; i < score_awds.Count; i++)
    {
        local awd = score_awds[i];
        if (pass_tm <= awd.tm)
        {
            break;
        }
    }
    if (i >= score_awds.Count) return null;

    return score_awds[i];
}

function score_awd_km()
{
    local curr_km = 0;
    local i = 0;
    local mark_idx = -1;
    local score_awds = _get_score_conf("score_awd_km");
    if (!score_awds) return null;

    for (i = 0; i < score_awds.Count; i++)
    {
        local awd = score_awds[i];
        if (score_km.kmcnt > awd.cnt)
        {
            mark_idx = i;
        }
    }
    if (mark_idx < 0)
    {
        return null;
    }
    local tmp = score_awds[mark_idx];
    return score_awds[mark_idx];
}

function _get_score_conf(key )
{
    local conf = lvl_conf;
    if (diff_lvl > 0)
    {
        if (key in lvl_conf.diff_lvl[diff_lvl] )
            {
            conf = lvl_conf.diff_lvl[diff_lvl];
        }
    }
    if (key in conf ) 
        {
        return conf[key];
    }
    return null;
}

function get_map_mon(mapid)
{
    if (mapid <= 0 || !(mapid in this.maps) ) return 0;

    local needkm = null;

    if (!(this.diff_lvl in map_need_km)) return 0;

    foreach (kmdata in this.map_need_km[this.diff_lvl])
    {
        if (kmdata.mapid == mapid)
        {
            needkm = kmdata;
            break;
        }
    }
    if (!needkm) return 0;

    local mapdata = this.maps[mapid];
    local moncnt = 0;
    if (0 == needkm.mid)
    {
        foreach (mon in mapdata.map_mons)
        {
            if (!mon.mondata.isdie && !("owner_cid" in mon.mondata)) moncnt++;
        }
    }
    else
    {
        foreach (val in mapdata.map_mons)
        {
            if (val.mondata.mid == needkm.mid && !val.mondata.isdie && !("owner_cid" in mon.mondata)) moncnt++;
        }
    }
    return moncnt;
}

function get_lvlmap_enter_conditon(mpid)
{   //进入副本地图条件
    if (is_map_lvlmap(mpid))
    {
        if ("diff_lvl" in lvl_conf)
            {
            local mapdata = lvl_conf.diff_lvl[this.diff_lvl];
            foreach (map in mapdata.map)
            {
                if (map.id == mpid)
                {
                    if ("dir_enter" in map)
                        {
                        return map.dir_enter[0];
                    }
                }
            }
        }
    }
    return null;
}
function check_lvlmap_awd(mpid, ply)
{
    if (is_map_lvlmap(mpid))
    {
        local noget = false;
        if (mpid in enter_map_plys)
            {
            local enter_plys = enter_map_plys[mpid];

            foreach (oldply in enter_plys)
            {
                if (oldply.cid == ply.pinfo.cid)
                {
                    if (!oldply.give)
                    {
                        noget = true;
                    }
                    oldply.give = true;
                    break;
                }
            }
        }
        if (noget)
        {
            if ("diff_lvl" in lvl_conf)
                {
                local mapdata = lvl_conf.diff_lvl[this.diff_lvl];
                foreach (map in mapdata.map)
                {
                    if (map.id == mpid)
                    {
                        if ("awdfin" in map)
                            {
                            return map.awdfin[0];
                        }
                    }
                }
            }
        }

    }
    return null;
}

}
}

// 获取侠客行每关霸主、首破、最近通关玩家信息，必须在glbdata_mgr.lock_data和glbdata_mgr.unlock_data间调用
function _get_glblvlmisrnk_data()
{
    local glbdata_mgr = global_data_mgrs.glbdata;
    local glblvlmisrnk_data = glbdata_mgr.get_data(db_glbdata_datatype.DBGDT_LVLMIS_RNK_INFO);
    if (!glblvlmisrnk_data)
    {
        // 尚未获取过侠客行每关霸主、首破、最近通关玩家信息

        // 获取侠客行每关霸主、首破、最近通关玩家信息
        glblvlmisrnk_data = db_obj.select_data_obj("glbdata", "datatp", (db_glbdata_datatype.DBGDT_LVLMIS_RNK_INFO).tostring());
        if (!glblvlmisrnk_data || glblvlmisrnk_data.Count <= 0)
        {
            local glblvlmisrnk_info = { datatp = db_glbdata_datatype.DBGDT_LVLMIS_RNK_INFO, data ={ ffin ={ }, bfin ={ }, rcntfin ={ } } };
            glblvlmisrnk_data = db_obj.new_data_obj("glbdata", false);
            glblvlmisrnk_data.mod_data(glblvlmisrnk_info);
            if (!glblvlmisrnk_data.db_insert())
            {
                Utility.trace_err( "glblvlmisrnk_data db insert error\n");
                return null;
            }

            glblvlmisrnk_data = [glblvlmisrnk_data];
        }

        glbdata_mgr.set_data(db_glbdata_datatype.DBGDT_LVLMIS_RNK_INFO, glblvlmisrnk_data);
    }

    return glblvlmisrnk_data[0];
}

// 获取每周荣誉排行榜信息
function _get_w_hexp_rnk_data()
{
    local glbdata_mgr = global_data_mgrs.glbdata;
    local whexprnk_data = glbdata_mgr.get_data(db_glbdata_datatype.DBGDT_W_HEXP_RNK_INFO);
    if (!whexprnk_data)
    {
        // 尚未获取过每周荣誉排行榜信息

        // 获取每周荣誉排行榜信息
        whexprnk_data = db_obj.select_data_obj("glbdata", "datatp", (db_glbdata_datatype.DBGDT_W_HEXP_RNK_INFO).tostring());
        if (!whexprnk_data || whexprnk_data.Count <= 0)
        {
            local glblvlmisrnk_info = { datatp = db_glbdata_datatype.DBGDT_W_HEXP_RNK_INFO, data ={ lw_info =[], cw_info =[], cw_time = sys.time() } };
            whexprnk_data = db_obj.new_data_obj("glbdata", false);
            whexprnk_data.mod_data(glblvlmisrnk_info);
            if (!whexprnk_data.db_insert())
            {
                Utility.trace_err( "whexprnk_data db insert error\n");
                return null;
            }

            whexprnk_data = [whexprnk_data];
        }

        glbdata_mgr.set_data(db_glbdata_datatype.DBGDT_W_HEXP_RNK_INFO, whexprnk_data);
    }

    return whexprnk_data[0];
}

// 获取战场排行榜数据
function _get_battle_rnk_data()
{
    local glbdata_mgr = global_data_mgrs.glbdata;
    local battle_rnk_data = glbdata_mgr.get_data(db_glbdata_datatype.DBGDT_BLT_RNK_INFO);
    if (!battle_rnk_data)
    {
        // 尚未获取过战场排行榜数据

        // 获取战场排行榜数据
        battle_rnk_data = db_obj.select_data_obj("glbdata", "datatp", (db_glbdata_datatype.DBGDT_BLT_RNK_INFO).tostring());
        if (!battle_rnk_data || battle_rnk_data.Count <= 0)
        {
            local glbbattlernk_info = { datatp = db_glbdata_datatype.DBGDT_BLT_RNK_INFO, data ={ linfo ={ }, cinfo ={ } } };
            battle_rnk_data = db_obj.new_data_obj("glbdata", false);
            battle_rnk_data.mod_data(glbbattlernk_info);
            if (!battle_rnk_data.db_insert())
            {
                Utility.trace_err( "battle_rnk_data db insert error\n");
                return null;
            }

            battle_rnk_data = [battle_rnk_data];
        }

        glbdata_mgr.set_data(db_glbdata_datatype.DBGDT_BLT_RNK_INFO, battle_rnk_data);
    }

    return battle_rnk_data[0];
}



// 获取首席大弟子形象信息，必须在glbdata_mgr.lock_data和glbdata_mgr.unlock_data间调用
function _get_carrchief_data()
{
    local glbdata_mgr = global_data_mgrs.glbdata;
    local carrchief_data = glbdata_mgr.get_data(db_glbdata_datatype.DBGDT_CARR_CHIEF);
    if (!carrchief_data)
    {
        // 尚未获取过首席大弟子形象信息

        // 获取首席大弟子形象信息
        carrchief_data = db_obj.select_data_obj("glbdata", "datatp", (db_glbdata_datatype.DBGDT_CARR_CHIEF).tostring());
        if (!carrchief_data || carrchief_data.Count <= 0)
        {
            local carrchief_info = { datatp = db_glbdata_datatype.DBGDT_CARR_CHIEF, data ={ showinfo ={ }, crttm ={ } } };
            carrchief_data = db_obj.new_data_obj("glbdata", false);
            carrchief_data.mod_data(carrchief_info);
            if (!carrchief_data.db_insert())
            {
                Utility.trace_err( "carrchief_data db insert error\n");
                return null;
            }

            carrchief_data = [carrchief_data];
        }

        glbdata_mgr.set_data(db_glbdata_datatype.DBGDT_CARR_CHIEF, carrchief_data);
    }

    return carrchief_data[0];
}

// 获取帮派领地信息，必须在glbdata_mgr.lock_data和glbdata_mgr.unlock_data间调用
function _get_clanter_data()
{
    local glbdata_mgr = global_data_mgrs.glbdata;
    local clanter_data = glbdata_mgr.get_data(db_glbdata_datatype.DBGDT_CLAN_TER);
    if (!clanter_data)
    {
        // 尚未获取过帮派领地信息

        // 获取帮派领地信息
        clanter_data = db_obj.select_data_obj("glbdata", "datatp", (db_glbdata_datatype.DBGDT_CLAN_TER).tostring());
        if (!clanter_data || clanter_data.Count <= 0)
        {
            local clanter_info = { datatp = db_glbdata_datatype.DBGDT_CLAN_TER, data ={ } };  // data[cltid] = {crttm=, showinfo=, clanid=, onwar=}
            clanter_data = db_obj.new_data_obj("glbdata", false);
            clanter_data.mod_data(clanter_info);
            if (!clanter_data.db_insert())
            {
                Utility.trace_err( "clan_ter db insert error\n");
                return null;
            }

            clanter_data = [clanter_data];
        }
        else
        {
            local clanter_info = clanter_data[0].get_data();
            foreach (val in clanter_info.data)
            {
                val.onwar = false; // 第一次读取数据时争夺战必然未开启
            }
            clanter_data[0].mod_data(clanter_info);
        }

        glbdata_mgr.set_data(db_glbdata_datatype.DBGDT_CLAN_TER, clanter_data);
    }

    return clanter_data[0];
}

function _get_clanter_info(cltid)
{
    local clanter_info = null;
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CLAN_TER, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local gld_clanter_data = _get_clanter_data();
    if (gld_clanter_data)
    {
        local glb_clanter_info = gld_clanter_data.get_data();
        if (cltid in glb_clanter_info.data)
        {
            clanter_info = glb_clanter_info.data[cltid];
        }
    }
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁

    return clanter_info;
}


function _check_lvl_requirement(ply, lvl_conf, difflvl_conf = null, cost_tp= null)
{
    if ("attchk" in lvl_conf)
    {
        foreach (attchk in lvl_conf.attchk)
        {
            local res = ply.check_att(attchk);
            if (res != game_err_code.RES_OK)
            {
                return res;
            }
        }
    }

    if (difflvl_conf && "attchk" in difflvl_conf )
    {
        foreach (attchk in difflvl_conf.attchk)
        {
            local res = ply.check_att(attchk);
            if (res != game_err_code.RES_OK)
            {
                return res;
            }
        }
    }

    if ("clan" in lvl_conf)
    {
        if (!("clanid" in ply.pinfo))
        {
            return game_err_code.NOT_IN_CLAN;
        }

        local clan_mgr = global_data_mgrs.clan;
        clan_mgr.lock_data(ply.pinfo.clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

        local clan_db_data = _get_clan_data(ply.pinfo.clanid);
        if (clan_db_data.Count <= 0)
        {
            // 目标帮派不存在
            clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
            return game_err_code.CLAN_NOT_EXIST;
        }

        local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
        if (!clan_pl)
        {
            // 不是帮派成员
            clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
            return game_err_code.NOT_IN_CLAN;
        }

        local clan_db_info = clan_db_data[0].get_data();
        local clan_pl_info = clan_pl.data.get_data();

        if (clan_db_info.lvl < lvl_conf.clan[0].lvl)
        {
            // 帮派等级不够
            clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
            return game_err_code.CLAN_LVL_REQUIRE;
        }

        if (lvl_conf.clangld > 0)
        {
            if (clan_pl_info.clang < lvl_conf.clangld)
            {
                clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
                return game_err_code.NOT_ENOUGH_CLANG;
            }
        }

        clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁

        // 增加帮派排名判断clan[0].minrnk，根据get_clans_list代码封装函数获取帮派排名列表，取前minrnk名可参加副本，否则返回错误代码
        local res = _check_lvl_clan_rnk_req(ply, lvl_conf.clan[0]);
        if (res != game_err_code.RES_OK)
        {
            return res;
        }
    }

    if (lvl_conf.premis != 0)
    {
        local fined_mis = ply.get_fined_mis(lvl_conf.premis);
        if (!fined_mis)
        {
            return game_err_code.LEVEL_PREMIS_REQUIRE;
        }
    }

    if (("acceptmis" in lvl_conf) && lvl_conf.acceptmis != 0)
    {
        local acept_mis = ply.get_accept_mis(lvl_conf.acceptmis);
        if (!acept_mis)
        {
            return game_err_code.LEVEL_ACCEPT_MIS_REQUIRE;
        }
    }

    if (lvl_conf.act_v > 0)
    {
        if (ply.pinfo.act_v < lvl_conf.act_v)
        {
            // 判断vip体力值
            //if(ply.pinfo.vip <= 0)
            //{
            //    return game_err_code.NOT_ENOUGH_ACT_V;
            //}

            //if(!("vip_data" in ply.pinfo))
            //{
            //    return game_err_code.NOT_ENOUGH_ACT_V;
            //}

            //if(ply.pinfo.vip_data.lmiscnt < lvl_conf.act_v)
            //{
            return game_err_code.NOT_ENOUGH_ACT_V;
            //}
        }
    }

    if (lvl_conf.hexp > 0)
    {
        if (ply.pinfo.hexp < lvl_conf.hexp)
        {
            return game_err_code.NOT_ENOUGH_HONER;
        }
    }

    local reqitm = null;
    local expenditm = null;
    local gld_cost = lvl_conf.gld;
    local yb_cost = lvl_conf.yb;

    if ("reqitm" in lvl_conf) reqitm = lvl_conf.reqitm;
    if ("expenditm" in lvl_conf) expenditm = lvl_conf.expenditm;

    local cost_conf;
    if (difflvl_conf && ("cost" in difflvl_conf) )
    {
        cost_conf = difflvl_conf.cost[0];
    }
    if (cost_tp)
    {
        if (!difflvl_conf)
        {
            return game_err_code.CONFIG_ERR;
        }

        if (!("cost_ch" in difflvl_conf) )
        {
            return game_err_code.ENTER_LVL_COST_CONF_ERR;
        }

        if (!(cost_tp in difflvl_conf.cost_ch ) )
        {
            return game_err_code.ENTER_LVL_COST_TP_ERR;
        }
        cost_conf = difflvl_conf.cost_ch[cost_tp].cost[0];
    }
    if (cost_conf)
    {
        if ("reqitm" in cost_conf) reqitm = cost_conf.reqitm;
        if ("expenditm" in cost_conf) expenditm = cost_conf.expenditm;
        if ("gld" in cost_conf) gld_cost = cost_conf.gld;
        if ("yb" in cost_conf) yb_cost = cost_conf.yb;
    }

    if (gld_cost > 0)
    {
        if (ply.pinfo.gold < lvl_conf.gld)
        {
            return game_err_code.NOT_ENOUGH_GOLD;
        }
    }

    if (yb_cost > 0)
    {
        if (ply.pinfo.yb < lvl_conf.yb)
        {
            return game_err_code.NOT_ENOUGH_YB;
        }
    }

    if (reqitm)
    {
        foreach (val in reqitm)
        {
            if (ply.get_item_total_count(val.tpid) <= 0)
            {
                return game_err_code.LEVEL_ITEM_REQUIRE;
            }
        }
    }

    if (expenditm)
    {
        foreach (val in expenditm)
        {
            if (!ply.have_cost_item(val.tpid, val.cnt))
            {
                return game_err_code.LEVEL_ITEM_REQUIRE;
            }
        }
    }

    if ("vip" in lvl_conf)
    {
        if (ply.pinfo.vip < lvl_conf.vip)
        {
            return game_err_code.VIP_LVL_NOT_ENOUGH;
        }
    }

    local lvlmis_conf = get_lvlmis_conf(lvl_conf.tpid);
    if (lvlmis_conf)
    {
        // 侠客行任务副本
        if (lvlmis_conf.prelvlmis > 0 && !(lvlmis_conf.prelvlmis in ply.pinfo.lvlmis.fined))
        {
            // 需要前置完成其他侠客行副本任务
            return game_err_code.LVLMIS_PREMIS_REQUIRE;
        }
    }

    return game_err_code.RES_OK;
}

// 检查副本帮派排行条件
function _check_lvl_clan_rnk_req(ply, clan_conf)
{
    if (!("minrnk" in clan_conf))
    {
        // minrnk是可选配置，存在才进行排名检查
        return game_err_code.RES_OK;
    }

    local clans = _get_clans_list();
    //sys.dumpobj(clans);
    if (!clans || !clans.clans || !clans.clans.Count)
    {
        // 不可能进入该代码分支，否则出错
        return game_err_code.INTERNAL_ERR; // 出错，返回失败消息，否则都可以进入该副本了
    }

    // 检查帮派排名是否在前minrnk名
    local is_found = false;
    local sub_clans = [];
    for (local i = 0; (i < clan_conf.minrnk) && (i < clans.clans.Count); ++i)
    {
        local val = clans.clans[i];
        if (val.id == ply.pinfo.clanid)
        {
            is_found = true;
            break;
        }
    }

    if (!is_found)
    {
        return game_err_code.CLAN_RNK_NOT_ENOUGH;
    }
    return game_err_code.RES_OK;
}

function _check_arena_requirement(ply, arena_conf)
{
    if ("attchk" in arena_conf)
    {
        foreach (attchk in arena_conf.attchk)
        {
            local res = ply.check_att(attchk);
            if (res != game_err_code.RES_OK)
            {
                return res;
            }
        }
    }

    return game_err_code.RES_OK;
}
function _check_carrchief_requirement(ply, carrchief_conf)
{
    if ("attchk" in carrchief_conf)
    {
        foreach (attchk in carrchief_conf.attchk)
        {
            local res = ply.check_att(attchk);
            if (res != game_err_code.RES_OK)
            {
                return res;
            }
        }
    }

    return game_err_code.RES_OK;
}

function _check_clantrit_req(ply, clantrit_conf)
{
    if (!("clanid" in ply.pinfo))
    {
        return game_err_code.NOT_IN_CLAN;
    }

    if ("attchk" in clantrit_conf)
    {
        foreach (attchk in clantrit_conf.attchk)
        {
            local res = ply.check_att(attchk);
            if (res != game_err_code.RES_OK)
            {
                return res;
            }
        }
    }

    local clan_mgr = global_data_mgrs.clan;
    clan_mgr.lock_data(ply.pinfo.clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

    local clan_db_data = _get_clan_data(ply.pinfo.clanid);
    if (clan_db_data.Count <= 0)
    {
        // 目标帮派不存在
        clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
        return game_err_code.CLAN_NOT_EXIST;
    }

    local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
    if (!clan_pl)
    {
        // 不是帮派成员
        clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
        return game_err_code.NOT_IN_CLAN;
    }

    local clan_db_info = clan_db_data[0].get_data();
    local clan_pl_info = clan_pl.data.get_data();

    if ("clanatt_chk" in clantrit_conf)
    {
        if (clan_db_info.lvl < clantrit_conf.clanatt_chk[0].lvl)
        {
            // 帮派等级不够
            clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
            return game_err_code.CLAN_LVL_REQUIRE;
        }
    }

    clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁

    if (clantrit_conf.tp == clan_teritory_type.CTT_WAR_TERRITORY)
    {
        // 需要争夺的帮派领地，判断帮派是否拥有该领地

        local is_my_clanter = true;

        local is_clanter_war_start = false;

        // 获取帮派领地信息
        local clanter_info = _get_clanter_info(clantrit_conf.id);

        if (!clanter_info)
        {
            // 不是你们帮派的领地
            return game_err_code.LEVEL_NOT_YOUR_CLAN_TER;
        }

        if (clanter_info.clanid != ply.pinfo.clanid)
        {
            // 不是你们帮派的领地
            return game_err_code.LEVEL_NOT_YOUR_CLAN_TER;
        }

        if (clanter_info.onwar)
        {
            // 争夺战进行中，不能进入领地
            return game_err_code.LEVEL_CLAN_TER_ON_WAR;
        }
    }

    return game_err_code.RES_OK;
}

function _check_cltwar_req(ply, clantrit_conf)
{
    if (!("clanid" in ply.pinfo))
    {
        return game_err_code.NOT_IN_CLAN;
    }

    if ("attchk" in clantrit_conf)
    {
        foreach (attchk in clantrit_conf.attchk)
        {
            local res = ply.check_att(attchk);
            if (res != game_err_code.RES_OK)
            {
                return res;
            }
        }
    }

    local clan_mgr = global_data_mgrs.clan;
    clan_mgr.lock_data(ply.pinfo.clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

    local clan_db_data = _get_clan_data(ply.pinfo.clanid);
    if (clan_db_data.Count <= 0)
    {
        // 目标帮派不存在
        clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
        return game_err_code.CLAN_NOT_EXIST;
    }

    local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
    if (!clan_pl)
    {
        // 不是帮派成员
        clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
        return game_err_code.NOT_IN_CLAN;
    }

    local clan_db_info = clan_db_data[0].get_data();
    local clan_pl_info = clan_pl.data.get_data();

    if ("clanatt_chk" in clantrit_conf)
    {
        if (clan_db_info.lvl < clantrit_conf.clanatt_chk[0].lvl)
        {
            // 帮派等级不够
            clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
            return game_err_code.CLAN_LVL_REQUIRE;
        }
    }

    clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁

    //申请的帮派才能进
    if ("war_req" in clantrit_conf )
    {
        local clanter_info = _get_clanter_info(clantrit_conf.id);
        //没帮派占领时 不需要申请
        if (clanter_info && clanter_info.clanid > 0)
        {
            if (clanter_info.clanid != ply.pinfo.clanid)
            {// 攻城方需要 申请才能进
                local request_clter = false;
                if ("war_reqs" in clanter_info ) 
                {
                    foreach (clanid in clanter_info.war_reqs)
                    {
                        if (clanid == ply.pinfo.clanid)
                        {
                            request_clter = true;
                            break;
                        }
                    }
                }
                if (!request_clter)
                {
                    return game_err_code.CLANTER_CLAN_NOT_REQUEST;
                }
            }
            else
            {// 守城方 没有帮派申请时 才能进
                if (!("war_reqs" in clanter_info) || clanter_info.war_reqs.Count == 0 )
                {
                    return game_err_code.CLANTER_CLAN_NOT_WAR;
                }
            }
        }
    }

    return game_err_code.RES_OK;
}

// 消耗进入副本需要的帮派货币
function _lvl_cost_clangld(ply, lvl_conf)
{
    if ("clan" in lvl_conf)
    {
        if (!("clanid" in ply.pinfo))
        {
            return game_err_code.NOT_IN_CLAN;
        }

        local clan_mgr = global_data_mgrs.clan;
        clan_mgr.lock_data(ply.pinfo.clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

        local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
        if (!clan_pl)
        {
            // 不是帮派成员
            clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
            return game_err_code.NOT_IN_CLAN;
        }

        local clan_pl_info = clan_pl.data.get_data();

        if (lvl_conf.clangld > 0)
        {
            if (clan_pl_info.clang < lvl_conf.clangld)
            {
                clan_mgr.unlock_data(clanid); // 解锁
                return game_err_code.NOT_ENOUGH_CLANG;
            }

            clan_pl_info.clang -= lvl_conf.clangld;

            // 更新帮派成员数据
            clan_pl.data.mod_data(clan_pl_info);
            clan_pl.data.db_update(); // 刷新至数据库

            // send self_attchange to client
            //::send_rpc(ply.pinfo.sid, 32, {clangadd=-lvl_conf.clangld}); 在enter_level时和hexp变化一起发送
        }

        clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
    }

    return game_err_code.RES_OK;
}

function _lvl_checkin_clan(llid, clanid, lvl_conf)
{
    if (!level.is_clanid_marked(llid, clanid))
    {
        // 未签入，判断是否有签入空间
        local cur_cnt = level.get_marked_clid_cnt(llid);

        local max_cnt = 1;
        if ("pvp" in lvl_conf)
        {
            local pvp_conf = lvl_conf.pvp[0];
            if ("side" in pvp_conf)
            {
                max_cnt = pvp_conf.side.Count; // 可签入帮派数量等于阵营数
            }
            else if ("death_match" in pvp_conf)
            {
                local death_match_conf = pvp_conf.death_match[0];
                if ("side" in death_match_conf)
                {
                    max_cnt = death_match_conf.side.Count; // 可签入帮派数量等于阵营数
                }
            }
            else if ("death_hold_map" in pvp_conf)
            {
                local death_hold_map_conf = pvp_conf.death_hold_map[0];
                if ("side" in death_hold_map_conf)
                {
                    max_cnt = death_hold_map_conf.side.Count; // 可签入帮派数量等于阵营数
                }
            }
        }

        if (cur_cnt < max_cnt)
        {
            // 签入帮派
            level.mark_clanid(llid, clanid);
        }
        else
        {
            // 签入帮派数量已满
            return false;
        }
    }

    return true;
}

function _check_lvl_tmchk(tmchks )
{
    // TO DO : check tmchk
    local local_tm = sys.get_local_time();
    //sys.dumpobj(local_tm);
    //sys.dumpobj(tmchks);
    local ret = game_err_code.RES_OK;
    foreach (tmchk in tmchks)
    {
        if ("tb" in tmchk && _compare_abs_tm(local_tm, tmchk.tb) < 0)
        {
            ret = game_err_code.LEVEL_TM_NOT_REACHED;
            continue;
        }
        if ("te" in tmchk && _compare_abs_tm(local_tm, tmchk.te) > 0)
        {
            ret = game_err_code.LEVEL_TM_FINISHED;
            continue;
        }
        if ("dtb" in tmchk && _compare_day_tm(local_tm, tmchk.dtb) < 0)
        {
            ret = game_err_code.LEVEL_TM_NOT_REACHED;
            continue;
        }
        if ("dte" in tmchk && _compare_day_tm(local_tm, tmchk.dte) > 0)
        {
            ret = game_err_code.LEVEL_TM_FINISHED;
            continue;
        }
        if ("wtb" in tmchk && local_tm.wd < tmchk.wtb)
        {
            ret = game_err_code.LEVEL_TM_NOT_REACHED;
            continue;
        }
        if ("wte" in tmchk && local_tm.wd >= tmchk.wte)
        {
            ret = game_err_code.LEVEL_TM_FINISHED;
            continue;
        }
        if ("wd" in tmchk)
        {
            local wd_find = false;
            foreach (wd in tmchk.wd)
            {
                if (local_tm.wd == wd)
                {
                    wd_find = true;
                    break;
                }
            }
            if (!wd_find)
            {
                ret = game_err_code.LEVEL_TM_NOT_REACHED;
                continue;
            }
        }
        if ("optm" in tmchk)
        {
            local opentm = game_data_conf.first_rnkact_tm + tmchk.optm * 86400;
            local open_local_tm = sys.trans_local_time(opentm);

            if (_compare_ymd_tm(local_tm, open_local_tm) < 0)
            {
                ret = game_err_code.LEVEL_TM_NOT_REACHED;
                continue;
            }
        }
        if ("cltm" in tmchk)
        {
            local closetm = game_data_conf.first_rnkact_tm + tmchk.cltm * 86400;
            local close_local_tm = sys.trans_local_time(closetm);

            if (_compare_ymd_tm(local_tm, close_local_tm) > 0)
            {
                ret = game_err_code.LEVEL_TM_FINISHED;
                continue;
            }
        }
        if ("cb_optm" in tmchk)
        {
            local opentm = game_data_conf.combsvr_ract_tm + tmchk.cb_optm * 86400;
            local open_local_tm = sys.trans_local_time(opentm);

            if (_compare_ymd_tm(local_tm, open_local_tm) < 0)
            {
                ret = game_err_code.LEVEL_TM_NOT_REACHED;
                continue;
            }
        }
        if ("cb_cltm" in tmchk)
        {
            local closetm = game_data_conf.combsvr_ract_tm + tmchk.cb_cltm * 86400;
            local close_local_tm = sys.trans_local_time(closetm);

            if (_compare_ymd_tm(local_tm, close_local_tm) > 0)
            {
                ret = game_err_code.LEVEL_TM_FINISHED;
                continue;
            }
        }

        ret = game_err_code.RES_OK;
        break;
    }
    return ret;
}

// 检查竞技场报名排队情况并撮合比赛 
function _check_arena_match()
{
    level.lock_aline(); // 锁定竞技场队列，锁定之后，所有退出代码必须解锁

    if (level.get_aline_cnt() <= 0)
    {
        // 没有队列
        level.unlock_aline(); // 解锁竞技场队列
        return;
    }

    local alines = level.get_alines();
    //sys.dumpobj(alines);

    foreach (arenaid, aline in alines)
    {
        // 获取角色竞技场队列信息，并撮合比赛
        local arena_conf = get_arena_conf(arenaid);
        if (!arena_conf)
        {
            continue;
        }

        if (aline.Count <= 0)
        {
            // 无人排队
            continue;
        }

        // 检查有效期
        if ("tmchk" in arena_conf)
        {
            local res = _check_lvl_tmchk(arena_conf.tmchk);
            if (res != game_err_code.RES_OK)
            {
                // 有效期已过

                // 清除排队队列
                foreach (pls in aline)
                {
                    level.rmv_aline_pl(arenaid, pls.cid);

                    // 客户端会关闭入口，无需通知客户端取消排队
                    //local sid = svr.get_sid_by_cid(pls.cid);
                    //if(sid != 0)
                    //{
                    //    // send check_in_lvl_res msg
                    //    ::send_rpc(sid, 240, {acancel=true});
                    //}
                }

                //sys.trace(sys.SLT_DETAIL, "clear arenaid["+arenaid+"] aline_pl\n");

                continue;
            }
        }

        local match_conf = arena_conf.match[0];

        if (aline.Count < match_conf.min_wait_ply || aline.Count < match_conf.plycnt)
        {
            // 最少开启人数不足
            continue;
        }

        local sort_func = function(a, b)
        {
            if (a.pt > b.pt)
            {
                return -1;
            }
            else if (a.pt < b.pt)
            {
                return 1;
            }
            return 0;
        }
        aline.sort(sort_func);

        for (local idx = 0; idx < aline.Count; ++idx)
        {
            local pl = aline[idx];

            if (svr.get_sid_by_cid(pl.cid) <= 0)
            {
                aline.remove(idx);
                --idx;
            }
        }

        local i = 0;
        while (aline.Count - i >= match_conf.plycnt)
        {
            // 按积分排序，匹配
            local lvl_pls = [];
            local j = 0;
            for (j = 0; j < match_conf.plycnt; ++j)
            {
                lvl_pls.push(aline[i + j]);
            }
            i += j;

            // 撮合成功，创建副本并通知客户端

            // 随机选择副本
            local idx = sys.rand_between(0, arena_conf.lvl.Count);
            local ltpid = arena_conf.lvl[idx].tpid;

            // 服务器创建副本难度固定为1
            local llid = level.create_lvl(ltpid, 1);
            if (llid <= 0)
            {
                // create error, send lvl_err_msg
                Utility.trace_err( "_check_arena_match create_lvl ltpid[" + ltpid + "] err\n");
                continue;
            }

            // 标记创建者
            level.mark_lvl_creator(llid, 0, level_multi_type.LMT_MULTI);

            if (lvl_pls.Count == 2)
            {
                foreach (idx,pls in lvl_pls)
                {
                    level.add_ply_to_lvl(llid, pls.cid);
                    level.mark_arena_pl(arenaid, pls.cid);

                    level.rmv_aline_pl(arenaid, pls.cid);

                    local sid = svr.get_sid_by_cid(pls.cid);
                    if (sid != 0)
                    {
                        // send check_in_lvl_res msg
                        local match_pl = lvl_pls[(idx == 0) ? 1 : 0];
                        //sys.dumpobj( match_pl );
                        ::send_rpc(sid, 240, { llid = llid, ltpid = ltpid, lmtp = level_multi_type.LMT_MULTI, match = true,is_first = false, m_ply = match_pl});
                    }
                }
            }
            else
            {
                foreach (pls in lvl_pls)
                {
                    level.add_ply_to_lvl(llid, pls.cid);
                    level.mark_arena_pl(arenaid, pls.cid);

                    level.rmv_aline_pl(arenaid, pls.cid);

                    local sid = svr.get_sid_by_cid(pls.cid);
                    if (sid != 0)
                    {
                        // send check_in_lvl_res msg
                        ::send_rpc(sid, 240, { llid = llid, ltpid = ltpid, lmtp = level_multi_type.LMT_MULTI, match = true,is_first = false});
                    }
                }
            }
        }
    }

    level.unlock_aline(); // 解锁竞技场队列
}

// 获取竞技场决赛创建信息，必须在glbdata_mgr.lock_data和glbdata_mgr.unlock_data间调用
function _get_chlvl_crt_data()
{
    local glbdata_mgr = global_data_mgrs.glbdata;
    local chlvl_crt_data = glbdata_mgr.get_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR);
    if (!chlvl_crt_data)
    {
        // 尚未获取过竞技场决赛创建信息

        // 获取竞技场决赛创建信息
        chlvl_crt_data = db_obj.select_data_obj("glbdata", "datatp", (db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR).tostring());
        if (!chlvl_crt_data || chlvl_crt_data.Count <= 0)
        {
            local chlvl_crt_info = { datatp = db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR, data ={ crttm ={ } } };
            chlvl_crt_data = db_obj.new_data_obj("glbdata", false);
            chlvl_crt_data.mod_data(chlvl_crt_info);
            if (!chlvl_crt_data.db_insert())
            {
                Utility.trace_err( "chlvl_crt_data db insert error\n");
                return null;
            }

            chlvl_crt_data = [chlvl_crt_data];
        }

        glbdata_mgr.set_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR, chlvl_crt_data);
    }

    return chlvl_crt_data[0];
}


function check_in_lvl(sid, rpc, ply)
{
    if ("arenaid" in rpc)
    {
        // 签入竞技场

        if (ply.is_in_lvl)
        {
            ::send_rpc(sid, 239 { res = game_err_code.LEVEL_ALREADY_IN_LEVEL});
            return;
        }

        if (ply.has_mission_buf())
        {
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CANT_ENTER_WITH_MISBUF});
            return;
        }

        local arena_conf = get_arena_conf(rpc.arenaid);
        if (!arena_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
            return;
        }

        // 根据arena_conf判断是否满足进入竞技场条件
        local res = _check_arena_requirement(ply, arena_conf);
        if (res != game_err_code.RES_OK)
        {
            // requirement error, send lvl_err_msg
            ::send_rpc(sid, 239, { res = res});
            return;
        }

        if (game_data_conf.is_battle_svr)
        {
            if (!("battle_lvl" in arena_conf) || arena_conf.battle_lvl != 1 )
            {
                ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
                return;
            }
        }
        else
        {
            if (("battle_lvl" in arena_conf) && arena_conf.battle_lvl == 1 )
            {
                if (rpc.chlvl)//目前不处理决赛
                {
                    ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
                    return;
                }

                bsclient_send_bs_check_in_lvl(sid, rpc, ply);
                return;
            }
        }


        if (rpc.chlvl)
        {
            // 签入决赛

            local chlvl_conf = arena_conf.chlvl;

            // 检查有效期
            if ("tmchk" in chlvl_conf)
            {
                local res = _check_lvl_tmchk(chlvl_conf.tmchk);
                if (res != game_err_code.RES_OK)
                {
                    ::send_rpc(sid, 239, { res = res});
                    return;
                }
            }

            local cur_tm_s = sys.time();

            // 判断本轮决赛是否已创建过
            local glbdata_mgr = global_data_mgrs.glbdata;
            glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
            local glb_chlvl_crt_data = _get_chlvl_crt_data();
            if (glb_chlvl_crt_data)
            {
                local glb_chlvl_crt_info = glb_chlvl_crt_data.get_data();

                if (arena_conf.id in glb_chlvl_crt_info.data.crttm)
                {
                    local crt_tm = glb_chlvl_crt_info.data.crttm[arena_conf.id];
                    if (cur_tm_s - crt_tm < chlvl_conf.crtholdtm)
                    {
                        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR); // 解锁
                        ::send_rpc(sid, 239, { res = game_err_code.ARENA_CHLVL_FINISH});
                        return;
                    }
                }
            }
            glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR);  // 解锁


            // 检查排行榜排名
            local rnk_data = _get_rank_info({ rnk_tp = gdctrl_type.GT_RANK_ARENA_PT, sub_tp = arena_conf.id});

            local in_board = false;
            for (local i = 0; i < rnk_data.rnkary.Count && i < chlvl_conf.plycnt; ++i)
            {
                local rnk_info = rnk_data.rnkary[i];
                if (rnk_info.cid == ply.pinfo.cid)
                {
                    in_board = true;
                    break;
                }
            }

            if (!in_board)
            {
                // 不在参加决赛排名范围内
                ::send_rpc(sid, 239, { res = game_err_code.ARENA_JOIN_CHLVL_NOT_QUALIFY});
                return;
            }

            local lvl_conf = get_level_conf(chlvl_conf.tpid);
            if (!lvl_conf)
            {
                // err, config not exist, send lvl_err_msg
                ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
                return;
            }

            // 检查决赛副本是否创建
            local llid = level.get_achlvl_llid(arena_conf.id);
            if (llid == 0)
            {
                // 未创建过该副本，创建

                // 服务器创建副本难度固定为1
                llid = level.create_lvl(chlvl_conf.tpid, 1);
                if (llid <= 0)
                {
                    // create error, send lvl_err_msg
                    Utility.trace_err( "check_in_lvl ltpid[" + chlvl_conf.tpid + "] create err\n");
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CREATE_ERROR});
                    return;
                }

                //// 标记创建者
                //level.mark_lvl_creator(llid, 0, level_multi_type.LMT_MULTI);

                // 标记决赛副本
                level.mark_achlvl_llid(arena_conf.id, llid);
            }

            local is_first = false;
            if (!level.is_ply_in_lvl(llid, ply.pinfo.cid))
            {
                if (level.get_ply_cnt(llid) >= chlvl_conf.plycnt)
                {
                    // 人数已满
                    ::send_rpc(sid, 239, { res = game_err_code.ARENA_CHLVL_PLY_FULL});
                    return;
                }

                level.add_ply_to_lvl(llid, ply.pinfo.cid);

                local lvlentried = ply.get_entried_lvl(lvl_conf.tpid);
                if (!lvlentried)
                {
                    lvlentried = { ecnt = 1, fcnt = 0, diff_lvl = 1, score = 0, besttm = 0};
                    ply.pinfo.lvlentried[lvl_conf.tpid] < -lvlentried;
                }
                else
                {
                    ++lvlentried.ecnt;
                }
                is_first = true;
            }

            // send check_in_lvl_res msg
            ::send_rpc(sid, 240, { llid = llid, ltpid = chlvl_conf.tpid, lmtp = lvl_conf.lmtp, match = false, is_first = is_first});
        }
        else
        {
            // 签入积分赛

            if (rpc.cancel)
            {
                // 取消签入

                level.lock_aline(); // 锁定竞技场队列，锁定之后，所有退出代码必须解锁

                level.rmv_aline_pl(rpc.arenaid, ply.pinfo.cid);

                level.unlock_aline(); // 解锁竞技场队列

                // send check_in_lvl_res msg
                ::send_rpc(sid, 240, { acancel = true});

                ply.pinfo.arenaid = 0;
            }
            else
            {
                // 检查有效期
                if ("tmchk" in arena_conf)
                {
                    local res = _check_lvl_tmchk(arena_conf.tmchk);
                    if (res != game_err_code.RES_OK)
                    {
                        ::send_rpc(sid, 239, { res = res});
                        return;
                    }
                }

                local pinfo = ply.pinfo;
                // 获取角色竞技场相关信息
                local arenapl_info = level.get_arenapl_data_add(db_obj.get_envid(), arena_conf.id, pinfo.cid,
                    { id = 0, cid = pinfo.cid, arenaid = arena_conf.id, pt = arena_conf.init_pt, tpt = arena_conf.init_pt, fcnt = 0, tfcnt = 0, win = 0, lose = 0});

                local pl_arena_info = { cid = pinfo.cid, carr = pinfo.carr, crttm = pinfo.crttm, cwin = pinfo.arena_cwin, carrlvl = pinfo.carrlvl, level = pinfo.level, name = pinfo.name, pt = arenapl_info.pt };

                if (game_data_conf.is_battle_svr)
                {
                    pl_arena_info.svrid < -pinfo.svrid;
                    pl_arena_info.svrnm < -pinfo.svrnm;
                }

                level.lock_aline(); // 锁定竞技场队列，锁定之后，所有退出代码必须解锁

                if (level.get_arena_pl_mark(ply.pinfo.cid) > 0)
                {
                    if (!ply.is_in_lvl)
                    {
                        level.rmv_arena_pl_mark(ply.pinfo.cid);
                    }
                    else
                    {
                        ::send_rpc(sid, 239, { res = game_err_code.ARENA_ALREADY_IN});
                        level.unlock_aline(); // 解锁竞技场队列                    
                        return;
                    }
                }

                if (!level.checkin_aline(rpc.arenaid, ply.pinfo.cid, pl_arena_info))
                {
                    // 签入失败
                    ::send_rpc(sid, 239, { res = game_err_code.ARENA_ALREADY_CHECKED_IN});
                    level.unlock_aline(); // 解锁竞技场队列
                    return;
                }

                level.unlock_aline(); // 解锁竞技场队列

                // 根据在线人数估算等待时间
                local status = game_data.get_status();
                local ev_tm = (300 - (60 * status.logedin_user / 200)).tointeger();
                if (ev_tm <= 0) ev_tm = 60;

                ::send_rpc(sid, 240, { arenaid = rpc.arenaid, tm = ev_tm});

                ply.pinfo.arenaid = rpc.arenaid;
            }
        }
    }
    else if ("ltpid" in  rpc)
    {
        // 签入副本
        if (_is_functional_block(gdctrl_type.GT_BLOCK_LVL, rpc.ltpid))
        {
            ::send_rpc(sid, 239, { res = game_err_code.FUNCTIONAL_TEMP_SHUTDOWN})
            return;
        }

        local lvl_conf = get_level_conf(rpc.ltpid);
        if (!lvl_conf)
        {
            // err, config not exist, send lvl_err_msg
            ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
            return;
        }

        if (lvl_conf.lctp != level_checkin_type.LCT_AUTO_CREATE)
        {
            // 非服务器创建副本，不能check in
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CANT_CHECK_IN});
            return;
        }

        if ("arenaid" in lvl_conf)
        {
            // 副本只能通过匹配方式进入
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ONLY_MATCH_ENTER});
            return;
        }

        if ("cltid" in lvl_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CLAN_ENTER});
            return;
        }

        if ("carr_chief" in  lvl_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NPC_NO_LVL_ENTRY});
            return;
        }

        if ("clanconq" in lvl_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CLAN_ENTER});
            return;
        }

        if ("slvl_diff" in lvl_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
            return;
        }

        local crtid = 0;
        local clanraid = false;

        if ("clanraid" in lvl_conf)
        {
            // 与帮派绑定的pve副本
            if (!("clanid" in ply.pinfo))
            {
                ::send_rpc(sid, 239, { res = game_err_code.NOT_IN_CLAN});
                return;
            }

            crtid = ply.pinfo.clanid;
            clanraid = true;
        }

        if (game_data_conf.is_battle_svr)
        {
            if (!("battle_lvl" in lvl_conf) || lvl_conf.battle_lvl != 1 )
            {
                ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
                return;
            }
        }
        else
        {
            if (("battle_lvl" in lvl_conf) && lvl_conf.battle_lvl == 1 )
            {
                bsclient_send_bs_check_in_lvl(sid, rpc, ply);
                return;
            }
        }

        local llid = level.get_llid_by_ltpid(rpc.ltpid, crtid, lvl_conf.lmtp);

        if (llid > 0 && level.is_ply_in_lvl(llid, ply.pinfo.cid))
        {
            // 已经在副本中了，直接返回签入成功消息

            // send check_in_lvl_res msg
            ::send_rpc(sid, 240, { llid = llid, ltpid = rpc.ltpid, lmtp = lvl_conf.lmtp, match = false, is_first = false});
            return;
        }

        // 检查有效期
        if ("tmchk" in lvl_conf)
        {
            local res = _check_lvl_tmchk(lvl_conf.tmchk);
            if (res != game_err_code.RES_OK)
            {
                ::send_rpc(sid, 239, { res = res});
                return;
            }
        }

        //跨服战 不检测角色条件  因为此时没有角色详细信息 留到进入时检测
        if (!game_data_conf.is_battle_svr)
        {
            local res = _check_lvl_requirement(ply, lvl_conf);
            if (res != game_err_code.RES_OK)
            {
                // requirement error, send lvl_err_msg
                ::send_rpc(sid, 239, { res = res});
                return;
            }

            local lvlentried = ply.get_entried_lvl(rpc.ltpid);

            if (lvlentried && ("cntleft" in lvlentried) && lvlentried.cntleft <= 0)
            {
                // 进入副本次数已达到上限
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
                return;
            }
        }

        local tid = team.get_ply_teamid(ply.pinfo.cid);

        if (llid <= 0)
        {
            // 创建副本

            if (lvl_conf.lmtp == level_multi_type.LMT_SINGLE)
            {
                // 检查单人副本创建条件
            }
            else if (lvl_conf.lmtp == level_multi_type.LMT_TEAM)
            {
                // 检查组队副本创建条件

                if (tid <= 0)
                {
                    // requirement error, send lvl_err_msg
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_TEAM});
                    return;
                }
            }
            //else if(lvl_conf.lmtp == level_multi_type.LMT_CLAN)
            //{
            //    // TO DO : 检查帮派副本创建条件
            //}
            else if (lvl_conf.lmtp == level_multi_type.LMT_MULTI)
            {
                // TO DO : 检查多人副本创建条件
            }

            if (clanraid)
            {
                // 帮派raid副本，必须帮主或长老才能创建

                local clan_mgr = global_data_mgrs.clan;
                clan_mgr.lock_data(ply.pinfo.clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

                local clan_db_data = _get_clan_data(ply.pinfo.clanid);
                if (clan_db_data.Count <= 0)
                {
                    // 目标帮派不存在
                    ::send_rpc(sid, 239, { res = game_err_code.CLAN_NOT_EXIST});
                    clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
                    return;
                }

                local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
                if (!clan_pl)
                {
                    // 不是帮派成员
                    ::send_rpc(sid, 239, { res = game_err_code.NOT_IN_CLAN});
                    clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
                    return;
                }

                clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁

                if (clan_pl.clanc < clan_c_tp.CCT_VP)
                {
                    // 帮派权限不够
                    ::send_rpc(sid, 239, { res = game_err_code.CLAN_RAID_NOT_CRT});
                    return;
                }
            }

            // 服务器创建副本难度固定为1
            llid = level.create_lvl(rpc.ltpid, 1);
            if (llid <= 0)
            {
                // create error, send lvl_err_msg
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CREATE_ERROR});
                return;
            }

            // 标记创建者
            level.mark_lvl_creator(llid, crtid, lvl_conf.lmtp);
        }

        // 判断帮派是否签入
        if ("clan" in lvl_conf)
        {
            if (!_lvl_checkin_clan(llid, ply.pinfo.clanid, lvl_conf))
            {
                // 已达到最大帮派数
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CLAN_NOT_CHECKIN});
                return;
            }
        }

        local ply_cnt = level.get_ply_cnt(llid);
        if (ply_cnt >= lvl_conf.maxply)
        {
            // 已达到最大玩家数
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_MAX_PLAYER_REACHED});
            return;
        }

        // send check_in_lvl_res msg
        ::send_rpc(sid, 240, { llid = llid, ltpid = rpc.ltpid, lmtp = lvl_conf.lmtp, match = false, is_first = true});
    }
    else if ("clteid" in rpc)
    {
        // 签入帮派领地

        if (ply.is_in_lvl)
        {
            ::send_rpc(sid, 239 { res = game_err_code.LEVEL_ALREADY_IN_LEVEL});
            return;
        }

        local clantrit_conf = get_clantrit_conf(rpc.clteid);
        if (!clantrit_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
            return;
        }

        if (rpc.war)
        {
            // 进入领地争夺战场

            if (clantrit_conf.tp != clan_teritory_type.CTT_WAR_TERRITORY || !("warlvl" in clantrit_conf))
            {
                // 无需争夺的帮派领地
                return game_err_code.PARAMETER_ERR;
            }

            local warlvl_conf = clantrit_conf.warlvl;

            // 检查有效期
            if ("tmchk" in warlvl_conf)
            {
                local res = _check_lvl_tmchk(warlvl_conf.tmchk);
                if (res != game_err_code.RES_OK)
                {
                    ::send_rpc(sid, 239, { res = res});
                    return;
                }
            }

            local cur_tm_s = sys.time();

            // 根据clantrit_conf判断是否满足进入帮派领地争夺战场条件
            local res = _check_cltwar_req(ply, clantrit_conf);
            if (res != game_err_code.RES_OK)
            {
                // requirement error, send lvl_err_msg
                ::send_rpc(sid, 239, { res = res});
                return;
            }

            local lvl_conf = get_level_conf(warlvl_conf.tpid);
            if (!lvl_conf)
            {
                // err, config not exist, send lvl_err_msg
                ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
                return;
            }

            local llid = level.get_llid_by_ltpid(warlvl_conf.tpid, 0, lvl_conf.lmtp);

            if (llid <= 0)
            {
                // 检查本轮领地争夺战是否创建

                // 获取帮派领地信息
                local clanter_info = _get_clanter_info(clantrit_conf.id);

                if (clanter_info)
                {
                    if (clanter_info.crttm + warlvl_conf.crtholdtm > cur_tm_s)
                    {
                        // 本轮领地争夺战已结束
                        ::send_rpc(sid, 239, { res = game_err_code.CLAN_TER_WAR_FIN});
                        return;
                    }
                }

                // 未创建
                // 服务器创建副本难度固定为1
                llid = level.create_lvl(warlvl_conf.tpid, 1);
                if (llid <= 0)
                {
                    // create error, send lvl_err_msg
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CREATE_ERROR});
                    return;
                }

                // 标记创建者
                level.mark_lvl_creator(llid, 0, lvl_conf.lmtp);
            }

            local is_first = false;
            if (!level.is_ply_in_lvl(llid, ply.pinfo.cid))
            {

                if (level.get_ply_cnt(llid) >= lvl_conf.maxply)
                {
                    // 人数已满
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_MAX_PLAYER_REACHED});
                    return;
                }

                level.add_ply_to_lvl(llid, ply.pinfo.cid);

                local lvlentried = ply.get_entried_lvl(lvl_conf.tpid);
                if (!lvlentried)
                {
                    lvlentried = { ecnt = 1, fcnt = 0, diff_lvl = 1, score = 0, besttm = 0};
                    ply.pinfo.lvlentried[lvl_conf.tpid] < -lvlentried;
                }
                else
                {
                    ++lvlentried.ecnt;
                }
                is_first = true;
            }

            // send check_in_lvl_res msg
            ::send_rpc(sid, 240, { llid = llid, ltpid = warlvl_conf.tpid, lmtp = lvl_conf.lmtp, match = false, is_first = is_first});
        }
        else
        {
            // 进入帮派领地

            // 根据clantrit_conf判断是否满足进入帮派领地条件
            local res = _check_clantrit_req(ply, clantrit_conf);
            if (res != game_err_code.RES_OK)
            {
                // requirement error, send lvl_err_msg
                ::send_rpc(sid, 239, { res = res});
                return;
            }

            local lvl_conf = get_level_conf(clantrit_conf.lvl_tpid);
            if (!lvl_conf)
            {
                // err, config not exist, send lvl_err_msg
                ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
                return;
            }

            // 检查该领地副本是否已创建
            local llid = level.get_clan_terit(clantrit_conf.id, ply.pinfo.clanid);
            if (llid == 0)
            {
                // 未创建过该副本，创建

                // 服务器创建副本难度固定为1
                llid = level.create_lvl(clantrit_conf.lvl_tpid, 1);
                if (llid <= 0)
                {
                    // create error, send lvl_err_msg
                    Utility.trace_err( "check_in_lvl ltpid[" + clantrit_conf.lvl_tpid + "] create err\n");
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CREATE_ERROR});
                    return;
                }

                //// 标记创建者
                level.mark_lvl_creator(llid, ply.pinfo.clanid, level_multi_type.LMT_MULTI);

                level.mark_clan_terit(clantrit_conf.id, ply.pinfo.clanid, llid);
            }

            if (!level.is_ply_in_lvl(llid, ply.pinfo.cid))
            {
                level.add_ply_to_lvl(llid, ply.pinfo.cid);
            }

            // send check_in_lvl_res msg
            ::send_rpc(sid, 240, { llid = llid, ltpid = clantrit_conf.lvl_tpid, lmtp = lvl_conf.lmtp, match = false, is_first = false});
        }
    }
    else if ("carrc" in rpc)
    {
        // 签入首席大弟子副本

        if (ply.has_mission_buf())
        {
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CANT_ENTER_WITH_MISBUF});
            return;
        }

        local carrchief_conf = get_carrchief_conf(ply.pinfo.carr);
        if (!carrchief_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
            return;
        }

        // 根据carrchief_conf判断是否满足进入副本条件
        local res = _check_carrchief_requirement(ply, carrchief_conf);
        if (res != game_err_code.RES_OK)
        {
            // requirement error, send lvl_err_msg
            ::send_rpc(sid, 239, { res = res});
            return;
        }

        // 检查有效期
        if ("tmchk" in carrchief_conf)
        {
            local res = _check_lvl_tmchk(carrchief_conf.tmchk);
            if (res != game_err_code.RES_OK)
            {
                ::send_rpc(sid, 239, { res = res});
                return;
            }
        }

        local lvl_conf = get_level_conf(carrchief_conf.ltpid);
        if (!lvl_conf)
        {
            // err, config not exist, send lvl_err_msg
            ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
            return;
        }

        local llid = level.get_llid_by_ltpid(carrchief_conf.ltpid, 0, lvl_conf.lmtp);

        if (llid <= 0)
        {
            // 检查本轮首席大弟子副本是否创建
            local cur_tm_s = sys.time();

            local glbdata_mgr = global_data_mgrs.glbdata;
            glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CARR_CHIEF, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
            local glb_carrchief_data = _get_carrchief_data();
            if (glb_carrchief_data)
            {
                local glb_carrchief_info = glb_carrchief_data.get_data();
                if (ply.pinfo.carr in glb_carrchief_info.data.crttm)
                {
                    if (glb_carrchief_info.data.crttm[ply.pinfo.carr] + carrchief_conf.crtholdtm > cur_tm_s)
                    {
                        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CARR_CHIEF); // 解锁
                        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CARRC_LVL_FINISH});
                        return;
                    }
                }
            }
            glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CARR_CHIEF);  // 解锁

            // 未创建
            // 服务器创建副本难度固定为1
            llid = level.create_lvl(carrchief_conf.ltpid, 1);
            if (llid <= 0)
            {
                // create error, send lvl_err_msg
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CREATE_ERROR});
                return;
            }

            // 标记创建者
            level.mark_lvl_creator(llid, 0, lvl_conf.lmtp);
        }

        local is_first = false;
        if (!level.is_ply_in_lvl(llid, ply.pinfo.cid))
        {

            if (level.get_ply_cnt(llid) >= lvl_conf.maxply)
            {
                // 人数已满
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CARRC_LVL_PLY_FULL});
                return;
            }

            level.add_ply_to_lvl(llid, ply.pinfo.cid);

            local lvlentried = ply.get_entried_lvl(lvl_conf.tpid);
            if (!lvlentried)
            {
                lvlentried = { ecnt = 1, fcnt = 0, diff_lvl = 1, score = 0, besttm = 0};
                ply.pinfo.lvlentried[lvl_conf.tpid] < -lvlentried;
            }
            else
            {
                ++lvlentried.ecnt;
            }
            is_first = true;
        }

        // send check_in_lvl_res msg
        ::send_rpc(sid, 240, { llid = llid, ltpid = carrchief_conf.ltpid, lmtp = lvl_conf.lmtp, match = false, is_first = is_first});
    }
    else if ("chl_clanid" in rpc)
    {
        // 签入帮派奴役战场

        if (!("clanid" in ply.pinfo))
        {
            // 不在帮派中
            ::send_rpc(sid, 239, { res = game_err_code.NOT_IN_CLAN});
            return;
        }

        local clanconq_conf = get_clanconq_conf();

        // 检查有效期
        if ("war_tmchk" in clanconq_conf)
        {
            local res = _check_lvl_tmchk(clanconq_conf.war_tmchk);
            if (res != game_err_code.RES_OK)
            {
                ::send_rpc(sid, 239, { res = res});
                return;
            }
        }

        local clanch_info = _get_clanch_info(rpc.chl_clanid);
        if (!clanch_info)
        {
            // 目标帮派无宣战信息
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_NO_CLANCHL_REC});
            return;
        }

        // 检查角色帮派信息

        if (rpc.chl_clanid != ply.pinfo.clanid)
        {
            // 宣战帮派非角色所在帮派，检查是否下属帮派

            if (rpc.chl_clanid != clanch_info.tclid)
            {
                // 目标帮派非防守方帮派，不能协助
                ::send_rpc(sid, 239, { res = game_err_code.CANT_ENTER_OTHER_CLANCONQ});
                return;
            }

            local clan_mgr = global_data_mgrs.clan;
            clan_mgr.lock_data(ply.pinfo.clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

            local clan_db_data = _get_clan_data(ply.pinfo.clanid);
            if (clan_db_data.Count <= 0)
            {
                // 目标帮派不存在
                ::send_rpc(sid, 239, { res = game_err_code.CLAN_NOT_EXIST});
                clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
                return;
            }
            local clan_db_info = clan_db_data[0].get_data();

            clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁

            local isconqed = false;
            foreach (conqedclan in clan_db_info.conq.clans)
            {
                if (conqedclan.id == rpc.chl_clanid)
                {
                    isconqed = true;
                    break;
                }
            }

            if (!isconqed)
            {
                // 目标帮派非自己下属，不能协助
                ::send_rpc(sid, 239, { res = game_err_code.CANT_ENTER_OTHER_CLANCONQ});
                return;
            }
        }

        local lvl_conf = get_level_conf(clanconq_conf.war_lvl);
        if (!lvl_conf)
        {
            // err, config not exist, send lvl_err_msg
            ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
            return;
        }

        local llid = level.get_llid_by_ltpid(lvl_conf.tpid, clanch_info.id, lvl_conf.lmtp);

        if (llid <= 0)
        {
            // 未创建
            // 服务器创建副本难度固定为1
            llid = level.create_lvl(lvl_conf.tpid, 1);
            if (llid <= 0)
            {
                // create error, send lvl_err_msg
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CREATE_ERROR});
                return;
            }

            // 标记创建者，宣战id
            level.mark_lvl_creator(llid, clanch_info.id, lvl_conf.lmtp);
        }

        local is_first = false;
        if (!level.is_ply_in_lvl(llid, ply.pinfo.cid))
        {

            if (level.get_ply_cnt(llid) >= lvl_conf.maxply)
            {
                // 人数已满
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CARRC_LVL_PLY_FULL});
                return;
            }

            level.add_ply_to_lvl(llid, ply.pinfo.cid);

            local lvlentried = ply.get_entried_lvl(lvl_conf.tpid);
            if (!lvlentried)
            {
                lvlentried = { ecnt = 1, fcnt = 0, diff_lvl = 1, score = 0, besttm = 0};
                ply.pinfo.lvlentried[lvl_conf.tpid] < -lvlentried;
            }
            else
            {
                ++lvlentried.ecnt;
            }
            is_first = true;
        }

        // send check_in_lvl_res msg
        ::send_rpc(sid, 240, { llid = llid, ltpid = lvl_conf.tpid, lmtp = lvl_conf.lmtp, match = false, is_first = is_first});
    }
    else if ("slvl_diff" in rpc)
    {
        if (ply.is_in_lvl)
        {
            ::send_rpc(sid, 239 { res = game_err_code.LEVEL_ALREADY_IN_LEVEL});
            return;
        }

        // 签入不同难度的服务器副本
        local slvl_diff_conf = get_slvl_diff_conf(rpc.slvl_diff);
        if (!slvl_diff_conf)
        {
            // 无不同难度的服务器副本配置
            ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
            return;
        }

        local lvl_conf = get_level_conf(slvl_diff_conf.ltpid);
        if (!lvl_conf || !(rpc.diff_lvl in lvl_conf.diff_lvl) )
        {
            // err, config not exist, send lvl_err_msg
            ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
            return;
        }

        if (lvl_conf.lctp != level_checkin_type.LCT_AUTO_CREATE)
        {
            // 非服务器创建副本，不能check in
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CANT_CHECK_IN});
            return;
        }

        // 检查有效期
        if ("tmchk" in lvl_conf )
        {
            local res = _check_lvl_tmchk(lvl_conf.tmchk);
            if (res != game_err_code.RES_OK)
            {
                ::send_rpc(sid, 239, { res = res});
                return;
            }
        }

        local llid = level.get_llid_by_ltpid(lvl_conf.tpid, rpc.diff_lvl, lvl_conf.lmtp);
        if (llid > 0)
        {
            if (level.is_ply_in_lvl(llid, ply.pinfo.cid))
            {
                // 已经在副本中了，直接返回签入成功消息
                // send check_in_lvl_res msg
                ::send_rpc(sid, 240, { llid = llid, ltpid = lvl_conf.tpid, lmtp = lvl_conf.lmtp, diff_lvl = rpc.diff_lvl, match = false, is_first = false});
                return;
            }
            else
            {
                if (level.get_ply_cnt(llid) >= lvl_conf.maxply)
                {
                    // 人数已满
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CARRC_LVL_PLY_FULL});
                    return;
                }
            }
        }

        //是否满足进入副本条件
        local diff_lvl_conf = lvl_conf.diff_lvl[rpc.diff_lvl];
        local res = null;
        local cost_tp;
        if ("cost_tp" in rpc )
        {
            cost_tp = rpc.cost_tp;
            res = _check_lvl_requirement(ply, lvl_conf, diff_lvl_conf, rpc.cost_tp);
        }
        else
        {
            res = _check_lvl_requirement(ply, lvl_conf, diff_lvl_conf);
        }
        if (res != game_err_code.RES_OK)
        {
            // requirement error, send lvl_err_msg
            ::send_rpc(sid, 239, { res = res});
            return;
        }

        if (llid <= 0)
        {
            // 未创建
            // 服务器创建副本难度
            llid = level.create_lvl(lvl_conf.tpid, rpc.diff_lvl);
            if (llid <= 0)
            {
                // create error, send lvl_err_msg
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CREATE_ERROR});
                return;
            }
            // 标记创建者，宣战id
            level.mark_lvl_creator(llid, rpc.diff_lvl, lvl_conf.lmtp);
        }

        local senddata = { llid = llid, ltpid = lvl_conf.tpid, lmtp = lvl_conf.lmtp, diff_lvl = rpc.diff_lvl, match = false, is_first = true };
        if (cost_tp) senddata.cost_tp < -cost_tp;
        // send check_in_lvl_res msg
        ::send_rpc(sid, 240, senddata);
    }
    else
    {
        ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
    }
}

function create_lvl(sid, rpc, ply)
{
    if (_is_functional_block(gdctrl_type.GT_BLOCK_LVL, rpc.ltpid))
    {
        ::send_rpc(sid, 239, { res = game_err_code.FUNCTIONAL_TEMP_SHUTDOWN});
        return;
    }

    local lvl_conf = get_level_conf(rpc.ltpid);
    if (!lvl_conf)
    {
        // err, config not exist, send lvl_err_msg
        ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
        return;
    }

    if (lvl_conf.lctp != level_checkin_type.LCT_USER_CREATE)
    {
        // 非用户创建副本，不能create_lvl
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CANT_CREATE_BY_USER});
        return;
    }

    local roomid = 0;
    if (("room" in lvl_conf ) )
    {
        if (lvl_conf.room != ply.pinfo.room)
        {
            svr.send_rpc(sid, 239, { res = game_err_code.NO_TARGET_ROOM } );
            return;
        }

        if (!is_room_owner(ply))
        {
            svr.send_rpc(sid, 239, { res = game_err_code.MUST_ROOM_OWNER } );
            return;
        }

        roomid = ply.pinfo.roomid;

        local llid = level.get_llid_by_ltpid(rpc.ltpid, roomid, 0);
        if (llid > 0)
        {
            // 该副本已创建过了，直接返回创建成功
            local lvlinfo = level.get_lvl_info(llid);
            svr.send_rpc(sid, 241, { llid = llid, ltpid = rpc.ltpid, diff_lvl = lvlinfo.diff_lvl, creator_cid = lvlinfo.creator, lmtp = lvl_conf.lmtp});
            return;
        }
    }
    if (ply.has_mission_buf())
    {
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CANT_ENTER_WITH_MISBUF});
        return;
    }

    if (("npcentry" in lvl_conf) && lvl_conf.npcentry != 0)
    {
        // 关联NPC
        local npc_conf = get_near_by_npc(rpc.npcid, ply);
        if (!npc_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NPC_NOT_NEAR_BY});
            return;
        }

        if (!("lentry" in npc_conf))
        {
            // err : NPC have no level entry
            ::send_rpc(sid, 239, { res = game_err_code.NPC_NO_LVL_ENTRY});
            return;
        }

        local has_entry = false;
        foreach (tpid in npc_conf.lentry)
        {
            if (tpid == rpc.ltpid)
            {
                has_entry = true;
            }
        }
        if (!has_entry)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NPC_NO_LVL_ENTRY});
            return;
        }
    }

    // 检查有效期
    if ("tmchk" in lvl_conf)
    {
        local res = _check_lvl_tmchk(lvl_conf.tmchk);
        if (res != game_err_code.RES_OK)
        {
            ::send_rpc(sid, 239, { res = res});
            return;
        }
    }

    if (!(rpc.diff_lvl in lvl_conf.diff_lvl) )
    {
        // 创建的副本难度过高
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_TOO_DIFFICULT});
        return;
    }

    local cur_tm_s = sys.time();

    if ("enter_cd" in lvl_conf)
    {
        if ((lvl_conf.enter_cdtp in ply.pinfo.lvlentercd) && ply.pinfo.lvlentercd[lvl_conf.enter_cdtp] > cur_tm_s)
        {
            // 进入副本的冷却时间未到，不能进入, send lvl_err_msg
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTER_CD_NOT_REACH});
            return;
        }
    }

    local diff_lvl_conf = lvl_conf.diff_lvl[rpc.diff_lvl];
    local res = _check_lvl_requirement(ply, lvl_conf, diff_lvl_conf);
    if (res != game_err_code.RES_OK)
    {
        // requirement error, send lvl_err_msg
        ::send_rpc(sid, 239, { res = res});
        return;
    }

    local lvlentried = ply.get_entried_lvl(rpc.ltpid);

    if (lvlentried && ("cntleft" in lvlentried) && lvlentried.cntleft <= 0)
    {
        // 进入副本次数已达到上限

        // 检查是否可以vip进入
        if (ply.pinfo.vip <= 0)
        {
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }

        if (!("vip_data" in ply.pinfo))
        {
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }

        if (!("lvlcnt" in ply.pinfo.vip_data))
        {
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }

        local vip_lvlcnt = null;
        foreach (lvlcnt in ply.pinfo.vip_data.lvlcnt)
        {
            if (lvlcnt.ltpid == rpc.ltpid && lvlcnt.cnt > 0)
            {
                vip_lvlcnt = lvlcnt;
            }
        }

        if (!vip_lvlcnt)
        {
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }

        if (!(vip_lvlcnt.ltpid in ply.pinfo.vip_conf.lvlcnt))
        {
            ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
            return;
        }

        local yb_cost = 0;
        if (lvl_conf.yb > 0)
        {
            yb_cost = lvl_conf.yb;
        }
        if (diff_lvl_conf && ("cost" in diff_lvl_conf) )
        {
            local cost_conf = diff_lvl_conf.cost[0];
            if ("yb" in cost_conf) 
            {
                yb_cost = cost_conf.yb;
            }
        }

        // 消耗vip进入次数，需消耗元宝
        local vip_lvl_conf = ply.pinfo.vip_conf.lvlcnt[vip_lvlcnt.ltpid];
        yb_cost += vip_lvl_conf.yb;

        if (ply.pinfo.yb < yb_cost)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NOT_ENOUGH_YB});
            return;
        }
    }

    local tid = 0;
    local tleader_cid = 0;
    if (lvl_conf.lmtp == level_multi_type.LMT_SINGLE)
    {
        local llid = level.get_llid_by_ltpid(rpc.ltpid, ply.pinfo.cid, level_multi_type.LMT_SINGLE);
        if (llid > 0)
        {
            // 该副本已创建过了，直接返回创建成功
            local lvlinfo = level.get_lvl_info(llid);
            ::send_rpc(sid, 241, { llid = llid, ltpid = rpc.ltpid, diff_lvl = lvlinfo.diff_lvl, creator_cid = lvlinfo.creator, lmtp = lvl_conf.lmtp});
            return;
        }
        //if(level.is_lvl_exist(rpc.ltpid, ply.pinfo.cid, level_multi_type.LMT_SINGLE))
        //{
        //    // 该副本已创建过了，不能再次创建
        //    ::send_rpc(sid, 239, {res=game_err_code.LEVEL_ALREADY_CREATED});
        //    return;
        //}
    }
    else if (lvl_conf.lmtp == level_multi_type.LMT_TEAM)
    {
        tid = team.get_ply_teamid(ply.pinfo.cid);

        if (tid <= 0)
        {
            // requirement error, send lvl_err_msg
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_TEAM});
            return;
        }

        local llid = level.get_llid_by_ltpid(rpc.ltpid, tid, level_multi_type.LMT_TEAM);
        if (llid > 0)
        {
            // 该副本已创建过了，直接返回创建成功
            local lvlinfo = level.get_lvl_info(llid);
            ::send_rpc(sid, 241, { llid = llid, ltpid = rpc.ltpid, diff_lvl = lvlinfo.diff_lvl, creator_cid = lvlinfo.creator, lmtp = lvl_conf.lmtp});
            return;
        }
        //if(level.is_lvl_exist(rpc.ltpid, tid, level_multi_type.LMT_TEAM))
        //{
        //    // 该副本已创建过了，不能再次创建
        //    ::send_rpc(sid, 239, {res=game_err_code.LEVEL_ALREADY_CREATED});
        //    return;
        //}

        tleader_cid = team.get_leader_cid(tid);
        if (tleader_cid != ply.pinfo.cid)
        {
            // requirement error, send lvl_err_msg
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_NEED_TEAM_LEADER_CRT});
            return;
        }
    }
    //else if(lvl_conf.lmtp == level_multi_type.LMT_CLAN)
    //{
    //    // TO DO : 检查帮派副本创建条件
    //}
    else if (lvl_conf.lmtp == level_multi_type.LMT_MULTI)
    {
        // TO DO : 检查多人副本创建条件
    }

    // TO DO : 判断帮派副本创建条件

    //local cur_diff_lvl = 1;
    //if(lvlentried)
    //{
    //    cur_diff_lvl = lvlentried.diff_lvl;
    //}
    //if(rpc.diff_lvl > cur_diff_lvl)
    //{
    //    // 创建的副本难度过高
    //    ::send_rpc(sid, 239, {res=game_err_code.LEVEL_TOO_DIFFICULT});
    //    return;
    //}

    if (rpc.diff_lvl in lvl_conf.diff_lvl)
    {
        // 根据diff_lvl等级判断是否能创建

        local diff_lvl_conf = lvl_conf.diff_lvl[rpc.diff_lvl];

        if ("attchk" in diff_lvl_conf)
        {
            foreach (attchk in diff_lvl_conf.attchk)
            {
                local res = ply.check_att(attchk);
                if (res != game_err_code.RES_OK)
                {
                    ::send_rpc(sid, 239, { res = res});
                    return;
                }
            }
        }
    }
    else
    {
        // 创建的副本难度过高
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_TOO_DIFFICULT});
        return;
    }

    local llid = level.create_lvl(rpc.ltpid, rpc.diff_lvl);

    //::g_debug( "====>>> on level.create_lvl() ltpid:" + rpc.ltpid + ", diff_lvl:" + rpc.diff_lvl );

    if (llid <= 0)
    {
        // requirement error, send lvl_err_msg
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CREATE_ERROR});
        return;
    }

    switch (lvl_conf.lmtp)
    {
        case level_multi_type.LMT_SINGLE:
            {
                level.mark_lvl_creator(llid, ply.pinfo.cid, level_multi_type.LMT_SINGLE);
            }
            break;
        case level_multi_type.LMT_TEAM:
            {
                level.mark_lvl_creator(llid, tid, level_multi_type.LMT_TEAM);

                // 通知其他队友副本创建成功
                team_bcast_rpc_except(tid, 241, { llid = llid, ltpid = rpc.ltpid, diff_lvl = rpc.diff_lvl, creator_cid = ply.pinfo.cid, lmtp = lvl_conf.lmtp}, sid);

                // 取消队伍发布
                team.unpublish(tid, tleader_cid);

                // 设置成需申请加入
                //team.set_dir_join(tid, false); 

                // send team_setting msg to client;
                //team_bcast_rpc(tid, 119, {dir_join=false});

                // 设置成不能申请加入
                team.set_joinable(tid, false);
            }
            break;
        //case level_multi_type.LMT_CLAN:
        //    {
        //        // TO DO : 帮派副本逻辑
        //    }
        //    break;
        case level_multi_type.LMT_MULTI:
            {
                // TO DO : 多人副本逻辑
            }
            break;
    }

    if ("clan" in lvl_conf)
    {
        // 签入帮派
        level.mark_clanid(llid, ply.pinfo.clanid);
    }

    //::send_rpc(sid, 241, {llid=llid, ltpid=rpc.ltpid, diff_lvl = rpc.diff_lvl, creator_cid=ply.pinfo.cid, lmtp=lvl_conf.lmtp});
    local msg = {
        llid=llid,
        ltpid=rpc.ltpid,
        diff_lvl = rpc.diff_lvl,
        creator_cid=ply.pinfo.cid,
        lmtp=lvl_conf.lmtp
    };
    if (roomid > 0)
    { //房间副本
        level.mark_lvl_creator(llid, roomid, 0);
        send_room_id(roomid, 241, msg);
    }
    else
    {
        svr.send_rpc(sid, 241, msg);
    }
}

function enter_lvl(sid, rpc, ply, worldsvr)
{

    if (ply.is_in_lvl)
    {
        ::send_rpc(sid, 239,{ res = game_err_code.LEVEL_ALREADY_IN_LEVEL});
        return;
    }

    local lvlinfo = level.get_lvl_info(rpc.llid);

    if (!lvlinfo)
    {
        // 副本不存在
        // send close_lvl_res
        ::send_rpc(sid, 248, { llid = rpc.llid});
        return;
    }

    local ltpid = lvlinfo.ltpid;

    if (_is_functional_block(gdctrl_type.GT_BLOCK_LVL, ltpid))
    {
        ::send_rpc(sid, 239, { res = game_err_code.FUNCTIONAL_TEMP_SHUTDOWN})
        return;
    }

    local lvl_conf = get_level_conf(ltpid);
    if (!lvl_conf)
    {
        // err, config not exist, send lvl_err_msg
        ::send_rpc(sid, 239, { res = game_err_code.INTERNAL_ERR});
        return;
    }

    if (("room" in lvl_conf ) && lvl_conf.room != ply.pinfo.room  )
    {
        ::send_rpc(sid, 239, { res = game_err_code.NO_TARGET_ROOM } );
        return;
    }
    // 竞技场匹配时允许进入副本
    //level.lock_aline(); // 锁定竞技场队列，锁定之后，所有退出代码必须解锁
    //if(level.get_pl_checkin_arenaid(ply.pinfo.cid) > 0)
    //{
    //    level.unlock_aline(); // 解锁竞技场队列
    //    ::send_rpc(sid, 239, {res=game_err_code.LEVEL_CANT_ENTER_WITH_ACHECK});
    //    return;
    //}
    //level.unlock_aline(); // 解锁竞技场队列

    if (ply.has_mission_buf())
    {
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CANT_ENTER_WITH_MISBUF});
        return;
    }

    switch (lvl_conf.lmtp)
    {
        case level_multi_type.LMT_SINGLE:
            {
                local cid = level.get_lvl_creator(rpc.llid);
                if (cid != ply.pinfo.cid)
                {
                    //ply.pinfo.llid = 0;

                    // player not in level, send lvl_err_msg
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
                    return;
                }
            }
            break;
        case level_multi_type.LMT_TEAM:
            {
                local tid = level.get_lvl_creator(rpc.llid);
                if (tid != team.get_ply_teamid(ply.pinfo.cid))
                {
                    //ply.pinfo.llid = 0;

                    // player not in team, send lvl_err_msg
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_TEAM});
                    return;
                }
            }
            break;
        //case level_multi_type.LMT_CLAN:
        //    {
        //        // TO DO : 检查是否满足进入条件
        //    }
        //    break;
        case level_multi_type.LMT_MULTI:
            {
                // TO DO : 检查是否满足进入条件
            }
            break;
    }

    // TO DO : 判断帮派副本进入条件

    if (("npcentry" in lvl_conf) && lvl_conf.npcentry != 0)
    {
        // 关联NPC
        local npc_conf = get_near_by_npc(rpc.npcid, ply);
        if (!npc_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NPC_NOT_NEAR_BY});
            return;
        }

        if (!("lentry" in npc_conf))
        {
            // err : NPC have no level entry
            ::send_rpc(sid, 239, { res = game_err_code.NPC_NO_LVL_ENTRY});
            return;
        }

        local has_entry = false;
        foreach (tpid in npc_conf.lentry)
        {
            if (tpid == ltpid)
            {
                has_entry = true;
            }
        }
        if (!has_entry)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NPC_NO_LVL_ENTRY});
            return;
        }
    }

    if (("enter_tmchk") in lvl_conf && lvl_conf.enter_tmchk != 0 && ("tmchk" in lvl_conf) )
    {//只有创建时间 才能进入
        local res = _check_lvl_tmchk(lvl_conf.tmchk);
        if (res != game_err_code.RES_OK)
        {
            ::send_rpc(sid, 239, { res = res});
            return;
        }
    }

    local not_in_lvl = !level.is_ply_in_lvl(rpc.llid, ply.pinfo.cid);
    // sys.trace( sys.SLT_DETAIL, "game_data_conf.is_battle_svr = " + game_data_conf.is_battle_svr + " \n" );
    if (game_data_conf.is_battle_svr)
    {
        if (not_in_lvl)
        {
            // 判断帮派是否签入
            if ("clan" in lvl_conf)
            {
                if (!_lvl_checkin_clan(rpc.llid, ply.pinfo.clanid, lvl_conf))
                {
                    // 已达到最大帮派数
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CLAN_NOT_CHECKIN});
                    return;
                }
            }

            local ply_cnt = level.get_ply_cnt(rpc.llid);
            //sys.trace(sys.SLT_DETAIL, "level ply_cnt["+ply_cnt+"]\n");
            if (ply_cnt >= lvl_conf.maxply)
            {
                // 已达到最大玩家数
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_MAX_PLAYER_REACHED});
                return;
            }

            level.add_ply_to_lvl(rpc.llid, ply.pinfo.cid);
        }

        ply.pinfo.llid = rpc.llid;
        ply.flush_db_data(false, false, false);

        level.enter_lvl(ply.pinfo.sid, rpc.llid);
        return;
    }

    if (not_in_lvl)
    {
        // 第一次进入该副本实例

        if ("arenaid" in lvl_conf)
        {
            // 副本只能通过匹配方式进入
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ONLY_MATCH_ENTER});
            return;
        }

        if ("cltid" in lvl_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CLAN_ENTER});
            return;
        }

        if ("carr_chief" in  lvl_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NPC_NO_LVL_ENTRY});
            return;
        }

        if ("clanconq" in lvl_conf)
        {
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CLAN_ENTER});
            return;
        }

        if ("clanraid" in lvl_conf)
        {
            // 与帮派绑定的pve副本
            if (!("clanid" in ply.pinfo))
            {
                ::send_rpc(sid, 239, { res = game_err_code.NOT_IN_CLAN});
                return;
            }

            local crt_clanid = level.get_lvl_creator(rpc.llid);
            if (crt_clanid != ply.pinfo.clanid)
            {
                ::send_rpc(sid, 239, { res = game_err_code.NOT_IN_YOUR_CLAN});
                return;
            }
        }

        local diff_lvl_conf;
        if ("slvl_diff" in lvl_conf )
        {//需要副本难度
            if (!("diff_lvl" in rpc) || !(rpc.diff_lvl in lvl_conf.diff_lvl) )
            {
                ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
                return;
            }

            diff_lvl_conf = lvl_conf.diff_lvl[rpc.diff_lvl];
        }
        else
        {
            if (lvlinfo.diff_lvl in lvl_conf.diff_lvl)
            {
                diff_lvl_conf = lvl_conf.diff_lvl[lvlinfo.diff_lvl];
            }
        }

        local diff_lvl = 1; //默认难度为1
        if (!diff_lvl_conf) diff_lvl = diff_lvl_conf.lv;

        local res = _check_lvl_requirement(ply, lvl_conf, diff_lvl_conf);
        if (res != game_err_code.RES_OK)
        {
            // requirement error, send lvl_err_msg
            ::send_rpc(sid, 239, { res = res});
            return;
        }

        local cur_tm_s = sys.time();

        if ("enter_cd" in lvl_conf)
        {
            if ((lvl_conf.enter_cdtp in ply.pinfo.lvlentercd) && ply.pinfo.lvlentercd[lvl_conf.enter_cdtp] > cur_tm_s)
            {
                // 进入副本的冷却时间未到，不能进入, send lvl_err_msg
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTER_CD_NOT_REACH});
                return;
            }
        }

        // 判断帮派是否签入
        if ("clan" in lvl_conf)
        {
            if (!_lvl_checkin_clan(rpc.llid, ply.pinfo.clanid, lvl_conf))
            {
                // 已达到最大帮派数
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_CLAN_NOT_CHECKIN});
                return;
            }
        }

        local cost_conf = null;
        if (diff_lvl_conf && ("cost" in diff_lvl_conf) )
        {
            cost_conf = diff_lvl_conf.cost[0];
        }
        local vip_lvlcnt = null;
        local lvlentried = ply.get_entried_lvl(ltpid);
        local cost_tp;
        if (lvl_conf.rep <= 0 && lvl_conf.dalyrep <= 0 &&
            diff_lvl_conf && ("cost_ch" in diff_lvl_conf) && !("cost_tp" in rpc) )
        {	//副本未配置次数消耗，已配置cost_tp，则需消耗cost_tp下一种
            ::send_rpc(sid, 239, { res = game_err_code.ENTER_LVL_COST_TP_ERR});
            return;
        }
        if ("cost_tp" in rpc)
        {
            cost_tp = rpc.cost_tp;
            if (!diff_lvl_conf || !("cost_ch" in diff_lvl_conf) )
            {
                ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
                return;
            }
            if (!(cost_tp in diff_lvl_conf.cost_ch ) )
            {
                ::send_rpc(sid, 239, { res = game_err_code.ENTER_LVL_COST_TP_ERR});
                return;
            }
            cost_conf = diff_lvl_conf.cost_ch[cost_tp].cost[0];
        }
        if (!cost_tp)
        {
            if (lvlentried && ("cntleft" in lvlentried) && lvlentried.cntleft <= 0)
            {	// 进入副本次数已达到上限
                // 检查是否可以vip进入
                if (ply.pinfo.vip <= 0)
                {
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
                    return;
                }

                if (!("vip_data" in ply.pinfo))
                {
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
                    return;
                }

                if (!("lvlcnt" in ply.pinfo.vip_data))
                {
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
                    return;
                }

                foreach (lvlcnt in ply.pinfo.vip_data.lvlcnt)
                {
                    if (lvlcnt.ltpid == ltpid && lvlcnt.cnt > 0)
                    {
                        vip_lvlcnt = lvlcnt;
                    }
                }

                if (!vip_lvlcnt)
                {
                    ::send_rpc(sid, 239, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
                    return;
                }
            }
        }

        local expenditm = null;
        local gld_cost = lvl_conf.gld;
        local yb_cost = lvl_conf.yb;
        local batpt_cost = lvl_conf.batpt;

        if ("expenditm" in lvl_conf) expenditm = lvl_conf.expenditm;
        if (cost_conf)
        {
            if ("expenditm" in cost_conf) expenditm = cost_conf.expenditm;
            if ("gld" in cost_conf) gld_cost = cost_conf.gld;
            if ("yb" in cost_conf) yb_cost = cost_conf.yb;
            if ("batpt" in cost_conf) batpt_cost = cost_conf.batpt;
        }

        if (vip_lvlcnt)
        {
            // 消耗vip进入次数，需消耗元宝

            if (!(vip_lvlcnt.ltpid in ply.pinfo.vip_conf.lvlcnt))
            {
                ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
                return;
            }
            local vip_lvl_conf = ply.pinfo.vip_conf.lvlcnt[vip_lvlcnt.ltpid];
            yb_cost += vip_lvl_conf.yb;
        }

        if (ply.pinfo.yb < yb_cost)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NOT_ENOUGH_YB});
            return;
        }

        if (ply.pinfo.gold < gld_cost)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NOT_ENOUGH_GOLD});
            return;
        }

        if (ply.pinfo.batpt < batpt_cost)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NOT_ENOUGH_BATPT});
            return;
        }
        if (expenditm)
        {
            foreach (val in expenditm)
            {
                if (!ply.have_cost_item(val.tpid, val.cnt))
                {   //所需道具不足
                    ::send_rpc(sid, 252, { res = game_err_code.PACKAGE_ITEM_REQUIRE});
                    return;
                }
            }
        }
        // TO DO : 不够严谨，可能存在多个请求同时在不同的线程中处理的情况，则将可能出现副本中玩家多于配置中限制的玩家数量，需通过加锁方式解决
        local ply_cnt = level.get_ply_cnt(rpc.llid);

        //sys.trace(sys.SLT_DETAIL, "level ply_cnt["+ply_cnt+"]\n");

        if (ply_cnt >= lvl_conf.maxply)
        {
            // 已达到最大玩家数
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_MAX_PLAYER_REACHED});
            return;
        }

        // 第一次进入消耗物品、金钱等

        // 扣除帮派货币，放在最开始，有可能扣除失败
        local res = _lvl_cost_clangld(ply, lvl_conf);
        if (res != game_err_code.RES_OK)
        {
            ::send_rpc(sid, 239, { res = res});
            return;
        }

        level.add_ply_to_lvl(rpc.llid, ply.pinfo.cid);

        local self_att_msg = { }
        if (lvl_conf.clangld > 0)
        {
            // 在_lvl_cost_clangld函数中扣除了，这里只合并消息
            self_att_msg.clangadd < - -lvl_conf.clangld;
        }
        if (lvl_conf.hexp > 0)
        {
            ply.sub_hexp(lvl_conf.hexp);
            self_att_msg.hexpadd < - -lvl_conf.hexp;
        }
        if (lvl_conf.act_v > 0)
        {
            //if(ply.pinfo.act_v < lvl_conf.act_v)
            //{
            //    // 扣除vip体力值
            //    if(ply.pinfo.vip > 0 && ("vip_data" in ply.pinfo))
            //    {
            //        ply.pinfo.vip_data.lmiscnt -= lvl_conf.act_v;

            //        // send get_vip_res msg
            //        ::send_rpc(sid, 46, {vip_data={lmiscnt=ply.pinfo.vip_data.lmiscnt}});
            //    }
            //}
            //else
            //{
            ply.sub_act_v(lvl_conf.act_v);
            self_att_msg.act_v < -ply.pinfo.act_v;
            //}
        }
        if (batpt_cost > 0)
        {
            ply.sub_batpt(batpt_cost);
            self_att_msg.batptadd < - -batpt_cost;
        }

        if (self_att_msg.Count > 0)
        {
            // 有变化需要通知

            // send self_attchange msg
            ::send_rpc(sid, 32, self_att_msg);
        }

        local costed_rmvitms = [];
        local costed_rmvitms_cnt = [];
        local modcnt_itm_ary = [];
        local modcnt_itm_ary_cnt = [];

        // 扣除金币花费
        local total_costed = { cnt = 0, rmvids =[], modcnts =[] };
        if (gld_cost > 0)
        {
            ply.sub_gold(gld_cost);
            total_costed.gold < - -gld_cost;

            // log gold
            svr.add_gold_log(gold_act_type.GLDAT_ENTER_LVL, 0, gld_cost);
        }
        if (yb_cost > 0)
        {
            ply.sub_yb(yb_cost);
            total_costed.yb < - -yb_cost;

            // log yb change
            _log_game_log(game_log_type.GLT_yb_log, { cid = ply.pinfo.cid, uid = ply.pinfo.uid, lvl = ply.pinfo.level, mmisid = ply.pinfo.mmisid, tm = cur_tm_s, act = yb_act_type.YAT_ENTER_LVL, sub = yb_cost, parid = ltpid});
            // 越南方需要发送的数据
            local g_conf_obj = sys.get_svr_config();
            if (("posturl" in g_conf_obj) && (g_conf_obj.posturl.Count > 0))
            {
                local par = { tp = 6, platuid = ply.user_info.platuid, platid = ply.user_info.platid, tm = cur_tm_s, yb = ply.pinfo.yb, yb_cost = yb_cost, itmtpid = yb_act_type.YAT_ENTER_LVL, ip = svr.get_session_ip(ply.pinfo.sid) };
                net.url_post(g_conf_obj.posturl + "&svrid=" + g_conf_obj.svrid, "par=" + sys.json_encode(par));
            }
        }
        // 扣除道具
        if (expenditm)
        {
            foreach (val in expenditm)
            {
                local costed = ply.cost_item(val.tpid, val.cnt);

                if (costed.cnt > 0)
                {
                    foreach (val in costed.rmvitms)
                    {
                        costed_rmvitms.push(val.itm);
                        costed_rmvitms_cnt.push(val);
                    }
                    foreach (val in costed.rmvids)
                    {
                        total_costed.rmvids.push(val);
                    }
                    foreach (val in costed.modcnts)
                    {
                        total_costed.modcnts.push(val);
                        modcnt_itm_ary.push(val.itm);
                        modcnt_itm_ary_cnt.push(val);
                    }
                    total_costed.cnt += costed.cnt;
                }
            }
        }
        if (total_costed.cnt > 0 || ("gold" in total_costed) || ("yb" in total_costed))
        {
            if (total_costed.rmvids.Count <= 0)
            {
                delete total_costed.rmvids;
            }

            if (total_costed.modcnts.Count <= 0)
            {
                delete total_costed.modcnts;
            }

            // send item_change msg
            ::send_rpc(sid, 75, total_costed);
        }

        if (!lvlentried)
        {
            lvlentried = { ecnt = 1, fcnt = 0, diff_lvl = diff_lvl, score = 0, besttm = 0};
            local cost_cnt = (cost_tp <= 0) ? 1 : 0;	//次数消耗
            if (lvl_conf.rep > 0)
            {
                lvlentried.cntleft < -lvl_conf.rep - cost_cnt;
            }
            else if (lvl_conf.dalyrep > 0)
            {
                lvlentried.cntleft < -lvl_conf.dalyrep - cost_cnt;
            }
            else if (("weekrep" in lvl_conf) && (lvl_conf.weekrep > 0))
            {
                lvlentried.cntleft < -lvl_conf.weekrep - cost_cnt;
            }
            if (diff_lvl_conf)
            {
                lvlentried.diff_lvl = diff_lvl_conf.lv;
            }
            ply.pinfo.lvlentried[ltpid] < -lvlentried;
        }
        else
        {
            ++lvlentried.ecnt;
            if (!cost_tp)
            {   //次数消耗
                if (vip_lvlcnt)
                {
                    --vip_lvlcnt.cnt;
                    // send get_vip_res msg
                    ::send_rpc(sid, 46, { ltpid = vip_lvlcnt.ltpid, lvlcnt = vip_lvlcnt.cnt});
                }
                else if ("cntleft" in lvlentried)
	            {   //次数消耗的需要统计次数，额外消耗的不影响计算次数
                    --lvlentried.cntleft;
                }
            }
            if (diff_lvl_conf)
            {
                lvlentried.diff_lvl = diff_lvl_conf.lv;
            }
        }

        if ("enter_cd" in lvl_conf)
        {
            // 记录进入冷却时间
            local cdtm = cur_tm_s + lvl_conf.enter_cd;
            ply.pinfo.lvlentercd[lvl_conf.enter_cdtp] < -cdtm;

            // 通知进入冷却时间变化
            // send get_associate_lvls_res msg
            ::send_rpc(sid, 243, { entercds =[{ cdtp = lvl_conf.enter_cdtp, cdtm = cdtm}]});
        }

        // log itm
        if (costed_rmvitms.Count > 0)
        {
            _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_ENTER_LVL_COST, itm_flag_type.IFT_RMV, ltpid, costed_rmvitms, costed_rmvitms_cnt);
        }
        if (modcnt_itm_ary.Count > 0)
        {
            _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_ENTER_LVL_COST, itm_flag_type.IFT_MODCNT, ltpid, modcnt_itm_ary, modcnt_itm_ary_cnt);
        }

        _try_up_cardgame(ply, cardgame_card_type.CCT_LVL, { ltpid = ltpid} );
    }
    else
    {
        local tm_cost_conf = get_tm_cost(lvlinfo.diff_lvl, lvl_conf);
        if (("yb" in tm_cost_conf) && tm_cost_conf.yb > ply.pinfo.yb )
        {
            ::send_rpc(sid, 239, { res = game_err_code.NOT_ENOUGH_YB});
            return;
        }

        if (("gold" in tm_cost_conf) && ply.pinfo.gold < tm_cost_conf.gold)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NOT_ENOUGH_GOLD});
            return;
        }

        if (("batpt" in tm_cost_conf) && ply.pinfo.batpt < tm_cost_conf.batpt)
        {
            ::send_rpc(sid, 239, { res = game_err_code.NOT_ENOUGH_BATPT});
            return;
        }
    }

    local awd_achives = { };
    if ("diff_lvl" in  lvl_conf )
    {
        foreach (diff_lvl in lvl_conf.diff_lvl)
        {
            if (!("achive" in diff_lvl ) ) continue;
            foreach (ach_conf in diff_lvl.achive)
            {
                if (ach_conf.id in awd_achives ) continue;
                awd_achives[ach_conf.id] < -ach_conf.id;
            }
        }
    }

    if (!game_data_conf.is_battle_svr)
    {//进入副本前 断开连接
        bsclient_connect_stop(sid, ply);
    }


    if ("clan_territory" in  game_data_conf.level )
    {
        foreach (clan_territory in game_data_conf.level.clan_territory)
        {
            if (!("warlvl" in clan_territory) || !("win_awd" in clan_territory.warlvl ) ) continue;
            foreach (awd in clan_territory.warlvl.win_awd)
            {
                if (!("achive" in awd ) ) continue;
                foreach (ach_conf in awd.achive)
                {
                    awd_achives[ach_conf.id] < -ach_conf.id;
                }
            }
        }
    }
    //::g_dump( " game_data_conf.level.clan_territory",  game_data_conf.level.clan_territory  );
    //::g_debug( " awd_achives.Count: ", awd_achives.Count );
    //::g_dump( " awd_achives:", awd_achives );

    // foreach( achid in awd_achives )
    // {
    //     rnkact_on_achive_entry( ply, achid );
    // }

    ply.pinfo.llid = rpc.llid;
    ply.flush_db_data(false, false, false);

    //::g_debug( "====>>>level.enter_lvl() llid:" + rpc.llid );

    level.enter_lvl(ply.pinfo.sid, rpc.llid);

}

function get_tm_cost(lvl, lvl_conf )
{
    //::g_debug( "====>>> get_tm_cost()  lvl:" + lvl );
    //::g_dump( "副本 配置 ===>>",  lvl_conf );

    if (!("tm_cost" in lvl_conf) ) return { };
    //::g_dump( "副本 diff_lvlSSS 配置 ===>>", lvl_conf.diff_lvl );

    local diff = lvl_conf.diff_lvl[lvl];
    //::g_dump( "副本 diff_lvl 配置===>>lvl:" + lvl, diff );

    if (!diff || !("tm_cost" in diff) ) return lvl_conf.tm_cost;

    //::g_dump( " get_tm_cost===>>lvl: " + lvl,  diff.tm_cost );

    return diff.tm_cost;
}


function get_associate_lvls_res(sid, rpc, ply)
{
    local ret = [];

    if (rpc.lmtp == 0 || rpc.lmtp == level_multi_type.LMT_SINGLE)
    {
        // 获取单人副本信息
        local single_lvl_ary = level.get_lvl_exist_infos(ply.pinfo.cid, level_multi_type.LMT_SINGLE);
        foreach (info in single_lvl_ary)
        {
            ret.push(info);
        }
    }

    if (rpc.lmtp == 0 || rpc.lmtp == level_multi_type.LMT_TEAM)
    {
        // 获取组队副本信息
        local tid = team.get_ply_teamid(ply.pinfo.cid);
        if (tid > 0)
        {
            local team_lvl_ary = level.get_lvl_exist_infos(tid, level_multi_type.LMT_TEAM);
            foreach (info in team_lvl_ary)
            {
                ret.push(info);
            }
        }
    }

    local arenaid = 0;
    if (rpc.lmtp == 0 || rpc.lmtp == level_multi_type.LMT_MULTI)
    {
        // 获取多人副本信息
        local multi_lvl_ary = level.get_lvl_exist_infos_cid(0, level_multi_type.LMT_MULTI, ply.pinfo.cid);
        foreach (info in multi_lvl_ary)
        {
            ret.push(info);
        }
    }

    //if(rpc.lmtp == 0 || rpc.lmtp == level_multi_type.LMT_CLAN)
    //{
    //    // 获取帮派副本信息
    //    if("clanid" in ply.pinfo && ply.pinfo.clanid > 0)
    //    {
    //        local clan_lvl_ary = level.get_lvl_exist_infos(ply.pinfo.clanid, level_multi_type.LMT_CLAN);
    //        foreach(info in clan_lvl_ary)
    //        {
    //            ret.push(info);
    //        }
    //    }
    //}

    // send get_associate_lvls_res msg
    local ret_msg = { lvls = ret };

    if (("entercd" in rpc) && rpc.entercd)
    {
        ret_msg.entercds < - [];
        foreach (cdtp, cdtm in ply.pinfo.lvlentercd)
        {
            ret_msg.entercds.push({ cdtp = cdtp, cdtm = cdtm});
        }
    }

    ::send_rpc(ply.pinfo.sid, 243, ret_msg);
}
function get_lvl_cnt_info_res(sid, rpc, ply)
{
    if ("ltpid" in rpc)
    {
        // 获取指定副本的完成情况

        local ret_msg = { lvls =[] };

        if (rpc.ltpid in ply.pinfo.lvlentried)
        {
            local info = ply.pinfo.lvlentried[rpc.ltpid];

            local lvl = { ltpid = rpc.ltpid, diff_lvl = info.diff_lvl, score = info.score, besttm = info.besttm };
            if ("cntleft" in info)
            {
                lvl.cntleft < -info.cntleft;
            }
            if ("fin_diff" in info)
            {
                lvl.fin_diff < -info.fin_diff;
            }
            ret_msg.lvls.push(lvl);
        }
        else
        {
            ret_msg.ltpid < -rpc.ltpid;
        }

        // send get_lvl_cnt_info_res msg
        ::send_rpc(ply.pinfo.sid, 244, ret_msg);
    }
    else
    {
        local ret = [];

        foreach (ltpid, info in ply.pinfo.lvlentried)
        {
            if (!("cntleft" in info) && (info.diff_lvl == 1))
            {
                continue;
            }

            local lvl = { ltpid = ltpid, diff_lvl = info.diff_lvl, score = info.score, besttm = info.besttm };
            if ("cntleft" in info)
            {
                lvl.cntleft < -info.cntleft;
            }
            if ("fin_diff" in info)
            {
                lvl.fin_diff < -info.fin_diff;
            }
            ret.push(lvl);
        }

        // send get_lvl_cnt_info_res msg
        ::send_rpc(ply.pinfo.sid, 244, { lvls = ret});
    }
}
function get_lvl_prize(sid, rpc, ply)
{
    //if(!ply.is_in_lvl)
    //{
    //    // send lvl_err_msg msg
    //    ::send_rpc(sid, 239, {res=game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
    //    return;
    //}

    //if(!ply.gmap || !ply.gmap.worldsvr)
    //{
    //    // send lvl_err_msg msg
    //    ::send_rpc(sid, 239, {res=game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
    //    return;
    //}

    //local lvl = ply.gmap.worldsvr;

    //if(!(rpc.ltpid in ply.pinfo.lvlprize))
    //{
    //    // send lvl_err_msg msg
    //    ::send_rpc(sid, 239, {res=game_err_code.LEVEL_NO_PRIZE});
    //    return;
    //}

    //local prize = ply.pinfo.lvlprize[rpc.ltpid];

    if (!("ltpid" in rpc))
    {
        if (!ply.is_in_lvl)
        {
            // send lvl_err_msg msg
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
            return;
        }

        if (!ply.gmap || !ply.gmap.worldsvr)
        {
            // send lvl_err_msg msg
            ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
            return;
        }

        rpc.ltpid < -ply.gmap.worldsvr.ltpid;
    }

    local prz_idx = 0;
    local prize = null;
    foreach (idx, prz in ply.pinfo.lvlprize)
    {
        if (prz.ltpid == rpc.ltpid)
        {
            prz_idx = idx;
            prize = prz;
            break;
        }
    }

    if (!prize)
    {
        // send lvl_err_msg msg
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_NO_PRIZE});
        return;
    }

    local itmtpid = 0;
    local cnt = 0;
    local ybcost = 0;
    local force_bnd = false;

    if ("tp" in prize)
    {
        // 转盘奖励
        itmtpid = prize.tp;
        cnt = prize.cnt;
        if (("bnd" in prize) && prize.bnd == 1 )
        {
            force_bnd = true;
        }
    }
    else
    {
        // 翻牌奖励
        local lvl_conf = get_level_conf(prize.ltpid);
        if (!lvl_conf)
        {
            ply.pinfo.lvlprize.remove(prz_idx);

            // send lvl_err_msg msg
            ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
            return;
        }

        if (!(prize.diff_lvl in lvl_conf.diff_lvl))
        {
            ply.pinfo.lvlprize.remove(prz_idx);

            // send lvl_err_msg msg
            ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
            return;
        }

        local diff_lvl_conf = lvl_conf.diff_lvl[prize.diff_lvl];

        if (!("awd_itm" in diff_lvl_conf) || diff_lvl_conf.awd_itm.Count <= 0)
        {
            ply.pinfo.lvlprize.remove(prz_idx);

            // send lvl_err_msg msg
            ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
            return;
        }

        local awd_itms = sys.deep_clone(diff_lvl_conf.awd_itm);

        if ("skipnor" in rpc)
        {
            // 忽略普通奖励卡牌
            local game_conf = get_general_game_conf();
            if (ply.pinfo.yb < game_conf.lvlprize_skipnor_yb)
            {
                // send lvl_err_msg msg
                ::send_rpc(sid, 239, { res = game_err_code.NOT_ENOUGH_YB});
                return;
            }

            ybcost = game_conf.lvlprize_skipnor_yb;

            for (local idx = awd_itms.Count - 1; idx >= 0; --idx)
            {
                local awdconf = awd_itms[idx];

                if (awdconf.lvl == 1)
                {
                    awd_itms.remove(idx);
                }
            }
        }

        // 选择卡牌组
        local total_rate = 0;
        foreach (awdconf in awd_itms)
        {
            total_rate += awdconf.rate;
        }

        local awdgrp_conf = awd_itms[0];
        local judg = sys.rand_between(0, total_rate);
        local cur_rate = 0;

        foreach (awdconf in awd_itms)
        {
            cur_rate += awdconf.rate;

            if (judg < cur_rate)
            {
                awdgrp_conf = awdconf;
                break;
            }
        }

        // 选择卡牌组中道具
        local awditms_conf = awdgrp_conf.drop_itm;
        total_rate = 0;
        foreach (itmconf in awditms_conf)
        {
            total_rate += itmconf.rate;
        }

        local awditm_conf = awditms_conf[0];
        judg = sys.rand_between(0, total_rate);
        cur_rate = 0;

        foreach (awdconf in awditms_conf)
        {
            cur_rate += awdconf.rate;

            if (judg < cur_rate)
            {
                awditm_conf = awdconf;
                break;
            }
        }

        itmtpid = awditm_conf.itmid;
        cnt = awditm_conf.cnt;

        if ("bnd" in awditm_conf && awditm_conf.bnd == 1)
        {
            force_bnd = true;
        }
    }

    // 给奖励
    local item_conf = get_item_conf(itmtpid);
    if (!item_conf)
    {
        // send lvl_err_msg msg
        ::send_rpc(sid, 239, { res = game_err_code.CONFIG_ERR});
        return;
    }

    local grid_left = ply.cha_info.item.maxi - (ply.pinfo.items.Count + ply.pinfo.ncitems.Count + ply.pinfo.eqpitems.Count);
    if (grid_left <= 0)
    {
        // err : not enough package space
        ::send_rpc(sid, 239, { res = game_err_code.PACKAGE_SPACE_NOT_ENOUGH});
        return;
    }

    local ret_data = _pre_add_item(ply, item_conf, cnt, grid_left);
    if (ret_data.res != game_err_code.RES_OK)
    {
        // pre add item err
        ::send_rpc(sid, 239, { res = ret_data.res});
        return;
    }

    local cur_tm_s = sys.time();
    local itm_change_rpc_data = { flag = item_change_msg_flag.ICMF_LVL_PRIZE };

    // 扣元宝
    if (ybcost > 0)
    {
        ply.sub_yb(ybcost);

        itm_change_rpc_data.yb < - -ybcost;

        // log yb change
        _log_game_log(game_log_type.GLT_yb_log, { cid = ply.pinfo.cid, uid = ply.pinfo.uid, lvl = ply.pinfo.level, mmisid = ply.pinfo.mmisid, tm = cur_tm_s, act = yb_act_type.YAT_LVLPRIZESKIPNOR, sub = ybcost, parid = rpc.ltpid});
        // 越南方需要发送的数据
        local g_conf_obj = sys.get_svr_config();
        if (("posturl" in g_conf_obj) && (g_conf_obj.posturl.Count > 0))
        {
            local par = { tp = 6, platuid = ply.user_info.platuid, platid = ply.user_info.platid, tm = cur_tm_s, yb = ply.pinfo.yb, yb_cost = ybcost, itmtpid = yb_act_type.YAT_LVLPRIZESKIPNOR, ip = svr.get_session_ip(ply.pinfo.sid) };
            net.url_post(g_conf_obj.posturl + "&svrid=" + g_conf_obj.svrid, "par=" + sys.json_encode(par));
        }
    }

    ply.pinfo.lvlprize.remove(prz_idx);

    local new_items_ary = _add_item(ply, item_conf, ret_data);
    if (force_bnd)
    {
        foreach (itm in new_items_ary)
        {
            itm.bnd = true;
        }
    }

    if (new_items_ary.Count > 0)
    {
        itm_change_rpc_data.add < -new_items_ary;

        ply.flush_db_data(false, false); // 同步数据至管理器

        // send item_change msg
        ::send_rpc(ply.pinfo.sid, 75, itm_change_rpc_data);

        local prize_itm = new_items_ary[0];
        local prize_itm_msg = { ltpid = rpc.ltpid, cid = ply.pinfo.cid, tpid = prize_itm.tpid };
        if ("cnt" in prize_itm)
        {
            prize_itm_msg.cnt < -prize_itm.cnt;
        }

        //if((lvl.lvl_conf.lmtp != level_multi_type.LMT_MULTI))
        //{
        //    // send lvl_get_prize_res msg
        //    lvl.broad_cast_msg(246, prize_itm_msg);
        //}
        //else
        {
            // 帮派和多人副本人数较多，不设置成就、得分、难度等级等内容，不广播用户结果

            // send lvl_get_prize_res msg
            ::send_rpc(ply.pinfo.sid, 246, prize_itm_msg);
        }

        local item_conf = get_item_conf(prize_itm.tpid);
        if (item_conf)
        {
            if ("broadcast" in item_conf.conf && item_conf.conf["broadcast"] == 1)
            {
                // 广播获得道具消息
                local msg = { cid = ply.pinfo.cid, name = ply.pinfo.name, tp = bcast_msg_tp.LVL_AWD_ITM, ltpid = rpc.ltpid, itm ={ id = prize_itm.tpid } };
                _broad_cast_sys_msg(msg);
            }
        }

        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_GET_LVL_AWARD, itm_flag_type.IFT_NEW_ADD, rpc.ltpid, new_items_ary, []);
    }

}
function leave_lvl(sid, rpc, ply)
{
    if (!ply.is_in_lvl)
    {
        // send lvl_err_msg msg
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
        return;
    }

    if (!ply.gmap || !ply.gmap.worldsvr)
    {
        // send lvl_err_msg msg
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
        return;
    }

    local lvl = ply.gmap.worldsvr;
    lvl.remove_player(ply.pinfo.sid);
    level.enter_world(ply.pinfo.sid, ply.pinfo.line);
}
function close_lvl(sid, rpc, ply)
{
    if (!ply.is_in_lvl)
    {
        // send lvl_err_msg msg
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
        return;
    }

    if (!ply.gmap || !ply.gmap.worldsvr)
    {
        // send lvl_err_msg msg
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
        return;
    }

    local lvl = ply.gmap.worldsvr;

    switch (lvl.lvl_conf.lmtp)
    {
        case level_multi_type.LMT_SINGLE:
            {
                if (lvl.creator != ply.pinfo.cid)
                {
                // player not in level, send lvl_err_msg
                ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
                    return;
                }
            }
            break;
        case level_multi_type.LMT_TEAM:
            {
                local leader_cid = team.get_leader_cid(lvl.creator);
                if (leader_cid != ply.pinfo.cid)
                {
                // 不是队长，不能关闭副本
                ::send_rpc(sid, 239, { res = game_err_code.NOT_YOUR_TEAM});
                    return;
                }
            }
            break;
        //case level_multi_type.LMT_CLAN:
        //    {
        //        // 帮派副本不能强制关闭
        //        ::send_rpc(sid, 239, {res=game_err_code.PARAMETER_ERR});
        //        return;
        //    }
        //    break;
        case level_multi_type.LMT_MULTI:
            {
            // 多人副本不能强制关闭
            ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
                return;
            }
            break;
    }

    if (lvl.lvl_conf.lctp == level_checkin_type.LCT_AUTO_CREATE)
    {
        // 服务器创建副本不能强制关闭
        ::send_rpc(sid, 239, { res = game_err_code.PARAMETER_ERR});
        return;
    }

    // send close_lvl_res msg
    lvl.broad_cast_msg(248, { llid = lvl.llid});
    lvl.fin();
}

// 获取pvp副本排行信息
function get_lvl_pvpinfo_board(sid, rpc, ply)
{
    if (!ply.is_in_lvl)
    {
        // send lvl_err_msg msg
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
        return;
    }

    if (!ply.gmap || !ply.gmap.worldsvr)
    {
        // send lvl_err_msg msg
        ::send_rpc(sid, 239, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
        return;
    }

    local lvl = ply.gmap.worldsvr;

    switch (rpc.tp)
    {
        case 1: // 获取kpboard
            {   // send lvl_pvpinfo_board_res msg
            ::send_rpc(sid, 237, { tp = 1, infos = lvl.kpboard});
            }
            break;
        case 2: // 获取cltwar
            {
                if (lvl.cltwar)
                {
                    local ret_msg = { tp = 2, max_clanid = lvl.cltwar.max_clanid, max_clanpt = lvl.cltwar.max_clanpt };
                    if (("clanid" in ply.pinfo) && (ply.pinfo.clanid in lvl.cltwar.clanpts))
                {
                        ret_msg.self_clanpt < -lvl.cltwar.clanpts[ply.pinfo.clanid];
                    }
                else
                {
                        ret_msg.self_clanpt < -0;
                    }
                // send lvl_pvpinfo_board_res msg
                ::send_rpc(sid, 237, ret_msg);
                }
            }
            break;
        case 4: // 获取当前地图怪物情况
            {   // send lvl_pvpinfo_board_res msg
                local moncnt = lvl.get_map_mon(rpc.mapid);
            ::send_rpc(sid, 237, { tp = 4, mapid = rpc.mapid, cntleft = moncnt});
            }
            break;
    }
}

// 获取竞技场相关信息
function _arena_info(sid, rpc, ply, arena_conf)
{
    // 获取角色竞技场相关信息
    local arenapl_info = level.get_arenapl_data_add(db_obj.get_envid(), arena_conf.id, ply.pinfo.cid,
        { id = 0, cid = ply.pinfo.cid, arenaid = arena_conf.id, pt = arena_conf.init_pt, tpt = arena_conf.init_pt, fcnt = 0, tfcnt = 0, win = 0, lose = 0});

    local ret_msg = { tp = rpc.tp, arenaid = arena_conf.id, pt = arenapl_info.pt, tpt = arenapl_info.tpt, win = arenapl_info.win, lose = arenapl_info.lose, fcnt = arenapl_info.fcnt, tfcnt = arenapl_info.tfcnt };

    if (arena_conf.id in ply.pinfo.chpawds)
    {
        ret_msg.chpawd < -true;
    }
    else
    {
        ret_msg.chpawd < -false;
    }

    level.lock_aline(); // 锁定竞技场队列，锁定之后，所有退出代码必须解锁
    local chkin_aid = level.get_pl_checkin_arenaid(ply.pinfo.cid);
    level.unlock_aline(); // 解锁竞技场队列

    if (chkin_aid > 0)
    {
        ret_msg.chkin_aid < -chkin_aid;

        // 根据在线人数估算等待时间
        local status = game_data.get_status();
        local ev_tm = (300 - (60 * status.logedin_user / 200)).tointeger();
        if (ev_tm <= 0) ev_tm = 60;

        ret_msg.tm < -ev_tm;
    }

    // 获取决赛副本结束时间
    local glb_chlvl_crt_info = null;
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local glb_chlvl_crt_data = _get_chlvl_crt_data();
    if (glb_chlvl_crt_data)
    {
        glb_chlvl_crt_info = glb_chlvl_crt_data.get_data();
    }
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR);  // 解锁

    if (glb_chlvl_crt_info && (arena_conf.id in glb_chlvl_crt_info.data.crttm))
    {
        ret_msg.chlvl_ftm < -glb_chlvl_crt_info.data.crttm[arena_conf.id];
    }

    // send arena msg
    ::send_rpc(sid, 235, ret_msg);
}

// 领取冠军奖励
function _arena_awd(sid, rpc, ply, arena_conf)
{
    local ret_msg = { arenaid = rpc.arenaid, tp = 2, chpawd = false };

    // 检查配置
    local cur_tm_s = sys.time();
    local game_conf = get_general_game_conf();
    local arena_conf = get_arena_conf(rpc.arenaid);
    if (!game_conf || !arena_conf)
    {
        ::send_rpc(sid, 252, { res = game_err_code.PARAMETER_ERR});      // send error msg
        return;
    }
    if (!("chlvl" in arena_conf))
    {
        ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});      // send error msg
        return;
    }

    // 检查有无奖励
    local chlvl = arena_conf.chlvl;
    if (!(rpc.arenaid in ply.pinfo.chpawds) || (!("awd" in chlvl) && !("achive" in chlvl)))
    {
        ::send_rpc(sid, 235, ret_msg);    // send chpawd msg
        return;
    }

    // 检查超时
    local chpawd_tm = ply.pinfo.chpawds[rpc.arenaid];
    if (cur_tm_s > chpawd_tm + game_conf.arena_awd_expire_tm)
    {
        // 删除奖励
        //Utility.trace_err( "award["+rpc.arenaid+"] timeout\n");
        // 发送错误消息，通知客户端奖励已超时，无法领取了
        ::send_rpc(sid, 252, { res = game_err_code.ARENA_AWD_EXPIRE});

        delete ply.pinfo.chpawds[rpc.arenaid];
        ::send_rpc(sid, 235, ret_msg);    // send chpawd msg
        return;
    }

    // 有奖励
    ret_msg.chpawd = true;
    //sys.trace(sys.SLT_DETAIL, "have award\n");

    // 检查目标能否获得奖励，主要是背包空间是否足够
    local add_itm = chlvl.awd;
    if (!("gld" in add_itm))
    {
        add_itm.gld < -0;
    }
    if (!("bndyb" in add_itm))
    {
        add_itm.bndyb < -0;
    }
    if (!("itm" in add_itm))
    {
        add_itm.itm < - [];
    }
    if (!("eqp" in add_itm))
    {
        add_itm.eqp < - [];
    }

    // 检查目标道具是否可添加至角色
    local res = _check_add_item_to_pl(add_itm, ply);
    if (res != game_err_code.RES_OK)
    {
        // send itm_merge_res msg
        ::send_rpc(sid, 252, { res = res});
        return;
    }
    //sys.dumpobj(add_itm);

    // 奖励成就
    if ("achive" in chlvl)
    {
        local achive;
        foreach (achive in chlvl.achive)
        {
            local ach_data = { };
            if (!ply.has_achive(achive.id))
            {
                ply.pinfo.achives.push(achive.id);
                ach_data.achive < -achive.id;
            }

            // 记录成就时效
            if ("tm" in achive)
            {
                ach_data.achive < -achive.id;
                ach_data.tm < -chpawd_tm + achive.tm;
                ply.pinfo.achivetm[achive.id] < -ach_data.tm;
            }

            if (ach_data.Count > 0)
            {
                ::send_rpc(ply.pinfo.sid, 5, ach_data); // send gain achive msg
            }
        }
    }

    // 添加金钱、道具、装备至角色
    local new_itm_id_ary = _add_item_to_pl(add_itm, ply);
    //sys.dumpobj(new_itm_id_ary);

    ply.flush_db_data(false, false); // 同步数据至管理器

    if (add_itm.gld > 0)
    {
        svr.add_gold_log(gold_act_type.GLDAT_ARENA_AWD, add_itm.gld, 0); // log gold
    }
    if (add_itm.bndyb > 0)
    {
        _log_bndyb_log(ply, cur_tm_s, bndyb_act_type.BAT_ARENA_AWD, add_itm.bndyb, 0, rpc.arenaid, 0);    // log bndyb change
    }
    if (new_itm_id_ary.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_ARENA_AWARD, itm_flag_type.IFT_NEW_ADD, rpc.arenaid, new_itm_id_ary, []);  // log itm
    }

    // 奖励完成
    //Utility.trace_err( "delete award["+rpc.arenaid+"]\n");
    delete ply.pinfo.chpawds[rpc.arenaid];

    ::send_rpc(sid, 235, ret_msg);    // send chpawd msg
}

// 获取冠军展示信息
function _arena_chpshow(sid, rpc, ply, arena_conf)
{
    local glb_chlvl_crt_info = null;

    // 获取决赛信息
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local glb_chlvl_crt_data = _get_chlvl_crt_data();
    if (glb_chlvl_crt_data)
    {
        glb_chlvl_crt_info = glb_chlvl_crt_data.get_data();
    }
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR);  // 解锁

    local ret_msg = { arenaid = arena_conf.id, tp = 4, showinfos =[] };

    if (!glb_chlvl_crt_info)
    {
        // send arena msg
        ::send_rpc(sid, 235, ret_msg);
        return;
    }

    if (!("chpshows" in glb_chlvl_crt_info.data))
    {
        // send arena msg
        ::send_rpc(sid, 235, ret_msg);
        return;
    }

    if (!(arena_conf.id in glb_chlvl_crt_info.data.chpshows))
    {
        // send arena msg
        ::send_rpc(sid, 235, ret_msg);
        return;
    }

    ret_msg.showinfos = glb_chlvl_crt_info.data.chpshows[arena_conf.id];

    // send arena msg
    ::send_rpc(sid, 235, ret_msg);
}

// 获取历史决赛次数
function _arena_chphcnt(sid, rpc, ply, arena_conf)
{
    local glb_chlvl_crt_info = null;

    // 获取决赛信息
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local glb_chlvl_crt_data = _get_chlvl_crt_data();
    if (glb_chlvl_crt_data)
    {
        glb_chlvl_crt_info = glb_chlvl_crt_data.get_data();
    }
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR);  // 解锁

    local ret_msg = { arenaid = arena_conf.id, tp = 5, chphcnt = 0 };

    if (!glb_chlvl_crt_info)
    {
        // send arena msg
        ::send_rpc(sid, 235, ret_msg);
        return;
    }

    if (!("chprnds" in glb_chlvl_crt_info.data))
    {
        // send arena msg
        ::send_rpc(sid, 235, ret_msg);
        return;
    }

    if (!(arena_conf.id in glb_chlvl_crt_info.data.chprnds))
    {
        // send arena msg
        ::send_rpc(sid, 235, ret_msg);
        return;
    }

    ret_msg.chphcnt = glb_chlvl_crt_info.data.chprnds[arena_conf.id].Count;

    // send arena msg
    ::send_rpc(sid, 235, ret_msg);
}

// 获取历史决赛晋级信息
function _arena_chphinfo(sid, rpc, ply, arena_conf)
{
    local glb_chlvl_crt_info = null;

    // 获取决赛信息
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local glb_chlvl_crt_data = _get_chlvl_crt_data();
    if (glb_chlvl_crt_data)
    {
        glb_chlvl_crt_info = glb_chlvl_crt_data.get_data();
    }
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CHLVL_CRT_MGR);  // 解锁

    local ret_msg = { arenaid = arena_conf.id, tp = 6, chphidx = rpc.chphidx, chprnd =[], chptm = 0 };

    if (!glb_chlvl_crt_info)
    {
        // send arena msg
        ::send_rpc(sid, 235, ret_msg);
        return;
    }

    if (!("chprnds" in glb_chlvl_crt_info.data))
    {
        // send arena msg
        ::send_rpc(sid, 235, ret_msg);
        return;
    }

    if (!(arena_conf.id in glb_chlvl_crt_info.data.chprnds))
    {
        // send arena msg
        ::send_rpc(sid, 235, ret_msg);
        return;
    }

    local chphrnd_ary = glb_chlvl_crt_info.data.chprnds[arena_conf.id];

    if (rpc.chphidx >= chphrnd_ary.Count)
    {
        // send arena msg
        ::send_rpc(sid, 235, ret_msg);
        return;
    }

    ret_msg.chprnd = chphrnd_ary[rpc.chphidx].plys;
    ret_msg.chptm = chphrnd_ary[rpc.chphidx].tm;

    // send arena msg
    ::send_rpc(sid, 235, ret_msg);
}

// 竞技场相关消息
function arena_do(sid, rpc, ply)
{
    local arena_conf = get_arena_conf(rpc.arenaid);
    if (!arena_conf)
    {
        ::send_rpc(sid, 252, { res = game_err_code.PARAMETER_ERR});
        return;
    }

    if (rpc.tp == 3)
    {
        // 获取冠军展示信息
        _arena_chpshow(sid, rpc, ply, arena_conf);
        return;
    }

    // 根据arena_conf判断是否满足进入竞技场条件
    local res = _check_arena_requirement(ply, arena_conf);
    if (res != game_err_code.RES_OK)
    {
        // requirement error, send lvl_err_msg
        ::send_rpc(sid, 252, { res = res});
        return;
    }

    if (!game_data_conf.is_battle_svr)
    {
        if (("battle_lvl" in arena_conf) && arena_conf.battle_lvl == 1 )
        {
            bsclient_ply_data_arena_do(sid, rpc, ply, arena_conf);
            return;
        }
    }

    switch (rpc.tp)
    {
        case 1: // 获取竞技场相关信息
            {
                _arena_info(sid, rpc, ply, arena_conf);
            }
            break;
        case 2: // 领取冠军奖励
            {
                if (ply.pinfo.bsinfo.state == bsc_join_state.BSCJS_JOIN_LEVEL)
                {
                ::send_rpc(sid, 252, { res = game_err_code.BS_IN_BATTLE_CNT_DO});
                    return;
                }

                _arena_awd(sid, rpc, ply, arena_conf);
            }
            break;
        //case 3: // 获取冠军展示信息
        //    {
        //        _arena_chpshow(sid, rpc, ply, arena_conf);
        //    }
        //    break;
        case 4: // 获取历史决赛次数
            {
                _arena_chphcnt(sid, rpc, ply, arena_conf);
            }
            break;
        case 5: // 获取历史决赛晋级信息
            {
                _arena_chphinfo(sid, rpc, ply, arena_conf);
            }
            break;
    }
}

// 获取首席大弟子信息
function _carrchief_info(sid, rpc, ply)
{
    // 获取首席大弟子形象
    local glb_carrchief_info = null;

    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CARR_CHIEF, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local glb_carrchief_data = _get_carrchief_data();
    if (glb_carrchief_data)
    {
        glb_carrchief_info = glb_carrchief_data.get_data();
    }
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CARR_CHIEF);  // 解锁

    local ret_msg = { tp = 1 };
    if (glb_carrchief_info)
    {
        if (rpc.carr in glb_carrchief_info.data.showinfo)
        {
            local showinfo = glb_carrchief_info.data.showinfo[rpc.carr];
            if (showinfo.Count > 0)
            {
                ret_msg.showinfo < -showinfo;
            }
        }
    }
    //sys.dumpobj(glb_carrchief_info);

    // send carrchief msg
    ::send_rpc(sid, 234, ret_msg);
}

// 获取首席大弟子奖励
function _carrchief_awd(sid, rpc, ply)
{
    local ret_msg = { tp = 2, carrchieftm = 0 };

    // 检查配置
    local cur_tm_s = sys.time();
    local game_conf = get_general_game_conf();
    local carrchief_conf = get_carrchief_conf(ply.pinfo.carr);
    if (!game_conf || !carrchief_conf)
    {
        ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});      // send error msg
        return;
    }

    if (!ply.pinfo.carrchief)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_NO_CARRC_AWD});      // send error msg
        ::send_rpc(sid, 234, ret_msg);    // send carrchief msg
        return;
    }

    //sys.trace(sys.SLT_DETAIL, "cur_tm_s["+cur_tm_s+"] ply.pinfo.carrchieftm["+ply.pinfo.carrchieftm+"] game_conf.carrc_awd_expire_tm["+game_conf.carrc_awd_expire_tm+"]\n");

    if (cur_tm_s > ply.pinfo.carrchieftm + game_conf.carrc_awd_expire_tm)
    {
        ply.pinfo.carrchief = false;
        ply.pinfo.carrchieftm = 0;

        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_CARRC_AWD_EXPIRE});      // send error msg
        ::send_rpc(sid, 234, ret_msg);    // send carrchief msg
        return;
    }

    local add_itm = null;

    // 检查有无奖励
    if ("awd" in carrchief_conf)
    {
        // 检查目标能否获得奖励，主要是背包空间是否足够
        add_itm = carrchief_conf.awd;
        if (!("gld" in add_itm))
        {
            add_itm.gld < -0;
        }
        if (!("bndyb" in add_itm))
        {
            add_itm.bndyb < -0;
        }
        if (!("itm" in add_itm))
        {
            add_itm.itm < - [];
        }
        if (!("eqp" in add_itm))
        {
            add_itm.eqp < - [];
        }

        // 检查目标道具是否可添加至角色
        local res = _check_add_item_to_pl(add_itm, ply);
        if (res != game_err_code.RES_OK)
        {
            // send itm_merge_res msg
            ::send_rpc(sid, 252, { res = res});
            return;
        }
    }

    local carrchieftm = ply.pinfo.carrchieftm;

    // 清除奖励记录
    ply.pinfo.carrchief = false;
    ply.pinfo.carrchieftm = 0;

    // 奖励成就
    if ("achive" in carrchief_conf)
    {
        local achive;
        foreach (achive in carrchief_conf.achive)
        {
            local ach_data = { };
            if (!ply.has_achive(achive.id))
            {
                ply.pinfo.achives.push(achive.id);
                ach_data.achive < -achive.id;
            }

            // 记录成就时效
            if ("tm" in achive)
            {
                ach_data.achive < -achive.id;
                ach_data.tm < -carrchieftm + achive.tm;
                ply.pinfo.achivetm[achive.id] < -ach_data.tm;
            }

            if (ach_data.Count > 0)
            {
                ::send_rpc(ply.pinfo.sid, 5, ach_data); // send gain achive msg
            }
        }
    }

    if (add_itm)
    {
        // 添加金钱、道具、装备至角色
        local new_itm_id_ary = _add_item_to_pl(add_itm, ply);

        ply.flush_db_data(false, false); // 同步数据至管理器

        if (add_itm.gld > 0)
        {
            svr.add_gold_log(gold_act_type.GLDAT_CARRCHIEF_AWD, add_itm.gld, 0); // log gold
        }
        if (add_itm.bndyb > 0)
        {
            _log_bndyb_log(ply, cur_tm_s, bndyb_act_type.BAT_CARRCHIEF_AWD, add_itm.bndyb, 0, ply.pinfo.carr, 0);    // log bndyb change
        }
        if (new_itm_id_ary.Count > 0)
        {
            _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_CARRCHIEF_AWARD, itm_flag_type.IFT_NEW_ADD, ply.pinfo.carr, new_itm_id_ary, []);  // log itm
        }
    }

    // 奖励完成

    ::send_rpc(sid, 234, ret_msg);    // send chpawd msg
}

// 首席大弟子相关消息
function carrchief_do(sid, rpc, ply)
{
    switch (rpc.tp)
    {
        case 1: // 获取竞技场相关信息
            {
                _carrchief_info(sid, rpc, ply);
            }
            break;
        case 2: // 领取冠军奖励
            {
                _carrchief_awd(sid, rpc, ply);
            }
            break;
    }
}

// 获取帮派领地所有者信息
function _clter_info(sid, rpc, ply)
{
    local clanter_info = _get_clanter_info(rpc.clteid);

    local ret_msg = { tp = 1, clteid = rpc.clteid, clanid = 0 };
    if (clanter_info)
    {
        ret_msg.clanid = clanter_info.clanid;
        local showinfo = [];
        local selfcid = ply.pinfo.cid;
        if (clanter_info.showinfo.Count > 0)
        {
            foreach (val in clanter_info.showinfo)
            {
                if (val.cid == selfcid)
                {
                    if (val.awdtm > 0)
                    {
                        ret_msg.awdtm < -val.awdtm;
                    }
                    showinfo.push(val);
                }
                else
                {
                    local clan_pl = clanmgr.get_clanpl(val.cid);
                    if (clan_pl && clan_pl.clanc == clan_c_tp.CCT_P)
                    {
                        showinfo.push(val);
                    }
                }
            }
        }
        ret_msg.showinfo < -showinfo;

        if (("stastic_cost" in clanter_info) && ("cost_awd" in clanter_info.stastic_cost) )
        {
            ret_msg.cost_awd < -clanter_info.stastic_cost.cost_awd;
        }

        if (("clanid"in ply.pinfo) && (ply.pinfo.clanid == clanter_info.clanid))
        {
            if ("dalyawd_tm" in clanter_info )
            {
                ret_msg.dalyawd_tm < -clanter_info.dalyawd_tm;
            }

            if ("costawd_tm" in clanter_info )
            {
                ret_msg.costawd_tm < -clanter_info.costawd_tm;
            }
        }
    }

    // send clanter_res msg
    ::send_rpc(sid, 232, ret_msg);
}

// 获取帮派领地奖励
function _clter_awd(sid, rpc, ply)
{
    local ret_msg = { tp = 2, clteid = rpc.clteid, awdtm = 0 };

    // 检查配置
    local cur_tm_s = sys.time();
    local game_conf = get_general_game_conf();
    local clantrit_conf = get_clantrit_conf(rpc.clteid);
    if (!game_conf || !clantrit_conf || !("warlvl" in clantrit_conf) || !("win_awd" in clantrit_conf.warlvl))
    {
        ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});      // send error msg
        return;
    }

    local clanter_info = null;
    local glb_clanter_info = null;
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CLAN_TER, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local gld_clanter_data = _get_clanter_data();
    if (gld_clanter_data)
    {
        glb_clanter_info = gld_clanter_data.get_data();
        if (rpc.clteid in glb_clanter_info.data)
        {
            clanter_info = glb_clanter_info.data[rpc.clteid];
        }
    }

    if (!clanter_info)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLAN_TER_NOT_OWN_AWD});   // send error msg
        ::send_rpc(sid, 232, ret_msg);    // send clanter_res msg
        return;
    }

    local own_awd_conf = null;
    local awdtm = 0;
    local leader_has_get = false;
    for (local i = 0; i < clanter_info.showinfo.Count; ++i)
    {
        local clanply = clanter_info.showinfo[i];
        if (clanply.cid != ply.pinfo.cid)
        {
            continue;
        }

        if (clanply.awdtm <= 0)
        {
            glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
            ::send_rpc(sid, 252, { res = game_err_code.CLAN_TER_NOT_OWN_AWD});   // send error msg
            ::send_rpc(sid, 232, ret_msg);    // send clanter_res msg
    return;
}

        if(clanply.awdtm + game_conf.clanter_awd_expire_tm<cur_tm_s)
        {
            // 过期，清除奖励
            clanply.awdtm = 0;

            // 同步至数据库
            gld_clanter_data.mod_data(glb_clanter_info);
            gld_clanter_data.db_update();
            
            glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
            ::send_rpc(sid, 252, { res = game_err_code.CLAN_TER_AWD_EXPIRE});   // send error msg
            ::send_rpc(sid, 232, ret_msg);    // send clanter_res msg
            return;
        }
    
        local own_awd = 2;
        if(!leader_has_get)
        {
            local clan_pcid = _get_clan_pcid(clanter_info.clanid);
            if(clanply.cid == clan_pcid)
            {   //帮主领一档奖励，其他人领一档奖励
                own_awd = 1;
                leader_has_get = true;
            }
        }
        if(!(own_awd in clantrit_conf.warlvl.win_awd))
        {
            // 奖励配置不存在，清除奖励
            clanply.awdtm = 0;

            // 同步至数据库
            gld_clanter_data.mod_data(glb_clanter_info);
            gld_clanter_data.db_update();
            
            glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
            ::send_rpc(sid, 252, { res = game_err_code.CLAN_TER_NOT_OWN_AWD});   // send error msg
            ::send_rpc(sid, 232, ret_msg);    // send clanter_res msg
            return;
        }

        own_awd_conf = clantrit_conf.warlvl.win_awd[own_awd];

        awdtm = clanply.awdtm;
        // 清除奖励记录
        clanply.awdtm = 0;

        break;
    }
    
    if(!own_awd_conf)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLAN_TER_NOT_OWN_AWD});   // send error msg
        ::send_rpc(sid, 232, ret_msg);    // send clanter_res msg
        return;
    }

    local add_itm = null;

    // 检查有无奖励
    if ("awd" in own_awd_conf)
    {
        // 检查目标能否获得奖励，主要是背包空间是否足够
        add_itm = own_awd_conf.awd;
        if (!("gld" in add_itm))
        {
            add_itm.gld<- 0;
        }
        if (!("bndyb" in add_itm))
        {
            add_itm.bndyb<- 0;
        }
        if(!("itm" in add_itm))
        {
            add_itm.itm<- [];
        }
        if(!("eqp" in add_itm))
        {
            add_itm.eqp<- [];
        }
        
        // 检查目标道具是否可添加至角色
        local res = _check_add_item_to_pl(add_itm, ply);
        if(res != game_err_code.RES_OK)
        {
            // send itm_merge_res msg
            ::send_rpc(sid, 252, { res = res});
            glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
            return;
        }
    }

    // 同步至数据库
    gld_clanter_data.mod_data(glb_clanter_info);
    gld_clanter_data.db_update();

    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁

    // 奖励成就
    if ("achive" in own_awd_conf)
    {
        local achive;
        foreach(achive in own_awd_conf.achive)
        {           
            local ach_data = { };
            if(!ply.has_achive(achive.id))
            {
                ply.pinfo.achives.push(achive.id);
                ach_data.achive<- achive.id;                
            }

            // 记录成就时效
            if ("tm" in achive)
            {
                ach_data.achive<- achive.id;   
                ach_data.tm<- awdtm + achive.tm;
                ply.pinfo.achivetm[achive.id] <- ach_data.tm;
            }

            if ( ach_data.Count > 0 )
            {
                ::send_rpc(ply.pinfo.sid, 5, ach_data); // send gain achive msg
            }
        }
    }

    if (add_itm)
    {
        // 添加金钱、道具、装备至角色
        local new_itm_id_ary = _add_item_to_pl(add_itm, ply);

ply.flush_db_data(false, false); // 同步数据至管理器

        if (add_itm.gld > 0)
        {
            svr.add_gold_log(gold_act_type.GLDAT_CLANTER_AWD, add_itm.gld, 0); // log gold
        }
        if (add_itm.bndyb > 0)
        {
            _log_bndyb_log(ply, cur_tm_s, bndyb_act_type.BAT_CLANTER_AWD, add_itm.bndyb, 0, rpc.clteid, 0);    // log bndyb change
        }
        if(new_itm_id_ary.Count > 0)
        {
            _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_CLANTER_AWD, itm_flag_type.IFT_NEW_ADD, rpc.clteid, new_itm_id_ary, []);  // log itm
        }
    }

    ::send_rpc(sid, 232, ret_msg);    // send clanter_res msg
}

// 获取帮派领地每日帮派资源
function _clter_daly_awd(sid, rpc, ply)
{
    local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
    if (!clan_pl)
    {
        // 不是帮派成员
        ::send_rpc(sid, 252, { res = game_err_code.NOT_IN_CLAN});
        return;
    }

    if (clan_pl.clanc < clan_c_tp.CCT_P)
    {
        // 帮主才能领取
        ::send_rpc(sid, 252, { res = game_err_code.CLAN_NO_AUTHORITY});
        return;
    }

    // 检查配置
    local cur_tm_s = sys.time();
    local game_conf = get_general_game_conf();
    local clantrit_conf = get_clantrit_conf(rpc.clteid);
    if (!game_conf || !clantrit_conf || !("daly_awd" in clantrit_conf) )
    {
        ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});      // send error msg
        return;
    }

    local clanter_info = null;
    local glb_clanter_info = null;
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CLAN_TER, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local gld_clanter_data = _get_clanter_data();
    if (gld_clanter_data)
    {
        glb_clanter_info = gld_clanter_data.get_data();
        if (rpc.clteid in glb_clanter_info.data)
        {
            clanter_info = glb_clanter_info.data[rpc.clteid];
        }
    }

    if (!clanter_info)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLAN_TER_NOT_OWN_DALYAWD});   // send error msg
        return;
    }

    if (clanter_info.clanid != clan_pl.clid)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_NOT_YOUR_CLAN_TER});   // send error msg
        return;
    }

    local cur_tm_s = sys.time();

    if ("dalyawd_tm" in clanter_info)
    {
        // 获取今天开始时间点
        local cur_local_tm = sys.trans_local_time(cur_tm_s);
        local today_start_tm_s = cur_tm_s - (cur_local_tm.h * 3600 + cur_local_tm.min * 60 + cur_local_tm.s);

        if (clanter_info.dalyawd_tm > today_start_tm_s)
        {
            // 今天已经获取过了
            glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
            ::send_rpc(sid, 252, { res = game_err_code.CLAN_TER_DALYAWD_FETCHED});   // send error msg
            return;
        }
    }
    clanter_info.dalyawd_tm < -cur_tm_s;

    // 同步至数据库
    gld_clanter_data.mod_data(glb_clanter_info);
    gld_clanter_data.db_update();

    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁

    // 通知已领取奖励
    ::send_rpc(sid, 232, { tp = 3, clteid = rpc.clteid, dalyawd_tm = cur_tm_s});    // send clanter_res msg

    local daly_awd_conf = clantrit_conf.daly_awd[0];
    local gld_add = 0;
    local yb_add = 0;
    if ("gld" in daly_awd_conf)
    {
        gld_add = daly_awd_conf.gld;
    }
    if ("yb" in daly_awd_conf)
    {
        yb_add = daly_awd_conf.yb;
    }

    // 给帮派资源
    local clan_mgr = global_data_mgrs.clan;
    clan_mgr.lock_data(clan_pl.clid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

    local clan_db_data = _get_clan_data(clan_pl.clid);
    if (clan_db_data.Count <= 0)
    {
        // 目标帮派不存在
        ::send_rpc(sid, 252, { res = game_err_code.CLAN_NOT_EXIST});
        clan_mgr.unlock_data(clan_pl.clid); // 解锁
        return;
    }

    local clan_db_info = clan_db_data[0].get_data();

    local rpc_msg_data = { };

    if (gld_add > 0)
    {
        if (chech_numeric_overflow(clan_db_info.gold, gld_add))
        {   //数字溢出，超出部分不处理
            clan_db_info.gold = 0x7fffffff;
        }
        else
        {
            clan_db_info.gold += gld_add;
        }
        rpc_msg_data.gold < -gld_add;
    }
    if (yb_add > 0)
    {
        clan_db_info.yb += yb_add;
        rpc_msg_data.yb < -yb_add;
    }

    // 更新帮派数据
    clan_db_data[0].mod_data(clan_db_info);
    clan_db_data[0].db_update();

    clan_mgr.unlock_data(clan_pl.clid); // 解锁

    if (rpc_msg_data.Count > 0)
    {
        // 广播帮派属性变化消息
        // broad cast clan_info_change msg
        _broad_cast_clan_msg(clan_pl.clid, 216, rpc_msg_data, clan_c_tp.CCT_NONE);
    }
}

// 获取帮派领地每日帮派资源
function _clter_stastic_cost_awd(sid, rpc, ply)
{
    local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
    if (!clan_pl)
    {
        // 不是帮派成员
        ::send_rpc(sid, 252, { res = game_err_code.NOT_IN_CLAN});
        return;
    }

    if (clan_pl.clanc < clan_c_tp.CCT_P)
    {
        // 帮主才能领取
        ::send_rpc(sid, 252, { res = game_err_code.CLAN_NO_AUTHORITY});
        return;
    }

    local clantrit_conf = get_clantrit_conf(rpc.clteid);
    if (!clantrit_conf || !("stastic_cost" in clantrit_conf) || !("cost_awd" in clantrit_conf) )
    {
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_NO_COST_AWD});      // send error msg
        return;
    }

    local ret_msg = { };

    local cur_tm_s = sys.time();

    local clanter_info = null;
    local glb_clanter_info = null;
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CLAN_TER, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local gld_clanter_data = _get_clanter_data();
    if (gld_clanter_data)
    {
        glb_clanter_info = gld_clanter_data.get_data();
        if (rpc.clteid in glb_clanter_info.data)
        {
            clanter_info = glb_clanter_info.data[rpc.clteid];
        }
    }

    if (!clanter_info)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_NO_COST_AWD});      // send error msg
        return;
    }

    if (clanter_info.clanid != clan_pl.clid)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_NOT_YOUR_CLAN_TER});   // send error msg
        return;
    }

    if (clanter_info.onwar)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_COST_AWD_EXPIRE});      // send error msg
        return;
    }

    if ("costawd_tm" in clanter_info )
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_COST_AWD_FETCHED});   // send error msg
        return;
    }

    if (!("stastic_cost" in clanter_info) || !("cost_awd" in clanter_info.stastic_cost) )
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_NO_COST_AWD});      // send error msg
        return;
    }

    clanter_info.costawd_tm < -cur_tm_s;

    // 同步至数据库
    gld_clanter_data.mod_data(glb_clanter_info);
    gld_clanter_data.db_update();

    local cost_awd_data = clanter_info.stastic_cost.cost_awd;
    if (cost_awd_data.gld > 0)
    {
        local gold = ply.add_gold(cost_awd_data.gld);
        if (gold > 0)
        {
            ret_msg.gold < -gold;
        }
    }

    if (cost_awd_data.yb > 0)
    {
        ret_msg.yb < -cost_awd_data.yb;
        ply.add_yb(cost_awd_data.yb);
    }

    if (ret_msg.Count > 0)
    {
        ply.flush_db_data(true, false); // 即时刷新角色数据，更新角色资金数量 
    }

    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁

    // 通知已领取奖励
    ::send_rpc(sid, 232, { tp = 4, clteid = rpc.clteid, costawd_tm = cur_tm_s});    // send clanter_res msg

    if (ret_msg.Count > 0)
    {
        ::send_rpc(sid, 75, ret_msg);

        local broad_msg = { cid = ply.pinfo.cid, name = ply.pinfo.name, clanid = clan_pl.clid, tp = bcast_msg_tp.CLTER_COST_AWD, par = 0, par1 = 0 };

        if ("gold" in ret_msg )
        {
            svr.add_gold_log(gold_act_type.GLDAT_GET_CLTER_COST_AWD, ret_msg.gold, 0);

            broad_msg.par = ret_msg.gold;
        }

        if ("yb" in ret_msg )
        {
            // log yb change
            _log_game_log(game_log_type.GLT_yb_log, { cid = ply.pinfo.cid, uid = ply.pinfo.uid, tm = sys.time(), act = yb_act_type.YAT_CLTER_COST_AWD, added = ret_msg.yb, parid = rpc.clteid});

            broad_msg.par1 = ret_msg.yb;
        }

        //广播   
        _broad_cast_sys_msg(broad_msg);
    }
}

function _get_clter_mon_hp_per(sid, rpc, ply )
{
    local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
    if (!clan_pl)
    {
        // 不是帮派成员
        ::send_rpc(sid, 252, { res = game_err_code.NOT_IN_CLAN});
        return;
    }

    local clanter_info = _get_clanter_info(rpc.clteid);
    if (clanter_info && ("mon_hp_per" in clanter_info) )
    {
         ::send_rpc(sid, 232, { tp = rpc.tp, clteid = rpc.clteid, mon_hp_pers = clanter_info.mon_hp_per} );
    }
}

function _repair_clter_mon_hp_per(sid, rpc, ply )
{
    local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
    if (!clan_pl)
    {
        // 不是帮派成员
        ::send_rpc(sid, 252, { res = game_err_code.NOT_IN_CLAN});
        return;
    }

    if (clan_pl.clanc < clan_c_tp.CCT_VP)
    {
        ::send_rpc(sid, 252, { res = game_err_code.CLAN_NO_AUTHORITY});
        return;
    }

    local clantrit_conf = get_clantrit_conf(rpc.clteid);
    if (!clantrit_conf || !("mon_hp_per" in clantrit_conf) )
    {
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_NOT_REPAIR_MON});      // send error msg
        return;
    }

    local mon_hp_per_conf = clantrit_conf.mon_hp_per[0];
    if (!("repair_itm" in mon_hp_per_conf) )
    {
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_NOT_REPAIR_MON});      // send error msg
        return;
    }

    if (!ply.have_cost_item(mon_hp_per_conf.repair_itm, 1))
    {
        ::send_rpc(sid, 252, { res = game_err_code.PACKAGE_ITEM_REQUIRE});
        return;
    }

    local mons_conf = null;
    foreach (mons in mon_hp_per_conf.mons)
    {
        if (mons.mapid == rpc.mapid)
        {
            mons_conf = mons;
            break;
        }
    }
    if (!mons_conf)
    {
        ::send_rpc(sid, 252, { res = game_err_code.PARAMETER_ERR});      // send error msg
        return;
    }

    local find_mon = false;
    foreach (monid in mons_conf.ids)
    {
        if (monid == rpc.mid)
        {
            find_mon = true;
            break;
        }
    }
    if (!find_mon)
    {
        ::send_rpc(sid, 252, { res = game_err_code.PARAMETER_ERR});      // send error msg
        return;
    }

    local clanter_info = null;
    local glb_clanter_info = null;
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CLAN_TER, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local gld_clanter_data = _get_clanter_data();
    if (gld_clanter_data)
    {
        glb_clanter_info = gld_clanter_data.get_data();
        if (rpc.clteid in glb_clanter_info.data)
        {
            clanter_info = glb_clanter_info.data[rpc.clteid];
        }
    }

    if (!clanter_info || !("mon_hp_per" in clanter_info) )
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_NOT_REPAIR_MON});      // send error msg
        return;
    }

    if (clanter_info.onwar)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_NOT_REPAIR_MON});      // send error msg
        return;
    }

    if (clanter_info.clanid != clan_pl.clid)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_NOT_YOUR_CLAN_TER});   // send error msg
        return;
    }

    local mon_hp_pers_data = clanter_info.mon_hp_per;
    local map_mon_hp_pers = null;
    foreach (hp_pers_data in mon_hp_pers_data)
    {
        if (hp_pers_data.mapid == rpc.mapid)
        {
            map_mon_hp_pers = hp_pers_data;
            break;
        }
    }
    if (!map_mon_hp_pers)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.PARAMETER_ERR});      // send error msg
        return;
    }

    local mon_hp_per = null;
    foreach (hp_per_data in map_mon_hp_pers.hp_pers)
    {
        if (hp_per_data.mid == rpc.mid)
        {
            mon_hp_per = hp_per_data;
            break;
        }
    }
    if (!mon_hp_per)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.PARAMETER_ERR});      // send error msg
        return;
    }

    if (mon_hp_per.hp_per >= 100)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        rpc.res < -game_err_code.CLANTER_NEED_NOT_REPAIR;
        ::send_rpc(sid, 232, rpc);      // send error msg
        return;
    }

    //消耗道具
    local costed = ply.cost_item(mon_hp_per_conf.repair_itm, 1);
    if (costed.cnt <= 0)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.COST_ITEM_ERROR});   // send error msg
        return;
    }

    mon_hp_per.hp_per = 100;

    // 同步至数据库
    gld_clanter_data.mod_data(glb_clanter_info);
    gld_clanter_data.db_update();
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁

    ply.flush_db_data(false, false); // 同步数据至管理器

    local costed_rmvitms = [];
    local costed_rmvitms_cnts = [];
    local modcnt_itm_ary = [];
    local modcnt_itm_ary_cnts = [];

    foreach (val in costed.rmvitms)
    {
        costed_rmvitms.push(val.itm);
        costed_rmvitms_cnts.push(val);
    }
    foreach (val in costed.modcnts)
    {
        modcnt_itm_ary.push(val.itm);
        modcnt_itm_ary_cnts.push(val);
    }

    if (costed.rmvids.Count <= 0)
    {
        delete costed.rmvids;
    }
    if (costed.modcnts.Count <= 0)
    {
        delete costed.modcnts;
    }

    // send item_change msg
    ::send_rpc(sid, 75, costed);

    rpc.res < -game_err_code.RES_OK;
    _broad_cast_clan_msg(clan_pl.clid, 232, rpc, clan_c_tp.CCT_NONE);      // send error msg

    local cur_tm_s = sys.time();
    // log itm
    if (costed_rmvitms.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_REPAIR_CITY_MON, itm_flag_type.IFT_RMV, mon_hp_per_conf.repair_itm, costed_rmvitms, costed_rmvitms_cnts);
    }
    if (modcnt_itm_ary.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_REPAIR_CITY_MON, itm_flag_type.IFT_MODCNT, mon_hp_per_conf.repair_itm, modcnt_itm_ary, modcnt_itm_ary_cnts);
    }
}

function _get_clter_request_info(sid, rpc, ply )
{
    //local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
    //if(!clan_pl)
    //{
    //    // 不是帮派成员
    //    ::send_rpc(sid, 252, {res=game_err_code.NOT_IN_CLAN});
    //    return;
    //}

    local clanter_info = _get_clanter_info(rpc.clteid);
    if (clanter_info && ("war_reqs" in clanter_info) )
    {
         ::send_rpc(sid, 232, { tp = rpc.tp, clteid = rpc.clteid, war_reqs = clanter_info.war_reqs} );
    }
    else
    {
        ::send_rpc(sid, 232, { tp = rpc.tp, clteid = rpc.clteid, war_reqs =[]} );
    }
}

function _request_clter(sid, rpc, ply )
{
    local clan_pl = clanmgr.get_clanpl(ply.pinfo.cid);
    if (!clan_pl)
    {
        // 不是帮派成员
        ::send_rpc(sid, 252, { res = game_err_code.NOT_IN_CLAN});
        return;
    }

    if (clan_pl.clanc < clan_c_tp.CCT_VP)
    {
        ::send_rpc(sid, 252, { res = game_err_code.CLAN_NO_AUTHORITY});
        return;
    }

    local clantrit_conf = get_clantrit_conf(rpc.clteid);
    if (!clantrit_conf || !("war_req" in clantrit_conf) )
    {
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_NEED_NOT_REQUEST});      // send error msg
        return;
    }

    local war_req_conf = clantrit_conf.war_req;
    if ("tmchk" in war_req_conf )
    {
        local res = _check_lvl_tmchk(war_req_conf.tmchk);
        if (res != game_err_code.RES_OK)
        {
            ::send_rpc(sid, 252, { res = res});
            return;
        }
    }

    local cost_gld = 0;
    if ("cost_gld" in war_req_conf )
    {
        cost_gld = war_req_conf.cost_gld;
        if (ply.pinfo.gold < cost_gld)
        {
            ::send_rpc(sid, 252, { res = game_err_code.NOT_ENOUGH_GOLD});
            return;
        }
    }

    local cost_itm_id = 0;
    if ("cost_itm" in war_req_conf )
    {
        cost_itm_id = war_req_conf.cost_itm;
        if (!ply.have_cost_item(cost_itm_id, 1))
        {
            ::send_rpc(sid, 252, { res = game_err_code.PACKAGE_ITEM_REQUIRE});
            return;
        }
    }

    local clanter_info = null;
    local glb_clanter_info = null;
    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CLAN_TER, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local gld_clanter_data = _get_clanter_data();
    if (gld_clanter_data)
    {
        glb_clanter_info = gld_clanter_data.get_data();
        if (rpc.clteid in glb_clanter_info.data)
        {
            clanter_info = glb_clanter_info.data[rpc.clteid];
        }
    }

    if (!clanter_info)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_NEED_NOT_REQUEST});      // send error msg
        return;
    }

    if (clanter_info.clanid == 0 || clanter_info.clanid == clan_pl.clid)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_CLAN_NEED_NOT_REQUEST});   // send error msg
        return;
    }

    if (clanter_info.onwar)
    {
        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
        ::send_rpc(sid, 252, { res = game_err_code.CLANTER_REQUEST_EXPIRE});      // send error msg
        return;
    }

    if ("war_reqs" in clanter_info )
    {
        foreach (clanid in clanter_info.war_reqs)
        {
            if (clanid == clan_pl.clid)
            {
                glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
                ::send_rpc(sid, 252, { res = game_err_code.CLANTER_CLAN_ALREADY_REQUEST});      // send error msg
                return;
            }
        }
    }

    //消耗道具
    local costed = null;
    if (cost_itm_id > 0)
    {
        costed = ply.cost_item(cost_itm_id, 1);
        if (costed.cnt <= 0)
        {
            glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁
            ::send_rpc(sid, 252, { res = game_err_code.COST_ITEM_ERROR});   // send error msg
            return;
        }
    }

    if (cost_gld > 0)
    {
        ply.sub_gold(cost_gld);
    }

    if ("war_reqs" in clanter_info )
    {
        clanter_info.war_reqs.push(clan_pl.clid);
    }
    else
    {
        clanter_info.war_reqs < - [clan_pl.clid];
    }

    // 同步至数据库
    gld_clanter_data.mod_data(glb_clanter_info);
    gld_clanter_data.db_update();
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁

    ply.flush_db_data(false, false); // 同步数据至管理器

    local costed_rmvitms = [];
    local costed_rmvitms_cnts = [];
    local modcnt_itm_ary = [];
    local modcnt_itm_ary_cnts = [];

    if (costed)
    {
        foreach (val in costed.rmvitms)
        {
            costed_rmvitms.push(val.itm);
            costed_rmvitms_cnts.push(val);
        }
        foreach (val in costed.modcnts)
        {
            modcnt_itm_ary.push(val.itm);
            modcnt_itm_ary_cnts.push(val);
        }

        if (costed.rmvids.Count <= 0)
        {
            delete costed.rmvids;
        }
        if (costed.modcnts.Count <= 0)
        {
            delete costed.modcnts;
        }
    }
    else
    {
        costed = { };
    }


    if (cost_gld > 0)
    {
        costed.gold < - -cost_gld;

        svr.add_gold_log(gold_act_type.GLDAT_CLTER_REQUEST_COST, 0, cost_gld);
    }

    if (costed.Count > 0)
    {
         // send item_change msg
        ::send_rpc(sid, 75, costed);
    }


    ::send_rpc(sid, 232, { tp = rpc.tp, clteid = rpc.clteid, clanid = clan_pl.clid} );

    //广播   
    _broad_cast_sys_msg( { cid = ply.pinfo.cid, name = ply.pinfo.name, clanid = clan_pl.clid, tp = bcast_msg_tp.CLTER_CLAN_REQUEST } );

    local cur_tm_s = sys.time();
    // log itm
    if (costed_rmvitms.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_REQUEST_CITY_WAR, itm_flag_type.IFT_RMV, cost_itm_id, costed_rmvitms, costed_rmvitms_cnts);
    }
    if (modcnt_itm_ary.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_REQUEST_CITY_WAR, itm_flag_type.IFT_MODCNT, cost_itm_id, modcnt_itm_ary, modcnt_itm_ary_cnts);
    }
}

// 帮派领地相关消息
function clter_do(sid, rpc, ply)
{
    switch (rpc.tp)
    {
        case 1: // 获取帮派领地所有者信息
            {
                _clter_info(sid, rpc, ply);
            }
            break;
        case 2: // 获取帮派领地奖励
            {
                _clter_awd(sid, rpc, ply);
            }
            break;
        case 3: // 获取帮派领地每日帮派资源
            {
                _clter_daly_awd(sid, rpc, ply);
            }
            break;
        case 4: // 获取攻城战消耗奖励
            {
                _clter_stastic_cost_awd(sid, rpc, ply);
            }
            break;
        case 5: // 获取攻城战消耗奖励
            {
                _get_clter_mon_hp_per(sid, rpc, ply);
            }
            break;
        case 6:
            {
                _repair_clter_mon_hp_per(sid, rpc, ply);
            }
            break;
        case 7: // 获取攻城战报名信息
            {
                _get_clter_request_info(sid, rpc, ply);
            }
            break;
        case 8: // 报名攻城战
            {
                _request_clter(sid, rpc, ply);
            }
            break;
    }
}

// 元宝立即通关副本
function _ybfin_lvl(sid, rpc, ply)
{
    local lvl_conf = get_level_conf(rpc.ltpid);
    if (!lvl_conf)
    {
        ::send_rpc(sid, 252, { res = game_err_code.PARAMETER_ERR});
        return;
    }

    local yb_fin_conf = null;
    local diff_lvl = 1;

    if ("yb_fin" in lvl_conf)
    {
        yb_fin_conf = lvl_conf.yb_fin[0];
    }

    if (("diff_lvl" in lvl_conf) && (rpc.diff_lvl in lvl_conf.diff_lvl))
    {
        local diff_lvl_conf = lvl_conf.diff_lvl[rpc.diff_lvl];

        if ("yb_fin" in diff_lvl_conf)
        {
            yb_fin_conf = diff_lvl_conf.yb_fin[0];
        }

        diff_lvl = rpc.diff_lvl;

        if ("attchk" in diff_lvl_conf)
        {
            foreach (attchk in diff_lvl_conf.attchk)
            {
                local res = ply.check_att(attchk);
                if (res != game_err_code.RES_OK)
                {
                    ::send_rpc(sid, 252, { res = res});
                    return;
                }
            }
        }
    }

    if (!yb_fin_conf)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_CANT_YB_FIN});
        return;
    }

    if (yb_fin_conf.combpt > ply.pinfo.combpt)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_YBFIN_COMBPT_REQUIRE});
        return;
    }

    local lvlentried = ply.get_entried_lvl(rpc.ltpid);
    if (!lvlentried)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_YB_FIN_NOT_COMPLETE});
        return;
    }

    if (!("fin_diff" in lvlentried))
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_YB_FIN_NOT_COMPLETE});
        return;
    }

    if (lvlentried.fin_diff < diff_lvl)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_YB_FIN_NOT_COMPLETE});
        return;
    }

    local res = _check_lvl_requirement(ply, lvl_conf);
    if (res != game_err_code.RES_OK)
    {
        // requirement error, send err msg
        ::send_rpc(sid, 252, { res = res});
        return;
    }

    local cur_tm_s = sys.time();

    if ("enter_cd" in lvl_conf)
    {
        if ((lvl_conf.enter_cdtp in ply.pinfo.lvlentercd) && ply.pinfo.lvlentercd[lvl_conf.enter_cdtp] > cur_tm_s)
        {
            // 进入副本的冷却时间未到，不能进入, send lvl_err_msg
            ::send_rpc(sid, 252, { res = game_err_code.LEVEL_ENTER_CD_NOT_REACH});
            return;
        }
    }

    local vip_lvlcnt = null;
    if (lvlentried && ("cntleft" in lvlentried) && lvlentried.cntleft <= 0)
    {
        // 进入副本次数已达到上限

        //::send_rpc(sid, 239, {res=game_err_code.LEVEL_ENTRY_CNT_REACHED});
        //return;

        // 检查是否可以vip进入
        if (ply.pinfo.vip <= 0)
        {
            ::send_rpc(sid, 252, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }

        if (!("vip_data" in ply.pinfo))
        {
            ::send_rpc(sid, 252, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }

        if (!("lvlcnt" in ply.pinfo.vip_data))
        {
            ::send_rpc(sid, 252, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }

        foreach (lvlcnt in ply.pinfo.vip_data.lvlcnt)
        {
            if (lvlcnt.ltpid == rpc.ltpid && lvlcnt.cnt > 0)
            {
                vip_lvlcnt = lvlcnt;
            }
        }

        if (!vip_lvlcnt)
        {
            ::send_rpc(sid, 252, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }
    }

    local yb_cost = 0;
    if (lvl_conf.yb > 0)
    {
        yb_cost += lvl_conf.yb;
    }

    if (vip_lvlcnt)
    {
        // 消耗vip进入次数，需消耗元宝

        if (!(vip_lvlcnt.ltpid in ply.pinfo.vip_conf.lvlcnt))
        {
            ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});
            return;
        }
        local vip_lvl_conf = ply.pinfo.vip_conf.lvlcnt[vip_lvlcnt.ltpid];
        yb_cost += vip_lvl_conf.yb;
    }

    // 计算立即完成元宝消耗
    yb_cost += yb_fin_conf.yb;

    if (ply.pinfo.yb < yb_cost)
    {
        ::send_rpc(sid, 252, { res = game_err_code.NOT_ENOUGH_YB});
        return;
    }

    // 模拟计算杀怪，获得金币、经验、道具等

    // 遍历需生成道具怪物，组织掉落表
    local drop_ids = { };
    local kms = { };
    foreach (mon in yb_fin_conf.m)
    {
        // 获取怪物掉落
        local mon_conf = get_monster_conf(mon.mid);
        if (!mon_conf)
        {
            continue;
        }

        if (!(mon.mid in kms))
        {
            kms[mon.mid] < -mon.cnt;
        }
        else
        {
            kms[mon.mid] += mon.cnt;
        }

        if ("drop" in mon_conf)
        {
            if (!(mon_conf.drop in drop_ids))
            {
                drop_ids[mon_conf.drop] < -mon.cnt;
            }
            else
            {
                drop_ids[mon_conf.drop] += mon.cnt;
            }
        }

        // 添加时效掉落
        local cur_local_tm = sys.get_local_time();
        foreach (val in game_data_conf.itemdrops.tm_exdrop)
        {
            if (!(mon.mid in val.mon))
            {
                continue;
            }

            if (_check_tm(cur_local_tm, val.tmchk) != 0)
            {
                // 时间未到或已结束
                continue;
            }
            local drop = val.mon[mon.mid].drop;

            if (!(drop in drop_ids))
            {
                drop_ids[drop] < -mon.cnt;
            }
            else
            {
                drop_ids[drop] += mon.cnt;
            }
        }
    }

    // 根据掉落表，生成掉落数组
    local itms = { gld = 0, bndyb = 0, itm =[], eqp =[] };
    foreach (dpid, cnt in drop_ids)
    {
        for (local i = 0; i < cnt; ++i)
        {
            local single_itms = drop_items(dpid, ply.pinfo.carr);

            foreach (itm in single_itms.itm)
            {
                local combed = false;
                foreach (itmt in itms.itm)
                {
                    if (itmt.id == itm.id)
                    {
                        itmt.cnt += itm.cnt;
                        combed = true;
                        break;
                    }
                }

                if (!combed)
                {
                    itms.itm.push(itm);
                }
            }
            foreach (eqp in single_itms.eqp)
            {
                itms.eqp.push(eqp);
            }

            itms.gld += single_itms.gld;
            itms.bndyb += single_itms.bndyb;
        }
    }

    // 通关奖励道具
    local prize_itms = [];
    local cardawd_itms = []; // 翻牌奖励
    local total_rate = 0;
    if ("drop_itm" in lvl_conf)
    {
        foreach (val in lvl_conf.drop_itm)
        {
            prize_itms.push(val);
            total_rate += val.rate;
        }
    }

    if (("diff_lvl" in lvl_conf) && (diff_lvl in lvl_conf.diff_lvl))
    {
        local diff_lvl_conf = lvl_conf.diff_lvl[diff_lvl];

        if ("drop_itm" in diff_lvl_conf)
        {
            // 难度额外掉落
            foreach (val in diff_lvl_conf.drop_itm)
            {
                prize_itms.push(val);
                total_rate += val.rate;
            }
        }

        if ("awd_itm" in diff_lvl_conf)
        {
            cardawd_itms = diff_lvl_conf.awd_itm;
        }
    }

    if (cardawd_itms.Count > 0)
    {
        // 抽牌奖励
        //ply.pinfo.lvlprize.push({ltpid=rpc.ltpid, diff_lvl=diff_lvl});
    }
    else if (prize_itms.Count > 0)
    {
        local judg = sys.rand_between(0, total_rate);
        local cur_rate = 0;

        foreach (prz in prize_itms)
        {
            cur_rate += prz.rate;

            if (judg < cur_rate)
            {
                // 根据道具掉落数量控制掉落结果
                if (!check_single_dpitm_ctrl(prz.itmid, prz.cnt))
                {
                    // 道具不能掉落了
                    continue;
                }

                itms.itm.push({ id = prz.itmid, cnt = prz.cnt});
                break;
            }
        }
    }

    local res = _check_add_item_to_pl(itms, ply);
    if (res != game_err_code.RES_OK)
    {
        ::send_rpc(sid, 252, { res = res});
        return;
    }

    // 第一次进入消耗物品、金钱等

    // 扣除帮派货币，放在最开始，有可能扣除失败
    local res = _lvl_cost_clangld(ply, lvl_conf);
    if (res != game_err_code.RES_OK)
    {
        ::send_rpc(sid, 252, { res = res});
        return;
    }

    local self_att_msg = { }
    if (lvl_conf.clangld > 0)
    {
        // 在_lvl_cost_clangld函数中扣除了，这里只合并消息
        self_att_msg.clangadd < - -lvl_conf.clangld;
    }
    if (lvl_conf.hexp > 0)
    {
        ply.sub_hexp(lvl_conf.hexp);
        self_att_msg.hexpadd < - -lvl_conf.hexp;
    }
    if (lvl_conf.act_v > 0)
    {
        //if(ply.pinfo.act_v < lvl_conf.act_v)
        //{
        //    // 扣除vip体力值
        //    if(ply.pinfo.vip > 0 && ("vip_data" in ply.pinfo))
        //    {
        //        ply.pinfo.vip_data.lmiscnt -= lvl_conf.act_v;

        //        // send get_vip_res msg
        //        ::send_rpc(sid, 46, {vip_data={lmiscnt=ply.pinfo.vip_data.lmiscnt}});
        //    }
        //}
        //else
        //{
        ply.sub_act_v(lvl_conf.act_v);
        self_att_msg.act_v < -ply.pinfo.act_v;
        //}
    }
    if (self_att_msg.Count > 0)
    {
        // 有变化需要通知

        // send self_attchange msg
        ::send_rpc(sid, 32, self_att_msg);
    }

    local costed_rmvitms = [];
    local costed_rmvitms_cnt = [];
    local modcnt_itm_ary = [];
    local modcnt_itm_ary_cnt = [];

    // 扣除金币花费
    local total_costed = { cnt = 0, rmvids =[], modcnts =[] };
    if (lvl_conf.gld > 0)
    {
        ply.sub_gold(lvl_conf.gld);
        total_costed.gold < - -lvl_conf.gld;

        // log gold
        svr.add_gold_log(gold_act_type.GLDAT_ENTER_LVL, 0, lvl_conf.gld);
    }
    if (yb_cost > 0)
    {
        ply.sub_yb(yb_cost);
        total_costed.yb < - -yb_cost;

        // log yb change
        _log_game_log(game_log_type.GLT_yb_log, { cid = ply.pinfo.cid, uid = ply.pinfo.uid, lvl = ply.pinfo.level, mmisid = ply.pinfo.mmisid, tm = cur_tm_s, act = yb_act_type.YAT_YB_FIN_LVL, sub = yb_cost, parid = rpc.ltpid});
        // 越南方需要发送的数据
        local g_conf_obj = sys.get_svr_config();
        if (("posturl" in g_conf_obj) && (g_conf_obj.posturl.Count > 0))
        {
            local par = { tp = 6, platuid = ply.user_info.platuid, platid = ply.user_info.platid, tm = cur_tm_s, yb = ply.pinfo.yb, yb_cost = yb_cost, itmtpid = yb_act_type.YAT_YB_FIN_LVL, ip = svr.get_session_ip(ply.pinfo.sid) };
            net.url_post(g_conf_obj.posturl + "&svrid=" + g_conf_obj.svrid, "par=" + sys.json_encode(par));
        }
    }
    // 扣除道具
    if ("expenditm" in lvl_conf)
    {
        foreach (val in lvl_conf.expenditm)
        {
            local costed = ply.cost_item(val.tpid, val.cnt);

            if (costed.cnt > 0)
            {
                foreach (val in costed.rmvitms)
                {
                    costed_rmvitms.push(val.itm);
                    costed_rmvitms_cnt.push(val);
                }
                foreach (val in costed.rmvids)
                {
                    total_costed.rmvids.push(val);
                }
                foreach (val in costed.modcnts)
                {
                    total_costed.modcnts.push(val);
                    modcnt_itm_ary.push(val.itm);
                    modcnt_itm_ary_cnt.push(val);
                }
                total_costed.cnt += costed.cnt;
            }
        }
    }
    if (total_costed.cnt > 0 || ("gold" in total_costed) || ("yb" in total_costed))
    {
        if (total_costed.rmvids.Count <= 0)
        {
            delete total_costed.rmvids;
        }

        if (total_costed.modcnts.Count <= 0)
        {
            delete total_costed.modcnts;
        }

        // send item_change msg
        ::send_rpc(sid, 75, total_costed);
    }

    ++lvlentried.ecnt;
    if (vip_lvlcnt)
    {
        --vip_lvlcnt.cnt;

        // send get_vip_res msg
        ::send_rpc(sid, 46, { ltpid = vip_lvlcnt.ltpid, lvlcnt = vip_lvlcnt.cnt});
    }
    else if ("cntleft" in lvlentried)
    {
        --lvlentried.cntleft;
    }

    if ("enter_cd" in lvl_conf)
    {
        // 记录进入冷却时间
        local cdtm = cur_tm_s + lvl_conf.enter_cd;
        ply.pinfo.lvlentercd[lvl_conf.enter_cdtp] < -cdtm;

        // 通知进入冷却时间变化
        // send get_associate_lvls_res msg
        ::send_rpc(sid, 243, { entercds =[{ cdtp = lvl_conf.enter_cdtp, cdtm = cdtm}]});
    }

    // 给道具
    local new_add_itm_ary = _add_item_to_pl(itms, ply, true);

    // 给经验
    local exp_add = sys.rand_between(yb_fin_conf.exp_min, yb_fin_conf.exp_max);
    ply.modify_exp(exp_add, true);

    // 给金币
    local gld_add = sys.rand_between(yb_fin_conf.gld_min, yb_fin_conf.gld_max);
    if (gld_add > 0)
    {
        gld_add = ply.add_gold(gld_add);
        if (gld_add > 0)
        {
            ::send_rpc(sid, 75, { gold = gld_add});

            // log gold
            svr.add_gold_log(gold_act_type.GLDAT_PICK_ITMPKG, gld_add, 0);
        }
    }

    local ret_msg = { tp = 1, ltpid = rpc.ltpid, diff_lvl = diff_lvl, awdtp = 0, exp = exp_add, gold = (gld_add + itms.gld), bndyb = itms.bndyb, itm = itms.itm, eqp = itms.eqp };

    // 给气海
    if ("skexp" in yb_fin_conf)
    {
        ply.modify_skexp(yb_fin_conf.skexp, 1); // 副本奖励气海值
        ret_msg.skexp < -yb_fin_conf.skexp;
    }

    if (cardawd_itms.Count > 0)
    {
        // 给抽牌奖励
        ply.pinfo.lvlprize.push({ ltpid = rpc.ltpid, diff_lvl = diff_lvl});
    }

    // 更新任务杀怪
    foreach (mis in ply.pinfo.misacept)
    {
        local conf = get_mis_conf(mis.misid);

        local mis_goal = _get_mission_goal(ply.pinfo, conf);

        // 更新角色任务杀怪数
        if ("km" in mis)
        {
            foreach (idx, km in mis.km)
            {
                if (!(km.monid in kms))
                {
                    continue;
                }

                local old_km_cnt = km.cnt;
                km.cnt += kms[km.monid];

                if (mis_goal && "kilmon" in mis_goal && mis_goal.kilmon.Count > idx)
                {
                    if (km.cnt >= mis_goal.kilmon[idx].cnt)
                    {
                        km.cnt = mis_goal.kilmon[idx].cnt;

                        if ("follow" in mis_goal.kilmon[idx])
                        {
                            local follow = mis_goal.kilmon[idx].follow;

                            local old_follow = 0;
                            if ("follow" in ply.pinfo)
                            {
                                old_follow = ply.pinfo.follow;
                            }

                            if (follow > 0)
                            {
                                ply.pinfo.follow < -follow;

                                if (old_follow != follow)
                                {
                                    // broad cast attchange msg
                                    ply.broad_cast_zone_msg(26, { iid = ply.pinfo.iid, follow = ply.pinfo.follow});
                                }
                            }
                            else if (old_follow > 0)
                            {
                                delete ply.pinfo.follow;

                                // broad cast attchange msg
                                ply.broad_cast_zone_msg(26, { iid = ply.pinfo.iid, follow = 0});
                            }
                        }
                    }
                }

                if (old_km_cnt != km.cnt)
                {
                    // send mis_data_modify msg
                    ::send_rpc(ply.pinfo.sid, 113, { misid = mis.misid, monid = km.monid, cnt = km.cnt});
                }
            }
        }
    }

    ply.pinfo.autofinlvl < -1; // 标记进行过委托任务

    // 尝试更新每日必做任务
    _try_up_dmis(ply, "lvlwin", { ltpid = rpc.ltpid, diff = diff_lvl});
    _try_up_dmis(ply, "lvlfin", { ltpid = rpc.ltpid, diff = diff_lvl});

    ply.flush_db_data(false, false);

    // send lvl_res msg
    ::send_rpc(sid, 233, ret_msg);

    // log itm
    if (new_add_itm_ary.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_GET_DP_ITEM, itm_flag_type.IFT_NEW_ADD, 0, new_add_itm_ary, []);
    }
    if (costed_rmvitms.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_ENTER_LVL_COST, itm_flag_type.IFT_RMV, rpc.ltpid, costed_rmvitms, costed_rmvitms_cnt);
    }
    if (modcnt_itm_ary.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_ENTER_LVL_COST, itm_flag_type.IFT_MODCNT, rpc.ltpid, modcnt_itm_ary, modcnt_itm_ary_cnt);
    }
}

// 扫荡副本
function _wipe_lvl(sid, rpc, ply)
{
    if (ply.pinfo.lvlwipe.maxcnt <= ply.pinfo.lvlwipe.lvls.Count)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_WIPE_MAX_REACH});
        return;
    }

    local lvl_conf = get_level_conf(rpc.ltpid);
    if (!lvl_conf)
    {
        ::send_rpc(sid, 252, { res = game_err_code.PARAMETER_ERR});
        return;
    }

    local yb_fin_conf = null;
    local diff_lvl = 1;

    if ("yb_fin" in lvl_conf)
    {
        yb_fin_conf = lvl_conf.yb_fin[0];
    }

    if (("diff_lvl" in lvl_conf) && (rpc.diff_lvl in lvl_conf.diff_lvl))
    {
        local diff_lvl_conf = lvl_conf.diff_lvl[rpc.diff_lvl];

        if ("yb_fin" in diff_lvl_conf)
        {
            yb_fin_conf = diff_lvl_conf.yb_fin[0];
        }

        diff_lvl = rpc.diff_lvl;

        if ("attchk" in diff_lvl_conf)
        {
            foreach (attchk in diff_lvl_conf.attchk)
            {
                local res = ply.check_att(attchk);
                if (res != game_err_code.RES_OK)
                {
                    ::send_rpc(sid, 252, { res = res});
                    return;
                }
            }
        }
    }

    if (!yb_fin_conf)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_CANT_WIPE});
        return;
    }

    if (!("wipetm" in yb_fin_conf))
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_CANT_WIPE});
        return;
    }

    if (yb_fin_conf.combpt > ply.pinfo.combpt)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_YBFIN_COMBPT_REQUIRE});
        return;
    }

    local lvlentried = ply.get_entried_lvl(rpc.ltpid);
    if (!lvlentried)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_YB_FIN_NOT_COMPLETE});
        return;
    }

    if (!("fin_diff" in lvlentried))
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_YB_FIN_NOT_COMPLETE});
        return;
    }

    if (lvlentried.fin_diff < diff_lvl)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_YB_FIN_NOT_COMPLETE});
        return;
    }

    local res = _check_lvl_requirement(ply, lvl_conf);
    if (res != game_err_code.RES_OK)
    {
        // requirement error, send err msg
        ::send_rpc(sid, 252, { res = res});
        return;
    }

    local cur_tm_s = sys.time();

    if ("enter_cd" in lvl_conf)
    {
        if ((lvl_conf.enter_cdtp in ply.pinfo.lvlentercd) && ply.pinfo.lvlentercd[lvl_conf.enter_cdtp] > cur_tm_s)
        {
            // 进入副本的冷却时间未到，不能进入, send lvl_err_msg
            ::send_rpc(sid, 252, { res = game_err_code.LEVEL_ENTER_CD_NOT_REACH});
            return;
        }
    }

    local vip_lvlcnt = null;
    if (lvlentried && ("cntleft" in lvlentried) && lvlentried.cntleft <= 0)
    {
        // 进入副本次数已达到上限

        //::send_rpc(sid, 239, {res=game_err_code.LEVEL_ENTRY_CNT_REACHED});
        //return;

        // 检查是否可以vip进入
        if (ply.pinfo.vip <= 0)
        {
            ::send_rpc(sid, 252, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }

        if (!("vip_data" in ply.pinfo))
        {
            ::send_rpc(sid, 252, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }

        if (!("lvlcnt" in ply.pinfo.vip_data))
        {
            ::send_rpc(sid, 252, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }

        foreach (lvlcnt in ply.pinfo.vip_data.lvlcnt)
        {
            if (lvlcnt.ltpid == rpc.ltpid && lvlcnt.cnt > 0)
            {
                vip_lvlcnt = lvlcnt;
            }
        }

        if (!vip_lvlcnt)
        {
            ::send_rpc(sid, 252, { res = game_err_code.LEVEL_ENTRY_CNT_REACHED});
            return;
        }
    }

    local yb_cost = 0;
    if (lvl_conf.yb > 0)
    {
        yb_cost += lvl_conf.yb;
    }

    if (vip_lvlcnt)
    {
        // 消耗vip进入次数，需消耗元宝

        if (!(vip_lvlcnt.ltpid in ply.pinfo.vip_conf.lvlcnt))
        {
            ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});
            return;
        }
        local vip_lvl_conf = ply.pinfo.vip_conf.lvlcnt[vip_lvlcnt.ltpid];
        yb_cost += vip_lvl_conf.yb;
    }

    if (ply.pinfo.yb < yb_cost)
    {
        ::send_rpc(sid, 252, { res = game_err_code.NOT_ENOUGH_YB});
        return;
    }

    // 第一次进入消耗物品、金钱等

    // 扣除帮派货币，放在最开始，有可能扣除失败
    local res = _lvl_cost_clangld(ply, lvl_conf);
    if (res != game_err_code.RES_OK)
    {
        ::send_rpc(sid, 252, { res = res});
        return;
    }

    local self_att_msg = { }
    if (lvl_conf.clangld > 0)
    {
        // 在_lvl_cost_clangld函数中扣除了，这里只合并消息
        self_att_msg.clangadd < - -lvl_conf.clangld;
    }
    if (lvl_conf.hexp > 0)
    {
        ply.sub_hexp(lvl_conf.hexp);
        self_att_msg.hexpadd < - -lvl_conf.hexp;
    }
    if (lvl_conf.act_v > 0)
    {
        //if(ply.pinfo.act_v < lvl_conf.act_v)
        //{
        //    // 扣除vip体力值
        //    if(ply.pinfo.vip > 0 && ("vip_data" in ply.pinfo))
        //    {
        //        ply.pinfo.vip_data.lmiscnt -= lvl_conf.act_v;

        //        // send get_vip_res msg
        //        ::send_rpc(sid, 46, {vip_data={lmiscnt=ply.pinfo.vip_data.lmiscnt}});
        //    }
        //}
        //else
        //{
        ply.sub_act_v(lvl_conf.act_v);
        self_att_msg.act_v < -ply.pinfo.act_v;
        //}
    }
    if (self_att_msg.Count > 0)
    {
        // 有变化需要通知

        // send self_attchange msg
        ::send_rpc(sid, 32, self_att_msg);
    }

    local costed_rmvitms = [];
    local costed_rmvitms_cnt = [];
    local modcnt_itm_ary = [];
    local modcnt_itm_ary_cnt = [];

    // 扣除金币花费
    local total_costed = { cnt = 0, rmvids =[], modcnts =[] };
    if (lvl_conf.gld > 0)
    {
        ply.sub_gold(lvl_conf.gld);
        total_costed.gold < - -lvl_conf.gld;

        // log gold
        svr.add_gold_log(gold_act_type.GLDAT_ENTER_LVL, 0, lvl_conf.gld);
    }
    if (yb_cost > 0)
    {
        ply.sub_yb(yb_cost);
        total_costed.yb < - -yb_cost;

        // log yb change
        _log_game_log(game_log_type.GLT_yb_log, { cid = ply.pinfo.cid, uid = ply.pinfo.uid, lvl = ply.pinfo.level, mmisid = ply.pinfo.mmisid, tm = cur_tm_s, act = yb_act_type.YAT_YB_FIN_LVL, sub = yb_cost, parid = rpc.ltpid});
        // 越南方需要发送的数据
        local g_conf_obj = sys.get_svr_config();
        if (("posturl" in g_conf_obj) && (g_conf_obj.posturl.Count > 0))
        {
            local par = { tp = 6, platuid = ply.user_info.platuid, platid = ply.user_info.platid, tm = cur_tm_s, yb = ply.pinfo.yb, yb_cost = yb_cost, itmtpid = yb_act_type.YAT_YB_FIN_LVL, ip = svr.get_session_ip(ply.pinfo.sid) };
            net.url_post(g_conf_obj.posturl + "&svrid=" + g_conf_obj.svrid, "par=" + sys.json_encode(par));
        }
    }
    // 扣除道具
    if ("expenditm" in lvl_conf)
    {
        foreach (val in lvl_conf.expenditm)
        {
            local costed = ply.cost_item(val.tpid, val.cnt);

            if (costed.cnt > 0)
            {
                foreach (val in costed.rmvitms)
                {
                    costed_rmvitms.push(val.itm);
                    costed_rmvitms_cnt.push(val);
                }
                foreach (val in costed.rmvids)
                {
                    total_costed.rmvids.push(val);
                }
                foreach (val in costed.modcnts)
                {
                    total_costed.modcnts.push(val);
                    modcnt_itm_ary.push(val.itm);
                    modcnt_itm_ary_cnt.push(val);
                }
                total_costed.cnt += costed.cnt;
            }
        }
    }
    if (total_costed.cnt > 0 || ("gold" in total_costed) || ("yb" in total_costed))
    {
        if (total_costed.rmvids.Count <= 0)
        {
            delete total_costed.rmvids;
        }

        if (total_costed.modcnts.Count <= 0)
        {
            delete total_costed.modcnts;
        }

        // send item_change msg
        ::send_rpc(sid, 75, total_costed);
    }

    ++lvlentried.ecnt;
    if (vip_lvlcnt)
    {
        --vip_lvlcnt.cnt;

        // send get_vip_res msg
        ::send_rpc(sid, 46, { ltpid = vip_lvlcnt.ltpid, lvlcnt = vip_lvlcnt.cnt});
    }
    else if ("cntleft" in lvlentried)
    {
        --lvlentried.cntleft;
    }

    if ("enter_cd" in lvl_conf)
    {
        // 记录进入冷却时间
        local cdtm = cur_tm_s + lvl_conf.enter_cd;
        ply.pinfo.lvlentercd[lvl_conf.enter_cdtp] < -cdtm;

        // 通知进入冷却时间变化
        // send get_associate_lvls_res msg
        ::send_rpc(sid, 243, { entercds =[{ cdtp = lvl_conf.enter_cdtp, cdtm = cdtm}]});
    }

    local start_tm_s = cur_tm_s;
    if (ply.pinfo.lvlwipe.lvls.Count > 0)
    {
        start_tm_s = ply.pinfo.lvlwipe.lvls[ply.pinfo.lvlwipe.lvls.Count - 1].fintm;
        if (start_tm_s < cur_tm_s)
        {
            start_tm_s = cur_tm_s;
        }
    }

    local new_wipe_lvl = { fintm = (start_tm_s + yb_fin_conf.wipetm), ltpid = rpc.ltpid, diff_lvl = diff_lvl };

    // 记录副本扫荡
    ply.pinfo.lvlwipe.lvls.push(new_wipe_lvl);

    ply.flush_db_data(false, false);

    // 通知新增副本扫荡记录
    // send lvl_res msg
    ::send_rpc(sid, 233, { tp = 4, fintm = new_wipe_lvl.fintm, ltpid = new_wipe_lvl.ltpid, diff_lvl = new_wipe_lvl.diff_lvl});

    // log itm
    if (costed_rmvitms.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_ENTER_LVL_COST, itm_flag_type.IFT_RMV, rpc.ltpid, costed_rmvitms, costed_rmvitms_cnt);
    }
    if (modcnt_itm_ary.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_ENTER_LVL_COST, itm_flag_type.IFT_MODCNT, rpc.ltpid, modcnt_itm_ary, modcnt_itm_ary_cnt);
    }
}

function _check_wipe_lvl_awd(ply, idx, ret, chktm= true)
{
    if (idx >= ply.pinfo.lvlwipe.lvls.Count)
    {
        return game_err_code.PARAMETER_ERR;
    }

    ret.wipe_lvl = ply.pinfo.lvlwipe.lvls[idx];

    if (chktm)
    {
        if (ret.wipe_lvl.fintm > sys.time())
        {
            return game_err_code.LEVEL_WIPE_TIME_NOT_FIN;
        }
    }

    local lvl_conf = get_level_conf(ret.wipe_lvl.ltpid);
    if (!lvl_conf)
    {
        return game_err_code.PARAMETER_ERR;
    }

    local yb_fin_conf = null;

    if ("yb_fin" in lvl_conf)
    {
        yb_fin_conf = lvl_conf.yb_fin[0];
    }

    if (("diff_lvl" in lvl_conf) && (ret.wipe_lvl.diff_lvl in lvl_conf.diff_lvl))
    {
        local diff_lvl_conf = lvl_conf.diff_lvl[ret.wipe_lvl.diff_lvl];

        if ("yb_fin" in diff_lvl_conf)
        {
            yb_fin_conf = diff_lvl_conf.yb_fin[0];
        }
    }

    if (!yb_fin_conf)
    {
        return game_err_code.LEVEL_CANT_WIPE;
    }

    ret.yb_fin_conf = yb_fin_conf;

    // 模拟计算杀怪，获得金币、经验、道具等

    // 遍历需生成道具怪物，组织掉落表
    local drop_ids = { };
    ret.kms < - { };
    foreach (mon in yb_fin_conf.m)
    {
        // 获取怪物掉落
        local mon_conf = get_monster_conf(mon.mid);
        if (!mon_conf)
        {
            continue;
        }

        if (!(mon.mid in ret.kms))
        {
            ret.kms[mon.mid] < -mon.cnt;
        }
        else
        {
            ret.kms[mon.mid] += mon.cnt;
        }

        if ("drop" in mon_conf)
        {
            if (!(mon_conf.drop in drop_ids))
            {
                drop_ids[mon_conf.drop] < -mon.cnt;
            }
            else
            {
                drop_ids[mon_conf.drop] += mon.cnt;
            }
        }

        // 添加时效掉落
        local cur_local_tm = sys.get_local_time();
        foreach (val in game_data_conf.itemdrops.tm_exdrop)
        {
            if (!(mon.mid in val.mon))
            {
                continue;
            }

            if (_check_tm(cur_local_tm, val.tmchk) != 0)
            {
                // 时间未到或已结束
                continue;
            }
            local drop = val.mon[mon.mid].drop;

            if (!(drop in drop_ids))
            {
                drop_ids[drop] < -mon.cnt;
            }
            else
            {
                drop_ids[drop] += mon.cnt;
            }
        }
    }

    // 根据掉落表，生成掉落数组
    local itms = { gld = 0, bndyb = 0, itm =[], eqp =[] };
    foreach (dpid, cnt in drop_ids)
    {
        for (local i = 0; i < cnt; ++i)
        {
            local single_itms = drop_items(dpid, ply.pinfo.carr);

            foreach (itm in single_itms.itm)
            {
                local combed = false;
                foreach (itmt in itms.itm)
                {
                    if (itmt.id == itm.id)
                    {
                        itmt.cnt += itm.cnt;
                        combed = true;
                        break;
                    }
                }

                if (!combed)
                {
                    itms.itm.push(itm);
                }
            }
            foreach (eqp in single_itms.eqp)
            {
                itms.eqp.push(eqp);
            }

            itms.gld += single_itms.gld;
            itms.bndyb += single_itms.bndyb;
        }
    }

    // 通关奖励道具
    local prize_itms = [];
    local cardawd_itms = []; // 翻牌奖励
    local total_rate = 0;
    if ("drop_itm" in lvl_conf)
    {
        foreach (val in lvl_conf.drop_itm)
        {
            prize_itms.push(val);
            total_rate += val.rate;
        }
    }

    if (("diff_lvl" in lvl_conf) && (ret.wipe_lvl.diff_lvl in lvl_conf.diff_lvl))
    {
        local diff_lvl_conf = lvl_conf.diff_lvl[ret.wipe_lvl.diff_lvl];

        if ("drop_itm" in diff_lvl_conf)
        {
            // 难度额外掉落
            foreach (val in diff_lvl_conf.drop_itm)
            {
                prize_itms.push(val);
                total_rate += val.rate;
            }
        }

        if ("awd_itm" in diff_lvl_conf)
        {
            cardawd_itms = diff_lvl_conf.awd_itm;
        }
    }

    if (cardawd_itms.Count > 0)
    {
        // 抽牌奖励
        //ply.pinfo.lvlprize.push({ltpid=ret.wipe_lvl.ltpid, diff_lvl=ret.wipe_lvl.diff_lvl});
    }
    else if (prize_itms.Count > 0)
    {
        local judg = sys.rand_between(0, total_rate);
        local cur_rate = 0;

        foreach (prz in prize_itms)
        {
            cur_rate += prz.rate;

            if (judg < cur_rate)
            {
                // 根据道具掉落数量控制掉落结果
                if (!check_single_dpitm_ctrl(prz.itmid, prz.cnt))
                {
                    // 道具不能掉落了
                    continue;
                }

                itms.itm.push({ id = prz.itmid, cnt = prz.cnt});
                break;
            }
        }
    }

    local res = _check_add_item_to_pl(itms, ply);
    if (res != game_err_code.RES_OK)
    {
        return res;
    }

    ret.itms = itms;
    ret.cardawd_itms = cardawd_itms;

    return game_err_code.RES_OK;
}
function _give_wipe_lvl_awd(sid, ply, idx, ret)
{
    local cur_tm_s = sys.time();

    // 移除角色副本扫荡记录
    ply.pinfo.lvlwipe.lvls.remove(idx);

    // 给道具
    local new_add_itm_ary = _add_item_to_pl(ret.itms, ply, true);

    // 给经验
    local exp_add = sys.rand_between(ret.yb_fin_conf.exp_min, ret.yb_fin_conf.exp_max);
    ply.modify_exp(exp_add, true);

    // 给金币
    local gld_add = sys.rand_between(ret.yb_fin_conf.gld_min, ret.yb_fin_conf.gld_max);
    if (gld_add > 0)
    {
        gld_add = ply.add_gold(gld_add);
        if (gld_add > 0)
        {
            ::send_rpc(sid, 75, { gold = gld_add});

            // log gold
            svr.add_gold_log(gold_act_type.GLDAT_PICK_ITMPKG, gld_add, 0);
        }
    }

    local awd_msg = { tp = 1, ltpid = ret.wipe_lvl.ltpid, diff_lvl = ret.wipe_lvl.diff_lvl, awdtp = 1, exp = exp_add, gold = (gld_add + ret.itms.gld), bndyb = ret.itms.bndyb, itm = ret.itms.itm, eqp = ret.itms.eqp };

    // 给气海
    if ("skexp" in ret.yb_fin_conf)
    {
        ply.modify_skexp(ret.yb_fin_conf.skexp, 1); // 副本奖励气海值
        awd_msg.skexp < -ret.yb_fin_conf.skexp;
    }

    if (ret.cardawd_itms.Count > 0)
    {
        // 给抽牌奖励
        ply.pinfo.lvlprize.push({ ltpid = ret.wipe_lvl.ltpid, diff_lvl = ret.wipe_lvl.diff_lvl});
    }

    // 更新任务杀怪
    foreach (mis in ply.pinfo.misacept)
    {
        local conf = get_mis_conf(mis.misid);

        local mis_goal = _get_mission_goal(ply.pinfo, conf);

        // 更新角色任务杀怪数
        if ("km" in mis)
        {
            foreach (idx, km in mis.km)
            {
                if (!(km.monid in ret.kms))
                {
                    continue;
                }

                local old_km_cnt = km.cnt;
                km.cnt += ret.kms[km.monid];

                if (mis_goal && "kilmon" in mis_goal && mis_goal.kilmon.Count > idx)
                {
                    if (km.cnt >= mis_goal.kilmon[idx].cnt)
                    {
                        km.cnt = mis_goal.kilmon[idx].cnt;

                        if ("follow" in mis_goal.kilmon[idx])
                        {
                            local follow = mis_goal.kilmon[idx].follow;

                            local old_follow = 0;
                            if ("follow" in ply.pinfo)
                            {
                                old_follow = ply.pinfo.follow;
                            }

                            if (follow > 0)
                            {
                                ply.pinfo.follow < -follow;

                                if (old_follow != follow)
                                {
                                    // broad cast attchange msg
                                    ply.broad_cast_zone_msg(26, { iid = ply.pinfo.iid, follow = ply.pinfo.follow});
                                }
                            }
                            else if (old_follow > 0)
                            {
                                delete ply.pinfo.follow;

                                // broad cast attchange msg
                                ply.broad_cast_zone_msg(26, { iid = ply.pinfo.iid, follow = 0});
                            }
                        }
                    }
                }

                if (old_km_cnt != km.cnt)
                {
                    // send mis_data_modify msg
                    ::send_rpc(ply.pinfo.sid, 113, { misid = mis.misid, monid = km.monid, cnt = km.cnt});
                }
            }
        }
    }

    // 尝试更新每日必做任务
    _try_up_dmis(ply, "lvlwin", { ltpid = ret.wipe_lvl.ltpid, diff = ret.wipe_lvl.diff_lvl});
    _try_up_dmis(ply, "lvlfin", { ltpid = ret.wipe_lvl.ltpid, diff = ret.wipe_lvl.diff_lvl});

    local tmleft = ret.wipe_lvl.fintm - cur_tm_s;
    local ret_msg = { tp = 5, idx = idx };
    if (tmleft > 0)
    {
        // 尚未完成的扫荡被立即完成，队列后续扫荡副本完成时间都需相应减少
        for (local i = idx; i < ply.pinfo.lvlwipe.lvls.Count; ++i)
        {
            ply.pinfo.lvlwipe.lvls[i].fintm -= tmleft;
        }
        ret_msg.redtm < -tmleft;
    }

    // 如果是侠客行副本，更新相关数据
    local lvlmis_conf = get_lvlmis_conf(ret.wipe_lvl.ltpid);
    if (lvlmis_conf)
    {
        // 尝试更新每日必做任务
        _try_up_dmis(ply, "lvlmis", lvlmis_conf.tpid);

        // 尝试更新排行活动值
        _rnkact_on_lvlmis(ply, lvlmis_conf);
    }

    ply.flush_db_data(false, false);

    // 通知副本扫荡完成
    // send lvl_res msg
    ::send_rpc(sid, 233, ret_msg);

    // 通知获得奖励
    // send lvl_res msg
    ::send_rpc(sid, 233, awd_msg);

    // log itm
    if (new_add_itm_ary.Count > 0)
    {
        _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_GET_DP_ITEM, itm_flag_type.IFT_NEW_ADD, 0, new_add_itm_ary, []);
    }
}
// 元宝完成扫荡副本cd
function _ybfin_wipe_lvl(sid, rpc, ply)
{
    local ret = { wipe_lvl = null, yb_fin_conf = null, itms = null, cardawd_itms = null, kms = null };
    local res = _check_wipe_lvl_awd(ply, rpc.idx, ret, false);
    if (res != game_err_code.RES_OK)
    {
        // send lvl_res msg
        ::send_rpc(sid, 252, { res = res});
        return;
    }

    local cur_tm_s = sys.time();

    // 找到当前在进行扫荡中的副本
    local i = 0;
    for (; i < ply.pinfo.lvlwipe.lvls.Count; ++i)
    {
        if (ply.pinfo.lvlwipe.lvls[i].fintm > cur_tm_s)
        {
            break;
        }
    }

    if (i < rpc.idx)
    {
        // 非当前进行中副本，不能立即完成扫荡
        ::send_rpc(sid, 252, { res = game_err_code.WAIT_LVLWIPE_CANT_YBFIN});
        return;
    }

    // 计算立即完成元宝消耗
    local yb_cost = (ceil(ret.yb_fin_conf.yb * (ret.wipe_lvl.fintm - cur_tm_s).tofloat() / ret.yb_fin_conf.wipetm)).tointeger();
    if (yb_cost <= 0)
    {
        yb_cost = 1;
    }

    if (ply.pinfo.yb < yb_cost)
    {
        ::send_rpc(sid, 252, { res = game_err_code.NOT_ENOUGH_YB});
        return;
    }

    ply.sub_yb(yb_cost);
    ::send_rpc(sid, 75, { yb = -yb_cost});

    // log yb change
    _log_game_log(game_log_type.GLT_yb_log, { cid = ply.pinfo.cid, uid = ply.pinfo.uid, lvl = ply.pinfo.level, mmisid = ply.pinfo.mmisid, tm = cur_tm_s, act = yb_act_type.YAT_YB_FIN_LVL, sub = yb_cost, parid = ret.wipe_lvl.ltpid});
    // 越南方需要发送的数据
    local g_conf_obj = sys.get_svr_config();
    if (("posturl" in g_conf_obj) && (g_conf_obj.posturl.Count > 0))
    {
        local par = { tp = 6, platuid = ply.user_info.platuid, platid = ply.user_info.platid, tm = cur_tm_s, yb = ply.pinfo.yb, yb_cost = yb_cost, itmtpid = yb_act_type.YAT_YB_FIN_LVL, ip = svr.get_session_ip(ply.pinfo.sid) };
        net.url_post(g_conf_obj.posturl + "&svrid=" + g_conf_obj.svrid, "par=" + sys.json_encode(par));
    }

    _give_wipe_lvl_awd(sid, ply, rpc.idx, ret);
}
// 领取扫荡副本奖励
function _get_wipe_lvl_awd(sid, rpc, ply)
{
    local ret = { wipe_lvl = null, yb_fin_conf = null, itms = null, cardawd_itms = null, kms = null };
    local res = _check_wipe_lvl_awd(ply, rpc.idx, ret, true);
    if (res != game_err_code.RES_OK)
    {
        // send lvl_res msg
        ::send_rpc(sid, 252, { res = res});
        return;
    }

    _give_wipe_lvl_awd(sid, ply, rpc.idx, ret);
}
// 增加副本扫荡队列
function _add_lvl_wipe_cnt(sid, rpc, ply)
{
    if (ply.pinfo.lvlwipe.maxcnt + rpc.cnt > ply.game_conf.lvlwipe_max_cnt)
    {
        ::send_rpc(sid, 252, { res = game_err_code.MAX_LEVEL_WIPE_CNT_REACH});
        return;
    }

    local yb_cost = rpc.cnt * ply.game_conf.lvlwipe_yb_per_cnt;

    if (ply.pinfo.yb < yb_cost)
    {
        ::send_rpc(sid, 252, { res = game_err_code.NOT_ENOUGH_YB});
        return;
    }

    ply.sub_yb(yb_cost);
    ::send_rpc(sid, 75, { yb = -yb_cost});

    // log yb change
    _log_game_log(game_log_type.GLT_yb_log, { cid = ply.pinfo.cid, uid = ply.pinfo.uid, lvl = ply.pinfo.level, mmisid = ply.pinfo.mmisid, tm = sys.time(), act = yb_act_type.YAT_ADDLVLWIPE, sub = yb_cost, parid = rpc.cnt});
    // 越南方需要发送的数据
    local g_conf_obj = sys.get_svr_config();
    if (("posturl" in g_conf_obj) && (g_conf_obj.posturl.Count > 0))
    {
        local par = { tp = 6, platuid = ply.user_info.platuid, platid = ply.user_info.platid, tm = sys.time(), yb = ply.pinfo.yb, yb_cost = yb_cost, itmtpid = yb_act_type.YAT_ADDLVLWIPE, ip = svr.get_session_ip(ply.pinfo.sid) };
        net.url_post(g_conf_obj.posturl + "&svrid=" + g_conf_obj.svrid, "par=" + sys.json_encode(par));
    }


    ply.pinfo.lvlwipe.maxcnt = ply.pinfo.lvlwipe.maxcnt + rpc.cnt;

    // 通知副本扫荡队列增加
    // send lvl_res msg
    ::send_rpc(sid, 233, { tp = 6, maxcnt = ply.pinfo.lvlwipe.maxcnt});
}

// 获取侠客行副本首破、最佳通关信息
function _get_lmis_rnk_info(sid, rpc, ply)
{
    // 记录侠客行副本霸主、首破、最近通关信息
    local fin_info_arys = [];

    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_LVLMIS_RNK_INFO, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local glblvlmisrnk_data = _get_glblvlmisrnk_data();
    if (glblvlmisrnk_data)
    {
        local glblvlmisrnk_info = glblvlmisrnk_data.get_data();
        if (glblvlmisrnk_info)
        {
            if (rpc.fintp == 0)
            {
                // 获取首破
                foreach (fin_info in glblvlmisrnk_info.data.ffin)
                {
                    fin_info_arys.push(fin_info);
                }
            }
            else if (rpc.fintp == 1)
            {
                // 获取最佳通关
                foreach (fin_info in glblvlmisrnk_info.data.bfin)
                {
                    fin_info_arys.push(fin_info);
                }
            }
        }
    }
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_LVLMIS_RNK_INFO);  // 解锁

    local sort_func = function(a, b)
    {
        if (a.ltpid > b.ltpid)
        {
            return -1;
        }
        else if (a.ltpid < b.ltpid)
        {
            return 1;
        }

        return 0;
    }
    fin_info_arys.sort(sort_func);

    local ret_ary = [];
    for (local i = 0; i < fin_info_arys.Count; ++i)
    {
        if (i < rpc.begin_idx)
        {
            continue;
        }

        if (i >= rpc.end_idx)
        {
            break;
        }

        ret_ary.push(fin_info_arys[i]);
    }

    // send lvl_res msg
    ::send_rpc(sid, 233, { tp = 2, fintp = rpc.fintp, begin_idx = rpc.begin_idx, total_cnt = fin_info_arys.Count, infos = ret_ary});
}

// 获取单个侠客行副本首破、最佳通关信息
function _get_lmis_single_info(sid, rpc, ply)
{
    local ffin_map = { };
    local bfin_map = { };

    local glbdata_mgr = global_data_mgrs.glbdata;
    glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_LVLMIS_RNK_INFO, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
    local glblvlmisrnk_data = _get_glblvlmisrnk_data();
    if (glblvlmisrnk_data)
    {
        local glblvlmisrnk_info = glblvlmisrnk_data.get_data();
        if (glblvlmisrnk_info)
        {
            ffin_map = glblvlmisrnk_info.data.ffin; // 首破
            bfin_map = glblvlmisrnk_info.data.bfin; // 最佳通关
        }
    }
    glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_LVLMIS_RNK_INFO);  // 解锁

    local ret_msg = { tp = 3, ltpid = rpc.ltpid };
    if (rpc.ltpid in ffin_map)
    {
        ret_msg.ffin < -ffin_map[rpc.ltpid];
    }
    if (rpc.ltpid in bfin_map)
    {
        ret_msg.bfin < -bfin_map[rpc.ltpid];
    }

    // send lvl_res msg
    ::send_rpc(sid, 233, ret_msg);
}

function _buy_lvl_state(sid, rpc, ply )
{
    if (!ply.is_in_lvl)
    {
        ::send_rpc(sid, 252, { res = game_err_code.LEVEL_YOU_NOT_IN_LEVEL});
        return;
    }

    local lvl = ply.gmap.worldsvr;
    if (!("buy_state" in lvl.lvl_conf) || !(rpc.id in lvl.lvl_conf.buy_state) )
    {
        ::send_rpc(sid, 252, { res = game_err_code.PARAMETER_ERR});
        return;
    }
    local buy_state_conf = lvl.lvl_conf.buy_state[rpc.id];

    local state_conf = get_skil_state_desc(buy_state_conf.state.tar_state);
    if (state_conf == null)
    {

        ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});
        return;
    }

    local state_cnt = 0;
    if ("states" in ply.pinfo)
    {
        local state_unique_conf = ("unique" in state_conf) ? state_conf.unique : null;
        foreach (idx, val in ply.pinfo.states.state_par)
        {
            if (val.id == state_conf.id)
            {
                state_cnt++;
            }
            else
            {
                if (state_unique_conf)
                {
                    if (("unique" in val.desc) && state_unique_conf.uni_tp == val.desc.unique.uni_tp )
                    {
                        if (val.desc.unique.uni_prior > state_unique_conf.uni_prior)
                        {
                            ::send_rpc(sid, 252, { res = game_err_code.STATE_HAS_BETTER_STATE});
                            return;
                        }
                    }
                }
            }
        }

        if (state_conf.max_cnt > 0)
        {
            if (state_conf.max_cnt <= state_cnt)
            {
                ::send_rpc(sid, 252, { res = game_err_code.STATE_CONT_ADD_MORE});
                return;
            }
        }
        else if (state_cnt > 0)
        {
           ::send_rpc(sid, 252, { res = game_err_code.STATE_CONT_ADD_MORE});
            return;
        }
    }

    if (!(state_cnt in buy_state_conf.level) )
    {
        ::send_rpc(sid, 252, { res = game_err_code.ITEM_CANT_ADD_STATE});
        return;
    }

    local buy_state_level_conf = buy_state_conf.level[state_cnt];

    //sys.dumpobj( rpc );
    //sys.dumpobj( buy_state_level_conf );
    //消耗
    local succ = false;
    local yb_cost = 0;
    local hexp_cost = 0;
    local gold_cost = 0;
    switch (rpc.cost_tp)
    {
        case 1:
            {
                if (!("yb" in buy_state_level_conf ) || buy_state_level_conf.yb == 0 )
                {
                    ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});
                    Utility.trace_err( "buy_state_conf yb not exist or is zero id=" + rpc.id + "  level=" + state_cnt + "\n");
                    return;
                }
                yb_cost = buy_state_level_conf.yb;
                if (ply.pinfo.yb < yb_cost)
                {
                    ::send_rpc(sid, 252, { res = game_err_code.NOT_ENOUGH_YB});
                    return;
                }

                succ = buy_state_level_conf.yrate > sys.rand_between(0, 100);
            }
            break;
        case 2:
            {
                if (!("hexp" in buy_state_level_conf ) || buy_state_level_conf.hexp == 0 )
                {
                    ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});
                    Utility.trace_err( "buy_state_conf hexp not exist or is zero id=" + rpc.id + "  level=" + state_cnt + "\n");
                    return;
                }

                hexp_cost = buy_state_level_conf.hexp;
                if (ply.pinfo.hexp < hexp_cost)
                {
                    ::send_rpc(sid, 252, { res = game_err_code.NOT_ENOUGH_HONER});
                    return;
                }

                succ = buy_state_level_conf.hrate > sys.rand_between(0, 100);
            }
            break;
        case 3:
            {
                if (!("gold" in buy_state_level_conf) || buy_state_level_conf.gold == 0 )
                {
                    ::send_rpc(sid, 252, { res = game_err_code.CONFIG_ERR});
                    Utility.trace_err( "buy_state_conf gold not exist or is zero id=" + rpc.id + "  level=" + state_cnt + "\n");
                    return;
                }
                gold_cost = buy_state_level_conf.gold;
                if (ply.pinfo.gold < gold_cost)
                {
                    ::send_rpc(sid, 252, { res = game_err_code.NOT_ENOUGH_GOLD});
                    return;
                }

                succ = buy_state_level_conf.grate > sys.rand_between(0, 100);
            }
            break;
        default:
            ::send_rpc(sid, 252, { res = game_err_code.PARAMETER_ERR})
            break;
    }

    local ret_msg = { tp = rpc.tp, succ = false }

    local state_obj;
    if (succ)
    {
        local cur_clock_tm = sys.clock_time();
        state_obj = add_state_to_pl(cur_clock_tm, ply, buy_state_conf.state, null, 1000, false);
        if (state_obj == null)
        {
            ::send_rpc(sid, 252, { res = game_err_code.ITEM_CANT_ADD_STATE});
            return;
        }
        ret_msg.succ = true;
    }

    if (yb_cost > 0)
    {
        local cur_tm_s = sys.time();
        ply.sub_yb(yb_cost);

        ret_msg.cost_yb < -yb_cost;

        lvl.on_ply_cost_yb(ply, yb_act_type.YAT_BUY_LVL_STATE, yb_cost);

        // log yb change
        _log_game_log(game_log_type.GLT_yb_log, { cid = ply.pinfo.cid, uid = ply.pinfo.uid, lvl = ply.pinfo.level, mmisid = ply.pinfo.mmisid, tm = cur_tm_s, act = yb_act_type.YAT_BUY_LVL_STATE, sub = yb_cost, parid = rpc.id, parcnt = 0});
        // 越南方需要发送的数据
        local g_conf_obj = sys.get_svr_config();
        if (("posturl" in g_conf_obj) && (g_conf_obj.posturl.Count > 0))
        {
            local par = { tp = 6, platuid = ply.user_info.platuid, platid = ply.user_info.platid, tm = cur_tm_s, yb = ply.pinfo.yb, yb_cost = yb_cost, itmtpid = yb_act_type.YAT_BUY_LVL_STATE, ip = svr.get_session_ip(ply.pinfo.sid) };
            net.url_post(g_conf_obj.posturl + "&svrid=" + g_conf_obj.svrid, "par=" + sys.json_encode(par));
        }
    }
    if (gold_cost > 0)
    {
        ply.sub_gold(gold_cost);

        ret_msg.cost_gold < -gold_cost;

        lvl.on_ply_cost_gold(ply, gold_act_type.GLDAT_BUY_LVL_STATE, gold_cost);

        svr.add_gold_log(gold_act_type.GLDAT_BUY_LVL_STATE, 0, gold_cost);
    }
    if (hexp_cost > 0)
    {
        ply.sub_hexp(hexp_cost);

        ret_msg.cost_hexp < -hexp_cost;
    }

    ::send_rpc(sid, 233, ret_msg);

    if (state_obj)
    {
        _remark_pl_state(ply, ply.pinfo);
        ply.broad_cast_zone_msg_and_self(24, { iid = ply.pinfo.iid, states =[state_obj]});

        if (("broadcast" in buy_state_conf) && buy_state_conf.broadcast == 1 )
        {
            // 广播抽奖获得道具消息
            _broad_cast_sys_msg({ cid = ply.pinfo.cid, name = ply.pinfo.name, tp = bcast_msg_tp.BUY_LVL_BUFF, par = state_conf.id, par1 = state_cnt + 1});
        }
    }
}

// 副本相关消息 
function lvl_do(sid, rpc, ply)
{
    switch (rpc.tp)
    {
        case 1: // 元宝立即通关副本
            {
                _ybfin_lvl(sid, rpc, ply);
            }
            break;
        case 2: // 获取侠客行副本首破、最佳通关信息
            {
                _get_lmis_rnk_info(sid, rpc, ply);
            }
            break;
        case 3: // 获取单个侠客行副本首破、最佳通关信息
            {
                _get_lmis_single_info(sid, rpc, ply);
            }
            break;
        case 4: // 扫荡副本
            {
                _wipe_lvl(sid, rpc, ply);
            }
            break;
        case 5: // 元宝完成扫荡副本cd
            {
                _ybfin_wipe_lvl(sid, rpc, ply);
            }
            break;
        case 6: // 领取扫荡副本奖励
            {
                _get_wipe_lvl_awd(sid, rpc, ply);
            }
            break;
        case 7: // 增加副本扫荡队列
            {
                _add_lvl_wipe_cnt(sid, rpc, ply);
            }
            break;
        case 8: // 获取副本扫荡相关信息
            {
            ::send_rpc(sid, 233, { tp = 7, lvlwipe = ply.pinfo.lvlwipe});
            }
            break;
        case 9: // 购买副本BUFF
            {
                _buy_lvl_state(sid, rpc, ply);
            }
            break;
    }
}
function _give_joinlvl_awd(player)
{
    //进入副本地图奖励
    local map_awd = null;
    local m_conf = this.lvl_conf.map;
    if (this.diff_lvl in this.lvl_conf.diff_lvl)
    {
        local diff_lvl_conf = this.lvl_conf.diff_lvl[this.diff_lvl];
        if ("map" in diff_lvl_conf) m_conf = diff_lvl_conf.map;
    }
    foreach (mapconf in m_conf)
    {
        if (player.pinfo.map_id == mapconf.id)
        {   //玩家当前进入的地图是否有奖励
            if ("awd" in mapconf)
            {
                map_awd = _get_awd_conf(player, mapconf.awd);
            }
            break;
        }
    }
    if (map_awd)
    {
        local cur_tm_s = sys.time();
        give_lvlfin_awd(cur_tm_s, [player], map_awd, this.ltpid);
    }
}

function _get_awd_conf(ply, conf)
{
    foreach (awdconf in conf)
    {
        if (!("attchk" in conf)) return awdconf;

        local pass = true;
        foreach (attchk in awdconf.attchk)
        {
            if (ply.check_att(attchk) != game_err_code.RES_OK)
            {
                pass = false;
                break;
            }
        }
        if (pass) return awdconf;
    }
    return null;
}

function give_lvlfin_awd(cur_tm_s, plys, awd_conf, ltpid)
{
    local exp = ("exp" in awd_conf) ? awd_conf.exp : 0;
    local gld = ("gld" in awd_conf) ? awd_conf.gld : 0;
    local bndyb = ("bndyb" in awd_conf) ? awd_conf.bndyb : 0;
    local skexp = ("skexp" in awd_conf) ? awd_conf.skexp : 0;
    local nobpt = ("nobpt" in awd_conf) ? awd_conf.nobpt : 0;
    local meript = ("meript" in awd_conf) ? awd_conf.meript : 0;
    local hexp = ("hexp" in awd_conf) ? awd_conf.hexp : 0;
    local real_gold = 0;
    local total_gold = 0;
    local item_change_msg;
    foreach (ply in plys)
    {
        if (exp > 0)
        {
            ply.modify_exp(exp);
        }
        item_change_msg = { };
        if (gld > 0)
        {
            real_gold = ply.add_gold(gld);
            if (real_gold > 0)
            {
                item_change_msg.gold < -real_gold;

                // 尝试更新排行活动值
                _rnkact_on_addgld(ply, real_gold);
                total_gold += real_gold;
            }
        }
        if (bndyb > 0)
        {
            // 获得礼券
            ply.add_bndyb(bndyb);
            item_change_msg.bndyb < -bndyb;

            // log bndyb change
            _log_bndyb_log(ply, cur_tm_s, bndyb_act_type.BAT_LVL_AWD, bndyb, 0, ltpid, 0);
        }
        if (skexp > 0)
        {
            ply.modify_skexp(skexp, 1); // 副本奖励气海值
        }
        if (nobpt > 0)
        {
            ply.add_nobpt(nobpt);
        }
        if (meript > 0)
        {
            ply.add_meript(meript);
        }
        if (hexp > 0)
        {
            local game_conf = get_general_game_conf();
            if ("give_lvlfin_hexp_cntlimit" in game_conf)
            {   //如果有限制， 判断玩家是否超过领取次数
                if (ply.pinfo.arena_cwin <= game_conf.give_lvlfin_hexp_cntlimit)
                {
                    ply.add_hexp(hexp);
                }
            }
            else
            {   //没有次数限制，就直接发放给玩家
                ply.add_hexp(hexp);
            }

        }
        if (item_change_msg.Count > 0)
        {
            // send item_change msg
            ::send_rpc(ply.pinfo.sid, 75, item_change_msg);
        }
    }

    if (total_gold > 0)
    {
        // log gold
        svr.add_gold_log(gold_act_type.GLDAT_LVL_AWD, total_gold, 0);
    }
}

llid = 0;
    ltpid = 0;
    diff_lvl = 0;
    creator = 0;
    lvl_conf = null;
    born_map_conf = null;
    
    sgplayers = null;
    sgplayersbycid = null;
    sgplayersbyside = null;
    sidary = null;
    maps = null;
    clansideid_ary = null;
    clanside = {};    // 帮派战场分阵营数组,帮派id为key，阵营id为值

    gpmap_maxply = 0;
    gp_maps = null; //地图组  id={maps={},plys={}}
    cid2gpid = null; //角色到地图组的映射 cid=gpid

    sgplayerside_bycid = null;
    sides_conf = null;

    sideclan = null;

    ghost_players=null;
    ghost_cnt = 0;
    death_match = false;
    death_hold_map = 0;
    preptm = 0; // 准备结束时间
    pkrest_tm = 0; // 地图pk设置恢复时间
    plyrespawn_tm = 0; // 玩家死亡后强制等待复活时间限制,=0，立即复活
    plyrespawn_tms = null;
    immrespawn = false; // 是否不允许原地复活

    start_tm = 0;
    end_tm = 0;
    close_tm = 0;
    state = level_state_type.LST_NONE;
    win = -1;

    service = null;

    active_finchk = true;
    kmfin = null;
    kmfinbyside = null;
    ptfinbyside = null;
    maxptside = -1;
    prize = null;
    score_km = null;
    is_score_km = false;

    tmfin = false;

    tm_cost = false;
    total_tm_cost = null;   //记录玩家总花费  用于离开时记录log [cid]={}

    tm_out = 0;
    joininfo = null;        //记录副本中玩家进入时间
    map_need_km = null;     //进入地图需要击杀怪物
    enter_map_plys = null;

    death_pts = null;
    death_ptconf = null;

    kumiteply = null;
    kumite_conf = null;
    kumitefight_ply = null;
    kumite_rnd_tm = 0;
    kumite_waittm = 0;

    cltwar = null;  // 帮派领地争夺战数据
    clcqwar = null; // 帮派奴役战数据

    kprec = null;
    kpboard = null; // 击杀排行榜，前10

    rounds_conf = null;
    cur_round = 0;
    round_tm = 0;
    round_plys = null; // 玩家晋级回合记录

    ignore_clan = false;
    ignore_team = false;
    rmv_leave_ply = false;
    plyside = 0;

    camp_side = {};  // 按世界阵营分边

    no_kp_hexp = false;

    clter_conf = null;  // 帮派领地配置
    next_check_clter_tm = 0;

    clanmon_aff = false;    // 是否开启帮派科技副本攻防加成
    clanmon_att_add = {};   // 本副本帮派科技怪物加成数据
    clanply_att_add = {};   // 本副本帮派玩家属性加成数据

    team_exdrop = null; // 组队额外掉落
    dota = null;    // dota战场内数据

    rnk_batl = false; // 是否有排行奖励战场

    kick_out_plys = null; //将被踢出副本的玩家

    stastic_lvl_cost = null; //统计副本内消耗{ yb[tp]=cost, total_yb_cost=0, gold[tp]=cost, total_gld_cost=0, }

	lvl_fined = false;
	
    