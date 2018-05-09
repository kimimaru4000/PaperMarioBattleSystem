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
    /// The Balloon for Sky Guys. When defeated, it pops, turning the Sky Guy into a normal Shy Guy.
    /// </summary>
    public sealed class SkyGuyBalloon : BattleEnemy
    {
        public SkyGuyBalloon(float layer) : base(new Stats(0, 1, 0, 0, 0))
        {
            Name = "Balloon";
            Layer = layer;

            ChangeHeightState(Enumerations.HeightStates.Airborne);

            //The Balloon is immune to every Status Effect
            Enumerations.StatusTypes[] statusTypes = UtilityGlobals.GetEnumValues<Enumerations.StatusTypes>();
            for (int i = 0; i < statusTypes.Length; i++)
            {
                EntityProperties.AddStatusProperty(statusTypes[i], new StatusPropertyHolder(0d, 0, 1));
            }

            LoadAnimations();
        }

        public override void LoadAnimations()
        {
            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/SkyGuy.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new LoopAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(117, 256, 34, 35), 200d),
                new Animation.Frame(new Rectangle(163, 263, 38, 37), 200d)));
            AnimManager.AddAnimationChildFrames(AnimationGlobals.IdleName,
                new Animation.Frame(new Rectangle(13, 259, 2, 31), 200d, new Vector2(0, 18), -.0001f),
                new Animation.Frame(new Rectangle(13, 259, 2, 31), 200d, new Vector2(0, 19), -.0001f));
        }
    }
}
