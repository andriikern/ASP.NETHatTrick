using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Net.Mime;

namespace HatTrick.API.HttpContents
{
    public sealed class XmlContent : HttpContent
    {
        private static readonly MediaTypeHeaderValue _defaultMediaType =
            MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Xml);

        private static Encoding GetEncodingFromCharset(
            string? charset
        )
        {
            Encoding encoding;

            try
            {
                if (string.IsNullOrEmpty(charset))
                {
                    encoding = Encoding.Default;
                }
                else if (
                    charset.Length > 2 &&
                    charset.StartsWith('\"') &&
                    charset.EndsWith('\"')
                )
                {
                    encoding = Encoding.GetEncoding(charset[1..^1]);
                }
                else
                {
                    encoding = Encoding.GetEncoding(charset);
                }
            }
            catch (ArgumentException exception)
            {
                throw new InvalidOperationException(
                    "The character set provided in ContentType is invalid.",
                    exception
                );
            }

            return encoding;
        }

        public static XmlContent Create(
            object? inputValue,
            Type inputType,
            MediaTypeHeaderValue? mediaType = null,
            XmlWriterSettings? settings = null,
            XmlAttributeOverrides? overrides = null,
            Type[]? extraTypes = null,
            XmlRootAttribute? root = null,
            string? defaultNamespace = null,
            string? location = null,
            XmlSerializerNamespaces? namespaces = null
        ) =>
            new XmlContent(
                inputType,
                inputValue,
                mediaType,
                settings,
                overrides,
                extraTypes,
                root,
                defaultNamespace,
                location,
                namespaces
            );

        public static XmlContent Create<T>(
            T inputValue,
            MediaTypeHeaderValue? mediaType = null,
            XmlWriterSettings? settings = null,
            XmlAttributeOverrides? overrides = null,
            Type[]? extraTypes = null,
            XmlRootAttribute? root = null,
            string? defaultNamespace = null,
            string? location = null,
            XmlSerializerNamespaces? namespaces = null
        ) =>
            Create(
                inputValue,
                typeof(T),
                mediaType,
                settings,
                overrides,
                extraTypes,
                root,
                defaultNamespace,
                location,
                namespaces
            );

        private readonly XmlWriterSettings? _settings;
        private readonly XmlAttributeOverrides? _overrides;
        private readonly XmlRootAttribute? _root;
        private readonly string? _defaultNamespace;
        private readonly string? _location;
        private readonly XmlSerializerNamespaces? _namespaces;
        private readonly Type _objectType;
        private readonly Type[] _extraTypes;
        private readonly object? _value;
        
        public Type ObjectType =>
            _objectType;

        public object? Value =>
            _value;

        private XmlContent(
            Type inputType,
            object? inputValue,
            MediaTypeHeaderValue? mediaType,
            XmlWriterSettings? settings,
            XmlAttributeOverrides? overrides,
            Type[]? extraTypes,
            XmlRootAttribute? root,
            string? defaultNamespace,
            string? location,
            XmlSerializerNamespaces? namespaces
        )
        {
            if (inputType is null)
            {
                throw new ArgumentNullException(nameof(inputType));
            }

            if (
                inputValue is not null &&
                !inputType.IsAssignableFrom(inputValue.GetType())
            )
            {
                throw new ArgumentException(
                    $"The specified type {inputType} must derive from the specific value's type {inputValue.GetType()}.",
                    nameof(inputType)
                );
            }

            _settings = settings;
            _overrides = overrides;
            _extraTypes = extraTypes ?? Array.Empty<Type>();
            _root = root;
            _defaultNamespace = defaultNamespace;
            _location = location;
            _namespaces = namespaces;
            _objectType = inputType;
            _value = inputValue;

            Headers.ContentType = mediaType ?? _defaultMediaType;
        }

        private Encoding GetEncoding() =>
            GetEncodingFromCharset(Headers.ContentType?.CharSet);

        protected override Task SerializeToStreamAsync(
            Stream stream,
            TransportContext? context
        )
        {
            var encoding = GetEncoding();

            using (
                var textWriter = new StreamWriter(
                    stream,
                    encoding: encoding,
                    leaveOpen: true
                )
            )
            using (var xmlWriter = XmlWriter.Create(textWriter, _settings))
            {
                var serialiser = new XmlSerializer(
                    ObjectType,
                    _overrides,
                    _extraTypes,
                    _root,
                    _defaultNamespace,
                    _location
                );

                serialiser.Serialize(
                    xmlWriter,
                    Value,
                    _namespaces
                );
            }

            return Task.CompletedTask;
        }

        protected override bool TryComputeLength(
            out long length
        )
        {
            length = 0;

            return false;
        }
    }
}
