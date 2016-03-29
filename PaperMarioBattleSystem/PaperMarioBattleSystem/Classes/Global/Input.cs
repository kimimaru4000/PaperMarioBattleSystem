using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Static class for handling input
    /// </summary>
    public static class Input
    {
        private static KeyboardState InputKeyboard = default(KeyboardState);

        private static KeyboardState KBState => Keyboard.GetState();

        public static bool GetKey(Keys key)
        {
            return KBState.IsKeyDown(key);
        }

        public static bool GetKeyUp(Keys key)
        {
            return (InputKeyboard.IsKeyDown(key) == true && KBState.IsKeyUp(key) == true);
        }

        public static bool GetKeyDown(Keys key)
        {
            return (InputKeyboard.IsKeyUp(key) == true && KBState.IsKeyDown(key) == true);
        }

        public static void ClearInputState(params Keys[] keys)
        {
            InputKeyboard = new KeyboardState(keys);
        }

        public static void UpdateInputState()
        {
            InputKeyboard = KBState;
        }
    }
}
