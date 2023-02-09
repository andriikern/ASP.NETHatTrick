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

        private static Boolean IsExceptionToCatch(
            [MaybeNullWhen(false), NotNullWhen(true)] Exception ex
        ) =>
            ex is ArgumentException ||
                ex is InvalidOperationException ||
                ex is IOException ||
                ex is ObjectDisposedException;

        private static Encoding GetEncoding(
            String? contentType
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

        public TextPlainInputFormatter()
        {
            foreach (var type in TextPlainMediaTypes)
            {
                SupportedMediaTypes.Add(type);
            }
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context
        )
        {
            string content;

            try
            {
                var request = context.HttpContext.Request;
                var encoding = GetEncoding(request.ContentType);

                using var reader = new StreamReader(request.Body, encoding);

                content = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            catch (Exception ex) when (IsExceptionToCatch(ex))
            {
                return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
            }

            return string.IsNullOrEmpty(content) ?
                await InputFormatterResult.NoValueAsync().ConfigureAwait(false) :
                await InputFormatterResult.SuccessAsync(content).ConfigureAwait(false);
        }

        public override Boolean CanRead(InputFormatterContext context)
        {
            var contentType = context.HttpContext.Request.ContentType;
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            contentType = contentType.Split(';', 1, StringSplitOptions.TrimEntries).First();

            return TextPlainMediaTypesRegex.Any(r => r.IsMatch(contentType));
        }
    }
}
