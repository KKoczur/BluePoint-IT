using System.Windows.Forms;
using System.Drawing;

namespace StatlookLogViewer.Views
{
    public delegate bool PreRemoveTab(int indx);
    public class TabControlEx : TabControl
    {
        public TabControlEx()
            : base()
        {
            PreRemoveTabPage = null;
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
        }

        public PreRemoveTab PreRemoveTabPage;

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index == 0)
            {
                Rectangle r = e.Bounds;
                r = GetTabRect(e.Index);
                r.Offset(2, 4);
                r.Width = 16;
                r.Height = 16;
                Brush b = new SolidBrush(Color.Blue);
                Font f = new(this.Font, FontStyle.Bold);
                e.Graphics.DrawImage(StatlookLogViewer.Properties.Resources.info_16, r);
                string titel = this.TabPages[e.Index].Text.Substring(6, TabPages[e.Index].Text.Length - 6);
                e.Graphics.DrawString(titel, f, b, new PointF(r.X + 16, r.Y));
            }
            else
            {
                Rectangle r = e.Bounds;
                r = GetTabRect(e.Index);
                r.Offset(2, 4);
                r.Width = 16;
                r.Height = 16;
                Brush b1 = new SolidBrush(Color.Red);
                Brush b2 = new SolidBrush(Color.Black);
                Pen p = new Pen(b1);
                e.Graphics.DrawImage(StatlookLogViewer.Properties.Resources.close_16, r);
                //e.Graphics.DrawLine(p, r.X, r.Y, r.X + r.Width, r.Y + r.Height);
                //e.Graphics.DrawLine(p, r.X + r.Width, r.Y, r.X, r.Y + r.Height);

                string titel = this.TabPages[e.Index].Text.Substring(6, TabPages[e.Index].Text.Length - 6);
                Font f = this.Font;
                e.Graphics.DrawString(titel, f, b2, new PointF(r.X + 16, r.Y));
            }
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            Point p = e.Location;
            for (int i = 1; i < TabCount; i++)
            {
                Rectangle r = GetTabRect(i);
                r.Offset(2, 4);
                r.Width = 16;
                r.Height = 16;
                if (r.Contains(p))
                {
                    CloseTab(i);
                }
            }
        }

        private void CloseTab(int i)
        {
            int PageNumber = SelectedIndex;
            if (PreRemoveTabPage != null)
            {
                bool closeIt = PreRemoveTabPage(i);
                if (!closeIt)
                    return;
            }
            TabPages.Remove(TabPages[i]);
            if (TabPages.Count > 1)
            {
                if (PageNumber == TabPages.Count)
                {
                    SelectedIndex = PageNumber - 1;
                }
                else
                {
                    SelectedIndex = PageNumber;
                }
            }
        }
    }
}