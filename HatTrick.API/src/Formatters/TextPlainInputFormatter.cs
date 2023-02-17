using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace HatTrick.API.Formatters
{
    public sealed class TextPlainInputFormatter : InputFormatter
    {
        private static IReadOnlyCollection<string> TextPlainMediaTypes { get; set; }
        private static IReadOnlyCollection<Regex> TextPlainMediaTypesRegex { get; set; }

        static TextPlainInputFormatter()
        {
            var textPlainMediaTypes = new List<string>()
            {
                MediaTypeNames.Text.Plain,
                "text/*"
            };
            var textPlainRegexMediaTypes = new List<Regex>(
                textPlainMediaTypes.Select(
                    t => new Regex(
                        Regex.Escape(
                            t.Replace(
                                "*",
                                /* language = regexp | jsregexp */ @"\w+"
                            )
                        ),
                        RegexOptions.CultureInvariant |
                            RegexOptions.IgnoreCase |
                            RegexOptions.Singleline |
                            RegexOptions.Compiled
                    )
                )
            );

            textPlainMediaTypes.TrimExcess();
            textPlainRegexMediaTypes.TrimExcess();

            TextPlainMediaTypes = textPlainMediaTypes.AsReadOnly();
            TextPlainMediaTypesRegex = textPlainRegexMediaTypes.AsReadOnly();
        }

        private static bool IsExceptionToCatch(
            [MaybeNullWhen(false), NotNullWhen(true)] Exception exception
        ) =>
            exception is ArgumentException ||
                exception is InvalidOperationException ||
                exception is IOException ||
                exception is ObjectDisposedException;

        private static Encoding GetEncoding(
            string? contentType
        )
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return Encoding.Default;
            }

            var parts = contentType.Split(
                ';',
                StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries
            );

            var charset = parts.SingleOrDefault(
                p => p.StartsWith(
                    "charset",
                    StringComparison.InvariantCultureIgnoreCase
                )
            );

            return string.IsNullOrEmpty(charset) ?
                Encoding.Default :
                Encoding.GetEncoding(
                    charset[(charset.IndexOf('=') + 1)..]
                        .Trim()
                        .ToUpperInvariant()
                );
        }

        private readonly ILogger<TextPlainInputFormatter>? _logger;

        public TextPlainInputFormatter(
            ILogger<TextPlainInputFormatter>? logger
        )
        {
            _logger = logger;

            foreach (var type in TextPlainMediaTypes)
            {
                SupportedMediaTypes.Add(type);
            }
        }

        public TextPlainInputFormatter() : this(null)
        {
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context
        )
        {
            string content;

            var request = context?.HttpContext.Request;

            _logger?.LogTrace(
                "Reading input from the HTTP request... Method: {method}, path: {path}, query: {query}, content type: {contentType}",
                    request?.Method,
                    request?.Path,
                    request?.QueryString,
                    request?.ContentType
            );

            try
            {
                if (context is null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                var encoding = GetEncoding(request!.ContentType);

                using var reader = new StreamReader(
                    request.Body,
                    encoding,
                    true,
                    -1,
                    true
                );

                content = await reader.ReadToEndAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
                when (IsExceptionToCatch(exception))
            {
                _logger?.LogError(
                    exception,
                    "Error while reading input from the HTTP request... Method: {method}, path: {path}, query: {query}, content type: {contentType}",
                        request!.Method,
                        request!.Path,
                        request!.QueryString,
                        request!.ContentType
                );

                return await InputFormatterResult.FailureAsync()
                    .ConfigureAwait(false);
            }

            _logger?.LogDebug(
                "Input successfully read from the HTTP request... Method: {method}, path: {path}, query: {query}, content type: {contentType}, content length: {length}",
                    request!.Method,
                    request!.Path,
                    request!.QueryString,
                    request!.ContentType,
                    content.Length
            );

            return string.IsNullOrEmpty(content) ?
                await InputFormatterResult.NoValueAsync()
                    .ConfigureAwait(false) :
                await InputFormatterResult.SuccessAsync(content)
                    .ConfigureAwait(false);
        }

        public override bool CanRead(
            InputFormatterContext context
        )
        {
            bool canRead = false;

            var request = context?.HttpContext.Request;

            _logger?.LogTrace(
                "Checking if possible to read input from the HTTP request... Method: {method}, path: {path}, query: {query}, content type: {contentType}",
                    request?.Method,
                    request?.Path,
                    request?.QueryString,
                    request?.ContentType
            );

            try
            {
                if (context is null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                var contentType = request!.ContentType;

                if (string.IsNullOrEmpty(contentType))
                {
                    return false;
                }

                contentType = contentType.Split(';', 1, StringSplitOptions.TrimEntries)
                    .First();

                canRead = TextPlainMediaTypesRegex.Any(r => r.IsMatch(contentType));
            }
            catch (Exception exception)
                when (IsExceptionToCatch(exception))
            {
                _logger?.LogError(
                    exception,
                    "Error while checking if possible to read input from the HTTP request... Method: {method}, path: {path}, query: {query}, content type: {contentType}",
                        request!.Method,
                        request!.Path,
                        request!.QueryString,
                        request!.ContentType
                );

                return false;
            }

            _logger?.LogDebug(
                "Successfully checked if possible to read from the HTTP request... Method: {method}, path: {path}, query: {query}, content type: {contentType}, can read: {canRead}",
                    request!.Method,
                    request!.Path,
                    request!.QueryString,
                    request!.ContentType,
                    canRead
            );

            return canRead;
        }
    }
}
