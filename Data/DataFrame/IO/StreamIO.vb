﻿#Region "Microsoft.VisualBasic::5fd2a4bbc75b0a2ba0ab7ad33a1067b9, ..\sciBASIC#\Data\DataFrame\IO\StreamIO.vb"

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
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.ComponentModels
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Linq

Namespace IO

    Public Module StreamIO

        ''' <summary>
        ''' 根据文件的头部的定义，从<paramref name="types"/>之中选取得到最合适的类型的定义
        ''' </summary>
        ''' <param name="csv"></param>
        ''' <param name="types"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function [GetType](csv As File, ParamArray types As Type()) As Type
            Dim head As String() = csv.First.ToArray
            Dim scores As New Dictionary(Of Type, Integer)

            For Each schema In types.Select(AddressOf SchemaProvider.CreateObject)
                Dim allNames As String() = schema.Properties.ToArray(Function(x) x.Name)
                Dim matches = (From p As String
                               In allNames
                               Where System.Array.IndexOf(head, p) > -1
                               Select 1).Sum
                Call scores.Add(schema.DeclaringType, matches)
            Next

            Dim desc = From x
                       In scores
                       Select type = x.Key, x.Value
                       Order By Value Descending
            Dim target As Type = desc.FirstOrDefault?.type
            Return target
        End Function

        ''' <summary>
        ''' Save this csv document into a specific file location <paramref name="path"/>.
        ''' </summary>
        ''' <param name="path">
        ''' 假若路径是指向一个已经存在的文件，则原有的文件数据将会被清空覆盖
        ''' </param>
        ''' <remarks>当目标保存路径不存在的时候，会自动创建文件夹</remarks>
        ''' 
        <Extension>
        Public Function SaveDataFrame(csv As IEnumerable(Of RowObject), Optional path$ = "", Optional encoding As Encoding = Nothing) As Boolean
            Dim stopwatch As Stopwatch = Stopwatch.StartNew

            If String.IsNullOrEmpty(path) Then
                Throw New NullReferenceException("path reference to a null location!")
            End If
            If encoding Is Nothing Then
                encoding = Encoding.UTF8
            End If

            Using out As StreamWriter = path.OpenWriter(encoding)
                For Each line$ In csv.Select(Function(r) r.AsLine)
                    Call out.WriteLine(line)
                Next
            End Using

            Call Console.WriteLine($"Generate csv file document using time {stopwatch.ElapsedMilliseconds} ms.")

            Return True
        End Function
    End Module
End Namespace
