using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erbsenzaehler.SummaryMail
{
    public class SummaryMailSender
    {
        private readonly SummaryMailRenderer _renderer;


        public SummaryMailSender(SummaryMailRenderer renderer)
        {
            _renderer = renderer;
        }
    }
}
