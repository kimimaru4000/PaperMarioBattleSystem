using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for objects that can be spun for a unique effect.
    /// <para>The only known example is Kent C. Koopa, who drops 6 coins when hitting his tail with Spin Smash.
    /// As Super Hammer and Ultra Hammer are TTYD's versions of Spin Smash, they would also likely spin him out.</para>
    /// </summary>
    public interface ISpinnableBehavior
    {
        /// <summary>
        /// The number of times the object was spun.
        /// </summary>
        int TimesSpun { get; }

        /// <summary>
        /// What happens when the object is spun.
        /// </summary>
        void HandleSpinOut();
    }
}
