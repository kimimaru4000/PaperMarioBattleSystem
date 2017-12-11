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
    /// The Water Block Status Effect.
    /// The entity's Defense is raised by 1 and it gains a +1 Resistance to Fire moves.
    /// <para>This Status Effect is inflicted with Sushie's Water Block move.</para>
    /// </summary>
    public sealed class WaterBlockStatus : DEFUpStatus
    {
        /// <summary>
        /// The amount Water Block increases the entity's Defense by.
        /// </summary>
        private const int DefenseBoost = 1;

        /// <summary>
        /// The Fire Resistance Water Block grants.
        /// </summary>
        private readonly ResistanceHolder FireResistance = new ResistanceHolder(ResistanceTypes.MinusDamage, 1);

        public WaterBlockStatus(int duration) : base(DefenseBoost, duration)
        {
            StatusType = Enumerations.StatusTypes.WaterBlock;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(461, 386, 28, 28));

            //ArrowRect = new Rectangle(508, 386, 26, 27);

            AfflictedMessage = "Mario will be protected by Water Block for a short time!";
            RemovedMessage = "The Water Block's effect has worn off!";
        }

        protected override void OnAfflict()
        {
            base.OnAfflict();
            EntityAfflicted.EntityProperties.AddResistance(Enumerations.Elements.Fire, FireResistance);
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            EntityAfflicted.EntityProperties.RemoveResistance(Enumerations.Elements.Fire, FireResistance);
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            base.OnSuppress(statusSuppressionType);

            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.EntityProperties.RemoveResistance(Enumerations.Elements.Fire, FireResistance);
            }
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            base.OnUnsuppress(statusSuppressionType);

            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.EntityProperties.AddResistance(Enumerations.Elements.Fire, FireResistance);
            }
        }

        public override StatusEffect Copy()
        {
            return new WaterBlockStatus(Duration);
        }
    }
}
