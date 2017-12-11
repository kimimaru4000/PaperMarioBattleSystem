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
            return GetKey(key, KBState);
        }

        public static bool GetKey(Keys key, KeyboardState keyboardState)
        {
            return keyboardState.IsKeyDown(key);
        }

        public static bool GetKeyUp(Keys key)
        {
            return GetKeyUp(key, InputKeyboard);
        }

        public static bool GetKeyUp(Keys key, KeyboardState keyboardState)
        {
            return (keyboardState.IsKeyDown(key) == true && KBState.IsKeyUp(key) == true);
        }

        public static bool GetKeyDown(Keys key)
        {
            return GetKeyDown(key, InputKeyboard);
        }

        public static bool GetKeyDown(Keys key, KeyboardState keyboardState)
        {
            return (keyboardState.IsKeyUp(key) == true && KBState.IsKeyDown(key) == true);
        }

        public static void ClearInputState(params Keys[] keys)
        {
            InputKeyboard = new KeyboardState(keys);
        }

        public static void UpdateInputState()
        {
            UpdateInputState(ref InputKeyboard);
        }

        public static void UpdateInputState(ref KeyboardState keyboardState)
        {
            keyboardState = KBState;
        }
    }
}
