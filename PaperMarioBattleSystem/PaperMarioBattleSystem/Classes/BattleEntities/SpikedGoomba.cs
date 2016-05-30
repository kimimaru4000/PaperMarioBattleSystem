using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class SpikedGoomba : Goomba
    {
        public SpikedGoomba()
        {
            Name = "Spiked Goomba";
            BattleStats = new Stats(1, 2, 0, 2, 0);

            AddPhysAttribute(Enumerations.PhysicalAttributes.Spiked);

            //TESTING
            //AddPhysAttribute(Enumerations.PhysicalAttributes.Burning);
            //AddPhysAttribute(Enumerations.PhysicalAttributes.Flying);
            //AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
            //
            //Debug.Log($"DAMAGE MODIFIER FOR SHARP IS " + BattleGlobals.GetDamageModifier(Enumerations.Elements.Fire, this));

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/SpikedGoomba");
            AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(34, 153, 28, 39), 1000d)));
        }
    }
}
