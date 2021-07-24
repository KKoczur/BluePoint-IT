using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UplookLogViewer
{
    public class IntegerComparer : IComparer
    {
        private int _colIndex;
        public IntegerComparer(int colIndex)
        {
            _colIndex = colIndex;
        }
        public int Compare(object x, object y)
        {
            int nx = int.Parse((x as ListViewItem).SubItems[_colIndex].Text);
            int ny = int.Parse((y as ListViewItem).SubItems[_colIndex].Text);
            return nx.CompareTo(ny);
        }
    }
}
