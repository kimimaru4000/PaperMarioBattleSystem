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
    /// The Sky Guy enemy. It's airborne, but if you pop its balloon, it falls to the ground and turns into a normal Shy Guy.
    /// </summary>
    public class SkyGuy : ShyGuy, ITattleableEntity, IWingedEntity
    {
        public IWingedBehavior WingedBehavior { get; private set; } = null;

        public SkyGuyBalloon Balloon { get; private set; } = null;

        public SkyGuy()
        {
            Name = "Sky Guy";

            AIBehavior = new DefaultEnemyAI(this);
            WingedBehavior = new SkyGuyWingedBehavior(this);

            ChangeHeightState(Enumerations.HeightStates.Airborne);

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(70d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Stop, new StatusPropertyHolder(90d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(90d, 1));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(80d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Paralyzed, new StatusPropertyHolder(90d, 1));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(75d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Lifted, new StatusPropertyHolder(90d, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(90d, 0));

            LoadAnimations();
        }

        public override void CleanUp()
        {
            base.CleanUp();

            WingedBehavior?.CleanUp();

            if (Balloon != null)
            {
                Balloon.HealthStateChangedEvent -= OnBalloonHealthStateChanged;

                //Kill off the balloon if it's not dead
                if (Balloon.IsDead == false)
                    Balloon.Die();

                Balloon = null;
            }
        }

        public override void LoadAnimations()
        {
            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/SkyGuy.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new ReverseAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(52, 3, 39, 33), 200d, new Vector2(0, 1)),
                new Animation.Frame(new Rectangle(3, 2, 40, 31), 200d),
                new Animation.Frame(new Rectangle(98, 1, 43, 30), 200d, new Vector2(0, -1))));
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();

            //Add the Balloon to battle
            Balloon = new SkyGuyBalloon(Layer - .0001f);
            BManager.AddEntity(Balloon, null, true);

            Balloon.Position = Position + new Vector2(2, -40);
            Balloon.SetBattlePosition(Balloon.Position);

            //Mark that the Balloon is a helper for the Sky Guy
            Balloon.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.HelperEntity, this);

            //Subscribe to see when the Balloon's dies
            Balloon.HealthStateChangedEvent -= OnBalloonHealthStateChanged;
            Balloon.HealthStateChangedEvent += OnBalloonHealthStateChanged;
        }

        #region Tattle Data

        public new bool CanBeTattled { get; set; } = true;

        public new string[] GetTattleLogEntry()
        {
            return null;
        }

        public new string GetTattleDescription()
        {
            if (WingedBehavior.Grounded == true) return base.GetTattleDescription();

            return "This is a Sky Guy.\nSky Guys are master balloonists.\n<k><p>" +
                   "Max HP: 7, Attack Power: 3,\nDefense Power: 0\n<k><p>" +
                   "We can attack the balloon, too.\n<wait value=\"250\">Once they fall, they're just\nlike normal Shy Guys.\n<k><p>" +
                   "I wish I had a balloon so I could\nfloat... That looks awesome!<k>";
        }

        #endregion

        private void OnBalloonHealthStateChanged(Enumerations.HealthStates newHealthState)
        {
            //If the Balloon dies, unsubscribe from the event and ground the Sky Guy
            if (newHealthState == Enumerations.HealthStates.Dead)
            {
                Balloon.HealthStateChangedEvent -= OnBalloonHealthStateChanged;

                //If the Sky Guy is already dead, don't handle grounding it
                if (IsDead == false)
                {
                    WingedBehavior.HandleGrounded();
                }

                //Set to null since we don't need the Balloon anymore
                Balloon = null;
            }
        }
    }
}
