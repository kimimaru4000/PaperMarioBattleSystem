using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The action command for Guard and Superguard. Both are contained in one class because they're both active
    /// at the same time in battle. Superguards take priority over Guards.
    /// <para>This action command is unique in that its timing varies based on the attack used. The result is not
    /// known until the attack hits. As such, this action command's responses aid in determining the result</para>
    /// </summary>
    public sealed class GuardCommand : ActionCommand
    {
        /// <summary>
        /// The timer for Guarding, based on a frame rate of 60 FPS.
        /// This window can be lengthened or shortened based on the Simplifier or Unsimplifier badges respectively
        /// </summary>
        private double GuardTimer = (8d / 60d);

        /// <summary>
        /// The timer for Superguarding, based on a frame rate of 60 FPS.
        /// This window can be lengthened or shortened based on the Simplifier or Unsimplifier badges respectively
        /// </summary>
        private double SuperguardTimer = (3d / 60d);

        private double GuardCooldown = (10d / 60d);
        private double SuperguardCooldown = (10d / 60d);

        private double PrevGuardInputTime = 0f;
        private double PrevGuardCooldown = 0f;
        private double PrevSuperguardInputTime = 0f;
        private double PrevSuperguardCooldown = 0f;

        private Keys GuardButton = Keys.Z;
        private Keys SuperguardButton = Keys.X;

        private Guard GuardAction = null;

        public GuardCommand(Guard guardAction) : base(guardAction)
        {
            GuardAction = guardAction;
        }

        public override void StartInput()
        {
            base.StartInput();

            PrevGuardInputTime = PrevSuperguardInputTime = 0f;
            PrevGuardCooldown = PrevSuperguardCooldown = 0f;
        }

        public override void EndInput()
        {
            base.EndInput();

            PrevGuardInputTime = PrevSuperguardInputTime = 0f;
            PrevGuardCooldown = PrevSuperguardCooldown = 0f;
        }

        protected override void ReadInput()
        {
            if (Input.GetKeyDown(GuardButton) == true && Time.ActiveMilliseconds >= PrevGuardCooldown)
            {
                PrevGuardInputTime = Time.ActiveMilliseconds;
                PrevGuardCooldown = Time.ActiveMilliseconds + GuardCooldown;
            }

            if (Input.GetKeyDown(SuperguardButton) == true && Time.ActiveMilliseconds >= PrevSuperguardCooldown)
            {
                PrevSuperguardInputTime = Time.ActiveMilliseconds;
                PrevSuperguardCooldown = Time.ActiveMilliseconds + SuperguardCooldown;
            }
        }
    }
}
