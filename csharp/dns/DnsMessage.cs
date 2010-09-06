﻿/* 
 Copyright (c) 2010, NHIN Direct Project
 All rights reserved.

 Authors:
    Umesh Madan     umeshma@microsoft.com
    Sean Nolan      seannol@microsoft.com
 
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of the The NHIN Direct Project (nhindirect.org). nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DnsResolver
{
    /// <summary>
    /// A base DNS message, consisting of a header and a question
    /// Actual DNS messages are DNSRequest and DnsResponse
    /// </summary>
    public class DnsMessage
    {
        DnsHeader m_header;
        DnsQuestion m_question;

        protected DnsMessage(Dns.RecordType qType, string qName)
        {
            m_header = new DnsHeader();

            m_header.IsRequest = true;
            m_header.OpCode = Dns.OpCode.QUERY;

            m_header.IsAuthoritativeAnswer = false;
            m_header.IsTruncated = false;
            m_header.IsRecursionDesired = true;
            m_header.IsRecursionAvailable = false;
            m_header.ResponseCode = Dns.ResponseCode.SUCCESS;
            m_header.QuestionCount = 1;
            m_header.AnswerCount = 0;
            m_header.NameServerAnswerCount = 0;
            m_header.AdditionalAnswerCount = 0;

            m_question = new DnsQuestion(qName, qType, Dns.Class.IN);
        }
        
        protected DnsMessage(ref DnsBufferReader reader)
        {
            this.Deserialize(ref reader);
        }
        
        protected DnsMessage(DnsBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            
            DnsBufferReader reader = buffer.CreateReader();
            this.Deserialize(ref reader);
        }
        
        public DnsHeader Header
        {
            get
            {
                return m_header;
            }
        }
        
        public DnsQuestion Question
        {
            get
            {
                return m_question;
            }            
        }
        
        public ushort RequestID
        {
            get
            {
                return m_header.UniqueID;
            }
            set
            {
                m_header.UniqueID = value;
            }
        }

        public bool IsSuccess
        {
            get
            {
                return (this.m_header != null && m_header.ResponseCode == Dns.ResponseCode.SUCCESS);
            }
        }

        public bool IsNameError
        {
            get
            {
                return (this.m_header != null && m_header.ResponseCode == Dns.ResponseCode.NAME_ERROR);
            }
        }
        
        public void Serialize()
        {
            DnsBuffer buffer = new DnsBuffer(DnsClient.UDPMaxBuffer);
            this.Serialize(buffer);
        }
        
        /// <summary>
        /// Serialize this Dns Message into the given DnsBuffer
        /// </summary>
        /// <param name="buffer">buffer to serialize into</param>
        public virtual void Serialize(DnsBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            m_header.Serialize(buffer);
            m_question.Serialize(buffer);
        }
                
        protected virtual void Deserialize(ref DnsBufferReader reader)
        {
            m_header = new DnsHeader(ref reader);
            m_question = new DnsQuestion(ref reader);
        }
    }
}
