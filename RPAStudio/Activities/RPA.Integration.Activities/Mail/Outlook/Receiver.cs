using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.Integration.Activities.Mail
{
    public struct Receiver
    {
        private const char addressSeparator = ';';

        public List<string> To
        {
            get;
            set;
        }

        public List<string> CC
        {
            get;
            set;
        }

        public List<string> BCC
        {
            get;
            set;
        }

        public Receiver(string to, string cc, string bcc)
        {
            this = default(Receiver);
            To = new List<string>();
            if (!string.IsNullOrWhiteSpace(to))
            {
                To = to.Split(';').ToList();
            }
            CC = new List<string>();
            if (!string.IsNullOrWhiteSpace(cc))
            {
                CC = cc.Split(';').ToList();
            }
            BCC = new List<string>();
            if (!string.IsNullOrWhiteSpace(bcc))
            {
                BCC = bcc.Split(';').ToList();
            }
        }
    }

}
