using StatlookLogViewer.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace StatlookLogViewer.Views;

public delegate bool PreRemoveTab(int indx);

public class TabControlEx : TabControl
{
    public PreRemoveTab PreRemoveTabPage;

    public TabControlEx()
    {
        PreRemoveTabPage = null;
        DrawMode = TabDrawMode.OwnerDrawFixed;
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index == 0)
        {
            var r = e.Bounds;
            r = GetTabRect(e.Index);
            r.Offset(2, 4);
            r.Width = 16;
            r.Height = 16;
            Brush b = new SolidBrush(Color.Blue);
            Font f = new(Font, FontStyle.Bold);
            e.Graphics.DrawImage(Resources.info_16, r);
            var titel = TabPages[e.Index].Text.Substring(6, TabPages[e.Index].Text.Length - 6);
            e.Graphics.DrawString(titel, f, b, new PointF(r.X + 16, r.Y));
        }
        else
        {
            var r = e.Bounds;
            r = GetTabRect(e.Index);
            r.Offset(2, 4);
            r.Width = 16;
            r.Height = 16;
            Brush b1 = new SolidBrush(Color.Red);
            Brush b2 = new SolidBrush(Color.Black);
            var p = new Pen(b1);
            e.Graphics.DrawImage(Resources.close_16, r);
            //e.Graphics.DrawLine(p, r.X, r.Y, r.X + r.Width, r.Y + r.Height);
            //e.Graphics.DrawLine(p, r.X + r.Width, r.Y, r.X, r.Y + r.Height);

            var titel = TabPages[e.Index].Text.Substring(6, TabPages[e.Index].Text.Length - 6);
            var f = Font;
            e.Graphics.DrawString(titel, f, b2, new PointF(r.X + 16, r.Y));
        }
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        var p = e.Location;
        for (var i = 1; i < TabCount; i++)
        {
            var r = GetTabRect(i);
            r.Offset(2, 4);
            r.Width = 16;
            r.Height = 16;
            if (r.Contains(p)) CloseTab(i);
        }
    }

    private void CloseTab(int i)
    {
        var pageNumber = SelectedIndex;
        if (PreRemoveTabPage != null)
        {
            var closeIt = PreRemoveTabPage(i);
            if (!closeIt)
                return;
        }

        TabPages.Remove(TabPages[i]);

        if (TabPages.Count <= 1) return;

        if (pageNumber == TabPages.Count)
            SelectedIndex = pageNumber - 1;
        else
            SelectedIndex = pageNumber;
    }
}