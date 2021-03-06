using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.Routing;
using Demos.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.AuditTrail;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Internal;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using MainDemo.Module;
using MainDemo.Module.BusinessObjects;

namespace MainDemo.Web {
    public class Global : System.Web.HttpApplication {
        public Global() {
#if DEBUG
            DevExpress.EasyTest.Framework.EasyTestTracer.Tracer.SetTraceLevel(System.Diagnostics.TraceLevel.Verbose);
#endif
            InitializeComponent();
        }
        protected void Application_Start(object sender, EventArgs e) {
            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.Latest;
            AboutInfo.Instance.Copyright = AssemblyInfo.AssemblyCopyright + " All Rights Reserved";
            RouteTable.Routes.RegisterXafRoutes();
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
#if DEBUG
            TestScriptsManager.EasyTestEnabled = true;
#endif
        }
        protected void Session_Start(object sender, EventArgs e) {
            Tracing.Initialize();
            WebApplication.SetInstance(Session, new MainDemoWebApplication());
            AuditTrailService.Instance.CustomizeAuditTrailSettings += new CustomizeAuditSettingsEventHandler(Instance_CustomizeAuditTrailSettings);
            WebApplication webApplication = WebApplication.Instance;
            webApplication.GetSecurityStrategy().RegisterXPOAdapterProviders();
            webApplication.LastLogonParametersReading += new EventHandler<LastLogonParametersReadingEventArgs>(Instance_LastLogonParametersReading);
            webApplication.CustomizeFormattingCulture += new EventHandler<CustomizeFormattingCultureEventArgs>(Instance_CustomizeFormattingCulture);
            SetConnectionString(webApplication);
            if(TestScriptsManager.EasyTestEnabled) {
                string connectionString = HttpContext.Current.Request.QueryString["connectionString"];
                if(!string.IsNullOrEmpty(connectionString)) {
                    webApplication.ConnectionString = connectionString;
                }
            }
            DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderControllerBase.ScriptRecorderEnabled = true;

            if(System.Diagnostics.Debugger.IsAttached && WebApplication.Instance.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
                WebApplication.Instance.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }
            WebApplication.Instance.SwitchToNewStyle();
            webApplication.Setup();
            webApplication.Start();
        }
        private void SetConnectionString(WebApplication webApplication) {
            if(MainDemoModule.IsSiteMode) {
                webApplication.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
            }
            else {
                ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
                if(connectionStringSettings != null) {
                    webApplication.ConnectionString = connectionStringSettings.ConnectionString;
                }
                if(string.IsNullOrEmpty(webApplication.ConnectionString) && webApplication.Connection == null) {
                    connectionStringSettings = ConfigurationManager.ConnectionStrings["SqlExpressConnectionString"];
                    if(connectionStringSettings != null) {
                        string connectionString = connectionStringSettings.ConnectionString;
                        if(connectionString != InMemoryDataStoreProvider.ConnectionString) {
                            connectionString = DemoDbEngineDetectorHelper.PatchSQLConnectionString(connectionString);
                            if(connectionString == DemoDbEngineDetectorHelper.AlternativeConnectionString) {
                                connectionString = InMemoryDataStoreProvider.ConnectionString;
                                UseSQLAlternativeInfoSingleton.Instance.FillFields(DemoDbEngineDetectorHelper.SQLServerIsNotFoundMessage, DemoXPODatabaseHelper.AlternativeName, DemoXPODatabaseHelper.InMemoryDatabaseUsageMessage);
                            }
                        }
                        webApplication.ConnectionString = connectionString;
                    }
                }
            }

        }
        private void Instance_CustomizeFormattingCulture(object sender, CustomizeFormattingCultureEventArgs e) {
            e.FormattingCulture = CultureInfo.GetCultureInfo("en-US");
        }
        private void Instance_LastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e) {
            if(string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Sam");
            }
        }
        private void Instance_CustomizeAuditTrailSettings(object sender, CustomizeAuditTrailSettingsEventArgs e) {
            e.AuditTrailSettings.Clear();
            e.AuditTrailSettings.AddType(typeof(Employee), true);
        }
        protected void Application_BeginRequest(object sender, EventArgs e) {
        }
        protected void Application_EndRequest(object sender, EventArgs e) {

        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e) {
        }
        protected void Application_Error(object sender, EventArgs e) {
            ErrorHandling.Instance.ProcessApplicationError();
        }
        protected void Session_End(object sender, EventArgs e) {
            WebApplication.DisposeInstance(Session);
        }
        protected void Application_End(object sender, EventArgs e) {
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}
