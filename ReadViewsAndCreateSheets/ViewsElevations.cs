using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadViewsAndCreateSheets
{
    internal class ViewsElevations
    {
     

        public double Elevation { get; set; }
        public string ViewName { get; set; }
        public View MyView { get; set; }
        public ViewsElevations(double elevation, string viewName,View view)
        {
            Elevation = elevation;
            ViewName = viewName;
            MyView = view;
        }
    }
}
