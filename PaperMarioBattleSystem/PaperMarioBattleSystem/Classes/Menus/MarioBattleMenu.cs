using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's battle menu. It contains Strategies, Items, Jump, Hammer, and Star Spirits
    /// </summary>
    public sealed class MarioBattleMenu : PlayerBattleMenu
    {
        public MarioBattleMenu(BattleEntity user) : base(user)
        {
            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            //Get Jump and Hammer battle textures based on Mario's equipment
            CroppedTexture2D jump = new CroppedTexture2D(tex, GetTexRectForBootLevel(User.BManager.Mario.MStats.BootLevel));
            CroppedTexture2D hammer = new CroppedTexture2D(tex, GetTexRectForHammerLevel(User.BManager.Mario.MStats.HammerLevel));

            CroppedTexture2D starPower = new CroppedTexture2D(tex, new Rectangle(182, 812, 24, 24));

            ActionButtons.Add(new ActionButton("Jump", jump,
                Enumerations.MoveCategories.Jump, new JumpSubMenu(user)));
            ActionButtons.Add(new ActionButton("Hammer", hammer,
                Enumerations.MoveCategories.Hammer, new HammerSubMenu(user)));
            ActionButtons.Add(new ActionButton("Special", starPower,
                Enumerations.MoveCategories.Special, new SpecialSubMenu(user)));

            Initialize(2);
        }

        private Rectangle GetTexRectForBootLevel(EquipmentGlobals.BootLevels bootLevel)
        {
            switch (bootLevel)
            {
                case EquipmentGlobals.BootLevels.Super: return new Rectangle(72, 812, 24, 24);
                case EquipmentGlobals.BootLevels.Ultra: return new Rectangle(107, 812, 26, 24);
                case EquipmentGlobals.BootLevels.Normal:
                default: return new Rectangle(36, 812, 24, 24);
            }
        }

        private Rectangle GetTexRectForHammerLevel(EquipmentGlobals.HammerLevels hammerLevel)
        {
            switch (hammerLevel)
            {
                case EquipmentGlobals.HammerLevels.Super: return new Rectangle(72, 844, 24, 24);
                case EquipmentGlobals.HammerLevels.Ultra: return new Rectangle(107, 844, 24, 24);
                case EquipmentGlobals.HammerLevels.Normal:
                default: return new Rectangle(36, 844, 24, 24);
            }
        }
    }
}
