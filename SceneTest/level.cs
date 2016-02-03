using SceneTestLib.Confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
    public class _score_km
    {
        public int mid { get; set; }
        public int cnt { get; set; }
    }

    public class ptfinbyside
    {
        public int pt { get; set; }
        public int maxpt { get; set; }
    }

    public class score_km
    {
        public Dictionary<int, _score_km> _score_km = new Dictionary<int, _score_km>();
        public int kmcnt { get; set; }

        public bool ContainsKey(int id)
        {
            return _score_km.ContainsKey(id);
        }
    }

    //public class death_pts {
    //    public int cid { get; set; }
    //    public int pt { get; set; }
    //}


    public class kmfin
    {
        public int cntleft { get; set; }
    }
    public class Death_Pt
    {
        public int cid;
        public int pt;
    }
    public class JoinInfo
    {
        public int cid { get; set; }
        public int sid { get; set; }
        public int mapid
        {
            get; set;
        }

        public long join_tm { get; set; }
    }

    public class round_player
    {
        public int cid
        {
            get; set;
        }
        public int rnd
        {
            get; set;
        }
    }
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

        public Dictionary<int, kmfin> kmfin
        {
            get; set;
        }
        public bool tmfin { get; set; }
        public Dictionary<int, IBaseUnit> plys { get; set; }
        public int plycnt { get; set; }
        public bool fin { get; set; }
        public bool closed { get; set; }
        public int win { get; set; }

        public long close_tm { get; set; }

        public group_map()
        {
            maps = new Dictionary<int, grid_map>();
            this.kmfin = new Dictionary<int, int>();
            this.tmfin = false;
            this.fin = false;
            this.closed = false;
            this.win = -1;

            this.plycnt = 0;
            this.plys = new Dictionary<int, IBaseUnit>();
        }

        public group_map(int _gpid)
        {
            maps = new Dictionary<int, grid_map>();
            this.kmfin = new Dictionary<int, int>();
            this.tmfin = false;
            this.fin = false;
            this.closed = false;
            this.win = -1;

            this.plycnt = 0;
            this.plys = new Dictionary<int, IBaseUnit>();

            this.gpid = _gpid;
        }

    }

    public class Level_Info
    {
        public int llid { get; set; }
        public int diff_lvl { get; set; }

        public int ltpid { get; set; }

        public int creator { get; set; }
    }

    public class Level : IService
    {

        public static Dictionary<int, Level_Info> level_infos = new Dictionary<int, Level_Info>();

        public Dictionary<int, IBaseUnit> sgplayers { get; set; }
        public Dictionary<int, IBaseUnit> sgplayersbycid { get; set; }
        public Dictionary<int, Dictionary<int, IBaseUnit>> sgplayersbyside { get; set; }

        public Dictionary<int, JoinInfo> joininfo { get; set; }

        //public Dictionary<int, IBaseUnit> dota { get; set; }
        public Dictionary<int, grid_map> maps { get; set; }
        public Dictionary<int, long> plyrespawn_tms = new Dictionary<int, long>();
        public List<group_map> gp_maps { get; set; }

        public Dictionary<int, Dictionary<int, IBaseUnit>> enter_map_plys = new Dictionary<int, Dictionary<int, IBaseUnit>>();

        public int gpmap_maxply { get; set; }

        public Dictionary<int, int> cid2gpid { get; set; }
        public Dictionary<int, int> sgplayerside_bycid { get; set; }

        public Dictionary<int, int> sideclan { get; set; }

        public level_conf lvl_conf { get; set; }

        public score_km score_km = new score_km();

        public int llid { get; set; }

        public int ltpid { get; set; }

        public int diff_lvl { get; set; }

        public int creator { get; set; }

        public long start_tm { get; set; }

        public long end_tm { get; set; }

        public int win = -1;

        public born_pos born_map_conf { get; set; }

        public Dictionary<int, round_conf> rounds_conf = new Dictionary<int, round_conf>();
        public int cur_round { get; set; }

        public long round_tm { get; set; }

        public bool rmv_leave_ply { get; set; }

        public List<side_conf> sides_conf = new List<side_conf>();

        public death_pt_conf death_ptconf { get; set; }

        public Dictionary<int, Death_Pt> death_pts = new Dictionary<int, Death_Pt>();

        public long pkrest_tm { get; set; }

        /// <summary>
        /// cid -> 剩余死亡次数 映射表
        /// </summary>
        public Dictionary<int, int> ghost_players { get; set; }

        public bool lvl_fined { get; set; }

        public long tm_out { get; set; }

        public long close_tm { get; set; }

        public level_state_type state { get; set; }

        public HashSet<int> sid_ary { get; set; }

        public Dictionary<int, kmfin> kmfin = new Dictionary<int, kmfin>();

        public Dictionary<int, round_player> round_plys = new Dictionary<int, round_player>();

        public Dictionary<int, ptfinbyside> ptfinbyside = new Dictionary<int, SceneTest.ptfinbyside>();
        //public Dictionary<int, death_pts> death_pts = new Dictionary<int, SceneTest.death_pts>();
        public int maxptside { get; set; }

        public bool tmfin = false;

        public bool ignore_clan = false;
        public bool ignore_team = false;
        public int plyside = 0;
        public int plyrespawn_tm { get; set; }

        public bool immrespawn { get; set; }

        public int ghost_cnt = 0;
        public bool death_match = false;
        public int death_hold_map = 0;
        public long preptm = 0;
        public bool rnk_batl = false;

        public bool is_score_km = false;

        public static void enter_world(int sid, int line)
        {

        }

        public static void set_lvl_end_tm(int llid, long end_tm)
        {

        }

        public Level()
        {
            this.sid_ary = new HashSet<int>();
            //::g_debug( "====>>> on sg_level constructor()");
            this.joininfo = new Dictionary<int, JoinInfo>();
            this.sgplayers = new Dictionary<int, IBaseUnit>();
            this.sgplayersbycid = new Dictionary<int, IBaseUnit>();
            this.sgplayersbyside = new Dictionary<int, Dictionary<int, IBaseUnit>>();
            this.maps = new Dictionary<int, grid_map>();

            this.gpmap_maxply = 0;
            this.gp_maps = new List<group_map>();
            this.cid2gpid = new Dictionary<int, int>();

            this.sgplayerside_bycid = new Dictionary<int, int>();

            this.sideclan = new Dictionary<int, int>();

            this.ghost_players = new Dictionary<int, int>();

            //this.dota = new Dictionary<int, IBaseUnit>();

            this.kmfin = { };
            this.kmfinbyside = { };
            this.ptfinbyside = { };
            this.maxptside = -1;
            this.prize = { };
            this.score_km = { };
            this.is_score_km = false;

            this.tmfin = false;

            this.tm_cost = false;
            this.total_tm_cost = null;
            this.tm_out = 0;
            this.map_need_km = { };
            this.enter_map_plys = { };
            this.kprec = { };
            this.kpboard = [];

            this.rounds_conf = { };
            this.round_plys = { };

            this.kick_out_plys = { };

            this.stastic_lvl_cost = null;
        }

        public void _on_player_leave(IBaseUnit ply, bool flush_data)
        {
            IMapUnit pl = ply.get_pack_data();

            pl.x = pl.lx;
            pl.y = pl.ly;
            pl.map_id = pl.lmpid;

            //pl.lmpid = 0;
            pl.llid = 0;
            // 清理dota数据

            //if (this.dota)
            //{
            //    pl.dota = null;
            //    if (pl.cid in this.dota.plys)
            //{
            //        delete this.dota.plys[pl.cid];
            //    }
            //    ply.re_calc_cha_data();
            //}

            if (flush_data)
            {
                ply.flush_db_data(false, false); // write back user data to mem db mgr
            }
        }

        public bool is_map_exist(int mapid)
        {
            return this.maps.ContainsKey(mapid);
        }
        public void broad_cast_msg(int cmd, Variant data)
        {
            //svr.mul_snd_rpc(sidary.data(), sidary.size(), cmd, data);
        }

        public static Level_Info get_lvl_info(int llid)
        {
            Level_Info info = null;
            Level.level_infos.TryGetValue(llid, out info);

            return info;
        }

        public static bool delete_lvl(int llid)
        {
            Level.level_infos.Remove(llid);
            return false;
        }

        public game_err_code _create_group_map(group_map gp_map)
        {
            foreach (var mapconf in lvl_conf.level_map)
            {
                var map = grid_map.create_map(mapconf.id, "sg_map");
                //sys.trace(sys.SLT_SYS, "sg_level instance id ["+llid+"] create map[" + lvl_conf.mapid +"]\n");

                if (map == null)
                {
                    Utility.trace_err("sg_level instance id [" + llid + "create gpmap[" + gp_map.gpid + "] create map[" + mapconf.id + "] failed!\n");
                    return game_err_code.LEVEL_CREATE_MAP_ERROR;
                }

                map.worldsvr = this;
                map.blvlmap = true;
                map.init(mapconf.id);

                gp_map.maps[mapconf.id] = map;

                //    if ("trigger" in mapconf)
                //{
                //        map.init_trigger(mapconf.trigger, sys.time() - this.start_tm);
                //    }

                //    if ("path" in mapconf)
                //{
                //        map.init_path(mapconf.path);
                //    }

                //    if ("immrespawn" in mapconf )
                //{
                //        map.immrespawn = mapconf.immrespawn == 1;
                //    }

                //    if (("ignore_side" in mapconf) )
                //{
                //        map.ignore_side = mapconf.ignore_side == 1;
                //    }
            }

            //sys.trace(sys.SLT_DETAIL, "this.diff_lvl ["+this.diff_lvl+"]\n");

            var diff_fin_check = false;

            var diff_lvl_conf = this.lvl_conf.get_diff_lvl_conf(this.diff_lvl);
            if (diff_lvl_conf != null)
            {
                // 根据diff_lvl等级初始化额外增加的数据,如新增怪物等



                // 地图追加配置
                if (diff_lvl_conf.level_map.Count > 0)
                {
                    foreach (var mapconf in diff_lvl_conf.level_map)
                    {
                        if (!gp_map.maps.ContainsKey(mapconf.id))
                            continue;

                        var map = gp_map.maps[mapconf.id];
                        map.add_init(mapconf, Utility.time() - this.start_tm);
                    }
                }

                // 增加地图怪物等级
                //        if ("mon_lvl_add" in diff_lvl_conf && diff_lvl_conf.mon_lvl_add > 0)
                //{
                //            foreach (map in gp_map.maps)
                //            {
                //                foreach (mon in map.map_mons)
                //                {
                //                    mon.setlevel(mon.mondata.level + diff_lvl_conf.mon_lvl_add);
                //                }
                //            }
                //        }

                if (diff_lvl_conf.fin_check != null)
                {
                    diff_fin_check = true;

                    if (diff_lvl_conf.fin_check.km.Count > 0)
                    {
                        foreach (var km in diff_lvl_conf.fin_check.km)
                        {
                            gp_map.kmfin[km.mid] = km.cnt;
                        }
                    }
                    if (diff_lvl_conf.fin_check.tm > 0)
                    {
                        gp_map.tmfin = true;
                    }
                }
            }

            if (lvl_conf.fin_check != null)
            {
                if (lvl_conf.fin_check.km.Count > 0)
                {
                    foreach (var km in lvl_conf.fin_check.km)
                    {
                        gp_map.kmfin[km.mid] = km.cnt;
                    }
                }
                if (lvl_conf.fin_check.tm > 0)
                {
                    gp_map.tmfin = true;
                }
            }
            return game_err_code.RES_OK;
        }

        public game_err_code add_player(int sid, IBaseUnit player)
        {
            long cur_tm_s = Utility.time();
            IMapUnit pl = player.get_pack_data();

            group_map enter_gp_map = null;

            //::g_debug( "======>> add_player() "  );

            if (this.gpmap_maxply > 0)
            {
                if (cid2gpid.ContainsKey(pl.cid))
                {
                    //先判断 是否已有分配
                    var gp_map = this.gp_maps[this.cid2gpid[pl.cid] - 1];
                    if (gp_map.fin)
                    {//已结束
                        return game_err_code.LEVEL_LVL_IS_FINISH;
                    }
                    if (gp_map.plycnt < this.gpmap_maxply)
                    {
                        enter_gp_map = gp_map;
                    }
                }

                if (enter_gp_map == null)
                {
                    for (int i = this.gp_maps.Count; i > 0; --i)
                    {//优先 后创建的
                        var gp_map = this.gp_maps[i - 1];
                        if (gp_map.plycnt < this.gpmap_maxply)
                        {
                            enter_gp_map = gp_map;
                            break;
                        }
                    }
                }

                if (enter_gp_map == null)
                {
                    enter_gp_map = new group_map(this.gp_maps.Count + 1);
                    var ret_code = this._create_group_map(enter_gp_map);
                    if (ret_code != game_err_code.RES_OK)
                    {
                        return ret_code;
                    }

                    this.gp_maps.push(enter_gp_map);
                    //sys.trace(sys.SLT_DETAIL, " this.gp_maps.len =["+this.gp_maps.len()+"]\n");
                }
            }
            //sys.trace(sys.SLT_DETAIL, "pl.lmpid["+pl.lmpid+"]\n");

            //if(pl.lmpid == 0)
            //{
            //    pl.lx = pl.x;
            //    pl.ly = pl.y;
            //    pl.lmpid = pl.map_id;

            //    pl.map_id = born_map_conf.id;
            //    pl.x = sys.rand(born_map_conf.born[0].x, born_map_conf.born[0].width);
            //    pl.y = sys.rand(born_map_conf.born[0].y, born_map_conf.born[0].height);
            //}

            var pl_clanid = 0;

            //::g_dump("======>>sgplayersbyside:", this.sgplayersbyside );

            if (this.sgplayersbyside.Count > 0)
            {
                // 需要记录助攻
                player.kp_asist_rec = true;

                // 分阵营
                var toj_sideid = 0;
                if (sgplayerside_bycid.ContainsKey(pl.cid))
                {
                    // 已经分配过的用户
                    toj_sideid = this.sgplayerside_bycid[pl.cid];
                }
                else
                {
                    if (this.plyside > 0)
                    {
                        // 玩家进入固定阵营
                        if (!sgplayersbyside.ContainsKey(this.plyside))
                        {
                            Utility.trace_err("lvl ltpid[" + ltpid + "] pvp plyside[" + plyside + "] not in side config!\n");
                            return game_err_code.CONFIG_ERR;
                        }
                        toj_sideid = this.plyside;
                    }
                    //else if (this.cltwar)
                    //{
                    //    // 帮派领地争夺战，守城的一边，其余攻城的一边
                    //    pl_clanid = clanmgr.get_pl_clanid(pl.cid);

                    //    if (pl_clanid == this.cltwar.owner_clanid)
                    //    {
                    //        // 领地拥有者帮派，防守方
                    //        toj_sideid = this.cltwar.defside;
                    //    }
                    //    else
                    //    {
                    //        // 非领地拥有者帮派，进攻方
                    //        toj_sideid = this.cltwar.atkside;
                    //    }
                    //}
                    //else if (this.clcqwar)
                    //{
                    //    // 帮派奴役战，攻打的一边，其余守的一边
                    //    pl_clanid = clanmgr.get_pl_clanid(pl.cid);

                    //    if (pl_clanid == this.clcqwar.atk_clanid)
                    //    {
                    //        // 进攻方帮派，进攻方
                    //        toj_sideid = this.clcqwar.atkside;
                    //    }
                    //    else
                    //    {
                    //        // 非进攻方帮派，防守方
                    //        toj_sideid = this.clcqwar.defside;
                    //    }
                    //}
                    //else if (!ignore_clan && ("clan" in this.lvl_conf))
                    //{
                    //        // 按帮派分阵营
                    //        local clanid = clanmgr.get_pl_clanid(pl.cid);

                    //        local min_plycnt = 65536;
                    //        if (clanid in this.clanside)
                    //    {
                    //            toj_sideid = this.clanside[clanid];
                    //            min_plycnt = this.sgplayersbyside[toj_sideid].len();
                    //        }
                    //    else if (this.clansideid_ary.len() > 0)
                    //        {
                    //            toj_sideid = this.clansideid_ary.pop();
                    //            this.clanside[clanid] < -toj_sideid;
                    //            min_plycnt = this.sgplayersbyside[toj_sideid].len();
                    //        }

                    //        //sys.trace(sys.SLT_DETAIL, "toj_sideid["+toj_sideid+"] min_plycnt["+min_plycnt+"]\n");

                    //        //// 在帮派中，搜索同一帮派队友信息
                    //        //foreach(sideid, plys in this.sgplayersbyside)
                    //        //{
                    //        //    foreach(ply in plys)
                    //        //    {
                    //        //        if("clanid" in ply.pinfo && ply.pinfo.clanid == clanid)
                    //        //        {
                    //        //            toj_sideid = sideid;
                    //        //            break;
                    //        //        }
                    //        //    }

                    //        //    if(toj_sideid != 0)
                    //        //    {
                    //        //        break;
                    //        //    }
                    //        //}

                    //        if (toj_sideid == 0)
                    //        {
                    //            // 按人数多寡分配
                    //            foreach (sideid, plys in this.sgplayersbyside)
                    //        {
                    //                if (plys.len() < min_plycnt)
                    //                {
                    //                    toj_sideid = sideid;
                    //                    min_plycnt = plys.len();
                    //                }
                    //            }
                    //        }

                    //        if (toj_sideid > 0 && min_plycnt <= 0)
                    //        {
                    //            // 没有人的阵营，新分配给加入角色所在帮派

                    //            local clan_mgr = global_data_mgrs.clan;
                    //            clan_mgr.lock_data(clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
                    //            local clan_db_data = _get_clan_data(clanid);
                    //            if (clan_db_data.len() > 0)
                    //            {
                    //                // 目标帮派存在
                    //                local clan_db_info = clan_db_data[0].get_data();

                    //                local sideclan_info = { sideid = toj_sideid, clanlvl = clan_db_info.lvl, clname = clan_db_info.clname };
                    //                this.sideclan[clanid] < -sideclan_info;

                    //                // 通知客户端新加入帮派
                    //                // send lvl_side_info msg
                    //                this.broad_cast_msg(247, { sideclan =[sideclan_info]});
                    //            }
                    //            clan_mgr.unlock_data(clanid); // 解锁
                    //        }
                    //    }
                    //else if (this.camp_side.len() > 0)
                    //{
                    //    // 按世界阵营分阵营
                    //    if (pl.camp in this.camp_side)
                    //{
                    //        toj_sideid = this.camp_side[pl.camp];
                    //    }
                    //else
                    //{
                    //        sys.trace(sys.SLT_ERR, "lvl tpid[" + lvl_conf.tpid + "] add ply cid[" + pl.cid + "] camp[" + pl.camp + "] side not exist!\n");
                    //    }
                    //}
                    //else
                    //{
                    //    if (!ignore_team)
                    //    {
                    //        local tid = team.get_ply_teamid(pl.cid);
                    //        if (tid != 0)
                    //        {
                    //            // 在队伍中，搜索同一队伍队友信息
                    //            foreach (sideid, plys in this.sgplayersbyside)
                    //        {
                    //                foreach (ply in plys)
                    //                {
                    //                    if ("teamid" in ply.pinfo && ply.pinfo.teamid == tid)
                    //                {
                    //                        toj_sideid = sideid;
                    //                        break;
                    //                    }
                    //                }

                    //                if (toj_sideid != 0)
                    //                {
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }

                    //    if (toj_sideid == 0)
                    //    {
                    //        // 按人数多寡分配
                    //        local min_plycnt = 65536;
                    //        foreach (sideid, plys in this.sgplayersbyside)
                    //    {
                    //            if (plys.len() < min_plycnt)
                    //            {
                    //                toj_sideid = sideid;
                    //                min_plycnt = plys.len();
                    //            }
                    //        }
                    //    }
                    //}
                }

                if (toj_sideid == 0)
                {
                    Utility.trace_err("sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] without side!\n");

                    return game_err_code.CONFIG_ERR;
                }

                this.sgplayerside_bycid[pl.cid] = toj_sideid;

                var toj_side_plys = this.sgplayersbyside[toj_sideid];
                toj_side_plys[pl.cid] = player;
                pl.lvlsideid = toj_sideid;

                var side_conf = this.sides_conf[toj_sideid];

                var born = side_conf.born;

                pl.map_id = born.mpid;

                pl.x = Utility.random_section(born.x, born.w);
                pl.y = Utility.random_section(born.y, born.h);

                // 发送自己阵营消息
                //::send_rpc(pl.sid, 247, { lvlsideid = pl.lvlsideid});
            }
            else
            {
                //::g_dump("======>>地图重生点:", born_map_conf );
                //::g_dump("======>>pinfo:", pl );

                pl.map_id = born_map_conf.mpid;
                pl.x = Utility.random_section(born_map_conf.x, born_map_conf.w);
                pl.y = Utility.random_section(born_map_conf.y, born_map_conf.h);

            }

            //sys.trace(sys.SLT_DETAIL, "pl.lmpid["+pl.lmpid+"]\n");

            // 判断角色ghost状态
            if (ghost_cnt > 0)
            {
                if (!this.ghost_players.ContainsKey(pl.cid))
                {
                    this.ghost_players[pl.cid] = ghost_cnt;
                }
                else
                {
                    if (this.ghost_players[pl.cid] <= 0)
                    {
                        pl.ghost = true;
                    }
                }
            }

            if (this.cur_round > 0 && this.cur_round != 1)
            {
                // 错过第一回合，进入即成为ghost
                this.ghost_players[pl.cid] = 0;
                pl.ghost = true;
            }

            if (this.rounds_conf.Count > 0)
            {
                if (!this.round_plys.ContainsKey(pl.cid))
                {
                    // 首次进入玩家，从第一回合开始
                    this.round_plys[pl.cid] = new round_player() { cid = pl.cid, rnd = 1 };
                }
            }

            //if (this.kumiteply)
            //{
            //    local found = false;
            //    foreach (kpl in this.kumiteply)
            //    {
            //        if (kpl.cid == pl.cid)
            //        {
            //            found = true;
            //            break;
            //        }
            //    }

            //    if (!found)
            //    {
            //        // 新加入角色，添加到车轮战队列中
            //        local kpl = { cid = pl.cid, name = pl.name, lvl = pl.level, stat = lvl_kumite_state.LKS_WAIT, pt = 0, combpt = pl.combpt };
            //        this.kumiteply.push(kpl);

            //        // 广播车轮战队列变化消息
            //        // send lvl_side_info msg
            //        this.broad_cast_msg(247, { kumiteply =[kpl]});

            //        // 车轮战除当前战斗角色之外所有角色都是ghost
            //        this.ghost_players[pl.cid] < -0;
            //    }

            //    if (this.ghost_players[pl.cid] <= 0)
            //    {
            //        player.pinfo.ghost = true;
            //    }
            //}

            // 判断dota地图
            //if (this.dota)
            //{
            //    this.dota.plys[player.pinfo.cid] < -player;
            //    player.pinfo.dota = { data ={ hero_tp = (player.pinfo.carr),level = 1,exp = 0,gld = 0}, conf = this.dota.conf};
            //    player.re_calc_cha_data();
            //}

            //// 计算副本中帮派科技属性加成数据
            //if (this.clanmon_aff)
            //{
            //    if (pl_clanid > 0)
            //    {
            //        if (!(pl_clanid in this.clanply_att_add))
            //    {
            //            _calc_clantechply_att(pl_clanid);
            //        }

            //        player.pinfo.clanatt_eff = true;
            //        player.pinfo.clanatts = this.clanply_att_add[pl_clanid];

            //        player.re_calc_cha_data();
            //    }
            //}

            player.ignore_team = this.ignore_team;
            player.ignore_clan = this.ignore_clan;

            if (enter_gp_map != null)
            {
                if (!enter_gp_map.maps.ContainsKey(pl.map_id))
                {
                    Utility.trace_err("sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] without map[" + pl.map_id + "]!\n");

                    return game_err_code.LEVEL_MAP_NOT_IN_LEVEL;
                }
                enter_gp_map.maps[pl.map_id].add_player_to_map(this, sid, player);
                enter_gp_map.plys[pl.cid] = player;
                enter_gp_map.plycnt++;
                this.cid2gpid[pl.cid] = enter_gp_map.gpid;
                //sys.trace(sys.SLT_ERR, "enter_gp_map gpid ["+enter_gp_map.gpid+"] pl.cid[" + pl.cid +"] enter_gp_map.plycnt =["+enter_gp_map.plycnt+"]!\n");
            }
            else
            {
                if (!this.maps.ContainsKey(pl.map_id))
                {
                    Utility.trace_err("sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] without map[" + pl.map_id + "]!\n");

                    return game_err_code.LEVEL_MAP_NOT_IN_LEVEL;
                }
                this.maps[pl.map_id].add_player_to_map(this, sid, player);
            }

            sgplayers[sid] = player;
            sgplayersbycid[pl.cid] = player;

            if (!this.sid_ary.Contains(sid))
            {
                this.sid_ary.Add(sid);
            }

            player.in_lvl = this;

            //if (this.tm_cost)
            //{
            //    local lvlentried = player.get_entried_lvl(lvl_conf.tpid);
            //    local diff_lvl = 1;
            //    if (lvlentried) diff_lvl = lvlentried.diff_lvl;
            //    local info = {
            //    stm = cur_tm_s,
            //    last_calc_tm = cur_tm_s,
            //    yb = 0,
            //    gold = 0,
            //    batpt = 0,
            //    diff_lvl = diff_lvl,
            //};
            //    this.total_tm_cost[pl.cid] < -info;
            //}

            if (this.tm_out > 0)
            {
                var tmpinfo = new JoinInfo() { cid = pl.cid, sid = sid, mapid = pl.map_id, join_tm = cur_tm_s };
                this.joininfo[pl.cid] = tmpinfo;
            }
            // 判断角色复活时间
            //if (this.plyrespawn_tm > 0)
            //{
            //    if (pl.cid in this.plyrespawn_tms)
            //{
            //        local respawn_tm = this.plyrespawn_tms[pl.cid] - cur_tm_s;
            //        if (respawn_tm > 0)
            //        {
            //            // 角色已死亡，尚不能复活
            //            player.allow_respawn_tm_s = respawn_tm;

            //            // 角色死亡
            //            pl.hp = 0;
            //            player.respawn_tm = sys.clock_time() + 30000; // 30 秒后安全复活
            //            pl.isdie = true;

            //            // broad cast die msg
            //            player.broad_cast_zone_msg_and_self(25, { iid = pl.iid});

            //        // send self_attchange msg
            //        ::send_rpc(pl.sid, 32, { respawn_tms = respawn_tm});
            //        }
            //        else
            //        {
            //            delete this.plyrespawn_tms[pl.cid];
            //        }
            //    }
            //}

            //    if ("enter_lvl" in this.lvl_conf )
            //{
            //        local enter_lvl_conf = this.lvl_conf.enter_lvl[0];
            //        if ("addmis" in enter_lvl_conf )
            //    {
            //            foreach (conf in enter_lvl_conf.addmis)
            //            {
            //                _acept_mis(pl.sid, conf, player, false, false);
            //            }
            //        }
            //        if ("addstate" in enter_lvl_conf )
            //    {
            //            local cur_clock_tm = sys.clock_time();
            //            local add_states = [];
            //            foreach (stateconf in enter_lvl_conf.addstate)
            //            {
            //                if ("attchk" in stateconf )
            //            {
            //                    local matched = true;
            //                    foreach (attchk in stateconf.attchk)
            //                    {
            //                        if (game_err_code.RES_OK != player.check_att(attchk))
            //                        {
            //                            matched = false;
            //                            break;
            //                        }
            //                    }
            //                    if (!matched) continue;
            //                }
            //                local can_add = true;
            //                if ("side" in stateconf)
            //            {   //指定哪边可添加buff
            //                    can_add = false;
            //                    foreach (sideid in stateconf.side)
            //                    {
            //                        if (player.pinfo.lvlsideid == sideid)
            //                        {
            //                            can_add = true;
            //                            break;
            //                        }
            //                    }
            //                }
            //                if (!can_add) continue;

            //                local state_obj = add_state_to_pl(cur_clock_tm, player, stateconf.state[0], null, 1000, false);
            //                if (state_obj)
            //                {
            //                    add_states.push(state_obj);
            //                }
            //            }

            //            if (add_states.len() > 0)
            //            {
            //                _remark_pl_state(player, player.pinfo);
            //                player.broad_cast_zone_msg_and_self(24, { iid = player.pinfo.iid, states = add_states});
            //            }
            //        }
            //    }

            // 统计数据
            //stastic.add_lvl_ply(llid, sid);

            return game_err_code.RES_OK;
        }


        public void remove_player(int sid, bool callfin = true)
        {
            //sys.trace(sys.SLT_DETAIL, "level ["+ltpid+"] remove_player sid["+sid+"]\n");
            //::g_debug( "======>> remove_player()" );

            IBaseUnit ply = null;
            if (!this.sgplayers.TryGetValue(sid, out ply))
                return;


            IMapUnit pl = ply.get_pack_data();
            //在副本一定时间就踢出
            if (this.tm_out > 0)
            {
                if (this.joininfo.ContainsKey(pl.cid))
                    this.joininfo.Remove(pl.cid);
            }
            foreach (var enter_plys in enter_map_plys.Values)
            {
                if (enter_plys.ContainsKey(pl.cid))
                {
                    enter_plys.Remove(pl.cid);
                }
            }


            if (ply.isdie())
            {
                ply.respawn(100, true);
            }

            if (this.gpmap_maxply > 0)
            {
                if (this.cid2gpid.ContainsKey(pl.cid))
                {
                    var gp_map = this.gp_maps[this.cid2gpid[pl.cid] - 1];
                    gp_map.maps[pl.map_id].rmv_player_from_map(this, sid);
                    gp_map.plys.Remove(pl.cid);
                    gp_map.plycnt--;
                }

            }
            else
            {
                if (this.maps.ContainsKey(pl.map_id))
                {
                    this.maps[pl.map_id].rmv_player_from_map(this, sid);
                }
            }

            this.sgplayersbycid.Remove(pl.cid);
            this.sgplayers.Remove(sid);

            if (pl.lvlsideid > 0)
            {
                // 在阵营中的用户
                this.sgplayersbyside[pl.lvlsideid].Remove(pl.cid);
            }

            this.sid_ary.Remove(sid);

            _on_player_leave(ply, true);

            ply.in_lvl = null;


            if (this.state != level_state_type.LST_FINED)
            {
                //                ply.pinfo.arena_cwin = 0;//????

                if (lvl_conf.exitfin > 0 && sgplayers.Count <= 0)
                {
                    this.fin();
                }
            }
            else
            {
                if (callfin && this.sgplayers.Count <= 0)
                {
                    this.fin();
                }
            }
        }


        public void fin(bool clear_service_rec = true)
        {
            lvl_fined = true;

            List<IBaseUnit> to_remove_ply = sgplayers.Values.ToList();


            foreach (var ply in to_remove_ply)
            {
                IMapUnit pl = ply.get_pack_data();
                this.remove_player(pl.sid, false);
                Level.enter_world(pl.sid, pl.line);
            }

            // fin maps
            if (this.gpmap_maxply > 0)
            {
                foreach (var gp_map in this.gp_maps)
                {
                    if (gp_map.closed) continue;

                    gp_map.closed = true;
                    foreach (var val in gp_map.maps.Values)
                    {
                        val.fin();
                    }
                    //sys.trace( sys.SLT_ERR, "gp_map.closed =["+gp_map.closed+"]\n" );              
                }
            }
            else
            {
                foreach (var val in maps.Values)
                {
                    val.fin();
                }
            }


            if (this.sid_ary != null)
            {
                this.sid_ary.Clear();
            }

            if (lvl_conf.lctp == (int)level_checkin_type.LCT_AUTO_MATCH)
            {
                // 匹配方式，去除匹配竞技场标记
                //sys.trace(sys.SLT_DETAIL, "rmv_arena_pl_mark");

                //local cid_ary = level.get_ply_cids(llid);
                //level.lock_aline();
                //foreach (cid in cid_ary)
                //{
                //    level.rmv_arena_pl_mark(cid);
                //}
                //level.unlock_aline();
            }

            if (this.lvl_conf.lmtp == (int)level_multi_type.LMT_TEAM)
            {
                //// 若是组队副本，清除不能加入设置
                //team.set_joinable(this.creator, true);

                //// 通知队伍副本结束
                //// send close_lvl_res msg
                //team_bcast_rpc(this.creator, 248, { llid = llid});
            }

            //if (this.clter_conf && this.clter_conf.tp == clan_teritory_type.CTT_WAR_TERRITORY)
            //{
            //    // 需争夺的帮派领地副本,取消帮派领地副本关联

            //    sys.trace(sys.SLT_DETAIL, "clter id[" + this.clter_conf.id + "] owner_clanid[" + this.creator + "] lvl close, clear mark\n");

            //    level.clear_clan_terit(this.clter_conf.id, this.creator);
            //}

            //sys.trace(sys.SLT_DETAIL, "level ["+ltpid+"] fin\n");

            //    if ((llid in service.lvls) && clear_service_rec)
            //{
            //        delete service.lvls[llid];
            //    }

            Level.delete_lvl(llid);

            // 统计数据
            //stastic.rmv_lvl(llid);
        }




        public game_err_code init(int llid)
        {
            //::g_debug( "====>>> on sg_level init(), llid:" + llid );

            //this.service = s;

            Level_Info linfo = Level.get_lvl_info(llid);

            //::g_dump( "====>>> 副本信息:", linfo );

            if (linfo == null)
            {
                this.fin();
                Utility.trace_err("lvl init err on linfo is null！");
                return game_err_code.INTERNAL_ERR;
            }

            ltpid = linfo.ltpid;
            this.diff_lvl = linfo.diff_lvl;
            this.creator = linfo.creator;

            this.lvl_conf = Utility.get_level_conf(ltpid);
            if (lvl_conf == null)
            {
                this.fin();
                Utility.trace_err("lvl init err on lvl_conf is null！");
                return game_err_code.CONFIG_ERR;
            }

            //this.is_score_km = (_get_score_conf("score_awd_km") != null);

            var diff_lvl_conf = lvl_conf.get_diff_lvl_conf(linfo.diff_lvl);
            if (null == diff_lvl_conf)
            {
                this.fin();
                Utility.trace_err("lvl init err on lvl_conf.diff_lvl is null！");
                return game_err_code.CONFIG_ERR;
            }


            long cur_tm_s = DateTime.Now.ToBinary();

            this.start_tm = cur_tm_s;
            if (this.lvl_conf.lctp == (int)level_checkin_type.LCT_AUTO_CREATE)
            {
                //    if ("tmchk" in this.lvl_conf )
                //{//计算准确开服时间
                //        local today_near_opntm = get_curr_open_tm(cur_tm_s, this.lvl_conf.tmchk);
                //        if (today_near_opntm != 0)
                //        {
                //            this.start_tm = today_near_opntm;
                //        }
                //    }
            }

            this.end_tm = this.start_tm + lvl_conf.tm * 60;

            if (diff_lvl_conf.born != null)
            {
                born_map_conf = diff_lvl_conf.born;//只有 idx=0 的配置生效
            }

            //::g_debug( "====>>副本 ltpid:" + ltpid );
            //::g_debug( "====>>副本 diff_lvl:" +  linfo.diff_lvl );
            //::g_dump( "====>>副本 diff_lvl_conf:",  diff_lvl_conf );
            //::g_dump( "====>>副本 born_map_conf 初始化出生点:",  born_map_conf );

            foreach (var mapconf in lvl_conf.level_map)
            {
                grid_map map = grid_map.create_map(mapconf.id, "sg_map");
                //sys.trace(sys.SLT_SYS, "sg_level instance id ["+llid+"] create map[" + lvl_conf.mapid +"]\n");
                if (map == null)
                {
                    Utility.trace_err("sg_level instance id [" + llid + "] create map[" + mapconf.id + "] failed!\n");

                    this.fin();
                    return game_err_code.LEVEL_CREATE_MAP_ERROR;
                }

                map.worldsvr = this;
                map.blvlmap = true;
                map.init(mapconf.id);

                this.maps[mapconf.id] = map;

                if (born_map_conf == null && mapconf.born != null)
                {
                    var binfo = mapconf.born;
                    born_map_conf = binfo;
                    //::g_dump( "====>>副本 默认 born:", born_map_conf );
                }


                //    if ("trigger" in mapconf)
                //{
                //        //sys.trace( sys.SLT_DETAIL, "cur_tm_s-this.start_tm=" + cur_tm_s-this.start_tm + "\n");
                //        map.init_trigger(mapconf.trigger, cur_tm_s - this.start_tm);
                //    }

                //    if ("path" in mapconf)
                //{
                //        map.init_path(mapconf.path);
                //    }

                //    if ("skill" in mapconf)
                //{
                //        map.add_map_skills(mapconf.skill);
                //    }

                //    if ("immrespawn" in mapconf )
                //{
                //        map.immrespawn = mapconf.immrespawn == 1;
                //    }

                //    if (("ignore_side" in mapconf) )
                //{
                //        map.ignore_side = mapconf.ignore_side == 1;
                //    }
                //}

                //sys.trace(sys.SLT_DETAIL, "this.diff_lvl ["+this.diff_lvl+"]\n");
            }
            bool diff_fin_check = false;

            if (diff_lvl_conf != null)
            {
                // 根据diff_lvl等级初始化额外增加的数据,如新增怪物等


                if (diff_lvl_conf.level_map.Count > 0)
                {
                    // 地图追加配置
                    foreach (var map in diff_lvl_conf.level_map)
                    {
                        if (this.maps.ContainsKey(map.id))
                            continue;

                        this.maps[map.id].add_init(map, Utility.time() - this.start_tm);
                    }
                }

                // 增加地图怪物等级
                //        if ("mon_lvl_add" in diff_lvl_conf && diff_lvl_conf.mon_lvl_add > 0)
                //{
                //            foreach (map in this.maps)
                //            {
                //                foreach (mon in map.map_mons)
                //                {
                //                    mon.setlevel(mon.mondata.level + diff_lvl_conf.mon_lvl_add);
                //                }
                //            }
                //        }

                if (diff_lvl_conf.fin_check != null)
                {
                    diff_fin_check = true;

                    if (diff_lvl_conf.fin_check.km.Count > 0)
                    {
                        foreach (var km in diff_lvl_conf.fin_check.km)
                        {
                            this.kmfin[km.mid] = km.cnt;
                        }
                    }

                    if (diff_lvl_conf.fin_check.tm > 0)
                        this.tmfin = true;

                }
            }

            if (born_map_conf == null)
            {
                Utility.trace_err("sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] without born map!\n");

                this.fin();
                return game_err_code.LEVEL_CREATE_NO_BORN_MAP;
            }

            if (!diff_fin_check && lvl_conf.fin_check != null)
            {
                if (lvl_conf.fin_check.km.Count > 0)
                {
                    foreach (var km in lvl_conf.fin_check.km)
                    {
                        this.kmfin[km.mid] = km.cnt;
                    }
                }
                if (lvl_conf.fin_check.tm > 0)
                    this.tmfin = true;
            }

            if (lvl_conf.gpmap_maxply > 0)
            {
                if (this.lvl_conf.gpmap_maxply < this.lvl_conf.maxply)
                {
                    this.gpmap_maxply = this.lvl_conf.gpmap_maxply;
                    this.gp_maps = new List<SceneTest.group_map>();
                    this.cid2gpid = new Dictionary<int, int>();

                    group_map group_map = new group_map()
                    {
                        gpid = 1,
                        maps = this.maps,
                        kmfin = this.kmfin,
                        tmfin = this.tmfin
                    };

                    gp_maps.Add(group_map);
                }
            }

            this.llid = llid;

            //        if (("no_kp_hexp" in this.lvl_conf) && (this.lvl_conf.no_kp_hexp == 1))
            //{
            //            no_kp_hexp = true;
            //        }

            var pvp = lvl_conf.pvp;
            if (pvp != null)
            {
                if (pvp.ignore_clan > 0)
                    this.ignore_clan = true;

                if (pvp.ignore_team > 0)
                    this.ignore_team = true;

                if (pvp.plyside > 0)
                    this.plyside = pvp.plyside;

                if (pvp.round != null)
                {
                    // 有多个回合的pvp副本，初始化回合
                    foreach (var round in pvp.round)
                    {
                        this.rounds_conf[round.id] = round;
                    }

                    this.cur_round = 1;
                    if (!this.rounds_conf.ContainsKey(this.cur_round))
                    {
                        Utility.trace_err("sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] pvp round without round 1!\n");
                        return game_err_code.CONFIG_ERR;
                    }

                    var res = _init_pvp(this.rounds_conf[this.cur_round], cur_tm_s); // 初始化第一回合配置
                    if (res != game_err_code.RES_OK)
                    {
                        return res;
                    }

                    this.round_tm = cur_tm_s + (this.rounds_conf[this.cur_round].tm * 60);
                }
                else
                {
                    var res = _init_pvp(pvp, cur_tm_s);
                    if (res != game_err_code.RES_OK)
                    {
                        return res;
                    }
                }
            }

            //        if ("dota" in lvl_conf)
            //{
            //            // 增加dota类的战场配置解析功能，初始化dota类战场数据
            //            local dota_conf = get_dota_conf(lvl_conf.dota);
            //            if (!dota_conf)
            //            {
            //                Utility.trace_err("invalid dota_conf[" + lvl_conf.dota + "]\n");
            //            }
            //            else
            //            {
            //                this.dota = { plys ={ }, conf = dota_conf};
            //            }
            //        }

            //if (this.clanmon_aff)
            //{
            //    // 帮派怪物加成
            //    foreach (map in this.maps)
            //    {
            //        foreach (mon in map.map_mons)
            //        {
            //            if (mon.mondata.lvlsideid != this.clanmon_att_add.sideid)
            //            {
            //                continue;
            //            }

            //            if (!("clanmon" in mon.monconf))
            //    {
            //                continue;
            //            }

            //            if (!(mon.monconf.clanmon in this.clanmon_att_add.atts))
            //    {
            //                continue;
            //            }

            //            mon.setclanatts(this.clanmon_att_add.atts[mon.monconf.clanmon]);
            //        }
            //    }
            //}

            if (lvl_conf.rmv_leave_ply == 1)
                this.rmv_leave_ply = true;

            //        if ("cltid" in lvl_conf)
            //{
            //            this.clter_conf = get_clantrit_conf(lvl_conf.cltid);
            //        }
            if (lvl_conf.plyrespawn_tm > 0)
            {
                this.plyrespawn_tm = lvl_conf.plyrespawn_tm;
                //this.plyrespawn_tms = { };
            }

            if (lvl_conf.immrespawn == 1)
            {
                this.immrespawn = true;
            }

            //        if ("team_exdrop" in lvl_conf)
            //{
            //            this.team_exdrop = lvl_conf.team_exdrop;
            //        }

            //        if ("tm_cost" in lvl_conf )
            //{
            //            this.tm_cost = true;
            //            total_tm_cost = { };
            //        }

            if (lvl_conf.tm_out > 0)
            {
                this.tm_out = lvl_conf.tm_out;
                this.joininfo = new Dictionary<int, JoinInfo>();
            }

            var diffs = lvl_conf.diff_lvl;

            //foreach (var dx in lvl_conf.diff_lvl)
            //{
            //    local map_km = { };
            //    foreach (map in diff_lvl.map)
            //    {
            //        if ("dir_enter" in map)
            //    {
            //            local dir_enter = map.dir_enter[0];
            //            if ("km" in dir_enter)
            //        {
            //                local km = dir_enter.km[0];
            //                map_km[map.id] < - { mid = km.mid, mapid = km.mapid};
            //            }
            //        }
            //    }
            //    map_need_km[diff_lvl.lv] < -map_km;
            //}

            //        if ("trig_finchk" in lvl_conf )
            //{
            //            this.active_finchk = false;
            //        }

            this.state = level_state_type.LST_PROCEED;

            Level.set_lvl_end_tm(llid, end_tm);

            this.sid_ary = new HashSet<int>();

            return game_err_code.RES_OK;
        }

        public game_err_code _init_pvp(pvp_conf pvp_conf, long cur_mt_s)
        {
            round_conf r_conf = new round_conf()
            {
                death_match = pvp_conf.death_match,
                death_pt = pvp_conf.death_pt,
                side = pvp_conf.side,
            };

            return _init_pvp(r_conf, cur_mt_s);
        }

        public game_err_code _init_pvp(round_conf pvp_conf, long cur_tm_s)
        {
            ghost_cnt = 0;
            death_match = false;
            death_hold_map = 0;
            preptm = 0;
            rnk_batl = false;

            //this.sgplayersbyside = {};
            //this.sgplayerside_bycid = {};
            //this.sides_conf = null;

            this.clansideid_ary = [];
            this.kmfin = new Dictionary<int, int>();
            this.kmfinbyside = {
            };
            this.ptfinbyside = {
            };
            this.maxptside = -1;
            this.death_pts = null;
            this.death_ptconf = null;
            this.kumiteply = null;
            this.kumite_conf = null;
            this.kumitefight_ply = null;
            this.kumite_waittm = 0;
            this.cltwar = null;
            this.clcqwar = null;

            this.kprec = {
            };
            this.kpboard = [];

            this.camp_side = {
            };

            var linfo = Level.get_lvl_info(llid);
            if (linfo == null)
            {
                this.fin();
                return game_err_code.INTERNAL_ERR;
            }

            if (pvp_conf.side.Count > 0)
            {
                // 分阵营pvp副本

                // 初始化阵营
                foreach (var sideconf in pvp_conf.side)
                {
                    if (sideconf.born == null)
                    {
                        Utility.trace_err("sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] side [" + sideconf.id + "] without born!\n");

                        this.fin();
                        return game_err_code.CONFIG_ERR;
                    }

                    if (!this.sgplayersbyside.ContainsKey(sideconf.id))
                        this.sgplayersbyside[sideconf.id] = new Dictionary<int, IBaseUnit>();

                    //    if ("fin_check" in sideconf)
                    //{
                    //        if ("km" in sideconf.fin_check[0])
                    //    {
                    //            local kmside = { };
                    //            foreach (km in sideconf.fin_check[0].km)
                    //            {
                    //                kmside[km.mid] < - { cntleft = km.cnt};
                    //            }
                    //            this.kmfinbyside[sideconf.id] < -kmside;
                    //        }
                    //    else if ("pt" in sideconf.fin_check[0])
                    //    {
                    //            this.ptfinbyside[sideconf.id] < - { pt = 0, maxpt = sideconf.fin_check[0].pt};
                    //        }
                    //    }

                    //    if ("clan" in this.lvl_conf)
                    //{
                    //        this.clansideid_ary.push(sideconf.id);
                    //    }

                    //    if ("camp" in sideconf)
                    //{
                    //        this.camp_side[sideconf.camp] < -sideconf.id; // 按世界阵营分边
                    //    }
                    //}
                    this.sides_conf = pvp_conf.side;
                }
            }
            else if (pvp_conf.death_pt != null)
            {
                // 混战积分模式pvp副本
                this.death_pts = new Dictionary<int, Death_Pt>();
                this.death_ptconf = pvp_conf.death_pt;
                this.preptm = cur_tm_s + this.death_ptconf.preptm;
            }
            else if (pvp_conf.death_match != null)
            {
                // 死亡竞赛模式pvp副本
                var death_match_conf = pvp_conf.death_match;

                this.ghost_cnt = death_match_conf.die2ghost;
                this.death_match = true;
                this.preptm = cur_tm_s + death_match_conf.preptm;

                //foreach(cid, ply in this.sgplayersbycid)
                //{
                //    this.ghost_players[cid] <- ghost_cnt;
                //}

                if (death_match_conf.side.Count > 0)
                {
                    foreach (var sideconf in death_match_conf.side)
                    {
                        if (sideconf.born == null)
                        {
                            Utility.trace_err("sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] death_match side [" + sideconf.id + "] without born!\n");

                            this.fin();
                            return game_err_code.CONFIG_ERR;
                        }

                        if (!this.sgplayersbyside.ContainsKey(sideconf.id))

                            this.sgplayersbyside[sideconf.id] = new Dictionary<int, IBaseUnit>();
                    }

                    this.sides_conf = death_match_conf.side;
                }
            }
            //    else if ("death_hold_map" in pvp_conf )
            //{
            //        // 死亡占地图模式pvp副本
            //        local death_hold_map_conf = pvp_conf.death_hold_map[0];

            //        this.death_hold_map = death_hold_map_conf.mapid;
            //        this.preptm = cur_tm_s + death_hold_map_conf.preptm;

            //        if (("side" in death_hold_map_conf) && death_hold_map_conf.side.len() > 0)
            //    {
            //            foreach (sideconf in death_hold_map_conf.side)
            //            {
            //                if (!("born" in sideconf))
            //            {
            //                    sys.trace(sys.SLT_ERR, "sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] death_match side [" + sideconf.id + "] without born!\n");

            //                    this.fin();
            //                    return game_err_code.CONFIG_ERR;
            //                }

            //                if (!(sideconf.id in this.sgplayersbyside))
            //            {
            //                    this.sgplayersbyside[sideconf.id] < - { };
            //                }
            //            }

            //            this.sides_conf = death_hold_map_conf.side;
            //        }
            //    }
            //    else if ("kumite" in pvp_conf)
            //{
            //        // 车轮战模式
            //        kumite_conf = pvp_conf.kumite[0];
            //        kumiteply = [];
            //        kumitefight_ply = { };
            //        this.kumite_waittm = cur_tm_s + kumite_conf.waittm;
            //        this.preptm = this.end_tm; // 人数未满最低开始人数前，一直是准备时间
            //    }
            //else if ("cltwar" in pvp_conf)
            //{
            //        // 帮派领地争夺战
            //        local cltwar_conf = pvp_conf.cltwar[0];

            //        if (!("cltwarid" in lvl_conf))
            //    {
            //            sys.trace(sys.SLT_ERR, "sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] cltwar level without cltwarid!\n");

            //            this.fin();
            //            return game_err_code.CONFIG_ERR;
            //        }

            //        local clantrit_conf = get_clantrit_conf(lvl_conf.cltwarid);
            //        if (!clantrit_conf)
            //        {
            //            sys.trace(sys.SLT_ERR, "sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] cltwarid[" + lvl_conf.cltwarid + "] config not exist!\n");

            //            this.fin();
            //            return game_err_code.CONFIG_ERR;
            //        }

            //        foreach (sideconf in cltwar_conf.side)
            //        {
            //            if (!("born" in sideconf))
            //        {
            //                sys.trace(sys.SLT_ERR, "sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] cltwar_conf side [" + sideconf.id + "] without born!\n");

            //                this.fin();
            //                return game_err_code.CONFIG_ERR;
            //            }

            //            if (!(sideconf.id in this.sgplayersbyside))
            //        {
            //                this.sgplayersbyside[sideconf.id] < - { };
            //            }
            //        }

            //        this.sides_conf = cltwar_conf.side;
            //        this.preptm = cur_tm_s + cltwar_conf.preptm;

            //        this.cltwar = {
            //            atkside = cltwar_conf.atkside, 
            //         defside = cltwar_conf.defside, ptmon = cltwar_conf.ptmon, ptper = cltwar_conf.ptper,
            //        clanpts ={ }, plypts ={ }, max_clanid = 0, max_clanpt = 0, owner_clanid = 0 };


            //        if ("tar_mid" in cltwar_conf )
            //    {
            //            this.cltwar.kmfin < - { tar_mid = cltwar_conf.tar_mid, cntleft = cltwar_conf.tar_mon_cnt};
            //        }
            //    else if ("hold_map" in cltwar_conf )
            //    {
            //            if (!is_map_exist(cltwar_conf.hold_map))
            //            {
            //                sys.trace(sys.SLT_ERR, "sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] cltwar_conf hold_map [" + cltwar_conf.hold_map + "], map not exist!\n");
            //                this.fin();

            //                return game_err_code.CONFIG_ERR;
            //            }
            //            this.cltwar.hold_map < - { mapid = cltwar_conf.hold_map, hold_clan = 0 };
            //        }

            //        //统计消耗
            //        if ("stastic_cost" in clantrit_conf )
            //    {
            //            local stastic_cost_conf = clantrit_conf.stastic_cost[0];
            //            _init_stastic_lvl_cost(stastic_cost_conf.init_yb, stastic_cost_conf.init_gld);
            //        }

            //        local mon_hp_per_data = null;
            //        local glbdata_mgr = global_data_mgrs.glbdata;
            //        glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CLAN_TER, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
            //        local gld_clanter_data = _get_clanter_data();
            //        if (gld_clanter_data)
            //        {
            //            local glb_clanter_info = gld_clanter_data.get_data();
            //            if (lvl_conf.cltwarid in glb_clanter_info.data)
            //        {
            //                local clanter_info = glb_clanter_info.data[lvl_conf.cltwarid];
            //                this.cltwar.owner_clanid = clanter_info.clanid;

            //                clanter_info.onwar = true; // 标记该帮派领地开始争夺
            //                                           //消除 消耗
            //                if ("stastic_cost" in clanter_info )
            //            {
            //                    delete clanter_info.stastic_cost;
            //                }
            //                if ("mon_hp_per" in clanter_info )
            //            {
            //                    mon_hp_per_data = clanter_info.mon_hp_per;
            //                    delete clanter_info.mon_hp_per;
            //                }
            //            }
            //        else
            //        {
            //                glb_clanter_info.data[lvl_conf.cltwarid] < - { crttm = cur_tm_s, showinfo =[], clanid = 0, onwar = true };
            //            }
            //            gld_clanter_data.mod_data(glb_clanter_info); // 同步至全局管理器
            //        }
            //        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁

            //        if (this.cltwar.owner_clanid > 0)
            //        {// 有占领帮派                
            //            if ("hold_map" in this.cltwar )
            //        {
            //                this.cltwar.hold_map.hold_clan = this.cltwar.owner_clanid;
            //            }
            //            // 需要计算占领帮派怪物属性加成
            //            _calc_clantechmon_att(this.cltwar.owner_clanid, this.cltwar.defside);

            //            //调整 初始化怪的血量
            //            if (mon_hp_per_data && ("mon_hp_per" in clantrit_conf) )
            //        {
            //                foreach (mon_hp_per in mon_hp_per_data)
            //                {
            //                    if (mon_hp_per.mapid in this.maps )
            //                {
            //                        local map_mon_bymid = this.maps[mon_hp_per.mapid].map_mon_bymid;
            //                        //sys.dumpobj( map_mon_bymid );
            //                        foreach (adjust_hp in mon_hp_per.hp_pers)
            //                        {
            //                            if (adjust_hp.mid in map_mon_bymid )
            //                        {
            //                                local mons = map_mon_bymid[adjust_hp.mid];
            //                                if (mons.len() > 0)
            //                                {//每种怪 只能有一个
            //                                    mons[0].adjust_hp_per(adjust_hp.hp_per);
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //else if ("clcqwar" in pvp_conf)
            //{
            //        // 帮派奴役战
            //        local clcqwar_conf = pvp_conf.clcqwar[0];

            //        local clanch_info = _get_clanch_info_byid(this.creator);
            //        if (!clanch_info)
            //        {
            //            this.fin();
            //            return game_err_code.LEVEL_NO_CLANCHL_REC;
            //        }

            //        foreach (sideconf in clcqwar_conf.side)
            //        {
            //            if (!("born" in sideconf))
            //        {
            //                sys.trace(sys.SLT_ERR, "sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] clcqwar_conf side [" + sideconf.id + "] without born!\n");

            //                this.fin();
            //                return game_err_code.CONFIG_ERR;
            //            }

            //            if (!(sideconf.id in this.sgplayersbyside))
            //        {
            //                this.sgplayersbyside[sideconf.id] < - { };
            //            }
            //        }

            //        this.sides_conf = clcqwar_conf.side;
            //        this.preptm = cur_tm_s + clcqwar_conf.preptm;

            //        this.clcqwar = {
            //            tar_mid = clcqwar_conf.tar_mid, cntleft = clcqwar_conf.tar_mon_cnt, atkside = clcqwar_conf.atkside, 
            //        defside = clcqwar_conf.defside, atk_clanid = clanch_info.fclid, def_clanid = clanch_info.tclid};

            //        // 获取防守帮派怪物属性加成
            //        _calc_clantechmon_att(this.clcqwar.def_clanid, this.clcqwar.defside);
            //    }
            else
            {
                Utility.trace_err("sg_level instance id [" + llid + "] tpid[" + lvl_conf.tpid + "] pvp config error!\n");

                this.fin();
                return game_err_code.CONFIG_ERR;
            }

            //    if ("rnk_awd" in pvp_conf)
            //{
            //        // 有排行奖励战场
            //        this.rnk_batl = true;
            //    }

            if (this.preptm > 0)
            {
                // 有准备时间，准备时间内不准pk
                foreach (var map in this.maps)
                {
                    map.set_pk_seting(map_pk_setting_type.MPST_PEACE);
                }
                this.pkrest_tm = this.preptm;
            }

            return game_err_code.RES_OK;
        }

        public void update(long tm_elasped_s)
        {
            long time_s = Utility.time();

            if (this.tm_out > 0 && this.joininfo != null)
            {
                foreach (var plyinfo in this.joininfo.Values)
                {
                    int cid = plyinfo.cid;
                    if (plyinfo.join_tm + this.tm_out <= time_s)
                    { //超过规定时间         
                        if (sgplayersbycid.ContainsKey(cid))
                        {   //踢出玩家
                            var sid = plyinfo.sid;
                            var line = sgplayersbycid[cid].get_pack_data().line;
                            this.remove_player(sid);
                            Level.enter_world(sid, line);
                        }
                        else
                        {
                            this.joininfo.Remove(cid);
                            //delete this.joininfo[cid]; //清除数据
                        }
                    }

                }
            }
            if (this.pkrest_tm > 0)
            {
                // 有准备时间
                if (time_s >= this.pkrest_tm)
                {
                    // 准备时间过后，恢复pk设置
                    foreach (var map in this.maps)
                    {
                        map.reset_pk_seting();
                    }

                    this.pkrest_tm = 0;
                }
            }

            //    var rmv_kicked = [];
            //    foreach (idx, data in this.kick_out_plys )
            //{
            //        if (data.tm <= time_s)
            //        {
            //            if (idx in this.sgplayers )
            //        {
            //                this.remove_player(idx);
            //                level.enter_world(idx, data.ply.pinfo.line);
            //            }
            //            rmv_kicked.push(idx);
            //        }
            //    }

            //if (rmv_kicked.len() > 0)
            //{
            //    foreach (idx in rmv_kicked)
            //    {
            //        delete kick_out_plys[idx];
            //    }
            //}

            if (lvl_fined)
            {
                return;
            }

            foreach (KeyValuePair<int, IBaseUnit> pair in sgplayers)
            {
                int sid = pair.Key;
                IBaseUnit val = pair.Value;

                if (val == null)
                {
                    //delete sgplayers[idx];
                    continue;
                }
                // check & update user db datas
                if (time_s - val.db_last_update_tm > 300) // 5 minutes
                {
                    long time_between = time_s - val.db_last_update_tm;

                    val.flush_db_data(true, false); // write back user data to db per 5 minutes

                    //if (service.need_check_fcm)
                    //{
                    //    val.check_fcm(time_s, time_between);
                    //}
                }
            }

            if (this.state == level_state_type.LST_FINED)
            {
                if (time_s > this.close_tm)
                {
                    this.fin();
                    return;
                }

                //return;
            }
            else
            {
                if (this.gpmap_maxply > 0)
                {
                    foreach (var gp_map in this.gp_maps)
                    {
                        if (gp_map.closed) continue;

                        if (gp_map.fin)
                        {
                            if (time_s > gp_map.close_tm)
                            {
                                this.gp_map_close(gp_map);
                            }
                        }
                    }
                }
            }

            if (this.gpmap_maxply > 0)
            {
                foreach (var gp_map in this.gp_maps)
                {
                    if (gp_map.closed) continue;

                    foreach (var val in gp_map.maps.Values)
                    {
                        val.update(this, tm_elasped_s);
                    }
                }
            }
            else
            {
                foreach (var val in maps.Values)
                {
                    val.update(this, tm_elasped_s);
                }
            }

            // 检查车轮战回合切换
            //this._check_kumite_round(time_s);

            //// 检查是否满足通关条件
            //if (this.active_finchk)
            //{
            //    this._check_finish(time_s);
            //}

            // 检查是否到时间结束
            if (time_s > this.end_tm)
            {
                if (this.tmfin)
                {
                    // 到时间结束胜利副本
                    this.win = 0;   // 全体玩家胜利
                }

                // 到结束时间了，副本结束
                this.lvl_finish(time_s);
            }
            else if (this.round_tm > 0 && time_s > this.round_tm)
            {
                // 到结束时间了，回合结束
                this.round_finish(time_s);
            }

            //if (this.lvl_conf.lmtp ==(int) level_multi_type.LMT_TEAM)
            //{
            //    // 检查队伍是否存在
            //    local leader_cid = team.get_leader_cid(this.creator);
            //    if (leader_cid == 0)
            //    {
            //        // 队伍已解散

            //        // TO DO : 因为队伍id(team_id)会回收再分配，有一定几率出现队伍被销毁后id回收又立即被分配出去而level尚未执行至update，该情况下，
            //        //         副本创建队伍已解散但是却无法成功被检测，可考虑记录创建者cid至组队副本信息中，通过进一步比对创建者cid来判断队伍是否被销毁；

            //        this.fin();
            //    }
            //}

            //if (this.clter_conf && this.clter_conf.tp == clan_teritory_type.CTT_WAR_TERRITORY && this.next_check_clter_tm < time_s)
            //{
            //    // 需争夺的帮派领地副本

            //    // 判断战斗是否进行中

            //    local clanter_info = _get_clanter_info(this.clter_conf.id);

            //    if (!clanter_info || clanter_info.onwar)
            //    {
            //        // 争夺战进行中，领地副本需要关闭

            //        this.end_tm = time_s;
            //    }

            //    this.next_check_clter_tm = time_s + 60; // 1分钟检查一次
            //}

            // TO DO : 处理结束逻辑
        }
        public void lvl_finish(long cur_tm_s)
        {
            if (this.state == level_state_type.LST_FINED)
            {
                return;
            }

            this.state = level_state_type.LST_FINED;
            this.close_tm = cur_tm_s + 30;


            _lvl_finish(cur_tm_s);
        }

        public void _lvl_finish(long cur_tm_s)
        {

            int score = 0;
            long cost_tm = cur_tm_s - this.start_tm;

            var win_plys = _get_win_plys(cur_tm_s);


            var diff_lvl_conf = lvl_conf.diff_lvl[diff_lvl];
            if (win_plys.Count > 0)
            {
                // 胜利通关，生成奖励
                // this.prize[cid] <- {tp=,cnt=}

                if (lvl_conf.score.Count > 0)
                {
                    // 计算得分
                    foreach (var score_conf in lvl_conf.score)
                    {
                        if (score_conf.tm >= cost_tm && score_conf.score > score)
                        {
                            score = score_conf.score;
                        }
                    }
                }



            }
        }

        public Dictionary<int, IBaseUnit> _get_win_plys(long cur_tm_s)
        {
            Dictionary<int, IBaseUnit> win_plys = new Dictionary<int, IBaseUnit>();

            if (this.death_pts != null)
            {
                // 混战积分模式

                // 按积分排序
                List<Death_Pt> ply_ary = new List<Death_Pt>();
                foreach (var val in this.death_pts.Values)
                {
                    int idx = 0;
                    for (; idx < ply_ary.Count; ++idx)
                    {
                        if (val.pt >= ply_ary[idx].pt)
                        {
                            break;
                        }
                    }
                    ply_ary.Insert(idx, val);
                }

                if (this.sgplayersbycid.Count == 1)
                {
                    // 只有一个角色
                    foreach (KeyValuePair<int, IBaseUnit> pair in this.sgplayersbycid)
                    {
                        ply_ary.push(new Death_Pt() { cid = pair.Key, pt = 1 });
                    }
                }

                int i = 0;
                for (; i < ply_ary.Count && i < this.death_ptconf.wincnt; ++i)
                {
                    int cid = ply_ary[i].cid;
                    if (this.sgplayersbycid.ContainsKey(cid))
                    {
                        win_plys[cid] = this.sgplayersbycid[cid];
                    }
                }
            }
            else if (this.win >= 0)
            {
                if (death_match)
                {
                    // 死亡模式
                    if (this.sgplayersbyside.Count > 0 && this.win > 0)
                    {
                        // 分阵营情况下，this.win为胜利方阵营id
                        win_plys = this.sgplayersbyside[this.win];
                    }
                    else
                    {
                        // 混战情况下this.win为胜利者角色id
                        if (this.sgplayersbyside.ContainsKey(this.win))
                        {
                            foreach (var p in this.sgplayersbyside[this.win].Values)
                                win_plys = this.sgplayersbyside[this.win];
                        }
                    }
                }
                //if (this.death_hold_map > 0)
                //{
                //    // 占地图模式
                //    if (this.sgplayersbyside.len() > 0 && this.win > 0)
                //    {
                //        // 分阵营情况下，this.win为胜利方阵营id
                //        win_plys = this.sgplayersbyside[this.win];
                //    }
                //    else
                //    {
                //        // 混战情况下this.win为胜利者角色id
                //        if (this.win in this.sgplayersbycid)
                //    {
                //            win_plys[this.win] < -this.sgplayersbycid[this.win];
                //        }
                //    }
                //}
                //else if (this.kumiteply)
                //{
                //    // 车轮战模式，胜利者角色id
                //    if (this.win in this.sgplayersbycid)
                //{
                //        win_plys[this.win] < -this.sgplayersbycid[this.win];
                //    }
                //}
                else
                {
                    win_plys = this.sgplayersbycid;
                    if (this.win > 0)
                    {
                        // 胜利者有阵营
                        win_plys = this.sgplayersbyside[this.win];
                    }
                }
            }

            return win_plys;
        }



        public void round_finish(long cur_tm_s)
        {
            if (this.state == level_state_type.LST_FINED)
            {
                return;
            }

            round_conf cur_round_conf = null;
            if (this.rounds_conf.Count > 0)
            {
                cur_round_conf = this.rounds_conf[this.cur_round];

                ++this.cur_round;
                if (!this.rounds_conf.ContainsKey(this.cur_round))
                {
                    this.lvl_finish(cur_tm_s);
                    return;
                }
            }
            else
            {
                this.lvl_finish(cur_tm_s);
                return;
            }

            var win_plys = _get_win_plys(cur_tm_s);

            if (win_plys.Count <= 0)
            {
                // 没有胜利进入下一回合的角色，副本结束
                this.lvl_finish(cur_tm_s);
                return;
            }

            //this.lvl_finish(cur_tm_s);
            //return;




            List<int> winplycids = new List<int>();
            foreach (var ply in win_plys.Values)
            {
                int cid = ply.get_pack_data().cid;
                winplycids.push(cid);

                // 记录玩家晋级回合
                if (this.round_plys.ContainsKey(cid))
                {
                    this.round_plys[cid].rnd = this.cur_round;
                }
                else
                {
                    this.round_plys[cid] = new round_player()
                    {
                        cid = cid,
                        rnd = this.cur_round
                    };
                }
            }



            // 初始化本回合配置

            this._init_pvp(this.rounds_conf[this.cur_round], cur_tm_s);


            // 记录回合结束时间
            this.round_tm = cur_tm_s + (this.rounds_conf[this.cur_round].tm * 60);

            //local ret_msg = { winplycids = winplycids, cur_round = this.cur_round, round_tm = this.round_tm };

            // 回复胜利角色的死亡次数
            if (this.ghost_cnt > 0)
            {
                foreach (KeyValuePair<int, IBaseUnit> pair in win_plys)
                {
                    this.ghost_players[pair.Key] = ghost_cnt;
                }

                //ret_msg.ghostcnt < -ghost_cnt;
            }

            // 初始化车轮战角色
            //if (this.kumiteply)
            //{
            //    foreach (cid, ply in win_plys)
            //{
            //        local kpl = { cid = cid, name = ply.pinfo.name, lvl = ply.pinfo.level, stat = lvl_kumite_state.LKS_WAIT, pt = 0, combpt = ply.pinfo.combpt };
            //        this.kumiteply.push(kpl);
            //        this.ghost_players[cid] < -0;

            //        ply.pinfo.ghost = true;

            //    // send lvl_side_info msg
            //    ::send_rpc(ply.pinfo.sid, 247, { ghost = true});

            //        // bcast attchange msg
            //        ply.broad_cast_zone_msg(26, { iid = ply.pinfo.iid, ghost = true});
            //    }

            //    // 广播车轮战队列变化消息
            //    // send lvl_side_info msg
            //    this.broad_cast_msg(247, { kumiteply = this.kumiteply});
            //}

            //if (this.preptm > 0)
            //{
            //    ret_msg.preptm < -preptm;
            //}

            // 广播回合切换消息

            // send lvl_side_info msg
            //this.broad_cast_msg(247, { round_change = ret_msg});
        }
        public void _check_finish(long time_s)
        {
            if (this.state == level_state_type.LST_FINED)
            {
                return;
            }

            //if (this.kmfinbyside.Count > 0)
            //{
            //    // 分阵营
            //    foreach (sideid, kmside in this.kmfinbyside)
            //{
            //        local finished = true;
            //        foreach (km in kmside)
            //        {
            //            if (km.cntleft > 0)
            //            {
            //                finished = false;
            //                break;
            //            }
            //        }

            //        if (finished)
            //        {
            //            this.win = sideid; // 该阵营胜利
            //            this.round_finish(time_s);

            //            break;
            //        }
            //    }
            //}
            //else
            if (this.kmfin.Count > 0)
            {
                // 杀怪胜利
                if (this.gpmap_maxply > 0)
                {
                    foreach (var gp_map in this.gp_maps)
                    {
                        if (gp_map.fin) continue;

                        bool finished = true;
                        foreach (var km in gp_map.kmfin.Values)
                        {
                            if (km.cntleft > 0)
                            {
                                finished = false;
                                break;
                            }
                        }

                        if (finished)
                        {
                            gp_map.win = 0; // 全体玩家胜利
                            this.gp_map_finish(gp_map, time_s);
                        }
                    }
                }
                else
                {
                    var finished = true;
                    foreach (var km in this.kmfin.Values)
                    {
                        if (km.cntleft > 0)
                        {
                            finished = false;
                            break;
                        }
                    }

                    if (finished)
                    {
                        this.win = 0; // 全体玩家胜利
                        this.round_finish(time_s);
                    }
                }
            }
            //else if (this.ptfinbyside.Count > 0)
            //{
            //    // 积分胜利制

            //    if (this.maxptside > 0)
            //    {
            //        // 某方达到最大积分
            //        this.win = this.maxptside;
            //        this.round_finish(time_s);
            //    }
            //    else if ((this.round_tm > 0 && time_s > this.round_tm) || (time_s > this.end_tm))
            //    {
            //        // 时间到了，得分最高的一方获胜
            //        local maxpt = 0;
            //        local maxside = -1;
            //        local draw = false;
            //        foreach (sideid, ptside in this.ptfinbyside)
            //    {
            //            if (ptside.pt > maxpt)
            //            {
            //                maxpt = ptside.pt;
            //                maxside = sideid;

            //                draw = false;
            //            }
            //            else if (ptside.pt == maxpt)
            //            {
            //                draw = true;
            //            }
            //        }

            //        if (!draw)
            //        {
            //            this.win = maxside;
            //            if (time_s > this.end_tm)
            //            {
            //                this.lvl_finish(time_s);
            //            }
            //            else
            //            {
            //                this.round_finish(time_s);
            //            }
            //        }
            //    }
            //}
            else if (death_match)
            {
                // 死亡模式
                if (time_s < this.preptm)
                {
                    return;
                }

                // 超过准备时间，开始判断是否结束

                if (this.sgplayersbyside.Count > 0)
                {
                    // 分阵营死亡模式
                    int alive_sideid = 0;
                    bool finish = true;
                    foreach (KeyValuePair<int, Dictionary<int, IBaseUnit>> pair in this.sgplayersbyside)
                    {
                        bool all_ghost = true;
                        var players = pair.Value;
                        foreach (KeyValuePair<int, IBaseUnit> px in players)
                        {
                            if (!this.ghost_players.ContainsKey(px.Key))
                                continue;

                            if (this.ghost_players[px.Key] > 0)
                            {
                                all_ghost = false;
                                break;
                            }
                        }

                        if (all_ghost)
                        {
                            continue;
                        }

                        if (alive_sideid != 0)
                        {
                            finish = false;
                            break;
                        }

                        alive_sideid = pair.Key;
                    }

                    if (finish)
                    {
                        if (alive_sideid > 0) this.win = alive_sideid;
                        this.round_finish(time_s);
                    }
                }
                else
                {
                    // 混战死亡模式

                    int alive_cid = 0;
                    bool finish = true;
                    foreach (KeyValuePair<int, IBaseUnit> pair in this.sgplayersbycid)
                    {
                        if (!this.ghost_players.ContainsKey(pair.Key))
                            continue;

                        if (this.ghost_players[pair.Key] <= 0)
                            continue;

                        if (alive_cid != 0)
                        {
                            finish = false;
                            break;
                        }

                        alive_cid = pair.Key;
                    }

                    if (finish)
                    {
                        if (alive_cid > 0) this.win = alive_cid;
                        this.round_finish(time_s);
                    }
                }
            }
            else if (this.death_hold_map > 0)
            {//地图上只剩下一个角色/阵营 则表示胜利

                if (time_s < this.preptm)
                {
                    return;
                }

                var map = this.maps[this.death_hold_map];
                // 超过准备时间，开始判断是否结束
                if (this.sgplayersbyside.Count > 0)
                {
                    // 分阵营死亡模式
                    int alive_sideid = 0;
                    bool finish = true;
                    if (map.map_players.Count > 0)
                    {
                        foreach (var ply in map.map_players.Values)
                        {
                            if (alive_sideid == 0)
                            {
                                alive_sideid = ply.get_pack_data().lvlsideid;
                                continue;
                            }

                            if (alive_sideid != ply.get_pack_data().lvlsideid)
                            {
                                finish = false;
                                break;
                            }
                        }
                    }

                    if (finish)
                    {
                        if (alive_sideid > 0) this.win = alive_sideid;
                        this.round_finish(time_s);
                    }
                }
                else
                {
                    if (map.map_players.Count <= 1)
                    {
                        foreach (var ply in map.map_players.Values)
                        {
                            this.win = ply.get_pack_data().cid;
                        }
                        this.round_finish(time_s);
                    }
                }
            }
            //else if (this.kumiteply)
            //{
            //    // 车轮战模式

            //    //if((this.kumiteply.Count >= kumite_conf.minply) )
            //    if ((this.kumiteply.Count >= 2 && time_s >= this.kumite_waittm)) // 满足2人以上且时间满足开始等待时间
            //    {
            //        if ((kumitefight_ply.Count <= 0))
            //        {
            //            // 车轮战未开始，开始
            //            local kpl1 = this.kumiteply[0];
            //            local kpl2 = this.kumiteply[1]; // 至少需要两名玩家才能开始！

            //            kpl1.stat = lvl_kumite_state.LKS_FIGHTING;
            //            kpl2.stat = lvl_kumite_state.LKS_FIGHTING;

            //            // 回合开始
            //            _start_kumite_round(time_s, kpl1, kpl2);

            //            // 发送队列变化消息
            //            // send lvl_side_info msg
            //            this.broad_cast_msg(247, { kumiteply =[kpl1, kpl2]});
            //        }
            //        else if (kumitefight_ply.Count <= 1)
            //        {
            //            // 当前战斗的玩家只有一个了
            //            foreach (cid,ply in kumitefight_ply)
            //        {
            //                this.win = cid;
            //                break;
            //            }
            //            this.round_finish(time_s);
            //        }
            //    }
            //}
            //else if (this.cltwar)
            //{
            //    if ("hold_map" in this.cltwar )
            //{//地图上剩下同帮派的角色 则表示占领此地图
            //        local map = this.maps[this.cltwar.hold_map.mapid];
            //        local hold_clan = 0;
            //        if (map.map_players.Count)
            //        {
            //            foreach (ply in map.map_players)
            //            {
            //                if (hold_clan == 0)
            //                {
            //                    hold_clan = ply.pinfo.clanid;
            //                    continue;
            //                }

            //                if (hold_clan != ply.pinfo.clanid)
            //                {
            //                    hold_clan = 0;
            //                    break;
            //                }
            //            }

            //            if (hold_clan != 0)
            //            {
            //                this.cltwar.hold_map.hold_clan = hold_clan;
            //                if (hold_clan == this.cltwar.owner_clanid)
            //                {// 防守方获胜
            //                    this.win = this.cltwar.defside;
            //                }
            //                else
            //                {  // 进攻方获胜
            //                    this.win = this.cltwar.atkside;
            //                }
            //                this.round_finish(time_s);
            //            }
            //        }
            //        else
            //        {
            //            // 防守方获胜
            //            this.win = this.cltwar.defside;
            //            this.round_finish(time_s);
            //        }
            //    }
            //else if ("kmfin" in this.cltwar )
            //{
            //        if (this.cltwar.kmfin.cntleft <= 0)
            //        {
            //            // 进攻方获胜
            //            this.win = this.cltwar.atkside;
            //            this.round_finish(time_s);
            //        }
            //    }
            //else if (time_s >= this.end_tm)
            //    {
            //        // 时间到了，防守方获胜
            //        this.win = this.cltwar.defside;
            //        this.lvl_finish(time_s);
            //    }
            //}
            //else if (this.clcqwar)
            //{
            //    if (this.clcqwar.cntleft <= 0)
            //    {
            //        // 进攻方获胜
            //        this.win = this.clcqwar.atkside;
            //        this.round_finish(time_s);
            //    }
            //    else if (time_s >= this.end_tm)
            //    {
            //        // 时间到了，防守方获胜
            //        this.win = this.clcqwar.defside;
            //        this.lvl_finish(time_s);
            //    }
            //}
        }



        public void gp_map_close(group_map gp_map)
        {
            if (gp_map.closed) return;

            gp_map.closed = true;

            List<IBaseUnit> to_remove_ply = new List<IBaseUnit>();
            foreach (var ply in gp_map.plys.Values)
            {
                to_remove_ply.push(ply);
            }

            foreach (var ply in to_remove_ply)
            {
                this.remove_player(ply.get_pack_data().sid, false);
                Level.enter_world(ply.get_pack_data().sid, ply.get_pack_data().line);
            }

            foreach (var val in gp_map.maps.Values)
            {
                val.fin();
            }

            // fin lvl
            bool all_gm_closed = true;
            foreach (var gx in this.gp_maps)
            {
                if (!gx.closed)
                {
                    all_gm_closed = false;
                    break;
                }
            }

            if (all_gm_closed)
            {
                this.fin();
            }
        }



        public void ply_change_map(IBaseUnit sprite, grid_map map)

        {
            throw new NotImplementedException();
        }

        public void on_km(IBaseUnit killer, IBaseUnit mon)
        {
            if (killer == null)
                return;

            IMapUnit pl = killer.get_pack_data();
            IMapUnit mondata = mon.get_pack_data();

            int mid = mondata.mid;
            if (is_score_km)
            {
                if (!score_km.ContainsKey(mid))
                    score_km._score_km[mid] = new _score_km() { mid = mid, cnt = 0 };

                var kminfo = score_km._score_km[mid];
                kminfo.cnt++;

                score_km.kmcnt++; //算个总数

                //::g_dump( "on_lvl_km score_km: ", score_km );
            }

            if (pl.lvlsideid > 0)
            {
                // 分阵营

                //if (this.cltwar)
                //{
                //    // 领地争夺战
                //    if (("kmfin" in this.cltwar) && mid == this.cltwar.kmfin.tar_mid)
                //{
                //        --this.cltwar.kmfin.cntleft;
                //        if (this.cltwar.kmfin.cntleft < 0)
                //        {
                //            this.cltwar.kmfin.cntleft = 0;
                //        }

                //        // 通知客户端杀怪 lvl_km msg
                //        if (killer.get_sprite_type() == map_sprite_type.MST_PLAYER)
                //        {
                //            this.broad_cast_msg(249, { sideid = pl.lvlsideid, cid = pl.cid, name = pl.name, mid = mid});
                //        }
                //        else
                //        {
                //            // 怪物杀死怪物
                //            this.broad_cast_msg(249, { sideid = pl.lvlsideid, cid = 0, name = killer.monconf.name, mid = mid});
                //        }
                //    }
                //}
                //else if (this.clcqwar)
                //{
                //    // 帮派奴役战
                //    if (mid == this.clcqwar.tar_mid)
                //    {
                //        --this.clcqwar.cntleft;
                //        if (this.clcqwar.cntleft < 0)
                //        {
                //            this.clcqwar.cntleft = 0;
                //        }

                //        // 通知客户端杀怪 lvl_km msg
                //        if (killer.get_sprite_type() == map_sprite_type.MST_PLAYER)
                //        {
                //            this.broad_cast_msg(249, { sideid = pl.lvlsideid, cid = pl.cid, name = pl.name, mid = mid});
                //        }
                //        else
                //        {
                //            // 怪物杀死怪物
                //            this.broad_cast_msg(249, { sideid = pl.lvlsideid, cid = 0, name = killer.monconf.name, mid = mid});
                //        }
                //    }
                //}
                //    else if (pl.lvlsideid in this.kmfinbyside)
                //{
                //        local kmside = this.kmfinbyside[pl.lvlsideid];

                //        if (mid in kmside)
                //    {
                //            local km = kmside[mid];

                //            --km.cntleft;

                //            // 通知客户端杀怪 lvl_km msg
                //            if (killer.get_sprite_type() == map_sprite_type.MST_PLAYER)
                //            {
                //                this.broad_cast_msg(249, { sideid = pl.lvlsideid, cid = pl.cid, name = pl.name, mid = mid});
                //            }
                //            else
                //            {
                //                // 怪物杀死怪物
                //                this.broad_cast_msg(249, { sideid = pl.lvlsideid, cid = 0, name = killer.monconf.name, mid = mid});
                //            }

                //            if (km.cntleft < 0)
                //            {
                //                km.cntleft = 0;
                //            }

                //            //_check_finish();
                //        }
                //    }
                //else if (pl.lvlsideid in this.ptfinbyside)
                //{
                //        if (this.maxptside < 0 && ("kmpt" in mon.monconf))
                //    {
                //            local sidept = this.ptfinbyside[pl.lvlsideid];

                //            // 杀怪得积分
                //            sidept.pt += mon.monconf.kmpt;

                //            // 通知客户端积分值变化 lvl_km msg
                //            this.broad_cast_msg(249, { sideid = pl.lvlsideid, pt = sidept.pt});

                //            if (sidept.pt >= sidept.maxpt)
                //            {
                //                this.maxptside = pl.lvlsideid;
                //            }
                //            // 胜利检查在update中进行
                //        }
                //    }
            }
            else if (killer.get_sprite_type() == map_sprite_type.MstPlayer)
            {
                //IMapUnit pl = killer.get_pack_data();

                if (kmfin.ContainsKey(mid))
                {
                    if (this.gpmap_maxply > 0)
                    {
                        var gp_map = this.gp_maps[this.cid2gpid[pl.cid] - 1];
                        var km = gp_map.kmfin[mid];
                        --km.cntleft;
                        if (km.cntleft < 0)
                        {
                            km.cntleft = 0;
                        }
                    }
                    else
                    {
                        var km = this.kmfin[mid];
                        --km.cntleft;
                        if (km.cntleft < 0)
                        {
                            km.cntleft = 0;
                        }
                    }
                    // 通知客户端杀怪 lvl_km msg
                    //this.broad_cast_msg(249, { sideid = pl.lvlsideid, cid = pl.cid, name = ply.pinfo.name, mid = mid});

                    //_check_finish();
                }
            }

            if (killer.get_sprite_type() == map_sprite_type.MstPlayer)
            {
                //        if ("bcast_kill" in this.lvl_conf )
                //{
                //            local bcast_kill_conf = this.lvl_conf.bcast_kill[0];
                //            foreach (monid in bcast_kill_conf.ids)
                //            {
                //                if (monid == mid)
                //                {
                //                    // 发送系统公告
                //                    _broad_cast_sys_msg({ cid = pl.cid, name = pl.name, tp = bcast_msg_tp.LVL_MON_KILLED, par = mid, par1 = this.ltpid});
                //                    break;
                //                }
                //            }
                //        }
            }
            if (mon.get_pack_data().owner_cid > 0)
            {
                //    if (this.diff_lvl in map_need_km)
                //{
                //        foreach (mapid,kmdata in this.map_need_km[this.diff_lvl])
                //    {
                //            if (kmdata.mapid == mon.gmap.mapid)
                //            {   //击杀地图需求怪，需广播到所有玩家
                //                if (kmdata.mid == 0 || kmdata.mid == mon.mondata.mid)
                //                {
                //                    local moncnt = this.get_map_mon(mon.gmap.mapid);
                //                    broad_cast_msg(237, { tp = 4, mapid = kmdata.mapid, cntleft = moncnt});
                //                }
                //                break;
                //            }
                //        }
                //    }

            }
        }

        public void on_kp(IBaseUnit killer, IBaseUnit tar_ply)
        {
            // 玩家角色被击杀

            if (killer != null)
            {
                // 尝试更新每日必做任务
                //_try_up_dmis(killer, "lvlkp", ltpid);
            }

            //if (no_kp_hexp)
            //{
            //    // 击杀不做任何处理
            //    return;
            //}

            // 判断是否要变成ghost
            //if (ghost_cnt > 0 || this.kumiteply)
            //{
            //    if (tar_ply.pinfo.cid in this.ghost_players)
            //{
            //        --this.ghost_players[tar_ply.pinfo.cid];
            //        if (this.ghost_players[tar_ply.pinfo.cid] <= 0)
            //        {
            //            this.ghost_players[tar_ply.pinfo.cid] = 0;

            //            if (!tar_ply.pinfo.ghost)
            //            {
            //                tar_ply.pinfo.ghost = true;

            //            // 通知玩家成为ghost

            //            // send lvl_side_info msg
            //            ::send_rpc(tar_ply.pinfo.sid, 247, { ghost = true});

            //                // bcast attchange msg
            //                tar_ply.broad_cast_zone_msg(26, { iid = tar_ply.pinfo.iid, ghost = true});
            //            }
            //        }
            //    }
            //}

            if (killer == null)
            {
                return;
            }

            IMapUnit killer_pl = killer.get_pack_data();

            // 更新阵营积分值
            if (this.maxptside < 0 && killer_pl.lvlsideid > 0)
            {
                if (this.ptfinbyside.ContainsKey(killer_pl.lvlsideid))
                {
                    var sidept = this.ptfinbyside[killer_pl.lvlsideid];

                    var kp_pt = 1;// 增加积分值

                    sidept.pt += kp_pt; // 增加积分值

                    // 通知客户端积分值变化 lvl_km msg
                    //this.broad_cast_msg(249, { sideid = killer.pinfo.lvlsideid, pt = sidept.pt});

                    if (sidept.pt >= sidept.maxpt)
                    {
                        this.maxptside = killer_pl.lvlsideid;
                    }
                    // 胜利检查在update中进行
                }

            }

            // 更新混战积分值
            if (this.death_pts != null)
            {
                var kp_pt = 1;// 增加积分值

                if (!this.death_pts.ContainsKey(killer_pl.cid))
                {
                    this.death_pts[killer_pl.cid] = new Death_Pt() { cid = killer_pl.cid, pt = kp_pt };
                }
                else
                {
                    this.death_pts[killer_pl.cid].pt += kp_pt;
                }

                // 通知客户端积分值变化通过kp模拟
            }

            // 计算荣誉值
            //local hexp_add = 0;
            //local hexp_add = 1;
            //if(killer.pinfo.level - tar_ply.pinfo.level < 10)
            //{
            //    hexp_add = (tar_ply.pinfo.level/10).tointeger() - 1;
            //    if(hexp_add <= 0) hexp_add = 1;
            //}
            //hexp_add = killer.add_hexp(hexp_add);

            //if(hexp_add > 0)
            //{
            //    // 尝试更新排行活动值
            //    _rnkact_on_pvphexp(killer, hexp_add);
            //}

            // 统计玩家击杀数
            //    local skprec = null;
            //    if (killer.pinfo.cid in this.kprec)
            //{
            //        skprec = this.kprec[killer.pinfo.cid];
            //        ++skprec.kp;
            //        ++skprec.ckp;
            //        skprec.lvl_hexp += hexp_add;
            //    }
            //else
            //{
            //        local clanid = 0;
            //        if ("clanid" in killer.pinfo)
            //    {
            //            clanid = killer.pinfo.clanid;
            //        }
            //        skprec = { cid = killer.pinfo.cid, sideid = killer.pinfo.lvlsideid, kp = 1, ckp = 1, dc = 0, ac = 0, lvl_hexp = hexp_add, bcasted = 0, clanid = clanid};
            //        this.kprec[killer.pinfo.cid] < -skprec;
            //    }

            // 统计玩家被杀数
            //    local tar_skprec = null;
            //    //local tar_old_ckp = 0;
            //    if (tar_ply.pinfo.cid in this.kprec)
            //{
            //        tar_skprec = this.kprec[tar_ply.pinfo.cid];
            //        ++tar_skprec.dc;
            //        //tar_old_ckp = tar_skprec.ckp;
            //        tar_skprec.ckp = 0; // 连杀清零
            //    }
            //else
            //{
            //        local clanid = 0;
            //        if ("clanid" in tar_ply.pinfo)
            //    {
            //            clanid = tar_ply.pinfo.clanid;
            //        }
            //        tar_skprec = { cid = tar_ply.pinfo.cid, sideid = tar_ply.pinfo.lvlsideid, kp = 0, ckp = 0, dc = 1, ac = 0, lvl_hexp = 0, bcasted = 0, clanid = clanid};
            //        this.kprec[tar_ply.pinfo.cid] < -tar_skprec;
            //    }

            // 通知客户端击杀记录变化
            // send mod_lvl_selfpvpinfo msg
            //::send_rpc(killer.pinfo.sid, 238, { kp = skprec.kp/*, lvl_hexp=skprec.lvl_hexp*/});
            //::send_rpc(tar_ply.pinfo.sid, 238, { dc = tar_skprec.dc});

            // 判断击杀者连杀数是否需要广播
            //    if (skprec.ckp in game_data_conf.general.pvp_combo_kp)
            //{
            //        local pvp_combo_kp_conf = game_data_conf.general.pvp_combo_kp[skprec.ckp];
            //        if (pvp_combo_kp_conf.bcasttp == 1)
            //        {
            //            // 副本广播
            //            // broad cast lvl_broadcast msg
            //            this.broad_cast_msg(236, { tp = 1, cid = killer.pinfo.cid, name = killer.pinfo.name, ckp = skprec.ckp});
            //        }
            //        else if (pvp_combo_kp_conf.bcasttp == 2)
            //        {
            //            // 全服广播
            //            _broad_cast_sys_msg({ cid = killer.pinfo.cid, name = killer.pinfo.name, tp = bcast_msg_tp.LVL_PVP_COMBO_KP, ltpid = ltpid, ckp = skprec.ckp});
            //        }

            //        if ("achive" in pvp_combo_kp_conf)
            //    {
            //            // 获得连杀成就
            //            if (!killer.has_achive(pvp_combo_kp_conf.achive))
            //            {
            //                killer.pinfo.achives.push(pvp_combo_kp_conf.achive);

            //            // 发送获得成就消息
            //            // send gain_achive msg
            //            ::send_rpc(killer.pinfo.sid, 5, { achive = pvp_combo_kp_conf.achive});
            //            }
            //        }

            //        skprec.bcasted = skprec.ckp;
            //    }
            // 判断被杀者连杀数是否需要广播
            //if (tar_skprec.bcasted)
            //{
            //    // 副本广播
            //    // broad cast lvl_broadcast msg
            //    this.broad_cast_msg(236, { tp = 2, cid = killer.pinfo.cid, name = killer.pinfo.name, tckp = tar_skprec.bcasted, tcid = tar_ply.pinfo.cid, tname = tar_ply.pinfo.name});

            //    tar_skprec.bcasted = 0;
            //}

            // 刷新击杀排行榜
            //local game_conf = get_general_game_conf();
            //local idx = this.kpboard.Count - 1;
            //if (game_conf.lvl_pvpinfo_cnt > 0 && (this.kpboard.Count < game_conf.lvl_pvpinfo_cnt || this.kpboard[idx].kp < skprec.kp))
            //{
            //    // 上榜了
            //    local already_in = false;
            //    for (; idx >= 0; --idx)
            //    {
            //        local cur_rec = this.kpboard[idx];
            //        if (cur_rec.cid == skprec.cid)
            //        {
            //            already_in = true;
            //            break;
            //        }
            //    }

            //    if (!already_in)
            //    {
            //        this.kpboard.push(skprec);
            //    }

            //    local sort_func = function(a, b)
            //        {
            //        if (a.kp > b.kp)
            //        {
            //            return -1;
            //        }
            //        else if (a.kp == b.kp && a.dc <= b.dc)
            //        {
            //            return -1;
            //        }

            //        return 1;
            //    }
            //    this.kpboard.sort(sort_func);

            //    for (; this.kpboard.Count > game_conf.lvl_pvpinfo_cnt;)
            //    {
            //        this.kpboard.pop();
            //    }
            //}

            // 刷新助攻数据
            //if (tar_ply.kp_asist_rec)
            //{
            //    foreach (asist_cid in tar_ply.beatk_ply_cids)
            //    {
            //        if (asist_cid == killer.pinfo.cid)
            //        {
            //            continue;
            //        }

            //        if (!(asist_cid in this.sgplayersbycid))
            //    {
            //            continue;
            //        }

            //        local asist_ply = this.sgplayersbycid[asist_cid];

            //        // 统计玩家助攻
            //        local asist_skprec = null;
            //        if (asist_cid in this.kprec)
            //    {
            //            asist_skprec = this.kprec[asist_cid];
            //            ++asist_skprec.ac;
            //        }
            //    else
            //    {
            //            local clanid = 0;
            //            if ("clanid" in asist_ply.pinfo)
            //        {
            //                clanid = asist_ply.pinfo.clanid;
            //            }
            //            asist_skprec = { cid = asist_cid, sideid = asist_ply.pinfo.lvlsideid, kp = 0, ckp = 0, dc = 0, ac = 1, lvl_hexp = 0, bcasted = 0, clanid = clanid};
            //            this.kprec[asist_cid] < -asist_skprec;
            //        }

            //    // 通知客户端助攻记录变化
            //    // send mod_lvl_selfpvpinfo msg
            //    ::send_rpc(asist_ply.pinfo.sid, 238, { ac = asist_skprec.ac});
            //    }
            //}
        }

        public void on_dmg(IBaseUnit atker, IBaseUnit tar_spr, double dmg)
        {
            // 造成伤害
            //if (this.kumiteply)
            //{
            //    // 车轮战，计算积分
            //    if (atker.get_sprite_type() != map_sprite_type.MST_PLAYER)
            //    {
            //        return;
            //    }

            //    if (!(atker.pinfo.cid in this.kumitefight_ply))
            //{
            //        return;
            //    }

            //    this.kumitefight_ply[atker.pinfo.cid].pt += (dmg / 100).tointeger();
            //}
            //else if (this.cltwar)
            //{
            //    // 攻城战，计算积分

            //    if (atker.get_sprite_type() != map_sprite_type.MST_PLAYER)
            //    {
            //        return;
            //    }

            //    if (!("clanid" in atker.pinfo))
            //{
            //        return;
            //    }

            //    if (atker.pinfo.clanid == this.cltwar.owner_clanid)
            //    {
            //        return;
            //    }

            //    local ptper = this.cltwar.ptper;
            //    if ((tar_spr.get_sprite_type() == map_sprite_type.MST_MONSTER) && (tar_spr.mondata.mid in this.cltwar.ptmon))
            //{
            //        ptper = this.cltwar.ptmon[tar_spr.mondata.mid].ptper;
            //    }

            //    local pt = (dmg / ptper).tointeger();
            //    local totalpt = pt;

            //    if (!(atker.pinfo.clanid in this.cltwar.clanpts))
            //{
            //        this.cltwar.clanpts[atker.pinfo.clanid] < -pt;
            //    }
            //else
            //{
            //        this.cltwar.clanpts[atker.pinfo.clanid] += pt;
            //        totalpt = this.cltwar.clanpts[atker.pinfo.clanid];
            //    }
            //    if ((totalpt > this.cltwar.max_clanpt))
            //    {
            //        this.cltwar.max_clanpt = totalpt;
            //        this.cltwar.max_clanid = atker.pinfo.clanid;
            //    }

            //    if (!(atker.pinfo.cid in this.cltwar.plypts))
            //{
            //        this.cltwar.plypts[atker.pinfo.cid] < - { pt = pt, clanid = atker.pinfo.clanid};
            //    }
            //else
            //{
            //        this.cltwar.plypts[atker.pinfo.cid].pt += pt;
            //    }
            //}
        }

        public Dictionary<int, IBaseUnit> _get_gp_map_win_plys(group_map gp_map, long cur_tm_s)
        {
            Dictionary<int, IBaseUnit> win_plys = new Dictionary<int, IBaseUnit>();

            if (this.death_pts != null)
            {
                // 混战积分模式

                // 按积分排序
                List<Death_Pt> ply_ary = new List<Death_Pt>();
                foreach (var val in this.death_pts.Values)
                {
                    if (!gp_map.plys.ContainsKey(val.cid))
                        continue;

                    int idx = 0;
                    for (; idx < ply_ary.Count; ++idx)
                    {
                        if (val.pt >= ply_ary[idx].pt)
                        {
                            break;
                        }
                    }
                    ply_ary.Insert(idx, val);
                }

                int i = 0;
                for (; i < ply_ary.Count && i < this.death_ptconf.wincnt; ++i)
                {
                    var cid = ply_ary[i].cid;
                    win_plys[cid] = gp_map.plys[cid];
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
                        var win_side_players = this.sgplayersbyside[gp_map.win];
                        foreach (var ply in win_side_players.Values)
                        {
                            int cid = ply.get_pack_data().cid;
                            if (gp_map.plys.ContainsKey(cid))
                            {
                                win_plys[cid] = ply;
                            }
                        }
                    }
                    else
                    {
                        // 混战情况下this.win为胜利者角色id
                        if (gp_map.plys.ContainsKey(gp_map.win))
                        {
                            win_plys[gp_map.win] = gp_map.plys[gp_map.win];
                        }
                    }
                }
                //else if (this.kumiteply)
                //{
                //    // 车轮战模式，胜利者角色id
                //    if (gp_map.win in gp_map.plys)
                //{
                //        win_plys[gp_map.win] < -gp_map.plys[gp_map.win];
                //    }
                //}
                else
                {
                    win_plys = gp_map.plys;
                    if (gp_map.win > 0)
                    {
                        // 胜利者有阵营
                        var win_side_players = this.sgplayersbyside[gp_map.win];
                        foreach (var ply in win_side_players.Values)
                        {
                            int cid = ply.get_pack_data().cid;
                            if (gp_map.plys.ContainsKey(cid))
                            {
                                win_plys[cid] = ply;
                            }
                        }
                    }
                }
            }

            return win_plys;
        }


        public void gp_map_finish(group_map gp_map, long cur_tm_s)
        {
            if (gp_map.fin)
            {
                return;
            }
            gp_map.fin = true;
            gp_map.close_tm = cur_tm_s + 30;

            int score = 0;
            long cost_tm = cur_tm_s - this.start_tm;

            var win_plys = _get_gp_map_win_plys(gp_map, cur_tm_s);



            if (win_plys.Count > 0)
            {
                if (lvl_conf.score != null && lvl_conf.score.Count > 0)
                {
                    // 计算得分
                    foreach (var score_conf in lvl_conf.score)
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

                    //}
                }


            }
            else
            {
            }

        }

        public void on_mon_respawn(IBaseUnit mon)
        {
            IMapUnit mondata = mon.get_pack_data();

            int mid = mondata.mid;
            //    if ("bcast_respawn" in this.lvl_conf )
            //{
            //        local bcast_respawn_conf = this.lvl_conf.bcast_respawn[0];
            //        foreach (monid in bcast_respawn_conf.ids)
            //        {
            //            if (monid == mid)
            //            {
            //                // 发送系统公告
            //                _broad_cast_sys_msg({ tp = bcast_msg_tp.LVL_MON_RESPAWN, par = mid, par1 = this.ltpid});
            //                break;
            //            }
            //        }
            //    }

        }

        public void on_ply_die(IBaseUnit ply)
        {


            if (this.plyrespawn_tm > 0)
            {
                ply.allow_respawn_tm_s = this.plyrespawn_tm;

                int cid = ply.get_pack_data().cid;
                this.plyrespawn_tms[cid] = Utility.time() + this.plyrespawn_tm;

                // send self_attchange msg
                //::send_rpc(ply.pinfo.sid, 32, { respawn_tms = ply.allow_respawn_tm_s});
            }
        }

    }
}
