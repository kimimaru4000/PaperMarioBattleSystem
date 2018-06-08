using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Class for global helpers for dealing with BattleEntities.
    /// </summary>
    public static class EntityGlobals
    {
        /// <summary>
        /// Gets the number of Badges of a particular BadgeType that a set of BattleEntities have equipped.
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <param name="badgeType">The BadgeType to check for.</param>
        /// <returns>The total number of Badges of the BadgeType that a set of BattleEntities have equipped.</returns>
        public static int GetCombinedEquippedBadgeCount(IList<BattleEntity> party, BadgeGlobals.BadgeTypes badgeType)
        {
            if (party == null || party.Count == 0) return 0;

            int count = 0;

            //Check the Badge count for each party
            for (int i = 0; i < party.Count; i++)
            {
                count += party[i].GetEquippedBadgeCount(badgeType);
            }

            return count;
        }

        /// <summary>
        /// Gets the number of Badges of a particular BadgeType, for both its Partner and Non-Partner versions, that a set of BattleEntities have equipped.
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <param name="badgeType">The BadgeType to check for.</param>
        /// <returns>The total number of Badges of the Partner and Non-Partner versions of the BadgeType that a set of BattleEntities have equipped.</returns>
        public static int GetCombinedEquippedNPBadgeCount(IList<BattleEntity> party, BadgeGlobals.BadgeTypes badgeType)
        {
            if (party == null || party.Count == 0) return 0;

            BadgeGlobals.BadgeTypes? npBadgeType = BadgeGlobals.GetNonPartnerBadgeType(badgeType);
            BadgeGlobals.BadgeTypes? pBadgeType = BadgeGlobals.GetPartnerBadgeType(badgeType);

            int count = 0;
            if (npBadgeType != null) count += GetCombinedEquippedBadgeCount(party, npBadgeType.Value);
            if (pBadgeType != null) count += GetCombinedEquippedBadgeCount(party, pBadgeType.Value);

            return count;
        }

        /// <summary>
        /// Checks if a set of BattleEntities have an AdditionalProperty.
        /// </summary>
        /// <param name="property">The AdditionalProperty to check.</param>
        /// <returns>true if any of the BattleEntities in the set have the AdditionalProperty, otherwise false</returns>
        public static bool CombinedHaveAdditionalProperty(IList<BattleEntity> party, Enumerations.AdditionalProperty property)
        {
            if (party == null || party.Count == 0) return false;

            //Check if anyone in the party has the property
            for (int i = 0; i < party.Count; i++)
            {
                //Return true on the first one we found
                if (party[i].EntityProperties.HasAdditionalProperty(property) == true) return true;
            }

            return false;
        }
    }
}
