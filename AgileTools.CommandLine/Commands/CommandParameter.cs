using System;

namespace AgileTools.CommandLine.Commands
{
    public class CommandParameter
    {
        public string Name { get; }
        public string Description { get; }

        protected Func<string, object> ConvertFunc { get; }
        protected Func<string, bool> TryParseFunc { get; }

        public virtual object Convert(string value) { return ConvertFunc(value);  }
        public virtual bool TryParse(string value) { return TryParseFunc(value); }

        public CommandParameter(string name, string description, Func<string, object> convertFunc, Func<string, bool> tryParseFunc)
        {
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            Description = !string.IsNullOrEmpty(name) ? description : throw new ArgumentNullException(nameof(description));
            ConvertFunc = convertFunc ?? throw new ArgumentNullException(nameof(convertFunc));
            TryParseFunc = tryParseFunc ?? throw new ArgumentNullException(nameof(tryParseFunc));
        }

        public  class DateTimeParameter : CommandParameter
        {
            public DateTimeParameter(string name, string description) :
                base(name, description, v => DateTime.Parse(v), v => DateTime.TryParse(v, out _))
            { }
        }

        public class IntParameter : CommandParameter
        {
            public IntParameter(string name, string description) :
                base(name, description, v => Int32.Parse(v), v => Int32.TryParse(v,out  _))
            { }
        }

        public class StringParameter : CommandParameter
        {
            public StringParameter(string name, string description, bool canBeEmpty) :
                base(name, description, v => v.Trim(), v => canBeEmpty || !string.IsNullOrEmpty(v))
            { }
        }

        public class EnumParameter<T> : CommandParameter where T : struct
        {
            public EnumParameter(string name, string description, T defaultValue) :
                base(name, description, v => Enum.Parse(typeof(T), v, true), v => Enum.TryParse<T>(v, true, out _))
            { }
        }
    }
}
