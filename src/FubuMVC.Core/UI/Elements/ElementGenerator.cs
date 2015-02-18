using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Forms;
using HtmlTags;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI.Elements
{
    public class ElementGenerator<T> : IElementGenerator<T> where T : class
    {
        private readonly ITagGenerator<ElementRequest> _tags;
        private Lazy<T> _model;

        public ElementGenerator(ITagGeneratorFactory factory, IFubuRequest request) : this(factory.GeneratorFor<ElementRequest>(), request)
        {
        }

        private ElementGenerator(ITagGenerator<ElementRequest> tags, IFubuRequest request)
        {
            _tags = tags;
            _model = new Lazy<T>(() => {
                return request.Get<T>();
            });
        }

        /// <summary>
        /// Probably only useful for testing
        /// </summary>
        /// <param name="library"></param>
        /// <param name="activators"></param>
        /// <returns></returns>
        public static ElementGenerator<T> For(HtmlConventionLibrary library, IEnumerable<ITagRequestActivator> activators = null)
        {
            var tags = new TagGenerator<ElementRequest>(library.For<ElementRequest>(),
                                                        new TagRequestBuilder(activators ?? new ITagRequestActivator[0]), new ActiveProfile());

            return new ElementGenerator<T>(tags, new InMemoryFubuRequest());
        } 

        public HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile = null, T model = null)
        {
            return build(expression, ElementConstants.Label, profile, model);
        }

        public HtmlTag InputFor(Expression<Func<T, object>> expression, string profile = null, T model = null)
        {
            return build(expression, ElementConstants.Editor, profile, model);
        }

        public HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile = null, T model = null)
        {
            return build(expression, ElementConstants.Display, profile, model);
        }

        public T Model
        {
            get { return _model.Value; }
            set { _model = new Lazy<T>(() => value); }
        }

        public ElementRequest GetRequest(Expression<Func<T, object>> expression, T model = null)
        {
            return new ElementRequest(expression.ToAccessor())
            {
                Model = model ?? Model
            };
        }

        private HtmlTag build(Expression<Func<T, object>> expression, string category, string profile = null, T model = null)
        {
            ElementRequest request = GetRequest(expression, model);
            return _tags.Build(request, category, profile);
        }

        private HtmlTag build(ElementRequest request, string category, string profile = null, T model = null)
        {
            request.Model = model ?? Model;
            return _tags.Build(request, category, profile: profile);
        }

        // Below methods are tested through the IFubuPage.Show/Edit method tests
        public HtmlTag LabelFor(ElementRequest request, string profile = null, T model = null)
        {
            return build(request, ElementConstants.Label, profile, model);
        }

        public HtmlTag InputFor(ElementRequest request, string profile = null, T model = null)
        {
            return build(request, ElementConstants.Editor, profile, model);
        }

        public HtmlTag DisplayFor(ElementRequest request, string profile = null, T model = null)
        {
            return build(request, ElementConstants.Display, profile, model);
        }
    }
}