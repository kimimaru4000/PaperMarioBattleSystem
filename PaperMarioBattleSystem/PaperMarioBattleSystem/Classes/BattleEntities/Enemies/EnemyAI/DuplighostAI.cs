using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Duplighost enemy AI.
    /// </summary>
    public sealed class DuplighostAI : EnemyAIBehavior
    {
        private Duplighost duplighost = null;

        public DuplighostAI(Duplighost dupliGhost) : base(dupliGhost)
        {
            duplighost = dupliGhost;
        }

        public override void PerformAction()
        {
            //If it's flipped, don't do anything
            if (duplighost.FlippableBehavior != null && duplighost.FlippableBehavior.Flipped == true)
            {
                duplighost.StartAction(new NoAction(), true, null);
                return;
            }

            if (duplighost.PartnerTypeDisguise == PartnerTypes.None)
            {
                //For testing, say that it's a 50% chance of disguising and headbutting
                int randVal = GeneralGlobals.Randomizer.Next(0, 2);

                //Ensure there's a Partner to copy
                bool partnerExists = (BattleManager.Instance.GetPartner() != null);

                if (randVal == 0 && partnerExists == true)
                {
                    duplighost.StartAction(new DisguiseAction(), false, BattleManager.Instance.GetPartner().GetTrueTarget());
                }
                else
                {
                    duplighost.StartAction(new HeadbuttAction(), false, BattleManager.Instance.GetFrontPlayer().GetTrueTarget());
                }
            }
            else
            {
                if (duplighost.PartnerTypeDisguise == PartnerTypes.Goombario)
                {
                    int rand = GeneralGlobals.Randomizer.Next(0, 2);

                    if (rand == 0)
                        duplighost.StartAction(new Bonk(), false, BattleManager.Instance.GetFrontPlayer().GetTrueTarget());
                    else
                        duplighost.StartAction(new Tattle(false), false, BattleManager.Instance.GetMario());
                }
                else if (duplighost.PartnerTypeDisguise == PartnerTypes.Kooper)
                {
                    duplighost.StartAction(new ShellToss(), false, BattleManager.Instance.GetFrontPlayer().GetTrueTarget());
                }
                else if (duplighost.PartnerTypeDisguise == PartnerTypes.Watt)
                {
                    duplighost.StartAction(new ElectroDashAction(), false, BattleManager.Instance.GetFrontPlayer().GetTrueTarget());
                }
                else
                {
                    duplighost.StartAction(new HeadbuttAction(), false, BattleManager.Instance.GetFrontPlayer().GetTrueTarget());
                }
            }

            //Duplighosts Auto-complete Action Commands
            //NOTE: With the current setup, they won't perform Action Commands since they're disabled for enemies by default
            //Make it more flexible to enable or disable them
            if (duplighost.PreviousAction != null && duplighost.PreviousAction.actionCommand != null)
                duplighost.PreviousAction.actionCommand.AutoComplete = true;
        }
    }
}
