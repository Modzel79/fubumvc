using System.Collections.Generic;
using System.Linq;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.LayoutAttachment
{
    [TestFixture]
    public class Use_layout_from_bottle_when_not_in_application : ViewIntegrationContext
    {
        public Use_layout_from_bottle_when_not_in_application()
        {
            

            RazorView<ViewModel1>("View1");
            RazorView<ViewModel2>("Folder1/View2");
            RazorView<ViewModel3>("Folder1/Folder2/View3");
            RazorView<ViewModel4>("Folder1/Folder2/View4");

            InBottle("BottleA");
            RazorView("Shared/Application").Write("Some content");
        }


        [Test]
        public void all_views_have_the_main_application_master()
        {

            var master = Views.Templates<RazorTemplate>().FirstOrDefault(x => x.Name() == "Application");
            master.ShouldNotBeNull();
            master.Origin.ShouldEqual("BottleA");

            Views.Templates<RazorTemplate>().Where(x => x != master).Each(view =>
            {
                view.Master.ShouldBeTheSameAs(master);
            });
        }
    }
}