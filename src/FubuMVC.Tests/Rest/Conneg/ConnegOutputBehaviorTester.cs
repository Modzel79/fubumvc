using System;
using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Rest.Conneg;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Tests.Rest.Projections;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Rest.Conneg
{
    [TestFixture]
    public class ConnegOutputBehaviorTester : InteractionContext<ConnegOutputBehavior<Address>>
    {
        private IMediaWriter<Address> writerFor(params string[] mimeTypes)
        {
            var writer = Services.AddAdditionalMockFor<IMediaWriter<Address>>();
            writer.Stub(x => x.Mimetypes).Return(mimeTypes);


            return writer;
        }

        protected override void beforeEach()
        {
            
        }

        [Test]
        public void select_writer_simple_case()
        {
            var w1 = writerFor("text/json", "application/json");
            var w2 = writerFor("text/xml");
            var w3 = writerFor("text/html");

            ClassUnderTest.SelectWriter(new CurrentMimeType("", "application/json"))
                .ShouldBeTheSameAs(w1);

        }

        [Test]
        public void select_writer_uses_the_accept_types_in_order()
        {
            var w1 = writerFor("text/json", "application/json");
            var w2 = writerFor("text/xml");
            var w3 = writerFor("text/html");

            ClassUnderTest.SelectWriter(new CurrentMimeType("", "text/html,application/json,text/json"))
                .ShouldBeTheSameAs(w3);
        }

        [Test]
        public void select_writer_goes_thru_progression_of_accept_types_until_it_finds_what_it_wants()
        {
            var w1 = writerFor("text/json", "application/json");
            var w2 = writerFor("text/xml");
            var w3 = writerFor("text/html");

            ClassUnderTest.SelectWriter(new CurrentMimeType("", "weird/html,wild/json,text/json"))
                .ShouldBeTheSameAs(w1);
        }

        [Test]
        public void select_writer_goes_thru_progression_of_accept_types_until_the_wild_card_picks_html_if_it_exists()
        {
            var w1 = writerFor("text/json", "application/json");
            var w2 = writerFor("text/xml");
            var w3 = writerFor("text/html");

            ClassUnderTest.SelectWriter(new CurrentMimeType("", "weird/html,wild/json,*/*"))
                .ShouldBeTheSameAs(w3);
        }

        [Test]
        public void select_writer_goes_thru_progression_of_accept_types_until_the_wild_card_picks_first_media_if_no_html_exists()
        {
            var w1 = writerFor("text/json", "application/json");
            var w2 = writerFor("text/xml");

            ClassUnderTest.SelectWriter(new CurrentMimeType("", "weird/html,wild/json,*/*"))
                .ShouldBeTheSameAs(w1);
        }

        [Test]
        public void select_writer_can_return_null_if_no_possible_writers_can_be_found_for_the_accept_types()
        {
            var w1 = writerFor("text/json", "application/json");
            var w2 = writerFor("text/xml");

            ClassUnderTest.SelectWriter(new CurrentMimeType("", "weird/html,wild/json"))
                .ShouldBeNull();
        }
    }
}