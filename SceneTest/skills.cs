using SceneTestLib.Confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
    public class Skill
    {

        public static void _remark_pl_state(IBaseUnit sprite, IMapUnit pl, bool recalc_att = true)
        {
            if (pl.states == null)
                return;

            if (pl.states.state_par.Count <= 0)
            {
                // 重新计算角色属性
                if (recalc_att)
                {
                    sprite.re_calc_cha_data();
                }

                pl.states = null;
                return;
            }

            pl.states.state = 0;
            pl.states.mov_spd_mul = 1.0;
            pl.states.atk_spd_mul = 1.0;
            pl.states.mp2hp = 0;
            pl.states.dmglnkper = 0;

            //var invisible = 0;

            foreach (var val in pl.states.state_par)
            {
                var i = 0;
                var mp2hp = 0;
                //        if (("attadj" in val) && ("mp2hp" in val.attadj) )
                //{
                //            mp2hp = val.attadj.mp2hp;
                //        }
                //for (; i < val.desc.s_states.len(); ++i)
                //{
                //    pl.states.state = (pl.states.state | val.desc.s_states[i].tp);

                //    //if((val.desc.s_states[i].tp&pl_state_type.PST_MOVE_SPEED_MUL)==pl_state_type.PST_MOVE_SPEED_MUL)
                //    //{
                //    //    pl.states.mov_spd_mul *= val.desc.s_states[i].par.tofloat() * val.par / 100000.0;
                //    //}

                //    //if((val.desc.s_states[i].tp&pl_state_type.PST_ATK_SPEED_MUL)==pl_state_type.PST_ATK_SPEED_MUL)
                //    //{
                //    //    pl.states.atk_spd_mul *= val.desc.s_states[i].par.tofloat() * val.par / 100000.0;
                //    //}

                //    if ((val.desc.s_states[i].tp & pl_state_type.PST_MP2HP) == pl_state_type.PST_MP2HP)
                //    {
                //        if (mp2hp > 0)
                //        {
                //            pl.states.mp2hp += mp2hp;
                //        }
                //        else
                //        {
                //            pl.states.mp2hp += (val.desc.s_states[i].par * val.par / 1000);
                //        }
                //    }
                //    if ((val.desc.s_states[i].tp & pl_state_type.PST_DMGLNK_M) == pl_state_type.PST_DMGLNK_M)
                //    {
                //        pl.states.dmglnkper += (val.desc.s_states[i].par * val.par / 1000);
                //    }

                //    // TO DO : speed add att
                //}

                //if("att" in val.desc)
                //{
                //    if("invisible" in val.desc.att)
                //    {
                //        // 隐身状态
                //        if(val.desc.att.invisible > invisible)
                //        {
                //            invisible = val.desc.att.invisible;
                //        }
                //    }

                //    if(!recalc_att)
                //    {
                //        foreach(key,att in val.desc.att)
                //        {
                //            if(key!="invisible") 
                //            {
                //                // 有需计算属性
                //                recalc_att = true;
                //                break;
                //            }
                //        }
                //    }
                //}
            }

            //if(recalc_att)
            //{
            // 有att调整字段，需要重新计算角色属性
            sprite.re_calc_cha_data();
            //}

            //sprite.visible_change(invisible);
        }

        public static void rmv_state_from_pl_by_att(string att_key, IBaseUnit sprite, IMapUnit pl)
        {
            if (pl.states == null)
                return;

            var removed_ids = new List<int>();
            for (var idx = 0; idx < pl.states.state_par.Count; ++idx)
            {
                var val = pl.states.state_par[idx];
                //if (val.desc.att != null && val.desc.att.ContainsKey(att_key))
                //{
                //    pl.states.state_par.RemoveAt(idx);
                //    --idx;
                //    removed_ids.push(val.desc.id);
                //}
            }

            if (removed_ids.Count > 0)
            {
                _remark_pl_state(sprite, pl);

                // broadcast rmv_state msg
                //sprite.broad_cast_zone_msg_and_self(31, {iid=pl.iid, ids=removed_ids    });
            }
        }

        //public static bool apply_skill_eff_to(long cur_clock_tm, IBaseUnit from, IBaseUnit target, dmg dmg, int aff, int skill_id, int percentage)
        //{
        //    tres sk_res = new tres();
        //    sk_res.rate = 100;

        //    return apply_skill_eff_to(cur_clock_tm, from, target, sk_res, aff, skill_id, percentage);

        //}

        public static bool apply_skill_eff_to(long cur_clock_tm, IBaseUnit from, IBaseUnit target, tres_conf sk_res, int aff, int skill_id, int percentage)
        {

            var dflag = true;
            if (target.isdie() || target.isghost())
            {
                // TO DO : 考虑有些技能能作用于尸体
                if (dflag) Utility.debug(" !apply_skill_eff_to  false 1");
                return false;
            }

            if (target.ignore_dmg())
            {
                if (dflag) Utility.debug(" !apply_skill_eff_to  false 2");
                return false;
            }

            //if ( "rate" in sk_res )
            if (sk_res.rate > 0)
            {
                if (Utility.random(0, 100) > sk_res.rate)
                {
                    if (dflag) Utility.debug(" !apply_skill_eff_to  false 3");
                    return false;
                }
            }

            var to_pl = target.get_pack_data();
            var frm_pl = from.get_pack_data();

            //var rpc_data = new Variant();
            //rpc_data["hited"] = 3;
            //rpc_data["sid"] = skill_id;
            //rpc_data["frm_iid"] = frm_pl.iid;
            //rpc_data["to_iid"] = to_pl.iid;

            if (target.has_state(pl_state_type.PST_SKILL_AVOID))
            {
                // 目标技能免疫
                // send single_skill_res msg to clients
                //rpc_data["hited"] = 0;//免疫
                //target.broad_cast_zone_msg_and_self(22, rpc_data);
                if (dflag) Utility.debug(" !apply_skill_eff_to  false 4");
                return false;
            }

            //if(from.get_sprite_type() == map_sprite_type.MST_MONSTER && target.get_sprite_type() == map_sprite_type.MST_MONSTER)
            //{
            //    return false; // 一家人不打一家人
            //}

            // TO DO : 根据aff判断敌人、盟友关系从而确定是否受此状态影响

            game_err_code check_res = Utility.check_target_type(from, target, aff, target.gmap.pk_seting);
            if (check_res != game_err_code.RES_OK)
                return false;

            if (from.get_sprite_type() == map_sprite_type.MstPlayer)
            {
                if (target.can_atk(from))
                {
                    target.atk_by(from);
                }
            }

            //if(sk_res.hp_dmg != 0)
            //{
            //    rpc_data.hp_dmg <- {};
            //    rpc_data.hp_dmg.hp_dmg <- sk_res.hp_dmg;

            //    to_pl.hp -= sk_res.hp_dmg;

            //    if(to_pl.hp > to_pl.max_hp)
            //    {
            //        to_pl.hp = to_pl.max_hp;
            //    }
            //}
            //if(sk_res.mp_dmg != 0)
            //{
            //    rpc_data.mp_dmg <- {};
            //    rpc_data.mp_dmg.mp_dmg <- sk_res.mp_dmg;

            //    to_pl.mp -= sk_res.mp_dmg;

            //    if(to_pl.mp > to_pl.max_mp)
            //    {
            //        to_pl.mp = to_pl.max_mp;
            //    }
            //}

            Utility.debug("apply_skill_eff_to try apply_dmg_on_pl!");

            //no_dmg  不计算伤害
            if (sk_res.no_dmg == 0)
            {
                var ret = grid_map.apply_dmg_on_pl(target, from, sk_res.convert2damage(), cur_clock_tm, percentage, true);
                Utility.debug("!apply_skill_eff_to apply_dmg_on_pl:" + ret);
                if (ret["hited"]._int32 != 3)
                {
                    // 未命中
                    // send single_skill_res msg to clients
                    //rpc_data["hited"] = ret["hited"];
                    //target.broad_cast_zone_msg_and_self(22, rpc_data);
                    if (dflag) Utility.debug(" !apply_skill_eff_to  false 20");
                    return false;
                }
            }


            //sys.trace(sys.SLT_DETAIL, "sk_res.rmv_stat ["+sk_res.rmv_stat+"]\n");

            if (sk_res.rmv_stat > 0)
            {
                int rmv_stat = sk_res.rmv_stat;

                if (to_pl.states != null)
                {
                    //sys.dumpobj(to_pl.states);
                    // 移除所有状态
                    rmv_stat_from_pl_by_gb(rmv_stat, target, to_pl);
                }
            }

            if (sk_res.rmv_1stat > 0)
            {
                if (to_pl.states != null)

                {
                    // 移除一个状态
                    rmv_1stat_from_pl_by_gb(sk_res.rmv_1stat, target, to_pl);
                }
            }

            if (sk_res.tar_state > 0)
            {
                int tar_state = sk_res.tar_state;
                var add_stat = true;
                if (sk_res.stat_rat > 0)
                {
                    // 有几率触发技能效果
                    var judg = Utility.random(0, 100);
                    if (judg > sk_res.stat_rat)
                    {
                        //sys.trace(sys.SLT_DETAIL, "stat_rat["+sk_res.stat_rat+"] < judg["+judg+"]\n");
                        if (dflag) Utility.debug(" !apply_skill_eff_to  false 21");
                        add_stat = false;
                    }
                }

                if (add_stat)
                {
                    // add state eff to character
                    var state_obj = add_state_to_pl(cur_clock_tm, target, sk_res, from, percentage);
                    if (state_obj != null)
                    {
                        // rpc_data.states < -state_obj;
                        //rpc_data.states <- {};
                        //rpc_data.states.id <- state_obj.id;
                        //rpc_data.states.par <- state_obj.par;
                        //rpc_data.states.start_tm <-state_obj.start_tm;
                        //rpc_data.states.end_tm <- state_obj.end_tm;
                    }
                }
            }

            if (sk_res.cd_red != null)
            {
                // 减少cd
                //var skcds = [];

                foreach (var cd_red in sk_res.cd_red)
                {
                    int sktp = cd_red.sktp;
                    int cd_reduce = (cd_red.red_tm * percentage / 10);
                    if (sktp == 0)
                    {


                        foreach (int sk_key in to_pl.skill_cd.Keys)
                        {
                            //cd -= (sk_res.cd_red.red_tm * 100 * per / 1000);
                            to_pl.skill_cd[sk_key] -= cd_reduce;

                            //skcds.push({ sktp = sktp, cdtm = cd - cur_clock_tm});
                        }
                    }
                    else if (to_pl.skill_cd.ContainsKey(sktp))
                    {
                        to_pl.skill_cd[sktp] -= cd_reduce;

                        //skcds.push({ sktp = cd_red.sktp, cdtm = to_pl.skill_cd[cd_red.sktp] - cur_clock_tm});
                    }
                }

                //        if (skcds.len() > 0)
                //        {
                //// send self_attchange msg
                //::send_rpc(to_pl.sid, 32, { skcds = skcds});
                //        }
            }

            if (!to_pl.cfmv)
            {
                if (sk_res.fmv != null)
                {
                    var judge = Utility.random(0, 100);
                    int fmv_rate = sk_res.fmv.rate;
                    int fmv_dir = sk_res.fmv.dir;
                    int fmv_rang = sk_res.fmv.rang;
                    if (judge < fmv_rate)
                    {
                        var dir = fmv_dir;
                        if (dir == 2)
                        {//随机前后
                            dir = Utility.random(0, 2);
                        }
                        // 迫使目标移动
                        if (dir == 0)
                        {
                            // 远离自己的方向
                            var dist_x = to_pl.x - frm_pl.x;
                            var dist_y = to_pl.y - frm_pl.y;

                            if ((dist_x != 0) || (dist_y != 0))
                            {
                                var vec = Utility.normalize_vec2(dist_x, dist_y);

                                var from_x = to_pl.x;
                                var from_y = to_pl.y;
                                to_pl.x += (int)(vec.x * fmv_rang);
                                to_pl.y += (int)(vec.y * fmv_rang);

                                var dest_pos = target.gmap.valpoint_on_vector(to_pl.x, to_pl.y, from_x, from_y, vec);

                                var to_grid_pos = target.gmap.get_grid_by_pt(dest_pos.x, dest_pos.y);
                                //if(!to_grid_pos || (dest_pos.x < 0) || (dest_pos.y < 0))
                                //{
                                //    // 目标坐标非法，出错
                                //    Utility.trace_err("fmv frm_pl.x["+frm_pl.x+"] frm_pl.y["+frm_pl.y+"] from_x["+from_x+"] y["+from_y+"] to_pl.x["+to_pl.x+"] to_pl.y["+to_pl.y+"] dest_pos.x["+dest_pos.x+"] dest_pos.y["+dest_pos.y+"] vec:\n");
                                //    sys.dumpobj(vec);
                                //}

                                to_pl.x = (int)dest_pos.x;
                                to_pl.y = (int)dest_pos.y;

                                //sys.trace(sys.SLT_DETAIL, "fmv to_pl.x["+to_pl.x+"] ["+to_pl.y+"]\n");

                                if (target.get_sprite_type() == map_sprite_type.MstPlayer && !target.is_in_lvl)
                                {
                                    to_pl.lx = to_pl.x;
                                    to_pl.ly = to_pl.y;
                                }
                            }
                        }
                        else if (dir == 1)
                        {
                            // 靠近自己的方向
                            var dist_x = frm_pl.x - to_pl.x;
                            var dist_y = frm_pl.y - to_pl.y;

                            if ((dist_x != 0) || (dist_y != 0))
                            {
                                var dist2 = dist_x * dist_x + dist_y * dist_y;
                                var vec = Utility.normalize_vec2(dist_x, dist_y);

                                if (fmv_rang == 0 || fmv_rang * fmv_rang >= dist2)
                                {
                                    // 拉近到自己身边一格
                                    var from_x = to_pl.x;
                                    var from_y = to_pl.y;

                                    to_pl.x = (int)(frm_pl.x - vec.x * game_const.map_grid_pixel);
                                    to_pl.y = (int)(frm_pl.y - vec.y * game_const.map_grid_pixel);

                                    var dest_pos = target.gmap.valpoint_on_vector(to_pl.x, to_pl.y, from_x, from_y, vec);
                                    to_pl.x = (int)dest_pos.x;
                                    to_pl.y = (int)dest_pos.y;

                                    if (target.get_sprite_type() == map_sprite_type.MstPlayer && !target.is_in_lvl)
                                    {
                                        to_pl.lx = to_pl.x;
                                        to_pl.ly = to_pl.y;
                                    }
                                }
                                else
                                {
                                    var from_x = to_pl.x;
                                    var from_y = to_pl.y;
                                    to_pl.x += (int)(vec.x * fmv_rang);
                                    to_pl.y += (int)(vec.y * fmv_rang);

                                    var dest_pos = target.gmap.valpoint_on_vector(to_pl.x, to_pl.y, from_x, from_y, vec);
                                    to_pl.x = (int)dest_pos.x;
                                    to_pl.y = (int)dest_pos.y;

                                    if (target.get_sprite_type() == map_sprite_type.MstPlayer && !target.is_in_lvl)
                                    {
                                        to_pl.lx = to_pl.x;
                                        to_pl.ly = to_pl.y;
                                    }
                                }
                            }
                        }
                    }

                    if (to_pl.moving != null)
                    {
                        // 停止移动
                        //var data = { tm = cur_clock_tm, iid = to_pl.iid, x = to_pl.x, y = to_pl.y, face = to_pl.face };
                        // send stop move msg to clients
                        //target.gmap.broadcast_map_rpc(10, data);
                        //target.broad_cast_zone_msg_and_self(10, data);

                        //delete to_pl.moving;
                        to_pl.moving = null;
                    }

                    to_pl.last_mvpts = null;

                    //rpc_data.fmv < - { to_x = to_pl.x, to_y = to_pl.y, dir = sk_res.fmv.dir};

                    if (to_pl.casting != null)
                    {
                        // 停止施法
                        //var data = { iid = to_pl.iid };
                        // send cancel_casting_res to clients
                        //target.broad_cast_zone_msg_and_self(29, data);

                        //delete to_pl.casting;
                        to_pl.casting = null;
                    }
                }
            }

            //var gmap = target.gmap;

            //if(to_pl.hp <= 0)
            //{
            //    // die!
            //    to_pl.hp = 0;
            //    target.die(from);

            //    rpc_data.isdie = true;
            //}

            // send single_skill_res msg to clients
            //gmap.broadcast_map_rpc(22, rpc_data);
            //target.broad_cast_zone_msg_and_self(22, rpc_data);

            return true;
        }



        public static SimpleState add_state_to_pl(long cur_clock_tm, IBaseUnit target, tres_conf state, IBaseUnit frm_sprite, int per, bool bremark = true)
        {
            if (target.isdie() || target.isghost())
            {
                // TO DO : 考虑有些技能能作用于尸体
                return null;
            }

            var pl = target.get_pack_data();

            var state_desc = Utility.get_skil_state_desc(state.tar_state);
            if (state_desc == null)
            {
                // Err: state not exist;
                Utility.trace_err("persist_game add state[" + state.tar_state + "] to[" + pl.iid + "], state not exist!\n");
                return null;
            }

            var state_tm = state.state_tm;
            //sys.trace(sys.SLT_DETAIL, "state_tm before["+state_tm+"]\n");

            var mul = 1000;
            var add = 0;
            //        if (0 in pl.stat_red)
            //{
            //            mul += pl.stat_red[0].mul;
            //            add += pl.stat_red[0].add;
            //        }
            //        if (state.tar_state in pl.stat_red)
            //{
            //            mul += pl.stat_red[state.tar_state].mul;
            //            add += pl.stat_red[state.tar_state].add;
            //        }

            //sys.trace(sys.SLT_DETAIL, "mul["+mul+"] add["+add+"]\n");

            state_tm = (state_tm * 1000 / mul - add);
            if (state_tm < 1) state_tm = 1;
            //sys.trace(sys.SLT_DETAIL, "state_tm after["+state_tm+"]\n");

            var new_end_tm = cur_clock_tm + state_tm * 100;
            //sys.trace(sys.SLT_DETAIL, "new_end_tm["+new_end_tm+"]\n");

            var remove_stateids = new List<int>();

            if (pl.states != null)
            {
                if (state_desc.max_cnt > 0)
                {
                    // 判断状态数量
                    var own_cnt = 0;
                    foreach (var val in pl.states.state_par)
                    {
                        if (val.id == state.tar_state)
                        {
                            ++own_cnt;

                            if (own_cnt >= state_desc.max_cnt)
                            {
                                // 超过最大数量限制，不在增加状态
                                return null;
                            }
                        }
                    }
                    if (own_cnt == 0)
                    {
                        //替换掉低级的
                        if (state_desc.unique != null)
                        {
                            for (var idx = 0; idx < pl.states.state_par.Count; ++idx)
                            {
                                var val = pl.states.state_par[idx];

                                if (val.desc.unique == null)
                                    continue;

                                if (state_desc.unique.uni_tp != val.desc.unique.uni_tp)
                                {
                                    continue;
                                }

                                if (state_desc.unique.uni_prior > val.desc.unique.uni_prior)
                                {
                                    pl.states.state_par.RemoveAt(idx);
                                    --idx;
                                    remove_stateids.push(val.desc.id);
                                }
                            }
                        }
                    }
                }
                else if (state_desc.unique != null)
                {
                    // 判断独有状态，根据独有类型实施替换

                    for (var idx = 0; idx < pl.states.state_par.Count; ++idx)
                    {
                        var val = pl.states.state_par[idx];

                        if (val.desc.unique == null)
                            continue;

                        if (state_desc.unique.uni_tp != val.desc.unique.uni_tp)
                        {
                            continue;
                        }

                        if (state_desc.unique.uni_prior > val.desc.unique.uni_prior)
                        {
                            if (new_end_tm > val.end_tm)
                            {
                                pl.states.state_par.RemoveAt(idx);
                                --idx;
                                remove_stateids.push(val.desc.id);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else if (state_desc.unique.uni_prior > val.desc.unique.uni_prior)
                        {
                            pl.states.state_par.RemoveAt(idx);
                            --idx;
                            remove_stateids.push(val.desc.id);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            var i = 0;
            for (; i < state_desc.s_states.Count; ++i)
            {
                int state_tp = state_desc.s_states[i].tp;
                if ((state_tp & (int)pl_state_type.PST_CANT_MOVE) == (int)pl_state_type.PST_CANT_MOVE)
                {
                    // stop sprite & broadcast msg
                    grid_map.update_pl_move(target, cur_clock_tm);

                    if (pl.moving != null)
                    {
                        // 停止移动
                        //var data = { tm = cur_clock_tm, iid = pl.iid, x = pl.x, y = pl.y, face = pl.face };
                        // send stop move msg to clients
                        //target.gmap.broadcast_map_rpc(10, data);
                        //target.broad_cast_zone_msg_and_self(10, data);

                        //delete pl.moving;
                        pl.moving = null;
                    }
                }
                if ((state_tp & (int)pl_state_type.PST_CANT_CAST_SKILL) == (int)pl_state_type.PST_CANT_CAST_SKILL)
                {
                    if (pl.holding != null)
                    {//先 停止聚气
                        if (target.gmap != null)
                        {
                            target.gmap.update_skill_holding(target, pl, 0);
                        }
                    }

                    if (pl.casting != null)
                    {
                        // 停止施法
                        //var data = { iid = pl.iid };
                        // send cancel_casting_res to clients
                        //target.broad_cast_zone_msg_and_self(29, data);

                        //delete pl.casting;
                        pl.casting = null;
                    }
                }
            }

            if (pl.states == null)
                pl.states = new UnitState();


            IMapUnit frm_pl = null;
            int frm_iid = 0;

            if (frm_sprite != null)
            {
                frm_pl = frm_sprite.get_pack_data();
                frm_iid = frm_pl.iid;
            }

            SimpleState obj = new SimpleState()
            {
                id = state.tar_state,
                par = (int)(state.state_par * per / 1000.0),
                start_tm = cur_clock_tm,
                end_tm = new_end_tm,
                desc = state_desc,
                frm_iid = frm_iid,
                per = per,
                attadj = new Dictionary<string, int>(),
            };

            pl.states.state_par.push(obj);

            // 首次添加定时状态
            if (state_desc.timer != null)
            {
                obj.tm_elapsed = 0;

                var timer = state_desc.timer;
                if (timer.add_stat != null)
                {
                    int tar_state = timer.add_stat.tar_state;
                    if (tar_state != state_desc.id)
                    {
                        var state_obj = add_state_to_pl(cur_clock_tm, target, timer.add_stat, frm_sprite, per, false);
                        if (state_obj != null)
                        {
                            // broadcast add state msg;
                            //target.broad_cast_zone_msg_and_self(24, { iid = pl.iid, states =[state_obj]});
                        }
                    }

                    if (timer.dmg != null)
                    {
                        if (timer.trang != null)
                        {

                            apply_rang_eff(cur_clock_tm, target, new Point2D(pl.x, pl.y), new List<tres_conf>() { timer.dmg.convert2tres() }, timer.trang, 0, obj.par);
                        }
                        else
                        {
                            grid_map.apply_dmg_on_pl(target, frm_sprite, timer.dmg.convert2damage(), cur_clock_tm, obj.par);
                        }

                    }

                    // 增加直接回蓝、回血功能
                    //            if ("dd" in state_desc.timer)
                    //{
                    //                apply_direct_dmg_on_pl(target, frm_sprite, state_desc.timer.dd[0], cur_clock_tm, obj.par);
                    //            }

                    if (target.isdie() || target.isghost())
                    {
                        // TO DO : 考虑有些技能能作用于尸体
                        return null;
                    }
                }

                if (state_desc.absorbdmg != null)
                {
                    obj.absorbed = 0;
                    obj.maxdmg = (state_desc.absorbdmg.maxdmg * obj.par / 1000);
                }

                if (state_desc.atkcnt != null)
                {
                    obj.atkedcnt = 0;
                }

                //if(frm_sprite && "hate" in state_desc && frm_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
                if (frm_sprite != null && state_desc.hate > 0)
                {
                    target.onhate(frm_sprite, state_desc.hate);
                }

                if (remove_stateids.Count > 0)
                {
                    // broadcast rmv_state msg
                    //target.broad_cast_zone_msg_and_self(31, { iid = pl.iid, ids = remove_stateids});
                }
            }

            if (bremark)
            {
                _remark_pl_state(target, pl);
            }

            return obj;
        }


        public static void apply_rang_eff(long cur_clock_tm, IBaseUnit from, IPoint2D center, List<tres_conf> sk_res, rang_conf trang, int sid, int per)
        {
            var gmap = from.gmap;
            var maxi = trang.maxi;
            if (maxi == 0)
                maxi = 0x7fffffff;
            int aff_count = 0;
            Utility.debug(" ================= apply_rang_eff ================= ");
            //var test_cost_tm = sys.clock_time();
            int cirang = trang.cirang;
            if (cirang > 0)
            {
                // 以center中心、cirang为半径的圆形区域
                var pow_range = cirang * cirang;
                //var inzoneplys = from.get_inz_plys(); 
                foreach (var val in gmap.map_players.Values)
                {
                    if (val.isdie() || val.isghost())
                    {
                        continue;
                    }

                    var pl_data = val.get_pack_data();

                    var dist_x = Math.Abs(pl_data.x - center.x);
                    if (dist_x > cirang)
                    {
                        Utility.debug("pow_range[" + pow_range + "] <= dist_x[" + dist_x + "]     center.x[" + center.x + "] , center.y[" + center.y + "] pl_data.x[" + pl_data.x + "] , pl_data.y[" + pl_data.y + "]");
                        continue; // 不在范围内
                    }

                    var dist_y = Math.Abs(pl_data.y - center.y);
                    if (dist_y > cirang)
                    {
                        Utility.debug("pow_range[" + pow_range + "] <= dist_y[" + dist_y + "]     center.x[" + center.x + "] , center.y[" + center.y + "] pl_data.x[" + pl_data.x + "] , pl_data.y[" + pl_data.y + "]");
                        continue; // 不在范围内
                    }
                    var rang1 = dist_x * dist_x + dist_y * dist_y;
                    if (rang1 > pow_range)
                    {
                        Utility.debug("pow_range[" + pow_range + "] <= rang1[" + rang1 + "]     center.x[" + center.x + "] , center.y[" + center.y + "] pl_data.x[" + pl_data.x + "] , pl_data.y[" + pl_data.y + "]");
                        continue;
                    }

                    var affed = false;
                    foreach (var tres in sk_res)
                    {
                        var ret = apply_skill_eff_to(cur_clock_tm, from, val, tres, tres.aff, sid, per);
                        if (ret)
                        {
                            affed = true;
                        }
                    }

                    if (affed)
                    {
                        ++aff_count;
                        if (aff_count >= maxi)
                        {
                            break;
                        }
                    }
                }
                if (aff_count < maxi)
                {
                    //var inzonemons = from.get_inz_mons(); 
                    foreach (IBaseUnit val in gmap.map_mons.Values)
                    {
                        if (val.isdie() || val.isghost())
                        {
                            continue;
                        }

                        var pl_data = val.get_pack_data();

                        var dist_x = Math.Abs(pl_data.x - center.x);
                        if (dist_x - pl_data.size > cirang) continue; // 不在范围内

                        var dist_y = Math.Abs(pl_data.y - center.y);
                        if (dist_y - pl_data.size > cirang) continue; // 不在范围内

                        var rang1 = Math.Sqrt(dist_x * dist_x + dist_y * dist_y);
                        if (rang1 - pl_data.size > cirang)
                        {
                            continue; // 不在范围内
                        }

                        var affed = false;
                        foreach (var tres in sk_res)
                        {
                            var ret = apply_skill_eff_to(cur_clock_tm, from, val, tres, tres.aff, sid, per);
                            if (ret)
                            {
                                affed = true;
                            }
                        }

                        if (affed)
                        {
                            ++aff_count;
                            if (aff_count >= maxi)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else if (trang.fan != null)
            {
                int fan_angle = trang.fan.angle;
                int fan_rang = trang.fan.rang;
                // 以from为中心，center为目标，fan.rang为半径，fan.angle为角度的扇形区域
                var frm_pl = from.get_pack_data();
                var x1 = center.x - frm_pl.x;
                var y1 = center.y - frm_pl.y;
                var rang1 = x1 * x1 + y1 * y1;
                if (rang1 != 0)
                {
                    rang1 = Math.Sqrt(rang1);
                    var h_angle = Math.Cos(fan_angle / 360.0 * Math.PI);
                    var maxrange = fan_rang;
                    var pow_maxrange = maxrange * maxrange;
                    //var inzoneplys = from.get_inz_plys();  
                    foreach (var val in gmap.map_players.Values)
                    {
                        if (val.isdie() || val.isghost())
                        {
                            continue;
                        }

                        var pl_data = val.get_pack_data();
                        var x2 = pl_data.x - frm_pl.x;
                        if (Math.Abs(x2) >= maxrange) continue; // 不在范围内

                        var y2 = pl_data.y - frm_pl.y;
                        if (Math.Abs(y2) >= maxrange) continue; // 不在范围内

                        double rang2 = x2 * x2 + y2 * y2;
                        //sys.trace( sys.SLT_DETAIL, "x2 = " + x2 + "   y2 = " + y2 + "\n"  );
                        if (rang2 >= pow_maxrange)
                        {
                            continue; // 不在范围内
                        }

                        rang2 = Math.Sqrt(rang2);
                        var angleVal = (rang2 != 0) ? (x1 * x2 + y1 * y2) / (rang2 * rang1) : 1;
                        //sys.trace( sys.SLT_DETAIL, "apply_rang_eff rang2 = " + rang2 + "rang1 = " + rang1 + "\n"  );
                        //sys.trace( sys.SLT_DETAIL, "apply_rang_eff angleVal = " + angleVal + "h_angle = " + h_angle + "\n"  );
                        if (angleVal < h_angle)
                        {
                            continue;// 不在范围内
                        }

                        var new_per = per;
                        if (trang.fan.decay != null && trang.fan.decay.Count > 0)
                        {//衰减
                            var dist = rang2;
                            foreach (var decay in trang.fan.decay)
                            {
                                if (decay.min <= dist && decay.max > dist)
                                {
                                    new_per *= (int)(decay.per / 100.0);
                                    break;
                                }
                            }
                        }

                        var affed = false;
                        foreach (var tres in sk_res)
                        {
                            var ret = apply_skill_eff_to(cur_clock_tm, from, val, tres, tres.aff, sid, new_per);
                            if (ret)
                            {
                                affed = true;
                            }
                        }

                        if (affed)
                        {
                            ++aff_count;
                            if (aff_count >= maxi) break;
                        }
                    }
                    if (aff_count < maxi)
                    {
                        //var inzonemons = from.get_inz_mons(); 
                        foreach (IBaseUnit val in gmap.map_mons.Values)
                        {
                            if (val.isdie() || val.isghost())
                            {
                                continue;
                            }

                            var pl_data = val.get_pack_data();
                            var x2 = pl_data.x - frm_pl.x;
                            if (Math.Abs(x2) - pl_data.size >= maxrange) continue; // 不在范围内

                            var y2 = pl_data.y - frm_pl.y;
                            if (Math.Abs(y2) - pl_data.size >= maxrange) continue; // 不在范围内

                            var rang2 = Math.Sqrt(x2 * x2 + y2 * y2);
                            //sys.trace( sys.SLT_DETAIL, "x2 = " + x2 + "   y2 = " + y2 + "\n"  );
                            if (rang2 - pl_data.size >= maxrange)
                            {
                                continue; // 不在范围内
                            }

                            var angleVal = (rang2 != 0) ? (x1 * x2 + y1 * y2) / (rang2 * rang1) : 1;
                            //sys.trace( sys.SLT_DETAIL, "apply_rang_eff rang2 = " + rang2 + "rang1 = " + rang1 + "\n"  );
                            //sys.trace( sys.SLT_DETAIL, "apply_rang_eff angleVal = " + angleVal + "h_angle = " + h_angle + "\n"  );
                            if (angleVal < h_angle)
                            {
                                continue;// 不在范围内
                            }

                            var new_per = per;
                            if (trang.fan.decay != null && trang.fan.decay.Count > 0)
                            {//衰减
                                var dist = rang2;
                                foreach (var decay in trang.fan.decay)
                                {
                                    if (decay.min <= dist && decay.max > dist)
                                    {
                                        new_per *= (int)(decay.per / 100.0);
                                        break;
                                    }
                                }
                            }

                            var affed = false;
                            foreach (var tres in sk_res)
                            {
                                var ret = apply_skill_eff_to(cur_clock_tm, from, val, tres, tres.aff, sid, new_per);
                                if (ret)
                                {
                                    affed = true;
                                }
                            }

                            if (affed)
                            {
                                ++aff_count;
                                if (aff_count >= maxi) break;
                            }
                        }
                    }
                }
            }
            else if (trang.ray != null)
            {
                // 以from为中心，center为目标，fan.dist为距离，fan.width为宽度的矩形区域
                var ray = trang.ray;
                var frm_pl = from.get_pack_data();
                var x1 = center.x - frm_pl.x;
                var y1 = center.y - frm_pl.y;
                var rang1 = x1 * x1 + y1 * y1;
                if (rang1 != 0)
                {
                    rang1 = Math.Sqrt(rang1);
                    var hwidth = ray.width / 2.0;
                    //var inzoneplys = from.get_inz_plys();       
                    foreach (var val in gmap.map_players.Values)
                    {
                        if (val.isdie() || val.isghost())
                        {
                            continue;
                        }

                        var pl_data = val.get_pack_data();

                        var x2 = pl_data.x - frm_pl.x;
                        var y2 = pl_data.y - frm_pl.y;

                        var dotVal = x1 * x2 + y1 * y2;
                        //sys.trace( sys.SLT_DETAIL, " iid = " + pl_data.iid + "      dotVal = " + dotVal + "\n"  );
                        if (dotVal < 0)
                        {
                            continue;// 不在范围内
                        }

                        var projVal = dotVal / rang1;
                        //sys.trace( sys.SLT_DETAIL, " dotVal = " + dotVal + "     projVal = " + projVal + "\n"  );
                        if (projVal - pl_data.size > ray.dist)
                        {
                            continue;// 不在范围内
                        }

                        var vertVal = x2 * x2 + y2 * y2 - projVal * projVal;//小数会产生误差  出现负数的情况
                                                                            //sys.trace( sys.SLT_DETAIL, "apply_rang_eff vertVal = " + vertVal + "  pl_data.size =" + pl_data.size + "\n"  );
                        if (vertVal > 0 && Math.Sqrt(vertVal) > hwidth)
                        {
                            continue;// 不在范围内
                        }

                        var affed = false;
                        foreach (var tres in sk_res)
                        {
                            var ret = apply_skill_eff_to(cur_clock_tm, from, val, tres, tres.aff, sid, per);
                            if (ret)
                            {
                                affed = true;
                            }
                        }

                        if (affed)
                        {
                            ++aff_count;
                            if (aff_count >= maxi) break;
                        }
                    }
                    if (aff_count < maxi)
                    {
                        //var inzonemons = from.get_inz_mons(); 
                        foreach (var val in gmap.map_mons.Values)
                        {
                            if (val.isdie() || val.isghost())
                            {
                                continue;
                            }

                            var pl_data = val.get_pack_data();

                            var x2 = pl_data.x - frm_pl.x;
                            var y2 = pl_data.y - frm_pl.y;

                            var dotVal = x1 * x2 + y1 * y2;
                            //sys.trace( sys.SLT_DETAIL, " iid = " + pl_data.iid + "      dotVal = " + dotVal + "\n"  );
                            if (dotVal < 0)
                            {
                                continue;// 不在范围内
                            }

                            var projVal = dotVal / rang1;
                            //sys.trace( sys.SLT_DETAIL, " dotVal = " + dotVal + "     projVal = " + projVal + "\n"  );
                            if (projVal - pl_data.size > ray.dist)
                            {
                                continue;// 不在范围内
                            }

                            var vertVal = x2 * x2 + y2 * y2 - projVal * projVal;//小数会产生误差  出现负数的情况
                                                                                //sys.trace( sys.SLT_DETAIL, "apply_rang_eff vertVal = " + vertVal + "  pl_data.size =" + pl_data.size + "\n"  );
                            if (vertVal > 0 && Math.Sqrt(vertVal) > hwidth)
                            {
                                continue;// 不在范围内
                            }

                            var affed = false;
                            foreach (var tres in sk_res)
                            {
                                var ret = apply_skill_eff_to(cur_clock_tm, from, val, tres, tres.aff, sid, per);
                                if (ret)
                                {
                                    affed = true;
                                }
                            }

                            if (affed)
                            {
                                ++aff_count;
                                if (aff_count >= maxi) break;
                            }
                        }
                    }
                }
            }

            //sys.trace( sys.SLT_DETAIL, " apply_rang_eff  tm= " + (sys.clock_time()-test_cost_tm) + " \n" );
        }




        public static void rmv_state_from_pl_by_id(int state_id, IBaseUnit sprite, IMapUnit pl)
        {
            if (pl.states == null || pl.states.state_par.Count <= 0)
                return;

            bool removed = false;
            for (var idx = 0; idx < pl.states.state_par.Count; ++idx)
            {
                var val = pl.states.state_par[idx];

                if (val.desc.id == state_id)
                {
                    pl.states.state_par.RemoveAt(idx);
                    --idx;
                    //removed_ids.push(val.desc.id);
                    removed = true;
                }
            }

            //if (removed_ids.len() > 0)
            if (removed)
            {
                _remark_pl_state(sprite, pl);

                // broadcast rmv_state msg
                //sprite.broad_cast_zone_msg_and_self(31, { iid = pl.iid, ids = removed_ids});
            }

        }



        public static void rmv_stat_from_pl_by_gb(int rmv_state_gb, IBaseUnit sprite, IMapUnit pl)
        {
            if (pl.states == null || pl.states.state_par.Count <= 0)
                return;

            bool removed = false;
            for (var idx = 0; idx < pl.states.state_par.Count; ++idx)
            {
                var val = pl.states.state_par[idx];
                if (val.desc.gb == rmv_state_gb)
                {
                    pl.states.state_par.RemoveAt(idx);
                    --idx;
                    //removed_ids.push(val.desc.id);
                    removed = true;
                }
            }

            //sys.dumpobj(removed_ids);

            //if (removed_ids.len() > 0)
            if (removed)
            {
                _remark_pl_state(sprite, pl);

                // broadcast rmv_state msg
                //sprite.broad_cast_zone_msg_and_self(31, { iid = pl.iid, ids = removed_ids});
            }
        }

        public static void rmv_1stat_from_pl_by_gb(int rmv_state_gb, IBaseUnit sprite, IMapUnit pl)
        {
            if (pl.states == null || pl.states.state_par.Count <= 0)
                return;

            bool removed = false;
            for (var idx = 0; idx < pl.states.state_par.Count; ++idx)
            {
                var val = pl.states.state_par[idx];
                if (val.desc.gb == rmv_state_gb)
                {
                    pl.states.state_par.RemoveAt(idx);
                    --idx;
                    //removed_ids.push(val.desc.id);
                    removed = true;
                    break;
                }
            }

            //if (removed_ids.len() > 0)
            if (removed)
            {
                _remark_pl_state(sprite, pl);

                // broadcast rmv_state msg
                //sprite.broad_cast_zone_msg_and_self(31, { iid = pl.iid, ids = removed_ids});
            }
        }

        public static void update_pl_state(long cur_clock_tm, IBaseUnit sprite)
        {
            var pl = sprite.get_pack_data();

            if (pl.states != null)
                return;

            var changed = false;
            //var add_state_rpc = { states =[] };
            //var rmv_state_rpc = { iid = pl.iid, ids =[] }

            var i = 0;
            for (; i < pl.states.state_par.Count; ++i)
            {
                var tm_elapsed = cur_clock_tm - pl.states.state_par[i].start_tm;
                var val = pl.states.state_par[i];
                var rmved = false;

                IBaseUnit frm_sprite = sprite.gmap.getSprite(val.frm_iid);


                if (val.end_tm <= cur_clock_tm)
                {
                    tm_elapsed = val.end_tm - val.start_tm;

                    // time reach, remove
                    pl.states.state_par.RemoveAt(i);
                    --i;
                    changed = true;
                    rmved = true;

                    //rmv_state_rpc.ids.push(val.desc.id);
                }

                // update time change states
                if (tm_elapsed > 500 || rmved)
                {
                    if (!rmved && tm_elapsed > 500)
                    {
                        tm_elapsed = 500;
                    }

                    var j = 0;
                    var desc_s_states = val.desc.s_states;
                    for (; j < desc_s_states.Count; ++j)
                    {
                        var s_state = desc_s_states[j];
                        // update hp change
                        if ((s_state.tp & (int)pl_state_type.PST_HP_TIME_ADD) == (int)pl_state_type.PST_HP_TIME_ADD)
                        {
                            int hp_add = (int)((s_state.par * val.par / 1000.0) * tm_elapsed / 1000.0);
                            //Utility.trace_err("pl.iid [" + pl.iid +"] hp_add ["+hp_add+"] \n");
                            pl.hp += hp_add;
                            if (pl.hp < 0)
                            {
                                pl.hp = 0;

                                // TO DO : die!
                                sprite.die(frm_sprite);

                                //var die_msg_rpc = { iid = pl.iid };
                                //if (frm_sprite)
                                //{
                                //    die_msg_rpc.frm_iid < -val.frm_iid;
                                //}
                                //// broad cast die msg
                                ////sprite.gmap.broadcast_map_rpc(25, die_msg_rpc);
                                //sprite.broad_cast_zone_msg_and_self(25, die_msg_rpc);

                                // player die, no more update
                                return;
                            }
                            else if (pl.hp > pl.max_hp)
                            {
                                pl.hp = pl.max_hp;
                            }
                        }

                        // update mp change
                        if ((s_state.tp & (int)pl_state_type.PST_MP_TIME_ADD) == (int)pl_state_type.PST_MP_TIME_ADD)
                        {
                            int mp_add = (int)((s_state.par * val.par / 1000.0) * tm_elapsed / 1000.0);
                            //Utility.trace_err("pl.iid [" + pl.iid +"] mp_add ["+mp_add+"] \n");
                            pl.mp += mp_add;
                            if (pl.mp < 0)
                            {
                                pl.mp = 0;
                            }
                            else if (pl.mp > pl.max_mp)
                            {
                                pl.mp = pl.max_mp;
                            }
                        }
                    }

                    // 处理定时获得指定状态的状态
                    if (val.desc.timer != null)
                    {
                        var timer = val.desc.timer;
                        val.tm_elapsed += tm_elapsed;

                        int interval = timer.interval;
                        if (val.tm_elapsed >= interval * 100)
                        {
                            val.tm_elapsed -= interval * 100;

                            if (timer.add_stat != null)
                            {
                                var add_stat = timer.add_stat;
                                if (add_stat.tar_state != val.desc.id)
                                {
                                    var state_obj = add_state_to_pl(cur_clock_tm, sprite, add_stat, frm_sprite, val.per, false);
                                    if (state_obj != null)
                                    {
                                        changed = true;
                                        //add_state_rpc.states.push(state_obj);
                                    }

                                }

                                if (timer.dmg != null)
                                {
                                    if (timer.rang != null)
                                    {
                                        apply_rang_eff(cur_clock_tm, sprite, new Point2D(pl.x, pl.y), new List<tres_conf>() { timer.dmg.convert2tres() }, timer.rang, 0, val.par);
                                    }
                                    else
                                    {
                                        grid_map.apply_dmg_on_pl(sprite, frm_sprite, timer.dmg.convert2damage(), cur_clock_tm, val.par);
                                    }

                                }

                                // 增加直接回蓝、回血功能
                                //                    if ("dd" in val.desc.timer)
                                //        {
                                //    apply_direct_dmg_on_pl(sprite, frm_sprite, val.desc.timer.dd[0], cur_clock_tm, val.par);
                                //}
                            }
                        }
                    }

                    val.start_tm += tm_elapsed;
                }

                if (pl.states == null)
                    break;
            }

            if (changed)
            {
                //sys.trace(sys.SLT_DETAIL, "pl.iid [" + pl.iid +"] state changed \n");
                _remark_pl_state(sprite, pl);
            }

            //if (add_state_rpc.states.len() > 0)
            //{
            //    // broadcast add state msg;
            //    add_state_rpc.iid < -pl.iid;
            //    sprite.broad_cast_zone_msg_and_self(24, add_state_rpc);
            //}

            //if (rmv_state_rpc.ids.len() > 0)
            //{
            //    // broadcast rmv_state msg
            //    sprite.broad_cast_zone_msg_and_self(31, rmv_state_rpc);
            //}
        }

        public static bool has_pl_state(int state_val, IMapUnit pl)
        {
            if (pl.states == null)
                return false;

            if ((pl.states.state & state_val) != state_val)
            {
                return false;
            }

            return true;
        }

        public static bool has_pl_state_bystid(int stid, IMapUnit pl)
        {
            if (pl.states == null)
                return false;

            foreach (var val in pl.states.state_par)
            {
                if (val.id == stid)
                {
                    return true;
                }
            }
            return false;
        }

        public static long get_pl_state_end_tm(int state_val, IMapUnit pl)
        {
            if (pl.states == null)
                return 0;

            if ((pl.states.state & state_val) != state_val)
            {
                return 0;
            }

            long end_tm = 0;

            foreach (var val in pl.states.state_par)
            {
                var i = 0;
                var desc_states = val.desc.s_states;
                for (; i < desc_states.Count; ++i)
                {
                    if ((desc_states[i].tp & state_val) != state_val)
                        continue;

                    if (val.end_tm > end_tm)
                    {
                        end_tm = val.end_tm;
                    }
                }
            }

            return end_tm;
        }

        public static int get_pl_state_par(int state_val, IMapUnit pl)
        {
            if (pl.states == null)
                return 0;

            if ((pl.states.state & state_val) != state_val)
            {
                return 0;
            }

            foreach (var val in pl.states.state_par)
            {
                var i = 0;
                var desc_states = val.desc.s_states;
                for (; i < desc_states.Count; ++i)
                {
                    if ((desc_states[i].tp & state_val) != state_val)
                    {
                        continue;
                    }

                    // TO DO : 考虑多个效果叠加情况
                    // 暂时只取头一个值返回

                    return desc_states[i].par * val.par / 100000;
                }
            }

            return 0;
        }

        public static game_err_code _add_bstate_to_ply(int bstate_id, IBaseUnit ply)
        {
            long cur_tm_s = DateTime.Now.ToBinary();

            var bstate_conf = Utility.get_bstate_desc(bstate_id);

            if (bstate_conf == null)
            {
                return game_err_code.CONFIG_ERR;
            }

            BState cur_exist_bstate = null;
            //var ret_state = null;

            IMapUnit pl = ply.get_pack_data();

            if (pl.bstates != null)
            {
                foreach (var bstate in pl.bstates)
                {
                    if (bstate.conf.id == bstate_id)
                    {
                        // 已经有这个祝福状态了
                        cur_exist_bstate = bstate;
                        break;
                    }

                    if (bstate.conf.uni_tp == bstate_conf.uni_tp)
                    {
                        //// 有相同类型的祝福状态（但id不同），不能添加
                        //return game_err_code.BSTATE_SAME_TP_BSTATE_EXIST;

                        // 移除相同类型不同id的祝福状态
                        pl.bstates.Remove(bstate);

                        // send bstate_change msg
                        //::send_rpc(ply.pinfo.sid, 19, { rmvid = bstate.id});
                        break;
                    }
                }
            }
            else
            {
                pl.bstates = new List<BState>();
            }

            if (cur_exist_bstate != null)
            {
                // 已有同一祝福状态，增加持续时间，更改参数
                //ret_state = { id = bstate_conf.id};

                if (bstate_conf.eff.maxpar > 0)
                {
                    if (cur_exist_bstate.par + bstate_conf.eff.par > bstate_conf.eff.maxpar)
                    {
                        // 超出允许叠加参数上限
                        return game_err_code.MAX_BSTATE_PAR_REACHED;
                    }

                    cur_exist_bstate.par += bstate_conf.eff.par;
                    //ret_state.par < -cur_exist_bstate.par;
                }

                if (bstate_conf.merg_tm == 1)
                {
                    cur_exist_bstate.end_tm = cur_exist_bstate.end_tm + bstate_conf.tm; // 延迟持续时间
                }
                else
                {
                    cur_exist_bstate.end_tm = cur_tm_s + bstate_conf.tm; // 延迟持续时间
                }
                //ret_state.end_tm < -cur_exist_bstate.end_tm;
            }
            else
            {
                long ticktm = 0;
                if (bstate_conf.eff.ticktm > 0)
                {
                    ticktm = cur_tm_s + bstate_conf.eff.ticktm;
                }

                BState bstate = new BState()
                {
                    id = bstate_conf.id,
                    par = bstate_conf.eff.par,
                    end_tm = cur_tm_s + bstate_conf.tm,
                    ticktm = ticktm,
                    conf = bstate_conf
                };
                pl.bstates.push(bstate);

                //ret_state = bstate;
            }

            // send bstate_change msg
            //::send_rpc(ply.pinfo.sid, 19, { mod = ret_state});

            return game_err_code.RES_OK;
        }

        // 移除指定祝福状态
        public static void rmv_bstate(int sid, Variant rpc, IBaseUnit ply)
        {
            IMapUnit pl = ply.get_pack_data();
            if (pl.bstates == null || pl.bstates.Count <= 0)
                return;

            int removed_id = rpc["id"]._int32;
            var to_rmv_idx = 0;
            BState to_rmv_bstate = null;
            for (int i = 0; i < pl.bstates.Count; i++)
            {
                if (pl.bstates[i].id == removed_id)
                {
                    to_rmv_bstate = pl.bstates[i];
                    to_rmv_idx = i;
                    pl.bstates.RemoveAt(i);
                    break;
                }
            }

            if (to_rmv_bstate == null)
                return;

            // send bstate_change msg
            //::send_rpc(ply.pinfo.sid, 19, { rmvid = to_rmv_bstate.id});
        }

        //获取祝福的经验倍率
        public static int _get_bstate_exp_mul(IBaseUnit ply)
        {
            IMapUnit pl = ply.get_pack_data();
            if (pl.bstates == null || pl.bstates.Count <= 0)
                return 0;


            foreach (var bstate in pl.bstates)
            {
                if (bstate.conf.eff.tp == (int)bstate_eff_type.BET_EXP_MUL)
                {
                    return bstate.conf.eff.par;
                }
            }

            return 0;
        }
        //获取祝福的掉落倍率
        public static int _get_bstate_rate_mul(IBaseUnit ply)
        {
            IMapUnit pl = ply.get_pack_data();
            if (pl.bstates == null || pl.bstates.Count <= 0)
                return 0;

            foreach (var bstate in pl.bstates)
            {
                if (bstate.conf.eff.tp == (int)bstate_eff_type.BET_RATE_MUL)
                {
                    return bstate.conf.eff.par;
                }
            }

            return 0;
        }

        public static void _update_pl_bstate(long cur_tm_s, IBaseUnit ply)
        {
            IMapUnit pl = ply.get_pack_data();
            if (pl.bstates == null || pl.bstates.Count <= 0)
                return;

            var bstates = pl.bstates;
            for (var idx = 0; idx < bstates.Count; ++idx)
            {
                var bstate = bstates[idx];

                if (bstate.end_tm < cur_tm_s)
                {
                    // 到时间，结束状态
                    bstates.RemoveAt(idx);
                    --idx;

                    // send bstate_change msg
                    //::send_rpc(ply.pinfo.sid, 19, { rmvid = bstate.id});

                    continue;
                }

                if (bstate.ticktm > 0 && bstate.ticktm < cur_tm_s)
                {
                    // 达到定时触发时间

                    bstate.ticktm = cur_tm_s + bstate.conf.eff.ticktm; // 更新下次触发时间

                    var par_change = 0;

                    if (bstate.conf.eff.tp == (int)bstate_eff_type.BET_ADD_HP)
                    {
                        // 增加生命状态

                        var hp_add = pl.max_hp - pl.hp;
                        if (hp_add <= 0)
                        {
                            continue;
                        }

                        if (hp_add > bstate.conf.eff.add)
                        {
                            hp_add = bstate.conf.eff.add;
                        }

                        if (hp_add > bstate.par)
                        {
                            hp_add = bstate.par;
                        }

                        ply.modify_hp(hp_add);

                        bstate.par -= hp_add; // 扣除存储值
                    }
                    else if (bstate.conf.eff.tp == (int)bstate_eff_type.BET_ADD_MP)
                    {
                        // 增加法力状态

                        var mp_add = pl.max_mp - pl.mp;
                        if (mp_add <= 0)
                        {
                            continue;
                        }

                        if (mp_add > bstate.conf.eff.add)
                        {
                            mp_add = bstate.conf.eff.add;
                        }

                        if (mp_add > bstate.par)
                        {
                            mp_add = bstate.par;
                        }

                        ply.modify_mp(mp_add);

                        bstate.par -= mp_add; // 扣除存储值
                    }
                    else
                    {
                        // err，错误的状态，需要删除
                        bstate.par = 0;
                    }

                    //var ret_msg = null;
                    if (bstate.par > 0)
                    {
                        //ret_msg = { mod ={ id = bstate.id, par = bstate.par} };
                    }
                    else
                    {
                        // 消耗完了存储值，删除状态
                        //ret_msg = { rmvid = bstate.id};

                        bstates.RemoveAt(idx);
                        --idx;
                    }

                    // send bstate_change msg
                    //::send_rpc(ply.pinfo.sid, 19, ret_msg);
                }
            }

            //if (ply.pinfo.bstates.len() <= 0)
            //{
            //    delete ply.pinfo.bstates;
            //}
        }
    }
}

