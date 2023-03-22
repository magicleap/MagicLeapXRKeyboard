using System.Collections.Generic;

namespace MagicLeap.XRKeyboard.Model
{
    public static class KeyboardCollections
    {
       

        public static Dictionary<string, string> NonStandardKeyToDisplayString = new Dictionary<string, string>()
                                                                                                {
                                                                                                    { "SHIFT_NEUTRAL", "\ue5f2"},
                                                                                                    { "SHIFT_SHIFT", "\uf7ae"},
                                                                                                    { "SHIFT_CAPS", "\ue318" },
                                                                                                    { "NUMBERPAD", "\uf045"},
                                                                                                    { "BACKSPACE", "\ue14a" },
                                                                                                    { "SPACE", " " },
                                                                                                    { "RETURN", "\ue31b" },
                                                                                                    { "SHIFT", "\ue5f2" },

                                                                                                };


        public static Dictionary<string, string> LabelsToCode = new Dictionary<string, string>()
                                                                                 {
                                                                                     { "BACKSPACE", "\u0008" },
                                                                                     { "SPACE", " " },
                                                                                     { "RETURN", "\n" },

                                                                                 };



    
    }
}
