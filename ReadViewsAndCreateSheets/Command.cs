using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadViewsAndCreateSheets
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand

    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                XYZ placement = null;
                int i = 1;
                List<ViewsElevations> viewsElevationsList = new List<ViewsElevations>();
                var Views = new FilteredElementCollector(doc).OfClass(typeof(View)).WhereElementIsNotElementType().Cast<View>().Where(x => x.ViewType.Equals(ViewType.FloorPlan) && x.CanBePrinted.Equals(true)).ToList();
                foreach (var view in Views)
                {
                    var elev = view.GenLevel.Elevation;
                    var viewName = view.Name;
                    ViewsElevations viewsEle = new ViewsElevations(elev, viewName,view);
                    viewsElevationsList.Add(viewsEle);
                }
                var sheet = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).WhereElementIsNotElementType().Cast<ViewSheet>().FirstOrDefault();
                uidoc.RequestViewChange(sheet);
                placement = uidoc.Selection.PickPoint("Please Select Placement Point");
                var titleBlock = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks).WhereElementIsElementType().FirstOrDefault();
                var orderedList = viewsElevationsList.OrderBy(e => e.Elevation).ToList();
                Transaction tr = new Transaction(doc, "Create sheet and place view");
                foreach (var viewEle in orderedList)
                {
                    tr.Start();
                    var mysheet = ViewSheet.Create(doc, titleBlock.Id);
                    tr.Commit();
                    if (sheet != null)
                    {
                        tr.Start();
                        mysheet.Name = viewEle.ViewName;
                        mysheet.SheetNumber = $"A{i + 100}";
                       
                        tr.Commit() ;
                        i++;
                    }
                   
                   
                    tr.Start();
                    Viewport.Create(doc, mysheet.Id, viewEle.MyView.Id, placement);

                    tr.Commit() ;

                }
                
            
            
                return Result.Succeeded;
            }
            catch (Exception ex)
            {

               TaskDialog.Show("Warning",ex.Message);
                return Result.Failed;
            }
        }

    }
}
