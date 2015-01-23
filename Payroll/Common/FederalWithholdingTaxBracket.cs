using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common
{
    public class FederalWithholdingTaxBracket
    {
        public int FederalWithholdingTaxBracketID { get; set; }
        public bool IsMarried { get; set; }
        public double MoreThan { get; set; }
        public double? LessThan { get; set; }
        public double AmountToAdd { get; set; }
        public double Percentage { get; set; }
        public double ExcessOver { get; set; }
    }
}
