using Miio.Devices.Models.Enums;

namespace Miio.Devices.Implementations.Yeelight
{
    public class YeelightCommands : BasicCommands
    {
        private static YeelightCommands _toggle;
        private static YeelightCommands _setBrightness;
        private static YeelightCommands _setCt;
        private static YeelightCommands _setRgb;
        private static YeelightCommands _setHsv;

        protected YeelightCommands(string actualCommandName) : base(actualCommandName)
        {
        }

        public static YeelightCommands TOGGLE
        {
            get
            {
                return _toggle ?? (_toggle = new YeelightCommands("toogle"));
            }
        }
        public static YeelightCommands SET_BRIGHTNESS
        {
            get
            {
                return _setBrightness ?? (_setBrightness = new YeelightCommands("set_bright"));
            }
        }
        public static YeelightCommands SET_COLOR_TEMPERATURE
        {
            get
            {
                return _setCt ?? (_setCt = new YeelightCommands("set_ct_abx"));
            }
        }
        public static YeelightCommands SET_RGB_COLOR
        {
            get
            {
                return _setRgb ?? (_setRgb = new YeelightCommands("set_rgb"));
            }
        }

        public static YeelightCommands SET_HSV_COLOR
        {
            get
            {
                return _setHsv ?? (_setHsv = new YeelightCommands("set_hsv"));
            }
        }
    }
}
