using SceneTestLib.Confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SceneTestLib
{
    public class ConfReader
    {

        public static Dictionary<int, bstate_conf> BState_Confs = new Dictionary<int, bstate_conf>();
        public static Dictionary<int, state_conf> State_Confs = new Dictionary<int, state_conf>();
        public static Dictionary<int, skill_conf> Skill_Confs = new Dictionary<int, skill_conf>();

        public static Dictionary<int, mon_conf> Monster_Confs = new Dictionary<int, mon_conf>();

        public static Dictionary<int, map_conf> Map_Confs = new Dictionary<int, map_conf>();

        public static void init()
        {
            string fileFolder = @"D:\PrimaryServer\whserver\data\gameconfs";
            
            load_monsters(@"D:\PrimaryServer\whserver\data\maps\monsters.xml");
            load_maps(@"D:\PrimaryServer\whserver\data\maps\map.xml");

            load_skills(fileFolder + "\\skills.xml");
        }

        public static void load_maps(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlElement root_node = null;
            foreach (var nodex in doc.ChildNodes)
            {
                if (nodex is XmlElement)
                {
                    XmlElement node = nodex as XmlElement;
                    root_node = node;
                    break;
                }
            }

            //string[] node_names = new string[] { "mon" };

            //foreach (string sx in node_names)
            foreach (XmlNode n in root_node.ChildNodes)
            {
                XmlElement node = n as XmlElement;
                if (node == null)
                    continue;

                if (node.Name == "map")
                {
                    map_conf bstate_conf = ConvertXMLElementToConfInstance<map_conf>(node);
                    Map_Confs.Add(bstate_conf.id, bstate_conf);
                }
            }

            Console.WriteLine("finish load monsters");
        }


        public static void load_monsters(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlElement root_node = null;
            foreach (var nodex in doc.ChildNodes)
            {
                if (nodex is XmlElement)
                {
                    XmlElement node = nodex as XmlElement;
                    root_node = node;
                    break;
                }
            }

            //string[] node_names = new string[] { "mon" };

            //foreach (string sx in node_names)
            foreach (XmlNode n in root_node.ChildNodes)
            {
                XmlElement node = n as XmlElement;
                if (node == null)
                    continue;

                if (node.Name == "mon")
                {
                    mon_conf bstate_conf = ConvertXMLElementToConfInstance<mon_conf>(node);
                    Monster_Confs.Add(bstate_conf.id, bstate_conf);
                }
            }

            Console.WriteLine("finish load monsters");
        }

        public static void load_skills(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlElement root_node = null;
            foreach (var nodex in doc.ChildNodes)
            {
                if (nodex is XmlElement)
                {
                    XmlElement node = nodex as XmlElement;
                    root_node = node;
                    break;
                }
            }

            //string[] node_names = new string[] { "bstate", "state", "skill" };

            foreach (XmlNode n in root_node.ChildNodes)
            {
                XmlElement node = n as XmlElement;
                if (node == null)
                    continue;

                if (node.Name == "bstate")
                {
                    bstate_conf bstate_conf = ConvertXMLElementToConfInstance<bstate_conf>(node);
                    BState_Confs.Add(bstate_conf.id, bstate_conf);
                }
                else if (node.Name == "state")
                {
                    state_conf state_conf = ConvertXMLElementToConfInstance<state_conf>(node);
                    State_Confs.Add(state_conf.id, state_conf);
                }
                else if (node.Name == "skill")
                {
                    skill_conf sk_conf = ConvertXMLElementToConfInstance<skill_conf>(node);
                    Skill_Confs.Add(sk_conf.id, sk_conf);
                }


                Console.WriteLine("finish load skills");
            }
        }


        public static T ConvertXMLElementToConfInstance<T>(XmlElement element) where T : class, new()
        {
            T instance = new T();
            var properties = instance.GetType().GetProperties();
            foreach (XmlNode attr in element.Attributes)
            {
                foreach (var p in properties)
                {
                    if (string.Compare(p.Name, attr.Name, StringComparison.Ordinal) == 0)
                    {
                        p.SetValue(instance, Convert.ChangeType(attr.Value, p.PropertyType), null);
                        break;
                    }
                }
            }

            foreach (XmlNode node in element.ChildNodes)
            {
                XmlElement ele = node as XmlElement;
                if (ele == null)
                    continue;

                PropertyInfo p_info = null;
                foreach (var pinfo in properties)
                {
                    if (string.Compare(pinfo.Name, ele.Name) == 0)
                    {
                        p_info = pinfo;
                        break;
                    }
                }

                if (p_info == null)
                    continue;

                object conf = null;

                switch (ele.Name)
                {
                    //list属性
                    case s_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<s_conf>(ele);
                        List<s_conf> value = p_info.GetValue(instance) as List<s_conf>;
                        value.Add(conf as s_conf);
                        break;

                    case att_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<att_conf>(ele);
                        List<att_conf> att_value = p_info.GetValue(instance) as List<att_conf>;
                        att_value.Add(conf as att_conf);
                        break;

                    case skill_lv_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<skill_lv_conf>(ele);
                        List<skill_lv_conf> skill_lv_value = p_info.GetValue(instance) as List<skill_lv_conf>;
                        skill_lv_value.Add(conf as skill_lv_conf);
                        break;

                    case tres_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<tres_conf>(ele);
                        List<tres_conf> tres_value = p_info.GetValue(instance) as List<tres_conf>;
                        tres_value.Add(conf as tres_conf);
                        break;

                    case map_mon_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<map_mon_conf>(ele);
                        List<map_mon_conf> map_mon_value = p_info.GetValue(instance) as List<map_mon_conf>;
                        map_mon_value.Add(conf as map_mon_conf);
                        break;




                    //单个属性
                    case eff_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<eff_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;

                    case unique_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<unique_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;


                    case timer_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<timer_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;

                    case dmg_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<dmg_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;



                    case rang_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<rang_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;

                    case cd_red_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<cd_red_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;

                    case ray_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<ray_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;

                    case decay_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<decay_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;

                    case absorbdmg_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<absorbdmg_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;



                    case jump_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<jump_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;

                    case teleport_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<teleport_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;

                    case mon_conf.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<mon_conf>(ele);
                        p_info.SetValue(instance, conf);
                        break;

                    case mon_att.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<mon_att>(ele);
                        p_info.SetValue(instance, conf);
                        break;

                    case mon_ai.xml_node_name:
                        conf = ConvertXMLElementToConfInstance<mon_ai>(ele);
                        p_info.SetValue(instance, conf);
                        break;
                }

            }

            return instance;
        }


        //public static void set_property_value<T>(T conf_instance, XmlElement current_node, PropertyInfo[] bstate_properties)
        //{
        //    foreach (var attr_x in current_node.Attributes)
        //    {
        //        XmlAttribute attr = attr_x as XmlAttribute;
        //        foreach (PropertyInfo px in bstate_properties)
        //        {
        //            if (string.Compare(attr.Name, px.Name, StringComparison.Ordinal) == 0)
        //            {
        //                px.SetValue(conf_instance, Convert.ChangeType(attr.Value, px.PropertyType), null);
        //                break;
        //            }
        //        }
        //    }

        //    foreach (var attr_x in current_node.ChildNodes)
        //    {
        //        if (!(attr_x is XmlElement))
        //            continue;

        //        XmlElement ele = attr_x as XmlElement;
        //        PropertyInfo p_info = null;
        //        foreach (var pinfo in bstate_properties)
        //        {
        //            if (string.Compare(pinfo.Name, ele.Name) == 0)
        //            {
        //                p_info = pinfo;
        //                break;
        //            }
        //        }

        //        if (p_info == null)
        //            continue;

        //        object conf = null;

        //        switch (ele.Name)
        //        {
        //            //list属性
        //            case s_conf.xml_node_name:
        //                conf = ConvertXMLElementToConfInstance<s_conf>(ele);
        //                List<s_conf> value = p_info.GetValue(conf_instance) as List<s_conf>;
        //                value.Add(conf as s_conf);
        //                break;

        //            case att_conf.xml_node_name:
        //                conf = ConvertXMLElementToConfInstance<att_conf>(ele);
        //                List<att_conf> att_value = p_info.GetValue(conf_instance) as List<att_conf>;
        //                att_value.Add(conf as att_conf);
        //                break;


        //            //单个属性
        //            case eff_conf.xml_node_name:
        //                conf = ConvertXMLElementToConfInstance<eff_conf>(ele);
        //                p_info.SetValue(conf_instance, conf);
        //                break;

        //            case unique_conf.xml_node_name:
        //                conf = ConvertXMLElementToConfInstance<unique_conf>(ele);
        //                p_info.SetValue(conf_instance, conf);
        //                break;


        //            case timer_conf.xml_node_name:
        //                conf = ConvertXMLElementToConfInstance<timer_conf>(ele);
        //                p_info.SetValue(conf_instance, conf);
        //                break;
        //        }


        //        continue;

        //        //string type_name = "SceneTestLib.Confs." + ele.Name + "_conf";
        //        //object ob = Assembly.GetAssembly(typeof(eff_conf)).CreateInstance(type_name);

        //        //if (ob == null)
        //        //{
        //        //    Console.WriteLine("err when create :" + type_name);
        //        //    continue;
        //        //}

        //        //Type type = ob.GetType();

        //        //set_property_value(ob, ele, type.GetProperties());

        //        //var conf_properties = conf_instance.GetType().GetProperties();

        //        //foreach (var cp_info in conf_properties)
        //        //{
        //        //    if (string.Compare(cp_info.Name, ele.Name, StringComparison.Ordinal) == 0)
        //        //    {
        //        //        var cp_value = cp_info.GetValue(conf_instance, null);

        //        //        if ((cp_value as IEnumerable<object>) != null)
        //        //        {
        //        //            var cp_ary = (cp_value as IEnumerable<object>).ToList<object>();
        //        //            cp_ary.Add(ob);

        //        //            cp_info.SetValue(conf_instance, cp_ary);
        //        //        }
        //        //        else
        //        //        {
        //        //            cp_info.SetValue(conf_instance, ob, null);
        //        //        }
        //        //    }
        //        //    //if(cp_info.Name == ele)
        //        //}
        //    }

        //}
    }
}
