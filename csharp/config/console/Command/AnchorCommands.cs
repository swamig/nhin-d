﻿/* 
 Copyright (c) 2010, Direct Project
 All rights reserved.

 Authors:
    Umesh Madan     umeshma@microsoft.com
  
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of The Direct Project (directproject.org) nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using System.ServiceModel;

using Health.Direct.Common.Certificates;
using Health.Direct.Common.Extensions;
using Health.Direct.Config.Client;
using Health.Direct.Config.Client.CertificateService;
using Health.Direct.Config.Store;
using Health.Direct.Config.Tools.Command;

namespace Health.Direct.Config.Console.Command
{
    /// <summary>
    /// Commands to manage Anchors
    /// </summary>
    public class AnchorCommands : CommandsBase<AnchorStoreClient>
    {
        //---------------------------------------
        //
        // Commands
        //
        //---------------------------------------

        internal AnchorCommands(ConfigConsole console, Func<AnchorStoreClient> client) : base(console, client)
        {
        }

        /// <summary>
        /// Import and add an anchor
        /// </summary>
        [Command(Name = "Anchor_Add", Usage = AnchorAddUsage)]
        public void AnchorAdd(string[] args)
        {
            string owner = args.GetRequiredValue(0);
            string filePath = args.GetRequiredValue(1);
            string password = args.GetOptionalValue(2, string.Empty);
            
            PushCerts(owner, GetCommand<CertificateCommands>().LoadCerts(filePath, password), false);
        }

        private const string AnchorAddUsage
            = "Import an anchor certificate from a file and push it into the store."
              + CRLF + "The anchor is used for both incoming & outgoing trust."
              + CRLF + "    owner filepath [password]"
              + CRLF + "\t owner: Anchor owner"
              + CRLF + "\t filePath: path fo the certificate file. Can be .DER, .CER or .PFX"
              + CRLF + "\t password: (optional) file password";
        
        /// <summary>
        /// Retrieve an anchor by its ID
        /// </summary>
        [Command(Name = "Anchor_ByID_Get", Usage = AnchorByIDGetUsage)]
        public void AnchorByIDGet(string[] args)
        {
            long anchorID = args.GetRequiredValue<int>(0);
            CertificateGetOptions options = GetCommand<CertificateCommands>().GetOptions(args, 1);

            Anchor[] anchors = Client.GetAnchors(new[] { anchorID }, options);
            this.Print(anchors);
        }

        private const string AnchorByIDGetUsage
            = "Get an anchor by its id."
              + CRLF + "    anchorID [options]"
              + CRLF + CertificateCommands.PrintOptionsUsage;
        
        
        /// <summary>
        /// Get all anchors for an owner
        /// </summary>
        [Command(Name = "Anchors_Get", Usage = AnchorsGetUsage)]
        public void AnchorsGet(string[] args)
        {
            string owner = args.GetRequiredValue(0);
            CertificateGetOptions options = GetCommand<CertificateCommands>().GetOptions(args, 1);
     
            Anchor[] anchors = Client.GetAnchorsForOwner(owner, options);
            this.Print(anchors);
        }

        private const string AnchorsGetUsage
            = "Get all anchors for an owner."
              + CRLF + "  owner [options]"
              + CRLF + "\t owner: Anchor owner"
              + CRLF + CertificateCommands.PrintOptionsUsage;
        
        /// <summary>
        /// List ALL anchors
        /// </summary>
        [Command(Name = "Anchors_List", Usage = AnchorsListUsage)]
        public void AnchorsList(string[] args)
        {
            CertificateGetOptions options = GetCommand<CertificateCommands>().GetOptions(args, 0);
            //
            // TODO: Give the ability to "more" through this list
            //
            foreach(Anchor anchor in Client.EnumerateAnchors(10, options))
            {
                this.Print(anchor);
                CommandUI.PrintSectionBreak();
            }
        }

        private const string AnchorsListUsage
            = "List all anchors"
            + CRLF + CertificateCommands.PrintOptionsUsage;
        
        /// <summary>
        /// Set the status of all anchors for an owner
        /// </summary>
        [Command(Name = "Anchor_Status_Set", Usage = AnchorStatusSetUsage)]
        public void AnchorStatusSet(string[] args)
        {
            string owner = args.GetRequiredValue(0);
            EntityStatus status = args.GetRequiredEnum<EntityStatus>(1);

            Client.SetAnchorStatusForOwner(owner, status);
        }

        private const string AnchorStatusSetUsage
            = "Set the status for ALL anchors for an owner."
              + CRLF + "    owner"
              + CRLF + "\t owner: Anchor owner"
              + CRLF + "\t status: " + EntityStatusString;
        
        /// <summary>
        /// Remove an anchor
        /// </summary>
        [Command(Name = "Anchor_Remove", Usage = AnchorRemoveUsage)]
        public void AnchorRemove(string[] args)
        {
            long anchorID = args.GetRequiredValue<long>(0);
            Client.RemoveAnchor(anchorID);
        }

        private const string AnchorRemoveUsage
            = "Remove anchors with given ID"
              + CRLF + "    anchorID";
        
        /// <summary>
        /// Mirrors what the production gateway would do
        /// </summary>
        [Command(Name = "Anchor_Resolve", Usage = AnchorResolveUsage)]
        public void AnchorResolve(string[] args)
        {
            MailAddress owner = new MailAddress(args.GetRequiredValue(0));
            CertificateGetOptions options = GetCommand<CertificateCommands>().GetOptions(args, 1);

            Anchor[] anchors = Client.GetAnchorsForOwner(owner.Address, options);
            if (ArrayExtensions.IsNullOrEmpty(anchors))
            {
                anchors = Client.GetAnchorsForOwner(owner.Host, options);
            }
            this.Print(anchors);
        }

        private const string AnchorResolveUsage
            = "Resolves anchors for an owner - like the Smtp Gateway would."
              + CRLF + "    owner [options]"
              + CRLF + "\t owner: Anchor owner"
              + CRLF + CertificateCommands.PrintOptionsUsage;

        //---------------------------------------
        //
        // Implementation details
        //
        //---------------------------------------               
        internal void PushCerts(string owner, IEnumerable<X509Certificate2> certs, bool checkForDupes)
        {
            foreach (X509Certificate2 cert in certs)
            {
                try
                {
                    if (!checkForDupes || !Client.Contains(owner, cert))
                    {
                        Client.AddAnchor(new Anchor(owner, cert, true, true));
                        WriteLine("Added {0}", cert.Subject);
                    }
                    else
                    {
                        WriteLine("Exists {0}", cert.ExtractEmailNameOrName());
                    }
                }
                catch(FaultException<ConfigStoreFault> ex)
                {
                    if (ex.Detail.Error == ConfigStoreError.UniqueConstraint)
                    {
                        WriteLine("Exists {0}", cert.Subject);
                    }
                    else
                    {
                        throw;
                    }                    
                }
            }
        }

        internal void PushCerts(string owner, IEnumerable<X509Certificate2> certs, bool checkForDupes, EntityStatus status)
        {
            PushCerts(owner, certs, checkForDupes);
            Client.SetAnchorStatusForOwner(owner, EntityStatus.Enabled);
        }
        
        void Print(Anchor[] anchors)
        {
            if (anchors == null || anchors.Length == 0)
            {
                WriteLine("No certificates found");
                return;
            }
            foreach (Anchor cert in anchors)
            {
                this.Print(cert);
                CommandUI.PrintSectionBreak();
            }
        }

        void Print(Anchor cert)
        {
            CommandUI.Print("Owner", cert.Owner);
            CommandUI.Print("Thumbprint", cert.Thumbprint);
            CommandUI.Print("ID", cert.ID);
            CommandUI.Print("CreateDate", cert.CreateDate);
            CommandUI.Print("ValidStart", cert.ValidStartDate);
            CommandUI.Print("ValidEnd", cert.ValidEndDate);
            CommandUI.Print("ForIncoming", cert.ForIncoming);
            CommandUI.Print("ForOutgoing", cert.ForOutgoing);
            
            if (cert.HasData)
            {
                X509Certificate2 x509 = cert.ToX509Certificate();
                GetCommand<CertificateCommands>().Print(x509);
            }
        }
    }
}