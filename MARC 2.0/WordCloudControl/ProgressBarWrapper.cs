using Gma.CodeCloud.Controls.TextAnalyses.Extractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARC2
{
    internal class ProgressBarWrapper : IProgressIndicator
    {
        private readonly System.Windows.Forms.ProgressBar m_ProgressBar;

        public ProgressBarWrapper(System.Windows.Forms.ProgressBar toolStripProgressBar)
        {
            m_ProgressBar = toolStripProgressBar;
        }

        public int Value
        {
            get { return m_ProgressBar.Value; }
            set { m_ProgressBar.Value = value; }
        }

        public virtual int Maximum
        {
            get { return m_ProgressBar.Maximum; }
            set { m_ProgressBar.Maximum = value; }
        }

        public virtual void Increment(int value)
        {
            m_ProgressBar.Increment(value);
            //System.Windows.Forms.Application.DoEvents();
        }
    }
}
