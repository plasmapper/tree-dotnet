using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PL.Tree
{
    /// <summary>
    /// Base class for all bidirectional converters from and to node value.
    /// </summary>
    public abstract class Converter
    {
        /// <summary>
        /// Converts from node value.
        /// </summary>
        /// <param name="nodeValue">Node value.</param>
        /// <returns>Converted value.</returns>
        public abstract object FromNodeValue(object nodeValue);

        /// <summary>
        /// Converts to node value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Node value.</returns>
        public abstract object ToNodeValue(object value);
    }

    /// <summary>
    /// Represents bidirectional converter from and to node value.
    /// </summary>
    /// <typeparam name="NodeValueType">Node value type.</typeparam>
    /// <typeparam name="T">Converted value type.</typeparam>
    public class Converter<NodeValueType, T> : Converter
    {
        /// <summary>
        /// Initializes a new instance of the Converter class.
        /// </summary>
        public Converter() { }

        public override object FromNodeValue(object value) => FromNodeValue((NodeValueType)value);

        public override object ToNodeValue(object value) => ToNodeValue((T)value);

        /// <summary>
        /// Converts from node value.
        /// </summary>
        /// <param name="value">Node value.</param>
        /// <returns>Converted value.</returns>
        public virtual T FromNodeValue(NodeValueType value) => (T)(object)value;

        /// <summary>
        /// Converts to node value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Node value.</returns>
        public virtual NodeValueType ToNodeValue(T value) => (NodeValueType)(object)value;
    }

    /// <summary>
    /// Represents bidirectional converter between node and string values.
    /// </summary>
    /// <typeparam name="NodeValueType">Node value type.</typeparam>
    public class NodeStringConverter<NodeValueType> : Converter<NodeValueType, string>
    {
        private readonly TypeConverter _typeConverter = TypeDescriptor.GetConverter(typeof(NodeValueType));
        private string _format = null;

        /// <summary>
        /// Initializes a new instance of the StringConverter class.
        /// </summary>
        /// <param name="format">String format.</param>
        public NodeStringConverter(string format = null) => _format = format;

        /// <summary>
        /// Gets and sets string format.
        /// </summary>
        public string Format
        {
            get
            {
                lock (this)
                    return _format;
            }
            set
            {
                lock (this)
                    _format = value;
            }
        }

        public override string FromNodeValue(NodeValueType value)
        {
            lock (this)
                return _format == null ? value?.ToString() : ((IFormattable)value)?.ToString(_format, null);
        }

        public override NodeValueType ToNodeValue(string value) => (NodeValueType)_typeConverter.ConvertFromString(value);
    }

    /// <summary>
    /// Represents bidirectional converter from and to Enum node.
    /// </summary>
    /// <typeparam name="NodeValueType">Node enum value type.</typeparam>
    /// <typeparam name="T">Converted value type.</typeparam>
    public class EnumNodeConverter<NodeValueType, T> : Converter<NodeValueType, T> where NodeValueType : Enum
    {
        private readonly Array _enumValues = Enum.GetValues(typeof(NodeValueType));
        private readonly List<T> _values;

        /// <summary>
        /// Initializes a new instance of the EnumNodeConverter class.
        /// </summary>
        /// <param name="values">List of values that correspond to Enum.GetValues(typeof(NodeValueType)). If null, then converted value is enum value index.</param>
        public EnumNodeConverter(List<T> values = null) => _values = values;

        public override T FromNodeValue(NodeValueType value) =>
            _values != null ? _values[Array.IndexOf(_enumValues, value)] : (T)(object)Array.IndexOf(_enumValues, value);

        public override NodeValueType ToNodeValue(T value) =>
            (NodeValueType)_enumValues.GetValue(_values != null ? _values.IndexOf(value) : (int)(object)value);
    }
}
