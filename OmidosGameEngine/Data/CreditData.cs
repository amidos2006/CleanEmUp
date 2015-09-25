using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmidosGameEngine.Data
{
    public class CreditData
    {
        public string Title
        {
            set;
            get;
        }

        public List<string> Names
        {
            set;
            get;
        }

        public CreditData()
        {
            Names = new List<string>();
        }
    }
}
