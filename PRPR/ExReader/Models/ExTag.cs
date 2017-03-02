using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.Models
{
    public class ExTag
    {

        public ExTag(string fullName)
        {
            FullName = fullName;
        }



        public string FullName { get; private set; }

        public string Name
        {
            get
            {
                var split = FullName.IndexOf(':');
                if (split == -1)
                {
                    return FullName;
                }
                else
                {
                    return FullName.Substring(split + 1);
                }
            }
        }

        public string Slave
        {
            get
            {
                var split = FullName.IndexOf(':');
                if (split == -1)
                {
                    return "";
                }
                else
                {
                    return FullName.Substring(0, split);
                }
            }
        }


        public ExTagType Type
        {
            get
            {
                if (Slave.CompareTo("") == 0)
                {
                    return ExTagType.None;
                }
                if (Slave.CompareTo("language") == 0)
                {
                    return ExTagType.Language;
                }
                if (Slave.CompareTo("artist") == 0)
                {
                    return ExTagType.Artist;
                }
                if (Slave.CompareTo("male") == 0)
                {
                    return ExTagType.Male;
                }
                if (Slave.CompareTo("female") == 0)
                {
                    return ExTagType.Female;
                }
                if (Slave.CompareTo("character") == 0)
                {
                    return ExTagType.Character;
                }
                if (Slave.CompareTo("parody") == 0)
                {
                    return ExTagType.Parody;
                }
                if (Slave.CompareTo("group") == 0)
                {
                    return ExTagType.Group;
                }
                if (Slave.CompareTo("reclass") == 0)
                {
                    return ExTagType.None;
                }
                else
                {
                    return ExTagType.None;
                }
            }
        }

        public override string ToString()
        {
            return FullName;
        }
        

        public string GetQueryName()
        {
            return $"{Slave}:\"{Name}$\"";
        }
    }



    public enum ExTagType
    {
        None = 0,
        Language,
        Artist,
        Male,
        Female,
        Character,
        Parody,
        Group
    }

}
