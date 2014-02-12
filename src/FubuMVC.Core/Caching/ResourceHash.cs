using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public interface IResourceHash
    {
        string CreateHash();
        IDictionary<string, string> Describe();
    }

    public class ResourceHash : IResourceHash
    {
        private readonly IEnumerable<IVaryBy> _varyBys;

        public ResourceHash(IEnumerable<IVaryBy> varyBys)
        {
            _varyBys = varyBys;
        }

        public static string For(params IVaryBy[] varyBys)
        {
            return new ResourceHash(varyBys).CreateHash();
        }

        public static string For(ICurrentChain chain)
        {
            return For(new VaryByResource(chain));
        }

        public static string For(Registration.Nodes.BehaviorChain getBlobChain, object request)
        {
            var varyByResource = new SimpleVaryBy();
            varyByResource.With("chain", getBlobChain.UniqueId.ToString());
            getBlobChain.Route.Input.RouteParameters.Each(x => varyByResource.With(x.Name, Convert.ToString(x.GetRawValue(request))));
            return For(varyByResource);
        }

        public string CreateHash()
        {
            return Describe().Select(x => "{0}={1}".ToFormat(x.Key, x.Value)).Join("&").ToHash();
        }

        public IDictionary<string, string> Describe()
        {
            var dictionary = new Dictionary<string, string>();

            _varyBys.Each(x => x.Apply(dictionary));

            return dictionary;
        }

    }
}