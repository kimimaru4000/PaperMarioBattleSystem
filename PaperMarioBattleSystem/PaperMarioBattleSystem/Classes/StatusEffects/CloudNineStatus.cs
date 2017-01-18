using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Cloud Nine Status Effect.
    /// The entity's Evasion is raised by a certain amount until it ends.
    /// <para>This Status Effect is inflicted with Lakilester's Cloud Nine move.</para>
    /// </summary>
    public sealed class CloudNineStatus : DodgyStatus
    {
        public CloudNineStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.CloudNine;

            //NOTE: I'm unsure of the exact value, but it seems less than 50%
            //I'll update this when I know for sure
            EvasionValue = 25;

            AfflictedMessage = "Chances of being attacked will decrease!";
            RemovedMessage = "The effect of Cloud Nine have worn off!";
        }

        public override StatusEffect Copy()
        {
            return new CloudNineStatus(Duration);
        }
    }
}
