using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARC2
{
    internal class InfiniteProgressBarWrapper : ProgressBarWrapper
    {
        public InfiniteProgressBarWrapper(System.Windows.Forms.ProgressBar toolStripProgressBar) : base(toolStripProgressBar)
        {
        }

        public override int Maximum
        {
            get { return 10000; }
            set
            {
                //base.Maximum = value;
            }
        }

        public override void Increment(int increment)
        {
            if (Value + increment > Maximum)
            {
                Value = 0;
            }
            base.Increment(increment);
        }
    }
}
