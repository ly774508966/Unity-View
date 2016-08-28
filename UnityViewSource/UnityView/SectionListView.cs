using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityView
{
    public class SectionListView : ListView
    {
        public interface ISectionListListener
        {
            void OnSectionSelected(int section);
            void OnSectionChanged(int section);
        }
    }
}
