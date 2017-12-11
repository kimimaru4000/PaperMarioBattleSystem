using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent that removes an <see cref="IWingedEntity"/>'s wings when it stops being targeted.
    /// </summary>
    public sealed class RemoveWingsBattleEvent : BattleEvent
    {
        private IWingedEntity WingedEntity = null;
        private BattleEntity Entity = null;

        public RemoveWingsBattleEvent(IWingedEntity wingedEntity)
        {
            WingedEntity = wingedEntity;
            Entity = wingedEntity as BattleEntity;
        }

        protected override void OnUpdate()
        {
            //Don't do anything with invalid input
            if (WingedEntity == null || Entity == null)
            {
                Debug.LogError($"A null {nameof(IWingedEntity)} has been passed in or it isn't a {nameof(BattleEntity)}. Fix this");
                End();
                return;
            }

            //If the entity is no longer targeted, remove its wings and end
            if (Entity.IsTargeted == false)
            {
                WingedEntity.RemoveWings();

                End();
            }
        }
    }
}
