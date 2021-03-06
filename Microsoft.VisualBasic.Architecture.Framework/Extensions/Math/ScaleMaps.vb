﻿#Region "Microsoft.VisualBasic::e39c71cd872e64e9f3eb30861ce7c53e, ..\sciBASIC#\Microsoft.VisualBasic.Architecture.Framework\Extensions\Math\ScaleMaps.vb"

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
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Mathematical

    <PackageNamespace("ScaleMaps",
                  Category:=APICategories.UtilityTools,
                  Publisher:="xie.guigang@live.com")>
    Public Module ScaleMaps

        ''' <summary>
        ''' Trims the data ranges, 
        ''' if n in <paramref name="Dbl"/> vector is less than <paramref name="min"/>, then set n = min;
        ''' else if n is greater than <paramref name="max"/>, then set n value to max, 
        ''' else do nothing.
        ''' </summary>
        ''' <param name="Dbl"></param>
        ''' <param name="min"></param>
        ''' <param name="max"></param>
        ''' <returns></returns>
        <Extension> Public Function TrimRanges(Dbl As Double(), min As Double, max As Double) As Double()
            If Dbl.IsNullOrEmpty Then
                Return New Double() {}
            End If

            For i As Integer = 0 To Dbl.Length - 1
                Dim n As Double = Dbl(i)

                If n < min Then
                    n = min
                ElseIf n > max Then
                    n = max
                End If

                Dbl(i) = n
            Next

            Return Dbl
        End Function

        <ExportAPI("Ranks.Mapping")>
        <Extension> Public Function GenerateMapping(Of T As INamedValue)(
                                                    data As IEnumerable(Of T),
                                               getSample As Func(Of T, Double),
                                          Optional Level As Integer = 10) As Dictionary(Of String, Integer)

            Dim samples As Double() = data.ToArray(Function(x) getSample(x))
            Dim levels As Integer() = samples.GenerateMapping(Level)
            Dim hash = data.ToArray(Function(x, i) New KeyValuePair(Of String, Integer)(x.Key, levels(i)))
            Return hash.ToDictionary
        End Function

        ''' <summary>
        ''' Linear mappings the vector elements in to another scale within specifc range from parameter <paramref name="Level"></paramref>.
        ''' (如果每一个数值之间都是相同的大小，则返回原始数据，因为最大值与最小值的差为0，无法进行映射的创建（会出现除0的错误）)
        ''' </summary>
        ''' <param name="data">Your input numeric vector.</param>
        ''' <param name="Level">The scaler range.</param>
        ''' <returns></returns>
        ''' <remarks>为了要保持顺序，不能够使用并行拓展</remarks>
        ''' <param name="offset">
        ''' The default scaler range output is [1, <paramref name="Level"></paramref>], but you can modify this parameter 
        ''' value for moving the range to [<paramref name="offset"></paramref>, <paramref name="Level"></paramref> + <paramref name="offset"></paramref>].
        ''' (默认是 [1, <paramref name="Level"></paramref>]，
        ''' 当offset的值为0的时候，则为[0, <paramref name="Level"></paramref>-1]，
        ''' 当然这个参数也可以使其他的值)
        ''' </param>
        <ExportAPI("Ranks.Mapping")>
        <Extension> Public Function GenerateMapping(data As IEnumerable(Of Double), Optional Level As Integer = 10, Optional offset As Integer = 1) As Integer()
            Dim array As Double() = data.ToArray

            If array.Length = 0 Then
                Return {}
            End If

            Dim MinValue As Double = array.Min
            Dim MaxValue As Double = array.Max
            Dim d As Double = MaxValue - MinValue

            If d = 0R Then ' 所有的值都是一样的，则都是同等级的
                Return 1.CopyVector(array.Length)
            End If

            Dim chunkBuf As Integer() = New Integer(array.Length - 1) {}
            Dim i As int = 0

            For Each x As Double In array
                Dim lv As Integer = Fix(Level * (x - MinValue) / d)
                chunkBuf(++i) = lv + offset
            Next

            Return chunkBuf
        End Function

        <Extension>
        Public Function LogLevels(data As IEnumerable(Of Double), base%, Optional level As Integer = 100) As Integer()
            Dim logvalues = data.ToArray(Function(x) Math.Log(x, base))
            Return logvalues.GenerateMapping(level)
        End Function

        <ExportAPI("Ranks.Log2")>
        <Extension> Public Function Log2Ranks(data As IEnumerable(Of Double), Optional Level As Integer = 100) As Integer()
            Dim log2Value = data.ToArray(Function(x) Math.Log(x, 2))
            Return log2Value.GenerateMapping(Level)
        End Function

        <ExportAPI("Ranks.Log2")>
        <Extension> Public Function Log2Ranks(data As IEnumerable(Of Integer), Optional Level As Integer = 10) As Integer()
            Return data.Select(Function(d) CDbl(d)).Log2Ranks
        End Function

        <ExportAPI("Ranks.Log2")>
        <Extension> Public Function Log2Ranks(data As IEnumerable(Of Long), Optional Level As Integer = 10) As Integer()
            Return data.Select(Function(d) CDbl(d)).Log2Ranks
        End Function

        ''' <summary>
        ''' 如果每一个数值之间都是相同的大小，则返回原始数据，因为最大值与最小值的差为0，无法进行映射的创建（会出现除0的错误）
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks>为了要保持顺序，不能够使用并行拓展</remarks>
        ''' 
        <ExportAPI("Ranks.Mapping")>
        <Extension> Public Function GenerateMapping(data As IEnumerable(Of Integer), Optional Level As Integer = 10, Optional offset% = 1) As Integer()
            Return GenerateMapping((From n In data Select CDbl(n)).ToArray, Level, offset)
        End Function

        <ExportAPI("Ranks.Mapping")>
        <Extension> Public Function GenerateMapping(data As IEnumerable(Of Long), Optional Level As Integer = 10) As Integer()
            Return GenerateMapping((From n In data Select CDbl(n)).ToArray, Level)
        End Function

        ''' <summary>
        ''' Function centers and/or scales the columns of a numeric matrix.
        ''' </summary>
        ''' <param name="data">numeric matrix</param>
        ''' <param name="center">either a logical value or a numeric vector of length equal to the number of columns of x</param>
        ''' <param name="isScale">either a logical value or a numeric vector of length equal to the number of columns of x</param>
        ''' <returns></returns>
        <ExportAPI("Scale", Info:="function centers and/or scales the columns of a numeric matrix.")>
        Public Function Scale(<Parameter("x", "numeric matrix")> data As IEnumerable(Of Double),
                              <Parameter("center", "either a logical value or a numeric vector of length equal to the number of columns of x")>
                              Optional center As Boolean = True,
                              <Parameter("scale", "either a logical value or a numeric vector of length equal to the number of columns of x")>
                              Optional isScale As Boolean = True) As Double()

            Dim avg As Double = data.Average
            Dim rms As Double = VBMathExtensions.RMS(data)

            If center Then
                data = (From n In data Select n - avg).ToArray
            End If

            If isScale Then
                data = (From n In data Select n / rms).ToArray
            End If

            Return data.ToArray
        End Function
    End Module
End Namespace
