﻿#Region "Microsoft.VisualBasic::885936b6fbb4d57df56eda9751cb7f64, ..\sciBASIC#\Microsoft.VisualBasic.Architecture.Framework\Tools\Network\Protocol\Streams\VarArray.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Net.Protocols.Streams.Array

    ''' <summary>
    ''' The bytes length of the element in thee source sequence is not fixed.
    ''' (序列里面的元素的长度是不固定的)
    ''' </summary>
    Public Class VarArray(Of T) : Inherits ArrayAbstract(Of T)

        Sub New(TSerialize As Func(Of T, Byte()), load As Func(Of Byte(), T))
            Call MyBase.New(TSerialize, load)
        End Sub

        Sub New(raw As Byte(),
                serilize As Func(Of T, Byte()),
                load As Func(Of Byte(), T))
            Call Me.New(serilize, load)

            Dim lb As Byte() = New Byte(INT64 - 1) {}
            Dim buf As Byte()
            Dim i As New Pointer
            Dim list As New List(Of T)
            Dim l As Long
            Dim x As T

            Do While raw.Length > i

                Call System.Array.ConstrainedCopy(raw, i << INT64, lb, Scan0, INT64)

                l = BitConverter.ToInt64(lb, Scan0)
                buf = New Byte(l - 1) {}

                Call System.Array.ConstrainedCopy(raw, i << buf.Length, buf, Scan0, buf.Length)

                x = load(buf)
                list += x
            Loop

            Values = list.ToArray
        End Sub

        ''' <summary>
        ''' Long + T + Long + T
        ''' 其中Long是一个8字节长度的数组，用来指示T的长度
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Serialize() As Byte()
            Dim list As New List(Of Byte)
            Dim LQuery = (From ind As SeqValue(Of T)
                          In Values.SeqIterator.AsParallel
                          Select ind.i,
                              byts = __serialization(ind.value)
                          Order By i Ascending)

            For Each x In LQuery
                Dim byts As Byte() = x.byts
                Dim l As Long = byts.Length
                Dim lb As Byte() = BitConverter.GetBytes(l)

                Call list.AddRange(lb)
                Call list.AddRange(byts)
            Next

            Return list.ToArray
        End Function
    End Class
End Namespace
