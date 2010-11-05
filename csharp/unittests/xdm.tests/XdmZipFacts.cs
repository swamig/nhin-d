﻿/* 
 Copyright (c) 2010, Direct Project
 All rights reserved.

 Authors:
    Arien Malec     arien.malec@nhindirect.org
  
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of The Direct Project (directproject.org) nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xunit;
using Ionic.Zip;
using Ionic.Zlib;

using Health.Direct.Xdm;
using Health.Direct.Xd.Tests;

namespace Health.Direct.Xdm.Tests
{
    public class XdmZipFacts
    {

        [Fact]
        public void XdmHasMetadataFile()
        {
            using (ZipFile z = XDMZipPackager.Default.Package(Examples.TestPackage))
            {
                var entries = z.Entries.Select(e => e.FileName);
                Assert.Contains(String.Format("{0}/{1}/{2}", XDMStandard.MainDirectory, XDMStandard.DefaultSubmissionSet, XDMStandard.MetadataFilename), entries);
            }
        }

        [Fact]
        public void XdmHasReadme()
        {
            using (ZipFile z = XDMZipPackager.Default.Package(Examples.TestPackage))
            {
                var entries = z.Entries.Where(e => e.FileName == XDMStandard.ReadmeFilename);
                Assert.Equal(1, entries.Count());
            }
        }

        [Fact]
        public void XdmHasIndex()
        {
            using (ZipFile z = XDMZipPackager.Default.Package(Examples.TestPackage))
            {
                var entries = z.Entries.Where(e => e.FileName == XDMStandard.IndexHtmFile);
                Assert.Equal(1, entries.Count());
            }
        }

        [Fact]
        public void XdmHasDocumentFile()
        {
            using (ZipFile z = XDMZipPackager.Default.Package(Examples.TestPackage))
            {
                string fileName = String.Format("{0}/{1}/DOC001", XDMStandard.MainDirectory, XDMStandard.DefaultSubmissionSet);
                var entries = z.Entries.Where(e => e.FileName == fileName );
                Assert.Equal(1, entries.Count());
            }
        }

        [Fact]
        public void XdmDocumentFileCorrect()
        {
            using (ZipFile z = XDMZipPackager.Default.Package(Examples.TestPackage))
            {
                z.Save("xdm.zip");
                string docName = String.Format("{0}/{1}/DOC001", XDMStandard.MainDirectory, XDMStandard.DefaultSubmissionSet);
                var entries = z.Entries.Where(e => e.FileName == docName);
                Assert.Equal(1, entries.Count());
                ZipEntry entry = entries.First();
                Assert.NotNull(entry);
                using (MemoryStream docStream = new MemoryStream())
                {
                    entry.Extract(docStream);
                    UTF8Encoding utf8 = new UTF8Encoding();
                    docStream.Seek(0, SeekOrigin.Begin);
                    string docText = utf8.GetString(docStream.ToArray());
                    Assert.Equal(Examples.TestDocument.DocumentString, docText);
                }
            }
        }


    }
}