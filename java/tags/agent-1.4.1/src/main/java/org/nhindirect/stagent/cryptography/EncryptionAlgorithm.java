/* 
Copyright (c) 2010, NHIN Direct Project
All rights reserved.

Authors:
   Umesh Madan     umeshma@microsoft.com
   Greg Meyer      gm2552@cerner.com
 
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer 
in the documentation and/or other materials provided with the distribution.  Neither the name of the The NHIN Direct Project (nhindirect.org). 
nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS 
BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
THE POSSIBILITY OF SUCH DAMAGE.
*/

package org.nhindirect.stagent.cryptography;

/**
 * Enumeration of supported encryption algorithms for message encryption. 
 * @author Greg Meyer
 * @author Umesh Madan
 */
public enum EncryptionAlgorithm 
{
    RSA_3DES,
    AES128,
    AES192,
    AES256;
    
    /**
     * Gets the encryption algorithm from an algorithm name
     * @param algorithmNam The encryption algorithm name
     * @param defaultAlgorithm The default algorithm to return is a corresponding name cannot be found
     * @return A encryption algorithm enumeration object corresponding the name
     */
    public static EncryptionAlgorithm fromString(String algorithmName, EncryptionAlgorithm defaultAlgorithm)
    {
    	if (algorithmName == null || algorithmName.isEmpty())
    		return defaultAlgorithm;
    	
    	if (algorithmName.equalsIgnoreCase("RSA_3DES"))
    		return RSA_3DES;
    	else if (algorithmName.equalsIgnoreCase("AES128"))
    		return AES128;
    	else if (algorithmName.equalsIgnoreCase("AES192"))
    		return AES192;
    	else if (algorithmName.equalsIgnoreCase("AES256"))
    		return AES256;
    	else
    		return defaultAlgorithm;
    }
}
