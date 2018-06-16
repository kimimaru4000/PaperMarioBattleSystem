using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Class for global values dealing with loading and unloading content
    /// </summary>
    public static class ContentGlobals
    {
        public const string ContentRoot = "Content";
        public const string AudioRoot = "Audio";
        public const string SoundRoot = "Audio/SFX/";
        public const string MusicRoot = "Audio/Music/";
        public const string SpriteRoot = "Sprites";
        public const string UIRoot = "UI";
        public const string BattleGFX = UIRoot + "/Battle/BattleGFX";
        public const string ShaderRoot = "Shaders/";
        public const string ShaderTextureRoot = ShaderRoot + "ShaderTextures/";

        public const string ConfigName = "PMBSConfig.xml";

        public const string LuigiPaletteExtension = "LEmblem";
        public const string WarioPaletteExtension = "WEmblem";
        public const string WaluigiPaletteExtension = "WLEmblem";
    }
}
