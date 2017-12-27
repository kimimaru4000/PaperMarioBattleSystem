using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for BattleObjects, or any non-BattleEntity.
    /// </summary>
    public abstract class BattleObject : IUpdateable, IDrawable, ICleanup
    {
        /// <summary>
        /// Whether the BattleObject is finished with its task and is ready to be removed or not.
        /// <para>This is most useful for BattleObjects that handle themselves. However, BattleObjects are not required to use this.</para>
        /// </summary>
        public bool ReadyForRemoval = false;

        protected BattleObject()
        {

        }

        public virtual void CleanUp()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Draw()
        {

        }
    }
}
