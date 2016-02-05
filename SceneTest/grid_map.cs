using SceneTest;
using SceneTestLib;
using SceneTestLib.Confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
    public class trigger_instance
    {
        public int cnt { get; set; }
        public int kmcnt
        {
            get; set;
        }

        public int tkmcnt
        {
            get; set;
        }

        public trigger_conf conf { get; set; }
    };
}
public class grid_map
{
    private Atomic_Int _iid_generator = new Atomic_Int(0);
    public Grd grd { get; set; }
    public bool blvlmap { get; set; }
    public map_conf map_conf = null;
    public Dictionary<int, IBaseUnit> map_mons = null;
    public Dictionary<int, List<IBaseUnit>> map_mon_bymid = new Dictionary<int, List<IBaseUnit>>();
    public Dictionary<int, IBaseUnit> map_sprites = null;
    /// <summary>
    /// key为iid
    /// </summary>
    public Dictionary<int, IBaseUnit> map_players = null;

    /// <summary>
    /// key为sid
    /// </summary>
    public Dictionary<int, IBaseUnit> map_players_bysid = null;

    /// <summary>
    /// key为cid
    /// </summary>
    public Dictionary<int, IBaseUnit> map_players_bycid = null;
    public int mapid = 0;
    public Dictionary<int, IBaseUnit> map_flys = null;
    public Level worldsvr = null;
    public map_pk_setting_type pk_seting = 0;
    public map_pk_setting_type origin_pk_seting = map_pk_setting_type.MPST_PEACE;
    public bool immrespawn = false; // 是否不允许原地复活
    public bool ignore_side = false;  // 忽略阵营情况，同阵营之间可以pk

    public Dictionary<int, SkillData> map_skills = null;

    public Dictionary<int, List<IBaseUnit>> petmon_cache = null; // 缓存的战斗宠物实例，以mid为key

    public Dictionary<int, map_item> map_dpitms = new Dictionary<int, map_item>();

    public List<link_conf> add_links = new List<link_conf>();

    public int map_dpidseed = 1;

    //    temp_sid_ary = null;
    //    map_dpitms = null;

    //trigger_conf = null;    // 触发器配置表
    //tmtriggers = null;      // 时间触发器
    //areatriggers = null;    // 区域触发器
    //kmtriggers = null;      // 杀怪触发器
    //useitmtriggers = null;  // 使用道具触发器
    //mistriggers = null;  // 任务触发器
    //othertriggers = null;   // 其他触发器

    //callmon_added = null;   // 召唤怪物触发器新增怪物数量

    //add_npcs = null;        // 动态添加的npc列表
    //add_links = null;       // 动态添加的link点列表
    //mapstats = null;        // 地图技能状态对象
    public int mapstatid_seed = 0;

    public Dictionary<int, List<Point2D>> paths = null;           // 路径表

    public long last_check_tm = 0;      // 最后一次update检测时间

    public long close_tm { get; set; }

    public bool map_fined = false;

    public Dictionary<int, trigger_instance> kmtriggers = new Dictionary<int, trigger_instance>();
    public Dictionary<int, trigger_conf> trigger_conf = new Dictionary<int, trigger_conf>();

    /// <summary>
    /// key为mid，value为数量
    /// </summary>
    public Dictionary<int, int> callmon_added = new Dictionary<int, int>();

    public int get_plycnt()
    {
        return this.map_players.Count;
    }

    public int width
    {
        get;
        private set;
    }

    public int height
    {
        get;
        private set;
    }

    public grid_map(int mapid)
    {
        map_conf = Utility.get_map_conf(mapid);

        this.grd = new Grd(map_conf);

        this.mapid = mapid;
        this.map_mons = new Dictionary<int, IBaseUnit>();
        this.map_mon_bymid = new Dictionary<int, List<IBaseUnit>>();
        this.map_sprites = new Dictionary<int, IBaseUnit>();
        this.map_players = new Dictionary<int, IBaseUnit>();
        this.map_players_bysid = new Dictionary<int, IBaseUnit>();
        this.map_players_bycid = new Dictionary<int, IBaseUnit>();
        this.pk_seting = this.get_map_pkseting();

        this.immrespawn = map_conf.immrespawn == 1;

        this.map_dpitms = new Dictionary<int, map_item>();

        this.last_check_tm = 0;
        this.map_skills = new Dictionary<int, SkillData>();

        var monster_count = this.get_monster_desc_count();
        this.petmon_cache = new Dictionary<int, List<IBaseUnit>>();

        //this.tmtriggers = { };
        //this.areatriggers = { };

        //this.useitmtriggers = { };
        //this.mistriggers = { };
        //this.othertriggers = { };
        this.callmon_added = new Dictionary<int, int>();

        //this.add_npcs = [];
        this.add_links = new List<link_conf>();
        //this.mapstats = [];
        this.paths = new Dictionary<int, List<Point2D>>();
        //this.team_drop_itm = {};

        //this.temp_sid_ary = int_ary.create_int_ary();





        var i = 0;
        for (; i < monster_count; ++i)
        {
            int mid = this.map_conf.map_mon[i].mid;
            var mon_conf = Utility.get_monster_conf(mid);
            if (null == mon_conf)
                throw new Exception("monsterconf not found for mid:" + mid);

            var m = new Monster(mon_conf);

            m.gmap = this;
            m.on_pos_change(m.mondata.x, m.mondata.y);
            IMapUnit mon = m.get_pack_data();

            this.map_mons[mon.iid] = m;
            this.map_sprites[mon.iid] = m;

            List<IBaseUnit> mons = null;
            if (this.map_mon_bymid.TryGetValue(mon.mid, out mons))
            {
                mons.Add(m);
            }
            else
            {
                mons = new List<IBaseUnit>();
                mons.Add(m);
                this.map_mon_bymid[mon.mid] = mons;
            }
        }
    }

    public SkillData get_map_skill(int skid)
    {
        SkillData conf = null;
        this.map_skills.TryGetValue(skid, out conf);

        return conf;
    }

    public Point2D valpoint_on_round(double x, double y, int range, double alpha)
    {
        throw new NotImplementedException();
    }
    public static bool is_pt_pos_in_range(IMapUnit p1, IMapUnit p2, int range)
    {
        var distx = (p2.x / game_const.map_grid_pixel) - (p1.x / game_const.map_grid_pixel);
        var disty = (p2.y / game_const.map_grid_pixel) - (p1.y / game_const.map_grid_pixel);

        var dist = Math.Sqrt(distx * distx + disty * disty);

        return dist <= range;
    }


    public List<Point2D> find_path(int src_grid_x, int src_grid_y, int dest_grid_x, int dest_grid_y)
    {
        return this.grd.find_Path(src_grid_x, src_grid_y, dest_grid_x, dest_grid_y);
    }

    public List<Point2D> find_path(double src_grid_x, double src_grid_y, double dest_grid_x, double dest_grid_y)
    {
        return find_path((int)src_grid_x, (int)src_grid_y, (int)dest_grid_x, (int)dest_grid_y);
    }

    public bool is_grid_walkable(int grid_x, int grid_y)
    {
        return grd.is_grid_walkable(grid_x, grid_y);
    }

    public Point2D get_grid_by_pt(double x, double y)
    {
        int gx = (int)(x / 32);
        int gy = (int)(y / 32);

        return new Point2D(gx, gy);
    }

    public bool has_sprite_by_iid(int sprite_iid)
    {
        return this.map_sprites.ContainsKey(sprite_iid);
    }

    public IBaseUnit get_sprite_by_iid(int sprite_iid)
    {
        IBaseUnit u = null;
        this.map_sprites.TryGetValue(sprite_iid, out u);

        return u;
    }

    public bool has_player_by_cid(int cid)
    {
        return this.map_players_bycid.ContainsKey(cid);
    }

    public IBaseUnit get_player_by_cid(int cid)
    {
        IBaseUnit ply = null;
        this.map_players_bycid.TryGetValue(cid, out ply);

        return ply;
    }

    public IBaseUnit get_player_by_sid(int sid)
    {
        IBaseUnit ply = null;
        this.map_players_bysid.TryGetValue(sid, out ply);

        return ply;
    }

    /// <summary>
    /// 返回iid
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public int add_player(int sid, double x, double y, IBaseUnit sgplayer)
    {
        return _iid_generator.Next_Value();
    }

    public void add_player_to_map(IService game_ref, int clientid, IBaseUnit sgplayer)
    {
        // 中断打坐
        //sgplayer.try_stop_recover();

        long cur_tm_s = DateTime.Now.ToBinary();

        var players_info = game_ref.sgplayers;

        sgplayer.gmap = this;

        IMapUnit pl = sgplayer.get_pack_data();

        pl.iid = this.add_player(clientid, pl.x, pl.y, sgplayer);

        this.map_sprites[pl.iid] = sgplayer;
        this.map_players[pl.iid] = sgplayer;

        this.map_players_bysid[clientid] = sgplayer;
        this.map_players_bycid[pl.cid] = sgplayer;

        if (this.blvlmap)
        {   //玩家在副本切换地图通知
            game_ref.ply_change_map(sgplayer, this);
        }
        // send map change msg
        //var map_change_data = {mpid=pl.map_id, iid=pl.iid, x=pl.x, y=pl.y,
        //                        speed=pl.speed, maxhp=pl.max_hp, maxmp=pl.max_mp,in_pczone=pl.in_pczone};

        //sys.dumpobj(dpitms);

        if (this.map_dpitms.Count > 0)
        {
            List<int> removed_items = new List<int>();
            foreach (var drop_item in map_dpitms)
            {
                drop_item.Value.left_tm = drop_item.Value.dis_tm - cur_tm_s;
                if (drop_item.Value.left_tm <= 0)
                    removed_items.Add(drop_item.Key);
            }
            foreach (int k in removed_items)
                this.map_dpitms.Remove(k);

            //map_change_data.dpitms < -this.map_dpitms;
        }
        //::send_rpc(clientid, 58, map_change_data);

        //// 尝试召唤战斗宠物
        //if (pl.pet_mon.mid > 0)
        //{
        //    if (pl.pet_mon.fintm > cur_tm_s)
        //    {
        //        sgplayer.call_petmon(pl.pet_mon.mid, pl.pet_mon.fintm);
        //    }
        //    else
        //    {
        //        pl.pet_mon.mid = 0;
        //        pl.pet_mon.fintm = 0;
        //    }
        //}

    }



    public void rmv_player_from_map(IService game_ref, int clientid)
    {
        //sys.trace(sys.SLT_SYS,"map ["+this.mapid+"] remove sid ["+clientid+"]\n");

        IBaseUnit ply = null;
        if (!game_ref.sgplayers.TryGetValue(clientid, out ply))
            return;

        IMapUnit pl = ply.get_pack_data();
        var iid = pl.iid;

        var cid = pl.cid;
        var sid = pl.sid;

        // 中断打坐
        //ply.try_stop_recover();



        // 尝试清除角色在地图上创建的攻击宠物对象实例
        ply.release_petmon(false, false);

        ply.gmap = null;

        IBaseUnit spr = this.get_sprite_by_iid(iid);

        if (spr == null)
        {
            Utility.trace_err("map [" + this.mapid + "] remove sid [" + clientid + "] iid [" + iid + "] not in map!\n");
        }

        IMapUnit ply_pl = ply.get_pack_data();

        if (spr != null)
        {
            IMapUnit spr_pl = spr.get_pack_data();
            if (spr.get_sprite_type() == map_sprite_type.MstPlayer && spr_pl.cid == ply_pl.cid)
            {
                // notify observer
                var plys = spr.get_inz_plys();
                var mons = spr.get_inz_mons();
                if (plys.Count > 0)
                {
                    foreach (var ply_x in plys)
                    {
                        ply_x.rmv_zone_player(clientid);
                    }
                    //svr.mul_snd_rpc(spr.get_inz_ply_sids(), spr.get_inz_ply_cnt(), 56, { iidary =[iid]});   // broad_cast player leave zone
                }
                if (mons.Count > 0)
                {
                    foreach (var mon in mons)
                    {
                        mon.rmv_zone_player(clientid);
                    }
                }

                //Utility.trace_info("player with sid["+clientid+"] iid["+iid+"] leave zone with plys.len["+plys.Count+"] mons.len["+mons.Count+"]\n");

                spr.gmap = null;
                spr.cancel_atk();

                spr_pl.jumping = null;
                spr_pl.casting = null;
                spr_pl.teleping = null;
                spr_pl.holding = null;
                spr_pl.moving = null;


                //this.rmv_player(spr);

                this.map_sprites.Remove(iid);
                this.map_players.Remove(iid);
            }
            else
            {
                // 切换地图时断线可能导致player的map_id已经修改，但是，目标地图尚未添加角色，从而引起该问题
                Utility.trace_err("map [" + this.mapid + "] remove sid [" + clientid + "] iid [" + iid + "] not player or not same player!\n");
            }
        }

        this.map_players_bycid.Remove(cid);
        this.map_players_bysid.Remove(sid);
    }


    public static bool _update_pl_tracing(IBaseUnit sprite, Variant trace, int frang, int trang, long cur_clock_tm)
    {
        var can_reach = true;

        var pl = sprite.get_pack_data();

        // 1秒搜索2次目标
        if (trace["trace_tm_left"]._int32 > 0 || cur_clock_tm - sprite.last_trace_target_tm > 500)
        {
            var target = sprite.gmap.map_sprites[trace["tar_iid"]._int32];
            var tar_pl = target.get_pack_data();

            double dist_x = pl.x - tar_pl.x;
            double dist_y = pl.y - tar_pl.y;
            double dist2 = dist_x * dist_x + dist_y * dist_y;

            if (dist2 > frang)
            {
                // 不在攻击范围，追击
                if (pl.speed > 0)
                {
                    Point2D cur_grid_pos = new Point2D()
                    {
                        x = (pl.x / game_const.map_grid_pixel),
                        y = (pl.y / game_const.map_grid_pixel)
                    };
                    Point2D tar_grid_pos = new Point2D()
                    {
                        x = (tar_pl.x / game_const.map_grid_pixel),
                        y = (tar_pl.y / game_const.map_grid_pixel)
                    };

                    var path = sprite.gmap.find_path(cur_grid_pos.x, cur_grid_pos.y, tar_grid_pos.x, tar_grid_pos.y);

                    // 在路径中删除当前格子
                    if (path != null && path.Count > 0)
                        path.pop();

                    if (path != null && path.Count > 0)
                    {
                        var i = 0;
                        for (; i < path.Count; ++i)
                        {
                            var pt = path[i];
                            var to_x = pt.x * game_const.map_grid_pixel + 16;
                            var to_y = pt.y * game_const.map_grid_pixel + 16;

                            dist_x = to_x - tar_pl.x;
                            dist_y = to_y - tar_pl.y;
                            if (dist_x * dist_x + dist_y * dist_y > trang)
                            {
                                break;
                            }
                        }

                        //Utility.trace_err("monster["+this.mondata.iid+"] trace tar_iid[" + trace.tar_iid +"] break on i[" +i+"]\n");

                        if (i == path.Count)
                        {
                            // TO DO : in attack range?
                            // 转化之后的格子坐标在攻击范围内，当前像素位置不在攻击范围之内！
                            var pt = path.pop();
                            path.Clear();
                            path.push(pt);
                        }
                        else if (i > 0)
                        {
                            path = path.slice(i - 1);
                        }
                        else
                        {
                            // 移动到路劲终点亦无法攻击对象？！
                            can_reach = false;
                        }

                        if (path.Count > 0 && (path[0].x.CompareTo(cur_grid_pos.x) > 0 || path[0].y.CompareTo(cur_grid_pos.y) > 0))
                        {
                            // 移动
                            var start_tm = cur_clock_tm - trace["trace_tm_left"]._int32;
                            //Utility.trace_err("sprite ["+pl.iid+"] trace tar_iid[" + trace.tar_iid +"] cur tm["+ cur_clock_tm +"] start tm[" +start_tm+"]\n");

                            pl.moving = new moving() { start_tm = start_tm, pts = path, to_x = path[0].x, to_y = path[0].y, float_x = (pl.x), float_y = (pl.y) };

                            //var data = {start_tm=pl.moving.start_tm, iid=pl.iid, frm_x=pl.x, frm_y=pl.y, to_x=path[0].x, to_y=path[0].y};
                            //// send move msg to clients
                            ////gmap.broadcast_map_rpc(9, data);
                            //sprite.broad_cast_zone_msg_and_self(9, data);
                        }
                    }
                    else
                    {
                        // TO DO : path not found?
                        // delete this.mondata.atking;
                        can_reach = false;
                    }
                }
            }
            // 在攻击范围,
            else if (dist2 <= trang)
            {
                // 在攻击范围,
                if (pl.moving != null)
                {
                    // 停止移动
                    //var cur_grid_pos  = {x=(pl.x/game_const.map_grid_pixel), y=(pl.y/game_const.map_grid_pixel)}
                    //var data = {tm=cur_clock_tm- trace.trace_tm_left, iid=pl.iid, x=pl.x, y=pl.y, face=pl.face};
                    //// send stop move msg to clients
                    ////gmap.broadcast_map_rpc(10, data);
                    //sprite.broad_cast_zone_msg_and_self(10, data);

                    //delete pl.moving;
                    pl.moving = null;
                }
            }

            sprite.last_trace_target_tm = cur_clock_tm;
        }

        return can_reach;
    }

    public static void update_pl_atk_tracing(IBaseUnit sprite, long cur_clock_tm)
    {
        // attack target

        // TO DO : 追击问题？
        // 追击情况下，攻击者的移动以被攻击者的位置为目标，当被攻击者移动时，攻击者的路径将随时变化，如何同步才能保证客户端与服务器位置同步且通信效率最优？

        var pl = sprite.get_pack_data();
        if (pl.atking != null)
        {
            return;
        }

        if (sprite.has_state(pl_state_type.PST_CANT_MOVE))
        {
            // can't move
            return;
        }

        var gmap = sprite.gmap;

        if (gmap.map_sprites.ContainsKey(pl.atking.tar_iid))
        {
            var atkrang = gmap.map_sprites[pl.atking.tar_iid].get_pack_data().size + pl.atkrange + pl.size;
            atkrang = atkrang * atkrang;
            _update_pl_tracing(sprite, pl.atking.ToVariant(), atkrang, atkrang, cur_clock_tm);
        }
        else
        {
            //delete pl.atking;
            pl.atking = null;
        }
    }

    public static void update_pl_follow_tracing(IBaseUnit sprite, long cur_clock_tm)
    {
        var pl = sprite.get_pack_data();
        if (pl.follow == null)
        {
            return;
        }

        if (sprite.has_state(pl_state_type.PST_CANT_MOVE))
        {
            // can't move
            return;
        }

        var gmap = sprite.gmap;

        if (gmap.map_sprites.ContainsKey(pl.follow.tar_iid))
        {
            _update_pl_tracing(sprite, pl.follow.ToVariant(), pl.follow.frang, pl.follow.trang, cur_clock_tm);
        }
        else
        {
            //delete pl.follow;
            pl.follow = null;
        }
    }

    //当单位配置中 keepdist = 1 时，攻击时将尽量保持与目标间的距离为自己的攻击距离
    public static void update_keep_atk_range(IBaseUnit sprite, long tm_elasped_s)
    {
        var pl = sprite.get_pack_data();
        if (pl.atking == null)
        {
            return;
        }

        if (sprite.has_state(pl_state_type.PST_CANT_MOVE))
        {
            // can't move
            return;
        }

        var gmap = sprite.gmap;

        if (gmap.has_sprite_by_iid(pl.atking.tar_iid))
        {
            //Utility.trace_err("monster["+this.mondata.iid+"] atking tar_iid[" + this.mondata.atking.tar_iid +"] cur tm[" +cur_clock_tm+"] last_trace_tm ["+ last_trace_target_tm +"] \n");

            var target = gmap.map_sprites[pl.atking.tar_iid];
            var tar_pl = target.get_pack_data();

            double dist_x = pl.x - tar_pl.x;
            double dist_y = pl.y - tar_pl.y;
            var dist2 = dist_x * dist_x + dist_y * dist_y;
            var dist = Math.Sqrt(dist2);
            var atk_rang2 = pl.atkrange * pl.atkrange;


            // 处于最佳攻击距离
            var best_atk_rang = pl.atkrange;
            if (dist.CompareTo(best_atk_rang) <= 0)
            {
                //Utility.trace_info("atk_trace stop:"+dist2+"="+atk_rang2+"\n");
                if (pl.moving != null)
                {
                    //// 停止移动
                    ////var cur_grid_pos  = {x=(pl.x/game_const.map_grid_pixel), y=(pl.y/game_const.map_grid_pixel)}
                    //var data = {tm=sys.clock_time()- pl.atking.trace_tm_left, iid=pl.iid, x=pl.x, y=pl.y, face=pl.face};
                    //// send stop move msg to clients
                    ////gmap.broadcast_map_rpc(10, data);
                    //sprite.broad_cast_zone_msg_and_self(10, data);

                    //delete pl.moving;
                    pl.moving = null;
                }
            }
            else
            {
                //if(dist > best_atk_rang)
                //{
                //    Utility.trace_info("atk_trace:"+dist+">"+best_atk_rang+"\n");
                //}
                //else if(dist < best_atk_rang)
                //{
                //    Utility.trace_info("atk_trace:"+dist+"<"+best_atk_rang+"\n");
                //}
                // 保持距离
                //var cur_pos = gmap.get_grid_by_pt(pl.x, pl.y);
                //var tar_pos = gmap.get_grid_by_pt(tar_pl.x, tar_pl.y);
                //var k = (best_atk_rang / dist);
                //var best_pos = {};
                //best_pos.x <- (tar_pos.x + (cur_pos.x - tar_pos.x) * k);
                //best_pos.y <- (tar_pos.y + (cur_pos.y - tar_pos.y) * k);

                // 计算和目标之间的角度atan(a)，范围为[-PI/2, PI/2]，为了获得整个圆周的范围，当(x<0)时角度为：PI+atan(a)
                /*      
                 *      |
                 * (-,-)|(+,-)
                 * -----O------>X
                 * (-,+)|(+,+)
                 *      |
                 *      \/Y
                 */
                double alpha = 0;
                if (dist != 0)
                {
                    alpha = Math.PI / 2;
                    if (dist_x != 0)
                    {
                        alpha = Math.Atan((dist_y / dist_x));
                        if (dist_x < 0)
                        {
                            alpha += Math.PI;
                        }
                    }
                    else if (dist_y < 0)
                    {
                        alpha = -Math.PI / 2;
                    }
                }
                //Utility.trace_info("atan("+dist_y+"/"+dist_x+"="+k+")="+alpha+"\n");
                // 在最佳攻击距离圈上选取最近可达点
                var new_pos = gmap.valpoint_on_round(tar_pl.x, tar_pl.y, best_atk_rang, alpha);
                if (new_pos != null)
                {
                    new_pos = gmap.get_grid_by_pt(new_pos.x, new_pos.y);
                    sprite._move_to(new_pos.x, new_pos.y);
                }
                //Utility.trace_info("keep range pos:("+tar_pos.x+","+tar_pos.y+","+best_atk_rang+")->("+new_pos.x+","+new_pos.y+")\n");
            }
        }
    }

    public static void _hold_pl_atk(IMapUnit pl, long cd_tm, long cur_clock_tm)
    {
        if (cur_clock_tm - pl.atking.start_tm > cd_tm)
        {
            pl.atking.start_tm = cur_clock_tm - cd_tm;
        }
    }
    public static void update_pl_atk(IBaseUnit sprite, long cur_clock_tm)
    {
        var pl = sprite.get_pack_data();
        if (pl.atking == null)
            return;

        var cd_tm = sprite.get_atk_cd();

        if (pl.casting != null)
        {
            // 吟唱中，不更新攻击
            _hold_pl_atk(pl, cd_tm, cur_clock_tm);
            return;
        }

        if (pl.holding != null)
        {
            // 聚气中，不更新攻击
            _hold_pl_atk(pl, cd_tm, cur_clock_tm);
            return;
        }

        if (sprite.has_state(pl_state_type.PST_CANT_ATTACK))
        {
            // can't attack
            _hold_pl_atk(pl, cd_tm, cur_clock_tm);
            return;
        }

        //Utility.trace_err("update [" + pl.iid +"] attack \n");

        if (!sprite.gmap.has_sprite_by_iid(pl.atking.tar_iid))
        {
            // 目标不在同一地图
            sprite.cancel_atk();
            // TO DO : send msg
            return;
        }

        var tar_sprite = sprite.gmap.map_sprites[pl.atking.tar_iid];
        var tar_pl = tar_sprite.get_pack_data();

        if (sprite.isghost() || tar_sprite.isghost())
        {
            // 对方是灵魂状态，停止攻击
            sprite.cancel_atk();
            return;
        }

        if (tar_sprite.ignore_dmg())
        {
            _hold_pl_atk(pl, cd_tm, cur_clock_tm);
            return;
        }

        if (cur_clock_tm < pl.skill_gen_cd)
        {
            // 与技能共cd
            _hold_pl_atk(pl, cd_tm, cur_clock_tm);
            return;
        }

        //Utility.trace_err("[" + pl.iid +"] attack ["+pl.atking.tar_iid+"] cur tm: ["+cur_clock_tm+"] atkstarttm["+pl.atking.start_tm+"]\n");

        // TO DO : update face

        var tm_interval = cur_clock_tm - pl.atking.start_tm;

        //Utility.trace_err("[" + pl.iid +"] attack ["+pl.atking.tar_iid+"] tm_interval: ["+tm_interval+"] cd_tm ["+cd_tm+"]\n");

        if (tm_interval < cd_tm)
        {
            // 没到攻击间隔
            return;
        }

        if (tar_pl.invisible > pl.observer)
        {
            // 看不到目标

            if (sprite.get_sprite_type() == map_sprite_type.MstMonster)
            {
                //delete pl.atking; // 停止攻击
                pl.atking = null;
            }
            else
            {
                _hold_pl_atk(pl, cd_tm, cur_clock_tm);
            }

            return;
        }

        //Utility.trace_err("pl.x [" + pl.x +"] pl.y ["+pl.y+"] tar_pl.x: ["+tar_pl.x+"] tar_pl.y: ["+tar_pl.y+"]\n");

        //Utility.trace_err("dist_x [" + dist_x +"] dist_y ["+dist_y+"] atkrange: ["+pl.atkrange+"]\n");

        var dist_x = tar_pl.x - pl.x;
        var dist_y = tar_pl.y - pl.y;
        var range = pl.atkrange + pl.size + tar_pl.size + 32; // 允许1格误差
        if (dist_x * dist_x + dist_y * dist_y > range * range)
        {
            // 不在攻击范围内
            _hold_pl_atk(pl, cd_tm, cur_clock_tm);
            return;
        }

        var atkcnt_trigered = false;
        var added_state = false;
        if (pl.states != null)
        {
            // 检查按攻击次数结算状态
            foreach (var val in pl.states.state_par)
            {
                if (val.atkedcnt < 0 || val.desc.atkcnt == null)
                    continue;

                ++val.atkedcnt;

                //Utility.trace_info("val.atkedcnt["+val.atkedcnt+"] val.desc.atkcnt.cnt["+val.desc.atkcnt.cnt+"]")

                if (val.atkedcnt < val.desc.atkcnt.cnt)
                {
                    continue;
                }

                //sys.dumpobj(val);

                // 触发结算状态
                atkcnt_trigered = true;

                if (val.desc.atkcnt.add_stat > 0)
                {
                    var state_obj = Skill.add_state_to_pl(cur_clock_tm, tar_sprite, new tres_conf(val.desc.atkcnt.add_stat), sprite, val.per, false);
                    if (state_obj != null)
                    {
                        added_state = true;
                        // broadcast add state msg;
                        //tar_sprite.broad_cast_zone_msg_and_self(24, { iid = tar_pl.iid, states =[state_obj]});
                    }
                }

                if (val.desc.atkcnt.dmg > 0)
                {
                    apply_dmg_on_pl(tar_sprite, sprite, new damage() { hp_dmg = val.desc.atkcnt.dmg }, cur_clock_tm, val.par);
                }
            }
        }

        if (pl.invisible > 0)
        {
            // 攻击就现身
            Skill.rmv_state_from_pl_by_att("invisible", sprite, pl);
        }

        //if(sprite.get_sprite_type()==map_sprite_type.MstPlayer)
        //{
        //    // 中断打坐
        //    sprite.try_stop_recover();
        //}

        tar_sprite.atk_by(sprite);

        if (atkcnt_trigered)
        {
            // 触发了攻击次数结算状态，该次攻击失效

            if (added_state)
            {
                Skill._remark_pl_state(tar_sprite, tar_pl);
            }
            if (pl.atking != null)
                pl.atking.start_tm = cur_clock_tm;

            return;
        }

        // 计算攻击次数
        var atk_count = (tm_interval / cd_tm);
        var atk_tm_left = tm_interval % cd_tm;
        var total_dmg = 0;

        pl.atking.start_tm = cur_clock_tm - atk_tm_left;
        pl.last_atk_tm = pl.atking.start_tm;

        pl.skill_gen_cd = pl.atking.start_tm + cd_tm; // 触发技能cd
        pl.skill_gen_cdst = pl.atking.start_tm; // 触发技能cd起始

        var real_cd_tm = pl.skill_gen_cd - cur_clock_tm;
        if (real_cd_tm < 0) real_cd_tm = 0;

        // 判断对象是否攻击免疫
        if (tar_sprite.has_state(pl_state_type.PST_ATK_AVOID))
        {
            // can't be attack
            // send avoid attack msg
            //Utility.trace_err("[" + pl.iid +"] attack ["+pl.atking.tar_iid+"] target avoid attack\n");
            //sprite.broad_cast_zone_msg_and_self(18, { hited = 0, frm_iid = pl.iid, to_iid = pl.atking.tar_iid});
            return;
        }
        //::g_dump( " update_pl_atk()  ===>>>> ", "") ;
        var ret = apply_dmg_on_pl_ex(tar_sprite, sprite, new damage() { dmg_min = 0, dmg_max = 0, noratk = 1, hp_dmg = 0, mp_dmg = 0 }, cur_clock_tm, (int)atk_count * 1000, true, true);

        //ret.atkcount < -atk_count;
        //ret.cdtm < -real_cd_tm;

        // send attack msg
        sprite.broad_cast_zone_msg_and_self(18, ret);
        if (tar_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
        {
            //目标不在视野  需要补发消息
            //    if (!sprite.is_ply_inzone(tar_pl.sid))
            //    {
            //::send_rpc(tar_pl.sid, 18, ret);
            //    }
        }
    }

    //默认的单位属性，用于在取不到单位对象的时候计算伤害值
    public const Variant g_dmg_calc_def_pl = null;
    //    {
    //    level = 1,
    //    atktp = atk_type.ATKTP_ATK,

    //    atk_min = 100,
    //    atk_max = 100,
    //    matk_min = 100,
    //    matk_max = 100,
    //    atk_rate = 1000, // 命中率
    //    miss_rate = 0,  // 闪避
    //    pkatkrate = 100,
    //    pkmisrate = 100,

    //    criatk = 0,
    //    criatk_debuffs = 0, //暴击抵抗，相对 criatk 的抵抗 ，千分比， 算法：受到暴击击的概率降低xx 
    //    cridmg = 0,
    //    exatk = 0,
    //    exatk_debuffs = 0, // 卓越一击率抵抗, 相对 exatk 的抵抗 ，千分比， 算法：受到卓越一击的概率降低xx
    //    exdmg = 0,
    //    dobrate = 0,
    //    igdef_rate = 0,
    //    igdef_rate_debuffs = 0, // 抵抗无视防御几率
    //    def_red = 0,
    //    dmg_red = 0,
    //    atk_dmg_mul = 0,
    //    igdmg_red = 0,  // 无视伤害减免几率，千分比
    //    igatk_dmg_mul = 0,// 无视伤害增益几率，千分比
    //    hpsuck = 0,     // 每次攻击吸收生命值
    //    hpsuck_dmgmul = 0, // 每次攻击吸收生命值增加伤害比例
    //    addResist = [0,0,0,0,0,0,0], // 属性攻击
    //    clansred = 0,
    //    iid = 0,
    //};

    // damage { noratk: } ??
    public static Variant apply_dmg_on_pl_ex(IBaseUnit tar_sprite, IBaseUnit frm_sprite, damage damage, long cur_clock_tm, int per, bool wpnatk = false, bool direct_dmg = false)
    {
        // TO DO : 优化： per = 1000时不需要计算增益
        //direct_dmg 默认伤害....? 技能伤害, 追击伤害, 有miss几率
        //wpnatk 是否默认攻击

        //::g_dump( "apply_dmg [ damage ] ===>>>> ",  damage ) ;
        //::g_dump( "apply_dmg [ flags ] ===>>>> ",  { wpnatk = wpnatk, direct_dmg = direct_dmg } ) ;
        if (frm_sprite != null)
        {
            frm_sprite.on_hurt(tar_sprite);
        }

        tar_sprite.on_be_hurt(frm_sprite);

        var tar_pl = tar_sprite.get_pack_data();

        // 取不到状态添加对象情况下，本函数frm_sprite可能为空，则需要默认的计算参数
        var pl = default(IMapUnit);// g_dmg_calc_def_pl;

        if (frm_sprite != null)
        {
            pl = frm_sprite.get_pack_data();
            pl.last_atk_tm = cur_clock_tm; // 触发普通攻击cd
        }

        var ret = new Variant();
        ret["hited"] = 3;
        ret["criatk"] = 0;
        ret["dmg"] = 0;
        ret["dobdmg"] = false;
        ret["isdie"] = false;
        ret["frm_iid"] = pl.iid;
        ret["to_iid"] = tar_pl.iid;


        // 计算玩家 VS NPC 等级调整伤害
        //var dmg_per_add = 0;
        var is_pvp = true;
        if (frm_sprite != null)
        {
            if (frm_sprite.get_sprite_type() == map_sprite_type.MstMonster)
            {
                is_pvp = false;
            }
        }

        if (tar_sprite.get_sprite_type() == map_sprite_type.MstMonster)
        {
            is_pvp = false;
        }

        // miss check
        var bAllMiss = false;
        if (direct_dmg)
        {
            if (is_pvp)
            {
                // pvp
                var atk_rate = 100 * (pl.pkatkrate / (pl.pkatkrate + tar_pl.pkmisrate)/* * (pl.level/(pl.level + tar_pl.level))*/);
                var lvl_dist = tar_pl.level - pl.level;
                if (lvl_dist >= 300)
                {
                    atk_rate -= 15;
                }
                else if (lvl_dist >= 200)
                {
                    atk_rate -= 10;
                }
                else if (lvl_dist >= 100)
                {
                    atk_rate -= 5;
                }

                var judg = Utility.random(0, 100);
                if (judg > atk_rate)
                {
                    // miss
                    ret["hited"] = 1;
                    return ret;
                }
            }
            else
            {
                // pve
                var atk_rate = 100.0 * (pl.atk_rate - tar_pl.miss_rate) / pl.atk_rate;
                if (atk_rate < 5)
                {
                    if (atk_rate < 0)
                    {
                        bAllMiss = true;
                    }
                    atk_rate = 5;
                }

                var judg = Utility.random(0, 100);
                if (judg > atk_rate)
                {
                    // miss
                    ret["hited"] = 1;
                    return ret;
                }
            }
        }

        double targetDef = tar_pl.def;
        if (is_pvp) targetDef += tar_pl.pkdef;

        targetDef = (targetDef - targetDef * (pl.def_red / 1000.0));




        var igdef_rate = pl.igdef_rate - tar_pl.igdef_rate_debuffs;
        if (igdef_rate > 0)
        {
            var judg = Utility.random(0, 1000);
            if (judg < igdef_rate)
            {
                targetDef = 0;
                ret["criatk"] = 1; // 无视防御
            }
        }


        //Utility.trace_info("apply_dmg_on_pl frm_sprite:["+frm_sprite+"] per["+per+"]\n");
        //sys.dumpobj(pl);

        //var combat_conf = get_general_combat_conf();

        double total_dmg = 0;

        // TO DO : 计算伤害 damage 是否是物理伤害
        if (pl.atktp == atk_type.ATKTP_MATK && !wpnatk && (damage.plyatk != 1))
        {
            //pl.atktp == atk_type.ATKTP_MATK  是 魔攻攻击分类角色
            // !wpnatk  不是默认攻击
            //  (!("plyatk" in damage) || damage.plyatk!=1) 不是物理攻击

            // 根据魔法攻击计算伤害

            int dmg_min = damage.dmg_min + pl.matk_min * damage.noratk;
            int dmg_max = damage.dmg_max + pl.matk_max * damage.noratk;
            if (is_pvp && pl.pkmatk != 0)
            {//pvp魔法攻击增加
                dmg_min += pl.pkmatk;
                dmg_max += pl.pkmatk;
            }

            if (damage.attr >= 0)
            {// 属性攻击
                dmg_min += pl.addResist[damage.attr];
                dmg_max += pl.addResist[damage.attr];
            }

            // TO DO : 根据装备武器不同来计算伤害
            //if(direct_dmg && frm_sprite)
            //{
            //    //frm_sprite.get_equip_by_pos(6); // 获取装备的武器
            //}

            double dmg = 0;
            var dmg_calced = false;

            if (damage.noratk > 0 && (pl.criatk - pl.criatk_debuffs > 0 || pl.exatk - pl.exatk_debuffs > 0))
            {
                var judg = Utility.random(0, 1000);
                if (judg < pl.exatk - pl.exatk_debuffs)
                {//卓越一击几率
                    dmg = dmg_max - targetDef;
                    dmg += dmg_max * (0.2 + pl.exper_add / 1000.0);
                    dmg += pl.exdmg;

                    ret["criatk"] = 2; // 卓越一击

                    dmg_calced = true;
                }
                else
                {
                    judg = Utility.random(0, 1000);
                    if (judg < pl.criatk - pl.criatk_debuffs)
                    {// 会心一击几率
                        dmg = dmg_max - targetDef;
                        dmg += pl.cridmg;

                        ret["criatk"] = 3; // 会心一击

                        dmg_calced = true;
                    }
                }
            }

            if (!dmg_calced)
            {
                dmg = Utility.random(dmg_min, dmg_max + 1);

                //if(is_pvp)
                //{
                //    dmg = dmg - (targetDef*tar_sprite.game_conf.pvp_def_per/100.0);
                //}
                //else
                //{
                //    dmg = dmg - targetDef;
                //}      
            }

            if (frm_sprite != null && frm_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
            {
                //// 技能攻击，技能额外加成
                //int dmg_mul = damage["noratk"];
                //var baseatt_conf = frm_sprite.carr_gen_conf.baseatt[0];
                //if( ("inte2atk" in baseatt_conf) && baseatt_conf.inte2atk != 0 )
                //{
                //    dmg_mul += pl.inte/(pl.inte+baseatt_conf.inte2atk);
                //}
                ////sys.trace( sys.SLT_DETAIL, "atk_type.ATKTP_MATK  dmg_mul=" + dmg_mul + " baseatt_conf.inte2atk =" + baseatt_conf.inte2atk + "\n")
                //dmg = dmg * dmg_mul;
            }

            if (dmg <= 0)
            {
                dmg = 1;
            }

            total_dmg += dmg;
        }
        else
        {
            // 根据攻击力计算伤害

            var skil_dmg_min = damage.dmg_min + pl.atk_min;
            var skil_dmg_max = damage.dmg_max + pl.atk_max;
            if (is_pvp && pl.pkatk != 0)
            {
                skil_dmg_min += pl.pkatk;
                skil_dmg_max += pl.pkatk;
            }

            if (damage.attr >= 0)
            {
                skil_dmg_min += pl.addResist[damage.attr];
                skil_dmg_max += pl.addResist[damage.attr];
            }

            double dmg = 0;
            var dmg_calced = false;

            if (damage.noratk > 0 && (pl.criatk - pl.criatk_debuffs > 0 || pl.exatk - pl.exatk_debuffs > 0))
            {
                var judg = Utility.random(0, 1000);
                if (judg < pl.exatk - pl.exatk_debuffs)
                {
                    dmg = skil_dmg_max;
                    dmg += skil_dmg_max * (0.2 + pl.exper_add / 1000.0);
                    dmg += pl.cridmg;
                    dmg += pl.exdmg;

                    ret["criatk"] = 2; // 卓越一击

                    dmg_calced = true;
                }
                else
                {
                    judg = Utility.random(0, 1000);
                    if (judg < pl.criatk - pl.criatk_debuffs)
                    {
                        dmg = skil_dmg_max;
                        dmg += pl.cridmg;

                        ret["criatk"] = 3; // 会心一击

                        dmg_calced = true;
                    }
                }
            }

            if (!dmg_calced)
            {
                dmg = Utility.random(skil_dmg_min, skil_dmg_max + 1);
            }

            //if(is_pvp)
            //{
            //    dmg = dmg -  (targetDef*tar_sprite.game_conf.pvp_def_per/100.0);
            //}
            //else
            //{
            //    dmg = dmg - targetDef;
            //}      

            if (!wpnatk && frm_sprite != null && frm_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
            {
                // 物理技能攻击，技能额外加成
                var dmg_mul = damage.noratk;
                //var baseatt_conf = frm_sprite.carr_gen_conf.baseatt[0];
                //if( ("inte2atk" in baseatt_conf) && baseatt_conf.inte2atk != 0 )
                //{
                //    dmg_mul += pl.inte/(pl.inte+baseatt_conf.inte2atk);
                //}
                //sys.trace( sys.SLT_DETAIL, "atk_type.ATKTP_ATK  dmg_mul=" + dmg_mul + "\n")
                dmg = dmg * dmg_mul;
            }

            if (dmg <= 0)
            {
                dmg = 1;
            }

            total_dmg += dmg;
        }

        if (bAllMiss)
        {//pve中 攻击者攻击几率 小于 被攻击者 miss几率时，攻击减益到30%
            total_dmg = (total_dmg * 30) / 100;
        }

        if (tar_pl.dmg_red != 1)
        {//伤害减免
            if (pl.igdmg_red <= 0)
            {   //无视伤害减免几率
                total_dmg = (total_dmg * tar_pl.dmg_red);
            }
            else
            {
                var judg = Utility.random(0, 1000);
                if (judg >= pl.igdmg_red)
                {
                    total_dmg = (total_dmg * tar_pl.dmg_red);
                }
            }
        }
        if (is_pvp && tar_pl.pkdmg_red != 1)
        { //pvp 伤害减免
            total_dmg = (total_dmg * tar_pl.pkdmg_red);
        }

        var tlevel = pl.level / 10;
        if (total_dmg >= 0 && total_dmg < tlevel)
        {
            if (tlevel < 1)
            {
                tlevel = 1;
            }

            total_dmg = tlevel;
        }

        // TO DO : sprite damage, 小精灵伤害

        if (direct_dmg)
        {
            // 计算生命损耗伤害加成
            if (frm_sprite != null && pl.hpsuck > 0)
            {
                // 损耗自己生命
                if (pl.hp > pl.hpsuck)
                {
                    pl.hp -= pl.hpsuck;
                    if (pl.hp < 0) pl.hp = 0;

                    // broadcast hpchange msg
                    //frm_sprite.broad_cast_zone_msg_and_self(26, {iid=pl.iid, hpchange={hpadd=-pl.hpsuck, die=(pl.hp <= 0), frm_iid=pl.iid}});

                    //if(pl.cid >0)
                    //{
                    //    var tid = team.get_ply_teamid(pl.cid);
                    //    if(tid > 0)
                    //    {
                    //        // 在队伍中，发送队友属性变化消息
                    //        team_bcast_rpc_except(tid, 138, {cid=pl.cid, hp=pl.hp}, pl.sid);
                    //    }
                    //}

                    if (pl.hpsuck_dmgmul > 0)
                    {
                        // 增加攻击伤害
                        total_dmg = total_dmg * ((1000.0 + pl.hpsuck_dmgmul) / 1000.0);
                    }
                }
            }
            var atk_dmg_mul = pl.atk_dmg_mul;
            if (pl.igatk_dmg_mul > 0)
            {   //无视伤害增益几率
                var judg = Utility.random(0, 1000);
                if (judg < pl.igatk_dmg_mul)
                {
                    atk_dmg_mul = 0;
                }
            }
            if (is_pvp)
            {
                total_dmg += (total_dmg * ((atk_dmg_mul + pl.pkatk_dmg_mul) / 1000.0));
            }
            else
            {
                total_dmg += (total_dmg * (atk_dmg_mul / 1000.0));
            }

            //// 计算技能伤害的属性加成值
            //if(damage.ContainsKey("attdmg_mul"))
            //{
            //    foreach(var attdmg in damage["attdmg_mul"])
            //    {
            //        var val = 0;
            //        if(attdmg.name in pl)
            //        {
            //            val = pl[attdmg.name];
            //        }
            //        total_dmg *= ((attdmg.base + val * attdmg.val ) / attdmg.basediv);
            //    }
            //}
            //if("attdmg_add" in damage)
            //{
            //    foreach(attdmg in damage.attdmg_add)
            //    {
            //        var val = 0;
            //        if(attdmg.name in pl)
            //        {
            //            val = pl[attdmg.name];
            //        }
            //        total_dmg += ((attdmg.base + val * attdmg.val ) / attdmg.basediv);
            //    }
            //}


            //if((tar_sprite.get_sprite_type()==map_sprite_type.MstMonster) && tar_pl.clanatt_eff)
            //{
            //    // 需要计算副本中帮派科技伤害减免
            //    var clanper = 1000 - tar_pl.clanred + pl.clansred;

            //    //Utility.trace_info("calc clanatt total_dmg["+total_dmg+"] clanper["+clanper+"] pl.clansred["+pl.clansred+"] tar_pl.clanred["+tar_pl.clanred+"]\n");

            //    total_dmg = (total_dmg * ((clanper)/1000.0));

            //    //Utility.trace_info("calc clanatt total_dmg["+total_dmg+"]\n");
            //}

            // TO DO : 吸蓝 mpsteal
        }

        total_dmg += damage.hp_dmg;

        total_dmg = (total_dmg * (per / 1000.0));
        //sys.dumpobj(damage);
        //Utility.trace_info("total_dmg:["+total_dmg+"]\n");

        if (total_dmg.CompareTo(0) > 0)
        {
            var real_atk_dmg = total_dmg;

            if (total_dmg > 0)
            {
                if (frm_sprite != null)
                {
                    tar_sprite.onhate(frm_sprite, total_dmg);
                    tar_sprite.ondmg(frm_sprite, total_dmg);

                    if (frm_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
                    {
                        if (frm_sprite.gmap.blvlmap)
                        {
                            frm_sprite.gmap.worldsvr.on_dmg(frm_sprite, tar_sprite, total_dmg);
                        }
                    }

                    if (direct_dmg)
                    {
                        frm_sprite.calc_weapon_dura(total_dmg); // 计算装备耐久损耗
                        frm_sprite.on_make_direct_dmg();
                    }
                }

                if (direct_dmg)
                {
                    tar_sprite.calc_eqp_dura(total_dmg); // 计算装备耐久损耗

                    // 计算双倍伤害
                    if (pl.dobrate > 0)
                    {
                        var judg = Utility.random(0, 1000);
                        if (judg < pl.dobrate)
                        {
                            total_dmg += total_dmg;
                            ret["dobdmg"] = true;
                        }
                    }
                }

                // 吸收伤害
                total_dmg = pl_absorb_dmg(tar_sprite, tar_pl, total_dmg);
                // 魔法抵扣生命
                total_dmg = pl_hpdmg_2_mpdmg(tar_sprite, tar_pl, total_dmg);
                // 伤害链接
                pl_dmg_link(tar_sprite, tar_pl, total_dmg);

            }

            if (is_pvp && total_dmg > 0)
            {
                var pkignore_dp = false;//无视dp  
                var pkigdp_rate = pl.pkigdp_rate - tar_pl.pkigdp_rate_debuffs;
                if (pkigdp_rate > 0)
                {
                    var judg = Utility.random(0, 1000);
                    pkignore_dp = judg < pkigdp_rate;
                }
                if (!pkignore_dp)
                {
                    if (tar_pl.dp < total_dmg)
                    {
                        var dp_dmg = tar_pl.dp;
                        tar_pl.dp = 0;

                        //剩余值 10倍作用于hp
                        total_dmg = ((total_dmg - dp_dmg));//*tar_sprite.game_conf.ply_hp_dmg_factor);
                        tar_pl.hp -= (int)total_dmg;
                        if (tar_pl.hp < 0) tar_pl.hp = 0;

                        ret["dp_dmg"] = dp_dmg;
                    }
                    else
                    {
                        tar_pl.dp -= (int)total_dmg;
                        if (tar_pl.dp > tar_pl.max_dp) tar_pl.dp = tar_pl.max_dp;

                        ret["dp_dmg"] = total_dmg;
                        total_dmg = 0;
                    }
                }
                else
                {
                    //剩余值 10倍作用于hp
                    total_dmg = (total_dmg * 10);//tar_sprite.game_conf.ply_hp_dmg_factor);
                    tar_pl.hp -= (int)total_dmg;
                    if (tar_pl.hp < 0) tar_pl.hp = 0;
                }
                //sys.trace( sys.SLT_DETAIL, "tar_pl.cid =" + tar_pl.cid + "    tar_pl.dp = " +  tar_pl.dp + "    tar_pl.hp = " +  tar_pl.hp + "\n");      
            }
            else
            {
                tar_pl.hp -= (int)total_dmg;
                if (tar_pl.hp < 0) tar_pl.hp = 0;
                if (tar_pl.hp > tar_pl.max_hp) tar_pl.hp = tar_pl.max_hp;
            }

            // broadcast hpchange msg
            //tar_sprite.broad_cast_zone_msg_and_self(26, {iid=tar_pl.iid, hpchange={hpadd=-total_dmg, die=(tar_pl.hp <= 0), frm_iid=pl.iid}});
            ret["isdie"] = (tar_pl.hp <= 0);
            ret["dmg"] = total_dmg;

            //if(tar_pl.cid >0)
            //{
            //    var tid = team.get_ply_teamid(tar_pl.cid);
            //    if(tid > 0)
            //    {
            //        // 在队伍中，发送队友属性变化消息
            //        if( is_pvp )
            //        {
            //            team_bcast_rpc_except(tid, 138, {cid=tar_pl.cid, hp=tar_pl.hp, dp=tar_pl.dp}, tar_pl.sid);
            //        }
            //        else
            //        {
            //            team_bcast_rpc_except(tid, 138, {cid=tar_pl.cid, hp=tar_pl.hp}, tar_pl.sid);
            //        }         
            //    }
            //}

            if (tar_pl.hp <= 0)
            {
                // die!
                tar_sprite.die(frm_sprite);
            }
            //目标死亡，本次伤害不反弹
            if (frm_sprite != null && real_atk_dmg > 0 && tar_pl.iid != pl.iid && tar_pl.hp > 0)
            {
                // 反弹伤害
                double dmg = 0;
                if (tar_pl.rev_dmg_mul > 0)
                {
                    dmg = (real_atk_dmg * ((tar_pl.rev_dmg_mul) / 1000.0));
                    if (dmg <= 0) dmg = 1;
                }
                if (tar_pl.rev_atk > 0)
                {
                    var judg = Utility.random(0, 1000);
                    if (judg < tar_pl.rev_atk)
                    {
                        dmg += real_atk_dmg;
                    }
                }

                //if(is_pvp)
                //{
                //    dmg = (dmg * combat_conf.pk_dmg_per/100);
                //}

                if (dmg > 0)
                {
                    // 吸收伤害
                    dmg = pl_absorb_dmg(frm_sprite, pl, dmg);
                    // 魔法抵扣生命
                    dmg = pl_hpdmg_2_mpdmg(frm_sprite, pl, dmg);

                    frm_sprite.onhate(tar_sprite, dmg);
                    frm_sprite.ondmg(tar_sprite, dmg);

                    //var frm_hpchange = {frm_iid=tar_pl.iid}
                    if (is_pvp)
                    {
                        //sys.trace( sys.SLT_DETAIL, "pl.dp =" + pl.dp + "pl.hp = "+ pl.hp + "\n");
                        if (pl.dp < dmg)
                        {
                            var dp_dmg = pl.dp;
                            pl.dp = 0;
                            //剩余值 10倍作用于hp
                            dmg = ((dmg - dp_dmg));//*tar_sprite.game_conf.ply_hp_dmg_factor);
                            pl.hp -= (int)dmg;
                            if (pl.hp < 0) pl.hp = 0;

                            //frm_hpchange.dpadd <- -dp_dmg;
                            //frm_hpchange.hpadd <- -dmg;
                            //frm_hpchange.die <- pl.hp <= 0;
                        }
                        else
                        {
                            pl.dp -= (int)dmg;

                            //frm_hpchange.dpadd <- -dmg;
                        }
                        //sys.dumpobj( frm_hpchange ); 
                    }
                    else
                    {
                        pl.hp -= (int)dmg;
                        if (pl.hp < 0) pl.hp = 0;

                        //frm_hpchange.hpadd <- -dmg;
                        //frm_hpchange.die <- pl.hp <= 0;
                    }

                    // broadcast hpchange msg
                    //frm_sprite.broad_cast_zone_msg_and_self(26, {iid=pl.iid, hpchange=frm_hpchange});

                    //if(pl.cid > 0)
                    ////if("cid" in pl)
                    //{
                    //    var tid = team.get_ply_teamid(pl.cid);
                    //    if(tid > 0)
                    //    {
                    //        // 在队伍中，发送队友属性变化消息
                    //        if ( is_pvp )
                    //        {
                    //             team_bcast_rpc_except(tid, 138, {cid=pl.cid, hp=pl.hp, dp=pl.dp}, pl.sid);
                    //        }
                    //        else
                    //        {
                    //            team_bcast_rpc_except(tid, 138, {cid=pl.cid, hp=pl.hp}, pl.sid);
                    //        }                       
                    //    }
                    //}

                    if (pl.hp <= 0)
                    {
                        // 被弹死了
                        frm_sprite.die(tar_sprite);
                    }
                }
            }
        }

        // 魔法值、怒气值调整
        var mpchanged = false;
        //var mpchange_rpc = {};
        if (damage.mp_dmg != 0)
        {
            var mpdmg = (damage.mp_dmg * per / 1000);

            tar_pl.mp -= mpdmg;
            if (tar_pl.mp < 0) tar_pl.mp = 0;
            if (tar_pl.mp > tar_pl.max_mp) tar_pl.mp = tar_pl.max_mp;

            //mpchange_rpc.mpadd <- -mpdmg;
            mpchanged = true;

            if (frm_sprite != null)
            {
                tar_sprite.onhate(frm_sprite, mpdmg);
            }
        }

        if (mpchanged && tar_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
        {
            // send mpchange msg
            //::send_rpc(tar_pl.sid, 32, mpchange_rpc);
        }

        return ret;
    }


    public static int update_pl_move(IBaseUnit sprite, long cur_clock_tm)
    {
        var pl = sprite.get_pack_data();

        bool dflag = true;

        if (pl.moving == null)
            return 0;


        //// for Debug...
        //if(sprite.get_sprite_type() == map_sprite_type.MstMonster)
        //{
        //    return;
        //}

        if (sprite.has_state(pl_state_type.PST_CANT_MOVE))
        {
            // can't move

            // 停止移动
            //var data = { tm = cur_clock_tm, iid = pl.iid, x = pl.x, y = pl.y, face = pl.face };
            //// send stop move msg to clients
            ////sprite.gmap.broadcast_map_rpc(10, data);
            //sprite.broad_cast_zone_msg_and_self(10, data);

            if (dflag) Utility.debug("update_pl_move pl_state_type.PST_CANT_MOVE!!");

            pl.moving = null;
            //delete pl.moving;
            return 0;
        }

        if (pl.moving.pts != null)
        {
            if (dflag) Utility.debug("update_pl_move pts  in pl.moving!!");
            return _update_pl_pt_move(sprite, cur_clock_tm);
        }

        return _update_pl_ori_move(sprite, cur_clock_tm);
    }

    public static int _update_pl_pt_move(IBaseUnit sprite, long cur_clock_tm)
    {
        var pl = sprite.get_pack_data();
        var speed = pl.speed;

        // moving
        var tmleft = cur_clock_tm - pl.moving.start_tm;
        if (tmleft <= 0)
        {
            return 0;
        }
        var move_distance = (tmleft * speed) / 1000.0;

        for (; move_distance > 0 && pl.moving.pts.Count > 0;)
        {
            var pt = pl.moving.pts.pop();
            var to_x = pt.x * game_const.map_grid_pixel + 16;
            var to_y = pt.y * game_const.map_grid_pixel + 16;

            //Utility.trace_err("pt.x: [" + pt.x + "] pt.y["+pt.y+"] to_x["+to_x+"] to_y["+to_y+"]\n");

            if (to_x == pl.x && to_y == pl.y)
            {
                continue;
            }

            var x_dis = to_x - pl.x;
            var y_dis = to_y - pl.y;

            var distance = Math.Sqrt((x_dis) * (x_dis) + (y_dis) * (y_dis));

            if (distance > move_distance)
            {
                pl.moving.float_x += (x_dis) / distance * move_distance;
                pl.moving.float_y += (y_dis) / distance * move_distance;

                pl.x = (int)(pl.moving.float_x);
                pl.y = (int)(pl.moving.float_y);
                //pl.x += ((to_x - pl.x)/distance * move_distance);
                //pl.y += ((to_y - pl.y)/distance * move_distance);

                //pl.x += (to_x - pl.x)/distance * move_distance;
                //pl.y += (to_y - pl.y)/distance * move_distance;

                move_distance = 0;
                pl.moving.pts.push(pt);

                if (sprite.get_sprite_type() == map_sprite_type.MstPlayer && !sprite.is_in_lvl)
                {
                    pl.lx = pl.x;
                    pl.ly = pl.y;
                }

                //Utility.trace_err("moving x: [" + pl.x +"] y:[" + pl.y + "] float_x["+pl.moving.float_x+"] float_y["+pl.moving.float_y+"] \n");
            }
            else
            {
                move_distance -= distance;
                pl.x = (int)to_x;
                pl.y = (int)to_y;

                pl.moving.float_x = (to_x);
                pl.moving.float_y = (to_y);

                if (sprite.get_sprite_type() == map_sprite_type.MstPlayer)
                {
                    if (!sprite.is_in_lvl)
                    {
                        pl.lx = pl.x;
                        pl.ly = pl.y;
                    }

                    if (pl.moving.ppts == null)
                        pl.moving.ppts = new List<Point2D>();

                    pl.moving.ppts.push(pt); // 记录历史经过的节点
                }

                //Utility.trace_err("Rich x: [" + pl.x +"] y:[" + pl.y + "] float_x["+pl.moving.float_x+"] float_y["+pl.moving.float_y+"] \n");
            }
        }

        sprite.on_pos_change(pl.x, pl.y);

        if (pl.moving.pts.Count <= 0)
        {
            if (pl.moving.ppts != null)
                pl.last_mvpts = pl.moving.ppts;

            pl.moving = null;



            //Utility.trace_err("Stop at x: [" + pl.x +"] y:[" + pl.y + "] \n");

            return (int)(move_distance * 1000 / pl.speed); // 停止后剩余时间
        }
        else
        {
            pl.moving.start_tm = cur_clock_tm;
        }
        return 0;
    }


    public static int _update_pl_ori_move(IBaseUnit sprite, long cur_clock_tm)
    {
        var pl = sprite.get_pack_data();
        var dflag = true;

        var speed = pl.speed;

        var tmleft = cur_clock_tm - pl.moving.start_tm;

        if (dflag) Utility.debug("_update_pl_ori_move speed[" + speed + "] tmleft[" + tmleft + "]");

        if (tmleft <= 0)
        {
            return 0;
        }

        // moving  
        var to_end = false;
        var move_distance = tmleft * speed / 1000.0;

        if (move_distance >= pl.moving.max_r)
        {
            to_end = true;
            move_distance = pl.moving.max_r;
        }

        var to_x = pl.moving.stx + (move_distance * pl.moving.ori_cos);
        var to_y = pl.moving.sty + (move_distance * pl.moving.ori_sin);

        if (dflag)
        {
            Utility.debug(" move_distance[" + move_distance + "]  tmleft[" + tmleft + "] speed[" + speed + "]  cur_clock_tm[" + cur_clock_tm + "]  pl.moving.start_tm[" + pl.moving.start_tm + "] pl.x[" + pl.x + "] pl.y[" + pl.y + "] to_x[" + to_x + "] to_y[" + to_y + "]");
        }

        if (to_end)
        {
            Utility.debug("_update_pl_ori_move to_end !!! ");
            pl.moving = null;
        }

        pl.x = (int)to_x;
        pl.y = (int)to_y;

        if (sprite.get_sprite_type() == map_sprite_type.MstPlayer && !sprite.is_in_lvl)
        {
            pl.lx = pl.x;
            pl.ly = pl.y;
        }
        sprite.on_pos_change(pl.x, pl.y);
        return 0;
    }

    public void apply_direct_dmg_on_pl(IBaseUnit tar_sprite, IBaseUnit frm_sprite, Variant damage, long cur_clock_tm, int per)
    {
        // TO DO : 优化： per = 1000时不需要计算增益

        var tar_pl = tar_sprite.get_pack_data();

        // 取不到状态添加对象情况下，本函数frm_sprite可能为空，则需要默认的计算参数
        var pl = default(IMapUnit);

        if (frm_sprite != null)
        {
            pl = frm_sprite.get_pack_data();
            pl.last_atk_tm = cur_clock_tm; // 触发普通攻击cd
        }

        //Utility.trace_info("apply_dmg_on_pl frm_sprite:["+frm_sprite+"] per["+per+"]\n");
        //sys.dumpobj(pl);

        //var hpchange = {};
        if (tar_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
        {// 玩家 防护值调整
            double dpdmg = 0;
            if (damage.ContainsKey("dp_dmg"))
            {
                dpdmg = (damage["dp_dmg"]._int32 * per / 1000);
            }
            if (damage.ContainsKey("dp_per"))
            {
                dpdmg += (damage["dp_per"]._int32 * per / 100000.0 * tar_pl.max_dp);
            }
            //    if (frm_sprite != null && damage.ContainsKey("dattadj"))
            //    {
            //        foreach (attname, val in damage.dattadj[0] )
            //{
            //            if (attname in pl )
            //    {
            //                dpdmg += (pl[attname] / val);
            //                //sys.trace( sys.SLT_DETAIL, " pl[" + attname + "]= " + pl[attname] + " mpdmg=" + mpdmg + "\n" );
            //            }
            //        }
            //    }

            if (dpdmg != 0)
            {

                tar_pl.dp -= (int)dpdmg;
                if (tar_pl.dp < 0) tar_pl.dp = 0;
                if (tar_pl.dp > tar_pl.max_dp) tar_pl.dp = tar_pl.max_dp;

                //sys.trace( sys.SLT_DETAIL, "dpdmg tar_pl.cid =" + tar_pl.cid + "    tar_pl.dp = " +  tar_pl.dp + "\n");      
                //hpchange.dpadd < - -dpdmg;
            }
        }

        double total_dmg = 0;
        //        if ("hp_dmg" in damage)
        //{
        //            total_dmg = (damage.hp_dmg * per / 1000.0);
        //            //sys.trace( sys.SLT_DETAIL, "per= " + per + " total_dmg=" + total_dmg + "\n" );
        //        }
        //        if ("hp_per" in damage )
        //{
        //            total_dmg += (damage.hp_per * per / 100000.0 * tar_pl.max_hp);
        //        }
        //        if (frm_sprite && ("hattadj" in damage) )
        //{
        //            foreach (attname, val in damage.hattadj[0] )
        //    {
        //                if (attname in pl )
        //        {
        //                    total_dmg += (pl[attname] / val);
        //                    //sys.trace( sys.SLT_DETAIL, " pl[" + attname + "]= " + pl[attname] + " total_dmg=" + total_dmg + "\n" );
        //                }
        //            }
        //        }

        //sys.dumpobj(damage);
        //Utility.trace_info("total_dmg:["+total_dmg+"]\n");
        if (total_dmg != 0)
        {
            if (total_dmg > 0)
            {
                if (frm_sprite != null)
                {
                    tar_sprite.onhate(frm_sprite, total_dmg);
                    tar_sprite.ondmg(frm_sprite, total_dmg);

                    if (frm_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
                    {
                        if (frm_sprite.gmap.blvlmap)
                        {
                            frm_sprite.gmap.worldsvr.on_dmg(frm_sprite, tar_sprite, total_dmg);
                        }
                    }
                }

                // 吸收伤害
                total_dmg = pl_absorb_dmg(tar_sprite, tar_pl, total_dmg);
                // 魔法抵扣生命
                total_dmg = pl_hpdmg_2_mpdmg(tar_sprite, tar_pl, total_dmg);
                // 伤害链接
                pl_dmg_link(tar_sprite, tar_pl, total_dmg);
            }

            tar_pl.hp -= (int)total_dmg;
            if (tar_pl.hp < 0) tar_pl.hp = 0;
            if (tar_pl.hp > tar_pl.max_hp) tar_pl.hp = tar_pl.max_hp;

            //hpchange.hpadd < - -total_dmg;
            //hpchange.die < -tar_pl.hp <= 0;
        }

        //    if (hpchange.Count > 0)
        //    {
        //        // broadcast hpchange msg
        //        hpchange.frm_iid < -pl.iid;
        //        tar_sprite.broad_cast_zone_msg_and_self(26, { iid = tar_pl.iid, hpchange = hpchange});

        //        if ("cid" in tar_pl)
        //{
        //            var tid = team.get_ply_teamid(tar_pl.cid);
        //            if (tid > 0)
        //            {
        //                // 在队伍中，发送队友属性变化消息
        //                team_bcast_rpc_except(tid, 138, { cid = tar_pl.cid, hp = tar_pl.hp, dp = tar_pl.dp}, tar_pl.sid);
        //            }
        //        }

        if (tar_pl.hp <= 0)
        {
            // die!
            tar_sprite.die(frm_sprite);
        }
        //}

        double mpdmg = 0;
        //        if ("mp_dmg" in damage)
        //{
        //            mpdmg = (damage.mp_dmg * per / 1000);
        //        }
        //        if ("mp_per" in damage )
        //{
        //            mpdmg += (damage.mp_per * per / 100000.0 * tar_pl.max_mp);
        //        }
        //        if (frm_sprite && ("mattadj" in damage) )
        //{
        //            foreach (attname, val in damage.mattadj[0] )
        //    {
        //                if (attname in pl )
        //        {
        //                    mpdmg += (pl[attname] / val);
        //                    //sys.trace( sys.SLT_DETAIL, " pl[" + attname + "]= " + pl[attname] + " mpdmg=" + mpdmg + "\n" );
        //                }
        //            }
        //        }

        //Utility.trace_info("mpdmg:["+mpdmg+"]\n");
        //if (mpdmg != 0)
        //{
        //    // 魔法值、怒气值调整
        //    var mpchange_rpc = { };

        //    tar_pl.mp -= mpdmg;
        //    if (tar_pl.mp < 0) tar_pl.mp = 0;
        //    if (tar_pl.mp > tar_pl.max_mp) tar_pl.mp = tar_pl.max_mp;

        //    mpchange_rpc.mpadd < - -mpdmg;

        //    if (frm_sprite)
        //    {
        //        tar_sprite.onhate(frm_sprite, mpdmg);
        //    }

        //    if (tar_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
        //    {
        //// send mpchange msg
        //::send_rpc(tar_pl.sid, 32, mpchange_rpc);
        //    }
        //}

    }

    public static Variant apply_dmg_on_pl(IBaseUnit tar_sprite, IBaseUnit frm_sprite, damage damage, long cur_clock_tm, int per, bool direct_dmg = false)
    {
        // TO DO : 优化： per = 1000时不需要计算增益
        //direct_dmg 非状态技能伤害


        var tar_pl = tar_sprite.get_pack_data();
        if (tar_pl == null)
        {
            return null;
        }

        if (tar_sprite.ignore_dmg())
        {
            return null;
        }
        //::g_dump( " apply_dmg_on_pl()  ===>>>>", "") ;
        //var ret = {hited=3, criatk=0, dmg=0, dobdmg=false, isdie=false, frm_iid=pl.iid, to_iid=tar_pl.iid};
        var ret = apply_dmg_on_pl_ex(tar_sprite, frm_sprite, damage, cur_clock_tm, per, false, direct_dmg);



        if (ret["hited"]._int32 == 3)
        {
            // broadcast hpchange msg
            //var hpchange = { frm_iid = ret.frm_iid, hited = ret.hited, criatk = ret.criatk, dobdmg = ret.dobdmg };
            //        if (ret.dmg != 0)
            //        {
            //            hpchange.hpadd < - -ret.dmg;
            //            hpchange.die < -ret.isdie;
            //        }
            //        if (("dp_dmg" in ret) && ret.dp_dmg != 0 )
            //{
            //            hpchange.dpadd < - -ret.dp_dmg;
            //        }
            //        tar_sprite.broad_cast_zone_msg_and_self(26, { iid = tar_pl.iid, hpchange = hpchange});
        }

        return ret;
    }

    public static double pl_absorb_dmg(IBaseUnit sprite, IMapUnit pl, double dmg)
    {
        if (pl.states == null)
            return dmg;


        List<int> remove_stateids = new List<int>();

        for (var idx = 0; idx < pl.states.state_par.Count; ++idx)
        {
            var val = pl.states.state_par[idx];

            if (val.desc.absorbdmg != null)
            {
                //Utility.trace_info("dmg before ["+dmg+"]\n");

                var per = (val.desc.absorbdmg.per / 10);
                if (per > 100) per = 100;

                var absorbdmg = (dmg * per / 100);

                //Utility.trace_info("per ["+per+"]\n");
                //Utility.trace_info("absorbdmg ["+absorbdmg+"]\n");
                //Utility.trace_info("val.maxdmg ["+val.maxdmg+"]\n");

                if (absorbdmg + val.absorbed > val.maxdmg)
                {
                    absorbdmg = val.maxdmg - val.absorbed;
                }

                dmg -= absorbdmg;
                val.absorbed += (int)absorbdmg;

                //Utility.trace_info("dmg after ["+dmg+"] aborbdmg["+absorbdmg+"] \n");

                if (val.absorbed >= val.maxdmg)
                {
                    // 移除吸收伤害状态
                    pl.states.state_par.RemoveAt(idx);
                    --idx;
                    remove_stateids.push(val.desc.id);
                }

                if (dmg <= 0)
                {
                    dmg = 0;
                    break;
                }
            }
        }


        if (remove_stateids.Count > 0)
        {
            // broadcast rmv_state msg
            //    sprite.broad_cast_zone_msg_and_self(31, {
            //        iid = pl.iid, ids = remove_stateids
            //});

            Skill._remark_pl_state(sprite, pl);
        }

        return dmg;
    }

    public static double pl_hpdmg_2_mpdmg(IBaseUnit sprite, IMapUnit pl, double dmg)
    {
        if (pl.states == null)
            return dmg;

        if (pl.states.mp2hp > 0)
        {
            //每次受到伤害时角色魔法值减少最大魔法值*2%
            var mp_cost = (pl.max_mp * 0.02);

            if (mp_cost <= pl.mp)
            {
                var mp_dmg = (dmg * pl.states.mp2hp / 100);
                if (mp_dmg > 0)
                {
                    dmg -= mp_dmg;

                    if (dmg < 0) dmg = 0;

                    pl.mp -= (int)mp_cost;
                    if (pl.mp < 0) pl.mp = 0;

                    // send mpchange msg
                    //        if ("sid" in pl) 
                    //{
                    //    ::send_rpc(pl.sid, 32, { mpadd = -mp_cost});
                    //        }
                }
            }
        }

        return dmg;
    }

    public static void pl_dmg_link(IBaseUnit frm_sprite, IMapUnit frm_pl, double dmg)
    {
        if (frm_pl.states == null)
            return;

        if (frm_pl.states.dmglnkper <= 0)
            return;

        if (frm_sprite.gmap == null)
            return;

        // 有伤害链接
        var share_dmg = (dmg * (frm_pl.states.dmglnkper / 1000.0));

        // 遍历并传递伤害
        foreach (var sprite in frm_sprite.gmap.map_sprites.Values)
        {
            if (!sprite.has_state(pl_state_type.PST_DMGLNK_S))
                continue;

            if (sprite.isdie())
                continue;

            var pl = sprite.get_pack_data();
            if (pl.iid == frm_pl.iid)
                continue;

            // 吸收伤害
            share_dmg = pl_absorb_dmg(sprite, pl, share_dmg);
            // 魔法抵扣生命
            share_dmg = pl_hpdmg_2_mpdmg(sprite, pl, share_dmg);

            pl.hp -= (int)share_dmg;
            if (pl.hp < 0) pl.hp = 0;

            sprite.onhate(frm_sprite, share_dmg);
            sprite.ondmg(frm_sprite, share_dmg);

            // broadcast hpchange msg
            //sprite.broad_cast_zone_msg_and_self(26, { iid = pl.iid, hpchange ={ hpadd = -share_dmg, die = (pl.hp <= 0), frm_iid = frm_pl.iid} });

            //        if ("cid" in pl)
            //{
            //            var tid = team.get_ply_teamid(pl.cid);
            //            if (tid > 0)
            //            {
            //                // 在队伍中，发送队友属性变化消息
            //                team_bcast_rpc_except(tid, 138, { cid = pl.cid, hp = pl.hp}, pl.sid);
            //            }
            //        }

            if (pl.hp <= 0)
            {
                // 被弹死了
                sprite.die(frm_sprite);
            }
        }
    }

    public Point2D valpoint_on_vector(double dest_x, double dest_y, double from_x, double from_y, Point2D vec)
    {
        //Utility.trace_info("dest_x["+from_x+"] dest_y["+from_y+"]\n");
        //sys.dumpobj(vec);

        var total_len = 0;
        var grid_pos = this.get_grid_by_pt(dest_x, dest_y);
        while (grid_pos == null || !is_grid_walkableEx((int)grid_pos.x, (int)grid_pos.y))
        {
            // 目标落点无法行走，更改落点
            dest_x = (dest_x - vec.x * game_const.map_grid_pixel);
            dest_y = (dest_y - vec.y * game_const.map_grid_pixel);
            total_len += 32;

            //Utility.trace_info("dest_x["+dest_x+"] dest_y["+dest_y+"]\n");

            if (total_len >= 1024)
            {
                dest_x = from_x;
                dest_y = from_y;
                break;
            }

            grid_pos = this.get_grid_by_pt(dest_x, dest_y);
        }

        return new Point2D(dest_x, dest_y);
    }


    public cast_skill_res _cast_skill(long cur_clock_tm, Variant rpc, skill_type tar_tp, IBaseUnit caster, IPoint2D to_pos, IBaseUnit tar_sprite, bool precast)
    {
        var cur_tm_s = DateTime.Now.ToBinary();

        if (caster.isghost())
            return new cast_skill_res(game_err_code.YOU_ARE_GHOST);

        var pl = caster.get_pack_data();
        if (pl.casting != null)
            return new cast_skill_res(game_err_code.SKIL_ALREADY_CASTING_SKILL);

        if (pl.jumping != null)
            return new cast_skill_res(game_err_code.SKIL_CANT_CAST_WHEN_JUMP);

        int skill_id = rpc["sid"]._int32;
        //有聚气状态  使用任何技能 将触发它
        if (pl.holding != null && skill_id != pl.holding.rpc["sid"]._int32)
            return new cast_skill_res(game_err_code.SKIL_ALREADY_HOLDING_SKILL);

        var skill_data = Utility.get_skil_skill_desc(skill_id);
        if (skill_data == null)
            return new cast_skill_res(game_err_code.SKIL_NOT_EXIST);

        if (skill_data.tar_tp != (int)tar_tp)
        {
            // Err: skill target type wrong         
            return new cast_skill_res(game_err_code.SKIL_CAST_TARGET_TYPE_ERR);
        }

        // check caster own the skill and get skill level
        var caster_skil_data = caster.get_skil_data(skill_id);
        if (caster_skil_data == null)
        {
            caster_skil_data = this.get_map_skill(skill_id);
            if (caster_skil_data == null)
            {
                return new cast_skill_res(game_err_code.SKIL_NOT_LEARN);
            }
        }

        if (tar_sprite != null)
        {
            var tar_pl = tar_sprite.get_pack_data();
            if (tar_pl.invisible > pl.observer)
            {
                // 看不到目标             
                return new cast_skill_res(game_err_code.SKIL_TARGET_NOT_AVALIBLE);
            }

            if (skill_data.tar_tp == (int)skill_type.ST_TARGET)
            {
                game_err_code check_res = Utility.check_target_type(caster, tar_sprite, skill_data.aff, this.pk_seting);
                if (check_res != game_err_code.RES_OK)
                    return null;
            }
        }

        var skill_level_conf = Utility.get_skil_skill_lvl_desc(skill_id, caster_skil_data.sklvl);
        if (skill_level_conf == null)
        {
            // Err: skill data error          
            Utility.trace_err("persist_game cast skill monster iid[" + pl.iid + "] skill [" + skill_id + "] sklvl[" + caster_skil_data.sklvl + "]  data error\n");
            return new cast_skill_res(game_err_code.SKIL_LVL_ERR);
        }

        if (cur_clock_tm < pl.skill_gen_cd)
        {
            //Utility.trace_err("persist_game cast skill caster [" + caster.get_iid() +"] skill ["+rpc.sid+"] cd not reach\n");         
            return new cast_skill_res(game_err_code.SKIL_CD_NOT_REACH);
        }


        if (pl.moving != null)
            update_pl_move(caster, cur_clock_tm);

        // rang check
        var cast_rang = pl.size + pl.atkrange;
        cast_rang += skill_data.cast_rang;

        if (to_pos != null)
        {
            if (tar_sprite != null)
            {
                var tar_pl = tar_sprite.get_pack_data();
                cast_rang += tar_pl.size;
                if (tar_pl.moving != null)
                    update_pl_move(tar_sprite, cur_clock_tm);
            }

            if (skill_level_conf.jump == null && skill_level_conf.teleport == null)
            {
                // 近战技能
                var dist_x = to_pos.x - pl.x;
                var dist_y = to_pos.y - pl.y;
                var rang = cast_rang + 45; // 允许1格误差
                if (dist_x * dist_x + dist_y * dist_y > rang * rang)
                {
                    //Utility.trace_err("persist_game cast skill caster [" + caster.get_iid() +"] skill ["+rpc.sid+"] ground skill out of rang\n");                 
                    return new cast_skill_res(game_err_code.SKIL_CAST_RANG_ERR);
                }
            }
        }

        // 计算吟唱时间
        if (precast && skill_data.cast_tm > 0)
        {
            var cast_tm = skill_data.cast_tm;
            var mul = 0;
            var add = 0;



            cast_tm = (cast_tm * 1000 / (1000 + mul) - add);
            //Utility.trace_info("cast_tm:["+cast_tm+"] \n");

            if (cast_tm > 0)
            {
                // 需要吟唱
                pl.moving = null;

                pl.casting = new casting()
                {
                    tar_tp = (int)tar_tp,
                    rpc = rpc,
                    end_tm = (cur_clock_tm + cast_tm * 100)
                };

                return new cast_skill_res(game_err_code.RES_OK);
            }
        }

        var hold_per = 1;
        if (skill_data.hold_tm > 0)
        {
            var hold_tm = skill_data.hold_tm;
            if (pl.holding != null)
            {
                hold_per = (int)(cur_clock_tm - pl.holding.start_tm) / (hold_tm * 100);
                if (hold_per > 1) hold_per = 1;

                pl.holding = null;
            }
            else
            {
                pl.moving = null;

                pl.holding = new holding()
                {
                    tar_tp = (int)tar_tp,
                    rpc = rpc,
                    start_tm = cur_clock_tm,
                    end_tm = cur_clock_tm + hold_tm * 100
                };

                return new cast_skill_res(game_err_code.RES_OK);
            }
        }

        if (pl.invisible > 0)
        {
            // 攻击就现身
            Skill.rmv_state_from_pl_by_att("invisible", caster, pl);
        }

        var cd_tm = skill_level_conf.cd;
        var cdmul = 0;

        cd_tm = (cd_tm * 1000 / (1000 + cdmul));
        if (cd_tm < 2) cd_tm = 2;
        //Utility.trace_info("cd_tm:["+cd_tm+"] \n");


        pl.skill_gen_cd = cur_clock_tm + cd_tm * 100;
        pl.skill_gen_cdst = cur_clock_tm;

        //Utility.trace_err("persist_game cast self skill caster [" + caster.get_iid() +"] skill ["+rpc.sid+"] cd ["+pl.skill_cd[skill_data.cdtp]+"]\n");


        var percent = 1000;


        percent = (percent * hold_per);
        if (percent <= 0) percent = 1;

        Variant rpc_data = new Variant();
        rpc["res"] = (int)game_err_code.RES_OK;
        rpc["start_tm"] = rpc["start_tm"];
        rpc["frm_iid"] = pl.iid;
        rpc["sid"] = rpc["sid"];

        cast_skill_res cast_skill_res = new cast_skill_res(game_err_code.RES_OK)
        {
            skill_data = skill_data,
            skill_level_conf = skill_level_conf,
            rpc_data = rpc_data,
            percent = percent
        };

        if (to_pos != null)
        {
            if (skill_level_conf.teleport != null)
            {
                // 瞬移
                pl.moving = null;
                pl.last_mvpts = null;

                var telep_tm = 0;

                telep_tm = skill_level_conf.teleport.tm * 100;

                var dest_pos = Utility.valpoint_on_line(pl, to_pos, skill_level_conf.teleport.rang);

                if (telep_tm == 0)
                {//立即
                    pl.x = (int)dest_pos.x;
                    pl.y = (int)dest_pos.y;

                    if (caster.get_sprite_type() == map_sprite_type.MstPlayer && !caster.is_in_lvl)
                    {
                        pl.lx = pl.x;
                        pl.ly = pl.y;
                    }
                }
                else
                {
                    pl.teleping = new teleping()
                    {
                        start_tm = cur_clock_tm,
                        end_tm = (cur_clock_tm + telep_tm),
                        telep = skill_level_conf.teleport,
                        rpc = rpc,
                        percent = percent,
                        dest_x = (int)dest_pos.x,
                        dest_y = (int)dest_pos.y
                    };
                }
                //rpc_data.telep < - { to_x = dest_pos.x, to_y = dest_pos.y, tm = telep_tm};
            }
            else if (skill_level_conf.jump != null)
            {
                // 跳跃
                pl.moving = null;
                pl.last_mvpts = null;

                var sub_len = 0;
                if (tar_sprite != null)
                {
                    sub_len = 45;
                }

                var dest_pos = Utility.valpoint_on_line(pl, to_pos, skill_level_conf.jump.jump_rang, sub_len);

                var dist_x = dest_pos.x - pl.x;
                var dist_y = dest_pos.y - pl.y;
                var rang = Math.Sqrt(dist_x * dist_x + dist_y * dist_y);

                long jump_tm = (long)(rang * 1000 / skill_level_conf.jump.speed);
                if (jump_tm <= 0) jump_tm = 1;

                //pl.x = dest_pos.x;
                //pl.y = dest_pos.y;

                rpc_data["jump"] = new Variant();
                rpc_data["jump"]["tm"] = jump_tm;
                rpc_data["jump"]["to_x"] = dest_pos.x;
                rpc_data["jump"]["to_y"] = dest_pos.y;

                var tar_iid = 0;
                if (tar_sprite != null)
                    tar_iid = tar_sprite.get_pack_data().iid;

                pl.jumping = new jumping()
                {
                    start_tm = cur_clock_tm,
                    during_tm = jump_tm,
                    end_tm = (cur_clock_tm + jump_tm),
                    jump = skill_level_conf.jump,
                    rpc = rpc,
                    percent = percent,
                    dest_x = (int)dest_pos.x,
                    dest_y = (int)dest_pos.y,
                    tar_iid = tar_iid
                };
            }


            //    if ("fly" in  skill_level_conf && skill_level_conf.fly.speed > 0)
            //{
            //        var dest_pos = { x = to_pos.x, y = to_pos.y };
            //        var dist_x = dest_pos.x - pl.x;
            //        var dist_y = dest_pos.y - pl.y;
            //        var rang = Math.Sqrt(dist_x * dist_x + dist_y * dist_y);
            //        if ("toend" in skill_level_conf.fly )
            //    {
            //            var max_rang = skill_level_conf.fly.rang + cast_rang;
            //            dest_pos.x = (dist_x * max_rang / rang + pl.x);
            //            dest_pos.y = (dist_y * max_rang / rang + pl.y);

            //            rang = max_rang;
            //        }

            //        var fly_tm = (rang * 1000 / skill_level_conf.fly.speed);
            //        var fly_end_tm = cur_clock_tm + fly_tm;

            //        var tar_iid = 0;
            //        if (tar_sprite) tar_iid = tar_sprite.get_pack_data().iid;

            //        // 记录飞行道具
            //        this.map_flys.push({ end_tm = fly_end_tm, frm_iid = pl.iid, rpc = rpc, cast_skill_res = cast_skill_res, tar_iid = tar_iid, frm_pos ={ x = pl.x, y = pl.y}, target_pos = dest_pos});

            //        rpc_data.fly < - { tm = fly_tm, to_x = dest_pos.x, to_y = dest_pos.y};
            //    }
        }
        //::g_dump( "cast_skill_res OK:", cast_skill_res );
        return cast_skill_res;
    }

    public void _post_cast_skill(long cur_clock_tm, IBaseUnit caster, Variant rpc, cast_skill_res cast_skill_res, IBaseUnit target_sprite, IPoint2D target_pos, bool check_fly = true)
    {
        // after skill cast, apply skill effect

        if (check_fly && cast_skill_res.rpc_data.ContainsKey("fly"))
            return;

        int skill_id = rpc["sid"]._int32;

        var level_conf = cast_skill_res.skill_level_conf;
        if (level_conf.tres != null)
        {
            if (level_conf.rang != null)
            {
                if (level_conf.jump != null)
                {
                    // 跳跃技能情况下，tres为起跳时以自己为中心的作用效果
                    Skill.apply_rang_eff(cur_clock_tm, caster, caster.get_pack_data(), level_conf.tres, level_conf.rang, skill_id, cast_skill_res.percent);
                }
                else
                {
                    if (target_pos != null)
                    {
                        // ground skill
                        Skill.apply_rang_eff(cur_clock_tm, caster, target_pos, level_conf.tres, level_conf.rang, skill_id, cast_skill_res.percent);
                    }
                    else if (target_sprite != null)
                    {


                        // target skill
                        Skill.apply_rang_eff(cur_clock_tm, caster, target_sprite.get_pack_data(), level_conf.tres, level_conf.rang, skill_id, cast_skill_res.percent);
                    }
                    else
                    {

                        // self skill
                        Skill.apply_rang_eff(cur_clock_tm, caster, caster.get_pack_data(), level_conf.tres, level_conf.rang, skill_id, cast_skill_res.percent);
                    }
                }
            }
            else
            {
                if (target_sprite != null)
                {
                    // target skill
                    foreach (var tres in level_conf.tres)
                    {
                        Skill.apply_skill_eff_to(cur_clock_tm, caster, target_sprite, tres, tres.aff, skill_id, cast_skill_res.percent);
                    }
                }
            }
        }

        //    if ("mapstat" in cast_skill_res.skill_lvl_data)
        //{
        //        var cur_tm_s = sys.time();

        //        // 创建地图技能状态对象(陷阱、区域持续伤害)
        //        foreach (mapstat_conf in cast_skill_res.skill_lvl_data.mapstat)
        //        {
        //            this._add_map_eff(caster, target_pos, mapstat_conf, cast_skill_res.percent, cur_tm_s, cur_clock_tm, rpc.sid);
        //        }
        //    }

        //    if ("callpetmon" in cast_skill_res.skill_lvl_data)
        //{
        //        var cur_tm_s = sys.time();

        //        // 召唤战斗宠物
        //        var callpetmon_conf = cast_skill_res.skill_lvl_data.callpetmon[0];
        //        caster.call_petmon(callpetmon_conf.mid, cur_tm_s + callpetmon_conf.tm);
        //    }
    }



    public bool do_cast_target_skill(IBaseUnit caster, Variant rpc, bool precast = true, bool senderr = true)
    {
        var tar_iid = rpc["to_iid"]._int32;

        IBaseUnit tar_sprite = this.get_sprite_by_iid(tar_iid);
        if (tar_sprite == null)
            return false;

        if (!this.has_sprite_by_iid(caster.iid))
            return false;

        if (tar_sprite.has_state(pl_state_type.PST_SKILL_AVOID))
        {
            if (senderr)
            {
                //::send_rpc(caster.pinfo.sid, 27, { res = game_err_code.SKIL_TARGET_AVOID});
            }
            return false;
        }

        if (tar_sprite.isdie() || tar_sprite.isghost())
        {
            if (senderr)
            {
                //::send_rpc(caster.pinfo.sid, 27, { res = game_err_code.SKIL_CANT_APPLY_TO_DEADMEN});
            }
            return false;
        }

        long cur_clock_tm = DateTime.Now.ToBinary();
        var cast_skill_res = _cast_skill(cur_clock_tm, rpc, skill_type.ST_TARGET, caster, tar_sprite.get_pack_data(), tar_sprite, precast);
        if (cast_skill_res == null)
            return false;

        if (cast_skill_res.res != game_err_code.RES_OK)
        {
            if (senderr)
            {
                //::send_rpc(caster.pinfo.sid, 27, cast_skill_res);
            }
            //Utility.trace_err("persist_game cast skill cast_skill_res.res = [" + cast_skill_res.res + "]\n");
            return false;
        }
        else if (cast_skill_res.rpc_data == null)
        {
            return true;
        }
        // TO DO : rang check & trace

        cast_skill_res.rpc_data["to_iid"] = tar_iid;

        // send cast target skill msg
        //caster.gmap.broadcast_map_rpc(13, cast_skill_res.rpc_data); 
        //caster.broad_cast_zone_msg_and_self(13, cast_skill_res.rpc_data);

        skill_lv_conf skill_lvl_data = cast_skill_res.skill_level_conf;
        if (skill_lvl_data.sres != null)
        {
            Skill.apply_skill_eff_to(cur_clock_tm, caster, caster, skill_lvl_data.sres, (int)skill_aff_type.SAT_SELF, rpc["sid"]._int32, cast_skill_res.percent);
        }
        _post_cast_skill(cur_clock_tm, caster, rpc, cast_skill_res, tar_sprite, tar_sprite.get_pack_data());

        //if("tres" in cast_skill_res.skill_lvl_data)
        //{
        //    if("trang" in cast_skill_res.skill_lvl_data)
        //    {
        //        foreach(tres in cast_skill_res.skill_lvl_data.tres)
        //        {
        //            apply_rang_eff(cur_clock_tm, caster, this.map_sprites[tar_iid].get_pack_data(), tres, tres.aff, cast_skill_res.skill_lvl_data.trang, rpc.sid, cast_skill_res.percent);
        //        }
        //    }
        //    else
        //    {
        //        foreach(tres in cast_skill_res.skill_lvl_data.tres)
        //        {
        //            apply_skill_eff_to(cur_clock_tm, caster, this.map_sprites[tar_iid], tres, tres.aff, rpc.sid, cast_skill_res.percent);
        //        }
        //    }
        //}

        if (caster.get_sprite_type() == map_sprite_type.MstPlayer)
        {
            if (!tar_sprite.isdie() && !tar_sprite.isghost())
            {
                if ((cast_skill_res.skill_data.aff & (int)skill_aff_type.SAT_ENERMY) > 0)
                {
                    foreach (var petmon in caster.petmon_insts)
                    {
                        var petmondata = petmon.get_pack_data();
                        if (petmondata.atking == null)
                        {
                            petmondata.atking = new atking()
                            { start_tm = cur_clock_tm, tar_iid = tar_iid, trace_tm_left = 0 };

                        }
                        else if (petmondata.atking.tar_iid != tar_iid)
                        {
                            petmondata.atking.tar_iid = tar_iid;
                        }
                    }
                }
            }
        }

        return true;
    }

    public bool do_cast_ground_skill(IBaseUnit caster, Variant rpc, bool precast = true, bool senderr = true)
    {
        Point2D center = null;
        IBaseUnit tar_sprite = null;
        IMapUnit cast_pl = caster.get_pack_data();

        if (this.map_sprites.ContainsKey(cast_pl.iid))
            return false;

        if (rpc.ContainsKey("to_iid"))
        {
            var tar_iid = rpc["to_iid"]._int32;
            tar_sprite = this.get_sprite_by_iid(tar_iid);
            if (tar_sprite == null)
                return false;


            if (tar_sprite.has_state(pl_state_type.PST_SKILL_AVOID))
                return false;

            if (tar_sprite.isdie() || tar_sprite.isghost())
                return false;

            var tar_pl = tar_sprite.get_pack_data();

            center = new Point2D(tar_pl.x, tar_pl.y);
            if (tar_pl.x == cast_pl.x && tar_pl.y == cast_pl.y)
            {
                double angle = rpc["angle"]._double;
                var radian = angle / 180.0 * Math.PI;
                center.x += 32 * Math.Cos(radian);
                center.y += 32 * Math.Sin(radian);
            }
        }
        else if (rpc.ContainsKey("x"))
        {
            int x = rpc["x"]._int32;
            int y = rpc["y"]._int32;

            if (x < 0 || y < 0 || x >= this.width || y >= this.height)
                return false;

            center = new Point2D(x * game_const.map_grid_pixel + 16, y * game_const.map_grid_pixel + 16);
        }
        else
        {
            return false;
        }

        var cur_clock_tm = DateTime.Now.ToBinary();
        var cast_skill_res = _cast_skill(cur_clock_tm, rpc, skill_type.ST_GROUND, caster, center, tar_sprite, precast);
        if (cast_skill_res == null) return false;

        if (cast_skill_res.res != game_err_code.RES_OK)
            return false;
        else if (cast_skill_res.rpc_data == null)
            return true;

        if (cast_skill_res.skill_level_conf.sres != null)
            Skill.apply_skill_eff_to(cur_clock_tm, caster, caster, cast_skill_res.skill_level_conf.sres, (int)skill_aff_type.SAT_SELF, rpc["sid"]._int32, cast_skill_res.percent);

        _post_cast_skill(cur_clock_tm, caster, rpc, cast_skill_res, tar_sprite, center);

        //if("tres" in cast_skill_res.skill_lvl_data && "trang" in cast_skill_res.skill_lvl_data)
        //{
        //    var center = {x=rpc.x*game_const.map_grid_pixel+16, y=rpc.y*game_const.map_grid_pixel+16};

        //    foreach(tres in cast_skill_res.skill_lvl_data.tres)
        //    {
        //        apply_rang_eff(cur_clock_tm, caster, center, tres, tres.aff, cast_skill_res.skill_lvl_data.trang, rpc.sid, cast_skill_res.percent);
        //    }
        //}

        if (tar_sprite != null)
        {
            int tar_iid = tar_sprite.get_pack_data().iid;

            if (caster.get_sprite_type() == map_sprite_type.MstPlayer)
            {
                if (!tar_sprite.isdie() && !tar_sprite.isghost())
                {
                    foreach (var petmon in caster.petmon_insts)
                    {
                        var petmondata = petmon.get_pack_data();
                        if (petmondata.atking == null)
                        {
                            petmondata.atking = new atking()
                            { start_tm = cur_clock_tm, tar_iid = tar_iid, trace_tm_left = 0 };

                        }
                        else if (petmondata.atking.tar_iid != tar_iid)
                        {
                            petmondata.atking.tar_iid = tar_iid;
                        }
                    }
                }
            }
        }

        return true;
    }

    public bool do_cast_self_skill(IBaseUnit caster, Variant rpc, bool precast = true, bool senderr = true)
    {
        var cur_clock_tm = DateTime.Now.ToBinary();
        var cast_skill_res = _cast_skill(cur_clock_tm, rpc, skill_type.ST_SELF, caster, null, null, precast);
        if (cast_skill_res == null)
            return false;

        if (cast_skill_res.res != game_err_code.RES_OK)
            return false;
        else if (cast_skill_res.rpc_data == null)
            return true;

        // send cast self skill msg
        //caster.gmap.broadcast_map_rpc(15, cast_skill_res.rpc_data); 
        caster.broad_cast_zone_msg_and_self(15, cast_skill_res.rpc_data);

        if (cast_skill_res.skill_level_conf.sres != null)
            Skill.apply_skill_eff_to(cur_clock_tm, caster, caster, cast_skill_res.skill_level_conf.sres, (int)skill_aff_type.SAT_SELF, rpc["sid"]._int32, cast_skill_res.percent);


        _post_cast_skill(cur_clock_tm, caster, rpc, cast_skill_res, null, null);

        //if("tres" in cast_skill_res.skill_lvl_data && "trang" in cast_skill_res.skill_lvl_data)
        //{
        //    foreach(tres in cast_skill_res.skill_lvl_data.tres)
        //    {
        //        apply_rang_eff(cur_clock_tm, caster, caster.get_pack_data(), tres, tres.aff, cast_skill_res.skill_lvl_data.trang, rpc.sid, cast_skill_res.percent);
        //    }
        //}

        return true;
    }


    public void update_skill_casting(IBaseUnit caster, IMapUnit pl, long cur_clock_tm)
    {
        if (pl.casting == null)
            return;

        var casting = pl.casting;
        if (cur_clock_tm < casting.end_tm)
        {
            return;
        }
        pl.casting = null;
        //delete pl.casting;

        var senderr = caster.get_sprite_type() == map_sprite_type.MstPlayer;
        switch (casting.tar_tp)
        {
            case (int)skill_type.ST_SELF:
                this.do_cast_self_skill(caster, casting.rpc, false, senderr);
                break;
            case (int)skill_type.ST_TARGET:
                this.do_cast_target_skill(caster, casting.rpc, false, senderr);
                break;
            case (int)skill_type.ST_GROUND:
                this.do_cast_ground_skill(caster, casting.rpc, false, senderr);
                break;
        }
    }

    public void update_skill_holding(IBaseUnit caster, IMapUnit pl, long cur_clock_tm)
    {
        if (pl.holding == null)
            return;

        var holding = pl.holding;
        if (cur_clock_tm > 0 && cur_clock_tm < holding.end_tm)
        {
            return;
        }

        var senderr = caster.get_sprite_type() == map_sprite_type.MstPlayer;
        switch (holding.tar_tp)
        {
            case (int)skill_type.ST_SELF:
                this.do_cast_self_skill(caster, holding.rpc, false, senderr);
                break;
            case (int)skill_type.ST_TARGET:
                this.do_cast_target_skill(caster, holding.rpc, false, senderr);
                break;
            case (int)skill_type.ST_GROUND:
                this.do_cast_ground_skill(caster, holding.rpc, false, senderr);
                break;
        }
    }

    public void update_jumping(IBaseUnit caster, IMapUnit pl, long cur_clock_tm)
    {
        if (pl.jumping == null)
            return;

        var jumping = pl.jumping;

        if (jumping.end_tm > cur_clock_tm)
        {
            long jumping_tm = cur_clock_tm - jumping.start_tm;
            pl.x += ((jumping.dest_x - pl.x) * jumping_tm / jumping.during_tm);
            pl.y += ((jumping.dest_y - pl.y) * jumping_tm / jumping.during_tm);

            //Utility.trace_info("pl.x["+pl.x+"] pl.y["+pl.y+"]\n");

            if (caster.get_sprite_type() == map_sprite_type.MstPlayer && !caster.is_in_lvl)
            {
                pl.lx = pl.x;
                pl.ly = pl.y;
            }

            return;
        }

        pl.x = jumping.dest_x;
        pl.y = jumping.dest_y;

        if (caster.get_sprite_type() == map_sprite_type.MstPlayer && !caster.is_in_lvl)
        {
            pl.lx = pl.x;
            pl.ly = pl.y;
        }

        int skill_id = jumping.rpc["sid"]._int32;

        if (jumping.jump.tres == null)
            return;

        if (jumping.jump.rang != null)
        {
            Skill.apply_rang_eff(cur_clock_tm, caster, caster.get_pack_data(), jumping.jump.tres, jumping.jump.rang, skill_id, jumping.percent);
        }
        else
        {
            IBaseUnit tar_sprite = caster.gmap.get_sprite_by_iid(pl.jumping.tar_iid);
            if (tar_sprite != null)
            {
                var tar_pl = tar_sprite.get_pack_data();

                var dist_x = tar_pl.x - pl.x;
                var dist_y = tar_pl.y - pl.y;

                if (dist_x * dist_x + dist_y * dist_y < 100 * 100)
                {
                    // 在范围内
                    foreach (var tres in jumping.jump.tres)
                        Skill.apply_skill_eff_to(cur_clock_tm, caster, tar_sprite, tres, tres.aff, skill_id, pl.jumping.percent);
                }
            }

        }
    }

    public void update_teleping(IBaseUnit caster, IMapUnit pl, long cur_clock_tm)
    {
        if (pl.teleping == null)
        {
            return;
        }

        if (pl.teleping.end_tm > cur_clock_tm)
        {
            return;
        }

        pl.x = pl.teleping.dest_x;
        pl.y = pl.teleping.dest_y;

        //Utility.trace_info("pl.x["+pl.x+"] pl.y["+pl.y+"]\n");
        if (caster.get_sprite_type() == map_sprite_type.MstPlayer && !caster.is_in_lvl)
        {
            pl.lx = pl.x;
            pl.ly = pl.y;
        }

        pl.teleping = null;
    }

    public void on_ply_dying(IBaseUnit ply, long cur_clock_tm)
    {
    }

    public void on_km(IBaseUnit killer, int mid)
    {
        if (this.kmtriggers.Count <= 0)
        {
            return;
        }

        // 更新触发器

        IMapUnit pl = null;
        if (killer != null)
        {
            pl = killer.get_pack_data();
        }

        List<int> to_rmv_tmtrgids = new List<int>();
        List<int> to_rmv_areatrgids = new List<int>();
        List<int> to_rmv_kmtrgids = new List<int>();
        List<int> to_rmv_uitmtrgids = new List<int>();
        List<int> to_rmv_mistrgids = new List<int>();
        List<int> to_add_trgconf = new List<int>();
        List<int> to_rmv_othertrgids = new List<int>();

        var need_sync_km_cnt = false;

        foreach (var trg in this.kmtriggers.Values)
        {
            // 触发
            if (trg.conf.km.mid != mid)
                continue;

            if (pl != null && (trg.conf.km.sideid != 0 && trg.conf.km.sideid != pl.lvlsideid))
                continue;

            if (killer != null && killer.get_sprite_type() == map_sprite_type.MstPlayer)
            {
                //if (!_trigger_attchk(killer, trg.conf))
                //{
                //    continue;
                //}
            }

            ++trg.kmcnt;



            if (trg.kmcnt < trg.tkmcnt)
            {
                continue;
            }

            trg.kmcnt = 0;
            --trg.cnt;

            if (trg.cnt <= 0)
            {
                to_rmv_kmtrgids.push(trg.conf.id);
            }

            this._trig_res(trg.conf, killer, to_add_trgconf, to_rmv_tmtrgids, to_rmv_areatrgids, to_rmv_kmtrgids, to_rmv_uitmtrgids, to_rmv_mistrgids, to_rmv_othertrgids);
        }



        foreach (var trid in to_rmv_tmtrgids)
        {

            //if (trid in this.tmtriggers) delete this.tmtriggers[trid];
        }
        foreach (var trid in to_rmv_areatrgids)
        {
            //if (trid in this.areatriggers) delete this.areatriggers[trid];
        }
        foreach (var trid in to_rmv_kmtrgids)
        {
            //if (trid in this.kmtriggers) delete this.kmtriggers[trid];
        }
        foreach (var trid in to_rmv_uitmtrgids)
        {
            //if (trid in this.useitmtriggers) delete this.useitmtriggers[trid];
        }
        foreach (var trid in to_rmv_mistrgids)
        {
            //if (trid in this.mistriggers) delete this.mistriggers[trid];
        }
        foreach (var trid in to_rmv_othertrgids)
        {
            //if (trid in this.othertriggers) delete this.othertriggers[trid];
        }
        foreach (var trid in to_add_trgconf)
        {
            this._add_triger(this.trigger_conf[trid]);
        }
    }

    public void _add_triger(trigger_conf trconf, long tm_elasped_s = 0)
    {

        //if ("timer" in trconf)
        //{
        //    //sys.trace(sys.SLT_DETAIL, "map["+this.mapid+"] add timer triger["+trconf.id+"]\n");

        //    local val = { tmleft = trconf.timer[0].tm - tm_elasped_s, cnt = trconf.timer[0].cnt, conf = trconf };
        //    this.tmtriggers[trconf.id] < -val;

        //    if ("showid" in trconf.timer[0])
        //    {
        //        // send add npc msg
        //        this.broadcast_map_rpc(6, { tmtrigs =[{ showid = trconf.timer[0].showid, tm = val.tmleft + sys.time()}]});
        //    }
        //}
        //else if ("area" in trconf)
        //{
        //    //sys.trace(sys.SLT_DETAIL, "map["+this.mapid+"] add area triger["+trconf.id+"]\n");

        //    this.areatriggers[trconf.id] < -trconf;
        //}
        //else 
        if (trconf.km != null)
        {
            //sys.trace(sys.SLT_DETAIL, "map["+this.mapid+"] add km triger["+trconf.id+"]\n");
            var tr_km_conf = trconf.km;
            var totalkmcnt = tr_km_conf.kmcnt;
            //if ("mulply" in tr_km_conf && tr_km_conf.mulply == 1 )
            //{
            //    totalkmcnt *= get_plycnt();
            //}
            this.kmtriggers[trconf.id] = new trigger_instance()
            {
                cnt = tr_km_conf.cnt,
                kmcnt = 0,
                tkmcnt = totalkmcnt,
                conf = trconf
            };

            //if ("showid" in trconf.km[0])
            //{
            //    // send add npc msg
            //    this.broadcast_map_rpc(6, { kmtrigs =[{ showid = tr_km_conf.showid, mid = tr_km_conf.mid, cnt = totalkmcnt, kmcnt = 0}]});
            //}
        }
        //else if ("useitm" in trconf)
        //{
        //    //sys.trace(sys.SLT_DETAIL, "map["+this.mapid+"] add useitm triger["+trconf.id+"]\n");

        //    this.useitmtriggers[trconf.id] < - { cnt = trconf.useitm[0].cnt, conf = trconf};
        //}
        //else if ("mis" in trconf)
        //{
        //    //sys.trace(sys.SLT_DETAIL, "map["+this.mapid+"] add useitm triger["+trconf.id+"]\n");

        //    this.mistriggers[trconf.id] < - { cnt = trconf.mis[0].cnt, conf = trconf};
        //}
        //else if ("other" in trconf)
        //{
        //    //sys.trace(sys.SLT_DETAIL, "map["+this.mapid+"] add other triger["+trconf.id+"]\n");

        //    this.othertriggers[trconf.id] < - { cnt = trconf.other[0].cnt, conf = trconf};
        //}
    }

    public void _trig_res(trigger_conf trconf, IBaseUnit trig_spr, List<int> to_add_trgconf, List<int> to_rmv_tmtrgids,
        List<int> to_rmv_areatrgids, List<int> to_rmv_kmtrgids, List<int> to_rmv_uitmtrgids,
        List<int> to_rmv_mistrgids, List<int> to_rmv_othertrgids)
    {
        //sys.trace(sys.SLT_DETAIL, "map["+this.mapid+"] _trig_res ["+trconf.id+"]\n");

        // 触发触发器
        if (trconf.rate > 0)
        {
            int judg = Utility.random(0, 100);
            //sys.trace(sys.SLT_DETAIL, "judg ["+judg+"] trig_res\n");
            if (judg > trconf.rate)
            {
                // 触发几率未命中，不产生效果
                Utility.trace_info("trigger miss trig_res\n");
                return;
            }
        }

        //if ("mod_monatt" in trconf )
        //{
        //    local mod_monatt_conf = trconf.mod_monatt[0];
        //    if ("matt" in mod_monatt_conf)
        //    {
        //        foreach (modatt in mod_monatt_conf.matt)
        //        {
        //            local adjust_att = { };
        //            if ("spwan_time" in modatt )
        //            {
        //                adjust_att.respawn_tm < -modatt.spwan_time;
        //            }
        //            if (adjust_att.len() > 0)
        //            {
        //                foreach (mon in this.map_mons)
        //                {
        //                    if (mon.mondata.mid == modatt.mid)
        //                    {
        //                        foreach (key, val in adjust_att )
        //                        {
        //                            mon.mondata[key] = val;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        if (trconf.addmon != null)
        {
            // 新增怪物
            var added_mon = this.add_init(trconf.addmon.map_mon);

            if (this.blvlmap && added_mon.Count > 0)
            {
                // 副本地图，根据难度调整怪物等级
                worldsvr._on_triger_addmons(added_mon);
            }
        }

        // 根据callmon触发结果新增添加或复活boss触发结果功能，若新增的话，新增的怪物是死的
        if (trconf.callmon != null)
        {
            // 召唤怪物
            //sys.trace(sys.SLT_DETAIL, "map["+this.mapid+"] callmon ["+trconf.id+"]\n");

            long cur_tm_s = Utility.time();
            //local /*call_mon*/ = [];
            var call_mon_conf = trconf.callmon;
            if (call_mon_conf.map_mon.Count > 0)
            {
                //local m;
                List<IBaseUnit> new_added_mons = new List<IBaseUnit>();
                foreach (var m_conf in call_mon_conf.map_mon)
                {
                    int mon_cnt = 0;
                    IBaseUnit mon = null;
                    foreach (var val in this.map_mons.Values)
                    {
                        IMapUnit mondata = val.get_pack_data();
                        if (mondata.mid == m_conf.mid && mondata.isdie)
                        {
                            mon = val;
                            break;
                        }
                    }
                    if (mon == null)
                    {
                        // 创建
                        //sys.trace(sys.SLT_DETAIL, "callmon["+m.mid+"], create\n");

                        bool bcrt = true;
                        if (this.callmon_added.ContainsKey(m_conf.mid))
                        {
                            if (this.callmon_added[m_conf.mid] >= call_mon_conf.mcnt.cnt)
                            {
                                bcrt = false; // 达到最大数量，不能创建
                            }
                        }

                        if (bcrt)
                        {
                            // 可以创建
                            mon = this.create_monster_byconf(m_conf);
                            if (mon == null)
                            {
                                Utility.trace_err("Err: in map [" + this.mapid + "] callmon mid [" + m_conf.mid + "] create error!\n");
                                return;
                            }

                            IMapUnit mondata = mon.get_pack_data();

                            mon.gmap = this;
                            mon.on_pos_change(mondata.x, mondata.y);

                            this.map_mons[mondata.iid] = mon;
                            this.map_sprites[mondata.iid] = mon;

                            List<IBaseUnit> list = null;
                            if (!this.map_mon_bymid.TryGetValue(mondata.mid, out list))
                            {
                                list = new List<IBaseUnit>();
                                this.map_mon_bymid.Add(mondata.mid, list);
                            }

                            list.Add(mon);

                            //mon.set_add_conf(m);

                            new_added_mons.push(mon);

                            if (this.callmon_added.ContainsKey(mondata.mid))
                                this.callmon_added[mondata.mid]++;
                            else
                                this.callmon_added[mondata.mid] = 1;
                        }
                    }
                    else
                    {
                        // 死亡则复活
                        if (mon.get_pack_data().isdie)
                        {
                            //sys.trace(sys.SLT_DETAIL, "callmon["+mon.mondata.mid+"], respawn\n");
                            mon.respawn(100, true);
                        }
                        else
                        {
                            //sys.trace(sys.SLT_DETAIL, "callmon["+mon.mondata.mid+"], nothing\n");
                        }
                    }

                    //if (("clanbcast" in m) && (m.clanbcast == 1))
                    //{
                    //    // 需要在全帮派广播怪物召唤消息
                    //    if (trig_spr && (trig_spr.get_sprite_type() == map_sprite_type.MST_PLAYER) && ("clanid" in trig_spr.pinfo))
                    //    {
                    //        // broad cast clan_msg msg
                    //        _broad_cast_clan_msg(trig_spr.pinfo.clanid, 226, { tp = 5, callmid = m.mid, cid = trig_spr.pinfo.cid, name = trig_spr.pinfo.name}, clan_c_tp.CCT_NONE);
                    //    }
                    //}
                }

                if (this.blvlmap && new_added_mons.Count > 0)
                {
                    // 副本地图，根据难度调整怪物等级
                    worldsvr._on_triger_addmons(new_added_mons);
                }
            }
            //else
            //{
            //    //sys.trace(sys.SLT_ERR, "map["+this.mapid+"] callmon ["+trconf.id+"] call_mon_conf without m\n");
            //}
        }

        //if (this.blvlmap && ("call_city_mon" in trconf) )
        //{
        //    if (worldsvr.clter_conf && worldsvr.clter_conf.tp == clan_teritory_type.CTT_WAR_TERRITORY)
        //    {
        //        local clterid = worldsvr.clter_conf.id;
        //        local call_city_mon_conf = trconf.call_city_mon[0];

        //        local glbdata_mgr = global_data_mgrs.glbdata;
        //        glbdata_mgr.lock_data(db_glbdata_datatype.DBGDT_CLAN_TER, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock
        //        local gld_clanter_data = _get_clanter_data();
        //        if (gld_clanter_data)
        //        {
        //            local glb_clanter_info = gld_clanter_data.get_data();
        //            if (clterid in glb_clanter_info.data )
        //            {
        //                local clanter_info = glb_clanter_info.data[clterid];
        //                if (clanter_info && !clanter_info.onwar && clanter_info.clanid > 0 &&
        //                    ("stastic_cost" in clanter_info) && ("cost_callmon" in clanter_info.stastic_cost) )
        //                {
        //                    //sys.dumpobj( clanter_info.cost );     
        //                    local cost_callmon_data = clanter_info.stastic_cost.cost_callmon;
        //                    if (call_city_mon_conf.cost_tp == "yb")
        //                    {
        //                        if (cost_callmon_data.yb > call_city_mon_conf.min_cost)
        //                        {
        //                            local call_mon_cost = this.call_cost_monster(call_city_mon_conf, call_city_mon_conf.cost_tp, cost_callmon_data.yb);
        //                            if (call_mon_cost > 0)
        //                            {
        //                                cost_callmon_data.yb -= call_mon_cost;
        //                                //sys.trace(sys.SLT_DETAIL, "call_city_mon call_mon_cost yb=["+call_mon_cost+"]\n");    
        //                                gld_clanter_data.mod_data(glb_clanter_info);
        //                                gld_clanter_data.db_update();
        //                            }
        //                        }
        //                    }
        //                    else if (call_city_mon_conf.cost_tp == "gold")
        //                    {
        //                        if (cost_callmon_data.gld > call_city_mon_conf.min_cost)
        //                        {
        //                            local call_mon_cost = this.call_cost_monster(call_city_mon_conf, call_city_mon_conf.cost_tp, can_use_gold);
        //                            if (call_mon_cost > 0)
        //                            {
        //                                cost_callmon_data.gld -= call_mon_cost;
        //                                //sys.trace(sys.SLT_DETAIL, "call_city_mon call_mon_cost gld=["+call_mon_cost+"]\n");  
        //                                gld_clanter_data.mod_data(glb_clanter_info);
        //                                gld_clanter_data.db_update();
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        glbdata_mgr.unlock_data(db_glbdata_datatype.DBGDT_CLAN_TER);  // 解锁                   
        //    }
        //}

        //if ("addbzone" in trconf )
        //{
        //    // 新增动态障碍
        //    foreach (addb in trconf.addbzone)
        //    {
        //        this.add_bzone(addb.id, addb.x, addb.y, addb.w, addb.h);
        //    }
        //}

        //if ("rmvbzone" in trconf )
        //{
        //    // 删除动态障碍
        //    foreach (rmvb in trconf.rmvbzone)
        //    {
        //        this.rmv_bzone(rmvb.id);
        //    }
        //}

        if (!string.IsNullOrEmpty(trconf.addtriger))
        {
            // 新增触发器
            foreach (var tx in trconf.addtriger.Split(','))
            {
                int tid = 0;
                int.TryParse(tx, out tid);
                if (tid > 0 && this.trigger_conf.ContainsKey(tid))
                {
                    to_add_trgconf.push(tid);
                }
            }
        }

        //if ("ran_addtr" in trconf)
        //{
        //    // 随机新增触发器
        //    local ran_addtr_conf = trconf.ran_addtr[0];
        //    local add_idxs = _n_choose_n(ran_addtr_conf.addtr, ran_addtr_conf.cnt);
        //    foreach (idx in add_idxs)
        //    {
        //        // 新增触发器                
        //        local add_ids = ran_addtr_conf.addtr[idx].ids;
        //        //sys.dumpobj( add_ids );
        //        foreach (addt in add_ids)
        //        {
        //            if (addt in this.trigger_conf ) 
        //            {
        //                to_add_trgconf.push(addt);
        //            }
        //        }
        //    }
        //}
        if (!string.IsNullOrEmpty(trconf.rmvtriger))
        {
            // 移除触发器
            foreach (string sx in trconf.rmvtriger.Split(','))
            {
                int rmvt = 0;
                int.TryParse(sx, out rmvt);
                if (rmvt <= 0)
                    continue;

                //if (rmvt in this.tmtriggers)
                //{
                //    to_rmv_tmtrgids.push(rmvt);
                //}
                //else if (rmvt in this.areatriggers)
                //{
                //    to_rmv_areatrgids.push(rmvt);
                //}
                //else 
                if (this.kmtriggers.ContainsKey(rmvt))
                {
                    to_rmv_kmtrgids.push(rmvt);
                }

                //else if (rmvt in this.useitmtriggers)
                //{
                //    to_rmv_uitmtrgids.push(rmvt);
                //}
                //else if (rmvt in this.mistriggers)
                //{
                //    to_rmv_mistrgids.push(rmvt);
                //}
                //else if (rmvt in this.othertriggers)
                //{
                //    to_rmv_othertrgids.push(rmvt);
                //}
            }
        }

        //if ("addnpc" in trconf)
        //{
        //    sys.trace(sys.SLT_DETAIL, "map[" + this.mapid + "] addnpc [" + trconf.id + "]\n");

        //    local cur_tm_s = sys.time();

        //    local add_npc = [];

        //    foreach (addnpc in trconf.addnpc)
        //    {
        //        local tmed_addnpc = addnpc;
        //        if (addnpc.dis_tm > 0)
        //        {
        //            tmed_addnpc = sys.deep_clone(addnpc);
        //            tmed_addnpc.dis_tm += cur_tm_s;
        //        }

        //        this.add_npcs.push(tmed_addnpc);
        //        add_npc.push(tmed_addnpc);
        //    }

        //    // broad add npc msg
        //    this.broadcast_map_rpc(6, { npcs = add_npc});

        //    //sys.dumpobj(add_npc);
        //}

        //if ("addlink" in trconf)
        //{
        //    //sys.trace(sys.SLT_DETAIL, "map["+this.mapid+"] addlink ["+trconf.id+"]\n");

        //    local cur_tm_s = sys.time();

        //    local addlinks = [];

        //    foreach (addlink in trconf.addlink)
        //    {
        //        local tmed_addlink = addlink;
        //        if (("dis_tm" in addlink) && addlink.dis_tm > 0)
        //        {
        //            tmed_addlink = sys.deep_clone(addlink);
        //            tmed_addlink.dis_tm += cur_tm_s;
        //        }

        //        tmed_addlink.to < -tmed_addlink.gto;
        //        this.add_links.push(tmed_addlink);
        //        addlinks.push(tmed_addlink);
        //    }

        //    // broad add npc msg
        //    this.broadcast_map_rpc(6, { links = addlinks});

        //    //sys.dumpobj(addlinks);
        //}

        //if ("rmvlink" in trconf)
        //{
        //    // TO DO : remove link
        //}

        //if ("modtrgtm" in trconf)
        //{
        //    // 修改倒计时触发器触发时间

        //    local cur_tm_s = sys.time();

        //    local tmtrigs = [];

        //    foreach (modtrgtm_conf in trconf.modtrgtm)
        //    {
        //        foreach (trid, trg in this.tmtriggers)
        //        {
        //            if (trid != modtrgtm_conf.tarid)
        //            {
        //                continue;
        //            }

        //            trg.tmleft += modtrgtm_conf.tmadd;

        //            if ("showid" in trg.conf.timer[0])
        //            {
        //                tmtrigs.push({ showid = trg.conf.timer[0].showid, tm = trg.tmleft + cur_tm_s});
        //            }
        //        }
        //    }

        //    if (tmtrigs.len() > 0)
        //    {
        //        // broad add npc msg
        //        this.broadcast_map_rpc(6, { tmtrigs = tmtrigs});
        //    }
        //}

        //if ("bcast" in trconf)
        //{
        //    // broad sys msg
        //    this.broadcast_map_rpc(160, { tp = 6, spec ={ tp = bcast_msg_tp.MAP_TRIGER_MSG, par = trconf.bcast}, msg = ""});
        //}

        //if (this.blvlmap)
        //{
        //    //sys.trace(sys.SLT_DETAIL, "map["+this.mapid+"] _trig_res ["+trconf.id+"] blvlmap with trig_spr["+trig_spr+"]\n");

        //    if (trig_spr)
        //    {
        //        if ("modatt" in trconf)
        //        {
        //            // 修改属性，如荣誉、经验值、游戏币等
        //            worldsvr._trig_modatt(trig_spr, trconf.modatt[0]);
        //        }

        //        if ("addstat" in trconf)
        //        {
        //            // 加buffer
        //            worldsvr._trig_addstat(trig_spr, trconf.addstat);
        //        }
        //    }
        //    else
        //    {
        //        if ("modatt" in trconf)
        //        {
        //            // 修改属性，如荣誉、经验值、游戏币等
        //            worldsvr._trig_modatt(null, trconf.modatt[0]);
        //        }
        //    }

        //    if ("vkm" in trconf)
        //    {
        //        // 虚拟杀怪
        //        worldsvr._trig_vkm(trig_spr, trconf.vkm, this);
        //    }

        //    if ("act_finchk" in trconf)
        //    {
        //        // 副本结束
        //        worldsvr._trig_active_finchk(trconf.act_finchk == 1);
        //    }

        //    if ("finlvl" in trconf)
        //    {
        //        // 副本结束
        //        worldsvr._trig_finlvl(trig_spr, trconf.finlvl, this);
        //    }
        //}
    }



    public born_pos get_born_area()
    {
        return new born_pos();
    }

    public bool is_player_in_map(int sid)
    {
        return this.map_players_bysid.ContainsKey(sid);
    }

    public bool is_player_in_map_bycid(int cid)
    {
        return this.map_players_bycid.ContainsKey(cid);
    }

    /*      
*      |           *
*      |            *
* -----O------------*---->X
*      | \ )alpha   *
*      |  \        *
*      |   \      *
*      \/   \   *
*      Y     *
*/
    public Point2D valpoint_on_round(double center_x, double center_y, double radius, double alpha)
    {
        //Utility.trace_info("valpoint_on_round("+alpha+")\n");
        Point2D ret = null;
        double alpha_offset = 0;
        for (int i = 0; i < 180; i += 5)
        {
            alpha_offset = (Math.PI * i / 180);
            if (i % 2 == 0)
            {
                alpha_offset = -alpha_offset;
            }
            var dest_x = (center_x + radius * Math.Cos(alpha + alpha_offset));
            var dest_y = (center_y + radius * Math.Sin(alpha + alpha_offset));
            var grid_pos = this.get_grid_by_pt(dest_x, dest_y);
            if (grid_pos != null && is_grid_walkableEx((int)grid_pos.x, (int)grid_pos.y))
            {
                ret = new Point2D(dest_x, dest_y);
                //Utility.trace_info("valpoint_on_round["+grid_pos.x+","+grid_pos.y+"], i="+i+" angle["+((alpha + alpha_offset)/(2*PI)*360)+"]\n");
                break;
            }
        }
        return ret;
    }

    public bool _is_pos_in_mvpath(int x, int y, IMapUnit pl, int dist_allow)
    {
        // 判断当前位置
        //Utility.trace_info("_is_pos_in_mvpath x["+x+"] y["+y+"]\n");

        double dist_x = x - pl.x;
        double dist_y = y - pl.y;

        //Utility.trace_info("pl x["+pl.x+"] y["+pl.y+"], dist_x["+dist_x+"] dist_y["+dist_y+"] \n");
        if (dist_allow * dist_allow >= dist_x * dist_x + dist_y * dist_y)
        {
            //Utility.trace_info("cur pos varified\n");
            return true;
        }

        var max_verify_pt = 20; // 最多20个验证点，共20*game_const.map_grid_pixel=640 或 45*20=900 的距离范围
        var verify_cnt = 0;

        if (pl.moving != null)
        {
            if (pl.moving.ppts != null)
            {
                //Utility.trace_info("pl.moving.ppts len["+pl.moving.ppts.Count+"] \n");

                verify_cnt = 0;
                var dist_allow_pow = 1024;       // 1格范围之内 32*game_const.map_grid_pixel
                for (var idx = pl.moving.ppts.Count - 1; idx >= 0; --idx)
                {
                    var pt = pl.moving.ppts[idx];

                    dist_x = x - (pt.x * game_const.map_grid_pixel + 16);
                    dist_y = y - (pt.y * game_const.map_grid_pixel + 16);

                    //Utility.trace_info("ppts pt idx["+idx+"] x["+pt.x+"] y["+pt.y+"], dist_x["+dist_x+"] dist_y["+dist_y+"] \n");

                    if (dist_allow_pow >= dist_x * dist_x + dist_y * dist_y)
                    {
                        //Utility.trace_info("moving ppts idx["+idx+"] varified\n");
                        return true;
                    }

                    ++verify_cnt;
                    if (verify_cnt >= max_verify_pt)
                    {
                        //Utility.trace_info("moving ppts idx["+idx+"] varify failed\n");
                        return false;
                    }
                }
            }
            else
            {
                //方向移动 以当前点为准
            }

            return false;
        }

        return false;
    }

    public IBaseUnit create_monster_byconf(map_mon_conf conf)
    {
        monsterconf mon_conf = Utility.get_monster_conf(conf.mid);
        if (null == mon_conf)
            throw new Exception("monsterconf not found for mid:" + conf.mid);

        Monster mon = new Monster(mon_conf);

        mon.set_location(conf.x, conf.y);
        mon.set_origin_location(conf.r_x, conf.r_y);
        mon.set_lvlside(conf.sideid);
        if (conf.spwan_time > 0)
            mon.set_conf_respawn_tm(conf.spwan_time);

        return mon;
    }

    public IBaseUnit new_pet_mon(IBaseUnit ply, map_mon_conf monconf)
    {
        // 分配战斗宠物实例
        IBaseUnit mon = null;
        IMapUnit mondata = null;

        List<IBaseUnit> list = null;
        this.petmon_cache.TryGetValue(monconf.mid, out list);

        if (list == null || list.Count <= 0)
        {
            mon = this.create_monster_byconf(monconf);
            if (mon == null)
            {
                Utility.trace_err("Err: in map [" + this.mapid + "] add_pet_mon mid [" + monconf.mid + "] create error!\n");
                return mon;
            }

            mon.gmap = this;

            mondata = mon.get_pack_data();
            this.map_mons[mondata.iid] = mon;
            this.map_sprites[mondata.iid] = mon;

            list = null;
            if (this.map_mon_bymid.TryGetValue(mondata.mid, out list))
                list.Add(mon);
            else
            {
                list = new List<IBaseUnit>();
                list.Add(mon);
                this.map_mon_bymid[mondata.mid] = list;
            }
        }
        else
        {
            mon = list.pop<IBaseUnit>();
            mondata = mon.get_pack_data();
        }

        IMapUnit pl = ply.get_pack_data();
        var ply_grid = this.get_grid_by_pt(pl.x, pl.y);
        mondata.org_init_x = (int)ply_grid.x;
        mondata.org_init_y = (int)ply_grid.y;
        mon.owner_ply = ply;
        mondata.owner_cid = pl.cid;
        mon.respawn(100, false);

        //var dest_pos = line_find_canwalk_grid(ply_grid.x, ply_grid.y, 3, 9);
        //mon._move_to(dest_pos.x, dest_pos.y);

        return mon;
    }

    public void release_pet_mon(IBaseUnit mon)
    {
        // 回收战斗宠物实例
        IMapUnit mondata = mon.get_pack_data();
        List<IBaseUnit> list = null;
        if (this.petmon_cache.TryGetValue(mondata.mid, out list))
            list.Add(mon);
        else
        {
            list = new List<IBaseUnit>();
            list.Add(mon);
            this.petmon_cache.Add(mondata.mid, list);
        }
    }

    public bool is_player_in_pc_zone(IBaseUnit ply)
    {
        if (this.map_conf.pk_zone.Count <= 0)
            return false;

        IMapUnit pl = ply.get_pack_data();
        foreach (var z in map_conf.pk_zone)
        {
            if (pl.x > z.x && pl.x < z.x + z.width &&
                pl.y > z.y && pl.y < z.y + z.height)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid_x">格子x</param>
    /// <param name="grid_y">格子y</param>
    /// <param name="radian"></param>
    /// <returns></returns>
    public Point2D find_furthest_canwalk_grid(int grid_x, int grid_y, double radian)
    {
        var ori_cos = Math.Cos(radian);
        var ori_sin = Math.Cos(radian);
        var ori_tan = ori_sin / ori_cos;

        var incr_x = ori_cos >= 0 ? 1 : -1;
        var incr_y = ori_sin >= 0 ? 1 : -1;

        var tmp = Math.Abs(ori_tan);
        var not_cross = Math.Abs(tmp - 1) >= 0.01; //是否对角线  0.01为容错值
        var main_y = tmp > 1;
        //sys.trace( sys.SLT_ERR, "find_furthest_canwalk_grid radian=[" + radian + "]    ori_tan=[" + ori_tan + "]   ori_cos=[" + ori_cos + "]  ori_sin=[" + ori_sin + "] tmp=[" + tmp + "]\n" );
        //以变化大的为基准  依次遍历经过的所有格子（与直线相交的）
        var grid_fir = grid_x;
        var grid_sec = grid_y;
        var incr_fir = incr_x;
        var incr_sec = incr_y;
        var radio = ori_tan;
        if (main_y)
        {
            grid_fir = grid_y;
            grid_sec = grid_x;
            incr_fir = incr_y;
            incr_sec = incr_x;
            radio = 1 / ori_tan;
        }

        var add_grid_fir = 0;
        double add_grid_sec = 0;
        while (true)
        {
            add_grid_fir += incr_fir;
            tmp = (add_grid_fir * radio);
            if (not_cross)
            {
                if (tmp != add_grid_sec)
                {
                    if (tmp - add_grid_sec != incr_sec)
                    {
                        //                        sys.trace( sys.SLT_ERR, "find_furthest_canwalk_grid radian=[" + radian + "] add_grid_fir=[" + add_grid_fir + "]  add_grid_sec=[" + add_grid_sec + "]  incr_fir=[" + incr_fir + "] incr_sec=[" + incr_sec + "]\n" );
                        //                        sys.trace( sys.SLT_ERR, " grid_x=[" + grid_x + "] grid_y=[" + grid_y + "] ori_tan=[" + ori_tan + "] radio=[" + radio + "] tmp=[" + tmp + "] \n" );
                    }

                    add_grid_sec = tmp;
                    add_grid_fir -= incr_fir; //退格
                }
            }
            else
            {
                add_grid_sec += incr_sec;
            }

            //sys.trace( sys.SLT_ERR, "get_ray_end_grid main_y=[" + main_y + "] add_grid_fir=[" + add_grid_fir + "]  add_grid_sec=[" + add_grid_sec + "]  incr_fir=[" + incr_fir + "] incr_sec=[" + incr_sec + "]\n" );
            if (main_y)
            {
                if (!is_grid_walkableEx((int)(grid_sec + add_grid_sec), grid_fir + add_grid_fir))
                {
                    break;
                }
                grid_x = (int)(grid_sec + add_grid_sec);
                grid_y = (int)(grid_fir + add_grid_fir);
            }
            else
            {
                if (!is_grid_walkableEx(grid_fir + add_grid_fir, (int)(grid_sec + add_grid_sec)))
                {
                    break;
                }
                grid_y = (int)(grid_sec + add_grid_sec);
                grid_x = grid_fir + add_grid_fir;
            }
        }

        return new Point2D() { x = grid_x, y = grid_y };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from_x">格子x</param>
    /// <param name="from_y">格子y</param>
    /// <param name="minr"></param>
    /// <param name="maxr"></param>
    /// <param name="max_cnt"></param>
    public Point2D line_find_canwalk_grid(int from_x, int from_y, double minr, double maxr, int max_cnt = 10)
    {
        var dist_x = Utility.random(minr, maxr);
        var add_x = -1;
        if (Utility.random(0, 2) > 0)
        {
            dist_x = -dist_x;
            add_x = 1;
        }
        var dist_y = Utility.random(minr, maxr);
        var add_y = -1;
        if (Utility.random(0, 2) > 0)
        {
            dist_y = -dist_y;
            add_y = 1;
        }
        int dest_x = from_x;
        int dest_y = from_y;
        while (dist_x != 0 && dist_y != 0)
        {
            dest_x = (int)(from_x + dist_x);
            dest_y = (int)(from_y + dist_y);
            if (is_grid_walkableEx(dest_x, dest_y))
            {
                break;
            }

            if (is_grid_walkableEx(dest_x + add_x, dest_y))
            {
                dest_x += add_x;
                break;
            }

            if (is_grid_walkableEx(dest_x, dest_y + add_y))
            {
                dest_y += add_y;
                break;
            }

            max_cnt--;
            if (max_cnt <= 0)
            {
                dest_x = from_x;
                dest_y = from_y;
                break;
            }

            dist_x += add_x;
            dist_y += add_y;
        }

        return new Point2D() { x = dest_x, y = dest_y };
    }

    public List<IBaseUnit> add_init(List<map_mon_conf> conf, long tm_elasped_s = 0)
    {
        List<IBaseUnit> added_mon = new List<IBaseUnit>();
        int add_mon_cnt = 0;
        int totalcnt = conf.Count;
        foreach (map_mon_conf amconf in conf)
        {
            IBaseUnit m = this.create_monster_byconf(amconf);
            if (m == null)
            {
                Utility.trace_err("Err: in map [" + mapid + "] add_mon mid [" + amconf.mid + "] create error!\n");
                continue;
            }

            m.gmap = this;
            IMapUnit mondata = m.get_pack_data();
            m.on_pos_change(mondata.x, mondata.y);

            this.map_mons[mondata.iid] = m;
            this.map_sprites[mondata.iid] = m;

            List<IBaseUnit> list = null;
            if (this.map_mon_bymid.TryGetValue(mondata.mid, out list))
                list.Add(m);
            else
            {
                list = new List<IBaseUnit>();
                list.Add(m);

                this.map_mon_bymid.Add(mondata.mid, list);
            }

            if (amconf.sideid > 0)
                mondata.lvlsideid = amconf.sideid;
            //m.set_add_conf(amconf);

            added_mon.push(m);

            ++add_mon_cnt;
            if (add_mon_cnt >= totalcnt)
            {
                break;
            }


            //Utility.trace_info("map [" + mapid +"] with ["+add_mon_cnt+"] add_mon created\n");
        }

        // TO DO : added trigger
        //    if ("trigger" in  conf)
        //{
        //        //Utility.trace_info("tm_elasped_s [" + tm_elasped_s +"] \n");
        //        this.init_trigger(conf.trigger, tm_elasped_s);
        //    }

        return added_mon;
    }


    public link_conf get_link_by_gto(int gto)
    {
        foreach (var l in this.map_conf.link)
        {
            if (l.gto == gto)
                return l;
        }

        return null;
    }




    public void update(IService game_ref, long tm_elasped_s)
    {
        //sys.trace(sys.SLT_SYS, "sg_map update\n");

        if (map_fined)
        {
            Utility.trace_err("err:gmap is finished mapid=[" + this.mapid + "]\n");
            return;
        }

        var cur_tm_s = Utility.time();
        var players_info = game_ref.sgplayers;

        //this.update_flys(cur_clock_tm);

        //this._update_dpitm(cur_tm_s);
        //this._update_team_dpitm(cur_tm_s);

        //// update players
        //var i = 0;
        //for(i=0;i<this.get_player_count();++i)
        //{
        //    var cid = this.get_player_cid(i);
        //    var latency = svr.get_session_latency(cid);

        //    //sys.trace(sys.SLT_SYS, "cur systm time ["+cur_clock_tm+"]ms\n");
        //    //sys.trace(sys.SLT_SYS, "player["+cid+"] latency ["+latency+"]ms\n");

        //    if(!(cid in players_info))
        //    {
        //        continue;
        //    }

        //    players_info[cid].update(game_ref, cur_clock_tm);
        //    //players_info[cid].update_move(game_ref, cur_clock_tm);
        //    //players_info[cid].update_atk(cur_clock_tm);
        //}

        //// update monsters
        //foreach(idx, val in map_mons)
        //{
        //    val.update(game_ref, cur_clock_tm);
        //    //val.update_move(cur_clock_tm);
        //    //val.update_atk(cur_clock_tm);
        //    //val.think(cur_clock_tm);
        //}

        // update sprites
        var time_trace_ms = Utility.time();
        foreach (var val in this.map_sprites.Values)
        {
            // logical update
            val.update(game_ref, cur_tm_s, tm_elasped_s);
        }
        time_trace_ms = Utility.time() - time_trace_ms;
        if (time_trace_ms > 200)
        {
            Utility.trace_info("map id[" + this.mapid + "] sprites len[" + this.map_sprites.Count + "] update cost time[" + time_trace_ms + "]\n");
        }

        // calculate per sprite zone change
        //foreach(idx, val in this.map_sprites)
        //{
        //    val.calc_zone_sprite();
        //}
        //this.calc_zone_sprite();

        //    foreach (idx, val in this.map_sprites)
        //{
        //        if (val.get_sprite_type() == map_sprite_type.MstPlayer)
        //        {
        //            // player character
        //            var lvz_iids = val.get_levz_iids();
        //            if (lvz_iids.Count > 0)
        //            {
        //            ::send_rpc(val.pinfo.sid, 56, { iidary = lvz_iids});   // send sprite leave zone to player
        //                if (val.pinfo.marryid > 0 && val.pinfo.mate_iid != 0)
        //                {
        //                    foreach (iid in lvz_iids)
        //                    {
        //                        if (val.pinfo.mate_iid == iid)
        //                        {
        //                            val.set_mate_iid(0);
        //                            break;
        //                        }
        //                    }
        //                }
        //            }

        //            var new_plys = val.get_new_inz_vsb_plys();
        //            var new_mons = val.get_new_inz_vsb_mons();

        //            //Utility.trace_info("player sid ["+val.pinfo.sid+"] on new plys:\n");
        //            //sys.dumpobj(new_plys);

        //            if (new_plys.Count > 0)
        //            {
        //                var plys_data = { pary =[] };
        //                foreach (idx,ply in new_plys)
        //            {
        //                    plys_data.pary.push(ply.pinfo);
        //                }

        //            ::send_rpc(val.pinfo.sid, 54, plys_data);   // send player enter zone to player

        //                //配偶是否进入视野
        //                if (val.pinfo.marryid > 0 && val.pinfo.mate_iid == 0)
        //                {
        //                    foreach (ply in new_plys)
        //                    {
        //                        if (ply.pinfo.cid == val.pinfo.mate_cid)
        //                        {
        //                            val.set_mate_iid(ply.pinfo.iid);
        //                            break;
        //                        }
        //                    }
        //                }
        //            }

        //            if (new_mons.Count > 0)
        //            {
        //                var mons_data = { monsters =[] };
        //                foreach (idx,mon in new_mons)
        //            {
        //                    if (!(mon.mondata.dieshow) && (mon.isdie()))
        //                    {
        //                        continue;
        //                    }

        //                    mons_data.monsters.push(mon.mondata);
        //                }

        //            ::send_rpc(val.pinfo.sid, 55, mons_data);   // send monster enter zone to player
        //            }
        //        }

        //        val.apply_new_inz_spr();
        //    }

        // 更新触发器

        //var to_rmv_tmtrgids = [];
        //var to_rmv_areatrgids = [];
        List<int> to_rmv_kmtrgids = new List<int>();
        //var to_rmv_uitmtrgids = [];
        //var to_rmv_mistrgids = [];
        List<int> to_add_trgconf = new List<int>();
        //var to_rmv_othertrgids = [];

        //foreach (trid, trg in this.tmtriggers)
        //{
        //    trg.tmleft -= tm_elasped_s;
        //    if (trg.tmleft > 0)
        //    {
        //        continue;
        //    }

        //    // 触发

        //    --trg.cnt;
        //    trg.tmleft = trg.conf.timer[0].tm;

        //    if (trg.cnt <= 0)
        //    {
        //        to_rmv_tmtrgids.push(trid);
        //    }

        //    this._trig_res(trg.conf, null, to_add_trgconf, to_rmv_tmtrgids, to_rmv_areatrgids, to_rmv_kmtrgids, to_rmv_uitmtrgids, to_rmv_mistrgids, to_rmv_othertrgids);
        //}

        //foreach (trid, trg in this.areatriggers)
        //{
        //    foreach (ply in map_players)
        //    {
        //        if (trg.area[0].sideid != 0 && trg.area[0].sideid != ply.pinfo.lvlsideid)
        //        {
        //            continue;
        //        }

        //        if (ply.pinfo.x > trg.area[0].x && ply.pinfo.y > trg.area[0].y && ply.pinfo.x < trg.area[0].x + trg.area[0].width && ply.pinfo.y < trg.area[0].y + trg.area[0].height)
        //        {
        //            // 触发

        //            if (!_trigger_attchk(ply, trg))
        //            {
        //                continue;
        //            }

        //            to_rmv_areatrgids.push(trid);

        //            this._trig_res(trg, ply, to_add_trgconf, to_rmv_tmtrgids, to_rmv_areatrgids, to_rmv_kmtrgids, to_rmv_uitmtrgids, to_rmv_mistrgids, to_rmv_othertrgids);
        //        }
        //    }
        //}

        // 间隔判断是否经过了自然日（参考player的sync_db_data实现），若经过了自然日，则重置每日重复触发器
        //if ((cur_tm_s > this.last_check_tm + 5)) // 5秒一次
        //{
        //    if (this.last_check_tm > 0)
        //    {
        //        var local_tm = sys.trans_local_time(cur_tm_s);
        //        var last_trg_tm = sys.trans_local_time(this.last_check_tm);

        //        if ((_compare_ymd_tm(local_tm, last_trg_tm) > 0))
        //        {
        //            foreach (trid, trconf in this.trigger_conf)
        //            {
        //                if ("dalyrep" in trconf)
        //                {
        //                    // 重置每日重复触发器
        //                    //Utility.trace_info("reset daly trigger!\n");

        //                    if (trid in this.tmtriggers) 
        //                    {
        //                        this.tmtriggers[trid].cnt = trconf.dalyrep;
        //                    }
        //                    else if (trid in this.areatriggers) 
        //                    {
        //                        to_rmv_areatrgids.push(trid);
        //                    }
        //                    else if (trid in this.kmtriggers) 
        //                    {
        //                        this.kmtriggers[trid].cnt = trconf.dalyrep;
        //                    }
        //                    else if (trid in this.useitmtriggers) 
        //                    {
        //                        this.useitmtriggers[trid].cnt = trconf.dalyrep;
        //                    }
        //                    else if (trid in this.mistriggers) 
        //                    {
        //                        this.mistriggers[trid].cnt = trconf.dalyrep;
        //                    }
        //                    else if (trid in this.othertriggers) 
        //                    {
        //                        this.othertriggers[trid].cnt = trconf.dalyrep;
        //                    }
        //                    else
        //                    {
        //                        to_add_trgconf.push(trid);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    this.last_check_tm = cur_tm_s;
        //}

        //foreach (trid in to_rmv_tmtrgids)
        //{
        //    if (trid in this.tmtriggers) delete this.tmtriggers[trid];
        //}
        //foreach (trid in to_rmv_areatrgids)
        //{
        //    if (trid in this.areatriggers) delete this.areatriggers[trid];
        //}

        foreach (var trid in to_rmv_kmtrgids)
        {
            if (this.kmtriggers.ContainsKey(trid))
                this.kmtriggers.Remove(trid);

        }
        //foreach (trid in to_rmv_uitmtrgids)
        //{
        //    if (trid in this.useitmtriggers) delete this.useitmtriggers[trid];
        //}
        //foreach (trid in to_rmv_mistrgids)
        //{
        //    if (trid in this.mistriggers) delete this.mistriggers[trid];
        //}
        //foreach (trid in to_rmv_othertrgids)
        //{
        //    if (trid in this.othertriggers) delete this.othertriggers[trid];
        //}

        foreach (var trid in to_add_trgconf)
        {
            this._add_triger(this.trigger_conf[trid]);
        }

        // 更新动态npc列表
        //for (var i = 0; i < this.add_npcs.Count; ++i)
        //{
        //    var npc = this.add_npcs[i];
        //    if (npc.dis_tm != 0 && npc.dis_tm <= cur_tm_s)
        //    {
        //        this.add_npcs.remove(i);
        //        --i;
        //        continue;
        //    }
        //}

        // 更新动态npc列表
        for (var i = 0; i < this.add_links.Count; ++i)
        {
            var link = this.add_links[i];
            //if (link.dis_tm != 0 && link.dis_tm <= cur_tm_s)
            {
                this.add_links.RemoveAt(i);
                --i;
                continue;
            }
        }

        // 更新地图技能状态对象
        //        for (var i = 0; i < this.mapstats.Count; ++i)
        //        {
        //            var mapstat = this.mapstats[i];
        //            var needrmv = _update_mapstat(mapstat, cur_tm_s, cur_clock_tm);
        //            if (needrmv)
        //            {
        //                this.mapstats.remove(i);
        //                --i;

        //                // bcast add_npcs msg
        //                this.broadcast_map_rpc(6, { mapstat = 2, mapstatrmv =[{ id = mapstat.id}]});

        //        continue;
        //    }
        //}
    }





    public void set_pk_seting(map_pk_setting_type pks)
    {
        this.pk_seting = pks;
    }
    public void reset_pk_seting()
    {
        this.pk_seting = this.origin_pk_seting;
    }

    public void debug_map()
    {
        throw new NotImplementedException();
        //    if (this.mapid != 1) return;

        //Utility.debug("map[" + this.mapid + "] width[" + this.get_width() + "] height[" + this.get_height() + "]");
        //    for (var y = 0; y < this.get_height(); y++)
        //    {
        //        for (var x = 0; x < this.get_width(); x++)
        //        {
        //        Utility.debug("idx[" + (x + y * this.get_width()) + "], x[" + x + "], y[" + y + "], walkable[" + is_grid_walkableEx(x, y) + "]");
        //        }
        //    }

        //    var path = find_path(100, 10, 44, 15);

        //    if (path != null && path.Count > 0)
        //    {
        //        var i = 0;
        //        for (; i < path.Count; ++i)
        //        {
        //            var pt = path[i];
        //        Utility.debug(" [100, 10 -> 44, 15 ],        idx[" + i + "], x[" + pt.x + "], y[" + pt.y + "]");
        //        }
        //    }

    }

    public int get_monster_desc_count()
    {
        return this.map_conf.map_mon.Count;
    }

    public map_pk_setting_type get_map_pkseting()
    {
        return (map_pk_setting_type)this.map_conf.pk;
    }

    public void release_monster(IBaseUnit mon)
    {

    }

    public void release_map()
    {

    }

    public void fin()
    {
        map_fined = true;
        // release monsters
        foreach (var val in map_mons.Values)
        {
            this.release_monster(val);
        }
        this.release_map();
        //this.temp_sid_ary.clear();
    }

    //debug_map();
    //Utility.trace_info("map [" + mapid +"] with ["+this.map_mons.Count+"] monster created\n");






    //public void init_path(path_conf)
    //{
    //    foreach (path in path_conf)
    //    {
    //        this.paths[path.id] < -path;
    //    }
    //}



    //public void add_map_skills(skills )
    //{
    //    if (!map_skills)
    //    {
    //        this.map_skills = { };
    //    }

    //    foreach (skill in skills)
    //    {
    //        this.map_skills[skill.skid] < -skill;
    //    }
    //}



    //public void _add_triger(trconf, tm_elasped_s= 0)
    //{
    //    if ("timer" in trconf)
    //        {
    //        //Utility.trace_info("map["+this.mapid+"] add timer triger["+trconf.id+"]\n");

    //        var val = { tmleft = trconf.timer[0].tm - tm_elasped_s, cnt = trconf.timer[0].cnt, conf = trconf };
    //        this.tmtriggers[trconf.id] < -val;

    //        if ("showid" in trconf.timer[0])
    //            {
    //            // send add npc msg
    //            this.broadcast_map_rpc(6, { tmtrigs =[{ showid = trconf.timer[0].showid, tm = val.tmleft + sys.time()}]});
    //        }
    //    }
    //        else if ("area" in trconf)
    //        {
    //        //Utility.trace_info("map["+this.mapid+"] add area triger["+trconf.id+"]\n");

    //        this.areatriggers[trconf.id] < -trconf;
    //    }
    //        else if ("km" in  trconf)
    //        {
    //        //Utility.trace_info("map["+this.mapid+"] add km triger["+trconf.id+"]\n");
    //        var tr_km_conf = trconf.km[0];
    //        var totalkmcnt = tr_km_conf.kmcnt;
    //        if ("mulply" in tr_km_conf && tr_km_conf.mulply == 1 )
    //            {
    //            totalkmcnt *= get_plycnt();
    //        }
    //        this.kmtriggers[trconf.id] < - { cnt = tr_km_conf.cnt, kmcnt = 0, tkmcnt = totalkmcnt,conf = trconf};

    //        if ("showid" in trconf.km[0])
    //            {
    //            // send add npc msg
    //            this.broadcast_map_rpc(6, { kmtrigs =[{ showid = tr_km_conf.showid, mid = tr_km_conf.mid, cnt = totalkmcnt, kmcnt = 0}]});
    //        }
    //    }
    //        else if ("useitm" in trconf)
    //        {
    //        //Utility.trace_info("map["+this.mapid+"] add useitm triger["+trconf.id+"]\n");

    //        this.useitmtriggers[trconf.id] < - { cnt = trconf.useitm[0].cnt, conf = trconf};
    //    }
    //        else if ("mis" in trconf)
    //        {
    //        //Utility.trace_info("map["+this.mapid+"] add useitm triger["+trconf.id+"]\n");

    //        this.mistriggers[trconf.id] < - { cnt = trconf.mis[0].cnt, conf = trconf};
    //    }
    //        else if ("other" in trconf)
    //        {
    //        //Utility.trace_info("map["+this.mapid+"] add other triger["+trconf.id+"]\n");

    //        this.othertriggers[trconf.id] < - { cnt = trconf.other[0].cnt, conf = trconf};
    //    }
    //}



    //public void add_npc_to_map(npc)
    //{
    //    if (!(npc.nid in game_data_conf.npcs.npc))
    //    {
    //        // err : no such NPC
    //        //Utility.trace_info("no such npc\n");
    //        return game_err_code.PARAMETER_ERR;
    //    }

    //    var cur_tm_s = sys.time();

    //    var add_npc = [];

    //    var tmed_addnpc = npc;
    //    if (npc.dis_tm > 0)
    //    {
    //        tmed_addnpc = sys.deep_clone(npc);
    //        tmed_addnpc.dis_tm += cur_tm_s;
    //    }

    //    this.add_npcs.push(tmed_addnpc);
    //    add_npc.push(tmed_addnpc);

    //    // broad add npc msg
    //    this.broadcast_map_rpc(6, { npcs = add_npc});

    //    return game_err_code.RES_OK;
    //}

    public void call_world_army_in_map(List<map_mon_conf> world_army_conf)
    {
        if (blvlmap) return;

        //sys.trace( sys.SLT_DETAIL, "call_world_army_in_map mapid=" + world_army_conf.mapid + "\n" );
        //if ("m" in world_army_conf )
        {
            Dictionary<int, List<IBaseUnit>> has_mons = new Dictionary<int, List<IBaseUnit>>();
            foreach (var m in world_army_conf)
            {
                List<IBaseUnit> mons = null;
                if (!has_mons.ContainsKey(m.mid))
                {
                    mons = new List<IBaseUnit>();
                    foreach (var val in this.map_mons.Values)
                    {
                        if (val.get_pack_data().mid == m.mid)
                        {
                            mons.push(val);
                        }
                    }
                    has_mons[m.mid] = mons;
                }
                else
                {
                    mons = has_mons[m.mid];
                }

                IBaseUnit mon = null;
                if (mons.Count > 0)
                {
                    mon = mons.pop();
                    if (mon.get_pack_data().isdie)
                    {
                        mon.respawn(100, true);
                    }
                    continue;
                }

                // 可以创建
                mon = this.create_monster_byconf(m);
                if (mon == null)
                {
                    Utility.trace_err("Err: in map [" + this.mapid + "] callmon mid [" + m.mid + "] create error!\n");
                    return;
                }

                mon.gmap = this;
                IMapUnit mondata = mon.get_pack_data();
                mon.on_pos_change(mondata.x, mondata.y);

                this.map_mons[mondata.iid] = mon;
                this.map_sprites[mondata.iid] = mon;

                if (this.map_mon_bymid.ContainsKey(mondata.mid))

                {
                    this.map_mon_bymid[mondata.mid] = new List<IBaseUnit>() { mon };
                }
                else
                {
                    this.map_mon_bymid[mondata.mid].push(mon);
                }
            }
        }

        //if ("n" in world_army_conf )
        //{
        //    foreach (n in world_army_conf.n)
        //    {
        //        this.add_npc_to_map(n);
        //    }
        //}
    }

    public game_err_code add_monster_to_map(map_mon_conf amconf)
    {
        var m = this.create_monster_byconf(amconf);
        if (m == null)
        {
            //Utility.trace_err("Err: in map [" + mapid +"] add_monster_to_map mid ["+amconf.mid+"] create error!\n");
            return game_err_code.PARAMETER_ERR;
        }

        if (amconf.sideid > 0)
        {
            m.get_pack_data().lvlsideid = amconf.sideid;
        }

        m.gmap = this;
        IMapUnit mondata = m.get_pack_data();
        m.on_pos_change(mondata.x, mondata.y);

        this.map_mons[mondata.iid] = m;
        this.map_sprites[mondata.iid] = m;

        if (!this.map_mon_bymid.ContainsKey(mondata.mid))

        {
            this.map_mon_bymid[mondata.mid] = new List<IBaseUnit>() { m };
        }
        else
        {
            this.map_mon_bymid[mondata.mid].push(m);
        }

        return game_err_code.RES_OK;
    }

    public int call_cost_monster(callmon_conf call_mon_conf, int cost_tp, int max_can_cost)
    {
        var cur_tm_s = Utility.time();
        var total_cost_cnt = 0;
        if (max_can_cost > 0 && call_mon_conf.map_mon.Count > 0)
        {
            List<IBaseUnit> new_added_mons = new List<IBaseUnit>();
            foreach (var m in call_mon_conf.map_mon)
            {
                //if (!(cost_tp in m) ) continue;

                //var call_cost = m[cost_tp];
                //if (call_cost <= 0) continue;

                //if (total_cost_cnt + call_cost > max_can_cost) continue;

                var mon_cnt = 0;
                IBaseUnit mon = null;
                foreach (var val in this.map_mons.Values)
                {
                    IMapUnit mondata = val.get_pack_data();
                    if (mondata.mid == m.mid && mondata.isdie)
                    {
                        mon = val;
                        break;
                    }
                }
                if (mon == null)
                {
                    // 创建
                    //Utility.trace_info("callmon["+m.mid+"], create\n");

                    var bcrt = true;
                    if (this.callmon_added.ContainsKey(m.mid))
                    {
                        if (this.callmon_added[m.mid] >= call_mon_conf.mcnt.cnt)
                        {
                            bcrt = false; // 达到最大数量，不能创建
                        }
                    }

                    if (bcrt)
                    {
                        // 可以创建
                        mon = this.create_monster_byconf(m);
                        if (mon == null)
                        {
                            Utility.trace_err("Err: in map [" + this.mapid + "] callmon mid [" + m.mid + "] create error!\n");
                            return -1;
                        }

                        //total_cost_cnt += call_cost;
                        mon.gmap = this;
                        IMapUnit mondata = mon.get_pack_data();
                        mon.on_pos_change(mondata.x, mondata.y);

                        this.map_mons[mondata.iid] = mon;
                        this.map_sprites[mondata.iid] = mon;

                        if (!this.map_mon_bymid.ContainsKey(mondata.mid))
                        {
                            this.map_mon_bymid[mondata.mid] = new List<IBaseUnit>() { mon };
                        }
                        else
                        {
                            this.map_mon_bymid[mondata.mid].push(mon);
                        }

                        //mon.set_add_conf(m);

                        new_added_mons.push(mon);

                        if (this.callmon_added.ContainsKey(m.mid))
                            this.callmon_added[m.mid]++;
                        else
                            this.callmon_added[m.mid] = 1;
                    }
                }
                else
                {
                    // 死亡则复活
                    if (mon.get_pack_data().isdie)
                    {
                        //Utility.trace_info("callmon["+mon.mondata.mid+"], respawn\n");
                        mon.respawn(100, true);
                        //total_cost_cnt += call_cost;
                    }
                    else
                    {
                        //Utility.trace_info("callmon["+mon.mondata.mid+"], nothing\n");
                    }
                }

                if (total_cost_cnt >= max_can_cost) break;
            }

            if (this.blvlmap && new_added_mons.Count > 0)
            {
                // 副本地图，根据难度调整怪物等级
                worldsvr._on_triger_addmons(new_added_mons);
            }
        }
        return total_cost_cnt;
    }

    public game_err_code kill_monster(IBaseUnit killer, int mid, long respawntm, int cnt)
    {
        var total_cnt = 0;
        foreach (var val in this.map_mons.Values)
        {
            IMapUnit mondata = val.get_pack_data();
            if (mondata.mid != mid)
            {
                continue;
            }

            if (val.isdie())
            {
                continue;
            }

            mondata.respawn_tm = respawntm;
            val.die(killer);

            //var die_msg_rpc = { iid = val.mondata.iid };
            //if (killer)
            //{
            //    die_msg_rpc.frm_iid < -killer.get_pack_data().iid;
            //}
            //// broad cast die msg
            ////sprite.gmap.broadcast_map_rpc(25, die_msg_rpc);
            //val.broad_cast_zone_msg_and_self(25, die_msg_rpc);

            ++total_cnt;
            if (total_cnt >= cnt)
            {
                break;
            }
        }

        return game_err_code.RES_OK;
    }

    public void init_trigger(List<trigger_conf> conf, long tm_elasped_s = 0)
    {
        //Utility.trace_info("map["+this.mapid+"] init triger:\n");
        //sys.dumpobj(conf);



        foreach (var trconf in conf)
        {
            if (trconf.initcr == 1)
            {
                // 初始化时创建             
                this._add_triger(trconf, tm_elasped_s);
            }

            this.trigger_conf[trconf.id] = trconf;

            //Utility.trace_info("map["+this.mapid+"] init triger["+trconf.id+"]\n");
        }
    }



    public void broadcast_map_rpc(cmd_id, data)
    {
        return svr.mul_snd_rpc(this.get_player_sids(), this.get_player_sid_count(), cmd_id, data);
    }
    public void broadcast_map_rpc_except(cmd_id, except, data)
    {
        return svr.mul_snd_rpc_except(this.get_player_sids(), this.get_player_sid_count(), cmd_id, data, except);
    }
    public void get_sprite_hp_info(sid, iid)
    {
        if (iid in this.map_sprites)
        {
            var pinfo = this.map_sprites[iid].get_pack_data();
            ::send_rpc(sid, 53, { res = 1, info = pinfo}); // send hp info msg
        }
        else
        {
            ::send_rpc(sid, 53, { res = -1}); // send hp info msg
        }
    }
    public void add_bzone(id, x, y, w, h)
    {
        this.add_block_zone(id, y, x, y + h, x + w);

        // 通知客户端动态障碍变化
        // send add_npcs msg
        this.broadcast_map_rpc(6, { blockzone = 1, bzadd =[{ id = id, left = x, right = x + w, top = y, bottom = y + h}]});
    }
    public void rmv_bzone(id)
    {
        this.rmv_block_zone(id);

        // 通知客户端动态障碍变化
        // send add_npcs msg
        this.broadcast_map_rpc(6, { blockzone = 2, bzrmv =[{ id = id}]});
    }


    //public void is_player_in_pk_zone(ply)
    //{
    //    if(!map_conf || !("pk_zone" in map_conf))
    //    {
    //        return false;
    //    }

    //    foreach(z in map_conf.pk_zone)
    //    {
    //        if(ply.pinfo.x > z.x && ply.pinfo.x < z.x + z.width && 
    //            ply.pinfo.y > z.y && ply.pinfo.y < z.y + z.height)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}
    //和平区域判断  先用pk_zone表示

    //--------------------------------------------------  掉落  start---------------------------------------------------------
    public void push_dropitm(ply, drop_id, position, kmtp, extra_itms= null, exdrops= null, extra_roll_itms= null, allpick= false) // extra_itms={eqp=[{id=xxx,flvl=xxx}], itm=[{id=xxx,cnt=xxx}]}
    {
        // 产生掉落道具
        var vip_line_rate_mul = 0
                    if (!this.blvlmap && this.worldsvr.is_vip_line)
        {
            var game_conf = get_general_game_conf();
            vip_line_rate_mul = game_conf.vip_line_rate_mul;
        }
        //掉率提升状态
        var buff_rate_mul = _get_bstate_rate_mul(ply);
        var itms = drop_items(drop_id, ply.pinfo.carr, ply.pinfo.kmgldper, vip_line_rate_mul + buff_rate_mul);

        //::g_dump( "push_dropitm, itms:", itms );

        if (exdrops && exdrops.Count > 0)
        {
            // 产生时效性额外掉落
            var cur_local_tm = sys.get_local_time();
            foreach (val in exdrops)
            {
                if (_check_tm(cur_local_tm, val.tmchk) != 0)
                {
                    // 时间未到或已结束
                    continue;
                }

                var exitms = drop_items(val.drop, ply.pinfo.carr, ply.pinfo.kmgldper);
                foreach (itm in exitms.itm)
                {
                    itms.itm.push(itm);
                }
                foreach (eqp in exitms.eqp)
                {
                    itms.eqp.push(eqp);
                }

                itms.gld += exitms.gld;
                itms.bndyb += exitms.bndyb;
            }
        }

        // 产生额外掉落道具
        if (extra_roll_itms != null)
        {
            foreach (eqp in extra_roll_itms.eqp)
            {
                itms.eqp.push(eqp);
            }
            foreach (itm in extra_roll_itms.itm)
            {
                itms.itm.push(itm);
            }
        }

        // 产生额外掉落道具
        if (extra_itms != null)
        {
            foreach (eqp in extra_itms.eqp)
            {
                itms.eqp.push(eqp);
            }
            foreach (itm in extra_itms.itm)
            {
                itms.itm.push(itm);
            }
        }

        // 有一定品质道具
        var tid = team.get_ply_teamid(ply.pinfo.cid);
        if (tid > 0)
        {
            //::Utility.debug( " ===>> team drop! " );
            this.push_team_drop_itm(tid, itms, position, ply, kmtp, allpick);
            return;
        }

        // 检查是否达到可拥有上限
        for (var idx = 0; idx < itms.itm.Count; ++idx)
        {
            var dpeditm = itms.itm[idx];

            var item_conf = get_item_conf(dpeditm.id);
            if (!item_conf)
            {
                itms.itm.remove(idx);
                --idx;
                continue;
            }

            if (!("count" in item_conf.conf) || item_conf.conf.count == 0)
            {
            continue;
        }

        var total_cnt = ply.get_item_total_count(item_conf.conf.id);
        total_cnt += this.get_owner_dpitm_cnt(ply, item_conf.conf.id, false);
        var cnt = item_conf.conf.count - total_cnt;
        if (cnt <= 0)
        {
            // 已达拥有上限
            itms.itm.remove(idx);
            --idx;
            continue;
        }

        if (dpeditm.cnt > cnt)
        {
            dpeditm.cnt = cnt;
        }


        for (var idx = 0; idx < itms.eqp.Count; ++idx)
        {
            var dpeditm = itms.eqp[idx];

            var item_conf = get_item_conf(dpeditm.id);
            if (!item_conf)
            {
                itms.eqp.remove(idx);
                --idx;
                continue;
            }

            if (!("count" in item_conf.conf) || item_conf.conf.count == 0)
            {
            continue;
        }

        var total_cnt = ply.get_item_total_count(item_conf.conf.id);
        total_cnt += this.get_owner_dpitm_cnt(ply, item_conf.conf.id, true);
        var cnt = item_conf.conf.count - total_cnt;
        if (cnt <= 0)
        {
            // 已达拥有上限
            itms.eqp.remove(idx);
            --idx;
            continue;
        }


        //::g_dump( "direct_push_drop_itm, itms:", itms );

        this.direct_push_drop_itm(ply, itms, position, kmtp, null, allpick);
    }

    public void direct_push_drop_itm(ply, itms, position, kmtp, tarnm= null, allpick= false)
    {
        var drop_itm = null;
        if (itms.gld > 0)
        {
            drop_itm = { tp = dpitm_owner_type.DOT_ALL, gold = itms.gld };
            this._direct_push_drop_itm(drop_itm, position);
        }

        if (itms.itm.Count > 0)
        {
            foreach (itm in itms.itm)
            {
                drop_itm = { tp = dpitm_owner_type.DOT_ONE, owner = ply.pinfo.cid, itm = _generate_drop_itm_data(itm) };
                if (allpick)
                {
                    drop_itm.tp = dpitm_owner_type.DOT_ALL;
                }
                this._direct_push_drop_itm(drop_itm, position);

                broad_cast_on_drop(ply.pinfo.cid, ply.pinfo.name, kmtp, itm, tarnm, 0, "itm");

                /*
                var item_conf = get_item_conf(itm.id);           
                if("broadcast" in item_conf.conf && item_conf.conf["broadcast"] == 1)
                {
                    // 广播获得道具消息
                    var msg = {cid=ply.pinfo.cid, name=ply.pinfo.name, tp=bcast_msg_tp.KILL_MON_GAIN_ITM, kmtp=kmtp, itm=itm};
                    if( tarnm )
                    {
                        msg.tarnm <- tarnm;
                    }                  
                    if ( this.blvlmap )
                    {
                        msg.par1 <- this.worldsvr.ltpid;
                    }
                    else
                    {
                        msg.par <- this.mapid;
                    }
                    _broad_cast_sys_msg(msg); // bcast_msg_tp.KILL_MON_GAIN_ITM
                }
                */
            }
        }
        if (itms.eqp.Count > 0)
        {
            foreach (eqp in itms.eqp)
            {
                drop_itm = { tp = dpitm_owner_type.DOT_ONE, owner = ply.pinfo.cid, eqp = _generate_drop_eqp_data(eqp) };
                if (allpick)
                {
                    drop_itm.tp = dpitm_owner_type.DOT_ALL;
                }
                this._direct_push_drop_itm(drop_itm, position);

                //带卓越属性的 装备 才需要广播
                if (drop_itm.eqp.exatt)
                {
                    broad_cast_on_drop(ply.pinfo.cid, ply.pinfo.name, kmtp, eqp, tarnm, 0, "eqp");
                    /*
                    var item_conf = get_equip_conf( eqp.id );
                    if("broadcast" in item_conf && item_conf["broadcast"] == 1)
                    {
                        // 广播获得道具消息
                        var msg = {cid=ply.pinfo.cid, name=ply.pinfo.name, tp=bcast_msg_tp.KILL_MON_GAIN_ITM, kmtp=kmtp, itm=drop_itm.eqp };
                        if(tarnm)
                        {
                            msg.tarnm <- tarnm;
                        }
                        if ( this.blvlmap )
                        {
                            msg.par1 <- this.worldsvr.ltpid;
                        }
                        else
                        {
                            msg.par <- this.mapid;
                        }
                        _broad_cast_sys_msg(msg); // bcast_msg_tp.KILL_MON_GAIN_ITM
                    }
                    */
                }
            }
        }
    }

    // key [itm, eqp]
    public void broad_cast_on_drop(cid, name, kmtp, itm, tarnm, teamid, key )
    {
        var keyfun = {
            itm = {
                conf_fun = get_item_conf,
                form_data = _generate_drop_itm_data,
            },
            eqp = {
                 conf_fun = get_equip_conf,
                 form_data = _generate_drop_eqp_data,
            },
        }




        var funinfo = keyfun[key];

        //::g_dump( " ===>> broad_cast_on_drop itm: ", itm );
        var item_conf = funinfo.conf_fun(itm.id);

        //::g_dump( " ===>> broad_cast_on_drop conf: ", item_conf );


        if (!item_conf)
        {
            //::g_dump( " ===>> 没有物品配置: ", itm );
            return;
        }

        if (key == "itm")
        {
            item_conf = item_conf.conf;
        }

        if (!("broadcast" in item_conf) ||
            item_conf["broadcast"] != 1 )      return;

        // 广播获得道具消息
        var msg = {
            cid = cid,
            //teamid = teamid, 
            name = name,
            tp = bcast_msg_tp.KILL_MON_GAIN_ITM,
            kmtp = kmtp,
            itm = funinfo.form_data( itm ) ,
        };
        if (tarnm)
        {
            msg.tarnm < -tarnm;
        }
        if (this.blvlmap)
        {
            msg.par1 < -this.worldsvr.ltpid;
        }
        else
        {
            msg.par < -this.mapid;
        }
        _broad_cast_sys_msg(msg); // bcast_msg_tp.KILL_MON_GAIN_ITM
    }



    public void _generate_drop_eqp_data(eqp )
    {
        var ret = { conf = eqp, id = eqp.id, exatt = false, add_att = false };
        if (("ex_att_grp" in eqp ) || (("exatt" in eqp) && eqp.exatt != 0) || (("veriex_cnt" in eqp)&& eqp.veriex_cnt > 0) )
        {
            ret.exatt = true;
        }
        if (("add_att_grp" in eqp ) || (("fp" in eqp) && eqp.fp != 0) )
        {
            ret.add_att = true;
        }
        if (("flvl" in eqp) && eqp.flvl != 0 )
        {
            ret.flvl < -eqp.flvl;
        }
        if (("flag" in eqp) && eqp.flag != 0 )
        {
            ret.flag < -eqp.flag;
        }
        if ((("rare_att" in eqp) && eqp.rare_att ) || (("rare_att_grp" in eqp) && eqp.rare_att_grp ) )
        {
            ret.rare_show < -true;
        }
        return ret;
    }

    public void _generate_drop_itm_data(itm )
    {
        var ret = { conf = itm, id = itm.id };
        if (("cnt" in itm) && itm.cnt > 1 )
        {
            ret.cnt < -itm.cnt;
        }
        return ret;
    }


    public void _direct_push_drop_itm(drop_item, position )
    {
        var dpid = map_dpidseed;

        ++map_dpidseed;
        if (map_dpidseed > 0x3ff)//1023
        {
            map_dpidseed = 1;
        }

        // 记录掉落物品
        var cur_tm = sys.time();
        //随机位置
        drop_item.x < -(position.x / game_const.map_grid_pixel);
        drop_item.y < -(position.y / game_const.map_grid_pixel);
        //sys.trace( sys.SLT_DETAIL, "drop_item.x = " + drop_item.x + "drop_item.y = " + drop_item.y + "\n");
        var rand_cnt = 9;
        while (rand_cnt > 0)
        {
            var rand_x = Utility.random(-1, 2);
            var rand_y = Utility.random(-1, 2);
            if (is_grid_walkableEx(drop_item.x + rand_x, drop_item.y + rand_y))
            {
                drop_item.x += rand_x;
                drop_item.y += rand_y;
                break;
            }
            --rand_cnt;
        }

        drop_item.dpid < -dpid;
        drop_item.left_tm < -100;
        if ("gold" in  drop_item )
        {
            drop_item.left_tm < -30;
            drop_item.dis_tm < -cur_tm + 30;//消失时候
        }
        else
        {
            drop_item.left_tm < -90;
            drop_item.dis_tm < -cur_tm + 90;//消失时候
        }
        drop_item.lose_tm < -cur_tm + 30;//失去拥有权时候
                                         //sys.dumpobj( drop_item );        

        this.map_dpitms.push(drop_item);
        if (this.map_dpitms.Count > 0x3ff)
        {
            this.map_dpitms = this.map_dpitms.slice(this.map_dpitms.Count - 0x3ff);
        }

        // broadcast item drop msg
        this.broadcast_map_rpc(76, drop_item);
    }

    public void push_team_drop_itm(teamid, itms, position, ply, kmtp, allpick= false)
    {
        var drop_itm = null;
        if (itms.gld > 0)
        {
            drop_itm = { tp = dpitm_owner_type.DOT_ALL, gold = itms.gld };
            this._direct_push_drop_itm(drop_itm, position);
        }

        //var leader_cid = team.get_leader_cid( teamid ); //广播使用

        if (itms.itm.Count > 0)
        {
            foreach (itm in itms.itm)
            {
                drop_itm = { tp = dpitm_owner_type.DOT_TEAM, owner = teamid, itm = _generate_drop_itm_data(itm) };
                if (allpick)
                {
                    drop_itm.tp = dpitm_owner_type.DOT_ALL;
                }
                this._direct_push_drop_itm(drop_itm, position);
                broad_cast_on_drop(ply.pinfo.cid, ply.pinfo.name, kmtp, itm, null, teamid, "itm");
            }
        }
        if (itms.eqp.Count > 0)
        {
            foreach (eqp in itms.eqp)
            {
                drop_itm = { tp = dpitm_owner_type.DOT_TEAM, owner = teamid, eqp = _generate_drop_eqp_data(eqp) };
                if (allpick)
                {
                    drop_itm.tp = dpitm_owner_type.DOT_ALL;
                }
                this._direct_push_drop_itm(drop_itm, position);
                //带卓越属性的 装备 才需要广播
                if (drop_itm.eqp.exatt)
                {
                    broad_cast_on_drop(ply.pinfo.cid, ply.pinfo.name, kmtp, eqp, null, teamid, "eqp");
                }
            }
        }
    }

    public void get_dpitm(dpitm_id, cur_tm_s )
    {
        var ret = null;
        foreach (idx, drop_item in this.map_dpitms )
        {
            if (drop_item.dpid == dpitm_id)
            {
                if (drop_item.dis_tm <= cur_tm_s)
                {
                    this.map_dpitms.remove(idx);
                }
                else
                {
                    ret = drop_item;
                }
                break;
            }
        }
        return ret;
    }

    //public void get_dpitm_cnt( itm_tp_id, is_eqp )
    //{
    //    var cnt = 0;
    //    foreach( drop_item in this.map_dpitms )
    //    {
    //        if( is_eqp )
    //        {
    //            if( "eqp" in drop_item )
    //            {
    //                if ( drop_item.eqp.id == itm_tp_id )  ++cnt;              
    //            }
    //        }
    //        else
    //        {
    //            if( "itm" in drop_item )
    //            {
    //                if ( drop_item.itm.id == itm_tp_id ) cnt += drop_item.itm.cnt;
    //            }
    //        }
    //    }
    //    //Utility.trace_info("cnt : " + cnt + "\n");
    //    return cnt;
    //}

    public void get_owner_dpitm_cnt(ply, itm_tp_id, is_eqp )
    {
        var cnt = 0;
        var tid = team.get_ply_teamid(ply.pinfo.cid);
        if (is_eqp)
        {
            foreach (drop_item in this.map_dpitms)
            {
                if ("eqp" in drop_item && drop_item.eqp.id == itm_tp_id )
                {
                    switch (drop_item.tp)
                    {
                        case dpitm_owner_type.DOT_ONE:
                            if (drop_item.owner == ply.pinfo.cid)
                            {
                                ++cnt;
                            }
                            break;
                        case dpitm_owner_type.DOT_TEAM:
                            if (tid > 0 && drop_item.owner == tid)
                            {
                                ++cnt;
                            }
                            break;
                    }
                }
            }
        }
        else
        {
            foreach (drop_item in this.map_dpitms)
            {
                if ("itm" in drop_item && drop_item.itm.id == itm_tp_id )
                {
                    switch (drop_item.tp)
                    {
                        case dpitm_owner_type.DOT_ONE:
                            if (drop_item.owner == ply.pinfo.cid)
                            {
                                ++cnt;
                            }
                            break;
                        case dpitm_owner_type.DOT_TEAM:
                            if (tid > 0 && drop_item.owner == tid)
                            {
                                ++cnt;
                            }
                            break;
                    }
                }
            }
        }
        //Utility.trace_info("cnt : " + cnt + "\n");
        return cnt;
    }

    public void rmv_dpitm(dpitm_id )
    {
        foreach (idx, drop_item in this.map_dpitms )
        {
            if (drop_item.dpid == dpitm_id)
            {
                this.map_dpitms.remove(idx);
                break;
            }
        }
    }

    public void _update_dpitm(cur_tm_s)
    {
        var to_del_dpitm = [];
        var i = 0;
        var drop_item = null;
        for (; i < this.map_dpitms.Count; ++i)
        {
            drop_item = this.map_dpitms[i];
            if (drop_item.dis_tm > cur_tm_s)
            {
                break;
            }
        }

        if (i > 0)
        {
            if (i == this.map_dpitms.Count)
            {
                this.map_dpitms = [];
            }
            else
            {
                this.map_dpitms = this.map_dpitms.slice(i);
            }
        }

        //for( i = 0; i < this.map_dpitms.Count; ++i )
        //{
        //    drop_item = this.map_dpitms[i];
        //    if ( drop_item.tp != dpitm_owner_type.DOT_ALL && drop_item.lose_tm < cur_tm_s )
        //    {
        //        drop_item.tp = dpitm_owner_type.DOT_ALL;
        //    }
        //}
    }

    //--------------------------------------------------  掉落  end---------------------------------------------------------

    //public void push_team_drop_itm(itm, position, ply_rolls, kmtp)
    //{
    //    var game_conf = get_general_game_conf();
    //    var cur_tm_s = sys.time();

    //    var drp_id = ++team_drop_itm_seed;
    //    if(team_drop_itm_seed > 0xfffff ) team_drop_itm_seed = 1;

    //    var team_drop = {id=drp_id, itm=itm, pos=position, ply_rolls=sys.deep_clone(ply_rolls), fintm=cur_tm_s+game_conf.team_pick_tm, kmtp=kmtp};

    //    this.team_drop_itm[drp_id] <- team_drop;

    //    return team_drop;
    //}
    //public void get_team_drop_itm(dpid)
    //{
    //    if(!(dpid in this.team_drop_itm))
    //    {
    //        return null;
    //    }

    //    return this.team_drop_itm[dpid];
    //}
    //public void _update_team_dpitm(cur_tm_s)
    //{
    //    var to_del_dpitm = [];
    //    foreach(id, dpitm in this.team_drop_itm)
    //    {
    //        if(cur_tm_s < dpitm.fintm)
    //        {
    //            continue;
    //        }

    //        // 超时了

    //        to_del_dpitm.push(dpitm);
    //    }

    //    foreach(dpitm in to_del_dpitm)
    //    {
    //        delete this.team_drop_itm[dpitm.id];
    //        this._pick_team_dpitm(dpitm);
    //    }
    //}
    //public void _pick_team_dpitm(team_drop)
    //{
    //    var cur_tm_s = sys.time();

    //    var allrolled = true;
    //    var max_roll = 0;
    //    var max_roll_ply = null;
    //    this.temp_sid_ary.reset();

    //    foreach(cid, rollv in team_drop.ply_rolls)
    //    {
    //        var tar_ply = null;
    //        if(this.is_player_in_map_bycid(cid))
    //        {
    //            tar_ply = this.map_players_bycid[cid];
    //            this.temp_sid_ary.push(tar_ply.pinfo.sid);
    //        }
    //        else
    //        {
    //            continue; // 离开地图视为放弃
    //        }

    //        if(rollv > max_roll)
    //        {
    //            max_roll = rollv;
    //            max_roll_ply = tar_ply;
    //        }
    //    }

    //    if(max_roll_ply)
    //    {
    //        // 检查目标道具是否可添加至角色
    //        var add_itm = {itm=[], eqp=[], gld=0, bndyb=0};
    //        if("flvl" in team_drop.itm)
    //        {
    //            add_itm.eqp.push(team_drop.itm);
    //        }
    //        else
    //        {
    //            add_itm.itm.push(team_drop.itm);
    //        }

    //        var res =_check_add_item_to_pl(add_itm, max_roll_ply);

    //        if(res == game_err_code.RES_OK)
    //        {
    //            // 添加道具至角色
    //            var new_add_itm_ary = _add_item_to_pl(add_itm, max_roll_ply);

    //            max_roll_ply.flush_db_data(false, false); // 同步数据至管理器

    //            var item_conf = get_item_conf(team_drop.itm.id);
    //            if("broadcast" in item_conf.conf && item_conf.conf["broadcast"] == 1)
    //            {
    //                // 广播获得道具消息
    //                _broad_cast_sys_msg({cid=max_roll_ply.pinfo.cid, name=max_roll_ply.pinfo.name, tp=1, kmtp=team_drop.kmtp, itm=team_drop.itm}); // bcast_msg_tp.KILL_MON_GAIN_ITM
    //            }

    //            // log itm
    //            if(new_add_itm_ary.Count > 0)
    //            {
    //                _log_itm_log(max_roll_ply, cur_tm_s, itm_act_type.IAT_GET_DP_ITEM, itm_flag_type.IFT_NEW_ADD, 0, new_add_itm_ary, []);
    //            }
    //        }
    //        else
    //        {
    //            // 不能添加至角色，增加掉落道具
    //            max_roll_ply.direct_push_drop_itm(add_itm, team_drop.pos, team_drop.kmtp);

    //            ::send_rpc(max_roll_ply.pinfo.sid, 252, {res=res});
    //        }

    //        // send team_pick_dpitm msg
    //        svr.mul_snd_rpc(this.temp_sid_ary.data(), this.temp_sid_ary.size(), 139, {tpid=team_drop.itm.id, cid=max_roll_ply.pinfo.cid, name=max_roll_ply.pinfo.name, rollv=max_roll});
    //    }
    //}



    public void collect_itm(game_ref, atker, rpc)
    {
        // 中断打坐
        //atker.try_stop_recover();

        var tar_iid = rpc.to_iid;
        if (!(tar_iid in this.map_sprites))
        {
            ::send_rpc(atker.pinfo.sid, 252, { res = game_err_code.COLLECT_TARGET_NOT_EXIST})
            return;
        }

        var tar_spr = this.map_sprites[tar_iid];
        var tar_pl = tar_spr.get_pack_data();

        if (tar_spr.get_sprite_type() != map_sprite_type.MstMonster || tar_spr.collect_tar <= 0)
        {
            ::send_rpc(atker.pinfo.sid, 252, { res = game_err_code.NOT_COLLECT_TARGET})
            return;
        }

        if (tar_spr.isdie())
        {
            ::send_rpc(atker.pinfo.sid, 252, { res = game_err_code.COLLECT_TARGET_DIE})
            return;
        }

        var pl = atker.get_pack_data();

        var dist_x = tar_pl.x - pl.x;
        var dist_y = tar_pl.y - pl.y;
        var game_conf = get_general_game_conf();

        if (dist_x * dist_x + dist_y * dist_y > game_conf.collect_itm_rng * game_conf.collect_itm_rng)
        {
            // 不在范围内
            ::send_rpc(atker.pinfo.sid, 252, { res = game_err_code.COLLECT_TARGET_NOT_NEARBY})
            return;
        }

        tar_pl.hp = 0;

        tar_spr.die(atker);

        // broad cast die msg
        //sprite.gmap.broadcast_map_rpc(25, {iid=pl.iid});
        tar_spr.broad_cast_zone_msg_and_self(25, { iid = tar_pl.iid, frm_iid = pl.iid});

        if ("colitm" in tar_spr.monconf)
        {
            // 采集获得道具
            var ply = atker;
            var item_conf = get_item_conf(tar_spr.monconf.colitm);
            if (item_conf)
            {
                var grid_left = ply.cha_info.item.maxi - (ply.pinfo.items.Count + ply.pinfo.ncitems.Count + ply.pinfo.eqpitems.Count);
                if (grid_left > 0)
                {
                    var ret = _pre_add_item(ply, item_conf, 1, grid_left, true);
                    if (ret.res == game_err_code.RES_OK)
                    {
                        var ret_msg = { };
                        var new_items_ary = _add_item(ply, item_conf, ret);

                        if (new_items_ary.Count > 0)
                        {
                            ret_msg.add < -new_items_ary;
                        }
                        if (ret.mul_item_modcnt_ary.Count > 0)
                        {
                            ret_msg.modcnts < -ret.mul_item_modcnt_ary;
                        }

                        if (ret_msg.Count > 0)
                        {
                            ret_msg.flag < -item_change_msg_flag.ICMF_COL_ITEM; // 采集物品获得
                            ::send_rpc(ply.pinfo.sid, 75, ret_msg);
                        }

                        // log itm
                        var cur_tm_s = sys.time();
                        if (new_items_ary.Count > 0)
                        {
                            _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_COLLECT_ITM, itm_flag_type.IFT_NEW_ADD, tar_pl.mid, new_items_ary, []);
                        }
                        if (ret.mul_item_modcnt_ary.Count > 0)
                        {
                            var mod_cnt_itms = [];
                            foreach (itm_info in ret.mul_item_modcnt_ary)
                            {
                                mod_cnt_itms.push(itm_info.itm);
                            }
                            _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_COLLECT_ITM, itm_flag_type.IFT_MODCNT, tar_pl.mid, mod_cnt_itms, ret.mul_item_modcnt_ary);
                        }
                    }
                }
            }
        }
    }

    public void collect_area_itm(game_ref, ply, rpc)
    {
        // 中断打坐
        //ply.try_stop_recover();

        var cur_tm_s = sys.time();
        if (ply.last_colarea_tm + 1 > cur_tm_s)
        {
            ::send_rpc(ply.pinfo.sid, 252, { res = game_err_code.MAP_COLAREA_CD});
            return;
        }

        if (!(this.mapid in game_data_conf.general.mapex))
        {
            ::send_rpc(ply.pinfo.sid, 252, { res = game_err_code.PARAMETER_ERR});
            return;
        }
        var mapex_conf = game_data_conf.general.mapex[this.mapid];

        if (!(rpc.area_id in mapex_conf.colarea))
        {
            ::send_rpc(ply.pinfo.sid, 252, { res = game_err_code.PARAMETER_ERR});
            return;
        }

        var colarea_conf = mapex_conf.colarea[rpc.area_id];

        if (ply.pinfo.x < (colarea_conf.x - 32) || ply.pinfo.x > (colarea_conf.x + colarea_conf.width + 32) ||
            ply.pinfo.y < (colarea_conf.y - 32) || ply.pinfo.y > (colarea_conf.y + colarea_conf.height + 32))
        {
            // 不在区域中
            ::send_rpc(ply.pinfo.sid, 252, { res = game_err_code.MAP_NOT_IN_COLAREA});
            return;
        }

        if ("itmid" in colarea_conf )
        {
            var item_conf = get_item_conf(colarea_conf.itmid);
            if (!item_conf)
            {
                ::send_rpc(ply.pinfo.sid, 252, { res = game_err_code.CONFIG_ERR});
                return;
            }

            // 判断是否任务采集道具
            if (!("ignoremis" in colarea_conf) || (colarea_conf.ignoremis != 1))
            {
                var mis_itm_need_cnt = 0;
                foreach (mis in ply.pinfo.misacept)
                {
                    var conf = get_mis_conf(mis.misid);

                    var mis_goal = _get_mission_goal(ply.pinfo, conf);
                    if (!mis_goal)
                    {
                        Utility.trace_err("mission [" + mis.misid + "] without goal!\n");
                    }
                    else
                    {
                        if ("colitm" in  mis_goal)
                        {
                            foreach (colitm in mis_goal.colitm)
                            {
                                if (colitm.tpid != colarea_conf.itmid)
                                {
                                    continue;
                                }

                                var own_cnt = ply.get_package_item_cnt(colitm.tpid);
                                //own_cnt += ply.get_dpitm_cnt(colitm.tpid);
                                mis_itm_need_cnt += (colitm.cnt - own_cnt);
                            }
                        }
                        if ("ownitm" in  mis_goal)
                        {
                            foreach (ownitm in mis_goal.ownitm)
                            {
                                if (ownitm.tpid != colarea_conf.itmid)
                                {
                                    continue;
                                }

                                var own_cnt = ply.get_package_item_cnt(ownitm.tpid);
                                //own_cnt += ply.get_dpitm_cnt(ownitm.tpid);
                                mis_itm_need_cnt += (ownitm.cnt - own_cnt);
                            }
                        }
                    }
                }

                if (mis_itm_need_cnt <= 0)
                {
                    // 没有任务需要采集该道具了，采集无法获得道具
                    return;
                }
            }

            var grid_left = ply.cha_info.item.maxi - (ply.pinfo.items.Count + ply.pinfo.ncitems.Count + ply.pinfo.eqpitems.Count);
            if (grid_left <= 0)
            {
                ::send_rpc(ply.pinfo.sid, 252, { res = game_err_code.PACKAGE_SPACE_NOT_ENOUGH});
                return;
            }

            var ret = _pre_add_item(ply, item_conf, 1, grid_left, true);
            if (ret.res != game_err_code.RES_OK)
            {
                ::send_rpc(ply.pinfo.sid, 252, { res = ret.res});
                return;
            }

            ply.last_colarea_tm = cur_tm_s;

            var ret_msg = { };
            var new_items_ary = _add_item(ply, item_conf, ret);

            if (new_items_ary.Count > 0)
            {
                ret_msg.add < -new_items_ary;
            }
            if (ret.mul_item_modcnt_ary.Count > 0)
            {
                ret_msg.modcnts < -ret.mul_item_modcnt_ary;
            }

            if (ret_msg.Count > 0)
            {
                ret_msg.flag < -item_change_msg_flag.ICMF_COL_ITEM; // 采集物品获得
                ::send_rpc(ply.pinfo.sid, 75, ret_msg);
            }

            // log itm
            if (new_items_ary.Count > 0)
            {
                _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_COLAREA_ITM, itm_flag_type.IFT_NEW_ADD, this.mapid, new_items_ary, []);
            }
            if (ret.mul_item_modcnt_ary.Count > 0)
            {
                var mod_cnt_itms = [];
                foreach (itm_info in ret.mul_item_modcnt_ary)
                {
                    mod_cnt_itms.push(itm_info.itm);
                }
                _log_itm_log(ply, cur_tm_s, itm_act_type.IAT_COLAREA_ITM, itm_flag_type.IFT_MODCNT, this.mapid, mod_cnt_itms, ret.mul_item_modcnt_ary);
            }
        }
        else if ("mid" in colarea_conf )
        {
            var send_data = [];
            foreach (mis in ply.pinfo.misacept)
            {
                var conf = get_mis_conf(mis.misid);
                var mis_goal = _get_mission_goal(ply.pinfo, conf);

                // 更新角色任务杀怪数
                if ("colm" in mis )
                {
                    foreach (idx, colm in mis.colm )
                    {
                        if (colm.monid == colarea_conf.mid)
                        {
                            var old_colm_cnt = colm.cnt;
                            ++colm.cnt;
                            if (mis_goal && ("colmon" in mis_goal) && mis_goal.colmon.Count > idx)
                            {
                                if (colm.cnt >= mis_goal.colmon[idx].cnt)
                                {
                                    colm.cnt = mis_goal.colmon[idx].cnt;
                                }
                            }

                            if (old_colm_cnt != colm.cnt)
                            {
                                send_data.push( { misid = mis.misid, colmid = colm.monid, cnt = colm.cnt} );
                            }
                            break;
                        }
                    }
                }
            }

            if (send_data.Count > 0)
            {
                ply.last_colarea_tm = cur_tm_s;
                foreach (data in send_data)
                {
                    // send mis_data_modify msg
                    ::send_rpc(ply.pinfo.sid, 113, data);
                }
            }
        }
    }
    public void teststep(pl, step )
    {
        if (!("cid" in pl ) ) return;
        if (!isdebug(pl)) return;
        Utility.debug("XXXXXXXXXXXXX[" + step + "]");
    }
    public void do_attack(game_ref, atker, rpc)
    {
        var tar_iid = rpc.to_iid;
        if (tar_iid != atker.get_iid() && tar_iid in this.map_sprites && atker.get_iid() in this.map_sprites)
        {
            var tar_spr = this.map_sprites[tar_iid];

            var pl = atker.get_pack_data();
            var tar_pl = tar_spr.get_pack_data();

            if (atker.get_sprite_type() == map_sprite_type.MstPlayer)
            {

                teststep(pl, 11);

                if (atker.isghost())
                {
                    // 灵魂状态，不能攻击
                    return;
                }

                // 中断打坐
                //atker.try_stop_recover();

                if (tar_spr.get_sprite_type() == map_sprite_type.MstPlayer)
                {
                    // 玩家打玩家，pk
                    if (this.pk_seting == map_pk_setting_type.MPST_PEACE)
                    {
                        // send err_msg msg
                        ::send_rpc(pl.sid, 252, { res = game_err_code.CANT_PK_IN_PEACE_MAP});
                        return;
                    }

                    if (pl.in_pczone || tar_pl.in_pczone)
                    {
                        // send err_msg msg
                        ::send_rpc(pl.sid, 252, { res = game_err_code.CANT_PK_IN_PEACE_ZONE});
                        return;
                    }

                    if (this.pk_seting == map_pk_setting_type.MPST_NORMAL)
                    {
                        if (!tar_spr.can_atk_direct())
                        {//不可攻击的玩家   
                         //未开pk状态  不可攻击 此类玩家
                         //if( pl.pk_state == pk_state_type.PKST_PEACE )
                         //{
                         //    ::send_rpc( pl.sid, 252, {res=game_err_code.CANT_ATK_PL_IN_PEACE});
                         //    return;
                         //}
                         //开pk状态 只能攻击指定的 此类玩家
                         //if( atker.get_pk_tar_iid() != tar_pl.iid ) return;   
                            if (!atker.can_atk(tar_spr))
                            {
                                teststep(pl, 12);
                                return;
                            }

                        }
                    }
                    else
                    {//pk地图
                        if (!atker.can_atk(tar_spr))
                        {
                            teststep(pl, 13);
                            return;
                        }
                        //{//盟友  
                        //    //未开pk状态  不可攻击盟友  
                        //    if( pl.pk_state == pk_state_type.PKST_PEACE )
                        //    {
                        //        ::send_rpc( pl.sid, 252, {res=game_err_code.CANT_ATK_PL_IN_PEACE});
                        //        return;
                        //    }

                        //    //开pk状态 只能攻击指定的盟友
                        //    if( atker.get_pk_tar_iid() != tar_pl.iid ) return;
                        //}       
                    }
                }
                else if (tar_spr.get_sprite_type() == map_sprite_type.MstMonster)
                {
                    if (tar_spr.owner_ply != null)
                    {//战宠     
                        var owner_sprite = tar_spr.owner_ply;
                        var ower_pl = owner_sprite.get_pack_data();
                        if (ower_pl.iid == pl.iid)
                        {
                            return { res = game_err_code.CANT_ATTACK_SELF_PET};
                        }
                        // 玩家打战宠，pk
                        if (this.pk_seting == map_pk_setting_type.MPST_PEACE)
                        {
                            // send err_msg msg
                            ::send_rpc(pl.sid, 252, { res = game_err_code.CANT_PK_IN_PEACE_MAP});
                            return;
                        }

                        if (pl.in_pczone || ower_pl.in_pczone)
                        {
                            // send err_msg msg
                            ::send_rpc(atker.pinfo.sid, 252, { res = game_err_code.CANT_PK_IN_PEACE_ZONE});
                            return;
                        }

                        if (this.pk_seting == map_pk_setting_type.MPST_NORMAL)
                        {
                            if (!owner_sprite.can_atk_direct())
                            {//不可攻击的玩家   
                             //未开pk状态  不可攻击 此类玩家
                             //if( pl.pk_state == pk_state_type.PKST_PEACE )
                             //{
                             //    ::send_rpc( pl.sid, 252, {res=game_err_code.CANT_ATK_PL_IN_PEACE});
                             //    return;
                             //}     

                                if (!atker.can_atk(owner_sprite))
                                {
                                    teststep(pl, 15);
                                    return;
                                }
                            }
                        }
                        else
                        {//pk地图
                            if (!atker.can_atk(owner_sprite))
                            {
                                teststep(pl, 16);
                                return;
                            }
                            //{//盟友  
                            //    //未开pk状态  不可攻击盟友  
                            //    if( pl.pk_state == pk_state_type.PKST_PEACE )
                            //    {
                            //        ::send_rpc( pl.sid, 252, {res=game_err_code.CANT_ATK_PL_IN_PEACE});
                            //        return;
                            //    }
                            //}       
                        }
                    }
                }
            }
            else
            {
                if (!atker.can_atk(tar_spr))
                {
                    teststep(pl, 17);

                    return;
                }
            }

            teststep(pl, 2);

            if (!tar_spr.isdie() && !tar_spr.isghost())
            {

                teststep(pl, 3);

                if (!("atking" in pl) || pl.atking.tar_iid != tar_iid)
                {

                    teststep(pl, 4);

                    var cur_clock_tm = sys.clock_time();

                    var cd_tm = atker.get_atk_cd();
                    var start_tm = cur_clock_tm - cd_tm;
                    if (start_tm < pl.last_atk_tm)
                    {
                        start_tm = pl.last_atk_tm;
                    }

                    if (rpc.start_tm - cur_clock_tm > 0)
                    {
                        rpc.start_tm = cur_clock_tm;
                    }

                    var data = { frm_iid = atker.get_iid(), to_iid = tar_iid, start_tm = rpc.start_tm };

                    pl.atking < - { start_tm = start_tm, tar_iid = tar_iid, trace_tm_left = cur_clock_tm - rpc.start_tm};

                    // broadcast attack msg
                    //this.broadcast_map_rpc(12, data);
                    atker.broad_cast_zone_msg_and_self(12, data);

                    // update attacker immediately
                    atker.update(game_ref, cur_clock_tm, 0);
                }

            }
        }
        else
        {
            // error, attacker or target not exist
            Utility.trace_err("persist_game attacker [" + atker.get_iid() + "] or target[" + tar_iid + "] not exist\n");
        }
    }

    public bool is_grid_walkableEx(int gx, int gy)
    {
        //return true;
        return this.is_grid_walkable(gx, gy);

    }
    //获取 射线上最远可行点




    public void _add_map_eff(caster, target_pos, mapstat_conf, per, cur_tm_s, cur_clock_tm, skid)
    {
        if (!(mapstat_conf.id in game_data_conf.skils.map_state))
        {
            return;
        }

        var conf = game_data_conf.skils.map_state[mapstat_conf.id];

        var pl = caster.get_pack_data();
        if (!target_pos)
        {
            target_pos = pl;
        }
        ++mapstatid_seed;

        var new_map_stat = {id=mapstatid_seed, tpid=mapstat_conf.id, x=target_pos.x, y=target_pos.y, tm=cur_tm_s+mapstat_conf.tm,
            crtiid=pl.iid, nexttm=0, trgcnt=0, per=per+mapstat_conf.per, conf=conf, skid=skid};

        this.mapstats.push(new_map_stat);

        // 通知客户端创建新的地图技能状态对象
        var add_npc_msg = { mapstat = 1, mapstatadd =[{ id = new_map_stat.id, tpid = new_map_stat.tpid, x = new_map_stat.x, y = new_map_stat.y, tm = new_map_stat.tm }]};
        this.broadcast_map_rpc(6, add_npc_msg);

        // 计算一次地图对象效果
        if (this._update_mapstat(new_map_stat, cur_tm_s, cur_clock_tm))
        {
            // 需要移除
            this.mapstats.pop();

            // bcast add_npcs msg
            this.broadcast_map_rpc(6, { mapstat = 2, mapstatrmv =[{ id = new_map_stat.id}]});
        }
    }

    // 更新飞行道具
    public void update_flys(cur_clock_tm)
    {
        for (var idx = 0; idx < this.map_flys.Count; ++idx)
        {
            var fly = this.map_flys[idx];
            //Utility.trace_info("update_flys\n");

            if (!(fly.frm_iid in this.map_sprites))
            {
            this.map_flys.remove(idx);
            --idx;
            continue;
        }

        if (fly.end_tm > cur_clock_tm)
        {
            continue;
        }

        var caster = this.map_sprites[fly.frm_iid];
        var target = null;
        if (fly.tar_iid in this.map_sprites)
            {
            target = this.map_sprites[fly.tar_iid];
        }

        this._post_cast_skill(cur_clock_tm, caster, fly.rpc, fly.cast_skill_res, target, fly.target_pos, false);
        this.map_flys.remove(idx);
        --idx;
    }

    public void _trigger_attchk(ply, conf)
    {
        if ("attchk" in conf)
        {
            foreach (attchk in conf.attchk)
            {
                if (ply.check_att(attchk) != game_err_code.RES_OK)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void on_accept_mis(ply, mis_conf )
    {
        _on_mis_trigger(ply, mis_conf.id, "accept");
    }

    public void on_comit_mis(ply, mis_conf )
    {
        _on_mis_trigger(ply, mis_conf.id, "comit");
    }

    public void _on_mis_trigger(IBaseUnit ply, misid, state )
    {
        if (this.mistriggers.Count <= 0)
        {
            return;
        }

        // 更新触发器       
        var to_rmv_tmtrgids = [];
        var to_rmv_areatrgids = [];
        var to_rmv_kmtrgids = [];
        var to_rmv_uitmtrgids = [];
        var to_rmv_mistrgids = [];
        var to_add_trgconf = [];
        var to_rmv_othertrgids = [];

        foreach (trid, trg in this.mistriggers)
        {
            var mis_trg_conf = trg.conf.mis[0];

            if (mis_trg_conf.misid != misid || mis_trg_conf.state != state)
            {
                continue;
            }

            if (!_trigger_attchk(ply, trg.conf))
            {
                continue;
            }

            // 触发
            --trg.cnt;

            if (trg.cnt <= 0)
            {
                to_rmv_mistrgids.push(trid);
            }

            this._trig_res(trg.conf, ply, to_add_trgconf, to_rmv_tmtrgids, to_rmv_areatrgids, to_rmv_kmtrgids, to_rmv_uitmtrgids, to_rmv_mistrgids, to_rmv_othertrgids);
        }

        foreach (trid in to_rmv_tmtrgids)
        {
            if (trid in this.tmtriggers) delete this.tmtriggers[trid];
        }
        foreach (trid in to_rmv_areatrgids)
        {
            if (trid in this.areatriggers) delete this.areatriggers[trid];
        }
        foreach (trid in to_rmv_kmtrgids)
        {
            if (trid in this.kmtriggers) delete this.kmtriggers[trid];
        }
        foreach (trid in to_rmv_uitmtrgids)
        {
            if (trid in this.useitmtriggers) delete this.useitmtriggers[trid];
        }
        foreach (trid in to_rmv_mistrgids)
        {
            if (trid in this.mistriggers) delete this.mistriggers[trid];
        }
        foreach (trid in to_rmv_othertrgids)
        {
            if (trid in this.othertriggers) delete this.othertriggers[trid];
        }
        foreach (trid in to_add_trgconf)
        {
            this._add_triger(this.trigger_conf[trid]);
        }
    }

    public void on_use_itm(IBaseUnit ply, itm_conf)
    {
        // 角色使用道具
        //Utility.trace_info("map["+this.mapid+"] on_use_itm ["+itm_conf.id+"]\n");

        if (this.useitmtriggers.Count <= 0)
        {
            return;
        }

        var clan_triged = false;

        // 更新触发器

        var to_rmv_tmtrgids = [];
        var to_rmv_areatrgids = [];
        var to_rmv_kmtrgids = [];
        var to_rmv_uitmtrgids = [];
        var to_rmv_mistrgids = [];
        var to_add_trgconf = [];
        var to_rmv_othertrgids = [];

        foreach (trid, trg in this.useitmtriggers)
        {
            var uitm_trg_conf = trg.conf.useitm[0];

            if (uitm_trg_conf.tpid != itm_conf.id)
            {
                continue;
            }

            if (uitm_trg_conf.sideid != 0 && uitm_trg_conf.sideid != ply.pinfo.lvlsideid)
            {
                continue;
            }

            //Utility.trace_info("map["+this.mapid+"] on_use_itm trid ["+trid+"]\n");

            if (ply.pinfo.x > uitm_trg_conf.x && ply.pinfo.y > uitm_trg_conf.y &&
                ply.pinfo.x < uitm_trg_conf.x + uitm_trg_conf.width && ply.pinfo.y < uitm_trg_conf.y + uitm_trg_conf.height)
            {

                if (!_trigger_attchk(ply, trg.conf))
                {
                    continue;
                }

                // 触发
                --trg.cnt;

                if (trg.cnt <= 0)
                {
                    to_rmv_uitmtrgids.push(trid);

                    if ("clantrig" in uitm_trg_conf)
                    {
                        clan_triged = true;
                    }
                }

                this._trig_res(trg.conf, ply, to_add_trgconf, to_rmv_tmtrgids, to_rmv_areatrgids, to_rmv_kmtrgids, to_rmv_uitmtrgids, to_rmv_mistrgids, to_rmv_othertrgids);
            }
        }

        foreach (trid in to_rmv_tmtrgids)
        {
            if (trid in this.tmtriggers) delete this.tmtriggers[trid];
        }
        foreach (trid in to_rmv_areatrgids)
        {
            if (trid in this.areatriggers) delete this.areatriggers[trid];
        }
        foreach (trid in to_rmv_kmtrgids)
        {
            if (trid in this.kmtriggers) delete this.kmtriggers[trid];
        }
        foreach (trid in to_rmv_uitmtrgids)
        {
            if (trid in this.useitmtriggers) delete this.useitmtriggers[trid];
        }
        foreach (trid in to_rmv_mistrgids)
        {
            if (trid in this.mistriggers) delete this.mistriggers[trid];
        }
        foreach (trid in to_rmv_othertrgids)
        {
            if (trid in this.othertriggers) delete this.othertriggers[trid];
        }
        foreach (trid in to_add_trgconf)
        {
            this._add_triger(this.trigger_conf[trid]);
        }

        if (clan_triged)
        {
            // 帮派触发的使用道具触发器（召唤帮派boss），需要记录触发时间

            if ("clanid" in ply.pinfo)
            {

                // 更新已方帮派数据
                var clan_mgr = global_data_mgrs.clan;
                clan_mgr.lock_data(ply.pinfo.clanid, server_const.GLOBAL_DATA_LOCK_TM); // 锁定数据对象，注意，所有跳出函数代码分支必须要有unlock

                var self_clan_db_data = _get_clan_data(ply.pinfo.clanid);
                if (self_clan_db_data.Count > 0)
                {
                    var self_clan_info = self_clan_db_data[0].get_data();

                    self_clan_info.data.uitrig[itm_conf.id] < -sys.time(); // 记录当前时间

                    self_clan_db_data[0].mod_data(self_clan_info);
                    self_clan_db_data[0].db_update();
                }

                clan_mgr.unlock_data(ply.pinfo.clanid); // 解锁
            }

        }
    }


    public void trig_other_triger(Variant trig_spr, int trigid)
    {
        var to_rmv_tmtrgids = [];
        var to_rmv_areatrgids = [];
        var to_rmv_kmtrgids = [];
        var to_rmv_uitmtrgids = [];
        var to_rmv_mistrgids =[];
        var to_add_trgconf = [];
        var to_rmv_othertrgids = [];

        //Utility.trace_info("trig_other_triger ["+trigid+"]\n");
        //sys.dumpobj(this.othertriggers);

        foreach (trid, trg in this.othertriggers)
        {
            // 触发
            if (trid != trigid)
            {
                continue;
            }

            if (trig_spr && trig_spr.get_sprite_type() == map_sprite_type.MstPlayer)
            {
                if (!_trigger_attchk(trig_spr, trg.conf))
                {
                    continue;
                }
            }

            --trg.cnt;

            if (trg.cnt <= 0)
            {
                to_rmv_othertrgids.push(trid);
            }

            this._trig_res(trg.conf, trig_spr, to_add_trgconf, to_rmv_tmtrgids, to_rmv_areatrgids, to_rmv_kmtrgids, to_rmv_uitmtrgids, to_rmv_mistrgids, to_rmv_othertrgids);
        }

        foreach (trid in to_rmv_tmtrgids)
        {
            if (trid in this.tmtriggers) delete this.tmtriggers[trid];
        }
        foreach (trid in to_rmv_areatrgids)
        {
            if (trid in this.areatriggers) delete this.areatriggers[trid];
        }
        foreach (trid in to_rmv_kmtrgids)
        {
            if (trid in this.kmtriggers) delete this.kmtriggers[trid];
        }
        foreach (trid in to_rmv_uitmtrgids)
        {
            if (trid in this.useitmtriggers) delete this.useitmtriggers[trid];
        }
        foreach (trid in to_rmv_mistrgids)
        {
            if (trid in this.mistriggers) delete this.mistriggers[trid];
        }
        foreach (trid in to_rmv_othertrgids)
        {
            if (trid in this.othertriggers) delete this.othertriggers[trid];
        }
        foreach (trid in to_add_trgconf)
        {
            this._add_triger(this.trigger_conf[trid]);
        }
    }

    public void _update_mapstat(mapstat, cur_tm_s, cur_clock_tm)
    {
        if (mapstat.nexttm > cur_clock_tm)
        {
            return false;
        }

        if (!(mapstat.crtiid in this.map_sprites))
        {
            return true;
        }

        mapstat.nexttm = cur_clock_tm + mapstat.conf.tick;

        var caster = this.map_sprites[mapstat.crtiid];

        var rang2 = mapstat.conf.rang * mapstat.conf.rang;

        var affed_spr = 0;
        foreach (spr in this.map_sprites)
        {
            var pl = spr.get_pack_data();

            var x_dist = mapstat.x - pl.x;
            var y_dist = mapstat.y - pl.y;

            if (x_dist * x_dist + y_dist * y_dist > rang2)
            {
                continue;
            }

            var triged = false;
            foreach (tres in mapstat.conf.tres)
            {
                if (apply_skill_eff_to(cur_clock_tm, caster, spr, tres, tres.aff, mapstat.skid, mapstat.per))
                {
                    triged = true;
                }
            }

            if (!triged)
            {
                continue;
            }

            ++affed_spr;
            ++mapstat.trgcnt;

            if (mapstat.conf.trgcnt > 0 && mapstat.trgcnt >= mapstat.conf.trgcnt)
            {
                // 触发次数完了，需要删除
                return true;
            }

            if (affed_spr >= mapstat.conf.maxi)
            {
                break;
            }
        }

        if (mapstat.tm <= cur_tm_s)
        {
            return true;
        }

        return false;
    }




}
}