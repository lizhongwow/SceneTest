using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace SceneTest
{
    public class _grid_map
    {
        private mapconf map_conf = null;
        public Dictionary<int, Monster> map_mons = new Dictionary<int, Monster>();
        private Dictionary<int, Monster> map_mon_bymid = new Dictionary<int, Monster>();
        public Dictionary<int, IBaseUnit> map_sprites = new Dictionary<int, IBaseUnit>();

        public Dictionary<int, oldPlayer> map_players = new Dictionary<int, oldPlayer>();
        private Dictionary<int, oldPlayer> map_players_bysid = new Dictionary<int, oldPlayer>();
        private Dictionary<int, oldPlayer> map_players_bycid = new Dictionary<int, oldPlayer>();


        public int mapid { get; set; }
        public bool blvlmap { get; set; }

        public map_pk_setting_type pk_seting { get; set; }

        private Dictionary<int, List<Monster>> petmon_cache = new Dictionary<int, List<Monster>>();

        private int last_check_tm { get; set; }

        private bool map_fined { get; set; }

        public bool map_finished { get; set; }

        public void create_monster(monsterconf m_conf)
        {

        }

        public grid_map(int mapid)
        {
            this.map_conf = gameConfs.get_map_conf(mapid);
            this.mapid = mapid;

            for (int i = 0; i < map_conf.m.Count; i++)
            {
                var m_c = gameConfs.get_monster_conf(map_conf.m[i].mid);

                this.create_monster(m_c);
            }
        }

        public bool has_sprite(int iid)
        {
            return map_sprites.ContainsKey(iid);
        }

        public IBaseUnit getSprite(int iid)
        {
            IBaseUnit unit = null;
            this.map_sprites.TryGetValue(iid, out unit);

            return unit;
        }


        public static void _update_pl_tracing(IBaseUnit sprite, Tracing trace, int frang, int trang, int cur_clock_tm)
        {
            bool can_reach = true;

            IMapUnit pl = sprite.get_pack_data();

            if (trace.trace_tm_left > 0 || cur_clock_tm - sprite.last_trace_target_tm > 500)
            {

            }
        }

        public void trig_other_triger(IBaseUnit mapUnit, int trigid)
        {

        }

        public long update_pl_move(IBaseUnit sprite, long now_time)
        {
            return 0;
        }

        public void update_pl_atk_tracing(IBaseUnit sprite, long now_time)
        {

        }

        public void update_pl_atk(IBaseUnit sprite, long now)
        {

        }

        public void update_skill_casting(IBaseUnit sprite, IMapUnit mapUnit, long now)
        {

        }
        public void update_skill_holding(IBaseUnit sprite, IMapUnit mapUnit, long now)
        {

        }
        public void update_jumping(IBaseUnit sprite, IMapUnit mapUnit, long now)
        {

        }
        public void update_teleping(IBaseUnit sprite, IMapUnit mapUnit, long now)
        {

        }

        public void update_keep_atk_range(IBaseUnit sprite, long now)
        {

        }

        public void update_pl_follow_tracing(IBaseUnit sprite, long now)
        {

        }

        public cast_skill_res _cast_skill(long time, Dictionary<string, string> rpc, int skill_type, IBaseUnit caster,
            IBaseUnit target, bool precast)
        {
            

            
        }

        public bool do_cast_target_skill(IBaseUnit caster, Dictionary<string, string> rpc, bool precast = true,
            bool senderr = true)
        {
            int tar_iid = 0;
            int.TryParse(rpc["to_iid"], out tar_iid);

            IBaseUnit tar_sprite = null;
            this.map_sprites.TryGetValue(tar_iid, out tar_sprite);

            if (tar_sprite != null)
            {
                if (tar_sprite.has_state(pl_state_type.PST_SKILL_AVOID))
                {
                    //if(senderr)

                    return false;
                }

                if(tar_sprite.isdie() || tar_sprite.isghost())
                    return false;

                long now = DateTime.Now.ToBinary();
                cast_skill_res castSkillRes=_cast_skill(now,)
            }

        }

        public cast_skill_res _cast_skill(long now, Dictionary<string, string> rpc, skill_type skill_type,
            IBaseUnit caster, IBaseUnit target, bool precast = false, Point2D to_pos = null)
        {
            if (caster.isghost())
                return new cast_skill_res() { res = game_err_code.YOU_ARE_GHOST };

            IMapUnit pl = caster.get_pack_data();
            if (pl.atking != null)
                return new cast_skill_res() { res = game_err_code.SKIL_ALREADY_CASTING_SKILL };

            if (pl.jumping != null)
                return new cast_skill_res(game_err_code.SKIL_CANT_CAST_WHEN_JUMP);

            int skill_id = Convert.ToInt32(rpc["sid"]);


            if (pl.holding != null && pl.holding.sid != skill_id)
                return new cast_skill_res(game_err_code.SKIL_ALREADY_HOLDING_SKILL);

            skill_conf skillConf = Utility.GetSkillConf(skill_id);
            if (skillConf == null)
                return new cast_skill_res(game_err_code.SKIL_NOT_EXIST);

            if (!skillConf.igccstate)
            {
                if (caster.has_state(pl_state_type.PST_CANT_CAST_SKILL))
                    return new cast_skill_res(game_err_code.SKIL_CANT_CAST);
            }

            if (skillConf.tar_tp != (int)skill_type)
                return new cast_skill_res(game_err_code.SKIL_CAST_TARGET_TYPE_ERR);

            int skill_level = caster.get_skil_data(skill_id);
            if (skill_level <= 0)
            {
                skill_level = this.get_map_skill(skill_id);
                if (skill_level <= 0)
                    return new cast_skill_res(game_err_code.SKIL_NOT_LEARN);
            }

            if (target != null)
            {
                IMapUnit tar_pl = target.get_pack_data();
                if (tar_pl.invisible > pl.invisible)
                    return new cast_skill_res(game_err_code.SKIL_TARGET_NOT_AVALIBLE);

                if (skillConf.tar_tp == (int)skill_type.ST_TARGET)
                {
                    if (caster.iid != target.iid)
                    {
                        if (!skillConf.Is_Target_Enermy())
                        {
                            if (caster.can_atk(target))
                                return new cast_skill_res(game_err_code.SKIL_INVALIDE_TAREGET);

                            bool isaly = caster.isaily(target);
                            if (isaly)
                            {
                                if (!skillConf.Is_Target_ALAI())
                                    return new cast_skill_res(game_err_code.SKIL_INVALIDE_TAREGET);
                            }
                            else
                            {
                                if (!skillConf.Is_Target_MID())
                                    return new cast_skill_res(game_err_code.SKIL_INVALIDE_TAREGET);
                            }
                        }
                        else if (caster.Is_Player())
                        {
                            if (target.Is_Player())
                            {
                                if (this.pk_seting == map_pk_setting_type.MPST_PEACE)
                                    return new cast_skill_res(game_err_code.CANT_PK_IN_PEACE_MAP);

                                if (pl.in_pczone || tar_pl.in_pczone)
                                    return new cast_skill_res(game_err_code.CANT_PK_IN_PEACE_ZONE);

                                if (this.pk_seting == map_pk_setting_type.MPST_NORMAL)
                                {
                                    if (!target.can_atk_direct())
                                        if (!caster.can_atk(target))
                                            return new cast_skill_res(game_err_code.NULL);
                                }
                                else
                                {
                                    if (!caster.can_atk(target))
                                        return new cast_skill_res(game_err_code.NULL);
                                }
                            }
                            else if (target.Is_Monster())
                            {
                                if (target.owner_ply != null)
                                {
                                    IBaseUnit owner_sprite = target.owner_ply;
                                    IMapUnit owner_pl = owner_sprite.get_pack_data();

                                    if (owner_sprite.iid == caster.iid)
                                        return new cast_skill_res(game_err_code.CANT_ATTACK_SELF_PET);

                                    if (this.pk_seting == map_pk_setting_type.MPST_PEACE)
                                        return new cast_skill_res(game_err_code.CANT_PK_IN_PEACE_MAP);

                                    if (pl.in_pczone || owner_pl.in_pczone)
                                        return new cast_skill_res(game_err_code.CANT_PK_IN_PEACE_ZONE);

                                    if (this.pk_seting == map_pk_setting_type.MPST_NORMAL)
                                    {
                                        if (!owner_sprite.can_atk_direct())
                                            if (!caster.can_atk(owner_sprite))
                                                return new cast_skill_res(game_err_code.NULL);
                                    }
                                    else
                                    {
                                        if (!caster.can_atk(owner_sprite))
                                            return new cast_skill_res(game_err_code.NULL);
                                    }
                                }
                                else
                                {
                                    if (!caster.can_atk(target))
                                        return new cast_skill_res(game_err_code.NULL);
                                }
                            }
                            else
                            {
                                if (!skillConf.Is_Target_SELF())
                                    return new cast_skill_res(game_err_code.SKIL_INVALIDE_TAREGET);
                            }
                        }
                    }
                }
            }

            skill_lv_conf skill_level_conf = skillConf.GetSkillLevelConf(skill_level);
            if (null == skill_level_conf)
                return new cast_skill_res(game_err_code.SKIL_LVL_ERR);

            if (caster.Is_Skill_In_CD(skill_id, now))
                return new cast_skill_res(game_err_code.SKIL_CD_NOT_REACH);

            if (pl.moving != null)
                update_pl_move(caster, now);

            int cast_rang = pl.size + pl.atkrange;
            if (skillConf.cast_rang > 0)
                cast_rang += skillConf.cast_rang;

            if (to_pos != null)
            {
                if (target != null)
                {
                    cast_rang += target.get_pack_data().size;
                    if (target.get_pack_data().moving != null)
                        update_pl_move(target, now);
                }

                if (skill_level_conf.jump == null && skill_level_conf.teleport == null)
                {
                    double d_x = to_pos.x - caster.x;
                    double d_y = to_pos.y - caster.y;
                    long rang = cast_rang + 45;
                    if (d_x * d_x + d_y * d_y > rang * rang)
                        return new cast_skill_res(game_err_code.SKIL_CAST_RANG_ERR);
                }
            }

            if (precast && skillConf.cast_tm > 0)
            {
                pl.moving = null;

                pl.casting = new casting() { end_tm = now + skillConf.cast_tm * 100, tar_tp = (int)skill_type, rpc = rpc };

                return new cast_skill_res(game_err_code.RES_OK);
            }

            if (skillConf.hold_tm < 0)
            {
                if (pl.holding != null)
                    pl.holding = null;
                else
                {
                    pl.moving = null;

                    pl.holding = new holding() { tar_tp = (int)skill_type, rpc = rpc, start_tm = now, end_tm = now + skillConf.hold_tm * 100 };

                    return new cast_skill_res(game_err_code.RES_OK);
                }
            }

            if (pl.invisible > 0)
            {
                oldSkill.rmv_state_from_pl_by_att("invisible", caster);
            }

            caster.Set_Skill_CD(skill_id, now, now + skillConf.cd_tm * 100);

            cast_skill_res res = new cast_skill_res(game_err_code.RES_OK);
            res.rpc_data = new Dictionary<string, string>();

            if (to_pos != null)
            {
                if (skill_level_conf.teleport != null)
                {
                    pl.moving = null;
                    pl.last_mvpts = null;

                    Point2D t_point = Utility.valpoint_on_line(caster, to_pos, skill_level_conf.teleport.rang);

                    if (skill_level_conf.teleport.tm <= 0)
                    {
                        caster.x = (int)t_point.x;
                        caster.y = (int)t_point.y;
                    }
                    else
                    {
                        pl.teleping = new teleping()
                        {
                            dest_x = (int)t_point.x,
                            dest_y = (int)t_point.y,
                            end_tm = now + skill_level_conf.teleport.tm * 100,
                            percent = 100,
                            rpc = rpc,
                            start_tm = now,
                            telep = skill_level_conf.teleport
                        };
                    }
                }
                else if (skill_level_conf.jump != null)
                {
                    pl.moving = null;
                    pl.last_mvpts = null;

                    int sub_len = 0;
                    if (target != null)
                        sub_len = 45;

                    Point2D dest_pos = Utility.valpoint_on_line(caster, to_pos, skill_level_conf.jump.rang, sub_len);

                    double dist_x = dest_pos.x - caster.x;
                    double dist_y = dest_pos.y - caster.y;
                    double rang = Math.Sqrt(dist_x * dist_x + dist_y * dist_y);

                    int jump_tm = (int)(rang * 1000 / skill_level_conf.jump.speed);
                    if (jump_tm <= 0)
                        jump_tm = 1;

                    pl.jumping = new jumping()
                    {
                        dest_x = (int)dest_pos.x,
                        dest_y = (int)dest_pos.y,
                        during_tm = jump_tm,
                        end_tm = now + jump_tm,
                        start_tm = now,
                        jump = skill_level_conf.jump,
                        percent = 100,
                        rpc = rpc,
                        tar_iid = (target == null) ? 0 : target.iid,
                    };

                }


            }

            return res;
        }

        public int get_map_skill(int skillid)
        {
            return 0;
        }

        public void _post_cast_skill(long now, IBaseUnit caster, Dictionary<string, string> rpc,
            cast_skill_res cast_skill_res, IBaseUnit target, Point2D target_pos, bool check_fly = true)
        {
            skill_lv_conf levelConf = cast_skill_res.LevelConf;
            if (levelConf.tres != null)
            {
                if (levelConf.trang != null)
                {
                    if (levelConf.jump != null)
                    {
                        oldSkill.apply_rang_eff(now, caster, new Point2D(caster.x, caster.y), levelConf.tres, levelConf.trang, 100);
                    }
                    else
                    {
                        if(target_pos != null)
                            oldSkill.apply_rang_eff(now,caster,target_pos,levelConf.tres,levelConf.trang,100);
                        else if (target != null)
                        {
                            oldSkill.apply_rang_eff(now, caster, new Point2D(target.x, target.y), levelConf.tres,
                                levelConf.trang, 100);
                        }
                        else
                        {
                            oldSkill.apply_rang_eff(now,caster,new Point2D(caster.x,caster.y),levelConf.tres,levelConf.trang,100 );
                        }
                    }
                }
                else
                {
                    if (target != null)
                    {
                        foreach (var tres in levelConf.tres)
                        {
                            oldSkill.apply_skill_eff_to(now, caster, target, tres, tres.aff, 100);
                        }
                    }
                }
            }
        }

        public bool do_cast_target_skill(IBaseUnit caster, Dictionary<string, string> rpc, bool precast = true)
        {
            int tar_iid = Utility.GetDicInt(rpc, "to_iid");
            if (this.map_sprites.ContainsKey(tar_iid) && this.map_sprites.ContainsKey(caster.iid))
            {
                IBaseUnit tar_sprite = this.map_sprites[tar_iid];

                if (tar_sprite.has_state(pl_state_type.PST_ATK_AVOID))
                    return false;

                if(tar_sprite.isdie() || tar_sprite.isghost())
                    return false;

                long now = DateTime.Now.ToBinary();

                var cast_skill_res = _cast_skill(now, rpc, skill_type.ST_TARGET, caster, tar_sprite, precast);
                if(cast_skill_res == null)
                    return false;

                if(cast_skill_res.res != game_err_code.RES_OK)
                    return false;

                if(cast_skill_res.LevelConf.sres != null)
                       oldSkill.apply_skill_eff_to(now,caster,caster,cast_skill_res.LevelConf.sres,cast_skill_res.)

                _post_cast_skill(now,caster,rpc,cast_skill_res,tar_sprite,tar_sprite.get_pack_data());

                if(caster.Is_Player()){
                    if(!tar_sprite.isdie() && !tar_sprite.isghost()){
                        if(caster.petmon_inst && (cast_skill_res.))
                    }
                }
            }
        }

        public static void apply_dmg_on_pl()
        {
            
        }
    }
}
