using System.Collections;
using System.Windows.Forms;

namespace StatlookLogViewer.Controller
{
    public class IntegerComparer : IComparer
    {
        private readonly int _colIndex;

        public IntegerComparer(int colIndex)
        {
            _colIndex = colIndex;
        }

        public int Compare(object x, object y)
        {
            var nx = int.Parse((x as ListViewItem)?.SubItems[_colIndex].Text ?? string.Empty);
            var ny = int.Parse((y as ListViewItem)?.SubItems[_colIndex].Text ?? string.Empty);
            return nx.CompareTo(ny);
        }
    }
}
