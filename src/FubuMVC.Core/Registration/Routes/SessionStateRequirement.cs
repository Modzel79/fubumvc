namespace FubuMVC.Core.Registration.Routes
{
    public class SessionStateRequirement
    {
        private readonly string _text;

        public static readonly SessionStateRequirement RequiresSessionState = new SessionStateRequirement("RequiresSessionState");
        public static readonly SessionStateRequirement DoesNotUseSessionState = new SessionStateRequirement("DoesNotUseSessionState");
        public static readonly SessionStateRequirement RequiresReadOnlySessionState = new SessionStateRequirement("RequiresReadOnlySessionState");

        private SessionStateRequirement(string text)
        {
            _text = text;
        }

        public override string ToString()
        {
            return _text;
        }
    }
}