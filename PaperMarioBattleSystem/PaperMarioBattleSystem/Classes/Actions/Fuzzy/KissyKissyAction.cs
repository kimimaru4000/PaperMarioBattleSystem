using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Kissy-Kissy attack used by Fuzzies, Swoopulas, Blooper Babies and more enemies.
    /// It damages Mario or his Partner, also healing the user for the damage dealt.
    /// Misses and interruptions occur in the sequence when the enemy latches onto its opponent right before the move starts.
    /// <para>This move has a lot of parameters and has an Action Command when being used by an enemy.
    /// The Action Command is performed by the player to end the attack early.</para>
    /// <para>If there is no Action Command, the attack can be guarded, and the move should stop after a number of attacks otherwise it will softlock.</para>
    /// </summary>
    public class KissyKissyAction : MoveAction
    {
        public override bool CommandEnabled => (HasActionCommand == true && DisableActionCommand == false);

        public KissyKissyAction(bool hasActionCommand, int numAttacks, int damagePerAttack, Elements damageElement,
            bool piercing, bool sideDirect, HeightStates[] heightsAffected)
        {
            Name = "Kissy-Kissy";

            //None of the enemies that use this move ever perform a charge; it makes sense to me to make it use up a charge
            MoveInfo = new MoveActionData(null, "Absorb health from your opponent.", MoveResourceTypes.FP, 0, CostDisplayTypes.Shown, MoveAffectionTypes.Enemy,
                TargetSelectionMenu.EntitySelectionType.Single, true, heightsAffected);

            ContactTypes contactType = ContactTypes.SideDirect;
            if (sideDirect == false) contactType = ContactTypes.TopDirect;

            DamageInfo = new DamageData(damagePerAttack, damageElement, piercing, contactType, null, false, false,
                DefensiveMoveOverrides.None, DamageEffects.None);

            SetMoveSequence(new KissyKissySequence(this, numAttacks));

            if (hasActionCommand == true)
            {
                //If Kissy-Kissy has an Action Command, it can't be Guarded or Superguarded
                DamageInfo.DefensiveOverride = DefensiveMoveOverrides.All;

                actionCommand = new MashButtonCommand(MoveSequence, 100d, 10d, MashButtonCommand.InfiniteTime, Microsoft.Xna.Framework.Input.Keys.A);
            }
        }
    }
}
