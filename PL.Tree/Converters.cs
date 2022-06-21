using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PL.Tree
{
    /// <summary>
    /// Base class for all bidirectional converters.
    /// </summary>
    public abstract class Converter
    {
        /// <summary>
        /// Converts source to target.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <returns>Target.</returns>
        public abstract object ConvertSourceToTarget(object source);

        /// <summary>
        /// Converts target to source.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <returns>Source.</returns>
        public abstract object ConvertTargetToSource(object target);
    }

    /// <summary>
    /// Base class for all bidirectional converters with specified source and target types.
    /// </summary>
    /// <typeparam name="TSource">Source type.</typeparam>
    /// <typeparam name="TTarget">Target type.</typeparam>
    public abstract class Converter<TSource, TTarget> : Converter
    {
        /// <summary>
        /// Initializes a new instance of the Converter class.
        /// </summary>
        public Converter() { }

        public override object ConvertSourceToTarget(object source) => ConvertSourceToTarget((TSource)source);

        public override object ConvertTargetToSource(object target) => ConvertTargetToSource((TTarget)target);

        /// <summary>
        /// Converts source to target.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <returns>Target.</returns>
        public abstract TTarget ConvertSourceToTarget(TSource source);

        /// <summary>
        /// Converts target to source.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <returns>Source.</returns>
        public abstract TSource ConvertTargetToSource(TTarget target);
    }

    /// <summary>
    /// Bidirectional converter that casts source to target and target to source.
    /// </summary>
    /// <typeparam name="TSource">Source type.</typeparam>
    /// <typeparam name="TTarget">Target type.</typeparam>
    public class CastConverter<TSource, TTarget> : Converter<TSource, TTarget>
    {
        public override TTarget ConvertSourceToTarget(TSource source) => (TTarget)(object)source;
        public override TSource ConvertTargetToSource(TTarget target) => (TSource)(object)target;
    }

    /// <summary>
    /// Bidirectional converter between some type and string.
    /// </summary>
    /// <typeparam name="TSource">Source type.</typeparam>
    public class StringConverter<TSource> : Converter<TSource, string>
    {
        private readonly TypeConverter _typeConverter = TypeDescriptor.GetConverter(typeof(TSource));
        private string _format = null;

        /// <summary>
        /// Initializes a new instance of the StringConverter class.
        /// </summary>
        /// <param name="format">String format.</param>
        public StringConverter(string format = null) => _format = format;

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

        public override string ConvertSourceToTarget(TSource source)
        {
            lock (this)
                return _format == null ? source?.ToString() : ((IFormattable)source)?.ToString(_format, null);
        }

        public override TSource ConvertTargetToSource(string target) => (TSource)_typeConverter.ConvertFromString(target);
    }

    /// <summary>
    /// Bidirectional converter between enum and it's value index.
    /// </summary>
    /// <typeparam name="TSource">Source enum type.</typeparam>
    public class EnumIndexConverter<TSource> : Converter<TSource, int> where TSource : Enum
    {
        private readonly Array _enumValues = Enum.GetValues(typeof(TSource));

        /// <summary>
        /// Initializes a new instance of the EnumIndexConverter class.
        /// </summary>
        public EnumIndexConverter() { }

        public override int ConvertSourceToTarget(TSource source) => Array.IndexOf(_enumValues, source);

        public override TSource ConvertTargetToSource(int target) => (TSource)_enumValues.GetValue(target);
    }

    /// <summary>
    /// Bidirectional converter between enum and value in the List with items that correspond to enum values.
    /// </summary>
    /// <typeparam name="TSource">Source enum type.</typeparam>
    /// <typeparam name="TTarget">Target type.</typeparam>
    public class EnumConverter<TSource, TTarget> : Converter<TSource, TTarget> where TSource : Enum
    {
        private readonly Array _enumValues = Enum.GetValues(typeof(TSource));
        private readonly List<TTarget> _targetValues;

        /// <summary>
        /// Initializes a new instance of the EnumConverter class.
        /// </summary>
        /// <param name="targetValues">List of values that correspond to enum values.</param>
        public EnumConverter(List<TTarget> targetValues) => _targetValues = targetValues;

        public override TTarget ConvertSourceToTarget(TSource source) => _targetValues[Array.IndexOf(_enumValues, source)];

        public override TSource ConvertTargetToSource(TTarget target) => (TSource)_enumValues.GetValue(_targetValues.IndexOf(target));
    }
}