using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The menu shown after selecting Items when the BattleEntity has the Double and/or Triple Dip Badge equipped.
    /// </summary>
    public sealed class ItemDipSubMenu : ActionSubMenu
    {
        public ItemDipSubMenu(BattleEntity user) : base(user)
        {
            Name = "Items";
            Position = new Vector2(230, 150);

            int doubleDipCount = User.GetEquippedNPBadgeCount(BadgeGlobals.BadgeTypes.DoubleDip);
            int tripleDipCount = User.GetEquippedNPBadgeCount(BadgeGlobals.BadgeTypes.TripleDip);

            Texture2D battleTex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            BattleActions.Add(new MenuAction(User, "Items", new CroppedTexture2D(battleTex, new Rectangle(216, 845, 22, 22)),
                "Select items to use in Battle.", new ItemSubMenu(User, 1, 0)));

            //Add Double Dip
            if (doubleDipCount > 0)
            {
                BattleActions.Add(new MenuAction(User, "Double Dip", new CroppedTexture2D(battleTex, new Rectangle(872, 107, 24, 21)),
                    "Lets you use 2 items in one turn.", 4, new ItemSubMenu(User, 2, 4)));
            }

            //Add Triple Dip
            if (doubleDipCount > 1 || tripleDipCount > 0)
            {
                BattleActions.Add(new MenuAction(User, "Triple Dip", new CroppedTexture2D(battleTex, new Rectangle(872, 137, 24, 21)),
                    "Lets you use 3 items in one turn.", 8, new ItemSubMenu(User, 3, 8)));
            }
        }
    }
}
