using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Life Shroom. It revives a BattleEntity that dies, healing 10 HP.
    /// </summary>
    public class LifeShroom : Mushroom, IRevivalItem
    {
        public int RevivalHPRestored { get; protected set; }

        public LifeShroom()
        {
            Name = "Life Shroom";
            Description = "Restores 10 HP when Mario or his partner falls.";

            Icon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Items.png"),
                new Rectangle(563, 7, 25, 23));
            HPRestored = 10;
            RevivalHPRestored = 10;

            ItemType |= ItemTypes.Revival;
        }
    }
}
