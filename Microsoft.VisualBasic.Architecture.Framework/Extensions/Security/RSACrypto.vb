﻿#Region "Microsoft.VisualBasic::1396ba92c7a38baee7781ef0e8edcfde, ..\sciBASIC#\Microsoft.VisualBasic.Architecture.Framework\Extensions\Security\RSACrypto.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.IO
Imports System.Text
Imports System.Security.Cryptography

Namespace SecurityString

    Public Class RSACrypto : Inherits SecurityString.SecurityStringModel

        Dim rsa As RSACryptoServiceProvider = New RSACryptoServiceProvider()

        Public Overrides Function Decrypt(encrypted() As Byte) As Byte()
            Return rsa.Decrypt(encrypted, False)
        End Function

        Public Overrides Function DecryptString(text As String) As String
            Dim encrypted As Byte() = System.Text.ASCIIEncoding.UTF8.GetBytes(text)
            Dim decText As String = System.Text.ASCIIEncoding.UTF8.GetString(encrypted)
            Return decText
        End Function

        Public Overrides Function Encrypt(input() As Byte) As Byte()
            Return rsa.Encrypt(input, False)
        End Function

        Public Overrides Function EncryptData(text As String) As String
            Dim encrypted() As Byte = rsa.Encrypt(System.Text.ASCIIEncoding.UTF8.GetBytes(text), False)
            Dim encText As String = System.Text.ASCIIEncoding.UTF8.GetString(encrypted)
            Return encText
        End Function
    End Class
End Namespace
