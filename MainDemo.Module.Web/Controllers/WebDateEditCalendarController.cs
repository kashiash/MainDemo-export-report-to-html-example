using System;
using DevExpress.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using MainDemo.Module.BusinessObjects;

namespace MainDemo.Module.Web.Controllers {
    public class WebDateEditCalendarController : ObjectViewController<DetailView, Employee> {
        protected override void OnActivated() {
            base.OnActivated();
            View.CustomizeViewItemControl(this, SetCalendarView, nameof(Employee.Birthday));
        }
        private void SetCalendarView(ViewItem viewItem) {
            var propertyEditor = viewItem as ASPxDateTimePropertyEditor;

            if(propertyEditor != null && propertyEditor.ViewEditMode == ViewEditMode.Edit) {
                ASPxDateEdit dateEdit = propertyEditor.Editor;
                dateEdit.PickerDisplayMode = DatePickerDisplayMode.ScrollPicker;
            }
        }
    }
}
