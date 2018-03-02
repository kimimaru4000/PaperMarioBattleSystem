using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Attack FX F Badge - Changes the sounds of Mario's attacks to the sound of laughing flowers.
    /// <para>This is an unused Badge in PM.</para>
    /// </summary>
    public sealed class AttackFXFBadge : Badge
    {
        public AttackFXFBadge()
        {
            Name = "Attack FX F";
            Description = "Changes the sound effects when Mario's attacking.";

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXF;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {

        }

        protected override void OnUnequip()
        {

        }
    }
}
