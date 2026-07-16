using System.Drawing;
using System.Windows.Forms;

namespace DachaMS
{
    public partial class BasePanel : UserControl
    {
        protected BasePanel()
        {
            BackColor = AppBranding.ContentBackgroundColor;
        }

        public virtual string PanelTitle
        {
            get { return string.Empty; }
        }

        protected static void UpdateFlowScroll(FlowLayoutPanel flow)
        {
            if (flow.Controls.Count == 0)
            {
                flow.AutoScroll = false;
                return;
            }

            Size preferred = flow.GetPreferredSize(new Size(flow.ClientSize.Width, 0));
            flow.AutoScroll = preferred.Height > flow.ClientSize.Height;
        }
    }
}
