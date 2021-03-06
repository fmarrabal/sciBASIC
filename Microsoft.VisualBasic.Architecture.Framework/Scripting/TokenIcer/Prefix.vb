﻿#Region "Microsoft.VisualBasic::6062ba82650746ccf2249c7827e7131e, ..\sciBASIC#\Microsoft.VisualBasic.Architecture.Framework\Scripting\TokenIcer\Prefix.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Namespace Scripting.TokenIcer

    ''' <summary>
    ''' 预定义的一些脚本的解析程序
    ''' </summary>
    Public Module Prefix

        Public Const undefined$ = NameOf(MathTokens.UNDEFINED)

        ' This is our token enumeration. It holds every token defined in the grammar
        ''' <summary>
        ''' Tokens is an enumeration of all possible token values.
        ''' </summary>
        Public Enum MathTokens
            UNDEFINED = 0
            CallFunc = 1
            Float = 2
            Factorial = 3
            [Integer] = 4
            ArrayType = 5
            ParamDeli = 6
            WhiteSpace = 7
            [Let] = 8
            Equals = 9
            LPair = 10
            RPair = 11
            Asterisk = 12
            Slash = 13
            Plus = 14
            Minus = 15
            Power = 16
            [Mod] = 17
            Pretend = 18
            [And] = 19
            [Not] = 20
            [Or] = 21
            var = 22
            varRef = 23
            constRef = 24
        End Enum

        ReadOnly _mathStack As New StackTokens(Of MathTokens)(Function(a, b) a = b) With {
            .LPair = MathTokens.LPair,
            .ParamDeli = MathTokens.ParamDeli,
            .Pretend = MathTokens.Pretend,
            .RPair = MathTokens.RPair,
            .WhiteSpace = MathTokens.WhiteSpace
        }

        Public Function MathExpression(expr As String) As Func(Of MathTokens)
            SyncLock MathParser
                Return MathParser.TokenParser(expr, _mathStack)
            End SyncLock
        End Function

        Public ReadOnly Property MathParser As TokenParser(Of MathTokens) =
            __getMathParser()

        Private Function __getMathParser() As TokenParser(Of MathTokens)
            Return New TokenParser(Of MathTokens)(MathParserHash, MathTokens.UNDEFINED)
        End Function

        ''' <summary>
        ''' These lines add each grammar rule to the dictionary
        ''' </summary>
        ''' <returns></returns>
        Public Function MathParserHash() As Dictionary(Of MathTokens, String)
            Dim tokens As Dictionary(Of MathTokens, String) =
                New Dictionary(Of MathTokens, String)

            tokens.Add(MathTokens.CallFunc, "->\s*[a-zA-Z_][a-zA-Z0-9_]*")
            tokens.Add(MathTokens.Float, "[0-9]+\.+[0-9]+")
            tokens.Add(MathTokens.Factorial, "[0-9]+!")
            tokens.Add(MathTokens.[Integer], "[0-9]+")
            tokens.Add(MathTokens.ArrayType, "[a-zA-Z_][a-zA-Z0-9_]*\(\)")
            tokens.Add(MathTokens.ParamDeli, ",")
            tokens.Add(MathTokens.WhiteSpace, "[ \t]+")
            tokens.Add(MathTokens.[Let], "[Ll][Ee][Tt]")
            tokens.Add(MathTokens.Equals, "=")
            tokens.Add(MathTokens.LPair, "\(")
            tokens.Add(MathTokens.RPair, "\)")
            tokens.Add(MathTokens.Asterisk, "\*")
            tokens.Add(MathTokens.Slash, "\/")
            tokens.Add(MathTokens.Plus, "\+")
            tokens.Add(MathTokens.Minus, "\-")
            tokens.Add(MathTokens.Power, "\^")
            tokens.Add(MathTokens.[Mod], "%")
            tokens.Add(MathTokens.Pretend, "Pretend")
            tokens.Add(MathTokens.[And], "[aA][nN][dD]")
            tokens.Add(MathTokens.[Not], "[nN][oO][tT]")
            tokens.Add(MathTokens.[Or], "[oO][rR]")
            tokens.Add(MathTokens.var, "[a-zA-Z_][a-zA-Z0-9_]*")
            tokens.Add(MathTokens.varRef, "\$[a-zA-Z0-9_]*")
            tokens.Add(MathTokens.constRef, "[&][a-zA-Z0-9_]*")

            Return tokens
        End Function

        ''' <summary>
        ''' 这个字符串表达式是否是科学记数法的数字？
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function IsScientificNotation(s$) As Boolean
            Dim n$ = Regex.Match(s, ScientificNotation, RegexOptions.IgnoreCase).Value
            Dim yes As Boolean = (n = s)
            Return yes
        End Function
    End Module
End Namespace
