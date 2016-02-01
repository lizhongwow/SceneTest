using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
    public class oldSkill
    {
        public static void update_pl_state(long now, IBaseUnit sprite)
        {

        }

        public static void rmv_state_from_pl_by_att(string att_key, IBaseUnit baseUnit)
        {

        }

        public static void rmv_stat_from_pl_by_gb(int state, IBaseUnit target)
        {

        }

        public static void rmv_1_stat_from_pl_by_gb(int state, IBaseUnit target)
        {

        }

        public static IBaseUnit_State add_state_to_pl(long now, IBaseUnit target, skill_state_conf sk_res, IBaseUnit from, int per)
        {

        }

        public static bool apply_skill_eff_to(long now, IBaseUnit from, IBaseUnit target, skill_state_conf sk_res,
            int aff, int percentage)
        {
            if (target.isghost() || target.isdie())
                return false;

            if (target.ignore_dmg())
                return false;

            if (sk_res.rate > 0)
            {
                if (new Random().Next(0, 100) > sk_res.rate)
                    return false;
            }

            if (target.has_state(pl_state_type.PST_SKILL_AVOID))
                return false;

            if (from.iid != target.iid)
            {
                if (!skill_conf.skill_target_type_validation(aff, skill_aff_type.SAT_ENERMY))
                {
                    if (from.can_atk(target))
                        return false;

                    if (from.isaily(target))
                    {
                        if (!skill_conf.skill_target_type_validation(aff, skill_aff_type.SAT_ALLY))
                            return false;
                    }
                    else
                    {
                        if (!skill_conf.skill_target_type_validation(aff, skill_aff_type.SAT_MID))
                            return false;
                    }
                }
                else
                {
                    if (from.Is_Player())
                    {
                        if (target.Is_Player())
                        {
                            if (target.get_pack_data().in_pczone || from.get_pack_data().in_pczone)
                                return false;

                            if (!from.can_atk(target))
                                return false;
                        }
                        else if (target.Is_Monster())
                        {
                            if (target.owner_ply != null)
                            {
                                if (target.owner_ply.iid == from.iid)
                                    return false;

                                if (target.owner_ply.get_pack_data().in_pczone || from.get_pack_data().in_pczone)
                                    return false;

                                if (!from.can_atk(target))
                                    return false;
                            }
                        }
                    }
                    else
                    {
                        if (!from.can_atk(target))
                            return false;
                    }
                }
            }
            else
            {
                if(!skill_conf.skill_target_type_validation(aff,skill_aff_type.SAT_SELF))
                    return false;
            }

            if (from.Is_Player())
            {
                if(target.can_atk(from))
                    target.atk_by(from);
            }

            if (sk_res.nodmg == 0)
            {
                //TODO do damage
            }

            if (sk_res.rmv_stat > 0)
                rmv_stat_from_pl_by_gb(sk_res.rmv_stat,target);

            if(sk_res.rmv_1_stat >0)
                rmv_1_stat_from_pl_by_gb(sk_res.rmv_1_stat,target);

            if (sk_res.tar_state > 0)
            {
                IBaseUnit_State state = add_state_to_pl(now, target, sk_res, from, 100);
            }

            if (sk_res.force_move_dir != -1)
            {
                int dir = sk_res.force_move_dir;
                if (sk_res.force_move_dir == 2)
                    dir = new Random().Next(0, 2);

                int delta_x = target.x - from.x;
                int delta_y = target.y - from.y;

                if (delta_x != 0 || delta_y != 0)
                {
                    Point2D vec=new Point2D(delta_x,delta_y);
                    vec = Utility.normalize_vec2(vec);

                    delta_x = (int)(sk_res.force_move_rang*vec.x);
                    delta_y = (int) (sk_res.force_move_rang*vec.y);

                    if (sk_res.force_move_dir == 0)
                    {
                        target.x += delta_x;
                        target.y += delta_y;
                    }
                    else
                    {
                        target.x -= delta_x;
                        target.y -= delta_y;
                    }
                }

                target.get_pack_data().moving = null;
                target.get_pack_data().casting = null;
                target.get_pack_data().last_mvpts = null;
            }

            return true;
        }

        public static void apply_rang_eff(long now, IBaseUnit from, Point2D center, List<skill_state_conf> sk_res,
            skill_conf_trang trang, int percentage)
        {
            grid_map gmap = from.gmap;
            int maxi = trang.maxi;
            if (maxi <= 0)
                maxi = int.MaxValue;

            int aff_count = 0;
            if (trang.cirang > 0)
            {
                long _rang = trang.cirang*trang.cirang;
                foreach (var m in gmap.map_players.Values)
                {
                    if(m.isdie() || m.isghost())
                        continue;

                    long _dist_x = Utility.distance2(m, from);

                    if(_dist_x > _rang)
                        continue;

                    bool affed = false;
                    foreach (skill_state_conf tres in sk_res)
                       affed= apply_skill_eff_to(now, from, m, tres, tres.aff, 100);

                    if (affed)
                    {
                        aff_count++;
                        if(aff_count >= maxi)
                            break;
                    }
                }

                if (aff_count < maxi)
                {
                    foreach (var m in gmap.map_mons.Values)
                    {
                        if(m.isdie() || m.isghost())
                            continue;

                        if(Utility.distance2(m,from) > _rang)
                            continue;

                        bool affed = false;
                        foreach (skill_state_conf tres in sk_res)
                            affed = apply_skill_eff_to(now, from, m, tres, tres.aff, 100);

                    }
                }
            }
        }

        //public static bool apply_skill_eff_to(long now, IBaseUnit from, IBaseUnit target, skill_state_conf tres, int aff)
        //{
        //    if(target.isdie() || target.isghost())
        //        return false;

        //    if(target.ignore_dmg())
        //        return false;

        //    if (tres.rate > 0)
        //    {
        //        if (new Random().Next(0, 100) > tres.rate)
        //            return false;
        //    }

        //    if(target.has_state(pl_state_type.PST_SKILL_AVOID))
        //        return false;

        //    if (from.iid != target.iid)
        //    {
        //        if (!skill_conf.skill_target_type_validation(aff, skill_aff_type.SAT_ENERMY))
        //        {
        //            #region
        //            if (from.can_atk(target))
        //                return false;

        //            if (from.isaily(target))
        //            {
        //                if(!skill_conf.skill_target_type_validation(aff,skill_aff_type.SAT_ALLY))
        //                    return false;
        //            }
        //            else
        //            {
        //                if(!skill_conf.skill_target_type_validation(aff,skill_aff_type.SAT_MID))
        //                    return false;
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            if (from.Is_Player())
        //            {
        //                if (target.Is_Player())
        //                {
        //                    if (from.get_pack_data().in_pczone || target.get_pack_data().in_pczone)
        //                        return false;

        //                    if(!from.can_atk(target))
        //                        return false;
        //                }
        //                else if(target.Is_Monster())
        //                {
        //                    if (target.owner_ply != null)
        //                    {
        //                        IBaseUnit owner = target.owner_ply;

        //                        if(owner.iid == from.iid)
        //                            return false;

        //                        if(owner.get_pack_data().in_pczone || from.get_pack_data().in_pczone)
        //                            return false;

        //                        if(!from.can_atk(owner))
        //                            return false;
        //                    }
        //                    else
        //                    {
        //                        if(!from.can_atk(target))
        //                            return false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if(!skill_conf.skill_target_type_validation(aff,skill_aff_type.SAT_SELF))
        //            return false;
        //    }

        //    if (from.Is_Player())
        //    {
        //        if(target.can_atk(from))
        //            target.atk_by(from);
        //    }

        //    if (tres.nodmg == 0)
        //    {
        //        //TODO apply_dmg_on_pl
        //    }

        //    if (tres.rmv_stat > 0)
        //    {
                
        //    }
        //}
    }
}
