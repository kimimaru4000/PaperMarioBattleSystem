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
    /// A Pokey enemy. It has 3 segments.
    /// </summary>
    public class Pokey : BattleEnemy, ISegmentEntity
    {
        public Pokey(Stats stats) : base(new Stats(11, 4, 0, 2, 0))
        {
            Name = "Pokey";

            #region Entity Property Setup

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(95, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(80, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Confused, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Immobilized, new StatusPropertyHolder(80, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Soft, new StatusPropertyHolder(95, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Burn, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Frozen, new StatusPropertyHolder(60, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.KO, new StatusPropertyHolder(100, 0));

            EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.TopSpiked);
            EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.SideSpiked);

            EntityProperties.SetVulnerableDamageEffects(Enumerations.DamageEffects.RemovesSegment);

            #endregion

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Pokey");
            AnimManager.SetSpriteSheet(spriteSheet);

            //Set segment count
            CurSegmentCount = MaxSegments;
        }

        #region Interface Implementation

        public uint MaxSegments { get; protected set; } = 3;

        public uint CurSegmentCount { get; protected set; } = 0;

        public void HandleSegmentRemoved(uint segmentsRemoved)
        {

        }

        public void HandleSegmentAdded(uint segmentsAdded)
        {
            
        }

        #endregion
    }
}
