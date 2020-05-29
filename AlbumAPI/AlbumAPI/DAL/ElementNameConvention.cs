using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumAPI.DAL
{
    public class ElementNameConvention : CamelCaseElementNameConvention, IMemberMapConvention
    {
        void IMemberMapConvention.Apply(BsonMemberMap memberMap)
        {
            string name = memberMap.MemberName;
            var firstLowercaseIndex = findFirstLowercaseIndex(name);

            if (firstLowercaseIndex < 0)
                memberMap.SetElementName(name.ToLowerInvariant());
            else if (firstLowercaseIndex > 1)
                memberMap.SetElementName(name.Substring(0, firstLowercaseIndex - 1).ToLowerInvariant() + name.Substring(firstLowercaseIndex - 1));
            else
                base.Apply(memberMap);
        }

        private static int findFirstLowercaseIndex(string name)
        {
            for (int i = 0; i < name.Length; i++)
            {
                if (char.IsLower(name, i))
                    return i;
            }

            return -1;
        }
    }
}

