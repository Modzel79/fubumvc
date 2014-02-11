using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteAccessorParameter : RouteParameter
    {
        private readonly Accessor _accessor;

        public RouteAccessorParameter(Accessor accessor)
        {
            _accessor = accessor;
            accessor.ForAttribute<RouteInputAttribute>(x => DefaultValue = x.DefaultValue);
        }

        public override string Name { get { return _accessor.Name; } }

        public override object GetRawValue(object input)
        {
            object rawValue = input == null ? DefaultValue : _accessor.GetValue(input);
            return rawValue ?? DefaultValue;
        }

        public override bool HasValue(object input)
        {
            var raw = _accessor.GetValue(input);
            return raw != null;
        }

        public override bool CanTemplate(object inputModel)
        {
            object rawValue = GetRawValue(inputModel);
            if (rawValue != null)
            {
                return _accessor.PropertyType.IsValueType
                           ? !rawValue.Equals(Activator.CreateInstance(_accessor.PropertyType))
                           : true;

            }
            return false;
        }

        public override int GetHashCode()
        {
            return (_accessor != null ? _accessor.GetHashCode() : 0);
        }

        public override string ToString()
        {
            if (DefaultValue != null) return string.Format("Accessor: {0}, DefaultValue: {1}", _accessor, DefaultValue);

            return "Accessor: {0}".ToFormat(_accessor);
        }

        public override bool Equals(RouteParameter other)
        {
            if (ReferenceEquals(null, other)) 
                return false;

            if (other is RouteAccessorParameter)
                return Equals(((RouteAccessorParameter)other)._accessor, this._accessor);
            else
                return false;
        }
    }

    public abstract class RouteParameter : DescribesItself
    {
        // Use the first property to get the name
        // use the full blown accessor to get the value
        private readonly Regex _regex;
        private readonly Regex _regexGreedy;

        public RouteParameter()
        {
            _regex = new Regex(@"{\*?" + Name + @"(?:\:.*?)?}", RegexOptions.Compiled);
            _regexGreedy = new Regex(@"{\*" + Name + @"(?:\:.*?)?}", RegexOptions.Compiled);
        }

        public abstract string Name { get;  }

        public object DefaultValue { get; set; }

        public bool CanSubstitute(object inputModel)
        {
            return GetRawValue(inputModel) != null;
        }

        public string Substitute(object input, string url)
        {
            object rawValue = GetRawValue(input);
            string parameterValue = rawValue.ToString();
            return substitute(url, parameterValue);
        }

        private string substitute(string url, string parameterValue)
        {
            var encodedValue = _regexGreedy.IsMatch(url) ? encodeParameterValue(parameterValue) : parameterValue.UrlEncoded();
            return _regex.Replace(url, encodedValue);
        }

        private static string encodeParameterValue(string parameterValue)
        {
            var values = parameterValue.Split('/');
            return values.Length > 0 ? values.Select(x => x.UrlEncoded()).Join("/") : values.UrlEncoded();
        }

        public string Substitute(RouteParameters parameters, string url)
        {
            return substitute(url, parameters[Name] ?? DefaultValue.ToString());
        }

        public virtual bool CanTemplate(object inputModel)
        {
            object rawValue = GetRawValue(inputModel);

            if (rawValue != null)
                return  true;

            return false;
        }

        public virtual string ToQueryString(object input)
        {
            object rawValue = GetRawValue(input);

            return makeQueryString(rawValue);
        }

        private string makeQueryString(object rawValue)
        {
            if (rawValue == null || string.Empty.Equals(rawValue))
            {
                return Name.UrlEncoded() + "=";
            }

            return Name.UrlEncoded() + "=" + rawValue.ToString().UrlEncoded();
        }

        public virtual string ToQueryString(RouteParameters parameters)
        {
            return makeQueryString(parameters[Name]);
        }

        public abstract object GetRawValue(object input);

        public bool IsSatisfied(RouteParameters routeParameters)
        {
            return routeParameters.Has(Name) || DefaultValue != null;
        }

        public abstract bool HasValue(object input);

        public virtual bool Equals(RouteParameter other)
        {
            if (ReferenceEquals(null, other)) 
                return false;

            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RouteParameter)) return false;
            return Equals((RouteParameter) obj);
        }

        void DescribesItself.Describe(Description description)
        {
            description.Title = Name;

            if (DefaultValue != null)
            {
                description.Properties["DefaultValue"] = DefaultValue.ToString();
            }
        }

        public override string ToString()
        {
            if (DefaultValue != null) 
                return string.Format("Parameter: {0}, DefaultValue: {1}", Name, DefaultValue);

            return "Parameter: {0}".ToFormat(Name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}