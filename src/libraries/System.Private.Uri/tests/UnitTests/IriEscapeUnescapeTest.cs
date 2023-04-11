// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using System.Tests;

using Xunit;

namespace System.Net.Test.Uri.IriTest
{
    /// <summary>
    /// IriEscapeUnescape heap corruption and crash test.
    /// These tests do not check for output correctness although they do validate that normalization is
    /// locale-independent.
    /// </summary>
    public class IriEscapeUnescapeTest
    {
        [Fact]
        public void Iri_EscapeUnescapeIri_FragmentInvalidCharacters()
        {
            string input = "%F4%80%80%BA";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_EscapedAscii()
        {
            string input = "%%%01%35%36";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_EscapedAsciiFollowedByUnescaped()
        {
            string input = "%ABabc";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_InvalidHexSequence()
        {
            string input = "%AB%FG%GF";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_InvalidUTF8Sequence()
        {
            string input = "%F4%80%80%7F";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_IncompleteEscapedCharacter()
        {
            string input = "%F4%80%80%B";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_ReservedCharacters()
        {
            string input = "/?#??#%[]";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_EscapedReservedCharacters()
        {
            string input = "%2F%3F%23%3F%3F%23%25%5B%5D";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_BidiCharacters()
        {
            string input = "\u200E";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_IncompleteSurrogate()
        {
            string input = "\uDBC0\uDC3A\uDBC0";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_InIriRange_AfterEscapingBufferInitialized()
        {
            string input = "\uDBC0\uDC3A\u00A1";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_BidiCharacter_AfterEscapingBufferInitialized()
        {
            string input = "\uDBC0\uDC3A\u200E";
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_UnicodePlane0()
        {
            EscapeUnescapeTestUnicodePlane(0x0, 0xFFFF, 0x100);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_UnicodePlane1()
        {
            EscapeUnescapeTestUnicodePlane(0x10000, 0x1FFFF, 0x100);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_UnicodePlane2()
        {
            EscapeUnescapeTestUnicodePlane(0x20000, 0x2FFFF, 0x100);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_UnicodePlane3_13()
        {
            EscapeUnescapeTestUnicodePlane(0x30000, 0xDFFFF, 0x500);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_UnicodePlane14()
        {
            EscapeUnescapeTestUnicodePlane(0xE0000, 0xEFFFF, 0x100);
        }

        [Fact]
        public void Iri_EscapeUnescapeIri_UnicodePlane15_16()
        {
            EscapeUnescapeTestUnicodePlane(0xF0000, 0x10FFFF, 0x100);
        }

        private void EscapeUnescapeTestUnicodePlane(int start, int end, int step)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = start; i < end; i += step)
            {
                if (i < 0xFFFF)
                {
                    // ConvertFromUtf32 doesn't allow surrogate codepoint values.
                    // This may generate invalid Unicode sequences when i is between 0xd800 and 0xdfff.
                    sb.Append((char)i);
                }
                else
                {
                    sb.Append(char.ConvertFromUtf32(i));
                }
            }

            string input = sb.ToString();
            EscapeUnescapeAllUriComponentsInDifferentCultures(input);
        }

        private void EscapeUnescapeAllUriComponentsInDifferentCultures(string uriInput)
        {
            UriComponents[] components = new UriComponents[]
            {
                UriComponents.AbsoluteUri,
                UriComponents.Fragment,
                UriComponents.Host,
                UriComponents.HostAndPort,
                UriComponents.HttpRequestUrl,
                UriComponents.KeepDelimiter,
                UriComponents.NormalizedHost,
                UriComponents.Path,
                UriComponents.PathAndQuery,
                UriComponents.Port,
                UriComponents.Query,
                UriComponents.Scheme,
                UriComponents.SchemeAndServer,
                UriComponents.SerializationInfoString,
                UriComponents.StrongAuthority,
                UriComponents.StrongPort,
                UriComponents.UserInfo,
            };

            string[] results_en = new string[components.Length];
            string[] results_zh = new string[components.Length];

            for (int i = 0; i < components.Length; i++)
            {
                results_en[i] = EscapeUnescapeTestComponent(uriInput, components[i]);
            }

            using (new ThreadCultureChange("zh-cn"))
            {
                for (int i = 0; i < components.Length; i++)
                {
                    results_zh[i] = EscapeUnescapeTestComponent(uriInput, components[i]);
                }

                for (int i = 0; i < components.Length; i++)
                {
                    Assert.True(
                        0 == string.CompareOrdinal(results_en[i], results_zh[i]),
                        "Detected locale differences when processing UriComponents." + components[i]);
                }
            }
        }

        private string EscapeUnescapeTestComponent(string uriInput, UriComponents component)
        {
            string? ret = null;
            HeapCheck hc = new HeapCheck(uriInput);

            unsafe
            {
                fixed (char* pInput = hc.HeapBlock)
                {
                    ret = IriHelper.EscapeUnescapeIri(pInput + hc.Offset, 0, uriInput.Length, component);
                }
            }

            // check for buffer under and overruns
            hc.ValidatePadding();

            return ret;
        }

        private class HeapCheck
        {
            private const char paddingValue = (char)0xDEAD;
            private const int padding = 32;
            private int _len;
            private char[] _memblock;

            private HeapCheck(int length)
            {
                _len = length;

                _memblock = new char[_len + padding * 2];
                for (int i = 0; i < _memblock.Length; i++)
                {
                    _memblock[i] = paddingValue;
                }
            }

            public HeapCheck(string input) : this(input.Length)
            {
                input.CopyTo(0, _memblock, padding, _len);
            }

            public HeapCheck(char[] input) : this(input.Length)
            {
                input.CopyTo(_memblock, padding);
            }

            public char[] HeapBlock
            {
                get { return _memblock; }
            }

            public int Offset
            {
                get { return padding; }
            }

            public void ValidatePadding()
            {
                for (int i = 0; i < _memblock.Length; i++)
                {
                    AssertValidPadding(i, _len, _memblock[i]);
                }
            }

            public static void ValidatePadding(ValueStringBuilder pooledArray, int expectedLength)
            {
                for (int i = 0; i < pooledArray.Length; i++)
                {
                    if ((i < padding) || (i >= padding + expectedLength))
                    {
                        AssertValidPadding(i, expectedLength, pooledArray[i]);
                    }
                }
            }

            public static ValueStringBuilder CreateFilledPooledArray(int length)
            {
                int size = length + padding * 2;
                ValueStringBuilder vsb = new ValueStringBuilder(size);
                vsb.Length = size;
                for (int i = 0; i < vsb.Length; i++)
                {
                    vsb[i] = paddingValue;
                }
                return vsb;
            }

            private static void AssertValidPadding(int i, int len, char memValue)
            {
                if ((i < padding) || (i >= padding + len))
                {
                    Assert.True(
                        (int)paddingValue == (int)memValue,
                        "Heap corruption detected: unexpected padding value at index: " + i +
                        " Data allocated at idx: " + padding + " - " + (len + padding - 1));
                }
            }
        }
    }
}
