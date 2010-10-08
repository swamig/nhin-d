/* 
 * Copyright (c) 2010, NHIN Direct Project
 * All rights reserved.
 *  
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright 
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright 
 *    notice, this list of conditions and the following disclaimer in the 
 *    documentation and/or other materials provided with the distribution.  
 * 3. Neither the name of the the NHIN Direct Project (nhindirect.org)
 *    nor the names of its contributors may be used to endorse or promote products 
 *    derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY 
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

package org.nhindirect.xd.transform;

import ihe.iti.xds_b._2007.ProvideAndRegisterDocumentSetRequestType;

import java.util.List;

import org.nhindirect.xd.transform.exception.TransformationException;

/**
 * Interface for handling the transformation of a Document to a
 * ProvideAndRegisterDocumentSetRequestType object.
 * 
 * @author beau
 */
public interface DocumentXdsTransformer
{
    /**
     * Transform a document to a ProvideAndRegisterDocumentSetRequestType
     * object.
     * 
     * TODO: This should take a File or the like, instead of Strings
     * representing data.
     * 
     * @param document
     *            String representation of the document data.
     * @param metadata
     *            String representation of the metadata.
     * @return a ProvideAndRegisterDocumentSetRequestType object.
     * @throws TransformationException
     */
    public ProvideAndRegisterDocumentSetRequestType transform(String document, String metadata)
            throws TransformationException;

    /**
     * Transform a list of documents to a
     * ProvideAndRegisterDocumentSetRequestType object.
     * 
     * @param meta
     *            The document metadata.
     * @param docs
     *            The document data.
     * @return a ProvideAndRegisterDocumentSetRequestType object.
     * @throws TransformationException
     * @Deprecated Deprecated until we find proper construction of
     *             multi-documents.
     */
    @Deprecated
    public ProvideAndRegisterDocumentSetRequestType transform(String meta, List<String> docs)
            throws TransformationException;

}
