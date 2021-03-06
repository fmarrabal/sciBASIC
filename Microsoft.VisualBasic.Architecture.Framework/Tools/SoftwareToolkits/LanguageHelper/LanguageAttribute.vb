﻿#Region "Microsoft.VisualBasic::24d16f47c4777dea438a18e6593c3781, ..\sciBASIC#\Microsoft.VisualBasic.Architecture.Framework\Tools\SoftwareToolkits\LanguageHelper\LanguageAttribute.vb"

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

Namespace SoftwareToolkits.Globalization

    <AttributeUsage(AttributeTargets.Property Or
        AttributeTargets.Field Or
        AttributeTargets.Enum, AllowMultiple:=True, Inherited:=True)>
    Public Class LanguageAttribute : Inherits Attribute

        Public ReadOnly Property Language As Integer
        Public ReadOnly Property Text As String

        ReadOnly _langEnum As Type

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Language">The language of the target object.</param>
        ''' <param name="Context"></param>
        ''' <param name="langEnum">This type define should be a enum type.</param>
        ''' <remarks></remarks>
        Sub New(Language As Integer, Context As String, Optional langEnum As Type = Nothing)
            Me.Language = Language
            Me.Text = Context
            Me._langEnum = langEnum
        End Sub

        Public Overrides Function ToString() As String
            If _langEnum Is Nothing Then
                Return Text
            Else
                Dim language As String = CTypeDynamic(Me.Language, _langEnum)
                Return $"@{language}  ""{Text}"""
            End If
        End Function

        Public Shared Narrowing Operator CType(context As LanguageAttribute) As String
            Return context.Text
        End Operator
    End Class
End Namespace
