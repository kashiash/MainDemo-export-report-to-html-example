using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using MainDemo.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainDemo.Module.Controllers
{
    public class SuperDuperController : ViewController
    {
        private SimpleAction prepareEmailAction;
        public SuperDuperController()
        {
            TargetObjectType = typeof(ReportDataV2);

            prepareEmailAction = new SimpleAction(this, $"{GetType().FullName}.{nameof(prepareEmailAction)}", PredefinedCategory.Reports);
            prepareEmailAction.Caption = "Fill text ine employees";
            prepareEmailAction.ImageName = "MailMerge";
            prepareEmailAction.Execute += prepareTextAction_Execute;

        }

        private void prepareTextAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var wybranySzablon = View.CurrentObject as ReportDataV2;
            IObjectSpace objectSpace = this.Application.CreateObjectSpace();
            if (wybranySzablon != null)
            {

                IReportDataV2 reportData = objectSpace.GetObject<ReportDataV2>(wybranySzablon);

                if (reportData != null)
                {


                    var collection = objectSpace.GetObjects(typeof(Employee));

                    if (collection != null)
                    {
                        int lk = 0;
      
                        foreach (var rec in collection)
                        {
                            var dataSource = new object[] { rec };
                            var htmlStream = GetHtmlBody(reportData, dataSource);
                            var employee = rec as Employee ;
                            employee.Text = Encoding.UTF8.GetString(htmlStream.ToArray());
                            lk++;

                        }
                    }
                }

            }
            objectSpace.CommitChanges();
        }

        private MemoryStream GetHtmlBody(IReportDataV2 reportData, object[] dataSource)
        {

            XtraReport report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
            report.DataSource = dataSource;
            MemoryStream htmlStream = new MemoryStream();

            var options = report.ExportOptions.Html;

            options.EmbedImagesInHTML = true;
            options.UseHRefHyperlinks = true;
            options.AllowURLsWithJSContent = true;
            options.CharacterSet = "UTF-8";
            options.RemoveSecondarySymbols = false;

            options.ExportMode = HtmlExportMode.SingleFilePageByPage;
            options.PageBorderColor = Color.Blue;
            options.PageBorderWidth = 3;
            report.ExportToHtml(htmlStream, options);
            return htmlStream;
        }
    }
}
