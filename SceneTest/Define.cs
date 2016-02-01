using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
    public class Define
    {
        // Fields
        public static uint COMBOBOX_CHANGE = 0x474;
        public static uint COMBOBOX_CLOSE = 0x475;
        public static uint COMBOBOX_ITEMROLLOUT = 0x479;
        public static uint COMBOBOX_ITEMROLLOVER = 0x478;
        public static uint COMBOBOX_OPEN = 0x476;
        public static uint COMBOBOX_SCROLL = 0x477;
        public static int SLIDER_CHANGE = 0x44d;
        public static int SLIDER_THUMBDRAG = 0x44e;
        public static int SLIDER_THUMBPRESS = 0x44f;
        public static int SLIDER_THUMBRELEASE = 0x450;
        public static uint SUPERTEXT_ELEMENTCLICK = 0x49c;
        public static uint SUPERTEXT_ELEMENTOUT = 0x49e;
        public static uint SUPERTEXT_ELEMENTOVER = 0x49d;

        // Methods
        public static EventType convertToEventType(string str)
        {
            switch (str)
            {
                case "joystickmove":
                    return EventType.JOYSTICK_MOVE;

                case "joystickend":
                    return EventType.JOYSTICK_END;

                case "joystickbegin":
                    return EventType.JOYSTICK_BEGIN;

                case "mousedown":
                case "inputdown":
                    return EventType.MOUSE_DOWN;

                case "mousemove":
                case "inputmove":
                    return EventType.MOUSE_MOVE;

                case "mouseup":
                case "inputup":
                    return EventType.MOUSE_UP;

                case "inputclick":
                case "click":
                    return EventType.MOUSE_CLICK;

                case "mouseover":
                    return EventType.MOUSE_OVER;

                case "mouseout":
                    return EventType.MOUSE_LEAVE;

                case "process":
                    return EventType.PROCESSS;

                case "sliderchange":
                    return EventType.SLIDERCHANGE;
            }
            return EventType.UNKNOWN;
        }

        // Nested Types
        public enum ButtonState
        {
            NORMAL,
            UP,
            DOWN,
            DISABLE,
            SELECTNO,
            SELECTYES,
            SELECTOVER
        }

        public enum Ccm_status
        {
            NONE,
            LOADING_MAINFILE,
            LOADING_CONF,
            FORMAT_CONF,
            LOADED,
            ERR
        }

        public enum DebugTrace
        {
            DTT_NONE,
            DTT_SYS,
            DTT_ERR,
            DTT_DTL
        }

        public enum Endian
        {
            BIG_ENDIAN,
            LITTLE_ENDIAN
        }

        public enum EventType
        {
            UNKNOWN,
            MOUSE_DOWN,
            MOUSE_MOVE,
            MOUSE_UP,
            MOUSE_CLICK,
            MOUSE_OVER,
            MOUSE_LEAVE,
            UI_MOUSE_DOWN,
            UI_MOUSE_UP,
            UI_MOUSE_MOVE,
            JOYSTICK_BEGIN,
            JOYSTICK_MOVE,
            JOYSTICK_END,
            SKANIM_BEGIN,
            SKANIM_END,
            PROCESSS,
            RAYCASTED,
            SLIDERCHANGE
        }

        public enum GREntityType
        {
            STATIC_MESH,
            CHARACTER,
            EFFECT_PARTICLE,
            CAMERA,
            LIGHTDIR,
            LIGHTPOINT,
            BILLBOARD,
            EFFECT_KNIFELIGHT
        }

        public enum MouseButton
        {
            None,
            Left,
            Middle,
            Right
        }

        public enum Movement
        {
            NONE,
            HORIZONTAL,
            VERTICAL
        }

        public enum PHEntityType
        {
            HEIGHTMAP,
            COLLIDER_MESH
        }

        public enum ProgressDirection
        {
            HORIZAL,
            VERTICAL
        }

        public enum SkAnimState
        {
            BEGIN,
            END
        }

        public enum TextAlign
        {
            LEFT,
            MIDDLE,
            RIGHT
        }

        public enum UIDirection
        {
            HORIZONTAL,
            VERTICAL,
            BOTH
        }
    }



}
